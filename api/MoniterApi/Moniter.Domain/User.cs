using System;
using Newtonsoft.Json;

namespace Moniter.Domain
{
    public class User
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        [JsonIgnore]
        public byte[] Hash { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }
    }
}