namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotificationsettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FollowMail", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ReplyReviewmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ThanksFavoritemail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ThanksFavoritemail");
            DropColumn("dbo.AspNetUsers", "ReplyReviewmail");
            DropColumn("dbo.AspNetUsers", "FollowMail");
        }
    }
}
