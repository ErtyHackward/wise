﻿using System;
using System.Collections.Generic;
using System.Data;
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
            return await _context.Providers.ToListAsync();
        }

        // POST: api/providers/test
        [HttpPost(), Route("test")]
        public async Task<ActionResult<ReportResponse>> TestProvider([FromBody] DataProviderConfiguration config)
        {
            var resp = new ReportResponse();

            try
            {
                var connection = Activator.CreateInstance(Type.GetType(config.DataProviderType)) as IDbConnection;
                connection.ConnectionString = config.ConnectionString;
                connection.Open();

                using (connection)
                {
                    if (connection.State == ConnectionState.Open)
                        resp.ReportTitle = "Соединение установлено успешно";
                }
            }
            catch (Exception x)
            {
                resp.ErrorText = x.Message;
            }

            return resp;
        }

        // GET: api/providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DataProviderConfiguration>> GetDataProviderConfiguration(int id)
        {
            var dataProviderConfiguration = await _context.Providers.FindAsync(id);

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
            if (id != dataProviderConfiguration.Id)
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
            _context.Providers.Add(dataProviderConfiguration);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDataProviderConfiguration", new { id = dataProviderConfiguration.Id }, dataProviderConfiguration);
        }

        // DELETE: api/providers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DataProviderConfiguration>> DeleteDataProviderConfiguration(int id)
        {
            var dataProviderConfiguration = await _context.Providers.FindAsync(id);
            if (dataProviderConfiguration == null)
            {
                return NotFound();
            }

            _context.Providers.Remove(dataProviderConfiguration);
            await _context.SaveChangesAsync();

            return dataProviderConfiguration;
        }

        private bool DataProviderConfigurationExists(int id)
        {
            return _context.Providers.Any(e => e.Id == id);
        }
    }
}
