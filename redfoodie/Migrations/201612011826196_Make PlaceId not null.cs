namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakePlaceIdnotnull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place");
            DropIndex("dbo.Restaurant", new[] { "PlaceId" });
            AlterColumn("dbo.Restaurant", "PlaceId", c => c.Int(nullable: false));
            CreateIndex("dbo.Restaurant", "PlaceId");
            AddForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place");
            DropIndex("dbo.Restaurant", new[] { "PlaceId" });
            AlterColumn("dbo.Restaurant", "PlaceId", c => c.Int());
            CreateIndex("dbo.Restaurant", "PlaceId");
            AddForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place", "Id");
        }
    }
}
