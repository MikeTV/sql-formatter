# AGENTS

## Core
- Tokenizer accepts `FormatConfig.QuerySeparators` to emit `QUERY_SEPARATOR` tokens (e.g., `GO`).
- `FormatConfig` exposes `WithQuerySeparators` and `PlusQuerySeparators` helpers for immutably setting query delimiters.

## Formatting
- `FormatQuerySeparator` resets indentation and handles semicolons and custom query delimiters.

## Dialects
- `TSqlFormatter` injects `GO` alongside the default semicolon when no custom query separators are supplied.
