using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moniter.Infrastructure.Security;
using Moniter.Models;

namespace Moniter.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MoniterContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
            context.Database.EnsureCreated();
            if (!context.Buildings.Any())
            {
                var building = new Building {Id = Guid.NewGuid(), Description = "一号楼", Name = "一号楼"};
                var floor = new Floor
                {
                    Id = Guid.NewGuid(),
                    Description = "二层",
                    BuildingId = building.Id,
                    Number = 2
                };

                var room = new Room
                {
                    Id = Guid.NewGuid(),
                    Name = "客房",
                    Number = 1001,
                    FloorId = floor.Id
                };

                context.Buildings.Add(building);
                context.Floors.Add(new Floor
                {
                    Id = Guid.NewGuid(),
                    Description = "一层",
                    BuildingId = building.Id,
                    Number = 1
                });
                context.Floors.Add(floor);
                context.Floors.Add(new Floor
                {
                    Id = Guid.NewGuid(),
                    Description = "三层",
                    BuildingId = building.Id,
                    Number = 3
                });
                context.Rooms.Add(room);
                context.Beds.Add(new Bed
                {
                    Id = Guid.NewGuid(),
                    Number = 1,
                    RoomId = room.Id,
                    SlaveNumber = "3004"
                });
            }

            var admin = context.Users.SingleOrDefault(u => u.Username == "admin");
            if (admin == null)
            {
                var salt = Guid.NewGuid().ToByteArray();
                context.Users.Add(new User
                {
                    Username = "admin",
                    Email = "zouli@hwaztech.com",
                    Role = UserRole.Admin,
                    Hash = passwordHasher.Hash("zouli@123", salt),
                    Salt = salt
                });
            }
            context.SaveChanges();
        }
    }
}