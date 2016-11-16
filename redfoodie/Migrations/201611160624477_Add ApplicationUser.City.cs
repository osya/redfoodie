namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplicationUserCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "City_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "City_Id");
            AddForeignKey("dbo.AspNetUsers", "City_Id", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "City_Id", "dbo.City");
            DropIndex("dbo.AspNetUsers", new[] { "City_Id" });
            DropColumn("dbo.AspNetUsers", "City_Id");
        }
    }
}
