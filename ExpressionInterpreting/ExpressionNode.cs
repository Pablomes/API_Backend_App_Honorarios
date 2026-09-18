namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class ExpressionNode
    {
        public ExpressionNode? Left { get; set; }
        public ExpressionNode? Right { get; set; }

        public ExpressionToken Token { get; set; }

        public ExpressionNodeType Type { get; set; }

        public ExpressionNode(ExpressionToken token, ExpressionNodeType type)
        {
            this.Token = token;
            this.Type = type;
        }

        public override string ToString()
        {
            return ToStringAsTree(0);
        }

        public string ToStringAsTree()
        {
            return ToStringAsTree(0);
        }

        public string ToStringAsTree(int depth)
        {
            string res =  depthIndicator(depth) + $"{{ {Type}, {Token} }}\n";

            if (Left != null)
                res += depthIndicator(depth) + "Left:\n" + Left.ToStringAsTree(depth + 1);
            if (Right != null)
                res += depthIndicator(depth) + "Right:\n" + Right.ToStringAsTree(depth + 1);

            return res;
        }

        private string depthIndicator(int depth)
        {
            string res = "";

            while (depth > 0)
            {
                depth--;
                res += "|   ";
            }

            return res;
        }
    }
}
