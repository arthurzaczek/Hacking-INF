namespace Hacking_INF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Log_Anonymous_Submissions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExampleResults", "User_ID", "dbo.Users");
            DropIndex("dbo.ExampleResults", new[] { "User_ID" });
            AddColumn("dbo.ExampleResults", "SessionID", c => c.Guid());
            AlterColumn("dbo.ExampleResults", "User_ID", c => c.Int());
            CreateIndex("dbo.ExampleResults", "User_ID");
            AddForeignKey("dbo.ExampleResults", "User_ID", "dbo.Users", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExampleResults", "User_ID", "dbo.Users");
            DropIndex("dbo.ExampleResults", new[] { "User_ID" });
            AlterColumn("dbo.ExampleResults", "User_ID", c => c.Int(nullable: false));
            DropColumn("dbo.ExampleResults", "SessionID");
            CreateIndex("dbo.ExampleResults", "User_ID");
            AddForeignKey("dbo.ExampleResults", "User_ID", "dbo.Users", "ID", cascadeDelete: true);
        }
    }
}
