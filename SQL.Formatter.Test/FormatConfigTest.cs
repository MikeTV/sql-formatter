using System.Collections.Generic;
using SQL.Formatter.Core;
using Xunit;

namespace SQL.Formatter.Test
{
    public class FormatConfigTest
    {
        [Fact]
        public void WithQuerySeparatorsReplacesExisting()
        {
            var cfg = FormatConfig.Builder()
                .QuerySeparators("GO")
                .Build()
                .WithQuerySeparators("END");

            Assert.Single(cfg.QuerySeparators);
            Assert.Equal("END", cfg.QuerySeparators[0]);
        }

        [Fact]
        public void PlusQuerySeparatorsAppendsSeparators()
        {
            var cfg = FormatConfig.Builder()
                .QuerySeparators("GO")
                .Build()
                .PlusQuerySeparators("END", "STOP");

            Assert.Equal(new List<string> { "GO", "END", "STOP" }, cfg.QuerySeparators);
        }
    }
}
