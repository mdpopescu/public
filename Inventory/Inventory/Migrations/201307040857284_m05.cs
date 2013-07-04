namespace Renfield.Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m05 : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Stocks", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
            CreateIndex("dbo.Stocks", "ProductId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Stocks", new[] { "ProductId" });
            DropForeignKey("dbo.Stocks", "ProductId", "dbo.Products");
        }
    }
}
