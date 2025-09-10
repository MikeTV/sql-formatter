using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.Core
{
    /// <summary>
    /// Configuration options that control formatting behaviour.
    /// </summary>
    public class FormatConfig
    {
        public static readonly string DefaultIndent = "  ";
        public static readonly int DefaultColumnMaxLength = 50;

        public readonly string Indent;
        public readonly int MaxColumnLength;
        public readonly Params Parameters;
        public readonly bool Uppercase;
        public readonly int LinesBetweenQueries;
        public readonly bool SkipWhitespaceNearBlockParentheses;
        public readonly List<string> QuerySeparators;

        public FormatConfig(
            string indent,
            int maxColumnLength,
            Params parameters,
            bool uppercase,
            int linesBetweenQueries,
            bool skipWhitespaceNearBlockParentheses,
            List<string> querySeparators)
        {
            Indent = indent;
            MaxColumnLength = maxColumnLength;
            Parameters = parameters == null ? Params.Empty : parameters;
            Uppercase = uppercase;
            LinesBetweenQueries = linesBetweenQueries;
            SkipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
            QuerySeparators = querySeparators ?? new List<string> { ";" };
        }

        /// <summary>
        /// Returns a copy of this configuration with the specified query separators.
        /// </summary>
        public FormatConfig WithQuerySeparators(List<string> querySeparators)
        {
            return new FormatConfig(
                Indent,
                MaxColumnLength,
                Parameters,
                Uppercase,
                LinesBetweenQueries,
                SkipWhitespaceNearBlockParentheses,
                querySeparators);
        }

        /// <summary>
        /// Returns a copy of this configuration with the specified query separators.
        /// </summary>
        public FormatConfig WithQuerySeparators(params string[] querySeparators)
        {
            return WithQuerySeparators(querySeparators.ToList());
        }

        /// <summary>
        /// Returns a copy of this configuration with additional query separators appended.
        /// </summary>
        public FormatConfig PlusQuerySeparators(List<string> querySeparators)
        {
            return WithQuerySeparators(QuerySeparators.Concat(querySeparators).ToList());
        }

        /// <summary>
        /// Returns a copy of this configuration with additional query separators appended.
        /// </summary>
        public FormatConfig PlusQuerySeparators(params string[] querySeparators)
        {
            return PlusQuerySeparators(querySeparators.ToList());
        }

        public static FormatConfigBuilder Builder()
        {
            return new FormatConfigBuilder();
        }

        public class FormatConfigBuilder
        {
            private string _indent = DefaultIndent;
            private int _maxColumnLength = DefaultColumnMaxLength;
            private Params _parameters;
            private bool _uppercase;
            private int _linesBetweenQueries;
            private bool _skipWhitespaceNearBlockParentheses;
            private List<string> _querySeparators;

            public FormatConfigBuilder() { }

            public FormatConfigBuilder Indent(string indent)
            {
                _indent = indent;
                return this;
            }

            public FormatConfigBuilder MaxColumnLength(int maxColumnLength)
            {
                _maxColumnLength = maxColumnLength;
                return this;
            }

            public FormatConfigBuilder Params(Params parameters)
            {
                _parameters = parameters;
                return this;
            }

            public FormatConfigBuilder Params<T>(Dictionary<string, T> parameters)
            {
                return Params(Core.Params.Of(parameters));
            }

            public FormatConfigBuilder Params<T>(List<T> parameters)
            {
                return Params(Core.Params.Of(parameters));
            }

            public FormatConfigBuilder Uppercase(bool uppercase)
            {
                _uppercase = uppercase;
                return this;
            }

            public FormatConfigBuilder LinesBetweenQueries(int linesBetweenQueries)
            {
                _linesBetweenQueries = linesBetweenQueries;
                return this;
            }

            public FormatConfigBuilder SkipWhitespaceNearBlockParentheses(bool skipWhitespaceNearBlockParentheses)
            {
                _skipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
                return this;
            }

            /// <summary>
            /// Sets tokens that separate individual queries, e.g. "GO" when SQL Server dialect is selected.
            /// </summary>
            public FormatConfigBuilder QuerySeparators(List<string> querySeparators)
            {
                _querySeparators = querySeparators;
                return this;
            }

            /// <summary>
            /// Sets tokens that separate individual queries, e.g. "GO" when SQL Server dialect is selected.
            /// </summary>
            public FormatConfigBuilder QuerySeparators(params string[] querySeparators)
            {
                _querySeparators = querySeparators.ToList();
                return this;
            }

            public FormatConfig Build()
            {
                return new FormatConfig(
                    _indent,
                    _maxColumnLength,
                    _parameters,
                    _uppercase,
                    _linesBetweenQueries,
                    _skipWhitespaceNearBlockParentheses,
                    _querySeparators);
            }
        }
    }
}
