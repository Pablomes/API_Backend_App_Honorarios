using API_Backend_App_Honorarios.ExpressionInterpreting;
using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API_Backend_App_Honorarios.Services
{
    public class CalculationService
    {
        private Interpreter interpreter;

        protected readonly EdificacionCalcDataDb edificacionCalcDataDb;
        protected readonly EdificacionPlantasCalcDataDb edificacionPlantasCalcDataDb;
        private Dictionary<int, double> coefsReduccionPlantas;

        protected readonly CoefHObraCivilCalcDataDb coefHObraCivilCalcDataDb;
        protected readonly CoefsObraCivilCalcDataDb coefsObraCivilCalcDataDb;
        protected readonly ObraCivilCalcDataDb obraCivilCalcDataDb;
        protected readonly PEMRObraCivilCalcDataDb pemrObraCivilCalcDataDb;
        private List<int> PEMRPresupuestos;

        protected readonly UrbanizacionCalcDataDb urbanizacionCalcDataDb;

        public CalculationService(EdificacionCalcDataDb edificacionCalcDataDb, EdificacionPlantasCalcDataDb edificacionPlantasCalcDataDb, 
            CoefHObraCivilCalcDataDb coefHObraCivilCalcDataDb, CoefsObraCivilCalcDataDb coefsObraCivilCalcDataDb,
            ObraCivilCalcDataDb obraCivilCalcDataDb, PEMRObraCivilCalcDataDb pEMRObraCivilCalcDataDb,
            UrbanizacionCalcDataDb urbanizacionCalcDataDb)
        {
            this.edificacionCalcDataDb = edificacionCalcDataDb;
            this.edificacionPlantasCalcDataDb = edificacionPlantasCalcDataDb;

            coefsReduccionPlantas = this.edificacionPlantasCalcDataDb.EdificacionPlantasCalcDatas.ToDictionary(p => p.Id, p => p.Coef);

            this.coefHObraCivilCalcDataDb = coefHObraCivilCalcDataDb;
            this.coefsObraCivilCalcDataDb = coefsObraCivilCalcDataDb;
            this.obraCivilCalcDataDb = obraCivilCalcDataDb;
            this.pemrObraCivilCalcDataDb = pEMRObraCivilCalcDataDb;

            PEMRPresupuestos = this.pemrObraCivilCalcDataDb.PEMRObraCivilCalcDatas.Select(p => p.Presupuesto).ToList();

            this.urbanizacionCalcDataDb = urbanizacionCalcDataDb;

            this.interpreter = new Interpreter(this.edificacionCalcDataDb, this.obraCivilCalcDataDb, this.urbanizacionCalcDataDb);
        }

        public async Task<HonorariosCalculationResponse> CalculateAsync(HonorariosCalculationRequest request)
        {
            switch (request) {
                case EdificationCalculationRequest r:
                    return await CalculateAsync(r);

                case CivilWorksCalculationRequest r:
                    return await CalculateAsync(r);

                case UrbanisationCalculationRequest r:
                    return await CalculateAsync(r);
            }

            throw new ArgumentException("Wrong argument type.");
        }

        public async Task<HonorariosCalculationResponse> CalculateAsync(EdificationCalculationRequest request)
        {
            HonorariosCalculationResponse response = new HonorariosCalculationResponse();

            interpreter.SetCalculationMode(CalculationMode.EDIFICACION);

            foreach (AddonInfo addon in request.SelectedAddons)
            {
                response.Responses.Add(new AddonCalculationResponse(addon.Id));
            }

            int useIdx = 0;

            foreach (EdificationUse use in request.Uses)
            {   
                if (!use.Valid)
                {
                    response.ProjectCosts.Add(0);

                    foreach (AddonInfo addon in request.SelectedAddons)
                    {
                        AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                        addonResponse.Values.Add(0);
                    }

                    continue;
                }

                KeyValuePair<int, double> floors = coefsReduccionPlantas.Where(kvp => kvp.Key <= use.RepeatedFloors).OrderByDescending(kvp => kvp.Key).FirstOrDefault();

                double floorCoef;

                if (floors.Key == 0 && !coefsReduccionPlantas.ContainsKey(0))
                    floorCoef = 1;
                else
                    floorCoef = floors.Value;

                double installationPEM = use.InstallationPEM ?? (double)(use.UnitPEM * use.Area * 0.2);
                interpreter.ClearVariables();
                interpreter.AddVariables(("PEM", (double)(use.UnitPEM * use.Area)), ("SUP", (double)use.Area), ("COP", floorCoef), ("PI", installationPEM));

                response.ProjectCosts.Add(await interpreter.GetValue(request.ProjectState.ToString()));

                foreach (AddonInfo addon in request.SelectedAddons)
                {
                    AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                    if (!addon.EnabledUses[useIdx])
                    {
                        addonResponse.Values.Add(0);
                        continue;
                    }

                    addonResponse.Values.Add(await interpreter.GetValue(addon.Id));
                }

                useIdx++;
            }

            return response;
        }

        public async Task<HonorariosCalculationResponse> CalculateAsync(CivilWorksCalculationRequest request)
        {
            HonorariosCalculationResponse response = new HonorariosCalculationResponse();

            interpreter.SetCalculationMode(CalculationMode.OBRA_CIVIL);

            foreach (AddonInfo addon in request.SelectedAddons)
            {
                response.Responses.Add(new AddonCalculationResponse(addon.Id));
            }

            ObraCivilCalcData? projectTypeFormula = await this.obraCivilCalcDataDb.ObraCivilCalcDatas.FindAsync(request.ProjectState.ToString());

            if (projectTypeFormula == null)
            {
                throw new ArgumentException("Invalid project type in request.");
            }

            int useIdx = 0;

            foreach (CivilWorksUse use in request.Uses)
            {

                if (!use.Valid)
                {
                    response.ProjectCosts.Add(0);

                    foreach (AddonInfo addon in request.SelectedAddons)
                    {
                        AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                        addonResponse.Values.Add(0);
                    }

                    continue;
                }

                double PEM = use.TotalPEM ?? (double)(use.UnitPEM * use.Area);

                int limit = this.PEMRPresupuestos.Where(x => x <= PEM).Cast<int?>().Max() ?? 0;

                PEMRObraCivilCalcData? pemr = await this.pemrObraCivilCalcDataDb.PEMRObraCivilCalcDatas.FindAsync(limit);

                double coef = pemr?.Coef ?? 1;
                double reducido = pemr?.Reducido ?? PEM;

                double CB = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CB")?.Coef ?? 1;
                double CAAP = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CAAP")?.Coef ?? 1;
                double CAMV = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CAMV")?.Coef ?? 1;
                double CAPR = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CAPR")?.Coef ?? 1;

                double CP = 0;

                switch (request.ProjectState)
                {
                    case CivilWorksProjectState.ANPP:
                        CP = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CPAP")?.Coef ?? 0;
                        break;
                    case CivilWorksProjectState.MEVP:
                        CP = this.coefsObraCivilCalcDataDb.CoefsObraCivilCalcDatas.Find("CPMV")?.Coef ?? 0;
                        break;
                }

                double HCoef = this.coefHObraCivilCalcDataDb.CoefHObraCivilCalcDatas.Find(use.Type)?.Coef ?? 1;

                interpreter.ClearVariables();
                interpreter.AddVariables(("PEM", PEM), ("LIM", limit), ("COEF", coef), ("PRES", reducido), ("CB", CB), ("CAAP", CAAP), ("CAMV", CAMV), ("CAPR", CAPR), ("CP", CP), ("H", HCoef));

                response.ProjectCosts.Add(await interpreter.GetValue(request.ProjectState.ToString()));

                foreach (AddonInfo addon in request.SelectedAddons)
                {
                    AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                    if (!addon.EnabledUses[useIdx])
                    {
                        addonResponse.Values.Add(0);
                        continue;
                    }

                    addonResponse.Values.Add(await interpreter.GetValue(addon.Id));
                }

                useIdx++;
            }

            return response;
        }

        public async Task<HonorariosCalculationResponse> CalculateAsync(UrbanisationCalculationRequest request)
        {
            HonorariosCalculationResponse response = new HonorariosCalculationResponse();

            interpreter.SetCalculationMode(CalculationMode.URBANIZACION);

            foreach (AddonInfo addon in request.SelectedAddons)
            {
                response.Responses.Add(new AddonCalculationResponse(addon.Id));
            }

            UrbanizacionCalcData? projectTypeFormula = await this.urbanizacionCalcDataDb.UrbanizacionCalcDatas.FindAsync(request.ProjectState.ToString());

            if (projectTypeFormula == null)
            {
                throw new ArgumentException("Invalid project type in request.");
            }

            int useIdx = 0;

            foreach (UrbanisationUse use in request.Uses)
            {
                if (!use.Valid)
                {
                    response.ProjectCosts.Add(0);

                    foreach (AddonInfo addon in request.SelectedAddons)
                    {
                        AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                        addonResponse.Values.Add(0);
                    }

                    continue;
                }

                double PEM = (double)(use.GreenArea * use.UnitPEMGreenArea + use.NetworkArea * use.UnitPEMNetworkArea);

                double SUP = (double)(use.GreenArea + 2 * use.NetworkArea);

                interpreter.ClearVariables();
                interpreter.AddVariables(("PEM", PEM), ("SUP", SUP), ("SUPV", (double)use.GreenArea), ("SUPN", (double)use.NetworkArea));

                response.ProjectCosts.Add(await interpreter.GetValue(request.ProjectState.ToString()));

                foreach (AddonInfo addon in request.SelectedAddons)
                {
                    AddonCalculationResponse addonResponse = response.Responses.Find(x => x.Id == addon.Id);

                    if (!addon.EnabledUses[useIdx])
                    {
                        addonResponse.Values.Add(0);
                        continue;
                    }

                    addonResponse.Values.Add(await interpreter.GetValue(addon.Id));
                }

                useIdx++;
            }

            return response;
        }
    }
}
