namespace Hacking_INF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExampleResults",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Course = c.String(nullable: false),
                        Example = c.String(nullable: false),
                        FirstAttempt = c.DateTime(nullable: false),
                        LastAttempt = c.DateTime(nullable: false),
                        Time = c.Int(),
                        NumOfTests = c.Int(nullable: false),
                        NumOfSucceeded = c.Int(nullable: false),
                        NumOfFailed = c.Int(nullable: false),
                        NumOfErrors = c.Int(nullable: false),
                        NumOfSkipped = c.Int(nullable: false),
                        NumOfCompilations = c.Int(nullable: false),
                        NumOfTestRuns = c.Int(nullable: false),
                        LinesOfCode = c.Int(),
                        CyclomaticComplexity = c.Int(),
                        MemErrors = c.Int(),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UID = c.String(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExampleResults", "User_ID", "dbo.Users");
            DropIndex("dbo.ExampleResults", new[] { "User_ID" });
            DropTable("dbo.Users");
            DropTable("dbo.ExampleResults");
        }
    }
}
