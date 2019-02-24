namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "EquipmentName", c => c.String());
            DropColumn("dbo.Inventories", "EquipementName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inventories", "EquipementName", c => c.String());
            DropColumn("dbo.Inventories", "EquipmentName");
        }
    }
}
