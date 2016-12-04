namespace redfoodie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueconstraint : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Restaurant", "UniqueName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Restaurant", new[] { "UniqueName" });
        }
    }
}
