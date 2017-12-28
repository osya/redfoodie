namespace redfoodie.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddImageFileName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RestaurantGroup", "ImageFileName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Restaurant", "ImageFileName", c => c.String(maxLength: 255));
            CreateIndex("dbo.Restaurant", "ImageFileName", true);
            CreateIndex("dbo.RestaurantGroup", "ImageFileName", true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.RestaurantGroup", new[] { "ImageFileName" });
            DropIndex("dbo.Restaurant", new[] { "ImageFileName" });
            AlterColumn("dbo.Restaurant", "ImageFileName", c => c.String());
            DropColumn("dbo.RestaurantGroup", "ImageFileName");
        }
    }
}
