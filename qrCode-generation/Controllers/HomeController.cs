
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
using qrCode_generation.ViewModel;

namespace qrCode_generation.Controllers
{
    public class HomeController : Controller
    {
        List<QRViewModel> qrvm = new List<QRViewModel>();


        public ActionResult Index()
        {
            return View();


        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View(qrvm);


        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
           
            try
            {
                
                if (file != null && file.ContentLength > 0)
                {

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
                      
                        int z, g;
                        for (z = 0, g = 1; g < csvData.Count; z++,g++)
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(csvData[g], QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            Bitmap qrCodeImage = qrCode.GetGraphic(25);
                            var codeBitmap = new Bitmap(qrCodeImage);
                            var bitmaptobyte = BitmapToBytes(codeBitmap);
                            Image image = (Image)codeBitmap;
                            var codeNo = csvData[g].Split(new string[] { "," }, StringSplitOptions.None);
                            QRViewModel qr = new QRViewModel();



                            qr.Image = bitmaptobyte;
                            qr.name = codeNo[0];
                            qrvm.Add(qr);

                        }
                       

                    }

                }
  
                ViewBag.Message = "File Uploaded Successfully!!";
                return View(qrvm);


            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();

            }
        }



        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }






    }
}