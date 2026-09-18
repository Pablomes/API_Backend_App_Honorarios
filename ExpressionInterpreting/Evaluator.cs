using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class Evaluator
    {
        private Interpreter interpreter;

        public Evaluator(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public async Task<double> Evaluate(ExpressionNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            double res = 0;

            switch (node.Type)
            {
                case ExpressionNodeType.ADD:
                    if (node.Left == null || node.Right == null) throw new InvalidOperationException("ADD node requires left and right children.");
                    res = await Evaluate(node.Left) + await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.SUBTRACT:
                    if (node.Left == null || node.Right == null) throw new InvalidOperationException("SUBTRACT node requires left and right children.");
                    res = await Evaluate(node.Left) - await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.MULTIPLY:
                    if (node.Left == null || node.Right == null) throw new InvalidOperationException("MULTIPLY node requires left and right children.");
                    res = await Evaluate(node.Left) * await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.DIVIDE:
                    if (node.Left == null || node.Right == null) throw new InvalidOperationException("DIVIDE node requires left and right children.");
                    res = await Evaluate(node.Left) / await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.POWER:
                    if (node.Left == null || node.Right == null) throw new InvalidOperationException("POWER node requires left and right children.");
                    res = Math.Pow(await Evaluate(node.Left), await Evaluate(node.Right));
                    break;
                case ExpressionNodeType.GROUP:
                    if (node.Right == null) throw new InvalidOperationException("GROUP node requires a child.");
                    res = await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.NEGATE:
                    if (node.Right == null) throw new InvalidOperationException("NEGATE node requires a child.");
                    res = -await Evaluate(node.Right);
                    break;
                case ExpressionNodeType.ATOM:
                    res = await EvaluateAtom(node);
                    break;
                default:
                    res = 0;
                    break;
            }

            return res;
        }

        public async Task<double> EvaluateAtom(ExpressionNode node)
        {
            if (node.Type != ExpressionNodeType.ATOM)
                throw new ArgumentException($"Wrong node type. Expected atom got {node.Type}.");

            if (node.Token.type == ExpressionTokenType.NUMBER)
            {
                return (double)node.Token.value;
            }

            if (node.Token.type == ExpressionTokenType.VARIABLE)
            {
                return await this.interpreter.GetValue(node.Token.source);
            }
                

            return 0;
        }
    }
}
