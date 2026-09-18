namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class Parser
    {
        public Parser() { }

        public ExpressionNode Parse(List<ExpressionToken> tokens)
        {
            int idx = 0;

            ExpressionNode res = null;

            try
            {
                res = Expression(tokens, ref idx);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ArgumentException("Invalid expression. The expression ends abruptly.");
            }

            if (tokens[idx].type != ExpressionTokenType.EOE)
                throw new ArgumentException($"Invalid expression {string.Join(',', tokens)}. The expression is missing some conjoining symbol and is therefore split.");

            return res;
        }

        private ExpressionNode Expression(List<ExpressionToken> tokens, ref int idx)
        {
            ExpressionToken token = null;

            ExpressionNode left = Term(tokens, ref idx);

            while (tokens[idx].type == ExpressionTokenType.PLUS || tokens[idx].type == ExpressionTokenType.MINUS)
            {
                token = tokens[idx++];

                ExpressionNode right = Term(tokens, ref idx);

                ExpressionNode res = new ExpressionNode(token, token.type == ExpressionTokenType.PLUS ? ExpressionNodeType.ADD : ExpressionNodeType.SUBTRACT);
                res.Left = left;
                res.Right = right;

                left = res;
            }

            return left;
        }

        private ExpressionNode Term(List<ExpressionToken> tokens, ref int idx)
        {
            ExpressionToken token = null;

            ExpressionNode left = Power(tokens, ref idx);

            while (tokens[idx].type == ExpressionTokenType.ASTERISK || tokens[idx].type == ExpressionTokenType.SLASH)
            {
                token = tokens[idx++];

                ExpressionNode right = Power(tokens, ref idx);

                ExpressionNode res = new ExpressionNode(token, token.type == ExpressionTokenType.ASTERISK ? ExpressionNodeType.MULTIPLY : ExpressionNodeType.DIVIDE);
                res.Left = left;
                res.Right = right;

                left = res;
            }

            return left;
        }

        private ExpressionNode Power(List<ExpressionToken> tokens, ref int idx)
        {
            ExpressionToken token = null;

            ExpressionNode left = Factor(tokens, ref idx);

            while (tokens[idx].type == ExpressionTokenType.CARAT)
            {
                token = tokens[idx++];

                ExpressionNode right = Factor(tokens, ref idx);

                ExpressionNode res = new ExpressionNode(token, ExpressionNodeType.POWER);
                res.Left = left;
                res.Right = right;

                left = res;
            }

            return left;
        }

        private ExpressionNode Factor(List<ExpressionToken> tokens, ref int idx)
        {
            bool isNegate = false;
            ExpressionToken token = null;

            while (tokens[idx].type == ExpressionTokenType.MINUS)
            {
                token = tokens[idx]; isNegate = !isNegate; idx++;
            }
                


            ExpressionNode right = Atom(tokens, ref idx);
            ExpressionNode res;

            if (isNegate)
            {
                res = new ExpressionNode(token, ExpressionNodeType.NEGATE);
                res.Right = right;
            }
            else
                res = right;

            return res;
        }

        private ExpressionNode Atom(List<ExpressionToken> tokens, ref int idx)
        {
            if (tokens[idx].type == ExpressionTokenType.LBRACKET)
            {
                ExpressionNode res = new ExpressionNode(tokens[idx], ExpressionNodeType.GROUP);
                idx++;
                res.Right = Expression(tokens, ref idx);
                if (tokens[idx].type != ExpressionTokenType.RBRACKET)
                    throw new ArgumentException($"Error in expression. Missing closing parenthesis.");

                idx++;
                return res;
            }

            return new ExpressionNode(tokens[idx++], ExpressionNodeType.ATOM);
        }
    }
}
