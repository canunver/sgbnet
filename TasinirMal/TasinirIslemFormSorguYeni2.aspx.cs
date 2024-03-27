using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ext1.Net;
using OrtakClass;
using TNS;
using TNS.MUH;
using TasinirMal;

namespace ButceMuhasebe
{
    /// <summary>
    /// Ödeme emri/MÝF belge sorgu iþlemlerini düzenleyen sýnýf
    /// </summary>
    public partial class TasinirIslemFormSorguYeni2 : TMMSayfa
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

//        /// <summary>
//        /// Sayfanýn yüklenmesi sýrasýnda çaðrýlan fonksiyon. Eðer sayfa PostBack edilmemiþse, Para Birimi Doldur ve Listele rutinlerini çaðýrýr.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

//            if (!IsPostBack && ddlDurum.Items.Count <= 0)
//            {
//                //Sayfaya giriþ izni varmý?
//                if (!TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI))
//                    GenelIslemler.SayfayaGirmesin(false);

//                //Dropdown kontrollerine deðerleri yerleþtir
//                for (int i = 2006; i <= DateTime.Now.Year; i++)
//                    ddlYil.Items.Add(new Ext1.Net.ListItem(i.ToString(), i.ToString()));

//                durumStore.DataSource = new object[]
//                    {
//                        new object[] { 1,Resources.ButceMuhasebe.FRMOES002},
//                        new object[] { 5,Resources.ButceMuhasebe.FRMOES003},
//                        new object[] { 23,Resources.ButceMuhasebe.FRMOES004},
//                        new object[] { 6,Resources.ButceMuhasebe.FRMOES005},
//                        new object[] { 9,Resources.ButceMuhasebe.FRMOES006},
//                        new object[] { 22,Resources.ButceMuhasebe.FRMOES007}
//                    };
//                durumStore.DataBind();
//                belgeTurStore.DataSource = new object[]
//                    {
//                        new object[] { 0,Resources.ButceMuhasebe.FRMOES013},
//                        new object[] { 1,Resources.ButceMuhasebe.MUHGNL025},
//                        new object[] { 2,Resources.ButceMuhasebe.FRMOES014}
//                    };
//                belgeTurStore.DataBind();

//                pgFiltre.Source["yevmiyeNo1"].DisplayName += " >=";
//                pgFiltre.Source["yevmiyeNo2"].DisplayName += " <=";
//                pgFiltre.Source["belgeNo1"].DisplayName += " >=";
//                pgFiltre.Source["belgeNo2"].DisplayName += " <=";
//                pgFiltre.Source["belgeTarihi1"].DisplayName += " >=";
//                pgFiltre.Source["belgeTarihi2"].DisplayName += " <=";

//                pgFiltre.UpdateProperty("yil", DateTime.Now.Year.ToString());
//                pgFiltre.UpdateProperty("durum", "1");
//                pgFiltre.UpdateProperty("belgeTur", "0");
//            }
//        }

//        OdemeEmriMIFFormlarVAN Sorgula(string hesapKod)
//        {
//            string hataStr = "";
//            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
//            f.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["yil"].Value, 0);
//            f.formDurum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["durum"].Value, 0);

//            if (f.yil == 0 || f.formDurum == 0)
//            {
//                hataStr = Resources.ButceMuhasebe.MUHGNL184;
//                GenelIslemler.MesajKutusu("Hata", hataStr);

//                return null;
//            }

//            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
//            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["muhasebe"].Value, 0);
//            f.birim = pgFiltre.Source["birim"].Value;

//            if (pgFiltre.Source["belgeTur"].Value != "")
//                f.sorguBelgeTur = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["belgeTur"].Value, -1);

//            string b1 = pgFiltre.Source["belgeNo1"].Value;
//            string b2 = pgFiltre.Source["belgeNo2"].Value;
//            if (b1 != "" || b2 != "")
//            {
//                if (b1 != "" && b1.Length < 9)
//                    b1 = f.kurum.Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
//                if (b2 != "" && b2.Length < 9)
//                    b2 = f.kurum.Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');

//                f.belgeNo = b1 + ";" + b2;
//            }

//            if (pgFiltre.Source["yevmiyeNo1"].Value != "" || pgFiltre.Source["yevmiyeNo2"].Value != "")
//                f.sorguYEVYevNo = pgFiltre.Source["yevmiyeNo1"].Value + ";" + pgFiltre.Source["yevmiyeNo2"].Value;

//            if (pgFiltre.Source["yevmiyeTarihi1"].Value != "")
//                f.sorguYEVYevTarih = new TNSDateTime(pgFiltre.Source["yevmiyeTarihi1"].Value).ToString() + ";";
//            if (pgFiltre.Source["yevmiyeTarihi2"].Value != "")
//                f.sorguYEVYevTarih += new TNSDateTime(pgFiltre.Source["yevmiyeTarihi2"].Value).ToString();

//            if (pgFiltre.Source["belgeTarihi1"].Value != "")
//                f.sorguIslemTarih = new TNSDateTime(pgFiltre.Source["belgeTarihi1"].Value).ToString() + ";";
//            if (pgFiltre.Source["belgeTarihi2"].Value != "")
//                f.sorguIslemTarih += new TNSDateTime(pgFiltre.Source["belgeTarihi2"].Value).ToString();

//            if (pgFiltre.Source["durumTarihi1"].Value != "")
//                f.sorguFormDurumTarih = new TNSDateTime(pgFiltre.Source["durumTarihi1"].Value).ToString() + ";";
//            if (pgFiltre.Source["durumTarihi2"].Value != "")
//                f.sorguFormDurumTarih += new TNSDateTime(pgFiltre.Source["durumTarihi2"].Value).ToString();

//            f.ilgiliAd = pgFiltre.Source["ilgili"].Value;
//            f.ilgiliNo = pgFiltre.Source["ilgiliNo"].Value;
//            f.islemYapan = pgFiltre.Source["islemYapan"].Value;
//            f.projeNo = pgFiltre.Source["yatirimProjeNo"].Value;
//            f.aciklama = pgFiltre.Source["aciklama"].Value;

//            f.OEBTur = pgFiltre.Source["OdEsTur"].Value;
//            f.OEBNo = pgFiltre.Source["OdEsNo"].Value;
//            if (pgFiltre.Source["OdEsTarihi"].Value.Trim() != "")
//                f.OEBTarih = new TNSDateTime(pgFiltre.Source["OdEsTarihi"].Value.Trim());

//            f.YEVCekNo = pgFiltre.Source["cekNoSorgu"].Value;
//            f.fisTur = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["fisTur"].Value, 0);

//            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
//            detay.hesapKod = pgFiltre.Source["hesapNo"].Value;
//            detay.hesapKod = detay.hesapKod.Split('-')[0];

//            detay.altHesapKod = pgFiltre.Source["althesapNo"].Value;
//            detay.tertip.kodlar[0] = pgFiltre.Source["kur"].Value;
//            detay.tertip.kodlar[1] = pgFiltre.Source["fon"].Value;
//            detay.tertip.kodlar[2] = pgFiltre.Source["fin"].Value;
//            detay.tertip.kodlar[3] = pgFiltre.Source["eko"].Value;

//            if (GenelIslemlerIstemci.MasrafPlaniMi())
//            {
//                if (detay.tertip.kodlar.Length > 4)
//                    detay.tertip.kodlar[4] = pgFiltre.Source["peb"].Value;
//            }

//            detay.borc = OrtakFonksiyonlar.ConvertToDbl(pgFiltre.Source["borc"].Value);
//            detay.alacak = OrtakFonksiyonlar.ConvertToDbl(pgFiltre.Source["alacak"].Value);


//            if (hesapKod == "103")
//            {
//                if (txtIslemCekNo.Text != "")
//                    f.YEVCekNo = txtIslemCekNo.Text;
//                else
//                {
//                    detay.hesapKod = "103";
//                    f.belgeTur = ENUMBelgeTuru.ODEMEEMRI;
//                    f.formDurum = (int)ENUMBelgeDurumu.YEVMIYE;
//                    detay.alacak = 0.00001;
//                }
//            }

//            f.detay.Add(detay);

//            return servisMUH.OdemeEmriMIFListele(kullanan, f, false);
//        }

//        /// <summary>
//        /// Formdaki kriterlere baðlý olarak ödeme emri/MIF listeleyen ve gridde görüntüleyen fonksiyon.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnListe_Click(object sender, DirectEventArgs e)
//        {
//            RowSelectionModel sm = this.grdListe.SelectionModel.Primary as RowSelectionModel;
//            sm.SelectedRows.Clear();
//            sm.UpdateSelection();

//            OdemeEmriMIFFormlarVAN fler = Sorgula("");
//            if (fler == null) return;

//            string durum = "";
//            string belgeTuru = "";
//            DataTable dt = new DataTable();
//            dt.Columns.Add("belgeNo", Type.GetType("System.String"));
//            dt.Columns.Add("yevmiyeNo", Type.GetType("System.String"));
//            dt.Columns.Add("belgeTur", Type.GetType("System.String"));
//            dt.Columns.Add("belgeTarih", Type.GetType("System.DateTime"));
//            dt.Columns.Add("muhasebe", Type.GetType("System.String"));
//            dt.Columns.Add("birim", Type.GetType("System.String"));
//            dt.Columns.Add("borc", Type.GetType("System.Decimal"));
//            dt.Columns.Add("alacak", Type.GetType("System.Decimal"));
//            dt.Columns.Add("ilgili", Type.GetType("System.String"));
//            dt.Columns.Add("yevmiyeTarih", Type.GetType("System.DateTime"));
//            dt.Columns.Add("durum", Type.GetType("System.String"));
//            dt.Columns.Add("durumTarih", Type.GetType("System.DateTime"));
//            dt.Columns.Add("islemYapan", Type.GetType("System.String"));
//            dt.Columns.Add("OEBNo", Type.GetType("System.String"));
//            dt.Columns.Add("OEBTarih", Type.GetType("System.DateTime"));
//            dt.Columns.Add("cekNo", Type.GetType("System.String"));
//            dt.Columns.Add("cekTarih", Type.GetType("System.DateTime"));
//            dt.Columns.Add("surecNo", Type.GetType("System.String"));
//            Session["OdemeEmriListe"] = dt;//bos datatable sessiona yaz.
//            Session["OdemeEmriSort"] = "";

//            if (fler.sonuc.islemSonuc)
//            {
//                foreach (OdemeEmriMIFForm ff in fler.odemeEmriMIFFormlar)
//                {
//                    durum = DurumDonustur(ff.formDurum);
//                    if (ff.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
//                        belgeTuru = "ÖDE";
//                    else
//                        belgeTuru = "MÝF";
//                    System.Data.DataRow row = dt.NewRow();
//                    row["belgeNo"] = ff.belgeNo;

//                    if (ff.YEVYevNo > 0)
//                        row["yevmiyeNo"] = ff.YEVYevNo;

//                    row["belgeTarih"] = ff.islemTarih.ToString();
//                    row["belgeTur"] = belgeTuru;
//                    row["muhasebe"] = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd;
//                    row["birim"] = ff.birim + " - " + ff.birimAd;
//                    row["ilgili"] = ff.ilgiliAd;

//                    if (!ff.YEVYevTarih.isNull)
//                        row["yevmiyeTarih"] = ff.YEVYevTarih.ToString();
//                    else
//                        row["yevmiyeTarih"] = new TNSDateTime().Oku();
//                    if (!ff.OEBTarih.isNull)
//                        row["OEBTarih"] = ff.OEBTarih.ToString();
//                    else
//                        row["OEBTarih"] = new TNSDateTime().Oku();

//                    row["borc"] = ff.borcToplam;
//                    row["alacak"] = ff.alacakToplam;
//                    row["durum"] = durum;
//                    row["durumTarih"] = ff.formDurumTarih.ToString();
//                    row["islemYapan"] = ff.islemYapan;
//                    row["OEBNo"] = ff.OEBNo;
//                    row["surecNo"] = ff.surecYil + "/" + ff.surecNo;

//                    row["cekNo"] = ff.YEVCekNo;
//                    if (!ff.YEVCekTarih.isNull)
//                        row["cekTarih"] = ff.YEVCekTarih.ToString();
//                    else
//                        row["cekTarih"] = new TNSDateTime().Oku();

//                    dt.Rows.Add(row);
//                }
//                Session["OdemeEmriListe"] = dt;//datatable a yüklenen listeyi sessiona yaz

//                RowExpander1.CollapseAll();
//                PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);

//                if (dt.Rows.Count == 0)
//                    GenelIslemler.MesajKutusu("Hata", "Verilen kriterlere uygun kayýt bulunamadý.");
//            }
//            else
//            {
//                GenelIslemler.MesajKutusu("Hata", fler.sonuc.hataStr);
//                return;
//            }
//        }
//        /// <summary>
//        /// Formdaki kriterlere baðlý olarak listelenen ödeme emri/MIF listesini yazdýran fonksiyon
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnListeYazdir_Click(object sender, DirectEventArgs e)
//        {
//            OdemeEmriMIFFormlarVAN fler = Sorgula("");

//            Tablo XLS = GenelIslemler.NewTablo();
//            int satir = 0;

//            int satirD = 0;
//            int sutunD = 0;
//            string baslik = "";
//            string sablonAd = "MUH-OdemeEmriMIFSorguListe.xlt";

//            baslik = "ÖDEME EMRÝ/MUHASEBE ÝÞLEM FÝÞÝ BELGELERÝ";

//            string sonucDosyaAd = System.IO.Path.GetTempFileName();

//            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/MUH/" + sablonAd, sonucDosyaAd);

//            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);

//            satir = satirD + 1;

//            XLS.HucreAdBulYaz("txtBaslik", baslik);
//            string yil = pgFiltre.Source["yil"].Value;
//            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
//            string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

//            XLS.HucreAdBulYaz("txtYil", yil);
//            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

//            int i = 0;
//            foreach (OdemeEmriMIFForm ff in fler.odemeEmriMIFFormlar)
//            {
//                i++;
//                XLS.HucreKopyala(satirD, 0, satirD, 20, satir, 0);

//                XLS.HucreDegerYaz(satir, 1, i.ToString());
//                XLS.HucreDegerYaz(satir, 2, ff.belgeNo);
//                if (ff.YEVYevNo > 0)
//                    XLS.HucreDegerYaz(satir, 3, ff.YEVYevNo);
//                XLS.HucreDegerYaz(satir, 4, ff.belgeTur.ToString());
//                XLS.HucreDegerYaz(satir, 5, ff.islemTarih.ToString());
//                XLS.HucreDegerYaz(satir, 6, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
//                XLS.HucreDegerYaz(satir, 7, ff.birim + " - " + ff.birimAd);
//                XLS.HucreDegerYaz(satir, 8, ff.borcToplam);
//                XLS.HucreDegerYaz(satir, 9, ff.alacakToplam);
//                XLS.HucreDegerYaz(satir, 10, ff.ilgiliAd);
//                XLS.HucreDegerYaz(satir, 11, ff.YEVYevTarih.ToString());
//                XLS.HucreDegerYaz(satir, 12, DurumDonustur(ff.formDurum));
//                XLS.HucreDegerYaz(satir, 13, ff.formDurumTarih.ToString());
//                XLS.HucreDegerYaz(satir, 14, ff.islemYapan);

//                satir++;
//            }

//            XLS.DosyaSaklaTamYol();
        //            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste", true, GenelIslemler.ExcelTur());
//        }
//        /// <summary>
//        /// Formdaki kriterlere baðlý olarak listelenen ödeme emri/MIF listesini yazdýran fonksiyon
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnCekNoYazdir_Click(object sender, DirectEventArgs e)
//        {
//            txtIslemCekNo.Text = e.ExtraParams["cekNo"] + "";

//            OdemeEmriMIFFormlarVAN fler = Sorgula("103");

//            Tablo XLS = GenelIslemler.NewTablo();
//            int satir = 0;
//            int satirD = 0;
//            int sutunD = 0;
//            string sablonAd = "MUH-OdemeEmriMIFCekListe.XLT";

//            string sonucDosyaAd = System.IO.Path.GetTempFileName();

//            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/MUH/" + sablonAd, sonucDosyaAd);

//            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);

//            satir = satirD + 1;

//            string yil = pgFiltre.Source["yil"].Value;
//            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
//            string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

//            XLS.HucreAdBulYaz("txtYil", yil);
//            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

//            int i = 0;
//            foreach (OdemeEmriMIFForm ff in fler.odemeEmriMIFFormlar)
//            {
//                i++;
//                XLS.HucreKopyala(satirD, 0, satirD, 20, satir, 0);

//                XLS.HucreDegerYaz(satir, 1, i.ToString());
//                XLS.HucreDegerYaz(satir, 2, ff.YEVYevNo);
//                XLS.HucreDegerYaz(satir, 3, ff.YEVYevTarih.ToString());
//                XLS.HucreDegerYaz(satir, 4, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
//                XLS.HucreDegerYaz(satir, 5, ff.birim + " - " + ff.birimAd);
//                XLS.HucreDegerYaz(satir, 6, ff.alacakToplam);
//                if (ff.ilgiliNo != "")
//                    ff.ilgiliAd += " (" + ff.ilgiliNo + ")";
//                XLS.HucreDegerYaz(satir, 7, ff.ilgiliAd);
//                XLS.HucreDegerYaz(satir, 8, ff.YEVCekNo);

//                satir++;
//            }

//            XLS.DosyaSaklaTamYol();
        //            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste", true, GenelIslemler.ExcelTur());
//        }

//        /// <summary>
//        /// btnIptal click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnIptal_Click(object sender, DirectEventArgs e)
//        {
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.IPTAL);
//        }
//        /// <summary>
//        /// btnYevmiyeNo click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnYevmiyeVer_Click(object sender, DirectEventArgs e)
//        {
//            txtIslemYevmiyeNo.Text = e.ExtraParams["yevmiyeNo"] + "";
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.ICMALYEVMIYENOVER);
//        }
//        /// <summary>
//        /// btnCekNo click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnCekNoVer_Click(object sender, DirectEventArgs e)
//        {
//            txtIslemCekNo.Text = e.ExtraParams["cekNo"] + "";
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.CEKNOVER);
//        }
//        /// <summary>
//        /// btnCekNoKaldir click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnCekNoKaldir_Click(object sender, DirectEventArgs e)
//        {
//            txtIslemCekNo.Text = "";
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.CEKNOVER);
//        }

