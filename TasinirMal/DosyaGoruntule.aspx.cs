using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;
using System.IO;

namespace TasinirMal
{
    public partial class DosyaGoruntule : TMMSayfaV2
    {

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                OnIzleme();
            }
        }

        private void OnIzleme()
        {
            string dosyaYolu = OrtakFonksiyonlar.ConvertToStr(Session["DosyaGoruntuleDosyaAdi"]);
            Session["DosyaGoruntuleDosyaAdi"] = "";

            if (string.IsNullOrEmpty(dosyaYolu) && !File.Exists(dosyaYolu))
                return;

            FileInfo dosya = new FileInfo(dosyaYolu);
            if (dosya.Exists)
            {
                if (dosya.Extension.ToLower() == ".docx" || dosya.Extension.ToLower() == ".doc")
                {
                    string yeniDosya = dosya.FullName.Replace(dosya.Extension, ".pdf");

                    Aspose.Words.Document doc = new Aspose.Words.Document(dosya.FullName);
                    doc.Save(yeniDosya, Aspose.Words.SaveFormat.Pdf);

                    dosya.Delete();
                    dosya = new FileInfo(yeniDosya);
                }
                else if (dosya.Extension.ToLower() == ".xlsx" || dosya.Extension.ToLower() == ".xls")
                {
                    string yeniDosya = dosya.FullName.Replace(dosya.Extension, ".pdf");

                    Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(dosya.FullName);
                    wb.Save(yeniDosya, Aspose.Cells.SaveFormat.Pdf);

                    dosya.Delete();
                    dosya = new FileInfo(yeniDosya);
                }

                if (dosya.Extension.ToLower() == ".pdf")
                {
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.Expires = 0;
                    Response.Buffer = false;
                    Response.WriteFile(dosya.FullName);
                    //Response.Flush();
                    dosya.Delete();
                    Response.End();
                }
                else if (dosya.Extension.ToLower() == ".jpg" || dosya.Extension.ToLower() == ".tif")
                {
                    System.IO.MemoryStream msResim = new System.IO.MemoryStream();
                    System.Drawing.Image img = System.Drawing.Image.FromFile(dosya.FullName);
                    img.Save(msResim, System.Drawing.Imaging.ImageFormat.Jpeg);
                    Response.Clear();
                    Response.ContentType = "image/jpeg";
                    Response.Expires = 0;
                    Response.Buffer = false;
                    Response.BinaryWrite(msResim.ToArray());
                    //Response.Flush();
                    img.Dispose();
                    msResim.Close();
                    dosya.Delete();
                    Response.End();
                }
                else if (dosya.Extension.ToLower() == ".txt")
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Type", "text/plain");
                    Response.HeaderEncoding = System.Text.Encoding.GetEncoding(1254);
                    Response.Write(File.ReadAllText(dosya.FullName, System.Text.Encoding.GetEncoding(1254)));
                    //Response.Flush();
                    dosya.Delete();
                    Response.End();
                }

            }
        }

    }
}