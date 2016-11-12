namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropHrefcolumnfromCity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.City", "Href");
        }
        
        public override void Down()
        {
            AddColumn("dbo.City", "Href", c => c.String());
        }
    }
}
