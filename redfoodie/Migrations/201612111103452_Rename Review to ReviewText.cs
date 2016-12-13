namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameReviewtoReviewText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vote", "ReviewText", c => c.String());
            DropColumn("dbo.Vote", "Review");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vote", "Review", c => c.String());
            DropColumn("dbo.Vote", "ReviewText");
        }
    }
}
