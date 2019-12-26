using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WiseDomain;

namespace WiseApi.Controllers
{
    [Route("api/report/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly WiseContext _context;

        public TestController(WiseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ReportResponse>> TestReport([FromBody] ReportConfiguration config)
        {

            return new ReportResponse { ReportTitle = $"Test query: {config.Query}" };
        }
    }
}