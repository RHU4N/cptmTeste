using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cptmApiTeste.Migrations
{
    /// <inheritdoc />
    public partial class AddInspecaoLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LATITUDE';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" ADD ""LATITUDE"" BINARY_DOUBLE';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LOCALIZACAO';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" ADD ""LOCALIZACAO"" NVARCHAR2(2000)';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LONGITUDE';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" ADD ""LONGITUDE"" BINARY_DOUBLE';
    END IF;
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LATITUDE';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" DROP COLUMN ""LATITUDE""';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LOCALIZACAO';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" DROP COLUMN ""LOCALIZACAO""';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'LONGITUDE';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" DROP COLUMN ""LONGITUDE""';
    END IF;
END;");
        }
    }
}