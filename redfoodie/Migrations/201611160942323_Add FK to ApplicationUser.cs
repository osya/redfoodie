namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKtoApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "City_Id", "dbo.City");
            DropIndex("dbo.AspNetUsers", new[] { "City_Id" });
            RenameColumn(table: "dbo.AspNetUsers", name: "City_Id", newName: "CityId");
            AlterColumn("dbo.AspNetUsers", "CityId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "CityId");
            AddForeignKey("dbo.AspNetUsers", "CityId", "dbo.City", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "CityId", "dbo.City");
            DropIndex("dbo.AspNetUsers", new[] { "CityId" });
            AlterColumn("dbo.AspNetUsers", "CityId", c => c.Int());
            RenameColumn(table: "dbo.AspNetUsers", name: "CityId", newName: "City_Id");
            CreateIndex("dbo.AspNetUsers", "City_Id");
            AddForeignKey("dbo.AspNetUsers", "City_Id", "dbo.City", "Id");
        }
    }
}
