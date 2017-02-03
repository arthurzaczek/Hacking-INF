namespace Hacking_INF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportedCompilerMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportedCompilerMessages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Course = c.String(nullable: false),
                        Example = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Messages = c.String(nullable: false),
                        Code = c.String(),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportedCompilerMessages", "User_ID", "dbo.Users");
            DropIndex("dbo.ReportedCompilerMessages", new[] { "User_ID" });
            DropTable("dbo.ReportedCompilerMessages");
        }
    }
}
