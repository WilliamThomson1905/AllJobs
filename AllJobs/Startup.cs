using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AllJobs.Startup))]
namespace AllJobs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
