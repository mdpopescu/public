namespace Renfield.Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Acquisitions", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sales", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "Date");
            DropColumn("dbo.Acquisitions", "Date");
        }
    }
}
