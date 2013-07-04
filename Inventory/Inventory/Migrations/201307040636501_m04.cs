namespace Renfield.Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "Version", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "Version");
        }
    }
}
