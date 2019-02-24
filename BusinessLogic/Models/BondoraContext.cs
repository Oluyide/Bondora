using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLogic.Models
{
    public partial class BondoraContext: DbContext
    {
        public BondoraContext(): base("Name=DefaultConnection")
        {
            Database.SetInitializer<BondoraContext>(new CreateDatabaseIfNotExists<BondoraContext>());

        }

        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<EquipmentsType> EquipmentsTypes { get; set; }
        public virtual DbSet<FeeSetup> FeeSetup { get; set; }
        public virtual DbSet<PaymentReadingType> PaymentReadingType { get; set; }
        public virtual DbSet<CustomerCart> CustomerCart { get; set; }
        


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
