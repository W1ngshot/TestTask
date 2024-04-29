using FluentMigrator;

namespace TestTask.ConversionReportApp.Infrastructure.Migrations;

[Migration(20240426214200)]
public class AddRequestV1Type : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'request_v1') THEN
            CREATE TYPE request_v1 as
            (
                  item_id           bigint
                , registration_id   bigint
                , request_from      timestamp with time zone
                , request_to        timestamp with time zone
                , requested_at      timestamp with time zone
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
        DROP TYPE IF EXISTS request_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}