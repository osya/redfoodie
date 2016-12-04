namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFollow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Follow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        FollowUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.FollowUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.FollowUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Follow", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Follow", "FollowUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Follow", new[] { "FollowUserId" });
            DropIndex("dbo.Follow", new[] { "UserId" });
            DropTable("dbo.Follow");
        }
    }
}
