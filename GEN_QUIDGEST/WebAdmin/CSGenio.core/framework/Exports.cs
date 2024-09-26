using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using System.Xml;

using CSGenio.business;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using Quidgest.Persistence;

using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Ionic.Zip;
using PdfSharpCore.Drawing;

namespace CSGenio.framework
{
    public class Exports
    {
        private User user;
        private List<QColumn> colunas = null;

        public Exports(User user)
        {
            this.user = user;
        }

        public byte[] ExportList(string listingControl, CriteriaSet conditions, IList<ColumnSort> orderBy, string exportType, string namedbedit)
        {
            return ExportList(listingControl, conditions, orderBy, exportType, null, namedbedit);
        }

        public byte[] ExportList(string listingControl, CriteriaSet conditions, IList<ColumnSort> orderBy, string exportType, string filename, string namedbedit)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos =
                PersistentSupport.getControlQueries();
            IDictionary<string, PersistentSupport.overrideDbeditQuery> controlosOverride =
                PersistentSupport.getControlQueriesOverride();

            SelectQuery qs = null;

            if (controlosOverride.ContainsKey(listingControl))
            {
                qs = controlosOverride[listingControl](user, "", conditions, orderBy, sp);
            }
            else
            {
                PersistentSupport.ControlQueryDefinition aux = controlos[listingControl];
                qs = new SelectQuery();
                foreach (SelectField field in aux.SelectFields)
                {
                    qs.SelectFields.Add(field);
                }
                qs.FromTable = aux.FromTable;
                foreach (TableJoin join in aux.Joins)
                {
                    qs.Joins.Add(join);
                }
                qs.Where(CriteriaSet.And()
                    .SubSet(aux.WhereConditions)
                    .SubSet(conditions));
            }

            if (qs.OrderByFields.Count == 0)
            {
                foreach (ColumnSort sort in orderBy)
                {
                    qs.OrderByFields.Add(sort);
                }
            }

            return ExportList(qs, exportType, filename, namedbedit);
        }