//        /// <summary>
//        /// btnYevmiyeNoKaldir click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnYevmiyeNoKaldir_Click(object sender, DirectEventArgs e)
//        {
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.YEVMIYEKALDIR);
//        }
//        /// <summary>
//        /// btnOnayla click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnOnayla_Click(object sender, DirectEventArgs e)
//        {
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.ONAYLI);
//        }
//        /// <summary>
//        /// btnOnayKaldir click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnOnayKaldir_Click(object sender, DirectEventArgs e)
//        {
//            BelgeDurumDegisikligi((int)ENUMBelgeDurumu.ONAYKALDIR);
//        }
//        /// <summary>
//        /// btnBelgeYazdir click olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void btnBelgeYazdir_Click(object sender, DirectEventArgs e)
//        {
//            RowSelectionModel sm = this.grdListe.SelectionModel.Primary as RowSelectionModel;
//            string seciliBelgeler = "";
//            foreach (SelectedRow row in sm.SelectedRows)
//            {
//                if (seciliBelgeler != "") seciliBelgeler += ";";
//                seciliBelgeler += row.RecordID;
//            }

//            if (string.IsNullOrEmpty(seciliBelgeler))
//            {
//                GenelIslemler.MesajKutusu("Bilgi", "Listeden herhangi bir belge seçilmediði için iþlem yapýlamadý.");
//                return;
//            }

