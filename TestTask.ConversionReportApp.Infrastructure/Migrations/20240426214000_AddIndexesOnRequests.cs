using FluentMigrator;

namespace TestTask.ConversionReportApp.Infrastructure.Migrations;

[Migration(20240426214000)]
public class AddIndexesOnRequests : Migration
{
    public override void Up()
    {
        Create.Index("ix_conversion_requests_requested_at").OnTable("conversion_requests")
            .OnColumn("requested_at").Ascending();
    }

    public override void Down()
    {
        Delete.Index("ix_conversion_requests_requested_at").OnTable("conversion_requests");
    }
}