namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHreftoCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.City", "ParentId", c => c.Int());
            AddColumn("dbo.City", "Href", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.City", "Href");
            DropColumn("dbo.City", "ParentId");
        }
    }
}