//            ////Birden fazla rapor dolayýsýyle sýkýþtýrýp göndereceðizdir.
//            string tempFileName = System.IO.Path.GetTempFileName();
//            string klasor = tempFileName + ".dir";
//            System.IO.DirectoryInfo dri = System.IO.Directory.CreateDirectory(klasor);
//            klasor += "\\";

//            string[] belgeler = seciliBelgeler.Split(';');

//            for (int i = 0; i < belgeler.Length; i++)
//            {
//                int yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["yil"].Value, 0);
//                string belgeNo = belgeler[i];

//                string tmpFile = System.IO.Path.GetTempFileName();
        //                string excelYazYer = klasor + belgeNo + "." + GenelIslemler.ExcelTur();

//                System.IO.File.Move(tmpFile, excelYazYer);
//                System.IO.File.Delete(tmpFile);

//                string donenBilgi = OdemeEmriMIFYazdirma.Yazdir(yil, belgeNo, excelYazYer, ENUMBelgeTuru.TANIMSIZ);
//            }

//            string[] dosyalar = { "" };
//            string sonucDosyaAd = System.IO.Path.GetTempFileName();

//            OrtakClass.Zip.Ziple(dri.FullName, sonucDosyaAd, dosyalar);
//            dri.Delete(true);
//            System.IO.File.Delete(tempFileName);
//            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Belgeler.zip", true, "zip");
//        }
//        /// <summary>
//        /// Listede detay kayýtlarýn gösterimini saðlayan yordam
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void BeforeExpand(object sender, DirectEventArgs e)
//        {
//            string id = e.ExtraParams["id"];

