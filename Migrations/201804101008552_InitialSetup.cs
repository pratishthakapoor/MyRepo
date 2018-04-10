namespace ProactiveBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BotDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        questions = c.String(),
                        server_details = c.String(),
                        middleware_details = c.String(),
                        database_details = c.String(),
                        token_details = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BotDetails");
        }
    }
}
