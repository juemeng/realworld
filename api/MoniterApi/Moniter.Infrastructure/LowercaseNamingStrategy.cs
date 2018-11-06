using Newtonsoft.Json.Serialization;

namespace Moniter.Infrastructure
{
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name.ToLowerInvariant();
        }
    }
}