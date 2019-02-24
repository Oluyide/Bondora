using BusinessLogic.Models;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;

[assembly: OwinStartupAttribute(typeof(Bondora2.Startup))]
namespace Bondora2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void equipements()
        {
            BondoraContext context = new BondoraContext();


            var equipemntsTypes = new List<EquipmentsType>()
                {
                    new EquipmentsType { TypeName = "Heavy"},
                    new EquipmentsType { TypeName = "Regular"},
                    new EquipmentsType { TypeName = "Specialized " },    
                };

                foreach (var type in equipemntsTypes)
                {
                    context.EquipmentsTypes.Add(type);
                }
                context.SaveChanges();

               
            }
        }
}
