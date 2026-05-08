using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cptmApiTeste.Migrations
{
    public partial class AddInspecaoUsuarioOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'USUARIO_ID';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" ADD ""USUARIO_ID"" NUMBER(10)';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_INDEXES
    WHERE INDEX_NAME = 'IX_INSPECAO_USUARIO_ID';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'CREATE INDEX ""IX_INSPECAO_USUARIO_ID"" ON ""INSPECAO"" (""USUARIO_ID"")';
    END IF;
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_INDEXES
    WHERE INDEX_NAME = 'IX_INSPECAO_USUARIO_ID';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'DROP INDEX ""IX_INSPECAO_USUARIO_ID""';
    END IF;
END;");

            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'USUARIO_ID';

    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" DROP COLUMN ""USUARIO_ID""';
    END IF;
END;");
        }
    }
}
