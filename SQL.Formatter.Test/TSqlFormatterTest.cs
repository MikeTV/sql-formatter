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

        [Fact]
        public void DoesNotTreatBeginTranAsBlock()
        {
            var sql = "BEGIN TRAN\nSELECT 1";
            var expected = "BEGIN TRAN\nSELECT\n  1";

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
    }
}
