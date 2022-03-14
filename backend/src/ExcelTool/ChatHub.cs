using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExcelTool
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            var us = Context.User;
            var us1 = Context.User?.Identity?.Name;
            Console.WriteLine($"有连接请求:{Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"有连接断开:{Context.ConnectionId},原因为{(exception != null ? exception.Message : null)}");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastChartData(List<ChartModel> data)
        {
            Console.WriteLine("有广播消息");
            //Clients.Client("").
            await Clients.All.SendAsync("broadcastchartdata", data);
        }

        public async Task BroadcastToConnection(string data, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("broadcasttoclient", data);
        }

        public async Task BroadcastToUser(string data, string userId)
        {
            await Clients.User(userId).SendAsync("broadcasttouser", data);
        }

        public string GetConnectionId()
        {
            //var us = Context.User;
            return Context.ConnectionId;
        }
    }
}
