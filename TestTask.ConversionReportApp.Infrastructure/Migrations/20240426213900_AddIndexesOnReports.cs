using FluentMigrator;

namespace TestTask.ConversionReportApp.Infrastructure.Migrations;

[Migration(20240426213900)]
public class AddIndexOnReports : Migration
{
    public override void Up()
    {
        Create.Index("ix_conversion_reports_requested_at").OnTable("conversion_reports")
            .OnColumn("requested_at").Descending();

        Create.Index("ix_conversion_reports_item_id_registration_id").OnTable("conversion_reports")
            .OnColumn("item_id").Ascending()
            .OnColumn("registration_id").Ascending();
    }

    public override void Down()
    {
        Delete.Index("ix_conversion_reports_requested_at").OnTable("conversion_reports");
        Delete.Index("ix_conversion_reports_item_id_registered_id").OnTable("conversion_reports");
    }
}