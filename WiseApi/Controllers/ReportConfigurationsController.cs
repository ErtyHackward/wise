using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportConfigurationsController : ControllerBase
    {
        private readonly WiseContext _context;

        public ReportConfigurationsController(WiseContext context)
        {
            _context = context;
        }

        // GET: api/reportconfigs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportConfiguration>>> GetReportConfigurations()
        {
            return await _context.ReportConfigurations.ToListAsync().ConfigureAwait(false);
        }

        [HttpPost(), Route("test")]
        public async Task<ActionResult<ReportResponse>> TestReport([FromBody] ReportConfiguration config)
        {
            var providerInfo = await _context.DataProviderConfigurations.FindAsync(config.DataProvider.DataProviderConfigurationId);
            var resp = new ReportResponse();

            try
            {
                var connection = Activator.CreateInstance(Type.GetType(providerInfo.DataProviderType)) as IDbConnection;

                connection.ConnectionString = providerInfo.ConnectionString;
                connection.Open();

                using (connection)
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = config.Query;

                    using (var reader = command.ExecuteReader())
                    {
                        List<List<object>> result = new List<List<object>>();

                        resp.ReportId = 0;
                        resp.RowsCount = 10;
                        resp.PreviewValues = result;
                        resp.Columns = new List<string>();

                        do
                        {
                            while (reader.Read())
                            {
                                if (resp.Columns.Count == 0)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        resp.Columns.Add(reader.GetName(i));
                                    }
                                }

                                List<object> items = new List<object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    items.Add(reader.GetValue(i));
                                }
                                result.Add(items);
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception x)
            {
                resp.ErrorText = x.Message;
            }

            return resp;
        }

        // GET: api/reportconfigs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportConfiguration>> GetReportConfiguration(int id)
        {
            var reportConfiguration = await _context.ReportConfigurations.FindAsync(id);

            if (reportConfiguration == null)
            {
                return NotFound();
            }

            return reportConfiguration;
        }

        // PUT: api/reportconfigs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportConfiguration(int id, ReportConfiguration reportConfiguration)
        {
            if (id != reportConfiguration.ReportConfigurationId)
            {
                return BadRequest();
            }

            _context.Entry(reportConfiguration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/reportconfigs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReportConfiguration>> PostReportConfiguration(ReportConfiguration reportConfiguration)
        {
            _context.ReportConfigurations.Add(reportConfiguration);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportConfiguration", new { id = reportConfiguration.ReportConfigurationId }, reportConfiguration);
        }

        // DELETE: api/reportconfigs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportConfiguration>> DeleteReportConfiguration(int id)
        {
            var reportConfiguration = await _context.ReportConfigurations.FindAsync(id);
            if (reportConfiguration == null)
            {
                return NotFound();
            }

            _context.ReportConfigurations.Remove(reportConfiguration);
            await _context.SaveChangesAsync();

            return reportConfiguration;
        }

        private bool ReportConfigurationExists(int id)
        {
            return _context.ReportConfigurations.Any(e => e.ReportConfigurationId == id);
        }
    }
}
