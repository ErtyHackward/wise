using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiseApi.Hubs
{
    public class ReportsHub : Hub
    {
        public async Task Subscribe(int reportId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"report{reportId}");
        }

        public async Task Unsubscribe(int reportId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"report{reportId}");
        }
    }
}
