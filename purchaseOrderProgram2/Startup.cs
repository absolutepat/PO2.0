using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(purchaseOrderProgram2.Startup))]
namespace purchaseOrderProgram2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
