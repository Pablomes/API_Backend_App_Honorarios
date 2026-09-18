namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class ExpressionToken
    {
        public ExpressionTokenType type { get; set; }
        public string source { get; set; }
        public double? value { get; set; }

        public ExpressionToken()
        {
            source = "";
        }

        public ExpressionToken(ExpressionTokenType type, string source)
        {
            this.type = type;
            this.source = source;
        }

        public ExpressionToken(ExpressionTokenType type, string source, double value) : this(type, source)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{{ {type.ToString()}, {source}, {value ?? '-'} }}";
        }
    }
}
