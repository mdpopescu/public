namespace Renfield.Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m00 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        SalePrice = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Acquisitions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.AcquisitionItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AcquisitionId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Acquisitions", t => t.AcquisitionId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.AcquisitionId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.SaleItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SaleId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sales", t => t.SaleId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.SaleId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.SaleItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SaleItems", "SaleId", "dbo.Sales");
            DropForeignKey("dbo.Acquisitions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AcquisitionItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.AcquisitionItems", "AcquisitionId", "dbo.Acquisitions");
            DropIndex("dbo.Sales", new[] { "CompanyId" });
            DropIndex("dbo.SaleItems", new[] { "ProductId" });
            DropIndex("dbo.SaleItems", new[] { "SaleId" });
            DropIndex("dbo.Acquisitions", new[] { "CompanyId" });
            DropIndex("dbo.AcquisitionItems", new[] { "ProductId" });
            DropIndex("dbo.AcquisitionItems", new[] { "AcquisitionId" });
            DropTable("dbo.SaleItems");
            DropTable("dbo.Sales");
            DropTable("dbo.AcquisitionItems");
            DropTable("dbo.Acquisitions");
            DropTable("dbo.Companies");
            DropTable("dbo.Products");
        }
    }
}
