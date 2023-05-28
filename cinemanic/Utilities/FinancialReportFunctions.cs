using cinemanic.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace cinemanic.Utilities
{
    /// <summary>
    /// Provides utility functions for generating financial report.
    /// </summary>
    public static class FinancialReportFunctions
    {
        /// <summary>
        /// Creates a paragraph with the specified text, size, and alignment.
        /// </summary>
        /// <param name="text">The text of the paragraph.</param>
        /// <param name="size">The font size of the paragraph.</param>
        /// <param name="alignment">The alignment of the paragraph.</param>
        /// <returns>The created paragraph.</returns>
        public static Paragraph CreateParagraph(string text, int size, int alignment)
        {
            var paragraph = new Paragraph(text);
            paragraph.Font.Size = 16;
            paragraph.Alignment = alignment;

            return paragraph;
        }

        /// <summary>
        /// Creates a report table based on the provided archived screenings data.
        /// </summary>
        /// <param name="archivedScreeningsData">The list of archived screenings data.</param>
        /// <returns>The created report table.</returns>
        public static PdfPTable CreateReportTable(List<ArchivedScreening> archivedScreeningsData)
        {
            PdfPTable table = new PdfPTable(6);
            table.SetWidths(new float[] { 2, 3, 1, 1, 1, 1 }); // Set custom column widths

            List<string> tableHeaders = new List<string> { "Date", "Movie", "With", "Room", "Empty Seats", "Income" };

            foreach (string header in tableHeaders) AddTableCell(table, header);

            foreach (var screening in archivedScreeningsData)
            {
                AddTableCell(table, screening.ScreeningDate.ToString("dd.MM.yy\nHH:mm"));
                AddTableCell(table, screening.Movie.Title);
                AddTableCell(table, GetScreeningProperties(screening));
                AddTableCell(table, screening.RoomId.ToString());
                AddTableCell(table, screening.SeatsLeft.ToString());
                AddTableCell(table, screening.GrossIncome.ToString() + "zl");
            }

            return table;
        }

        /// <summary>
        /// Adds a table cell with the specified text to the provided table.
        /// </summary>
        /// <param name="table">The table to add the cell to.</param>
        /// <param name="text">The text of the cell.</param>
        private static void AddTableCell(PdfPTable table, string text)
        {
            var paragraph = new Paragraph(text);
            paragraph.SetLeading(0, 1.5f);

            PdfPCell cell = new PdfPCell();
            cell.AddElement(paragraph);
            cell.PaddingTop = 1;
            cell.PaddingLeft = 4;
            cell.PaddingBottom = 12;
            table.AddCell(cell);
        }

        /// <summary>
        /// Gets the screening properties for the provided archived screening.
        /// </summary>
        /// <param name="screening">The archived screening.</param>
        /// <returns>A string representation of the screening properties.</returns>
        private static string GetScreeningProperties(ArchivedScreening screening)
        {
            StringBuilder sb = new StringBuilder();
            if (screening.Subtitles) sb.AppendLine("Sub");
            if (screening.Lector) sb.AppendLine("Lector");
            if (screening.Dubbing) sb.AppendLine("Dub");
            if (screening.Is3D) sb.AppendLine("3D");
            return sb.ToString();
        }
    }
}
