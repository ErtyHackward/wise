using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiseApi;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private readonly WiseContext _context;

        public UserGroupsController(WiseContext context)
        {
            _context = context;
        }

        // GET: api/UserGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroup>>> GetGroups()
        {
            return await _context.Groups.ToListAsync();
        }

        // GET: api/UserGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroup>> GetUserGroup(int id)
        {
            var userGroup = await _context.Groups.FindAsync(id);

            if (userGroup == null)
            {
                return NotFound();
            }

            return userGroup;
        }

        // PUT: api/UserGroups/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserGroup(int id, UserGroup userGroup)
        {
            if (id != userGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(userGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserGroupExists(id))
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

        // POST: api/UserGroups
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<UserGroup>> PostUserGroup(UserGroup userGroup)
        {
            _context.Groups.Add(userGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserGroup", new { id = userGroup.Id }, userGroup);
        }

        // DELETE: api/UserGroups/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserGroup>> DeleteUserGroup(int id)
        {
            var userGroup = await _context.Groups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return userGroup;
        }

        private bool UserGroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
