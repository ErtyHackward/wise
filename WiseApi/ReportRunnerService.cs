using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using WiseApi.Hubs;
using WiseDomain;

namespace WiseApi
{
    public class ReportRunnerService 
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IServiceScopeFactory _scopeFactory;

        public ReportRunnerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private void UpdateStatus(int reportId, int runId, ReportRunStatus status, string errorText = null)
        {
            using var scope = _scopeFactory.CreateScope();
            
            var run = new ReportRun
            {
                Id = runId,
                Status = status,
                ErrorText = errorText,
                FinishedAt = DateTime.Now
            };

            var context = scope.ServiceProvider.GetService<WiseContext>();

            context.Runs.Attach(run);
            context.Entry(run).Property(x => x.Status).IsModified = true;

            if (errorText != null)
                context.Entry(run).Property(x => x.ErrorText).IsModified = true;

            if (status == ReportRunStatus.Done || status == ReportRunStatus.Failed)
                context.Entry(run).Property(x => x.FinishedAt).IsModified = true;

            context.SaveChanges();

            var hub = scope.ServiceProvider.GetService<IHubContext<ReportsHub>>();
            hub.Clients.Group($"report{reportId}").SendAsync("StatusChanged", runId, status);
            Logger.Info($"Report status changed {runId} {status}");
        }

        public void RunAsync(ReportRun run)
        {
            if (!ThreadPool.QueueUserWorkItem(Run, run, false))
                throw new ApplicationException("Failed to queue run task");
        }

        private async void Run(ReportRun run)
        {
            Logger.Info($"Starting report generation {run.Id}");
            var config = run.Report;
            var reportParameters = run.CustomParameterValues;

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<WiseContext>();
            var env = scope.ServiceProvider.GetService<IWebHostEnvironment>();

            config = await context.Reports.Include(r => r.DataProvider).Include(r => r.CustomParameters).Where(r => r.Id == config.Id).FirstOrDefaultAsync();

            var providerInfo = await context.Providers.FindAsync(config.DataProvider.Id);
            var resp = new ReportResponse
            {
                FinalQuery = config.Query
            };


            if (config.CustomParameters != null)
            {
                foreach (var par in config.CustomParameters)
                {
                    var value = reportParameters?.Find(p => p.Id == par.Id).Value;

                    switch (par.Type)
                    {
                        case ReportCustomParameterType.Check:
                            resp.FinalQuery = resp.FinalQuery.Replace(par.QueryId, value is true ? par.QueryValue : string.Empty, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        default:
                            resp.FinalQuery = resp.FinalQuery.Replace(par.QueryId, value == null ? string.Empty : par.QueryValue, StringComparison.InvariantCultureIgnoreCase);
                            break;
                    }
                }
            }

            if (resp.FinalQuery.Contains("$timeFilter"))
            {
                resp.FinalQuery = resp.FinalQuery.Replace("$timeFilter", $"BETWEEN '{run.QueryTimeFrom:s}' AND '{run.QueryTimeTo:s}'");
            }

            try
            {
                UpdateStatus( config.Id, run.Id, ReportRunStatus.Querying);
                Logger.Info($"Query is {resp.FinalQuery}");
                var connection = Activator.CreateInstance(Type.GetType(providerInfo.DataProviderType)) as IDbConnection;

                connection.ConnectionString = providerInfo.ConnectionString;
                connection.Open();
                

                var timer = Stopwatch.StartNew();

                using (connection)
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = resp.FinalQuery;

                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<List<object>>();

                        resp.ReportId = config.Id;
                        resp.PreviewValues = result;
                        resp.Columns = new List<string>();

                        var count = 0;

                        do
                        {
                            while (reader.Read())
                            {
                                count++;
                                if (resp.Columns.Count == 0)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        resp.Columns.Add(reader.GetName(i));
                                    }
                                }
                                
                                var items = new List<object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    items.Add(reader.GetValue(i));
                                }
                                result.Add(items);
                            }
                        } while (reader.NextResult());
                        resp.RowsCount = count;
                        resp.ColumnsCount = resp.Columns.Count;
                    }

                    if (string.IsNullOrEmpty(env.WebRootPath))
                        env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    var dirPath = Path.Combine(env.WebRootPath, "reportfiles", config.Id.ToString(), run.Id.ToString());

                    Logger.Info($"Report directory is {dirPath}");

                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    using (var file = File.CreateText(Path.Combine(dirPath, "report.json")))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(file, resp);
                    }

                    using (var excel = new ExcelPackage())
                    {

                        excel.Workbook.Properties.Title = config.Title;
                        //excel.Workbook.Properties.Author
                        excel.Workbook.Properties.Comments = resp.FinalQuery;

                        var ws = excel.Workbook.Worksheets.Add(DateTime.Now.ToString("yyyy-MM-dd"));
                        for (int i = 0; i < resp.ColumnsCount; i++)
                        {
                            ws.Cells[1, i + 1].Value = resp.Columns[i];
                        }

                        for (int rIndex = 0; rIndex < resp.RowsCount; rIndex++)
                        {
                            for (int cIndex = 0; cIndex < resp.ColumnsCount; cIndex++)
                            {
                                ws.Cells[rIndex + 2, cIndex + 1].Value = resp.PreviewValues[rIndex][cIndex];
                            }
                        }

                        using (var fs = File.OpenWrite(Path.Combine(dirPath, "report.xlsx")))
                        {
                            fs.SetLength(0);
                            excel.SaveAs(fs);
                        }
                    }

                    timer.Stop();
                    resp.GenerationTimeMs = (int)timer.ElapsedMilliseconds;
                    UpdateStatus(config.Id, run.Id, ReportRunStatus.Done);
                }
            }
            catch (Exception x)
            {
                UpdateStatus(config.Id, run.Id, ReportRunStatus.Failed, x.Message);
                resp.ErrorText = x.Message;
                Logger.Error(x, $"Report execution failed");
            }
        }
    }
}
