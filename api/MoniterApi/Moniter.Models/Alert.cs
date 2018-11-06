using System;

namespace Moniter.Models
{
    public class Alert:Entity
    {
        public AlertMessage Message { get; set; }
        public string Description { get; set; }
        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; }
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorDescription { get; set; }
        public Guid MasterId { get; set; }
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomName { get; set; }
        public int BedNumber { get; set; }
        public string SlaveNumber { get; set; }
        public DateTime AlertTime { get; set; }
        public AlertStatus Status { get; set; }
        public Guid? RespondUserId { get; set; }
        public string RespondUserName { get; set; }
    }
}