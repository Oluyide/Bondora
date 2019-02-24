namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquipmentsTypes", "RentTerms", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquipmentsTypes", "RentTerms");
        }
    }
}
