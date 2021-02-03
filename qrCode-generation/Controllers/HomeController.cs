
using QRCoder;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using qrCode_generation.Models;
using LumenWorks.Framework.IO.Csv;
using System.Web.Mvc;
using System.Data;

namespace qrCode_generation.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();


        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {
                
                if (file != null && file.ContentLength > 0)
                {
                    PdfDocument doc = new PdfDocument();

                    PdfPageBase page = doc.Pages.Add();
                    PdfGrid grid = new PdfGrid();
                    PdfGridRow row = grid.Rows.Add();
                    row = grid.Rows.Add();
                    float width = page.Canvas.ClientSize.Width - (grid.Columns.Count + 1);
                    grid.Columns.Add(5);
                    grid.Style.CellPadding = new PdfPaddings(1, 1, 1, 1);
                    grid.Columns[0].Width = width * 0.20f;

                    grid.Columns[1].Width = width * 0.20f;
                    grid.Columns[2].Width = width * 0.20f;
                    grid.Columns[3].Width = width * 0.20f;
                    grid.Columns[4].Width = width * 0.20f;

                    grid.Rows[0].Height = width * 0.15f;

                    grid.Rows[1].Height = width * 0.15f;

                    string outputPDFfile = "D:/qrPdf1.pdf";

                    var csvTable = new DataTable();

                    string resultLine = string.Empty;

                    using (BinaryReader b = new BinaryReader(file.InputStream))
                    {
                        byte[] binData = b.ReadBytes(file.ContentLength);
                        resultLine = System.Text.Encoding.UTF8.GetString(binData);
                        string[] csv = new string[] { };
                        List<string> csvData = new List<string>();

                        csv = resultLine.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                        for(int i=0;i<csv.Length;i++)
                        {
                            if(csv[i]=="")
                            {
                                break;
                            }
                            csvData.Add(csv[i]);
                            
                        }
                        for (var l = 0; l < csvData.Count-2; l++)
                        {

                            if (l < csvData.Count - 3)
                            {
                                row = grid.Rows.Add();
                                grid.Rows[l + 1].Height = grid.Rows[l].Height;
                            }
                            else grid.Rows[l].Height = grid.Rows[0].Height;
                        }
                        int z, g;
                        for (z = 0, g = 1; g < csvData.Count-1; z++,g++)
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(csvData[g], QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            Bitmap qrCodeImage = qrCode.GetGraphic(35);
                            var codeBitmap = new Bitmap(qrCodeImage);
                            Image image = (Image)codeBitmap;
                            PdfGridCellContentList lst = new PdfGridCellContentList();

                            PdfGridCellContent textAndStyle = new PdfGridCellContent();
                            PdfGridCellContent text = new PdfGridCellContent();
                            PdfGridCellContent full = new PdfGridCellContent();
                            textAndStyle.Image = PdfImage.FromImage(image);

                            textAndStyle.ImageSize = new SizeF(65, 65);

                            var codeNo = csvData[g].Split(new string[] { "," }, StringSplitOptions.None);
                            
                            text.Text = "\n" + codeNo[1];
               
                            lst.List.Add(textAndStyle);
                            lst.List.Add(text);
                            for (var k = 0; k < 5; k++)
                            {
                                
                                    grid.Rows[z].Cells[k].Value = lst;
                                
                            }
                            PdfLayoutResult result = grid.Draw(page, new PointF(10, 30));


                        }

                        doc.SaveToFile(outputPDFfile, FileFormat.PDF);

                        System.Diagnostics.Process.Start(outputPDFfile);


                    }

                }
  
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();


            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();

            }
        }







        //public ActionResult generateQrcode()
        //{

        //    PdfDocument doc = new PdfDocument();

        //    PdfPageBase page = doc.Pages.Add();
        //    PdfGrid grid = new PdfGrid();
        //    PdfGridRow row = grid.Rows.Add();
        //    row = grid.Rows.Add();
        //    float width = page.Canvas.ClientSize.Width - (grid.Columns.Count + 1);
        //    grid.Columns.Add(5);
        //    grid.Style.CellPadding = new PdfPaddings(1, 1, 1, 1);
        //    grid.Columns[0].Width = width * 0.20f;

        //    grid.Columns[1].Width = width * 0.20f;
        //    grid.Columns[2].Width = width * 0.20f;
        //    grid.Columns[3].Width = width * 0.20f;
        //    grid.Columns[4].Width = width * 0.20f;

        //    grid.Rows[0].Height = width * 0.13f;

        //    grid.Rows[1].Height = width * 0.13f;

        //    string outputPDFfile = "D:/qrPdf.pdf";


        //    var csvTable = new DataTable();
        //    string rowInfo = "";

        //    using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"D:/200data.csv")), true))
        //    {
        //        csvTable.Load(csvReader);
        //        int total_row = csvTable.Rows.Count;

        //        for (var l = 0; l < csvTable.Rows.Count; l++)
        //        {

        //            if (l < csvTable.Rows.Count - 1)
        //            {
        //                row = grid.Rows.Add();
        //                grid.Rows[l + 1].Height = grid.Rows[l].Height;
        //            }
        //            else grid.Rows[l].Height = grid.Rows[0].Height;
        //        }
        //        for (var i = 0; i < csvTable.Rows.Count; i++)
        //        {
        //            rowInfo = "";

        //            for (var j = 0; j < csvTable.Columns.Count; j++)
        //            {
        //                //ekta row er information->qrCode Conversion->printing on pdf
        //                rowInfo = rowInfo + csvTable.Rows[i][j].ToString() + ",--";
        //                QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //                QRCodeData qrCodeData = qrGenerator.CreateQrCode(rowInfo, QRCodeGenerator.ECCLevel.Q);
        //                QRCode qrCode = new QRCode(qrCodeData);
        //                Bitmap qrCodeImage = qrCode.GetGraphic(1);
        //                var codeBitmap = new Bitmap(qrCodeImage);
        //                Image image = (Image)codeBitmap;

        //                PdfGridCellContentList lst = new PdfGridCellContentList();

        //                PdfGridCellContent textAndStyle = new PdfGridCellContent();
        //                textAndStyle.Image = PdfImage.FromImage(image);

        //                textAndStyle.ImageSize = new SizeF(65, 65);

        //                lst.List.Add(textAndStyle);
        //                for (var k = 0; k < 5; k++)
        //                {
        //                    grid.Rows[i].Cells[k].Value = lst;

        //                }
        //                PdfLayoutResult result = grid.Draw(page, new PointF(10, 30));

        //            }

        //        }


        //    }
        //    doc.SaveToFile(outputPDFfile, FileFormat.PDF);

        //    System.Diagnostics.Process.Start(outputPDFfile);
        //    return View();
        //}

      
    }
}