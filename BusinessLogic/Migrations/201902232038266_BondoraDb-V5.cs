namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BondoraDbV5 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.FeeSetups", name: "TimeofRent_Id", newName: "TypeofRent_Id");
            AddColumn("dbo.CustomerCarts", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerCarts", "CustomerName");
            RenameColumn(table: "dbo.FeeSetups", name: "TypeofRent_Id", newName: "TimeofRent_Id");
        }
    }
}
