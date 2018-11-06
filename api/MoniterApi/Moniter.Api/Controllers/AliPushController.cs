using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moniter.Features.PushNotification;

namespace Moniter.Api.Controllers
{
    [Route("push")]
    public class AliPushController : Controller
    {
        private readonly IAliyunPushClient _pushClient;

        public AliPushController(IAliyunPushClient pushClient)
        {
            _pushClient = pushClient;
        }

        [HttpGet]
        public void PushMessage()
        {
            _pushClient.PushMessage(new List<string>(), "", "");
        }
    }
}