using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wam.Project.Startup))]
namespace Wam.Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
