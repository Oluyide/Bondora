namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerCarts", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CustomerCarts", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CustomerCarts", "RentDays", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerCarts", "RentDays");
            DropColumn("dbo.CustomerCarts", "EndDate");
            DropColumn("dbo.CustomerCarts", "StartDate");
        }
    }
}
