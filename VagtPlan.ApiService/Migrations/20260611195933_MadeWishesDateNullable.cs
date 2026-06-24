using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class MadeWishesDateNullable : Migration
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
                          AND is_nullable = 'NO'
                    ) THEN
                        ALTER TABLE "SpecialWishes" ALTER COLUMN "StartDate" DROP NOT NULL;
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'EndDate'
                          AND is_nullable = 'NO'
                    ) THEN
                        ALTER TABLE "SpecialWishes" ALTER COLUMN "EndDate" DROP NOT NULL;
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
                              AND is_nullable = 'YES'
                        ) THEN
                            ALTER TABLE "SpecialWishes" ALTER COLUMN "StartDate" SET NOT NULL;
                        END IF;

                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'public' AND table_name = 'SpecialWishes' AND column_name = 'EndDate'
                              AND is_nullable = 'YES'
                        ) THEN
                            ALTER TABLE "SpecialWishes" ALTER COLUMN "EndDate" SET NOT NULL;
                        END IF;
                    END IF;
                END
                $EF$;
                """);
        }
    }
}
