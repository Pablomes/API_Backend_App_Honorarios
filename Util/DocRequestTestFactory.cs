using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Models;
using System.Collections.Generic;

namespace API_Backend_App_Honorarios.Tests
{
    public static class DocRequestTestFactory
    {
        public static (DocRequest request, List<AddonCalculationResponse> simulatedResults) CreateEdifTestData()
        {
            var request = new DocRequest
            {
                // Prueba a cambiar a "OBCI" o "URBA" para ver las distintas tablas
                ProjectType = "EDIF",
                ActuationId = "Construcción de edificio de 20 viviendas en Calle Ficticia 123, Valencia",
                CalculationRequest = new EdificationCalculationRequest
                {
                    ProjectState = EdificationProjectState.PBED,
                    SelectedAddons = new List<AddonInfo> {
                        new AddonInfo { Id = "Proyecto básico", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio previo", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Anteproyecto", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de impacto ambiental", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de integración paisajística", EnabledUses = new List<bool>{true, true} },
                    },
                    Uses = new List<EdificationUse>
                    {
                        new EdificationUse { RepeatedFloors = 3, Name = "Plantas de vivienda", Area = 8000, UnitPEM = 1000, InstallationPEM = 200000 },
                        new EdificationUse { RepeatedFloors = 0, Name = "Sótano de aparcamiento", Area = 1000, UnitPEM = 500}
                    }
                }
            };

            var simulatedResults = new List<AddonCalculationResponse>
            {
                new AddonCalculationResponse { Id = "Proyecto básico", Values = new List<double>([15450.00, 15450.00])},
                new AddonCalculationResponse { Id = "Estudio previo", Values = new List<double>([0, 15450.00]) },
                new AddonCalculationResponse { Id = "Anteproyecto", Values = new List<double>([15450.00, 0]) },
                new AddonCalculationResponse { Id = "Estudio de impacto ambiental", Values = new List<double>([15450.00, 15450.00]) },
                new AddonCalculationResponse { Id = "Estudio de integración paisajística", Values = new List<double>([0, 15450.00]) }
            };

            return (request, simulatedResults);
        }

        public static (DocRequest request, List<AddonCalculationResponse> simulatedResults) CreateObciTestData()
        {
            var request = new DocRequest
            {
                // Prueba a cambiar a "OBCI" o "URBA" para ver las distintas tablas
                ProjectType = "OBCI",
                ActuationId = "Construcción de edificio de 20 viviendas en Calle Ficticia 123, Valencia",
                CalculationRequest = new CivilWorksCalculationRequest
                {
                    ProjectState = CivilWorksProjectState.PRCO,
                    SelectedAddons = new List<AddonInfo> {
                        new AddonInfo { Id = "Proyecto básico", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio previo", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Anteproyecto", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de impacto ambiental", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de integración paisajística", EnabledUses = new List<bool>{true, true} },
                    },
                    Uses = new List<CivilWorksUse>
                    {
                        new CivilWorksUse { Area = 6000, UnitPEM = 1000, TotalPEM = 6000000, Type = "Puentes y viaductos" },
                        new CivilWorksUse { Area = 825, UnitPEM = 750, TotalPEM = 618750, Type = "Depuradoras (EDAR)" }
                    }
                }
            };

            var simulatedResults = new List<AddonCalculationResponse>
            {
                new AddonCalculationResponse { Id = "Proyecto básico", Values = new List<double>([15450.00])},
                new AddonCalculationResponse { Id = "Estudio previo", Values = new List<double>([0]) },
                new AddonCalculationResponse { Id = "Anteproyecto", Values = new List<double>([15450.00]) },
                new AddonCalculationResponse { Id = "Estudio de impacto ambiental", Values = new List<double>([15450.00]) },
                new AddonCalculationResponse { Id = "Estudio de integración paisajística", Values = new List<double>([0]) }
            };

            return (request, simulatedResults);
        }

        public static (DocRequest request, List<AddonCalculationResponse> simulatedResults) CreateUrbaTestData()
        {
            var request = new DocRequest
            {
                // Prueba a cambiar a "OBCI" o "URBA" para ver las distintas tablas
                ProjectType = "URBA",
                ActuationId = "Construcción de edificio de 20 viviendas en Calle Ficticia 123, Valencia",
                CalculationRequest = new UrbanisationCalculationRequest
                {
                    ProjectState = UrbanisationProjectState.PROY,
                    SelectedAddons = new List<AddonInfo> {
                        new AddonInfo { Id = "Proyecto básico", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio previo", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Anteproyecto", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de impacto ambiental", EnabledUses = new List<bool>{true, true} },
                        new AddonInfo { Id = "Estudio de integración paisajística", EnabledUses = new List<bool>{true, true} },
                    },
                    Uses = new List<UrbanisationUse>
                    {
                        new UrbanisationUse { Name = "Calle principal", GreenArea = 500, NetworkArea = 18000, UnitPEMGreenArea = 750, UnitPEMNetworkArea = 1000 },
                        new UrbanisationUse { Name = "Jardin", GreenArea = 8000, NetworkArea = 100, UnitPEMGreenArea = 750, UnitPEMNetworkArea = 1000 }
                    }
                }
            };

            var simulatedResults = new List<AddonCalculationResponse>
            {
                new AddonCalculationResponse { Id = "Proyecto básico", Values = new List<double>([15450.00])},
                new AddonCalculationResponse { Id = "Estudio previo", Values = new List<double>([0]) },
                new AddonCalculationResponse { Id = "Anteproyecto", Values = new List<double>([15450.00]) },
                new AddonCalculationResponse { Id = "Estudio de impacto ambiental", Values = new List<double>([15450.00]) },
                new AddonCalculationResponse { Id = "Estudio de integración paisajística", Values = new List<double>([0]) }
            };

            return (request, simulatedResults);
        }
    }
}