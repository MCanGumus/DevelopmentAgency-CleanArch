using FastReport;
using DA.Models;
using System.Drawing;

namespace DA.Components.System
{
    public class FastReportClass
    {
        IWebHostEnvironment _webHostEnvironment;

        public FastReportClass(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string GetReport(string Reportname, string createdReportName, string DataSetName, string barcode, string Subject, string Title, List<Parameters> parameters, string logoPath)
        {
            try
            {
                string directory = _webHostEnvironment.WebRootPath;

                Report report = new Report();
                string report_path = Path.Combine(directory, "Reports/");
                string reportName = string.Empty;
                report.Load(report_path + Reportname);
                //result.TableName = DataSetName;
                //report.RegisterData((IEnumerable<object>)lst, DataSetName);

                foreach (Parameters item in parameters)
                    report.SetParameterValue(item.key, item.value);

                //Yazı tekliflerinde Picture2 kullanılmadığı için bu kontrolü yaptım 

                //PictureObject pic = new PictureObject();
                //pic = report.FindObject("Picture1") as PictureObject;

                //if (pic != null)
                //{
                //    pic.Image = new Bitmap(logoPath);
                //}

                if (report.Report.Prepare())
                {
                    FastReport.Utils.Config.WebMode = true;

                    // Set PDF export props
                    FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Subject = Subject;
                    pdfExport.Title = Title;
                    //pdfExport.Compressed = true;
                    //pdfExport.AllowPrint = true;
                    //pdfExport.EmbeddingFonts = false;
                    pdfExport.Author = "DA";
                    //pdfExport.Creator = "Orjeen";

                    var strm = new MemoryStream();
                    report.Export(pdfExport, strm);
                    report.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    reportName = createdReportName;
                    if (File.Exists(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf"))
                    {
                        File.Delete(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf");
                    }
                    var fileStream = new FileStream(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf", FileMode.Create, FileAccess.Write);
                    strm.CopyTo(fileStream);
                    fileStream.Dispose();
                }
                return (reportName + ".pdf");
            }
            catch (Exception ex)
            {
                return "HATA" + ex.Message;
            }
        }

        public string GetReportWithTable(string Reportname, string createdReportName, string DataSetName, string barcode, string Subject, string Title, List<Parameters> parameters, object lst, string logoPath)
        {
            try
            {
                string directory = _webHostEnvironment.WebRootPath;

                Report report = new Report();
                string report_path = Path.Combine(directory, "Reports/");
                string reportName = string.Empty;
                report.Load(report_path + Reportname);
               // result.TableName = DataSetName;
                report.RegisterData((IEnumerable<object>)lst, DataSetName);

                foreach (Parameters item in parameters)
                    report.SetParameterValue(item.key, item.value);

                //Yazı tekliflerinde Picture2 kullanılmadığı için bu kontrolü yaptım 

                //PictureObject pic = new PictureObject();
                //pic = report.FindObject("Picture1") as PictureObject;

                //if (pic != null)
                //{
                //    pic.Image = new Bitmap(logoPath);
                //}

                if (report.Report.Prepare())
                {
                    FastReport.Utils.Config.WebMode = true;

                    // Set PDF export props
                    FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Subject = Subject;
                    pdfExport.Title = Title;
                    //pdfExport.Compressed = true;
                    //pdfExport.AllowPrint = true;
                    //pdfExport.EmbeddingFonts = false;
                    pdfExport.Author = "DA";
                    //pdfExport.Creator = "Orjeen";

                    var strm = new MemoryStream();
                    report.Export(pdfExport, strm);
                    report.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    reportName = createdReportName;
                    if (File.Exists(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf"))
                    {
                        File.Delete(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf");
                    }
                    var fileStream = new FileStream(Path.Combine(directory, "Files/Reports/") + reportName + ".pdf", FileMode.Create, FileAccess.Write);
                    strm.CopyTo(fileStream);
                    fileStream.Dispose();
                }
                return (reportName + ".pdf");
            }
            catch (Exception ex)
            {
                return "HATA" + ex.Message;
            }
        }


    }
}

