namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVerified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Verified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Verified");
        }
    }
}
