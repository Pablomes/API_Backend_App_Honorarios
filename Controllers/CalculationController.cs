using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Services;

namespace API_Backend_App_Industrializacion.Controllers
{
    [ApiController]
    [Route("honorarios")]
    public class CalculationController : ControllerBase
    {

        private readonly CalculationService service;
        private readonly ILogger<CalculationController> logger;

        public CalculationController(
            CalculationService service,
            ILogger<CalculationController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<HonorariosCalculationResponse>> calculateValues([FromBody] HonorariosCalculationRequest request)
        {
            if (request == null) return BadRequest("No body in POST request.");

            HonorariosCalculationResponse response = null;

            try
            {
                switch (request)
                {
                    case EdificationCalculationRequest edif:
                        logger.LogInformation("Calculation 'honorarios' value for EdificationCalculationRequest.");
                        response = await service.CalculateAsync(edif);
                        break;

                    case CivilWorksCalculationRequest obci:
                        logger.LogInformation("Calculation 'honorarios' value for CivilWorksCalculationRequest.");
                        response = await service.CalculateAsync(obci);
                        break;

                    case UrbanisationCalculationRequest urba:
                        logger.LogInformation("Calculation 'honorarios' value for UrbanisationCalculationRequest.");
                        response = await service.CalculateAsync(urba);
                        break;
                }

                if (response == null) throw new ArgumentException("Invalid type of request.");

                return response;
            } 
            catch (ArgumentException e)
            {
                logger.LogWarning(e, "Invalid calculation request.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                logger.LogWarning(e, "Invalid variable name in request or derived formulae.");
                return BadRequest(e.Message);
            }

            return BadRequest("Unsupported calculation request type.");
        }
    }
}
