using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class MadeWorkDayDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $EF$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Workdays' AND column_name = 'Date'
                          AND udt_name = 'timestamptz'
                    ) THEN
                        ALTER TABLE "Workdays"
                            ALTER COLUMN "Date" TYPE date
                            USING "Date"::date;
                    END IF;
                END
                $EF$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $EF$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Workdays' AND column_name = 'Date'
                          AND udt_name = 'date'
                    ) AND NOT EXISTS (SELECT 1 FROM "Workdays" LIMIT 1) THEN
                        ALTER TABLE "Workdays"
                            ALTER COLUMN "Date" TYPE timestamp with time zone
                            USING "Date"::timestamp with time zone;
                    END IF;
                END
                $EF$;
                """);
        }
    }
}
