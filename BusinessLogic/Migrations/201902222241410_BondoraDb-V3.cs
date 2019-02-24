namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeeSetups", "Fee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PaymentReadingTypes", "TypeofRent", c => c.String());
            DropColumn("dbo.PaymentReadingTypes", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PaymentReadingTypes", "Type", c => c.String());
            DropColumn("dbo.PaymentReadingTypes", "TypeofRent");
            DropColumn("dbo.FeeSetups", "Fee");
        }
    }
}
