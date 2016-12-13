namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaxLengthforImageFilename : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RestaurantGroup", "ImageFileName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.RestaurantGroup", new[] { "ImageFileName" });
        }
    }
}
