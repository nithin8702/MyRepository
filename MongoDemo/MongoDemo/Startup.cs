using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MongoDemo.Startup))]
namespace MongoDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
