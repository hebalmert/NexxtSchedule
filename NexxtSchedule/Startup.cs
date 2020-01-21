using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NexxtSchedule.Startup))]
namespace NexxtSchedule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
