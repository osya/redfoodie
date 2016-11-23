namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageFileNameproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ImageFileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ImageFileName");
        }
    }
}
