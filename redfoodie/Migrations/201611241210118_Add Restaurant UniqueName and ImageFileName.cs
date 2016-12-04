namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRestaurantUniqueNameandImageFileName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "UniqueName", c => c.String(maxLength: 255));
            AddColumn("dbo.Restaurant", "ImageFileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurant", "ImageFileName");
            DropColumn("dbo.Restaurant", "UniqueName");
        }
    }
}
