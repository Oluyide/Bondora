namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EquipementName = c.String(),
                        EquipmentsType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentsTypes", t => t.EquipmentsType_Id)
                .Index(t => t.EquipmentsType_Id);
            
            CreateTable(
                "dbo.EquipmentsTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "EquipmentsType_Id", "dbo.EquipmentsTypes");
            DropIndex("dbo.Inventories", new[] { "EquipmentsType_Id" });
            DropTable("dbo.EquipmentsTypes");
            DropTable("dbo.Inventories");
        }
    }
}
