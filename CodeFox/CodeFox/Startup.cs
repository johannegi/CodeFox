using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodeFox.Startup))]
namespace CodeFox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
