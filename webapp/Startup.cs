using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eSPP.Startup))]
namespace eSPP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
