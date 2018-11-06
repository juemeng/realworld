using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Moniter.Api.Hub
{
    public class DeviceHub : Microsoft.AspNetCore.SignalR.Hub
    {
//        public async Task SendMessage(string user, string message)
//        {
//            await Clients.All.SendAsync("ReceiveMessage", user, message);
//        }
        
        
        public bool SendMessage(string data)
        {
            
            var length = data.Length;
            return true;
        }


        
    }
    
    
}