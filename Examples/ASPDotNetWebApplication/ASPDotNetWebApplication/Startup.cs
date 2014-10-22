using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASPDotNetWebApplication.Startup))]
namespace ASPDotNetWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
