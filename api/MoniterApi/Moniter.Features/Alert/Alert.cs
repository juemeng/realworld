using System;
using Moniter.Infrastructure;
using Moniter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moniter.Features.Alert
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Alert
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlertStatus Status { get; set; }
        public DateTime AlertTime { get; set; }
        public string RespondUserName { get; set; }
    }
    
    public static class AlertConverter
    {
        public static Alert ToViewModel(this Models.Alert alert)
        {
            return new Alert
            {
                Id = alert.Id,
                Title = alert.Description,
                Description = $"{alert.FloorNumber}层-{alert.RoomName}-{alert.BedNumber}床 分机号:{alert.SlaveNumber}",
                Status = alert.Status,
                AlertTime = alert.AlertTime,
                RespondUserName = string.IsNullOrEmpty(alert.RespondUserName)?@"无人处理":alert.RespondUserName
            };
        }
    }
}