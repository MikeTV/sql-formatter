# AGENTS

## T-SQL Formatting
- `TSqlFormatter.DoDialectConfig` lists tokens considered opening and closing parentheses. Include `BEGIN` and `END` here to ensure blocks indent properly.
- Tokenizer builds its parentheses regex directly from the dialect configuration; updating the config automatically adjusts token types.
- Add multi-word top-level keywords (e.g., `CREATE PROCEDURE`, `ALTER PROCEDURE`) to `s_reservedTopLevelWords` in `TSqlFormatter` to ensure stored procedure definitions are formatted as standalone statements.
