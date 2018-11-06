using System.Collections.Generic;
using Moniter.Models;

namespace Moniter.Features.Users
{
    public class User
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }
        
        public UserRole Role { get; set; }

        public string Token { get; set; }
    }

    public class LoginUser : User
    {
        public int NoticeCount { get; set; }
    }


    public class UserEnvelope
    {
        public UserEnvelope(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
    
    public class UsersEnvelope
    {
        public UsersEnvelope(List<User> users)
        {
            Users = users;
        }

        public List<User> Users { get; }
    }
}