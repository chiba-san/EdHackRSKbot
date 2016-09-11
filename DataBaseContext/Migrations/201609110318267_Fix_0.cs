namespace DataBaseContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix_0 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Quizs", "OwnerId", "dbo.Users");
            DropIndex("dbo.Quizs", new[] { "OwnerId" });
            DropColumn("dbo.Quizs", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Quizs", "OwnerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Quizs", "OwnerId");
            AddForeignKey("dbo.Quizs", "OwnerId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
