using Ext1.Net;
using OrtakClass;
using System;
using System.Drawing.Imaging;
using System.IO;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class DosyaOnizleme : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                string resimID = Request.QueryString["resimID"] + "";
                string dosyaID = Request.QueryString["dosyaID"] + "";

                bool sonuc = DosyaGoruntule(this, resimID, dosyaID);

                if (!sonuc)
                {
                    string uyari = Request.QueryString["uyari"] + "";
                    if (uyari == "yok") return;
                    GenelIslemler.MesajKutusu("Bilgi", "Bu dosya tipi için ön izleme yapılamıyor.");
                    X.AddScript("parent.hidePopWin();");
                }
            }
        }

        public bool DosyaGoruntule(istemciUzayi.GenelSayfa sayfa, string resimKod, string dosyaKod)
        {
            if (string.IsNullOrEmpty(resimKod.Trim()) && string.IsNullOrEmpty(dosyaKod.Trim()))
            {
                sayfa.Response.Clear();
                sayfa.Response.End();
                return false;
            }

            ObjectArray liste = new ObjectArray();

            if (!string.IsNullOrEmpty(resimKod.Trim()))
            {
                liste = servisTMM.TasinirResimGetir(0, resimKod);
            }
            else if (!string.IsNullOrEmpty(dosyaKod.Trim()))
            {
                TNS.TMM.IslemDosya bilgi = new TNS.TMM.IslemDosya();
                bilgi.dosyaKod = OrtakFonksiyonlar.ConvertToInt(dosyaKod, 0);
                liste = servisTMM.IslemDosyaGetir(bilgi);
            }

            if (liste.objeler.Count == 0) return false;

            string contentType = "application/pdf";
            bool gonder = true;
            string gonderilecekDosyaTmp = Path.GetTempFileName();
            string dosyaTmp = "";
            string ext = "";

            if (resimKod != "")
            {
                foreach (TasResim item in liste.objeler)
                {
                    string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                    item.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");
                    string dosya = Path.Combine(item.kayitEdilecekDosyaYol, "RESIM");
                    dosya = Path.Combine(dosya, "RESIM" + "_" + item.resimID);
                    if (File.Exists(dosya))
                        item.resim = File.ReadAllBytes(dosya);

                    dosyaTmp = Path.GetTempFileName();
                    File.WriteAllBytes(dosyaTmp, item.resim);
                    ext = Path.GetExtension(item.adi);
                }
            }
            else if (dosyaKod != "")
            {
                foreach (TNS.TMM.IslemDosya item in liste.objeler)
                {
                    string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                    item.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirIslemBelgesi");
                    string dosya = Path.Combine(item.kayitEdilecekDosyaYol, item.yil.ToString());
                    dosya = Path.Combine(dosya, item.yil + "_" + item.dosyaKod);
                    if (File.Exists(dosya))
                        item.resim = File.ReadAllBytes(dosya);

                    dosyaTmp = Path.GetTempFileName();
                    File.WriteAllBytes(dosyaTmp, item.resim);
                    ext = Path.GetExtension(item.adi);
                }
            }

            ext = ext.Replace(".", "");
            if (ext.ToUpper().StartsWith("PDF"))
            {
                gonderilecekDosyaTmp = dosyaTmp;
            }
            else if (ext.ToUpper().StartsWith("XLS"))
            {
                Aspose.Cells.Workbook pdfDocument = new Aspose.Cells.Workbook(dosyaTmp);
                pdfDocument.Save(gonderilecekDosyaTmp, Aspose.Cells.SaveFormat.Pdf);
            }
            else if (ext.ToUpper().StartsWith("DOC") || ext.ToUpper().StartsWith("RTF") || ext.ToUpper().StartsWith("HTM") || ext.ToUpper().StartsWith("TXT") || ext.ToUpper().StartsWith("ODT"))
            {
                Aspose.Words.Document pdfDocument = new Aspose.Words.Document(dosyaTmp);
                pdfDocument.Save(gonderilecekDosyaTmp, Aspose.Words.SaveFormat.Pdf);
            }
            else if (ext.ToUpper().StartsWith("PPT"))
            {
                //Aspose.Slides.Presentation pdfDocument = new Aspose.Slides.Presentation(dosyaTmp);
                //pdfDocument.Save(gonderilecekDosyaTmp, Aspose.Slides.Export.SaveFormat.Pdf);
            }
            else if (ext.ToUpper().StartsWith("TIF"))
            {
                ConvertImageToPdf(dosyaTmp, gonderilecekDosyaTmp);
            }
            else if (ext.ToUpper().StartsWith("JPEG") || ext.ToUpper().StartsWith("JPG") || ext.ToUpper().StartsWith("PNG") || ext.ToUpper().StartsWith("BMP") || ext.ToLower().StartsWith("gif"))
            {
                if (ext.ToLower() == "jpg" || ext.ToLower() == "jpeg")
                    contentType = "image/jpeg";
                else if (ext.ToLower() == "bmp")
                    contentType = "image/bmp";
                else if (ext.ToLower() == "png")
                    contentType = "image/png";
                else if (ext.ToLower() == "gif")
                    contentType = "image/gif";

                gonderilecekDosyaTmp = dosyaTmp;
            }
            else
                gonder = false;

            if (gonder)
            {
                FileInfo file = new FileInfo(gonderilecekDosyaTmp);
                if (file.Exists)
                {
                    sayfa.Response.Clear();
                    sayfa.Response.ContentType = contentType;
                    sayfa.Response.Expires = 0;
                    sayfa.Response.Buffer = false;
                    sayfa.Response.WriteFile(file.FullName);
                    file.Delete();
                    sayfa.Response.End();
                }
            }
            else
            {
                return false;
                //Evrak önizlemede sorun oluşuyor
                //OrtakSiniflar.MesajKutusu("Bilgi", "Bu dosya tipi için ön izleme yapılamıyor.");
                //Ext.Net.X.AddScript("hidePopWin(parentAutoLoadControl.id);");
            }

            return true;
        }

        public void ConvertImageToPdf(string inputFileName, string outputFileName)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document();
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            using (System.Drawing.Image image = System.Drawing.Image.FromFile(inputFileName))
            {
                // Find which dimension the frames in this image represent. For example
                // the frames of a BMP or TIFF are "page dimension" whereas frames of a GIF image are "time dimension".
                FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);

                // Get the number of frames in the image.
                int framesCount = image.GetFrameCount(dimension);

                // Loop through all frames.
                for (int frameIdx = 0; frameIdx < framesCount; frameIdx++)
                {
                    // Insert a section break before each new page, in case of a multi-frame TIFF.
                    if (frameIdx != 0)
                        builder.InsertBreak(Aspose.Words.BreakType.SectionBreakNewPage);

                    // Select active frame.
                    image.SelectActiveFrame(dimension, frameIdx);

                    // We want the size of the page to be the same as the size of the image.
                    // Convert pixels to points to size the page to the actual image size.
                    Aspose.Words.PageSetup ps = builder.PageSetup;
                    ps.PageWidth = Aspose.Words.ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
                    ps.PageHeight = Aspose.Words.ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);

                    // Insert the image into the document and position it at the top left corner of the page.
                    builder.InsertImage(
                        image,
                        Aspose.Words.Drawing.RelativeHorizontalPosition.Page,
                        0,
                        Aspose.Words.Drawing.RelativeVerticalPosition.Page,
                        0,
                        ps.PageWidth,
                        ps.PageHeight,
                        Aspose.Words.Drawing.WrapType.None);
                }
            }

            // Save the document to PDF.
            doc.Save(outputFileName, Aspose.Words.SaveFormat.Pdf);
        }


    }
}