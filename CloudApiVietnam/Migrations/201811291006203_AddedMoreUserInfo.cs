namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMoreUserInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "BirthDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "FirstName", c => c.String());
            AddColumn("dbo.Users", "LastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "FirstName");
            DropColumn("dbo.Users", "BirthDate");
        }
    }
}
