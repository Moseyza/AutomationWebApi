using ServerWrapper.ClientSideServices;

namespace BatisServiceProvider.Services
{
    public class ServiceBase<TSrv>
    {
        public TSrv Service => (new ClientSideTaskedBaseServiceFactory()).Get<TSrv>();
    }
}
