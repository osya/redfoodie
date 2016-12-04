namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlaces : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Restaurant", "CityId", "dbo.City");
            DropIndex("dbo.Restaurant", new[] { "CityId" });
            CreateTable(
                "dbo.Place",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.City", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            AddColumn("dbo.Restaurant", "PlaceId", c => c.Int());
            CreateIndex("dbo.Restaurant", "PlaceId");
            AddForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place", "Id");
            DropColumn("dbo.City", "ParentId");
            DropColumn("dbo.Restaurant", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurant", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.City", "ParentId", c => c.Int());
            DropForeignKey("dbo.Restaurant", "PlaceId", "dbo.Place");
            DropForeignKey("dbo.Place", "CityId", "dbo.City");
            DropIndex("dbo.Restaurant", new[] { "PlaceId" });
            DropIndex("dbo.Place", new[] { "CityId" });
            DropColumn("dbo.Restaurant", "PlaceId");
            DropTable("dbo.Place");
            CreateIndex("dbo.Restaurant", "CityId");
            AddForeignKey("dbo.Restaurant", "CityId", "dbo.City", "Id", cascadeDelete: true);
        }
    }
}