//            Store storeTertip = new Store { ID = "StoreListe_" + id };

//            JsonReader reader = new JsonReader();
//            reader.IDProperty = "HesapKod";
//            reader.Fields.Add("HesapKod", RecordFieldType.String);
//            reader.Fields.Add("Tertip", RecordFieldType.String);
//            reader.Fields.Add("Borc", RecordFieldType.Float);
//            reader.Fields.Add("Alacak", RecordFieldType.Float);
//            storeTertip.Reader.Add(reader);

//            List<object> data = new List<object>();

//            //Formu listele
//            int yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["yil"].Value, 0);
//            ENUMBelgeTuru belgeTur = (ENUMBelgeTuru)OrtakFonksiyonlar.ConvertToInt(0, 0);
//            OdemeEmriMIFFormlarVAN fler = OdemeEmriMIFYazdirma.Listele(kullanan, servisMUH, yil, id);
//            OdemeEmriMIFForm form = (OdemeEmriMIFForm)fler.odemeEmriMIFFormlar[0];
//            foreach (OdemeEmriMIFDetay detay in form.detay)
//                data.Add(new { HesapKod = detay.hesapKod, Tertip = detay.tertip.TertipStrOl(), Borc = detay.borc, Alacak = detay.alacak });

//            storeTertip.DataSource = data;
//            storeTertip.Render();

