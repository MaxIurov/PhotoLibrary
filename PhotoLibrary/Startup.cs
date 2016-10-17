using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotoLibrary.Startup))]
namespace PhotoLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
