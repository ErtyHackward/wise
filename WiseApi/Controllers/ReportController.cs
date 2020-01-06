using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseApi.Hubs;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly WiseContext _context;
        private readonly IHubContext<ReportsHub> _hubContext;

        public ReportController(WiseContext context, IHubContext<ReportsHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }

        // GET: api/reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportConfiguration>>> GetReportConfigurations()
        {
            return await _context.Reports.ToListAsync();
        }

        // GET: api/reports/begin/5
        [HttpPost("begin/{id}")]
        public async Task<IActionResult> BeginQuery()
        {
            return Ok();
        }

        // POST: api/reports/test
        [HttpPost(), Route("test")]
        public async Task<ActionResult<ReportResponse>> TestReport([FromBody] ReportConfiguration config)
        {
            var providerInfo = await _context.Providers.FindAsync(config.DataProvider.Id);
            var resp = new ReportResponse();

            try
            {
                var connection = Activator.CreateInstance(Type.GetType(providerInfo.DataProviderType)) as IDbConnection;

                connection.ConnectionString = providerInfo.ConnectionString;
                connection.Open();

                var timer = Stopwatch.StartNew();

                using (connection)
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = config.Query;

                    using (var reader = command.ExecuteReader())
                    {
                        List<List<object>> result = new List<List<object>>();
                        
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
                                    List<object> items = new List<object>();
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
            var reportConfiguration = await _context.Reports.Include(r => r.DataProvider).FirstOrDefaultAsync(r => r.Id == id);

            if (reportConfiguration == null)
            {
                return NotFound();
            }

            return reportConfiguration;
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

            _context.Entry(reportConfiguration).State = EntityState.Modified;

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
