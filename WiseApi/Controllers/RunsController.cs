using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Authorize]
    [Route("api/runs")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private readonly WiseContext _context;

        public RunsController(WiseContext context)
        {
            _context = context;
        }

        // GET: api/ReportRuns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportRun>>> GetRuns()
        {
            return await _context.Runs.ToListAsync();
        }

        // GET: api/ReportRuns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportRun>> GetReportRun(int id)
        {
            var reportRun = await _context.Runs.FindAsync(id);

            if (reportRun == null)
            {
                return NotFound();
            }

            return reportRun;
        }

        // PUT: api/ReportRuns/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportRun(int id, ReportRun reportRun)
        {
            if (id != reportRun.Id)
            {
                return BadRequest();
            }

            _context.Entry(reportRun).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportRunExists(id))
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

        // POST: api/ReportRuns
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReportRun>> PostReportRun(ReportRun reportRun)
        {
            _context.Runs.Add(reportRun);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportRun", new { id = reportRun.Id }, reportRun);
        }

        // DELETE: api/ReportRuns/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportRun>> DeleteReportRun(int id)
        {
            var reportRun = await _context.Runs.FindAsync(id);
            if (reportRun == null)
            {
                return NotFound();
            }

            _context.Runs.Remove(reportRun);
            await _context.SaveChangesAsync();

            return reportRun;
        }

        private bool ReportRunExists(int id)
        {
            return _context.Runs.Any(e => e.Id == id);
        }
    }
}
