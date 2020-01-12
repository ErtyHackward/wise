using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseApi.Hubs;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Authorize]
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly WiseContext _context;
        private readonly IHubContext<ReportsHub> _hubContext;
        private readonly ReportRunnerService _service;

        public ReportController(WiseContext context, IHubContext<ReportsHub> hub, ReportRunnerService service)
        {
            _context = context;
            _hubContext = hub;
            _service = service;
        }

        // GET: api/reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportConfiguration>>> GetReportConfigurations()
        {
            return await _context.Reports.ToListAsync();
        }

        // POST: api/reports/5/begin
        [HttpPost("{id}/begin")]
        public async Task<ReportRun> BeginQuery([FromBody] ReportRun run)
        {

            run.Report = await _context.Reports.FindAsync(run.Report.Id);

            _context.Runs.Add(run);
            await _context.SaveChangesAsync();

            _service.RunAsync(run);

            return run; //CreatedAtAction("GetReportRuns", new { id = run.Report.Id }, run);
        }

        // POST: api/reports/test
        [HttpPost(), Route("test")]
        public async Task<ActionResult<ReportResponse>> TestReport([FromBody] ReportRun run)
        {
            var config = run.Report;
            var reportParameters = run.CustomParameterValues;
            
            var providerInfo = await _context.Providers.FindAsync(config.DataProvider.Id);
            var resp = new ReportResponse();

            resp.FinalQuery = config.Query;

            if (config.CustomParameters != null)
            {
                foreach (var par in config.CustomParameters)
                {
                    object value = reportParameters?.Find(p => p.Id == par.Id).Value;

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
                        
                        resp.ReportId = 0;
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

                                if (count < 10)
                                {
                                    var items = new List<object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        items.Add(reader.GetValue(i));
                                    }
                                    result.Add(items);
                                }
                            }
                        } while (reader.NextResult());
                        resp.RowsCount = count;
                    }

                    timer.Stop();
                    resp.GenerationTimeMs = (int)timer.ElapsedMilliseconds;
                }
            }
            catch (Exception x)
            {
                resp.ErrorText = x.Message;
            }

            return resp;
        }

        // GET: api/reports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportConfiguration>> GetReportConfiguration(int id)
        {
            var reportConfiguration = await _context.Reports.Include(r => r.DataProvider).Include(r => r.CustomParameters).FirstOrDefaultAsync(r => r.Id == id);

            if (reportConfiguration == null)
            {
                return NotFound();
            }

            return reportConfiguration;
        }

        // GET: api/reports/5/runs
        [HttpGet("{id}/runs")]
        public async Task<ActionResult<Paginated<List<ReportRun>>>> GetReportRuns(int id, int page = 1)
        {
            var paginated = new Paginated<List<ReportRun>>
            {
                ItemsPerPage = 10, 
                Page = page
            };

            paginated.List = await _context.Runs.Where(r => r.Report.Id == id).OrderByDescending(r => r.StartedAt).Skip(paginated.Skip).Take(paginated.ItemsPerPage).ToListAsync();

            if (paginated.List == null)
            {
                return NotFound();
            }

            if (paginated.Page == 1 && paginated.List.Count < paginated.ItemsPerPage)
                paginated.TotalPages = 1;
            else
                paginated.TotalPages = await _context.Runs.Where(r => r.Report.Id == id).CountAsync();
            
            return paginated;
        }

        // PUT: api/reports/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportConfiguration(int id, ReportConfiguration reportConfiguration)
        {
            if (id != reportConfiguration.Id)
            {
                return BadRequest();
            }

            var repCurrent = await _context.Reports.AsNoTracking().Include(r => r.CustomParameters).FirstOrDefaultAsync(r => r.Id == id);

            _context.Entry(reportConfiguration).State = EntityState.Modified;

            if (reportConfiguration.CustomParameters != null)
            {
                foreach (var par in reportConfiguration.CustomParameters)
                {
                    if (par.Id != 0)
                        _context.Entry(par).State = EntityState.Modified;
                    else
                        _context.Entry(par).State = EntityState.Added;
                }
            }           

            if (repCurrent.CustomParameters != null)
            {
                if (reportConfiguration.CustomParameters == null)
                {
                    // delete all
                    foreach (var par in repCurrent.CustomParameters)
                    {
                        _context.Entry(par).State = EntityState.Deleted;
                    }
                }
                else
                {
                    // delete deleted
                    foreach (var par in repCurrent.CustomParameters.Where(p => !reportConfiguration.CustomParameters.Any(p1 => p1.Id == p.Id)))
                    {
                        _context.Entry(par).State = EntityState.Deleted;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReportsListChanged");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportConfigurationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/reports
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReportConfiguration>> PostReportConfiguration(ReportConfiguration reportConfiguration)
        {

            reportConfiguration.DataProvider = await _context.Providers.FindAsync(reportConfiguration.DataProvider.Id);

            _context.Reports.Add(reportConfiguration);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReportsListChanged");

            return CreatedAtAction("GetReportConfiguration", new { id = reportConfiguration.Id }, reportConfiguration);
        }

        // DELETE: api/reports/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportConfiguration>> DeleteReportConfiguration(int id)
        {
            var reportConfiguration = await _context.Reports.FindAsync(id);
            if (reportConfiguration == null)
            {
                return NotFound();
            }

            _context.Reports.Remove(reportConfiguration);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReportsListChanged");

            return reportConfiguration;
        }

        private bool ReportConfigurationExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
