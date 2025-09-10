# AGENTS

## Core
- Tokenizer accepts `FormatConfig.QuerySeparators` to emit `QUERY_SEPARATOR` tokens (e.g., `GO`).
- `FormatConfig` exposes `WithQuerySeparators` and `PlusQuerySeparators` helpers for immutably setting query delimiters.

## Formatting
- `FormatQuerySeparator` resets indentation and handles semicolons and custom query delimiters.

## Dialects
- `Dialect.TSql` uses `WithQuerySeparators` to insert `"GO"` when no query separators are supplied.
