namespace DataBaseContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserResponse = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                        Created = c.DateTime(false, defaultValueSql: "GETDATE()"),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        UserId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Order = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        QuizId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Order = c.Int(nullable: false),
                        IsAnswer = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Status = c.Int(nullable: false, defaultValue:1),
                        Cover = c.String(),
                        Created = c.DateTime(false, defaultValueSql: "GETDATE()"),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId, cascadeDelete: false)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Facebook = c.String(),
                        Telegram = c.String(),
                        PhoneNumber = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Created = c.DateTime(false, defaultValueSql: "GETDATE()"),
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserForQuizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        QuizId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.QuizId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserForQuizs", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserForQuizs", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.Answers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Questions", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.Quizs", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Options", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropIndex("dbo.UserForQuizs", new[] { "QuizId" });
            DropIndex("dbo.UserForQuizs", new[] { "UserId" });
            DropIndex("dbo.Quizs", new[] { "OwnerId" });
            DropIndex("dbo.Options", new[] { "QuestionId" });
            DropIndex("dbo.Questions", new[] { "QuizId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropIndex("dbo.Answers", new[] { "UserId" });
            DropTable("dbo.UserForQuizs");
            DropTable("dbo.Users");
            DropTable("dbo.Quizs");
            DropTable("dbo.Options");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
