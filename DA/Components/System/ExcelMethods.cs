using ClosedXML.Excel;
using System.Data;
using System.Xml;

namespace DA.Components.System
{
    public static class ExcelMethods
    {
        public static string ChangeXMLChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        public static void CreateExcelFile(int listCount, string path, string reportName, DataTable dataTable)
        {
            #region Excel Dosyasını Oluştur ve İndir

            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            string fileName = string.Format("{0}_{1}.xlsm", reportName, DateTime.Now.ToString("ddMMyyyyHHmmss"));
            string filePath = path + fileName;
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(dataTable, "Report");
            int ColumnRange = listCount + 2;
            ws.Range("D2", "D" + ColumnRange).Style.Alignment.WrapText = true;
            ws.Columns(1, 20).AdjustToContents();
            ws.Column(4).Width = 150;
            wb.SaveAs(filePath);
            #endregion

        }
    }
}
