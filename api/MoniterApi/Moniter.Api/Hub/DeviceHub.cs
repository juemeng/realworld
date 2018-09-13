using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Moniter.Api.Hub
{
    public class DeviceHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}