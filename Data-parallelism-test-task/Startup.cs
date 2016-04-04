using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DataParallelismTest.Startup))]
namespace DataParallelismTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
