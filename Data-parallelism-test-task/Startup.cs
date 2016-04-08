using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ConcurrencyTest.Startup))]
namespace ConcurrencyTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
