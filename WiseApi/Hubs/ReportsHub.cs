using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiseApi.Hubs
{
    public class ReportsHub : Hub
    {
        public async Task SendMessage(int reportId, string message)
        {
            await Clients.All.SendAsync("ReportStatus", reportId, message);
        }
    }
}
