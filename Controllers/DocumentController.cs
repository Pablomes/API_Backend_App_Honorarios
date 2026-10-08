using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Pdf;
using API_Backend_App_Honorarios.Services;
using API_Backend_App_Honorarios.Tests;
using API_Backend_App_Honorarios.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using PdfSharp.Pdf;

namespace API_Backend_App_Honorarios.Controllers
{
    [ApiController]
    [Route("honorariosDoc")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly ProjectPersistenceService _projectPersistence;
        private readonly CalculationService calculationService;
        private readonly FetchService fetchService;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IConfiguration configuration;

        public DocumentController(ILogger<DocumentController> logger, ProjectPersistenceService persistenceService, CalculationService calculationService, FetchService fetchService, IAzureClientFactory<BlobServiceClient> clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _projectPersistence = persistenceService;
            blobServiceClient = clientFactory.CreateClient("DevBlobStorage");
            this.configuration = configuration;
            this.calculationService = calculationService;
            this.fetchService = fetchService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> GenerateTestPDF()
        {
            var (testRequest, testResults) = DocRequestTestFactory.CreateEdifTestData();

            string fakeUuid = "Test-123456789";
            DateTime fakeDate = DateTime.Now;

            HonorariosCalculationResponse response = new HonorariosCalculationResponse { ProjectCosts = new List<double>(), Responses = testResults };

            HonorariosDoc documento = new HonorariosDoc(testRequest, response, fakeUuid, fakeDate, this.fetchService);

            PdfDocument? doc = await documento.GeneratePdf();

            if (doc == null) return Problem("Error al generar el PDF de prueba");

            using MemoryStream memStream = new MemoryStream();
            doc.Save(memStream, false);
            doc.Close();
            doc.Dispose();

            memStream.Position = 0;

            return File(memStream.ToArray(), "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult?> GeneratePDF([FromBody] DocRequest request)
        {
            _logger.LogInformation("Iniciando generación de PDF de Honorarios");

            HonorariosCalculationResponse response = await calculationService.CalculateAsync(request.CalculationRequest, true);

            ProyectoHonorario proyectoRegistrado = await _projectPersistence.RegisterProjectAsync(request.ProjectType, request.ActuationId);

            HonorariosDocVertical documento = new HonorariosDocVertical(request, response, proyectoRegistrado.CVE, proyectoRegistrado.FechaHoraCreacion, fetchService);
            PdfDocument? doc = await documento.GeneratePdf();

            if (doc == null) return Problem("Error al generar PDF");

            using MemoryStream memStream = new MemoryStream();
            doc.Save(memStream, false);
            doc.Close();
            doc.Dispose();
            memStream.Position = 0;

            string containerName = configuration["BlobContainerName"] ?? "honorarios-pdf";
            string blobName = $"{proyectoRegistrado.CVE}.pdf";

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);

            try
            {
                await blobClient.UploadAsync(memStream);
            }
            catch (Exception e)
            {
                return Problem(
                    title: "Internal Server Error",
                    detail: "An unexpected error occured.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return File(memStream.ToArray(), "application/pdf", $"Honorarios_{proyectoRegistrado.CVE}.pdf");
        }

        [HttpGet("{cve}")]
        public async Task<IActionResult> GetExistingProject(string cve)
        {
            if (!(await this._projectPersistence.checkProjectExists(cve)))
            {
                _logger.LogWarning($"Project {cve} not found in database.");
                return NotFound($"Documento {cve} no se ha encontrado.");
            }

            string containerName = configuration["BlobContainerName"] ?? "honorarios-pdf";
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient($"{cve}.pdf");

            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("Document {DocumentCode} not found in Blob Storage.", cve);
                return NotFound($"Documento {cve} no se ha encontrado.");
            }

            var downloadInfo = await blobClient.DownloadStreamingAsync();
            return File(downloadInfo.Value.Content, "application/pdf", $"{cve}.pdf");
        }
    }
}    