        public byte[] ExportList<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType, string namedbedit) where A : IArea
        {
            return ExportList<A>(listing, conditions, columns, exportType, null, namedbedit);
        }

        public byte[] ExportList<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType, string filename, string namedbedit) where A : IArea
        {
            this.colunas = columns;
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            SelectQuery qs = sp.getSelectQueryFromListingMVC<A>(conditions, listing);

            return ExportList(qs, exportType, filename,namedbedit);
        }

        private byte[] ExportList(SelectQuery qs, string exportType, string filename,string namedbedit)
        {
            ExportType type = this.getExportType(exportType);
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            //get data
            sp.openConnection();
            DataMatrix res = sp.Execute(qs);
            sp.closeConnection();

            if (colunas == null)
            {
                colunas = new List<QColumn>();
                //get column
                foreach (DataColumn dc in res.DbDataSet.Tables[0].Columns)
                {
                    string[] colcap = dc.Caption.Split('.');
                    AreaInfo ainfo = business.Area.GetInfoArea(colcap[0]);
                    colunas.Add(new QColumn(dc.Caption, ainfo.DBFields[colcap[1]]));
                }
            }

            return ExportList(res, exportType, filename, namedbedit);
        }


        public byte[] ExportList(DataMatrix values, List<QColumn> columns, string exportType, string filename, string namedbedit)
        {
            this.colunas = columns;
            return ExportList(values, exportType, filename, namedbedit);
        }

        public byte[] ExportTemplate(List<QColumn> columns, string exportType, string filename, string namedbedit)
        {
            this.colunas = columns;
            DataTable dt = new DataTable();
            foreach (QColumn campopedido in columns)
            {
                dt.Columns.Add(campopedido.Name, campopedido.Type.Type);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            DataMatrix values = new DataMatrix(ds);
            return ExportList(values, exportType, filename, namedbedit);
        }


        public byte[] ExportList(DataMatrix values, string exportType, string filename, string namedbedit)
        {
            ExportType type = this.getExportType(exportType);
            byte[] fileBinary = null;

            switch (type)
            {
                case ExportType.pdf:
                    ExportToPDF pdf = new ExportToPDF();
                    return pdf.GetPDF(values, colunas, namedbedit, user);

                case ExportType.xlsx:
                    ExportToExcel excel = new ExportToExcel();
                    return excel.GetExcel(filename, values, colunas, namedbedit, user);

                case ExportType.ods:
                    ExportToODS ods = new ExportToODS();
                    return ods.GetOds(values, colunas, user);

                case ExportType.csv:
                    ExportToCSV csv = new ExportToCSV();
                    return csv.GetCSV(values, colunas, user);

                case ExportType.xml:
                    ExportToXML xml = new ExportToXML();
                    return xml.GetXML(values, colunas, user);
            }

            return fileBinary;
        }

        public bool ExportListValidation<A>(ListingMVC<A> listing, CriteriaSet conditions, List<QColumn> columns, string exportType) where A : IArea
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            SelectQuery qs = sp.getSelectQueryFromListingMVC<A>(conditions, listing);
   
            //get data
            sp.openConnection();
            DataMatrix res = sp.Execute(qs);
            sp.closeConnection();


            ExportType type = this.getExportType(exportType);

            switch (type)
            {
                case ExportType.pdf:
                    return ExportToPDF.ValidatePage(columns, res, user);
            }

            return true;
        }

        private ExportType getExportType(string exportType)
        {
            switch (exportType)
            {
                case "csv":
                    return ExportType.csv;

                case "xlsx":
                case "xls":
                    return ExportType.xlsx;

                case "ods":
                    return ExportType.ods;

                case "pdf":
                    return ExportType.pdf;

                case "xml":
                    return ExportType.xml;

                default:
                    throw new FrameworkException("Tipo de exportação não suportado!", "Exports.getExportType", "Tipo de exportação '" + exportType + "' inexistente");
            }
        }

        #region

        private enum ExportType
        {
            csv,
            xlsx,
            ods,
            pdf,
            xml
        }

        public class QColumn
        {
            public string Name { get; private set; }
            public FieldType Type { get; private set; }
            public FieldFormatting Formatting { get; private set; }
            public string ArrayName { get; private set; }
            public string Description { get; private set; }
            public int Size { get; private set; }
            public int Decimals { get; private set; }
            public bool Visible { get; set; }


            public QColumn(FieldRef Qfield, FieldType fieldType, string descricao, int size, int decimais, bool visivel)
            {
                this.Name = Qfield.FullName;
                this.Type = fieldType;
                this.Formatting = fieldType.Formatting;
                this.ArrayName = null;
                this.Description = descricao;
                this.Size = size;
                this.Decimals = decimais;
                this.Visible = visivel;
            }

            public QColumn(FieldRef Qfield, FieldType fieldType, string descricao, int size, int decimais, bool visivel, string arrayName)
            {
                this.Name = Qfield.FullName;
                this.Type = fieldType;
                this.Formatting = fieldType.Formatting;
                this.ArrayName = arrayName;
                this.Description = descricao;
                this.Size = size;
                this.Decimals = decimais;
                this.Visible = visivel;
            }

            public QColumn(string fieldName, Field campoBD)
            {
                this.Name = fieldName;
                this.Description = campoBD.FieldDescription;
                this.Size = Math.Max(campoBD.FieldSize, campoBD.FieldDescription.Length) * 8;
                this.Decimals = campoBD.Decimals;
                this.Type = campoBD.FieldType;
                this.Formatting = campoBD.FieldFormat;
                this.Visible = true;
                this.ArrayName = String.IsNullOrEmpty(campoBD.ArrayName) ? null : campoBD.ArrayName;
            }

            private string m_BaseArea;
            public string BaseArea
            {
                get
                {
                    if (this.m_BaseArea == null)
                    {
                        this.m_BaseArea = this.Name.Split('.')[0];
                        this.m_FieldName = this.Name.Split('.')[1];
                    }

                    return this.m_BaseArea;
                }
            }
            private string m_FieldName;
            public string FieldName
            {
                get
                {
                    if (this.m_FieldName == null)
                    {
                        this.m_BaseArea = this.Name.Split('.')[0];
                        this.m_FieldName = this.Name.Split('.')[1];
                    }

                    return this.m_FieldName;
                }
            }

            public static int Sum(IList<QColumn> columns)
            {
                int sum = 0;

                foreach (QColumn col in columns)
                    sum += col.Size;

                return sum;
            }

            public static List<QColumn> GetVisibleColumns(IList<QColumn> columns)
            {
                List<QColumn> visibleColumns = new List<QColumn>();

                foreach (QColumn col in columns)
                    if (col.Visible)
                        visibleColumns.Add(col);

                return visibleColumns;
            }

        }

        private class ExportToPDF
        {
            readonly static double TableMaxWidth = PdfSharpCore.PageSizeConverter.ToSize(PdfSharpCore.PageSize.A4).Height - Unit.FromCentimeter(2.2).Point;
            readonly static double WidthScaleFactorMin = 0.5; // do not reduce column width by more than half
            readonly static Color TableBorder = new Color(0, 0, 0);
            readonly static Color TableBlue = new Color(235, 240, 249);
            readonly static Color TableGray = new Color(242, 242, 242);
            readonly static Font HeaderFont = new Font("Arial", 12);
            readonly static Font TableFont = new Font("Arial", 10);
            readonly static Unit TableBorderWidth = new Unit(1);
            readonly static XGraphics DefaultGraphics = XGraphics.CreateMeasureContext(new XSize(2000, 2000), XGraphicsUnit.Point, XPageDirection.Downwards);


            private Document document;
            private Table table;

            /// <summary>
            /// Generate pdf document with table content.
            /// </summary>
            public byte[] GetPDF(DataMatrix values, List<QColumn> columns, string namedbedit, User user = null)
            {
                // Create a MigraDoc document
                this.document = new Document();
                document.UseCmykColor = false;
                document.DefaultPageSetup.PageFormat = PageFormat.A4;
                document.DefaultPageSetup.Orientation = Orientation.Landscape;
                document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(1.0);
                document.DefaultPageSetup.FooterDistance = Unit.FromCentimeter(0.5);

                PageSetup pgSetup = document.DefaultPageSetup.Clone();
                document.Info.Title = namedbedit;

                DefineStyles();

                CreatePage(columns, values, namedbedit, user);

                FillContent(columns, values, user);

                // Create a renderer for PDF that uses Unicode font encoding
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                byte[] buffer;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdfRenderer.Save(ms, false);
                    buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);
                }

                return buffer;
            }

            /// <summary>
            /// Defines the styles used to format the MigraDoc document.
            /// </summary>
            private void DefineStyles()
            {
                // Get the predefined style Normal.
                Style style = this.document.Styles["Normal"];
                // Because all styles are derived from Normal, the next line changes the
                // font of the whole document. Or, more exactly, it changes the font of
                // all styles and paragraphs that do not redefine the font.
                style.Font = TableFont.Clone();

                style = this.document.Styles[StyleNames.Header];
                style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

                style = this.document.Styles[StyleNames.Footer];
                style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

                // Create a new style called Table based on style Normal
                style = this.document.Styles.AddStyle("Table", "Normal");
                style.Font = TableFont.Clone();
                style.Font.Color = Colors.Black;

                // Create a new style called Reference based on style Normal
                style = this.document.Styles.AddStyle("Reference", "Normal");
                style.ParagraphFormat.SpaceBefore = "5mm";
                style.ParagraphFormat.SpaceAfter = "5mm";
                style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
            }

            /// <summary>
            /// Check if the content can fit the PDF page.
            /// PDFSharp/MigraDoc does not split tables by column/vertically.
            /// </summmary>
            public static bool ValidatePage(List<QColumn> columns, DataMatrix values, User user = null)
            {
                // Measure strings for column width estimation
                Font tableHeaderFont = HeaderFont.Clone();
                tableHeaderFont.Bold = true;
                TextMeasurement tmHeader = new TextMeasurement(DefaultGraphics, tableHeaderFont);
                TextMeasurement tmBody = new TextMeasurement(DefaultGraphics, TableFont);
                double totalColWidth = 0;

                for (int i = 0; i < columns.Count; i++)
                {
                    // Minimum column width, based on column title
                    double minColWidth =
                        tmHeader.MeasureString(columns[i].Description, UnitType.Point).Width +
                        TableBorderWidth + Unit.FromCentimeter(0.5).Point;

                    // Find the average width from column values
                    double totalRowWidth = 0;

                    // Look at the first 100 rows in order to determine medium width
                    string columnName = columns[i].Name;
                    int currRow = 0;
                    int maxRows = values.NumRows < 100 ? values.NumRows - 1 : 100;
                    while (currRow <= maxRows)
                    {
                        string rowVal = getTextFromData(values.GetDirect(currRow, columnName), columns[i], user);
                        double colValMeasure = tmBody.MeasureString(rowVal, UnitType.Point).Width + TableBorderWidth + Unit.FromCentimeter(0.25).Point;
                        totalRowWidth += colValMeasure;
                        currRow++;
                    }

                    double columnWidth = (totalRowWidth + minColWidth) / (maxRows + 1);

                    // Enforce minimum column width
                    if (columnWidth < minColWidth)
                        columnWidth = minColWidth;

                    // Add column width to total columns width
                    totalColWidth += columnWidth;
                }

                double scaleFactor = TableMaxWidth / totalColWidth;

                if (scaleFactor < WidthScaleFactorMin)
                    return false;
                else
                    return true;
            }


            /// <summary>
            /// Creates the static parts of the PDF document.
            /// </summary>
            private void CreatePage(List<QColumn> columns, DataMatrix values, String namebdedit, User user = null)
            {
                // Each MigraDoc document needs at least one section.
                Section section = this.document.AddSection();
                section.PageSetup.StartingNumber = 1;

                // Add page header
                Paragraph to = new Paragraph();
                to.Format.Alignment = ParagraphAlignment.Center;
                to.Format.Font = HeaderFont.Clone();
                to.Format.Font.Bold = true;
                //to.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
                to.Format.Font.Color = Colors.Black;
                to.AddText(namebdedit);
                section.Headers.Primary.Add(to);
                section.AddParagraph();
                section.AddParagraph();

                // Add page footer
                Paragraph footer = new Paragraph();
                footer.AddPageField();
                footer.AddChar(Chars.Hyphen);
                footer.AddNumPagesField();
                footer.Format.Alignment = ParagraphAlignment.Center;
                section.Footers.Primary.Add(footer);

                // Create the item table
                this.table = section.AddTable();
                this.table.Style = "Table";
                this.table.Borders.Color = TableBorder;
                this.table.Borders.Width = TableBorderWidth;
                this.table.Borders.Left.Width = 0.5;
                this.table.Borders.Right.Width = 0.5;
                this.table.Rows.LeftIndent = 0;

                // Measure strings for column width estimation
                Font tableHeaderFont = HeaderFont.Clone();
                tableHeaderFont.Bold = true;
                TextMeasurement tmHeader = new TextMeasurement(DefaultGraphics, tableHeaderFont);
                TextMeasurement tmBody = new TextMeasurement(DefaultGraphics, this.document.Styles["Table"].Font.Clone());
                double totalColWidth = 0;

                for (int i = 0; i < columns.Count; i++)
                {
                    // Minimum column width, based on column title
                    double minColWidth =
                        tmHeader.MeasureString(columns[i].Description, UnitType.Point).Width +
                        this.table.Borders.Width + Unit.FromCentimeter(0.5).Point;

                    // Find the average width from column values
                    double totalRowWidth = 0;

                    // Look at the first 100 rows in order to determine medium width
                    string columnName = columns[i].Name;
                    int currRow = 0;
                    int maxRows = values.NumRows < 100 ? values.NumRows - 1 : 100;
                    while (currRow <= maxRows)
                    {
                        string rowVal = getTextFromData(values.GetDirect(currRow, columnName), columns[i], user);
                        double colValMeasure = tmBody.MeasureString(rowVal, UnitType.Point).Width + this.table.Borders.Width + Unit.FromCentimeter(0.25).Point;
                        totalRowWidth += colValMeasure;
                        currRow++;
                    }

                    double columnWidth = (totalRowWidth + minColWidth) / (maxRows + 1);

                    // Enforce minimum column width
                    if (columnWidth < minColWidth)
                        columnWidth = minColWidth;

                    // Create column
                    Column column = this.table.AddColumn(Unit.FromPoint(columnWidth));
                    column.Format.Alignment = ParagraphAlignment.Left;

                    // Add column width to total columns width
                    totalColWidth += columnWidth;
                }

                // Scale column width to the maximum table width
                // Scale factor < 1 - reduction
                // Scale factor > 1 - enlargement
                double scaleFactor = Math.Max(TableMaxWidth / totalColWidth, WidthScaleFactorMin);
                for (int i = 0; i < columns.Count; i++)
                {
                    this.table.Columns[i].Width = this.table.Columns[i].Width * scaleFactor;
                }

                // Create the header of the table
                Row row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.DodgerBlue;
                row.Format.Font.Color = Colors.White;

                this.table.SetEdge(0, 0, columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

                for (int c = 0; c < columns.Count; c++)
                {
                    row.Cells[c].VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[c].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[c], columns[c].Description));
                    /*
                    row.Cells[c].Format.Font.Bold = true;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                    row.Cells[0].MergeDown = 1;
                    */
                }
            }

            /// <summary>
            /// Creates the dynamic parts of the PDF document.
            /// </summary>
            private void FillContent(List<QColumn> columns, DataMatrix values, User user = null)
            {
                //run each row of the menu list
                for(int i = 0; i < values.NumRows; i++)
                {
                    Row rowPDF = this.table.AddRow();

                    if (rowPDF.Index % 2 == 0)
                    {
                        rowPDF.Shading.Color = Colors.LightGray;
                    }

                    this.table.SetEdge(0, this.table.Rows.Count - 1, columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75);

                    for (int c = 0; c < columns.Count; c++)
                    {
                        string text =  getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);

                        rowPDF.Cells[c].Format.FirstLineIndent = 0;
                        rowPDF.Cells[c].Format.LeftIndent = 0;
                        rowPDF.Cells[c].Format.RightIndent = 0.25;
                        rowPDF.Cells[c].AddParagraph(AdjustIfTooWideToFitIn(rowPDF.Cells[c], text));
                    }
                }
            }

            /// <summary>
            /// Adjust text value if it is too wide to fit inside the cell
            /// </summary>
            private string AdjustIfTooWideToFitIn(Cell cell, string text)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    Column column = cell.Column;
                    Unit availableWidth = column.Width - column.Table.Borders.Width - cell.Borders.Width - Unit.FromCentimeter(0.25).Point;

                    string[] splitted = text.Split(" ".ToCharArray());

                    Dictionary<string, string> distinct = new Dictionary<string, string>();
                    List<string> tooWideWords = new List<string>();
                    foreach (string s in splitted)
                        if (!distinct.ContainsKey(s))
                        {
                            distinct.Add(s, s);
                            if (TooWide(s, availableWidth))
                                tooWideWords.Add(s);
                        }

                    var adjusted = new StringBuilder(text);
                    foreach (string word in tooWideWords)
                    {
                        var replacementWord = MakeFit(word, availableWidth);
                        adjusted.Replace(word, replacementWord);
                    }

                    return adjusted.ToString();
                }
                else
                    return text;
            }

            /// <summary>
            /// Checks if the word is too wide
            /// </summary>
            private bool TooWide(string word, Unit width)
            {
                TextMeasurement tm = new TextMeasurement(DefaultGraphics, this.document.Styles["Table"].Font.Clone());
                double f = tm.MeasureString(word, UnitType.Point).Width;
                return f > width.Point;
            }

            /// <summary>
            /// Makes the supplied word fit into the available width
            /// </summary>
            /// <returns>modified version of the word with inserted Returns at appropriate points</returns>
            private string MakeFit(string word, Unit width)
            {
                var adjustedWord = new StringBuilder();
                var current = string.Empty;
                foreach (char c in word)
                {
                    if (TooWide(current + c, width))
                    {
                        adjustedWord.Append(current);
                        adjustedWord.Append(Chars.CR);
                        current = c.ToString();
                    }
                    else
                    {
                        current += c;
                    }
                }
                adjustedWord.Append(current);

                return adjustedWord.ToString();
            }
        }

        private class ExportToExcel
        {
            private double CentimeterToPixel(double Centimeter)
            {
                double pixel = -1;
                pixel = (Centimeter * 300) / 2.5399999d;
                return (double)pixel;
            }

            /// <summary>
            /// Generate Excel document with table content.
            /// </summary>
            public byte[] GetExcel(string fileName, DataMatrix values, List<QColumn> columns, string namedbedit, User user = null)
            {
                //temporary file path
                string DocumentPath = AppDomain.CurrentDomain.BaseDirectory + @"\temp\" + fileName;
                FileInfo ExcelFile = new FileInfo(DocumentPath);
                ExcelPackage xmlPackage = new ExcelPackage(ExcelFile);
                //create a worksheet
                ExcelWorksheet worksheet = xmlPackage.Workbook.Worksheets.Add("Excel");
                worksheet.HeaderFooter.FirstHeader.CenteredText = string.Format("&12&\"Arial,Regular Bold\" {0}", namedbedit);

                //fill first row with the columns name
                for (int i = 0; i < columns.Count; i++)
                {
                    //double tableWidth = 27.5; //Vamos assumir a largura de uma pagina A4 menos 2,2cm de margens. Ou seja 29.7 - 2.2 = 27.5cm
                    //double colPercent = (columns[i].Size * tableWidth) / QColumn.Sum(columns);
                    //worksheet.Column(i + 1).Width = CentimeterToPixel(colPercent);
                    //worksheet.Column(i + 1).AutoFit();
                    worksheet.Cells[1, i + 1].Value = columns[i].Description;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 141, 210));
                    worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Font.Size = 11;
                    worksheet.Cells[1, i + 1].Style.Font.Name = "Arial";
                }
                int idx = 2;
                //run each row of the menu list
                for(int i = 0; i < values.NumRows; i++)
                {
                    for (int c = 0; c < columns.Count; c++)
                    {
                        if (idx % 2 != 0)
                        {
                            worksheet.Cells[idx, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[idx, c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(241, 244, 249));
                        }

                        worksheet.Cells[idx, c + 1].Style.Font.Size = 10;
                        worksheet.Cells[idx, c + 1].Style.Font.Name = "Arial";

                        // If the value is null, column type is disregarded
                        if (values.GetDirect(i, columns[c].Name) == null || values.GetDirect(i, columns[c].Name) == DBNull.Value)
                        {
                            worksheet.Cells[idx, c + 1].Value = null;
                            continue;
                        }

                        // Verify column type
                        switch (columns[c].Type.Formatting)
                        {
                            case FieldFormatting.DATA:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.Date;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.DATAHORA:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.DateTime;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.DATASEGUNDO:
                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = Configuration.DateFormat.DateTimeSeconds;
                                worksheet.Cells[idx, c + 1].Value = values.GetDate(i, columns[c].Name).ToOADate();
                                break;

                            case FieldFormatting.INTEIRO:
                            case FieldFormatting.LOGICO:
                                // Ignore logical arrays
                                if (columns[c].Type == FieldType.ARRAY_COD_LOGICO)
                                   goto default;

                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = "0";
                                worksheet.Cells[idx, c + 1].Value = values.GetInteger(i, columns[c].Name);
                                break;

                            case FieldFormatting.FLOAT:
                                // Ignore numeric arrays
                                if (columns[c].Type == FieldType.ARRAY_COD_NUMERICO)
                                   goto default;

                                // Excel already changes number separators for localization
                                string numberFormat = "0";
                                int decimais = columns[c].Decimals;

								//BPM - Add 2 decimals for defaults in the money
                                if (columns[c].Type.Id == "$")
                                    decimais += 2;

                                if (decimais > 0)
                                {
                                    numberFormat += "." + new string('0', decimais);
                                }

                                worksheet.Cells[idx, c + 1].Style.Numberformat.Format = numberFormat;


                                worksheet.Cells[idx, c + 1].Value = values.GetNumeric(i, columns[c].Name);
                                break;

                            case FieldFormatting.CARACTERES:
                            default:
                                string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                                worksheet.Cells[idx, c + 1].Value = text;
                                break;
                        }
                    }
                    idx++;
                }

                // Autofit columns to data values
                worksheet.Cells.AutoFitColumns();

                return xmlPackage.GetAsByteArray();
            }
        }

        private class ExportToODS
        {
            // Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
            private static string[,] namespaces = new string[,]
            {
                {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
                {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
                {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
                {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},
                {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
                {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
                {"dc", "http://purl.org/dc/elements/1.1/"},
                {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
                {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
                {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
                {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
                {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
                {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
                {"math", "http://www.w3.org/1998/Math/MathML"},
                {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
                {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
                {"ooo", "http://openoffice.org/2004/office"},
                {"ooow", "http://openoffice.org/2004/writer"},
                {"oooc", "http://openoffice.org/2004/calc"},
                {"dom", "http://www.w3.org/2001/xml-events"},
                {"xforms", "http://www.w3.org/2002/xforms"},
                {"xsd", "http://www.w3.org/2001/XMLSchema"},
                {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
                {"rpt", "http://openoffice.org/2005/report"},
                {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
                {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
                {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
            };

            // Read zip stream (.ods file is zip file).
            private ZipFile GetZipFile(Stream stream)
            {
                return ZipFile.Read(stream);
            }

            // Read zip file (.ods file is zip file).
            private ZipFile GetZipFile(string inputFilePath)
            {
                return ZipFile.Read(inputFilePath);
            }

            private XmlDocument GetContentXmlFile(ZipFile zipFile)
            {
                // Get file(in zip archive) that contains data ("content.xml").
                ZipEntry contentZipEntry = zipFile["content.xml"];

                // Extract that file to MemoryStream.
                Stream contentStream = new MemoryStream();
                contentZipEntry.Extract(contentStream);
                contentStream.Seek(0, SeekOrigin.Begin);

                // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
                XmlDocument contentXml = new XmlDocument();
                contentXml.Load(contentStream);

                return contentXml;
            }

            private XmlNamespaceManager InitializeXmlNamespaceManager(XmlDocument xmlDocument)
            {
                XmlNamespaceManager nmsManager = new XmlNamespaceManager(xmlDocument.NameTable);

                for (int i = 0; i < namespaces.GetLength(0); i++)
                    nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

                return nmsManager;
            }

            // In ODF sheet is stored in table:table node
            private XmlNodeList GetTableNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
            {
                return contentXmlDocument.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
            }

            /// <summary>
            /// Generate OpenDocument SpreadSheet file with table content.
            /// </summary>
            public byte[] GetOds(DataMatrix values, List<QColumn> columns, User user = null)
            {
                ZipFile templateFile = this.GetZipFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("CSGenio.core.resources.Exports.template.ods"));

                XmlDocument contentXml = this.GetContentXmlFile(templateFile);

                XmlNamespaceManager nmsManager = this.InitializeXmlNamespaceManager(contentXml);

                XmlNode automaticStylesNode = this.GetAutomaticStylesNode(contentXml, nmsManager);

                this.CreateColumnsStyles(automaticStylesNode, columns.Count);
                this.CreateRowsStyles(automaticStylesNode);

                XmlNode sheetsRootNode = this.GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

                this.SaveSheet(values, sheetsRootNode, columns, user);

                this.SaveContentXml(templateFile, contentXml);

                byte[] buffer;
                using (MemoryStream ms = new MemoryStream())
                {
                    templateFile.Save(ms);
                    buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);
                }

                return buffer;
            }

            private XmlNode GetAutomaticStylesNode(XmlDocument contentXml, XmlNamespaceManager nmsManager)
            {
                return contentXml.SelectNodes("/office:document-content/office:automatic-styles", nmsManager).Item(0);
            }

            private void CreateColumnsStyles(XmlNode automaticStylesNode, int numColumns)
            {
                XmlDocument ownerDocument = automaticStylesNode.OwnerDocument;

                for (int i = 0; i < numColumns; i++)
                {
                    XmlElement style = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));

                    XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
                    styleName.Value = "co" + (i + 2).ToString();
                    style.Attributes.Append(styleName);

                    XmlAttribute styleFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
                    styleFamily.Value = "table-column";
                    style.Attributes.Append(styleFamily);

                    XmlElement tableColumnProperties = ownerDocument.CreateElement("style:table-column-properties", this.GetNamespaceUri("style"));

                    XmlAttribute foBreakBefore = ownerDocument.CreateAttribute("fo:break-before", this.GetNamespaceUri("fo"));
                    foBreakBefore.Value = "auto";
                    tableColumnProperties.Attributes.Append(foBreakBefore);

                    //XmlAttribute styleColumnWidth = ownerDocument.CreateAttribute("style:column-width", this.GetNamespaceUri("style"));
                    //double tableWidth = 27.5; //Vamos assumir a largura de uma pagina A4 menos 2,2cm de margens. Ou seja 29.7 - 2.2 = 27.5cm
                    //double colPercent = (col.Size * tableWidth) / QColumn.Sum(columns);
                    //styleColumnWidth.Value = Math.Round(colPercent, 2).ToString().Replace(",", ".") + "cm";
                    //styleColumnWidth.Value = "auto";
                    //tableColumnProperties.Attributes.Append(styleColumnWidth);

                    XmlAttribute styleAutoWidth = ownerDocument.CreateAttribute("style:use-optimal-column-width", this.GetNamespaceUri("style"));
                    styleAutoWidth.Value = "true";
                    tableColumnProperties.Attributes.Append(styleAutoWidth);

                    style.AppendChild(tableColumnProperties);

                    automaticStylesNode.AppendChild(style);
                }
            }

            private void CreateRowsStyles(XmlNode automaticStylesNode)
            {
                XmlDocument ownerDocument = automaticStylesNode.OwnerDocument;

                #region Style Cell Header
                XmlElement styleHeader = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
                XmlAttribute styleName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
                styleName.Value = "ce1";
                styleHeader.Attributes.Append(styleName);
                XmlAttribute styleFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
                styleFamily.Value = "table-cell";
                styleHeader.Attributes.Append(styleFamily);
                XmlAttribute parentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
                parentStyleName.Value = "Default";
                styleHeader.Attributes.Append(parentStyleName);

                XmlElement tableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
                XmlAttribute foBackColor = ownerDocument.CreateAttribute("fo:background-color", this.GetNamespaceUri("fo"));
                foBackColor.Value = "#008DD2";
                tableCellProperties.Attributes.Append(foBackColor);
                XmlAttribute textAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
                textAlignSource.Value = "fix";
                tableCellProperties.Attributes.Append(textAlignSource);
                XmlAttribute repeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
                repeatContent.Value = "false";
                tableCellProperties.Attributes.Append(repeatContent);
                XmlAttribute foBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
                //foBorder.Value = "0.002cm solid #000000";
                tableCellProperties.Attributes.Append(foBorder);

                XmlElement paragraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
                XmlAttribute textAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
                textAlign.Value = "center";
                paragraphProperties.Attributes.Append(textAlign);
                XmlAttribute marginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
                marginLeft.Value = "0cm";
                paragraphProperties.Attributes.Append(marginLeft);

                XmlElement textProperties = ownerDocument.CreateElement("style:text-properties", this.GetNamespaceUri("style"));
                XmlAttribute color = ownerDocument.CreateAttribute("fo:color", this.GetNamespaceUri("fo"));
                color.Value = "#FFFFFF";
                textProperties.Attributes.Append(color);
                #endregion

                styleHeader.AppendChild(tableCellProperties);
                styleHeader.AppendChild(paragraphProperties);
                styleHeader.AppendChild(textProperties);
                automaticStylesNode.AppendChild(styleHeader);

                #region Style Cell Body
                XmlElement styleBody = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
                XmlAttribute styleBodyName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
                styleBodyName.Value = "ce2";
                styleBody.Attributes.Append(styleBodyName);
                XmlAttribute styleBodyFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
                styleBodyFamily.Value = "table-cell";
                styleBody.Attributes.Append(styleBodyFamily);
                XmlAttribute styleBodyParentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
                styleBodyParentStyleName.Value = "Default";
                styleBody.Attributes.Append(styleBodyParentStyleName);

                XmlElement styleBodyTableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
                XmlAttribute styleBodyTextAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
                styleBodyTextAlignSource.Value = "fix";
                styleBodyTableCellProperties.Attributes.Append(styleBodyTextAlignSource);
                XmlAttribute styleBodyRepeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
                styleBodyRepeatContent.Value = "false";
                styleBodyTableCellProperties.Attributes.Append(styleBodyRepeatContent);
                XmlAttribute styleBodyFoBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
                //styleBodyFoBorder.Value = "0.002cm solid #000000";
                styleBodyTableCellProperties.Attributes.Append(styleBodyFoBorder);

                XmlElement styleBodyParagraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
                XmlAttribute styleBodyTextAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
                styleBodyTextAlign.Value = "start";
                styleBodyParagraphProperties.Attributes.Append(styleBodyTextAlign);
                XmlAttribute styleBodyMarginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
                styleBodyMarginLeft.Value = "0cm";
                styleBodyParagraphProperties.Attributes.Append(styleBodyMarginLeft);
                #endregion

                styleBody.AppendChild(styleBodyTableCellProperties);
                styleBody.AppendChild(styleBodyParagraphProperties);
                automaticStylesNode.AppendChild(styleBody);

                #region Style Alternative Cell Body
                XmlElement altstyleBody = ownerDocument.CreateElement("style:style", this.GetNamespaceUri("style"));
                XmlAttribute altstyleBodyName = ownerDocument.CreateAttribute("style:name", this.GetNamespaceUri("style"));
                altstyleBodyName.Value = "ce3";
                altstyleBody.Attributes.Append(altstyleBodyName);
                XmlAttribute altstyleBodyFamily = ownerDocument.CreateAttribute("style:family", this.GetNamespaceUri("style"));
                altstyleBodyFamily.Value = "table-cell";
                altstyleBody.Attributes.Append(styleBodyFamily);
                XmlAttribute altstyleBodyParentStyleName = ownerDocument.CreateAttribute("style:parent-style-name", this.GetNamespaceUri("style"));
                altstyleBodyParentStyleName.Value = "Default";
                altstyleBody.Attributes.Append(altstyleBodyParentStyleName);

                XmlElement altstyleBodyTableCellProperties = ownerDocument.CreateElement("style:table-cell-properties", this.GetNamespaceUri("style"));
                XmlAttribute altfoBackColor = ownerDocument.CreateAttribute("fo:background-color", this.GetNamespaceUri("fo"));
                altfoBackColor.Value = "#d3d3d3";
                altstyleBodyTableCellProperties.Attributes.Append(altfoBackColor);
                XmlAttribute altstyleBodyTextAlignSource = ownerDocument.CreateAttribute("style:text-align-source", this.GetNamespaceUri("style"));
                altstyleBodyTextAlignSource.Value = "fix";
                altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyTextAlignSource);
                XmlAttribute altstyleBodyRepeatContent = ownerDocument.CreateAttribute("style:repeat-content", this.GetNamespaceUri("style"));
                altstyleBodyRepeatContent.Value = "false";
                altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyRepeatContent);
                XmlAttribute altstyleBodyFoBorder = ownerDocument.CreateAttribute("fo:border", this.GetNamespaceUri("fo"));
                //altstyleBodyFoBorder.Value = "0.002cm solid #000000";
                altstyleBodyTableCellProperties.Attributes.Append(altstyleBodyFoBorder);

                XmlElement altstyleBodyParagraphProperties = ownerDocument.CreateElement("style:paragraph-properties", this.GetNamespaceUri("style"));
                XmlAttribute altstyleBodyTextAlign = ownerDocument.CreateAttribute("fo:text-align", this.GetNamespaceUri("fo"));
                altstyleBodyTextAlign.Value = "start";
                altstyleBodyParagraphProperties.Attributes.Append(altstyleBodyTextAlign);
                XmlAttribute altstyleBodyMarginLeft = ownerDocument.CreateAttribute("fo:margin-left", this.GetNamespaceUri("fo"));
                altstyleBodyMarginLeft.Value = "0cm";
                altstyleBodyParagraphProperties.Attributes.Append(altstyleBodyMarginLeft);
                #endregion

                altstyleBody.AppendChild(altstyleBodyTableCellProperties);
                altstyleBody.AppendChild(altstyleBodyParagraphProperties);
                automaticStylesNode.AppendChild(altstyleBody);
            }

            private XmlNode GetSheetsRootNodeAndRemoveChildrens(XmlDocument contentXml, XmlNamespaceManager nmsManager)
            {
                XmlNodeList tableNodes = this.GetTableNodes(contentXml, nmsManager);

                XmlNode sheetsRootNode = tableNodes.Item(0).ParentNode;
                // remove sheets from template file
                foreach (XmlNode tableNode in tableNodes)
                    sheetsRootNode.RemoveChild(tableNode);

                return sheetsRootNode;
            }

            private void SaveSheet(DataMatrix values, XmlNode sheetsRootNode, List<QColumn> columns, User user = null)
            {
                XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;
                XmlNode sheetNode = ownerDocument.CreateElement("table:table", this.GetNamespaceUri("table"));

                XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", this.GetNamespaceUri("table"));
                sheetName.Value = "Folha";
                sheetNode.Attributes.Append(sheetName);

                XmlAttribute styleName = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                styleName.Value = "ta1";
                sheetNode.Attributes.Append(styleName);

                XmlAttribute print = ownerDocument.CreateAttribute("table:print", this.GetNamespaceUri("table"));
                print.Value = "false";
                sheetNode.Attributes.Append(print);

                this.SaveColumnDefinition(columns.Count, sheetNode, ownerDocument);

                this.CreateTableHeaders(sheetNode, ownerDocument, columns);

                this.SaveRows(values, columns, sheetNode, ownerDocument, user);

                sheetsRootNode.AppendChild(sheetNode);
            }

            private void SaveColumnDefinition(int numColumns, XmlNode sheetNode, XmlDocument ownerDocument)
            {
                for (int i = 0; i < numColumns; i++)
                {
                    XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", this.GetNamespaceUri("table"));

                    XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                    styleCell.Value = "co" + (i + 2).ToString();
                    columnDefinition.Attributes.Append(styleCell);

                    XmlAttribute defaultStyleCell = ownerDocument.CreateAttribute("table:default-cell-style-name", this.GetNamespaceUri("table"));
                    defaultStyleCell.Value = "ce2";
                    columnDefinition.Attributes.Append(defaultStyleCell);

                    sheetNode.AppendChild(columnDefinition);
                }
            }

            private void CreateTableHeaders(XmlNode sheetNode, XmlDocument ownerDocument, List<QColumn> columns)
            {
                XmlNode rowNode = ownerDocument.CreateElement("table:table-row", this.GetNamespaceUri("table"));
                XmlAttribute style = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                style.Value = "ro1";
                rowNode.Attributes.Append(style);

                foreach (var col in columns)
                {
                    XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", this.GetNamespaceUri("table"));

                    XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                    styleCell.Value = "ce1";
                    cellNode.Attributes.Append(styleCell);

                    XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", this.GetNamespaceUri("office"));
                    valueType.Value = "string";
                    cellNode.Attributes.Append(valueType);

                    XmlElement cellValue = ownerDocument.CreateElement("text:p", this.GetNamespaceUri("text"));
                    cellValue.InnerText = col.Description;
                    cellNode.AppendChild(cellValue);

                    rowNode.AppendChild(cellNode);
                }

                sheetNode.AppendChild(rowNode);
            }

            private void SaveRows(DataMatrix values, List<QColumn> columns, XmlNode sheetNode, XmlDocument ownerDocument, User user = null)
            {
                for (int i = 0; i < values.NumRows; i++)
                {
                    XmlNode rowNode = ownerDocument.CreateElement("table:table-row", this.GetNamespaceUri("table"));
                    XmlAttribute style = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                    style.Value = "ro1";
                    rowNode.Attributes.Append(style);

                    for (int c = 0; c < columns.Count; c++)
                    {
                        this.SaveCell(values.GetDirect(i, columns[c].Name), columns[c], rowNode, ownerDocument, i, user);
                    }

                    sheetNode.AppendChild(rowNode);
                }
            }

            private void SaveCell(object value, QColumn column, XmlNode rowNode, XmlDocument ownerDocument, int rowIndex, User user = null)
            {
                XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", this.GetNamespaceUri("table"));

                XmlAttribute styleCell = ownerDocument.CreateAttribute("table:style-name", this.GetNamespaceUri("table"));
                styleCell.Value = (rowIndex % 2 == 0 ? "ce2" : "ce3");
                cellNode.Attributes.Append(styleCell);

                // We save values as text (string)
                XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", this.GetNamespaceUri("office"));
                valueType.Value = "string";
                cellNode.Attributes.Append(valueType);

                XmlElement cellValue = ownerDocument.CreateElement("text:p", this.GetNamespaceUri("text"));
                cellValue.InnerText = getTextFromData(value, column, user);
                cellNode.AppendChild(cellValue);

                rowNode.AppendChild(cellNode);
            }

            private void SaveContentXml(ZipFile templateFile, XmlDocument contentXml)
            {
                templateFile.RemoveEntry("content.xml");

                MemoryStream memStream = new MemoryStream();
                contentXml.Save(memStream);
                memStream.Seek(0, SeekOrigin.Begin);

                templateFile.AddEntry("content.xml", memStream);
            }

            private string GetNamespaceUri(string prefix)
            {
                for (int i = 0; i < namespaces.GetLength(0); i++)
                {
                    if (namespaces[i, 0] == prefix)
                        return namespaces[i, 1];
                }

                throw new InvalidOperationException("Can't find that namespace URI");
            }
        }

        private class ExportToCSV
        {
            /// <summary>
            /// Função que converte uma string com characters de quebras de linha to um Qvalue string válido
            /// </summary>
            /// <param name="valorCampo">Qvalue do Qfield</param>
            /// <returns>Qfield string formatado</returns>
            private string memo2String(string Qvalue)
            {
                if (Qvalue.Contains(";"))
                    Qvalue = Qvalue.Replace(";", ",");

                if (Qvalue.Contains("\n\r\n"))
                    Qvalue = Qvalue.Replace("\n\r\n", " ");

                if (Qvalue.Contains("\n\r"))
                    Qvalue = Qvalue.Replace("\n\r", " ");

                if (Qvalue.Contains("\r\n"))
                    Qvalue = Qvalue.Replace("\r\n", " ");

                if (Qvalue.Contains("\n"))
                    Qvalue = Qvalue.Replace("\n", " ");

                if (Qvalue.Contains("\r"))
                    Qvalue = Qvalue.Replace("\r", " ");

                return Qvalue;
            }

            /// <summary>
            /// Generate CSV file with table content.
            /// </summary>
            public byte[] GetCSV(DataMatrix values, List<QColumn> columns, User user = null)
            {
                StringBuilder conteudoCSV = new StringBuilder();

                foreach (var col in columns)
                    conteudoCSV.Append(col.Description + ";");

                conteudoCSV.Append(";\r\n");

                //preenche a table com os dados
                for (int i = 0; i < values.NumRows; i++)
                {
                    for (int c = 0; c < columns.Count; c++)
                    {
                        string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                        conteudoCSV.Append(memo2String(text) + ";");
                    }
                    conteudoCSV.Append(";\r\n");
                }

                byte[] buffer;
                using (MemoryStream ms = new MemoryStream())
                {
                    var stringBytes = System.Text.Encoding.Default.GetBytes(conteudoCSV.ToString());
                    ms.Write(stringBytes, 0, stringBytes.Length);
                    buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);
                }

                return buffer;
            }
        }

        private class ExportToXML
        {
            /// <summary>
            /// Generate XML file with table content.
            /// </summary>
            public byte[] GetXML(DataMatrix values, List<QColumn> columns, User user = null)
            {
                List<string> tagname = new List<string>();
                foreach (var col in columns)
                    tagname.Add(col.Description);

                using (MemoryStream stream = new MemoryStream())
                {
                    XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Table");
                    for(int i = 0; i < values.NumRows; i++)
                    {
                        List<string> Qvalues = new List<string>();
                        for (int c = 0; c < columns.Count; c++)
                        {
                            string text = getTextFromData(values.GetDirect(i, columns[c].Name), columns[c], user);
                            Qvalues.Add(text);
                        }
                        CreateNode(tagname,Qvalues, writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();

                    byte[] byteArray = stream.ToArray();
                    writer.Close();

                    return byteArray;
                }
            }

            /// <summary>
            /// Create a XML Node
            /// </summary>
            private void CreateNode(List<string> tags, List<string> Qvalues, XmlTextWriter writer)
            {
                writer.WriteStartElement("Registo");
                for (int i = 0; i < tags.Count; i++ )
                {
                    writer.WriteStartElement(tags[i].Replace(" ","_"));
                    writer.WriteString(Qvalues[i]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

        }

        private static string getTextFromData(object data, QColumn column, User user = null)
        {
			string lang = "";
			if(user != null)
				lang = user.Language;
            string text = Conversion.internal2String(data, column.Type);
            if ((column.Type == FieldType.ARRAY_COD_TEXTO || column.Type == FieldType.ARRAY_COD_NUMERICO || column.Type == FieldType.ARRAY_COD_LOGICO)
                && !String.IsNullOrEmpty(column.ArrayName) && !String.IsNullOrEmpty(text))
            {
                ArrayInfo array = new ArrayInfo(column.ArrayName);
                if (array.Elements.Contains(text))// MH [21/03/2016] - Validação se o código exists. Caso contrario provoca erro de execução.
                    text = array.GetDescription(text, /*dbSearch.Language*/lang);
                else text = string.Empty;
            }

            return text;
        }

        #endregion

       #region Import

        public List<A> ImportList<A>(List<Exports.QColumn> columns, string exportType, byte[] file) where A : IArea
        {
            ExportType importType = getExportType(exportType);
            List<object[]> rows = new List<object[]>();
            int rowCount = 1;
            List<A> results = new List<A>();

			//import by file Type
            switch (importType)
            {
                case ExportType.xlsx:
                    rows =  ImportExcel(columns, file, ref rowCount);
                    break;
            }

            if(!this.CheckIfRightFile(rows[0], columns))
            {
                throw new FrameworkException("Ficheiro com cabeçalho incorrecto",
                    "Exports.ImportarListagem", "Ficheiro com cabeçalho incorrecto");
            }

            Dictionary<String, List<Exports.QColumn>> columnsByArea = this.GetColumnsByArea(columns);


            //Process values into DbArea models
            rows.RemoveAt(0);//Start at 1 to avoid reading Header
            foreach (object[] row in rows)
            {
                //List to prevent more than one querie per upper table
                List<string> importedUpperTables = new List<string>();

                A area = (A)Activator.CreateInstance(typeof(A), user);
                for (int col = 0; col < columns.Count; col++)
                {
                    object value = row[col];
                    if (value == null)
                        continue;


                    string fieldBaseArea = columns[col].BaseArea;

                    //Check if foreign Key
                    if (fieldBaseArea != area.Alias )
                    {
                        if (importedUpperTables.Contains(fieldBaseArea)) // upper table searched already
                            continue;

                        //JGF 2022.03.24 Get the foreign key name and not the primary key of the other area
                        var relation = area.ParentTables[fieldBaseArea];
                        string alias = area.Alias + '.' + relation.SourceRelField;

                        importedUpperTables.Add(fieldBaseArea);
                        List <QColumn> searchColumns= columnsByArea[fieldBaseArea];

                        //Get values from this row that come from upper table
                        List<object> upperValues = this.GetValuesFromRow(row, columns, fieldBaseArea);

                        //Get value from Above table with WHERE clause with all fields
                        AreaInfo parentTable = business.Area.GetInfoArea(fieldBaseArea);
                        value = this.ImportFromParent(upperValues, parentTable, searchColumns);

                        area.insertNameValueField(alias, value);
                    } else
                    {
                        area.insertNameValueField(columns[col].Name, value);
                    }


                }
                results.Add(area);
            }

            return results;
        }

        private Dictionary<String, List<Exports.QColumn>> GetColumnsByArea(List<Exports.QColumn> columns)
        {
            Dictionary<String, List<Exports.QColumn>> areas = new Dictionary<String, List<Exports.QColumn>>();
            foreach (Exports.QColumn column in columns)
            {
                if(areas.ContainsKey(column.BaseArea))
                {
                    List<Exports.QColumn> columnsByArea = areas[column.BaseArea];
                    columnsByArea.Add(column);
                } else
                {
                    List<Exports.QColumn> columnsByArea = new List<Exports.QColumn> { column };
                    areas.Add(column.BaseArea, columnsByArea);
                }
            }

            return areas;
        }

        private bool CheckIfRightFile(object[] values, List<Exports.QColumn> columns)
        {
			//Check file header for columns descriptions
            for (int col = 0; col < columns.Count; col++)
            {
                object value = values[col];
                if(value == null || value.ToString() != columns[col].Description)
                {
                    return false;
                }
            }

            return true;
        }

        private List<object> GetValuesFromRow(object[] row, List<QColumn> searchColumns, string fieldBaseArea)
        {
            List<object> upperValues = new List<object>();
            for (int col = 0; col < searchColumns.Count; col++)
            {
                if (searchColumns[col].BaseArea == fieldBaseArea)
                {
                    upperValues.Add(row[col]);
                }
            }

            return upperValues;
        }

        private object ImportFromParent(List<object> upperValues, AreaInfo parentTable, List<QColumn> searchColumns)
        {
            //There should be a cache here to prevent repetitive queries
            object value = null;

            //Check if Find is necessary
            if (upperValues.Count > 0)
            {
				string parentName = parentTable.Alias;
                SelectQuery qs = new SelectQuery();

                qs.Select(parentName, parentTable.PrimaryKeyName);
                qs.From(parentTable.QSystem, parentTable.TableName, parentTable.Alias);
                CriteriaSet where = CriteriaSet.And();

                for (int col = 0; col < searchColumns.Count; col++)
                {
                    where.Equal(parentName, searchColumns[col].FieldName, upperValues[col]);
                }

                qs.Where(where);
                qs.PageSize(1);

                PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

                //get data
                sp.openConnection();
                DataMatrix res = sp.Execute(qs);
                sp.closeConnection();

				//If results, get Key
                if (res.NumRows > 0)
                {
                    value = res.GetDirect(0, 0);
                }
            }

            return value;
        }

        private List<object[]> ImportExcel(List<Exports.QColumn> columns, byte[] file, ref int rowCount)
        {
            int columnCount = columns.Count;
            List<object[]> results =  new List<object[]>();

            MemoryStream memStream = new MemoryStream(file);
            using (var package = new ExcelPackage(memStream))
            {
                int firstIndex = package.Compatibility.IsWorksheets1Based ? 1 : 0;
                var currentSheet = package.Workbook.Worksheets[firstIndex];

                rowCount = currentSheet.Dimension.End.Row;// Here is where my issue is


                for (int rowIterator = 0; rowIterator < rowCount; rowIterator++)
                {
                    object[] row = new object[columnCount];
                    for(int colIterator = 0; colIterator < columnCount; colIterator++){
                        //TODO: Parse by Type
                        row[colIterator] = currentSheet.Cells[rowIterator + 1, colIterator + 1].Value;
                    }
                    results.Add(row);
                }
            }

            return results;
        }


        #endregion
    }
}
