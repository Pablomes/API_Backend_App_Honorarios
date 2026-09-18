using API_Backend_App_Industrializacion.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;

namespace API_Backend_App_Industrializacion.Controllers
{
    [ApiController]
    [Route("industrializacionDoc")]
    public class DocumentController : ControllerBase
    {
        /*

        private readonly ILogger<DocumentController> logger;
        private readonly ProjectPersistenceService projectPersistence;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IConfiguration configuration;

        public DocumentController(ILogger<DocumentController> logger, ProjectPersistenceService persistenceService, IAzureClientFactory<BlobServiceClient> clientFactory, IConfiguration configuration)
        {
            this.logger = logger;
            this.projectPersistence = persistenceService;
            this.blobServiceClient = clientFactory.CreateClient("DevBlobStorage");
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult?> generatePDF([FromBody] DocRequest request)
        {
            logger.LogInformation("Generating industrializacion PDF");

            Proyecto? proyectoRegistrado = await projectPersistence.registerProject(request.ProjectName, request.Location, request.Developer, request.Projector);

            if (proyectoRegistrado == null)
            {
                logger.LogError("Industrializacion project registration returned null");
                return Problem(
                    title: "Internal Server Error",
                    detail: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            IndustrializacionDoc documento = new IndustrializacionDoc(new DocParameters(request), proyectoRegistrado.CVE, proyectoRegistrado.FechaHoraCreacion);

            PdfDocument? doc = documento.generatePdf();

            if (doc == null)
            {
                logger.LogError("Industrializacion PDF generation returned null");
                return Problem(
                    title: "Internal Server Error",
                    detail: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            using MemoryStream memStream = new MemoryStream();
            doc.Save(memStream, false);
            doc.Close();

            doc.Dispose();

            memStream.Position = 0;

            string containerName = configuration["BlobContainerName"] ?? "pdf-documents";
            string blobName = $"{proyectoRegistrado.CVE}.pdf";

            var containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);

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
                    detail: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return File(
                fileContents: memStream.ToArray(),
                contentType: "application/pdf",
                fileDownloadName: "industrializacion_proyecto.pdf"
            );

        }

        [HttpGet("{cve}")]
        public async Task<IActionResult> GetDocument(string cve)
        {
            if (!(await this.projectPersistence.checkProjectExists(cve)))
            {
                logger.LogWarning($"Project {cve} not found in database.");
                return NotFound($"Documento {cve} no se ha encontrado.");
            }

            string containerName = configuration["BlobContainerName"] ?? "pdf-documents";
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient($"{cve}.pdf");

            if (!await blobClient.ExistsAsync())
            {
                logger.LogWarning("Document {DocumentCode} not found in Blob Storage.", cve);
                return NotFound($"Documento {cve} no se ha encontrado.");
            }

            var downloadInfo = await blobClient.DownloadStreamingAsync();
            return File(downloadInfo.Value.Content, "application/pdf", $"{cve}.pdf");
        }
    */
        
    }
}
