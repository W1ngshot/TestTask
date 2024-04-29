using FluentMigrator;

namespace TestTask.ConversionReportApp.Infrastructure.Migrations;

[Migration(20240426213000)]
public class InitSchema : Migration
{
    public override void Up()
    {
        Create.Table("conversion_reports")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("registration_id").AsInt64().NotNullable()
            .WithColumn("request_from").AsDateTimeOffset().NotNullable()
            .WithColumn("request_to").AsDateTimeOffset().NotNullable()
            .WithColumn("conversion_ratio").AsDecimal().NotNullable()
            .WithColumn("payments_count").AsInt64().NotNullable()
            .WithColumn("requested_at").AsDateTimeOffset().NotNullable();

        Create.Table("conversion_requests")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("registration_id").AsInt64().NotNullable()
            .WithColumn("request_from").AsDateTimeOffset().NotNullable()
            .WithColumn("request_to").AsDateTimeOffset().NotNullable()
            .WithColumn("requested_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("conversion_reports");
    }
}