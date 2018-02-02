using Isa2017Cinema.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Isa2017Cinema.Startup))]
namespace Isa2017Cinema
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            ApplicationDbContext AppContext = ApplicationDbContext.Create();
            Initializer Initializer = new Initializer();
            Initializer.InitializeDatabase(AppContext);
            AppContext.setRoles();
        }
    }
}
