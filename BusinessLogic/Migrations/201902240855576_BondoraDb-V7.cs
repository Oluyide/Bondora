namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerCarts", "UserId", c => c.String());
            AlterColumn("dbo.CustomerCarts", "RentDays", c => c.Int(nullable: false));
            DropColumn("dbo.CustomerCarts", "RentalPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerCarts", "RentalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerCarts", "RentDays", c => c.String());
            AlterColumn("dbo.CustomerCarts", "UserId", c => c.Guid(nullable: false));
        }
    }
}
