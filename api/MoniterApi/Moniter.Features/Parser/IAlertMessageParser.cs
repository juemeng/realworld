using Moniter.Models;

namespace Moniter.Features.Parser
{
    public interface IAlertMessageParser
    {
        AlertInfo Parse(BufferData buffer);
    }
}