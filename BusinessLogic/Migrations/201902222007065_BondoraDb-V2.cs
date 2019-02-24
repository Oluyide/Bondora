namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerCarts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        RentalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsCheckedOut = c.Boolean(nullable: false),
                        InventoryItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inventories", t => t.InventoryItem_Id)
                .Index(t => t.InventoryItem_Id);
            
            CreateTable(
                "dbo.FeeSetups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FeeTypeName = c.String(),
                        TimeofRent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentReadingTypes", t => t.TimeofRent_Id)
                .Index(t => t.TimeofRent_Id);
            
            CreateTable(
                "dbo.PaymentReadingTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.EquipmentsTypes", "LoyaltyPoint", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeeSetups", "TimeofRent_Id", "dbo.PaymentReadingTypes");
            DropForeignKey("dbo.CustomerCarts", "InventoryItem_Id", "dbo.Inventories");
            DropIndex("dbo.FeeSetups", new[] { "TimeofRent_Id" });
            DropIndex("dbo.CustomerCarts", new[] { "InventoryItem_Id" });
            DropColumn("dbo.EquipmentsTypes", "LoyaltyPoint");
            DropTable("dbo.PaymentReadingTypes");
            DropTable("dbo.FeeSetups");
            DropTable("dbo.CustomerCarts");
        }
    }
}
