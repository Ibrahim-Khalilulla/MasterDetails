namespace MasterDetails.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aaaa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Item",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Image = c.Binary(storeType: "image"),
                        EntryDate = c.DateTime(nullable: false, storeType: "date"),
                        Quantity = c.Long(nullable: false),
                        CategoryID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Category", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "CategoryID", "dbo.Category");
            DropIndex("dbo.Item", new[] { "CategoryID" });
            DropTable("dbo.Item");
            DropTable("dbo.Category");
        }
    }
}
