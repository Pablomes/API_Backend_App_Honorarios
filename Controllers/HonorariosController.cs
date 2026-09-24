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
    public class HonorariosController : ControllerBase
    {

        private readonly CalculationService calcService;
        private readonly FetchService fetchService;
        private readonly ILogger<HonorariosController> logger;

        public HonorariosController(
            CalculationService calcService, FetchService fetchService,
            ILogger<HonorariosController> logger)
        {
            this.calcService = calcService;
            this.fetchService = fetchService;
            this.logger = logger;
        }

        [HttpGet("docs/{type}")]
        public async Task<IActionResult> GetDocs(string type)
        {
            switch (type)
            {
                case "edif":
                    return Ok(await fetchService.GetEdificationDocs());
                case "obci":
                    return Ok(await fetchService.GetCivilWorksDocs());
                case "urba":
                    return Ok(await fetchService.GetUrbanisationDocs());
                default:
                    return BadRequest("Invalid document type.");
            }
        }

        [HttpGet("projects/{type}")]
        public async Task<IActionResult> GetProjects(string type)
        {
            switch (type)
            {
                case "edif":
                    return Ok(await fetchService.GetEdificationProjects());
                case "obci":
                    return Ok(await fetchService.GetCivilWorksProjects());
                case "urba":
                    return Ok(await fetchService.GetUrbanisationProjects());
                default:
                    return BadRequest("Invalid document type.");
            }
        }

        [HttpGet("")]


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
                        response = await calcService.CalculateAsync(edif);
                        break;

                    case CivilWorksCalculationRequest obci:
                        logger.LogInformation("Calculation 'honorarios' value for CivilWorksCalculationRequest.");
                        response = await calcService.CalculateAsync(obci);
                        break;

                    case UrbanisationCalculationRequest urba:
                        logger.LogInformation("Calculation 'honorarios' value for UrbanisationCalculationRequest.");
                        response = await calcService.CalculateAsync(urba);
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
