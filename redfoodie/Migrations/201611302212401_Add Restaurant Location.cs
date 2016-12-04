namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class AddRestaurantLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "Location", c => c.Geography());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurant", "Location");
        }
    }
}
