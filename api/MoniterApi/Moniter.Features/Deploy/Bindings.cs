using System;
using Moniter.Models;

namespace Moniter.Features.Deploy
{
    public class Bindings
    {
        public Guid FloorId { get; set; }
        public Guid? MasterId { get; set; }
        public string FloorInfo { get; set; }
        public BindingStatus Status { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; } = 22;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    
    public static class BindingsConverter
    {
        public static Bindings ToBindingsViewModel(this Models.Floor floor)
        {
            return new Bindings
            {
                FloorId = floor.Id,
                MasterId = floor.MasterId,
                FloorInfo = $"{floor.Building.Name}-{floor.Number}-{floor.Description}",
                Status = floor.MasterId.HasValue?BindingStatus.Bound:BindingStatus.None,
                Host = floor.Binding?.Host,
                Port = floor.Binding?.Port,
                UserName = floor.Binding?.UserName,
                Password = floor.Binding?.Password
            };
        }
    }
}