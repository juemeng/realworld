using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Moniter.Models;

namespace Moniter.Features.Parser
{
    public class AlertMessageParse : IAlertMessageParser
    {
        public AlertInfo Parse(BufferData buffer)
        {
            var alertData = buffer.Data;
            //80 48 4 132
            //95 17 72 184
            var hexAlertData = alertData.Select(x => x.ToString("X")).ToArray();
            var commandCode = alertData[0];
            var alertMessage = (AlertMessage) commandCode;
            var description = alertMessage
                .GetType()
                .GetMember(alertMessage.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
            var secondNum = hexAlertData[2];
            if (alertData[2] < 10)
            {
                secondNum = "0" + secondNum;
            }
            return new AlertInfo
            {
                AlertTime = DateTime.Now,
                Description = description,
                Message = alertMessage,
                MasterId = buffer.MasterId,
                SlaveNumber = hexAlertData[1]+secondNum
            };
        }
    }
}