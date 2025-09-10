using System.Collections.Generic;
using SQL.Formatter.Core;
using SQL.Formatter.Language;
using SQL.Formatter.Test.Behavior;
using SQL.Formatter.Test.Feature;
using Xunit;

namespace SQL.Formatter.Test
{
    public class TSqlFormatterTest
    {
        public readonly SqlFormatter.Formatter Formatter = SqlFormatter.Of(Dialect.TSql);

        [Fact]
        public void BehavesLikeSqlFormatterTest()
        {
            BehavesLikeSqlFormatter.Test(Formatter);
        }

        [Fact]
        public void CaseTest()
        {
            Case.Test(Formatter);
        }

        [Fact]
        public void CreateTableTest()
        {
            CreateTable.Test(Formatter);
        }

        [Fact]
        public void AlterTableTest()
        {
            AlterTable.Test(Formatter);
        }

        [Fact]
        public void StringsTest()
        {
            Strings.Test(Formatter, new List<string>
            {
                StringLiteral.DoubleQuote,
                StringLiteral.SingleQuote,
                StringLiteral.NSingleQuote,
                StringLiteral.Bracket
            });
        }

        [Fact]
        public void BetweenTest()
        {
            Between.Test(Formatter);
        }

        [Fact]
        public void SchemaTest()
        {
            Schema.Test(Formatter);
        }

        [Fact]
        public void OperatorsTest()
        {
            Operators.Test(Formatter, new List<string>
            {
                "%",
                "&",
                "|",
                "^",
                "~",
                "!=",
                "!<",
                "!>",
                "+=",
                "-=",
                "*=",
                "/=",
                "%=",
                "|=",
                "&=",
                "^=",
                "::"
            });
        }

        [Fact]
        public void JoinTest()
        {
            Join.Test(Formatter, new List<string>
            {
                "NATURAL"
            });
        }

        [Fact]
        public void FormatsInsertWithoutInto()
        {
            Assert.Equal(
                "INSERT\n"
                + "  Customers (ID, MoneyBalance, Address, City)\n"
                + "VALUES\n"
                + "  (12, -123.4, 'Skagen 2111', 'Stv');",
                Formatter.Format(
                    "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');"));
        }

        [Fact]
        public void RecognizesAtVariables()
        {
            Assert.Equal(
                "SELECT\n"
                + "  @variable,\n"
                + "  @\"var name\",\n"
                + "  @[var name];",
                Formatter.Format(
                    "SELECT @variable, @\"var name\", @[var name];"));
        }

        [Fact]
        public void ReplacesAtVariablesWithParamValues()
        {
            Assert.Equal(
                "SELECT\n"
                + "  a,\n"
                + "  b\n"
                + "FROM\n"
                + "  t\n"
                + "  CROSS JOIN t2 on t.id = t2.id_t",
                Formatter.Format(
                    "SELECT a, b FROM t CROSS JOIN t2 on t.id = t2.id_t"));
        }

        [Fact]
        public void FormatsSelectQueryWithCrossJoin()
        {
            Assert.Equal(
                "SELECT\n"
                + "  'var value',\n"
                + "  'var value1',\n"
                + "  'var value2';",
                Formatter.Format(
                    "SELECT @variable, @\"var name1\", @[var name2];", new Dictionary<string, string>
                    {
                        { "variable", "'var value'"},
                        { "var name1", "'var value1'"},
                        { "var name2", "'var value2'"},
                    }));
        }

        [Fact]
        public void TokenizerTreatsBeginEndAsParentheses()
        {
            var tokenizer = new TSqlFormatter(FormatConfig.Builder().Build()).Tokenizer();
            var tokens = tokenizer.Tokenize("BEGIN SELECT 1 END");

            Assert.Equal(TokenTypes.OPEN_PAREN, tokens.Get(0).Type);
            Assert.Equal(TokenTypes.CLOSE_PAREN, tokens.Get(tokens.Size() - 1).Type);
        }

