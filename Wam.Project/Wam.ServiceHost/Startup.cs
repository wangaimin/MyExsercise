using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wam.ServiceHost.Startup))]
namespace Wam.ServiceHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
