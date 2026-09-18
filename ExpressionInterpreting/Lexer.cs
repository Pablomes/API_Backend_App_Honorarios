namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class Lexer
    {
        public Lexer() { }

        public List<ExpressionToken> Tokenise(string expression)
        {
            int idx = 0;

            List<ExpressionToken> result = new();

            while (idx < expression.Length)
            {
                char curr = expression[idx];

                if (curr == '+')
                    result.Add(new ExpressionToken(ExpressionTokenType.PLUS, expression[idx..(idx + 1)]));
                else if (curr == '-')
                    result.Add(new ExpressionToken(ExpressionTokenType.MINUS, expression[idx..(idx + 1)]));
                else if (curr == '*')
                    result.Add(new ExpressionToken(ExpressionTokenType.ASTERISK, expression[idx..(idx + 1)]));
                else if (curr == '/')
                    result.Add(new ExpressionToken(ExpressionTokenType.SLASH, expression[idx..(idx + 1)]));
                else if (curr == '^')
                    result.Add(new ExpressionToken(ExpressionTokenType.CARAT, expression[idx..(idx + 1)]));
                else if (curr == '(')
                    result.Add(new ExpressionToken(ExpressionTokenType.LBRACKET, expression[idx..(idx + 1)]));
                else if (curr == ')')
                    result.Add(new ExpressionToken(ExpressionTokenType.RBRACKET, expression[idx..(idx + 1)]));
                else if (char.IsDigit(curr))
                {
                    result.Add(parseNumber(expression, ref idx));
                }
                else if (char.IsLetter(curr))
                {
                    result.Add(parseName(expression, ref idx));
                }
                else if (curr != ' ')
                    throw new ArgumentException("Invalid expression for parser.");

                if (result.Last().type == ExpressionTokenType.ERROR)
                    throw new ArgumentException($"Invalid expression for parser. Error in {result.Last().source}.");

                idx++;
            }

            result.Add(new ExpressionToken(ExpressionTokenType.EOE, ""));

            return result;
        }

        private ExpressionToken parseNumber(string expression, ref int idx)
        {
            bool hasDecimal = false;
            int tokenStartIdx = idx;

            while (idx + 1 < expression.Length && (char.IsDigit(expression[idx + 1]) || expression[idx + 1] == '.'))
            {
                idx++;

                if (expression[idx] == '.' && hasDecimal)
                    return new ExpressionToken(ExpressionTokenType.ERROR, expression[tokenStartIdx..(idx + 1)]);

                if (expression[idx] == '.')
                    hasDecimal = true;
            }

            double value;

            if (!double.TryParse(expression[tokenStartIdx..(idx + 1)], out value))
                return new ExpressionToken(ExpressionTokenType.ERROR, expression[tokenStartIdx..(idx + 1)]);

            return new ExpressionToken(ExpressionTokenType.NUMBER, expression[tokenStartIdx..(idx + 1)], value);

        }

        private ExpressionToken parseName(string expression, ref int idx)
        {
            int tokenStartIdx = idx;

            while (idx + 1 < expression.Length && char.IsLetterOrDigit(expression[idx + 1]))
                idx++;

            return new ExpressionToken(ExpressionTokenType.VARIABLE, expression[tokenStartIdx..(idx + 1)]);
        }
    }
}