//            GridPanel grid = new GridPanel
//            {
//                ID = "GridPanelRow_" + id,
//                StoreID = "{raw}StoreListe_" + id,
//                Height = 150,
//                Width = 520,
//                Layout = "TableLayout"
//            };

//            grid.ColumnModel.Columns.Add(new Column { Header = "Hesap Kodu", DataIndex = "HesapKod", Width = 120 });
//            grid.ColumnModel.Columns.Add(new Column { Header = "Tertip", DataIndex = "Tertip", Width = 220 });
//            grid.ColumnModel.Columns.Add(new Column { Header = "Borç", DataIndex = "Borc", Width = 80, Align = Ext1.Net.Alignment.Right });
//            grid.ColumnModel.Columns.Add(new Column { Header = "Alacak", DataIndex = "Alacak", Width = 80, Align = Ext1.Net.Alignment.Right });
//            grid.ColumnModel.ID = "GridPanelRowCM_" + id;
//            grid.View.Add(new Ext1.Net.GridView { ID = "GridPanelRowView_" + id, ForceFit = true });

//            //important
//            X.Get("row-" + id).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);
//            grid.Render("row-" + id, RenderMode.RenderTo);
//        }
//        /// <summary>
//        /// StoreListe kontrolünün data refresh olayýný iþleyen yordam.
//        /// </summary>
//        /// <param name="sender">Çaðýran, object tipinde sýnýf</param>
//        /// <param name="e">Olay argümaný, Event Args tipinde sýnýf</param>
//        protected void StoreListe_Refresh(object sender, StoreRefreshDataEventArgs e)
//        {
//            string sortCol = e.Sort;
//            string grupCol = OrtakFonksiyonlar.ConvertToStr(e.Parameters["groupBy"]);
//            if (sortCol != "")
//            {
//                if (e.Dir == Ext1.Net.SortDirection.ASC)
//                    sortCol += " ASC";
//                else
//                    sortCol += " DESC";
//            }
//            if (grupCol != "")
//            {
//                grupCol += " ASC";
//                sortCol = grupCol + "," + sortCol;
//            }

