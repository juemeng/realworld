namespace Moniter.Features.Deploy
{
    public interface IRemoteClient
    {
        void Deploy(DeployInfo info);
        void UpdateConfig(DeployInfo info,ServerConfig newConfig);
    }
}