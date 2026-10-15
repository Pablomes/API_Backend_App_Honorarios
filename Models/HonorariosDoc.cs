using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Persistence;
using API_Backend_App_Honorarios.Services;
using API_Backend_App_Industrializacion.Models;
using Microsoft.AspNetCore.Identity;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace API_Backend_App_Honorarios.Pdf
{
    public class HonorariosDoc
    {

        private static readonly XColor PrimaryColor = XColor.FromArgb(0, 154, 135);
        private static readonly XColor SecondaryBg = XColor.FromArgb(207, 207, 207);
        private static readonly XColor TextColor = XColor.FromArgb(51, 51, 51);
        private static readonly XColor DarkerSecondaryColor = XColor.FromArgb(85, 85, 85);
        private static readonly XColor BorderColor = XColor.FromArgb(220, 220, 220);
        private static readonly XColor TableHeaderColor = XColor.FromArgb(231, 230, 230);
        private static readonly XColor BackgroundColor = XColor.FromArgb(250, 250, 250);

        private readonly XFont TitleFont = new XFont("Montserrat", 14, XFontStyleEx.Bold);
        private readonly XFont TabFont = new XFont("Montserrat", 12, XFontStyleEx.Bold);
        private readonly XFont SectionFont = new XFont("Montserrat", 11, XFontStyleEx.Bold);
        private readonly XFont RegularFont = new XFont("Montserrat", 9, XFontStyleEx.Regular);
        private readonly XFont TableHeaderFont = new XFont("Montserrat", 8, XFontStyleEx.Bold);
        private readonly XFont CellFont = new XFont("Montserrat", 8, XFontStyleEx.Regular);
        private readonly XFont SummaryValueFont = new XFont("Montserrat", 10, XFontStyleEx.Bold);
        private readonly XFont SummaryTotalFont = new XFont("Montserrat", 18, XFontStyleEx.Bold);

        public static readonly CultureInfo esES = CultureInfo.GetCultureInfo("es-ES");

        //double[] usesTableLeftCoef = { 0, 0.32, 0.45, 0.57, 0.69, 0.81 };
        double[] usesTableLeftCoef = { 0, 0.3, 0.44, 0.58, 0.72, 0.86 };

        private const double SummaryTotalMaxFontSize = 18;
        private const double SummaryTotalMinFontSize = 6;

        private XGraphics graphics = null!;
        private XTextFormatter textFormat = null;
        private PdfPage page = null!;
        private double margin = 30;
        private double tableWidth;
        private double rightBoxWidth = 180;
        private double gap = 25;
        private double footHeight = 50;
        private const double SummaryRowLineHeight = 12;

        private DocRequest request;
        private HonorariosCalculationResponse response;
        private string CVECode;
        private DateTime genTime;

        private FetchService fetchService;

        public HonorariosDoc(DocRequest request, HonorariosCalculationResponse response, string CVECode, DateTime genTime, FetchService fetchService)
        {
            this.request = request;
            this.response = response;
            this.CVECode = CVECode;
            this.genTime = genTime;
            this.fetchService = fetchService;
        }

        public async Task<PdfDocument> GeneratePdf()
        {
            PdfDocument doc = new PdfDocument();
            page = doc.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Portrait;
            graphics = XGraphics.FromPdfPage(page);
            textFormat = new XTextFormatter(graphics);

            tableWidth = page.Width.Point - (margin * 2);// - rightBoxWidth - gap;

            double nextY = DrawMainHeader();
            nextY = DrawActuationDataSection(nextY + 10);

            nextY = DrawUsesTable(nextY + 20);

            nextY = await DrawServiceContentSection(nextY + 30);

            await DrawSummaryBox(nextY + 20);

            DrawFooter();

            return doc;
        }

        private double DrawMainHeader()
        {
            double currentY = margin;

            XStringFormat titleFormat = new XStringFormat();
            titleFormat.LineAlignment = XLineAlignment.Near;
            titleFormat.Alignment = XStringAlignment.Near;
            textFormat.DrawString("Estimación del coste de la prestación de servicios de arquitectura e ingeniería", TitleFont, new XSolidBrush(TextColor), new XRect(new XPoint(margin, currentY), new XPoint(page.Width.Point - margin, currentY + 2 * TitleFont.Height)), titleFormat);

            currentY += 20 + TitleFont.Height * 2;

            return currentY;
        }

        private double DrawActuationDataSection(double currentY)
        {
            double labelWidth = 120;
            double padding = 5;
            double lineHeight = 11;
            const int maxLines = 2;

            graphics.DrawString("1. DATOS DE LA", SectionFont, new XSolidBrush(TextColor), new XPoint(margin, currentY));
            graphics.DrawString("ACTUACIÓN", SectionFont, new XSolidBrush(TextColor), new XPoint(margin, currentY + 12));

            bool isEmpty = string.IsNullOrEmpty(request.ActuationId);
            string actuationText = isEmpty ? "Promotor e identificación de la actuación" : request.ActuationId;
            XBrush textBrush = isEmpty ? new XSolidBrush(XColors.LightGray) : new XSolidBrush(TextColor);

            double boxWidth = tableWidth - labelWidth;
            double textWidth = boxWidth - padding * 2;

            List<string> lines = new List<string>();
            foreach (string paragraph in actuationText.Replace("\r", "").Split('\n'))
            {
                List<string> paragraphLines = SplitTextToFitWidth(paragraph, RegularFont, textWidth, graphics);
                if (paragraphLines.Count == 0) paragraphLines.Add("");
                lines.AddRange(paragraphLines);
            }

            if (lines.Count > maxLines)
            {
                string last = lines[maxLines - 1].TrimEnd('-');

                while (last.Length > 0 && graphics.MeasureString(last + "…", RegularFont).Width > textWidth)
                    last = last.Substring(0, last.Length - 1);

                lines[maxLines - 1] = last + "…";
                lines = lines.GetRange(0, maxLines);
            }

            double boxHeight = maxLines * lineHeight + padding * 2;

            XRect actuationRect = new XRect(margin + labelWidth, currentY - 10, boxWidth, boxHeight);
            graphics.DrawRectangle(new XPen(SecondaryBg, 1), actuationRect);

            for (int i = 0; i < lines.Count; i++)
            {
                graphics.DrawString(
                    lines[i],
                    RegularFont,
                    textBrush,
                    new XPoint(actuationRect.X + padding, actuationRect.Y + padding + i * lineHeight),
                    XStringFormats.TopLeft);
            }

            return actuationRect.Bottom - 10;
        }

        private double DrawUsesTable(double startY)
        {
            double currentY = startY;
            double headerRowHeight = request.ProjectType switch
            {
                "EDIF" => 28,
                "OBCI" => 28,
                "URBA" => 36,
                _ => 20
            };
            double rowHeight = 20;

            XRect headerRect = new XRect(margin, currentY, tableWidth, headerRowHeight);
            graphics.DrawRectangle(new XSolidBrush(TableHeaderColor), headerRect);
            graphics.DrawRectangle(new XPen(SecondaryBg, 1), headerRect);


            double ColX(int index) => margin + 5 + (tableWidth * usesTableLeftCoef[index]);
            double ColWidth(int index) => (((index < usesTableLeftCoef.Length - 1) ? usesTableLeftCoef[index + 1] : 1) - usesTableLeftCoef[index]) * tableWidth;

            switch (request.ProjectType)
            {
                case "EDIF":

                    DrawHeaderLines(new[] { "Usos" }, 0, currentY, headerRowHeight, false);
                    DrawHeaderLines(new[] { "Nº", "Plantas repetidas" }, 1, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "Superficie", "m²" }, 2, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM unitario", "€/m²" }, 3, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM", "instalaciones" }, 4, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM Uso", "€" }, 5, currentY, headerRowHeight);
                    break;

                case "OBCI":
                    DrawHeaderLines(new[] { "Tipo de actuación" }, 0, currentY, headerRowHeight, false);
                    DrawHeaderLines(new[] { "Medición" }, 2, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM unitario" }, 3, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM Act.", "€" }, 5, currentY, headerRowHeight);
                    break;

                case "URBA":
                    DrawHeaderLines(new[] { "Zonas de actuación" }, 0, currentY, headerRowHeight, false);
                    DrawHeaderLines(new[] { "Superficie", "Zona verde" }, 1, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "Superficie", "Red viaria" }, 2, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM unitario", "Zona verde", "€/m²" }, 3, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM unitario", "Red viaria", "€/m²" }, 4, currentY, headerRowHeight);
                    DrawHeaderLines(new[] { "PEM Zona", "€" }, 5, currentY, headerRowHeight);
                    break;
            }

            currentY += headerRowHeight;

            int topRowMargin = 12;

            int useIdx = 1;

            switch (request.ProjectType)
            {
                case "EDIF":
                    {
                        EdificationCalculationRequest calcRequest = (EdificationCalculationRequest)request.CalculationRequest;
                        List<EdificationUse> uses = calcRequest.Uses;

                        int useCount = 0;

                        foreach (EdificationUse use in uses)
                        {
                            useCount++;

                            string installPEM = use.InstallationPEM == null ? "" : $"{((double)use.InstallationPEM).ToString("N0", esES)} €";

                            string name = use.Name == "" ? $"Uso {useIdx}" : use.Name;

                            List<string> nameLines = SplitTextToFitWidth(name, CellFont, ColWidth(0), graphics);

                            if (nameLines.Count > 2)
                            {
                                nameLines[1] = nameLines[1].TrimEnd('-') + "…";
                                nameLines = nameLines[0..2];
                            }

                            double currRowHeight = rowHeight + (nameLines.Count - 1) * 8;

                            XRect rowRect = new XRect(margin, currentY, tableWidth, currRowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);

                            DrawCellText(useCount.ToString(), 0, currentY, currRowHeight, false, 0);

                            DrawCellTextLines(nameLines, 0, currentY, currRowHeight, false, 20);



                            DrawCellText($"{(use.RepeatedFloors ?? 0).ToString("N0", esES)}", 1, currentY, currRowHeight, true);
                            DrawCellText($"{(use.Area ?? 0).ToString("N0", esES)} m²", 2, currentY, currRowHeight, true);
                            DrawCellText($"{(use.UnitPEM ?? 0).ToString("N0", esES)} €/m²", 3, currentY, currRowHeight, true);
                            DrawCellText(installPEM, 4, currentY, currRowHeight, true);
                            DrawCellText($"{((use.Area * use.UnitPEM) ?? 0).ToString("N0", esES)} €", 5, currentY, currRowHeight, true);

                            currentY += currRowHeight;
                            useIdx++;
                        }

                        if (uses.Count == 0)
                        {
                            XRect rowRect = new XRect(margin, currentY, tableWidth, rowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);

                            graphics.DrawString("No hay usos de edificación disponibles.", CellFont, new XSolidBrush(XColors.Gray), new XRect(margin + 5, currentY, tableWidth, rowHeight), XStringFormats.CenterLeft);
                            currentY += rowHeight;
                        }

                        break;
                    }

                case "OBCI":
                    {
                        CivilWorksCalculationRequest calcRequest = (CivilWorksCalculationRequest)request.CalculationRequest;
                        List<CivilWorksUse> uses = calcRequest.Uses;

                        int useCount = 0;

                        Dictionary<string, string> areaSymbols = new()
                        {
                            ["Carreteras y autovías"] = "km",
                            ["Ferrocarriles"] = "km",
                            ["Puentes y viaductos"] = "m²",
                            ["Túneles"] = "m",
                            ["Obras marítimas y portuarias"] = "m",
                            ["Aeropuertos (parte civil)"] = "m²",
                            ["Presas y embalses"] = "m³",
                            ["Canales y conducciones de gran diámetro"] = "m",
                            ["Redes de abastecimiento y saneamiento"] = "m",
                            ["Estaciones de bombeo"] = "ud",
                            ["Depuradoras (EDAR)"] = "nº hab",
                            ["Trasvases y encauzamiento fluvial"] = "m"
                        };

                        Dictionary<string, string> unitPEMSymbols = new()
                        {
                            ["Carreteras y autovías"] = "€/km",
                            ["Ferrocarriles"] = "€/km",
                            ["Puentes y viaductos"] = "€/m² tablero",
                            ["Túneles"] = "€/m",
                            ["Obras marítimas y portuarias"] = "€/m",
                            ["Aeropuertos (parte civil)"] = "€/m²",
                            ["Presas y embalses"] = "€/m³ presa",
                            ["Canales y conducciones de gran diámetro"] = "€/m",
                            ["Redes de abastecimiento y saneamiento"] = "€/m",
                            ["Estaciones de bombeo"] = "€/ud",
                            ["Depuradoras (EDAR)"] = "€/hab.",
                            ["Trasvases y encauzamiento fluvial"] = "€/m"
                        };

                        foreach (CivilWorksUse use in uses)
                        {
                            useCount++;

                            string name = use.Type == "" ? $"Uso {useIdx}" : use.Type;

                            List<string> nameLines = SplitTextToFitWidth(name, CellFont, ColWidth(0), graphics);

                            if (nameLines.Count > 2)
                            {
                                nameLines[1] = nameLines[1].TrimEnd('-') + "…";
                                nameLines = nameLines[0..2];
                            }

                            double currRowHeight = rowHeight + (nameLines.Count - 1) * 8;

                            XRect rowRect = new XRect(margin, currentY, tableWidth, currRowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);



                            DrawCellText(useCount.ToString(), 0, currentY, currRowHeight, false, 0);

                            DrawCellText(name, 0, currentY, currRowHeight, false, 20);

                            string measurement = use.Area == null ? "" : $"{((double)use.Area).ToString("N0", esES)} {areaSymbols[use.Type]}";
                            string unitPEM = use.UnitPEM == null ? "" : $"{((double)use.UnitPEM).ToString("N0", esES)} {unitPEMSymbols[use.Type]}";


                            DrawCellText(measurement, 2, currentY, currRowHeight, true);
                            DrawCellText(unitPEM, 3, currentY, currRowHeight, true);

                            DrawCellText($"{(use.TotalPEM ?? 0).ToString("N0", esES)} €", 5, currentY, currRowHeight, true);

                            currentY += currRowHeight;
                            useIdx++;
                        }

                        if (uses.Count == 0)
                        {
                            XRect rowRect = new XRect(margin, currentY, tableWidth, rowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);

                            graphics.DrawString("No hay usos de obra civil disponibles.", CellFont, new XSolidBrush(XColors.Gray), new XRect(margin + 5, currentY, tableWidth, rowHeight), XStringFormats.CenterLeft);
                            currentY += rowHeight;
                        }

                        break;
                    }
                case "URBA":
                    {
                        UrbanisationCalculationRequest calcRequest = (UrbanisationCalculationRequest)request.CalculationRequest;
                        List<UrbanisationUse> uses = calcRequest.Uses;

                        int useCount = 0;

                        foreach (UrbanisationUse use in uses)
                        {
                            useCount++;

                            string name = use.Name == "" ? $"Uso {useIdx}" : use.Name;

                            List<string> nameLines = SplitTextToFitWidth(name, CellFont, ColWidth(0), graphics);

                            if (nameLines.Count > 2)
                            {
                                nameLines[1] = nameLines[1].TrimEnd('-') + "…";
                                nameLines = nameLines[0..2];
                            }

                            double currRowHeight = rowHeight + (nameLines.Count - 1) * 8;

                            XRect rowRect = new XRect(margin, currentY, tableWidth, currRowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);



                            DrawCellText(useCount.ToString(), 0, currentY, currRowHeight, false, 0);

                            DrawCellText(name, 0, currentY, currRowHeight, false, 20);


                            DrawCellText($"{(use.GreenArea ?? 0).ToString("N0", esES)} m²", 1, currentY, currRowHeight, true);
                            DrawCellText($"{(use.NetworkArea ?? 0).ToString("N0", esES)} m²", 2, currentY, currRowHeight, true);
                            DrawCellText($"{(use.UnitPEMGreenArea ?? 0).ToString("N0", esES)} €/m²", 3, currentY, currRowHeight, true);
                            DrawCellText($"{(use.UnitPEMNetworkArea ?? 0).ToString("N0", esES)} €/m²", 4, currentY, currRowHeight, true);
                            DrawCellText($"{((use.GreenArea * use.UnitPEMGreenArea + use.NetworkArea * use.UnitPEMNetworkArea) ?? 0).ToString("N0", esES)} €", 5, currentY, currRowHeight, true);

                            currentY += currRowHeight;
                            useIdx++;
                        }

                        if (uses.Count == 0)
                        {
                            XRect rowRect = new XRect(margin, currentY, tableWidth, rowHeight);
                            graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);

                            graphics.DrawString("No hay usos de urbanización disponibles.", CellFont, new XSolidBrush(XColors.Gray), new XRect(margin + 5, currentY, tableWidth, rowHeight), XStringFormats.CenterLeft);
                            currentY += rowHeight;
                        }

                        break;
                    }
            }

            return currentY;
        }

        private void DrawCellText(string text, int colIndex, double currentY, double rowHeight, bool centerHorizontally = true, double leftOffset = 0)
        {
            if (string.IsNullOrEmpty(text)) return;


            double startX = margin + (tableWidth * usesTableLeftCoef[colIndex]) + leftOffset;
            double endX = (colIndex < usesTableLeftCoef.Length - 1)
                ? margin + (tableWidth * usesTableLeftCoef[colIndex + 1])
                : margin + tableWidth;


            double padding = centerHorizontally ? 0 : 5;
            double colWidth = endX - startX - padding;


            XStringFormat format = centerHorizontally ? XStringFormats.Center : XStringFormats.CenterLeft;

            XRect cellRect = new XRect(startX + padding, currentY, colWidth, rowHeight);
            graphics.DrawString(text, CellFont, new XSolidBrush(TextColor), cellRect, format);
        }

        private void DrawCellTextLines(List<string> lines, int colIndex, double currentY, double rowHeight, bool centerHorizontally = true, double leftOffset = 0)
        {
            if (lines == null || lines.Count == 0) return;

            double startX = margin + (tableWidth * usesTableLeftCoef[colIndex]) + leftOffset;
            double endX = (colIndex < usesTableLeftCoef.Length - 1)
                ? margin + (tableWidth * usesTableLeftCoef[colIndex + 1])
                : margin + tableWidth;

            double padding = centerHorizontally ? 0 : 5;
            double colWidth = endX - startX - padding;

            double lineHeight = 9;
            double totalTextHeight = lines.Count * lineHeight;
            double startY = currentY + (rowHeight - totalTextHeight) / 2;

            XStringFormat format = centerHorizontally ? XStringFormats.Center : XStringFormats.CenterLeft;

            for (int i = 0; i < lines.Count; i++)
            {
                XRect lineRect = new XRect(startX + padding, startY + (i * lineHeight), colWidth, lineHeight);
                graphics.DrawString(lines[i], CellFont, new XSolidBrush(TextColor), lineRect, format);
            }
        }

        private void DrawHeaderLines(string[] lines, int colIndex, double currentY, double rowHeight, bool centerHorizontally = true)
        {

            double startX = margin + (tableWidth * usesTableLeftCoef[colIndex]);
            double endX = (colIndex < usesTableLeftCoef.Length - 1)
                ? margin + (tableWidth * usesTableLeftCoef[colIndex + 1])
                : margin + tableWidth;


            double padding = centerHorizontally ? 0 : 5;
            double colWidth = endX - startX - padding;


            double lineHeight = 9;
            double totalTextHeight = lines.Length * lineHeight;


            double startY = currentY + (rowHeight - totalTextHeight) / 2;


            XStringFormat format = centerHorizontally ? XStringFormats.Center : XStringFormats.CenterLeft;


            for (int i = 0; i < lines.Length; i++)
            {
                XRect lineRect = new XRect(startX + padding, startY + (i * lineHeight), colWidth, lineHeight);
                graphics.DrawString(lines[i], TableHeaderFont, new XSolidBrush(TextColor), lineRect, format);
            }
        }

        private List<string> SplitTextToFitWidth(string text, XFont font, double maxWidth, XGraphics gfx)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            double Measure(string s) => gfx.MeasureString(s, font).Width;

            string currentLine = "";

            foreach (string word in text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string candidate = currentLine.Length == 0 ? word : currentLine + " " + word;

                if (Measure(candidate) <= maxWidth)
                {
                    currentLine = candidate;
                    continue;
                }

                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                string w = word;
                while (Measure(w) > maxWidth)
                {
                    int len = w.Length - 1;
                    while (len > 1 && Measure(w.Substring(0, len) + "-") > maxWidth)
                        len--;

                    lines.Add(w.Substring(0, len) + "-");
                    w = w.Substring(len);
                }
                currentLine = w;
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine);

            return lines;
        }

        private async Task<double> DrawServiceContentSection(double startY)
        {
            double currentY = startY;
            double rowHeight = 20;
            double headerRowHeight = rowHeight;

            graphics.DrawString("2. CONTENIDO DEL SERVICIO", SectionFont, new XSolidBrush(TextColor), new XPoint(margin, currentY));

            int maxLines = 1;

            switch (request.ProjectType)
            {
                case "EDIF":
                    foreach (EdificationUse use in ((EdificationCalculationRequest)request.CalculationRequest).Uses)
                    {
                        int numLines = SplitTextToFitWidth(use.Name, TableHeaderFont, 0.14 * tableWidth, graphics).Count;
                        maxLines = numLines > maxLines ? numLines : maxLines;
                    }
                    break;
                case "OBCI":
                    foreach (CivilWorksUse use in ((CivilWorksCalculationRequest)request.CalculationRequest).Uses)
                    {
                        int numLines = SplitTextToFitWidth(use.Type, TableHeaderFont, 0.14 * tableWidth, graphics).Count;
                        maxLines = numLines > maxLines ? numLines : maxLines;
                    }
                    break;
                case "URBA":
                    foreach (UrbanisationUse use in ((UrbanisationCalculationRequest)request.CalculationRequest).Uses)
                    {
                        int numLines = SplitTextToFitWidth(use.Name, TableHeaderFont, 0.14 * tableWidth, graphics).Count;
                        maxLines = numLines > maxLines ? numLines : maxLines;
                    }
                    break;
            }

            if (maxLines > 2) maxLines = 2;

            headerRowHeight += (maxLines - 1) * 8;

            currentY += 15;

            XRect headerRect = new XRect(margin, currentY, tableWidth, headerRowHeight);
            graphics.DrawRectangle(new XSolidBrush(TableHeaderColor), headerRect);
            graphics.DrawRectangle(new XPen(BorderColor, 1), headerRect);

            double ColX(int index) => margin + 5 + (tableWidth * usesTableLeftCoef[index]);
            double ColWidth(int index) => (((index < usesTableLeftCoef.Length - 1) ? usesTableLeftCoef[index + 1] : 1) - usesTableLeftCoef[index]) * tableWidth;

            DrawHeaderLines(new[] { "Documentos y actividades" }, 0, currentY, headerRowHeight, false);

            int useIdx = 1;

            switch (request.ProjectType)
            {
                case "EDIF":
                    foreach (EdificationUse use in ((EdificationCalculationRequest)request.CalculationRequest).Uses)
                    {
                        List<string> lines = SplitTextToFitWidth(use.Name == "" ? $"Uso {useIdx}" : use.Name, TableHeaderFont, ColWidth(useIdx), graphics);

                        if (lines.Count > 2)
                        {
                            lines[1] = lines[1].TrimEnd('-') + "…";
                            lines = lines[0..2];
                        }

                        DrawHeaderLines(lines.ToArray(), useIdx++, currentY, headerRowHeight);
                    }
                    break;
                case "OBCI":
                    foreach (CivilWorksUse use in ((CivilWorksCalculationRequest)request.CalculationRequest).Uses)
                    {
                        List<string> lines = SplitTextToFitWidth(use.Type == "" ? $"Uso {useIdx}" : use.Type, TableHeaderFont, ColWidth(useIdx), graphics);

                        if (lines.Count > 2)
                        {
                            lines[1] = lines[1].TrimEnd('-') + "…";
                            lines = lines[0..2];
                        }

                        DrawHeaderLines(lines.ToArray(), useIdx++, currentY, headerRowHeight);
                    }
                    break;
                case "URBA":
                    foreach (UrbanisationUse use in ((UrbanisationCalculationRequest)request.CalculationRequest).Uses)
                    {
                        List<string> lines = SplitTextToFitWidth(use.Name == "" ? $"Uso {useIdx}" : use.Name, TableHeaderFont, ColWidth(useIdx), graphics);

                        if (lines.Count > 2)
                        {
                            lines[1] = lines[1].TrimEnd('-') + "…";
                            lines = lines[0..2];
                        }

                        DrawHeaderLines(lines.ToArray(), useIdx++, currentY, headerRowHeight);
                    }
                    break;
            }

            DrawHeaderLines(new[] { "Coste €" }, 5, currentY, headerRowHeight, true);

            currentY += headerRowHeight;

            double col0Width = (tableWidth * usesTableLeftCoef[1]) - 5;

            string projectState = "";

            switch (this.request.ProjectType)
            {
                case "EDIF":
                    {
                        projectState = ((EdificationCalculationRequest)this.request.CalculationRequest).ProjectState.ToString();
                        EdificacionTiposProyecto tipoProyecto = await fetchService.GetEdificationProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
                case "OBCI":
                    {
                        projectState = ((CivilWorksCalculationRequest)this.request.CalculationRequest).ProjectState.ToString();
                        ObraCivilTiposProyecto tipoProyecto = await fetchService.GetCivilWorksProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
                case "URBA":
                    {
                        projectState = ((UrbanisationCalculationRequest)this.request.CalculationRequest).ProjectState.ToString();
                        UrbanizacionTiposProyecto tipoProyecto = await fetchService.GetUrbanisationProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
            }

            List<string> projectNameLines = SplitTextToFitWidth(projectState, CellFont, col0Width, graphics);
            if (projectNameLines.Count == 0) projectNameLines.Add(projectState);

            double projectRowHeight = rowHeight + (projectNameLines.Count - 1) * 8;

            XRect projectRowRect = new XRect(margin, currentY, tableWidth, projectRowHeight);
            graphics.DrawRectangle(new XPen(BorderColor, 1), projectRowRect);

            DrawCellTextLines(projectNameLines, 0, currentY, projectRowHeight, false);

            int projectIdx = 1;
            double projectTotal = 0;

            foreach (double value in this.response.ProjectCosts)
            {
                projectTotal += value;
                DrawCellText($"{FormatMoney(value)} €", projectIdx, currentY, projectRowHeight);
                projectIdx++;
            }

            DrawCellText($"{FormatMoney(projectTotal)} €", 5, currentY, projectRowHeight);

            currentY += projectRowHeight;

            List<AddonCalculationResponse> _addonResponses = response?.Responses ?? new List<AddonCalculationResponse>();

            if (_addonResponses != null && _addonResponses.Any())
            {

                foreach (var addon in _addonResponses)
                {
                    string addonName = addon.Id;

                    switch (this.request.ProjectType)
                    {
                        case "EDIF":
                            {
                                EdificacionDocs doc = await fetchService.GetEdificationDocById(addonName);
                                addonName = doc?.Nombre ?? addonName;
                                break;
                            }
                        case "OBCI":
                            {
                                ObraCivilDocs doc = await fetchService.GetCivilWorksDocById(addonName);
                                addonName = doc?.Nombre ?? addonName;
                                break;
                            }
                        case "URBA":
                            {
                                UrbanizacionDocs doc = await fetchService.GetUrbanisationDocById(addonName);
                                addonName = doc?.Nombre ?? addonName;
                                break;
                            }
                    }

                    List<string> nameLines = SplitTextToFitWidth(addonName, CellFont, col0Width, graphics);
                    if (nameLines.Count == 0) nameLines.Add(addonName);

                    double thisRowHeight = rowHeight + (nameLines.Count - 1) * 8;

                    XRect rowRect = new XRect(margin, currentY, tableWidth, thisRowHeight);
                    graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);

                    DrawCellTextLines(nameLines, 0, currentY, thisRowHeight, false);

                    int idx = 1;
                    double total = 0;

                    foreach (double value in addon.Values)
                    {
                        total += value;
                        DrawCellText($"{FormatMoney(value)} €", idx, currentY, thisRowHeight);
                        idx++;
                    }

                    DrawCellText($"{FormatMoney(total)} €", 5, currentY, thisRowHeight);

                    currentY += thisRowHeight;
                }
            }
            else
            {
                XRect rowRect = new XRect(margin, currentY, tableWidth, rowHeight);
                graphics.DrawRectangle(new XPen(BorderColor, 1), rowRect);
                graphics.DrawString("No hay documentos o actividades.", CellFont, new XSolidBrush(XColors.Gray), new XRect(margin + 5, currentY, tableWidth, rowHeight), XStringFormats.CenterLeft);
                currentY += rowHeight;
            }

            return currentY;
        }

        private double CalcProjectCost()
        {
            double total = 0;

            foreach (double cost in this.response.ProjectCosts)
            {
                total += cost;
            }

            return total;
        }

        private double CalcAdditionalDocsCost()
        {
            double total = 0;

            foreach (AddonCalculationResponse addonResponse in this.response.Responses)
            {
                foreach (double cost in addonResponse.Values)
                {
                    total += cost;
                }
            }

            return total;
        }

        private double CalcTotalPEM()
        {
            double total = 0;

            switch (this.request.ProjectType)
            {
                case "EDIF":
                    foreach (EdificationUse use in ((EdificationCalculationRequest)this.request.CalculationRequest).Uses)
                    {
                        total += ((use.UnitPEM ?? 0) * (use.Area ?? 0)) + (use.InstallationPEM ?? 0);
                    }
                    break;
                case "OBCI":
                    foreach (CivilWorksUse use in ((CivilWorksCalculationRequest)this.request.CalculationRequest).Uses)
                    {
                        total += use.TotalPEM ?? 0;
                    }
                    break;
                case "URBA":
                    foreach (UrbanisationUse use in ((UrbanisationCalculationRequest)this.request.CalculationRequest).Uses)
                    {
                        total += ((use.GreenArea ?? 0) * (use.UnitPEMGreenArea ?? 0) + (use.NetworkArea ?? 0) * (use.UnitPEMNetworkArea ?? 0));
                    }
                    break;
            }

            return total;
        }

        private async Task DrawSummaryBox(double startY)
        {
            double currentY = startY;
            double startX = margin;
            double width = this.tableWidth;

            double projectCost = CalcProjectCost();
            double additionalDocsCost = CalcAdditionalDocsCost();
            double totalPEM = CalcTotalPEM();
            double estimatedCost = projectCost + additionalDocsCost;
            double projectPerEstimated = estimatedCost == 0 ? 0 : (projectCost / estimatedCost);
            double additionalPerEstimated = estimatedCost == 0 ? 0 : (additionalDocsCost / estimatedCost);
            double costPerPEMPercentage = totalPEM == 0 ? 0 : (estimatedCost / totalPEM) * 100;

            double innerMargin = 15;
            double totalWidthPercentage = 0.25;
            double progressBarsPercentage = 0.5;
            double extraInfoPercentage = 1 - totalWidthPercentage - progressBarsPercentage;
            double rowWidth = progressBarsPercentage * width - innerMargin;

            string projectState = "";
            switch (this.request.ProjectType)
            {
                case "EDIF":
                    {
                        projectState = ((EdificationCalculationRequest)(this.request.CalculationRequest)).ProjectState.ToString();
                        EdificacionTiposProyecto tipoProyecto = await fetchService.GetEdificationProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
                case "OBCI":
                    {
                        projectState = ((CivilWorksCalculationRequest)(this.request.CalculationRequest)).ProjectState.ToString();
                        ObraCivilTiposProyecto tipoProyecto = await fetchService.GetCivilWorksProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
                case "URBA":
                    {
                        projectState = ((UrbanisationCalculationRequest)(this.request.CalculationRequest)).ProjectState.ToString();
                        UrbanizacionTiposProyecto tipoProyecto = await fetchService.GetUrbanisationProjectById(projectState);
                        projectState = tipoProyecto?.Nombre ?? projectState;
                        break;
                    }
            }

            string projectStateLabel = projectState == "" ? "Actuación" : projectState;
            string projectCostValue = $"{FormatMoney(projectCost)} €";

            List<string> projectStateLines = WrapSummaryLabel(projectStateLabel, projectCostValue, rowWidth);
            double projectStateExtraHeight = (projectStateLines.Count - 1) * SummaryRowLineHeight;

            XRect headerRect = new XRect(startX, currentY, width * totalWidthPercentage, innerMargin * 2 + 35 + projectStateExtraHeight);
            graphics.DrawRectangle(new XSolidBrush(PrimaryColor), headerRect);

            // RECUADRO GRISACEO
            XRect backgroundRect = new XRect(startX + width * totalWidthPercentage, startY, width * (1 - totalWidthPercentage), innerMargin * 2 + 35 + projectStateExtraHeight);
            graphics.DrawRectangle(new XSolidBrush(BackgroundColor), backgroundRect);

            graphics.DrawString("COSTE ESTIMADO:", TableHeaderFont, new XSolidBrush(XColors.White), new XPoint(startX + 15, currentY + 20));

            string totalValue = $"{FormatMoney(estimatedCost)} €";

            double totalBoxWidth = width * totalWidthPercentage;
            double availableTotalWidth = totalBoxWidth - innerMargin * 2;

            XFont totalFont = GetFittingFont(totalValue, availableTotalWidth, SummaryTotalMaxFontSize, SummaryTotalMinFontSize);
            double baselineOffset = (SummaryTotalFont.Size - totalFont.Size) / 2;

            graphics.DrawString(
                totalValue,
                totalFont,
                new XSolidBrush(XColors.White),
                new XPoint(startX + innerMargin, currentY + 40 - baselineOffset));

            double rowX = startX + width * totalWidthPercentage + innerMargin / 2;
            XPen unfilledProgressPen = new XPen(BorderColor, 3);
            XPen filledProgressPen = new XPen(PrimaryColor, 3);

            currentY += innerMargin;
            DrawSummaryRow(projectStateLabel, projectCostValue, rowX, currentY, rowWidth);
            currentY += 10 + projectStateExtraHeight;

            graphics.DrawLine(unfilledProgressPen, rowX, currentY, rowX + rowWidth, currentY);
            graphics.DrawLine(filledProgressPen, rowX, currentY, rowX + projectPerEstimated * rowWidth, currentY);


            currentY += 20;
            DrawSummaryRow("Documentación adicional", $"{FormatMoney(additionalDocsCost)} €", rowX, currentY, rowWidth);
            currentY += 10;
            graphics.DrawLine(unfilledProgressPen, rowX, currentY, rowX + rowWidth, currentY);
            graphics.DrawLine(filledProgressPen, rowX, currentY, rowX + additionalPerEstimated * rowWidth, currentY);


            // LINEA VERTICAL DE SEPARACION
            graphics.DrawLine(new XPen(PrimaryColor, 1), startX + width * (1 - extraInfoPercentage), startY + 5, startX + width * (1 - extraInfoPercentage), startY + 45 + innerMargin + projectStateExtraHeight);


            currentY = startY + innerMargin;
            rowX = rowX + width * progressBarsPercentage;
            double extraInfoWidth = width * extraInfoPercentage - innerMargin;
            DrawSummaryRow("PEM Total", "", rowX, currentY, extraInfoWidth);
            DrawSummaryRow("", $"{FormatMoney(totalPEM)} €", rowX, currentY + 12, extraInfoWidth);

            currentY += 35;
            DrawSummaryRow("Coste/PEM", "", rowX, currentY, extraInfoWidth);
            DrawSummaryRow("", $"{costPerPEMPercentage.ToString("N1", HonorariosDoc.esES).TrimEnd('0').TrimEnd(HonorariosDoc.esES.NumberFormat.NumberDecimalSeparator[0])} %", rowX, currentY + 12, extraInfoWidth);

            currentY += 15;



        }

        private List<string> WrapSummaryLabel(string label, string value, double width)
        {
            double valueWidth = graphics.MeasureString(value, SummaryValueFont).Width;
            double gap = 8;
            double maxLabelWidth = Math.Max(width - valueWidth - gap, 20);

            List<string> lines = SplitTextToFitWidth(label, RegularFont, maxLabelWidth, graphics);
            if (lines.Count == 0) lines.Add(label);
            return lines;
        }

        private double DrawSummaryRow(string label, string value, double x, double y, double width)
        {
            List<string> labelLines = WrapSummaryLabel(label, value, width);

            for (int i = 0; i < labelLines.Count; i++)
            {
                graphics.DrawString(labelLines[i], RegularFont, new XSolidBrush(TextColor), new XPoint(x, y + i * SummaryRowLineHeight));
            }

            var valueSize = graphics.MeasureString(value, SummaryValueFont);
            double valueY = y + (labelLines.Count - 1) * SummaryRowLineHeight / 2.0;
            graphics.DrawString(value, SummaryValueFont, new XSolidBrush(TextColor), new XPoint(x + width - valueSize.Width, valueY));

            return (labelLines.Count - 1) * SummaryRowLineHeight;
        }

        private double DrawSummaryRow(string label1, string label2, string value, double x, double y, double width)
        {
            graphics.DrawString(label1, RegularFont, new XSolidBrush(TextColor), new XPoint(x, y - 6));
            graphics.DrawString(label2, RegularFont, new XSolidBrush(TextColor), new XPoint(x, y + 6));
            var valueSize = graphics.MeasureString(value, SummaryValueFont);
            graphics.DrawString(value, SummaryValueFont, new XSolidBrush(TextColor), new XPoint(x + width - valueSize.Width, y));
            return 0;
        }

        private XFont GetFittingFont(string text, double maxWidth, double maxSize, double minSize, XFontStyleEx style = XFontStyleEx.Bold)
        {
            double size = maxSize;
            XFont font = new XFont("Montserrat", size, style);

            while (size > minSize && graphics.MeasureString(text, font).Width > maxWidth)
            {
                size -= 0.5;
                font = new XFont("Montserrat", size, style);
            }

            return font;
        }

        private void DrawFooter()
        {

            graphics.DrawLine(new XPen(PrimaryColor), margin, page.Height.Point - footHeight, page.Width.Point - margin, page.Height.Point - footHeight);

            const string resourceName = "API_Backend_App_Honorarios.Materials.Images.LogoIVEPDF.png";

            Assembly assembly = typeof(IndustrializacionDoc).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}");

            using MemoryStream memStream = new MemoryStream();
            stream.CopyTo(memStream);
            memStream.Position = 0;

            using XImage logo = XImage.FromStream(memStream);

            const double imageModifier = 0.6;
            double imageMargin = (1 - imageModifier) * footHeight / 2;
            double image_margin = 10;

            double imageHeight = footHeight * imageModifier;
            double imageWidth = (imageHeight / logo.PixelHeight) * logo.PixelWidth;

            graphics.DrawImage(logo, new XRect(margin + image_margin, page.Height.Point - footHeight + imageMargin, imageWidth, imageHeight));

            string dateString = "DD/MM/YYYY";

            dateString = $"{genTime.Day: 00}/{genTime.Month: 00}/{genTime.Year: 0000}";

            graphics.DrawString(
            dateString,
            RegularFont,
            new XSolidBrush(TextColor),
            new XRect(page.Width.Point / 2, page.Height.Point - footHeight + 1.5 * imageMargin, page.Width.Point / 2 - margin, RegularFont.Size),
            XStringFormats.CenterRight
            );

            string codeString = $"CVE: {CVECode}";

            graphics.DrawString(
            codeString,
            RegularFont,
            new XSolidBrush(TextColor),
            new XRect(page.Width.Point / 2, page.Height.Point - 1.2 * imageMargin - RegularFont.Size, page.Width.Point / 2 - margin, RegularFont.Size),
            XStringFormats.CenterRight
            );
        }

        private static string FormatMoney(double value)
        {
            double rounded;

            if (value < 100)
            {
                rounded = Math.Ceiling(value / 10) * 10;
                return rounded.ToString("N0", esES);
            }

            rounded = Math.Ceiling(value / 100) * 100;
            return rounded.ToString("N0", esES);
        }

    }
}