//            RowExpander1.CollapseAll();

//            DataTable dt = (DataTable)Session["OdemeEmriListe"];//sessiondan oku
//            string sakliSortCol = OrtakFonksiyonlar.ConvertToStr((string)Session["OdemeEmriSort"]);//önceden iþlenmiþ sort bilgilerini oku

//            if (sakliSortCol != sortCol)//önceden iþlenmiþ sort bilgilerinden farklý mý?
//            {
//                System.Data.DataView dv = new System.Data.DataView(dt, "", sortCol, DataViewRowState.CurrentRows);
//                dt = dv.ToTable();

//                Session["OdemeEmriListe"] = dt;
//                Session["OdemeEmriSort"] = sortCol;
//            }

//            List<DataRow> liste = dt.AsEnumerable().ToList();

//            e.Total = 0;
//            if (liste != null && liste.Count != 0)
//            {
//                var limit = e.Limit;
//                if ((e.Start + e.Limit) > liste.Count)
//                    limit = liste.Count - e.Start;

//                e.Total = liste.Count;
//                List<DataRow> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
//                StoreListe.DataSource = rangeData.CopyToDataTable();
//                StoreListe.DataBind();
//            }
//            else
//            {
//                StoreListe.DataSource = new object[] { };
//                StoreListe.DataBind();
//            }
//        }

