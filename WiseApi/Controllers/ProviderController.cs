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
    [Route("api/providers")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly WiseContext _context;

        public ProviderController(WiseContext context)
        {
            _context = context;
        }

        // GET: api/providers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataProviderConfiguration>>> GetDataProviderConfigurations()
        {
            return await _context.DataProviderConfigurations.ToListAsync();
        }

        // GET: api/providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DataProviderConfiguration>> GetDataProviderConfiguration(int id)
        {
            var dataProviderConfiguration = await _context.DataProviderConfigurations.FindAsync(id);

            if (dataProviderConfiguration == null)
            {
                return NotFound();
            }

            return dataProviderConfiguration;
        }

        // PUT: api/providers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataProviderConfiguration(int id, DataProviderConfiguration dataProviderConfiguration)
        {
            if (id != dataProviderConfiguration.DataProviderConfigurationId)
            {
                return BadRequest();
            }

            _context.Entry(dataProviderConfiguration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataProviderConfigurationExists(id))
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

        // POST: api/providers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DataProviderConfiguration>> PostDataProviderConfiguration(DataProviderConfiguration dataProviderConfiguration)
        {
            _context.DataProviderConfigurations.Add(dataProviderConfiguration);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDataProviderConfiguration", new { id = dataProviderConfiguration.DataProviderConfigurationId }, dataProviderConfiguration);
        }

        // DELETE: api/providers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DataProviderConfiguration>> DeleteDataProviderConfiguration(int id)
        {
            var dataProviderConfiguration = await _context.DataProviderConfigurations.FindAsync(id);
            if (dataProviderConfiguration == null)
            {
                return NotFound();
            }

            _context.DataProviderConfigurations.Remove(dataProviderConfiguration);
            await _context.SaveChangesAsync();

            return dataProviderConfiguration;
        }

        private bool DataProviderConfigurationExists(int id)
        {
            return _context.DataProviderConfigurations.Any(e => e.DataProviderConfigurationId == id);
        }
    }
}