        [Fact]
        public void FormatsNestedBeginEndBlocks()
        {
            var sql = "BEGIN BEGIN SELECT 1 END END";
            var expected = "BEGIN\n" +
                           "  BEGIN\n" +
                           "    SELECT\n" +
                           "      1\n" +
                           "  END\n" +
                           "END";
            Assert.Equal(expected, Formatter.Format(sql));
        }
        public void FormatsQueriesSeparatedByGo()
        {
            var sql = "SELECT 1\nGO\nSELECT 2";
            var expected = "SELECT\n" +
                            "  1\n" +
                            "GO\n" +
                            "SELECT\n" +
                            "  2";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void FormatsMultipleQueriesSeparatedBySemicolons()
        {
            var sql = "SELECT 1;SELECT 2;SELECT 3";
            var expected =
                "SELECT\n" +
                "  1;\n" +
                "SELECT\n" +
                "  2;\n" +
                "SELECT\n" +
                "  3";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void DoesNotTreatBeginTranAsBlock()
        {
            var sql = "BEGIN TRAN\nSELECT 1";
            var expected = "BEGIN TRAN\nSELECT\n  1";

            Assert.Equal(expected, Formatter.Format(sql));
        }
        public void PlacesGoOnItsOwnLineEvenWithoutLineBreaks()
        {
            var sql = "SELECT 1 GO SELECT 2";
            var expected =
                "SELECT\n" +
                "  1\n" +
                "GO\n" +
                "SELECT\n" +
                "  2";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void DoesNotTreatBeginDistributedTranAsBlock()
        {
            var sql = "BEGIN DISTRIBUTED TRAN\nSELECT 1";
            var expected = "BEGIN DISTRIBUTED TRAN\nSELECT\n  1";

            Assert.Equal(expected, Formatter.Format(sql));

        }

        [Fact]
        public void FormatsCreateProcedureDefinition()
        {
            Assert.Equal(
                "CREATE PROCEDURE\n"
                + "  test AS\n"
                + "SELECT\n"
                + "  1;",
                Formatter.Format("CREATE PROCEDURE test AS SELECT 1;"));
        }

        [Fact]
        public void FormatsAlterProcedureDefinition()
        {
            Assert.Equal(
                "ALTER PROCEDURE\n"
                + "  test AS\n"
                + "SELECT\n"
                + "  1;",
                Formatter.Format("ALTER PROCEDURE test AS SELECT 1;"));

        }

        [Fact]
        public void FormatsProcedureBodyWithSemicolon()
        {
            Assert.Equal(
                "CREATE PROCEDURE\n"
                + "  test AS BEGIN\n"
                + "    SET\n"
                + "      NOCOUNT ON;\n"
                + "    SELECT\n"
                + "      1;\n"
                + "    SELECT\n"
                + "      2;\n"
                + "  END;",
                Formatter.Format("CREATE PROCEDURE test AS BEGIN SET NOCOUNT ON; SELECT 1; SELECT 2; END;"));
        }

        [Fact]
        public void KeepsIndentInsideParenthesesWithSemicolon()
        {
            var sql = "SELECT (SELECT 1; SELECT 2);";
            var expected = "SELECT\n"
                           + "  (\n"
                           + "    SELECT\n"
                           + "      1;\n"
                           + "    SELECT\n"
                           + "      2\n"
                           + "  );";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        public void HandlesLowercaseAndMixedCaseGo()
        {
            var sql = "SELECT 1\n" +
                      "go\n" +
                      "SELECT 2\n" +
                      "Go\n" +
                      "SELECT 3";
            var expected =
                "SELECT\n" +
                "  1\n" +
                "go\n" +
                "SELECT\n" +
                "  2\n" +
                "Go\n" +
                "SELECT\n" +
                "  3";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void DoesNotTreatGoPrefixAsSeparator()
        {
            var sql = "SELECT 1\nGONEXT\nSELECT 2";
            var expected =
                "SELECT\n" +
                "  1 GONEXT\n" +
                "SELECT\n" +
                "  2";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void IgnoresGoAndSemicolonInsideStringsAndComments()
        {
            var sql = "SELECT 'GO' AS label; -- GO should not split\nSELECT 1";
            var expected =
                "SELECT\n" +
                "  'GO' AS label;\n" +
                "-- GO should not split\n" +
                "SELECT\n" +
                "  1";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void SemicolonAtEndOfInputProducesNoTrailingNewline()
        {
            var sql = "SELECT 1;";
            var expected =
                "SELECT\n" +
                "  1;";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void GoAtEndOfInputProducesNoTrailingNewline()
        {
            var sql = "SELECT 1\nGO";
            var expected =
                "SELECT\n" +
                "  1\n" +
                "GO";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void ConsecutiveGoSeparatorsArePreserved()
        {
            var sql = "SELECT 1\nGO\nGO\nSELECT 2";
            var expected =
                "SELECT\n" +
                "  1\n" +
                "GO\n" +
                "GO\n" +
                "SELECT\n" +
                "  2";
            Assert.Equal(expected, Formatter.Format(sql));
        }

        [Fact]
        public void CustomSeparatorsDisableDefaultGoAndSemicolon()
        {
            var cfg = SQL.Formatter.Core.FormatConfig.Builder()
                .QuerySeparators("END")
                .Build();

            var sql = "SELECT 1 END SELECT 2; GO SELECT 3";
            var expected =
                "SELECT\n" +
                "  1\n" +
                "END\n" +
                "SELECT\n" +
                "  2 ; GO\nSELECT\n" +
                "  3";

            Assert.Equal(expected, Formatter.Format(sql, cfg));
        }
    }
}
