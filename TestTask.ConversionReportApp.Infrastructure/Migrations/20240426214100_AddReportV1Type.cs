using FluentMigrator;

namespace TestTask.ConversionReportApp.Infrastructure.Migrations;

[Migration(20240426214100)]
public class AddReportV1Type : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'report_v1') THEN
            CREATE TYPE report_v1 as
            (
                  item_id           bigint
                , registration_id   bigint
                , request_from      timestamp with time zone
                , request_to        timestamp with time zone
                , requested_at      timestamp with time zone
                , conversion_ratio  decimal
                , payments_count    bigint
            );
        END IF;
    END
$$;";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
DO $$
    BEGIN
        DROP TYPE IF EXISTS report_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}