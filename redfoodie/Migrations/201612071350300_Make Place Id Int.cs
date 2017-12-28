namespace redfoodie.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakePlaceIdInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place");
            DropIndex("dbo.Restaurant", new[] { "PlaceId" });
            DropPrimaryKey("dbo.Place");
            DropColumn("dbo.Place", "Id");
            AddColumn("dbo.Place", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Restaurant", "PlaceId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Place", "Id");
            CreateIndex("dbo.Restaurant", "PlaceId");
            AddForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place");
            DropIndex("dbo.Restaurant", new[] { "PlaceId" });
            DropPrimaryKey("dbo.Place");
            AlterColumn("dbo.Restaurant", "PlaceId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Place", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Place", "Id");
            CreateIndex("dbo.Restaurant", "PlaceId");
            AddForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place", "Id");
        }
    }
}
