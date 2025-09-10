# AGENTS

## T-SQL Formatting
- `TSqlFormatter.DoDialectConfig` lists tokens considered opening and closing parentheses. Include `BEGIN` and `END` here to ensure blocks indent properly.
- Tokenizer builds its parentheses regex directly from the dialect configuration; updating the config automatically adjusts token types.
- Add multi-word top-level keywords (e.g., `CREATE PROCEDURE`, `ALTER PROCEDURE`) to `s_reservedTopLevelWords` in `TSqlFormatter` to ensure stored procedure definitions are formatted as standalone statements.
- `TSqlFormatter` tracks `_blockDepth` (parentheses depth) so semicolons inside parentheses don't reset indentation and `SET` behaves as a newline keyword within procedures.

## Core
- Tokenizer accepts `FormatConfig.QuerySeparators` to emit `QUERY_SEPARATOR` tokens (e.g., `GO`).
- `FormatConfig` exposes `WithQuerySeparators` and `PlusQuerySeparators` helpers for immutably setting query delimiters.

## Formatting
- `FormatQuerySeparator` resets indentation and handles semicolons and custom query delimiters.

## Dialects
- `TSqlFormatter` injects `GO` alongside the default semicolon when no custom query separators are supplied.
