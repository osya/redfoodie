namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaxLengthtoBio : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Bio", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Bio", c => c.String());
        }
    }
}