//        void BelgeDurumDegisikligi(int durum)
//        {
//            RowSelectionModel sm = this.grdListe.SelectionModel.Primary as RowSelectionModel;
//            string seciliBelgeler = "";
//            foreach (SelectedRow row in sm.SelectedRows)
//            {
//                if (seciliBelgeler != "") seciliBelgeler += ";";
//                seciliBelgeler += row.RecordID;
//            }

//            if (string.IsNullOrEmpty(seciliBelgeler))
//            {
//                GenelIslemler.MesajKutusu("Bilgi", "Listeden herhangi bir belge seçilmediði için iþlem yapýlamadý.");
//                return;
//            }
//            string[] belgeler = seciliBelgeler.Split(';');

//            OdemeEmriMIFFormlar fler = new OdemeEmriMIFFormlar();
//            for (int i = 0; i < belgeler.Length; i++)
//            {
//                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
//                f.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["yil"].Value, 0);
//                f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();

//                f.belgeNo = belgeler[i];
//                f.islemYapan = kullanan.kullaniciKodu;
//                f.formDurum = durum;


//                if (txtIslemYevmiyeNo.Text.Trim() != "" && durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
//                    f.YEVYevNo = OrtakFonksiyonlar.ConvertToInt(txtIslemYevmiyeNo.Text.Trim(), 0);

