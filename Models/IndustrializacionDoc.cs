using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace API_Backend_App_Industrializacion.Models
{
    public class IndustrializacionDoc
    {
        /*

        private static readonly XColor PrimaryColour = XColor.FromArgb(80, 129, 79);
        private static readonly XColor SecondaryColour = XColor.FromArgb(240, 240, 240);
        private static readonly XColor DarkerSecondaryColour = XColor.FromArgb(85, 85, 85);
        private static readonly XColor TextColour = XColor.FromArgb(51, 51, 51);
        private static readonly XColor BackgroundColour = XColor.FromArgb(255, 255, 255);

        private readonly XFont TitleFont = new XFont("Montserrat", 12, XFontStyleEx.Bold); // 9pt / 0.75
        private readonly XFont HeaderFont = new XFont("Montserrat", 6, XFontStyleEx.Bold); 
        private readonly XFont SubtitleFont = new XFont("Montserrat", 10.6, XFontStyleEx.Bold); // 8pt / 0.75
        private readonly XFont SubdonutFont = new XFont("Montserrat", 9.3, XFontStyleEx.Bold);
        private readonly XFont SectionFont = new XFont("Montserrat", 9.3, XFontStyleEx.Bold); // 7pt / 0.75 (IGUAL 8pt, NO SE...)
        private readonly XFont RegularFont = new XFont("Montserrat", 7, XFontStyleEx.Regular); 
        private readonly XFont HeaderCategoryFont = new XFont("Montserrat", 6, XFontStyleEx.Regular); 
        private readonly XFont BigWheelFont = new XFont("Montserrat", 15.1, XFontStyleEx.Bold); // 11.3 / 0.75
        private readonly XFont MediumWheelFont = new XFont("Montserrat", 14, XFontStyleEx.Bold); // 10.5 / 0.75
        private readonly XFont SmallWheelFont = new XFont("Montserrat", 9.3, XFontStyleEx.Bold); // 7 / 0.75

        private XGraphics graphics = null!;
        private PdfPage page = null!;

        private double PageWidth => page.Width.Point;
        private double PageHeight => page.Height.Point;

        private double leftMargin = 0.0;
        private double footHeight = 59.535;

        private string[] sectionTitles = { "Cimentación", "Estructura", "Envolvente", "Distribución interior" };
        private string[] sectionID = { "CIME", "ESTR", "ENVO", "DINT" };
        private List<string>[] subsectionNames = { new List<string>() { "Zapatas Aisladas y/o corridas", "Muros de contención" },
                                                    new List<string>() { "Sistema estructural", "Escaleras", "Caja del ascensor" },
                                                    new List<string>() { "Fachadas", "Cubiertas" },
                                                    new List<string>() { "Módulos 3D no estructurales", "Cuartos húmedos o de servicios 3D/2D", "Particiones interiores" } };
        private List<string>[] subsectionID = { new List<string>() { "ZAPA", "MUCO" },
                                                    new List<string>() { "SEST", "ESCA", "ASCE" },
                                                    new List<string>() { "FACH", "CUBI" },
                                                    new List<string>() { "3DNE", "CHSV", "PAIN" } };


        private DocParameters? Parameters;

        private string CVECode;
        private DateTime genTime;

        public IndustrializacionDoc(string CVECode, DateTime genTime)
        {
            this.CVECode = CVECode;
            this.genTime = genTime;
        }

        public IndustrializacionDoc(DocParameters parameters, string CVECode, DateTime genTime) : this(CVECode, genTime)
        {
            this.Parameters = parameters;
        }

        public PdfDocument? generatePdf()
        {
            if (Parameters == null) return null;

            PdfDocument doc = new PdfDocument();

            page = doc.AddPage();

            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Landscape;

            graphics = XGraphics.FromPdfPage(page);

            DrawHeader();

            DrawGlobalDonut();

            DrawSubDonuts();

            DrawInformationTable();

            DrawFoot();

            return doc;
        }


        private void DrawHeader()
        {
            const double headerHeight = 42.525;
            const double headerWidth = 779.625;

            const double headerVerticalPos = 26;
            double headerHorizontalPos = (PageWidth - headerWidth) / 2;

            leftMargin = headerHorizontalPos;

            const double textMargin = 11.4;

            const double borderWidth = 1;
            const double separatorWidth = 0.3;

            const double separatorRatio = 0.41; // AJUSTAR A GUSTO PARA DAR MAS ESPACIO A LOS METADATOS

            double headerLeftWidth = headerWidth * separatorRatio;
            double headerRightWidth = headerWidth - headerLeftWidth;
            double headerDivisionPoint = headerHorizontalPos + headerLeftWidth;

            // BORDE DE LA CABECERA
            graphics.DrawRectangle(new XPen(PrimaryColour, borderWidth), headerHorizontalPos, headerVerticalPos, headerWidth, headerHeight);

            // TITULO DEL DOCUMENTO
            graphics.DrawString(
                "NIVEL DE INDUSTRIALIZACIÓN DEL PROYECTO",
                TitleFont,
                new XSolidBrush(TextColour),
                new XRect(headerHorizontalPos + textMargin, headerVerticalPos + (headerHeight - TitleFont.Size) / 2, headerLeftWidth - textMargin, TitleFont.Size),
                XStringFormats.CenterLeft
            );

            // SEPARADOR
            graphics.DrawLine(new XPen(PrimaryColour, separatorWidth), headerDivisionPoint, headerVerticalPos + textMargin, headerDivisionPoint, headerVerticalPos + headerHeight - textMargin);

            // METADATOS EN LA CABECERA
            XSolidBrush categoryBrush = new XSolidBrush(DarkerSecondaryColour);
            XSolidBrush textBrush = new XSolidBrush(TextColour);

            double leftCategoryWidth = 32.6;
            double rightCategoryWidth = 37;

            graphics.DrawString(
                "Proyecto: ",
                HeaderCategoryFont,
                categoryBrush,
                new XRect(headerDivisionPoint + textMargin, headerVerticalPos + textMargin, leftCategoryWidth, HeaderCategoryFont.Size),
                XStringFormats.CenterLeft//CenterRight
            );

            graphics.DrawString(
                "Ubicación: ",
                HeaderCategoryFont,
                categoryBrush,
                new XRect(headerDivisionPoint + textMargin, headerVerticalPos + headerHeight - textMargin - HeaderCategoryFont.Size, leftCategoryWidth, HeaderCategoryFont.Size),
                XStringFormats.CenterLeft//CenterRight
            );

            graphics.DrawString(
                "Promotor: ",
                HeaderCategoryFont,
                categoryBrush,
                new XRect(headerDivisionPoint + headerRightWidth / 2 + textMargin / 2, headerVerticalPos + textMargin, rightCategoryWidth, HeaderCategoryFont.Size),
                XStringFormats.CenterLeft//CenterRight
            );

            graphics.DrawString(
                "Proyectista: ",
                HeaderCategoryFont,
                categoryBrush,
                new XRect(headerDivisionPoint + headerRightWidth / 2 + textMargin / 2, headerVerticalPos + headerHeight - textMargin - HeaderCategoryFont.Size, rightCategoryWidth, HeaderCategoryFont.Size),
                XStringFormats.CenterLeft//CenterRight
            );

            graphics.DrawString(
                this.Parameters?.ProjectName ?? "",
                HeaderFont,
                textBrush,
                new XRect(headerDivisionPoint + textMargin + leftCategoryWidth, headerVerticalPos + textMargin, headerRightWidth / 2 - (3/2) * textMargin - leftCategoryWidth, HeaderFont.Size),
                XStringFormats.CenterLeft
            );

            graphics.DrawString(
                this.Parameters?.Location ?? "",
                HeaderFont,
                textBrush,
                new XRect(headerDivisionPoint + textMargin + leftCategoryWidth, headerVerticalPos + headerHeight - textMargin - HeaderFont.Size, headerRightWidth / 2 - (3 / 2) * textMargin - leftCategoryWidth, HeaderFont.Size),
                XStringFormats.CenterLeft
            );

            graphics.DrawString(
                this.Parameters?.Developer ?? "",
                HeaderFont,
                textBrush,
                new XRect(headerDivisionPoint  + headerRightWidth / 2 + textMargin / 2 + rightCategoryWidth, headerVerticalPos + textMargin, headerRightWidth / 2 - (3 / 2) * textMargin - rightCategoryWidth, HeaderFont.Size),
                XStringFormats.CenterLeft
            );

            graphics.DrawString(
                this.Parameters?.Projector ?? "",
                HeaderFont,
                textBrush,
                new XRect(headerDivisionPoint + headerRightWidth / 2 + textMargin / 2 + rightCategoryWidth, headerVerticalPos + headerHeight - textMargin - HeaderFont.Size, headerRightWidth / 2 - (3 / 2) * textMargin - rightCategoryWidth, HeaderFont.Size),
                XStringFormats.CenterLeft
            );
        }

        private void drawProgressDonut(double centerX, double centerY, double radius, double value, XFont font)
        {
            double innerRadius = radius * 0.75;

            XRect donutRect = new XRect(centerX - radius, centerY - radius, 2 * radius, 2 * radius);
            XRect tinyRect = new XRect(centerX - innerRadius, centerY - innerRadius, 2 * innerRadius, 2 * innerRadius);

            // CIRCULO DE FONDO
            graphics.DrawEllipse(new XSolidBrush(SecondaryColour), donutRect);
            //graphics.DrawArc(new XPen(SecondaryColour, radius - innerRadius), donutRect, -90, 360);

            // CIRCULO DE PROGRESO
            if (value > 0)
                graphics.DrawPie(new XSolidBrush(PrimaryColour), donutRect, -90, value / 100.0 * 360.0);
            //graphics.DrawArc(new XPen(PrimaryColour, radius - innerRadius), donutRect, -90, value / 100.0 * 360.0);

            // CIRCULO MASCARA
            graphics.DrawEllipse(new XSolidBrush(BackgroundColour), tinyRect);

            // LABEL CON EL VALOR
            graphics.DrawString(
                $"{value.ToString("0.0", new CultureInfo("es-ES"))}%",
                font,
                new XSolidBrush(TextColour),
                donutRect,
                XStringFormats.Center
            );
        }

        private void DrawGlobalDonut()
        {
            double titleYPos = 121.9;

            graphics.DrawString(
                "NIVEL DE INDUSTRIALIZACIÓN",
                SubtitleFont,
                new XSolidBrush(TextColour),
                new XRect(leftMargin, titleYPos, PageWidth / 2 - leftMargin, SubtitleFont.Size),
                XStringFormats.Center
            );

            double centerX = (PageWidth - 2 * leftMargin) / 4 + leftMargin;
            double centerY = 232.47;
            double radius = 66.62;


            drawProgressDonut(centerX, centerY, radius, this.Parameters?.TotalIndustrializacion ?? 0, BigWheelFont);

        }

        private void DrawSubDonuts()
        {
            double titleYPos = PageHeight - 232.47;

            double titleWidth = (PageWidth / 2 - leftMargin) / 2;

            graphics.DrawString(
                "Componentes prefabricados",
                SubdonutFont,
                new XSolidBrush(TextColour),
                new XRect(leftMargin, titleYPos, titleWidth, SubdonutFont.Size),
                XStringFormats.Center
            );

            graphics.DrawString(
                "Reducción de tiempo y medios",
                SubdonutFont,
                new XSolidBrush(TextColour),
                new XRect(leftMargin + titleWidth, titleYPos, titleWidth, SubdonutFont.Size),
                XStringFormats.Center
            );

            double radius = 48.195;

            drawProgressDonut(leftMargin + titleWidth / 2, PageHeight + radius - 201.285, radius, this.Parameters?.CompPrefabricados ?? 0, MediumWheelFont);

            drawProgressDonut(leftMargin + 3 * titleWidth / 2, PageHeight + radius - 201.285, radius, this.Parameters?.ReduccionTiempo ?? 0, MediumWheelFont);
        }

        private void DrawFoot()
        {

            // LINEA DE SEPARACION DEL PIE
            graphics.DrawLine(new XPen(PrimaryColour), leftMargin, PageHeight - footHeight, PageWidth - leftMargin, PageHeight - footHeight);

            const string resourceName = "API_Backend_App_Industrializacion.Materials.Images.LogoIVEPDF.png"; 

            Assembly assembly = typeof(IndustrializacionDoc).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}");

            using MemoryStream memStream = new MemoryStream();
            stream.CopyTo(memStream);
            memStream.Position = 0;

            using XImage logo = XImage.FromStream(memStream);

            const double imageModifier = 0.6;
            double imageMargin = (1 - imageModifier) * footHeight / 2;
            double imageLeftMargin = 10;

            double imageHeight = footHeight * imageModifier;
            double imageWidth = (imageHeight / logo.PixelHeight) * logo.PixelWidth;

            graphics.DrawImage(logo, new XRect(leftMargin + imageLeftMargin, PageHeight - footHeight + imageMargin, imageWidth, imageHeight));

            string dateString = "DD/MM/YYYY";

            dateString = $"{genTime.Day: 00}/{genTime.Month: 00}/{genTime.Year: 0000}";

            graphics.DrawString(
            dateString,
            RegularFont,
            new XSolidBrush(TextColour),
            new XRect(PageWidth / 2, PageHeight - footHeight + 2 * imageMargin, PageWidth / 2 - leftMargin, RegularFont.Size),
            XStringFormats.CenterRight
            );

            string codeString = $"CVE: {CVECode}";

            graphics.DrawString(
            codeString,
            RegularFont,
            new XSolidBrush(TextColour),
            new XRect(PageWidth / 2, PageHeight - 1.5 * imageMargin - RegularFont.Size, PageWidth / 2 - leftMargin, RegularFont.Size),
            XStringFormats.CenterRight
            );
        }

        private void DrawInformationTable()
        {
            double titleYPos = 121.9;
            double titleXOffset = 31.185;

            graphics.DrawString(
                "DETALLE POR CAPÍTULOS",
                SubtitleFont,
                new XSolidBrush(TextColour),
                new XRect(PageWidth / 2 + titleXOffset, titleYPos, PageWidth / 2, SubtitleFont.Size),
                XStringFormats.CenterLeft
            );

            double verticalMargin = 32.6025;
            double verticalPadding = 17.01;

            double availableSectionHeight = PageHeight - footHeight - titleYPos - SubtitleFont.Size - verticalMargin * 2;

            double sectionHeight = (availableSectionHeight - 3 * verticalPadding) / 4;
            double sectionWidth = 218.295;

            for (int i = 0; i < 4; i++)
            {
                DrawSection(i + 1, sectionTitles[i], sectionID[i], subsectionNames[i], subsectionID[i],
                    PageWidth / 2 + titleXOffset, titleYPos + SubtitleFont.Size + verticalMargin + (sectionHeight + verticalPadding) * i,
                    sectionWidth, sectionHeight, PageWidth / 2 - titleXOffset - leftMargin);
            }

            // PROGRESS WHEEL TITLES
            double wheelTitleMargin = verticalMargin - 11.34;

            double wheelTitleWidth = (PageWidth / 2 - leftMargin - titleXOffset - sectionWidth) / 2;

            /* Componentes prefabricados */

        /*
            graphics.DrawString(
                "Componentes",
                RegularFont,
                new XSolidBrush(PrimaryColour),
                new XRect(PageWidth / 2 + titleXOffset + sectionWidth, titleYPos + SubtitleFont.Size + wheelTitleMargin, wheelTitleWidth, RegularFont.Size),
                XStringFormats.Center
            );

            graphics.DrawString(
                "prefabricados",
                RegularFont,
                new XSolidBrush(PrimaryColour),
                new XRect(PageWidth / 2 + titleXOffset + sectionWidth, titleYPos + SubtitleFont.Size + wheelTitleMargin + RegularFont.Size, wheelTitleWidth, RegularFont.Size),
                XStringFormats.Center
            );

            /**/

            /* Reduccion de tiempos y medios */

        /*
            graphics.DrawString(
                "Reducción de",
                RegularFont,
                new XSolidBrush(PrimaryColour),
                new XRect(PageWidth / 2 + titleXOffset + sectionWidth + wheelTitleWidth, titleYPos + SubtitleFont.Size + wheelTitleMargin, wheelTitleWidth, RegularFont.Size),
                XStringFormats.Center
            );

            graphics.DrawString(
                "tiempos y medios",
                RegularFont,
                new XSolidBrush(PrimaryColour),
                new XRect(PageWidth / 2 + titleXOffset + sectionWidth + wheelTitleWidth, titleYPos + SubtitleFont.Size + wheelTitleMargin + RegularFont.Size, wheelTitleWidth, RegularFont.Size),
                XStringFormats.Center
            );
        }

        private void DrawSection(int sectionNum, string title, string ID, List<string> subsectionTitles, List<string> subsectionID, double topLeftX, double topLeftY, double width, double height, double fullWidth)
        {
            double verticalMargin = 2.835;

            double subsectionHorizontalMargin = 14.175;
            double subsectionVerticalMargin = 8.505;

            graphics.DrawString(
                $"{sectionNum}. {title}",
                SectionFont,
                new XSolidBrush(TextColour),
                new XRect(topLeftX, topLeftY + verticalMargin, width, SectionFont.Size),
                XStringFormats.CenterLeft
            );

            double subsectionTopX = topLeftX + subsectionHorizontalMargin;
            

            double subsectionWidth = width - 2 * subsectionHorizontalMargin;
            double subsectionHeight = (height - subsectionVerticalMargin * subsectionTitles.Count - verticalMargin - SectionFont.Size) / subsectionTitles.Count;

            for (int i = 0; i < subsectionTitles.Count; i++)
            {
                double subsectionTopY = topLeftY + SectionFont.Size + subsectionVerticalMargin * (i + 1) + subsectionHeight * i;

                DrawSubsection(sectionNum, i + 1, subsectionTitles[i], ID, subsectionID[i], subsectionTopX, subsectionTopY, subsectionWidth, subsectionHeight);
            }

            DrawSectionWheels(ID, topLeftX + width, topLeftY + SectionFont.Size + subsectionVerticalMargin, fullWidth - width, height - verticalMargin - SectionFont.Size);
        }

        private void DrawSubsection(int sectionNum, int subsectionNum, string title, string sectionID, string ID, double topLeftX, double topLeftY, double width, double height)
        {
            double leftMargin = 2.835;
            double valueWidth = 34.02;

            graphics.DrawString(
                $"{sectionNum}.{subsectionNum} {title}",
                RegularFont,
                new XSolidBrush(TextColour),
                new XRect(topLeftX + leftMargin, topLeftY, width - valueWidth, height),
                XStringFormats.CenterLeft
            );

            string labelValue = "-";

            if (this.Parameters != null && this.Parameters.SubsectionLabelValues != null && this.Parameters.SubsectionLabelValues.ContainsKey(sectionID))
            {
                if (this.Parameters.SubsectionLabelValues[sectionID].ContainsKey(ID))
                {
                    labelValue = this.Parameters.SubsectionLabelValues[sectionID][ID];
                }
            }

            graphics.DrawString(
                $"{labelValue}",
                RegularFont,
                new XSolidBrush(PrimaryColour),
                new XRect(topLeftX + width - valueWidth, topLeftY, valueWidth, height),
                XStringFormats.Center
            );

            double lineSeparation = 2.835;

            double linePosY = topLeftY + height / 2 + RegularFont.Size / 2 + lineSeparation;

            graphics.DrawLine(new XPen(SecondaryColour, 1), topLeftX, linePosY, topLeftX + width, linePosY);
        }

        private void DrawSectionWheels(string sectionID, double topX, double topY, double width, double height)
        {
            double radius = 25.515;

            double donutWidth = width / 2;

            double prefabricadosValue = 0.0;
            double reduccionTiempoValue = 0.0;

            if (this.Parameters?.SectionCompPrefabricados.ContainsKey(sectionID) ?? false)
                prefabricadosValue = this.Parameters.SectionCompPrefabricados[sectionID];

            if (this.Parameters?.SectionReduccionTiempo.ContainsKey(sectionID) ?? false)
                reduccionTiempoValue = this.Parameters.SectionReduccionTiempo[sectionID];

            drawProgressDonut(topX + donutWidth / 2, topY + height / 2, radius, prefabricadosValue, SmallWheelFont);

            drawProgressDonut(topX + 3 * donutWidth / 2, topY + height / 2, radius, reduccionTiempoValue, SmallWheelFont);
        }

        */
    }
}
