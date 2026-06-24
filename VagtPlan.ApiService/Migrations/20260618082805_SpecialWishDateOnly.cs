using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class SpecialWishDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $EF$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'StartDate'
                          AND udt_name = 'timestamptz'
                    ) THEN
                        ALTER TABLE "SpecialWishes"
                            ALTER COLUMN "StartDate" TYPE date
                            USING "StartDate"::date;
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'EndDate'
                          AND udt_name = 'timestamptz'
                    ) THEN
                        ALTER TABLE "SpecialWishes"
                            ALTER COLUMN "EndDate" TYPE date
                            USING "EndDate"::date;
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
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'SpecialWishes')
                       AND NOT EXISTS (SELECT 1 FROM "SpecialWishes" LIMIT 1) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'StartDate'
                              AND udt_name = 'date'
                        ) THEN
                            ALTER TABLE "SpecialWishes"
                                ALTER COLUMN "StartDate" TYPE timestamp with time zone
                                USING "StartDate"::timestamp with time zone;
                        END IF;

                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'EndDate'
                              AND udt_name = 'date'
                        ) THEN
                            ALTER TABLE "SpecialWishes"
                                ALTER COLUMN "EndDate" TYPE timestamp with time zone
                                USING "EndDate"::timestamp with time zone;
                        END IF;
                    END IF;
                END
                $EF$;
                """);
        }
    }
}
