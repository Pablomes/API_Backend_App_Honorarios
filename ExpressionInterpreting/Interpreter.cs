using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Persistence;
using System.Threading.Tasks;

namespace API_Backend_App_Honorarios.ExpressionInterpreting
{
    public class Interpreter
    {
        private Dictionary<string, double> variables = new();
        private HashSet<string> resolving = new();
        
        private Lexer lexer = new();
        private Parser parser = new();
        private Evaluator evaluator;

        private EdificacionCalcDataDb edificacionCalcData;
        private ObraCivilCalcDataDb obraCivilCalcData;
        private UrbanizacionCalcDataDb urbanizacionCalcData;

        private CalculationMode calcMode = CalculationMode.EDIFICACION;

        public Interpreter(EdificacionCalcDataDb ecd, ObraCivilCalcDataDb occd, UrbanizacionCalcDataDb ucd)
        {
            edificacionCalcData = ecd;
            obraCivilCalcData = occd;
            urbanizacionCalcData = ucd;

            evaluator = new Evaluator(this);
        }

        public Interpreter(EdificacionCalcDataDb ecd, ObraCivilCalcDataDb occd, UrbanizacionCalcDataDb ucd, params (string name, double value)[] vars) : this(ecd, occd, ucd)
        {
            this.AddVariables(vars);
        }

        public void AddVariable(string name, double value)
        {
            variables.Add(name, value);
        }

        public void AddVariables(params (string name, double value)[] vars)
        {
            foreach (var (name, value) in vars)
            {
                variables.Add(name, value);
            }
        }

        public void ClearVariables()
        {
            this.variables.Clear();
        }

        public void SetCalculationMode(CalculationMode mode)
        {
            this.calcMode = mode;
        }

        public async Task<double> GetValue(string variableName)
        {
            switch (this.calcMode)
            {
                case CalculationMode.EDIFICACION:
                    return await GetEdificacionValue(variableName);
                case CalculationMode.OBRA_CIVIL:
                    return await GetObraCivilValue(variableName);
                case CalculationMode.URBANIZACION:
                    return await GetUrbanizacionValue(variableName);
            }

            return 0;
        }

        public async Task<double> GetEdificacionValue(string variableName)
        {
            double value = 0;

            if (variables.TryGetValue(variableName, out value))
            {
                return value;
            }

            if (!resolving.Add(variableName))
                throw new InvalidOperationException($"Existe una dependencia circular para el calculo de la variable {variableName}.");

            try
            {
                EdificacionCalcData? calcData = await edificacionCalcData.EdificacionCalcDatas.FindAsync(variableName) ?? throw new KeyNotFoundException($"La variable {variableName} no se encuentra ni inicializada ni en la BBDD.");

                double result = await this.Interpret(calcData.Formula);

                variables.Add(variableName, result);

                return result;
            }
            finally
            {
                resolving.Remove(variableName);
            }
        }

        public async Task<double> GetObraCivilValue(string variableName)
        {
            if (variables.TryGetValue(variableName, out double value))
                return value;

            if (!resolving.Add(variableName))
                throw new InvalidOperationException($"Existe una dependencia circular para el calculo de la variable {variableName}.");

            try
            {
                ObraCivilCalcData? calcData = await obraCivilCalcData.ObraCivilCalcDatas.FindAsync(variableName) ?? throw new KeyNotFoundException($"La variable {variableName} no se encuentra ni inicializada ni en la BBDD.");

                double result = await this.Interpret(calcData.Formula);

                variables.Add(variableName, result);

                return result;
            }
            finally
            {
                resolving.Remove(variableName);
            }
        }

        public async Task<double> GetUrbanizacionValue(string variableName)
        {

            if (variables.TryGetValue(variableName, out double value))
                return value;

            if (!resolving.Add(variableName))
                throw new InvalidOperationException($"Existe una dependencia circular para el calculo de la variable {variableName}.");

            try
            {
                UrbanizacionCalcData? calcData = await urbanizacionCalcData.UrbanizacionCalcDatas.FindAsync(variableName) ?? throw new KeyNotFoundException($"La variable {variableName} no se encuentra ni inicializada ni en la BBDD.");

                double result = await this.Interpret(calcData.Formula);

                variables.Add(variableName, result);

                return result;
            }
            finally
            {
                resolving.Remove(variableName);
            }
        }

        private async Task<double> Interpret(string expression)
        {
            try
            {         
                List<ExpressionToken> tokens = lexer.Tokenise(expression);

                ExpressionNode ast = parser.Parse(tokens);

                double result = await evaluator.Evaluate(ast);

                return result;
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}
