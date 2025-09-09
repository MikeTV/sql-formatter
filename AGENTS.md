# AGENTS

## T-SQL Formatting
- `TSqlFormatter.DoDialectConfig` lists tokens considered opening and closing parentheses. Include `BEGIN` and `END` here to ensure blocks indent properly.
- Tokenizer builds its parentheses regex directly from the dialect configuration; updating the config automatically adjusts token types.
