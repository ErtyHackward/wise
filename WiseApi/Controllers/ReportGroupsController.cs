using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportGroupsController : ControllerBase
    {
        private readonly WiseContext _context;

        public ReportGroupsController(WiseContext context)
        {
            _context = context;
        }

        // GET: api/ReportGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportGroup>>> GetReportGroup()
        {
            if (HttpContext.User.IsInRole("admin"))
                return await _context.ReportGroup.ToListAsync();

            var id = HttpContext.User.FindFirstValue(JwtClaimTypes.Subject);

            return await _context.ReportGroup.Where(g => g.AccessMode == AccessMode.Everybody || g.AllowedUserGroups.Any(j => j.Group.UserGroups.Any(uj => uj.User.Login == id))).ToListAsync();
        }

        // GET: api/ReportGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportGroup>> GetReportGroup(int id)
        {
            var reportGroup = await _context.ReportGroup.FindAsync(id);

            if (reportGroup == null)
            {
                return NotFound();
            }

            return reportGroup;
        }

        // PUT: api/ReportGroups/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportGroup(int id, ReportGroup reportGroup)
        {
            if (id != reportGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(reportGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportGroupExists(id))
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

        // POST: api/ReportGroups
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReportGroup>> PostReportGroup(ReportGroup reportGroup)
        {
            _context.ReportGroup.Add(reportGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportGroup", new { id = reportGroup.Id }, reportGroup);
        }

        // DELETE: api/ReportGroups/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportGroup>> DeleteReportGroup(int id)
        {
            var reportGroup = await _context.ReportGroup.FindAsync(id);
            if (reportGroup == null)
            {
                return NotFound();
            }

            _context.ReportGroup.Remove(reportGroup);
            await _context.SaveChangesAsync();

            return reportGroup;
        }

        private bool ReportGroupExists(int id)
        {
            return _context.ReportGroup.Any(e => e.Id == id);
        }
    }
}