//                if (durum == (int)ENUMBelgeDurumu.CEKNOVER)
//                    f.YEVCekNo = txtIslemCekNo.Text.Trim();

//                if (OrtakFonksiyonlar.ConvertToDbl(f.belgeNo) > 0)
//                    fler.formDizi.Add(f);
//            }

//            Sonuc sonuc = servisMUH.BelgeDurumDegistir(kullanan, fler);

//            string olumluBilgi = "";
//            string olumsuzBilgi = "";

//            if (durum == (int)ENUMBelgeDurumu.ONAYLI)
//            {
//                olumluBilgi = Resources.ButceMuhasebe.FRMOES034;
//                olumsuzBilgi = Resources.ButceMuhasebe.FRMOES035;
//            }
//            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
//            {
//                olumluBilgi = Resources.ButceMuhasebe.FRMOES036;
//                olumsuzBilgi = Resources.ButceMuhasebe.FRMOES037;
//            }
//            else if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
//            {
//                olumluBilgi = Resources.ButceMuhasebe.FRMOES038;
//                olumsuzBilgi = Resources.ButceMuhasebe.FRMOES039;
//            }
//            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
//            {
//                olumluBilgi = Resources.ButceMuhasebe.FRMOES040;
//                olumsuzBilgi = Resources.ButceMuhasebe.FRMOES041;
//            }

//            if (sonuc.islemSonuc)
//            {
//                btnListe_Click(null, null);
//                GenelIslemler.MesajKutusu("Bilgi", olumluBilgi + sonuc.bilgiStr);
//            }
//            else
//                GenelIslemler.MesajKutusu("Hata", olumsuzBilgi + sonuc.hataStr);
//        }

//        string DurumDonustur(int durum)
//        {
//            if (durum == (int)ENUMBelgeDurumu.YENI)
//                return "Yeni";
//            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
//                return "Deðiþtirildi";
//            else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
//                return "Onaylý";
//            else if (durum == (int)ENUMBelgeDurumu.YEVMIYE)
//                return "Yevmiyeli";
//            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
//                return "Ýptal";
//            else
//                return "Belirsiz - " + durum;
//        }
//        /// <summary>
//        /// Handles the Refresh event of the HesapStore control.
//        /// </summary>
//        /// <param name="sender">The source of the event.</param>
//        /// <param name="e">The <see cref="Ext1.Net.StoreRefreshDataEventArgs"/> instance containing the event data.</param>
//        protected void HesapStore_Refresh(object sender, StoreRefreshDataEventArgs e)
//        {
//            if (string.IsNullOrEmpty(e.Parameters["query"]))
//                return;

//            List<object> liste = HesapListesiDoldur(e.Parameters["query"]);

//            e.Total = 0;
//            if (liste != null && liste.Count != 0)
//            {
//                var limit = e.Limit;
//                if ((e.Start + e.Limit) > liste.Count)
//                    limit = liste.Count - e.Start;

//                e.Total = liste.Count;
//                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
//                StoreHesapPlan.DataSource = (object[])rangeData.ToArray();
//                StoreHesapPlan.DataBind();
//            }
//            else
//            {
//                StoreHesapPlan.DataSource = new object[] { };
//                StoreHesapPlan.DataBind();
//            }
//        }
//        List<object> HesapListesiDoldur(string kriter)
//        {
//            HesapPlaniSatir h = new HesapPlaniSatir();
//            h.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["yil"].Value, 0);
//            if (h.yil == 0) h.yil = DateTime.Now.Year;
//            h.hesapKodHesapAd = kriter;
//            h.detay = false;
//            HesapPlaniVAN hesap = servisMUH.HesapPlaniListele(kullanan, h);

//            List<object> liste = new List<object>();
//            foreach (HesapPlaniSatir detay in hesap.hesapPlani.satir)
//            {
//                liste.Add(new
//                {
//                    KOD = detay.hesapKod + " - " + detay.aciklama
//                });
//            }
//            return liste;
//        }
    }
}
