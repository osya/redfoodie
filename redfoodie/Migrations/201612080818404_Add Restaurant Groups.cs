namespace redfoodie.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddRestaurantGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RestaurantGroup",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RestaurantGroupRestaurant",
                c => new
                    {
                        RestaurantGroup_Id = c.String(nullable: false, maxLength: 128),
                        Restaurant_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RestaurantGroup_Id, t.Restaurant_Id })
                .ForeignKey("dbo.RestaurantGroup", t => t.RestaurantGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.Restaurant", t => t.Restaurant_Id, cascadeDelete: true)
                .Index(t => t.RestaurantGroup_Id)
                .Index(t => t.Restaurant_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RestaurantGroupRestaurant", "Restaurant_Id", "dbo.Restaurant");
            DropForeignKey("dbo.RestaurantGroupRestaurant", "RestaurantGroup_Id", "dbo.RestaurantGroup");
            DropIndex("dbo.RestaurantGroupRestaurant", new[] { "Restaurant_Id" });
            DropIndex("dbo.RestaurantGroupRestaurant", new[] { "RestaurantGroup_Id" });
            DropTable("dbo.RestaurantGroupRestaurant");
            DropTable("dbo.RestaurantGroup");
        }
    }
}
