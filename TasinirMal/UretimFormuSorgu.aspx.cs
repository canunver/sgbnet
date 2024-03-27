using System;
using OrtakClass;
using TNS.KYM;
using System.Collections.Generic;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    public partial class UretimFormuSorgu : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENAMBAR"));
            }
        }

        private UretimFormu Filtre()
        {
            UretimFormu filtre = new UretimFormu();

            filtre.sorguYil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            filtre.girenMuhasebe.kod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            filtre.girenHarcamaBirimi.kod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            filtre.girenAmbar.kod = pgFiltre.Source["prpAmbar"].Value.Trim();

            filtre.sorguBelgeNoBasla = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeNo1"].Value, 0);
            filtre.sorguBelgeNoBitis = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpBelgeNo2"].Value, 0);
            filtre.sorguBelgeTarihBasla = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            filtre.sorguBelgeTarihBitis = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());

            filtre.anaUrun.hesapKod = pgFiltre.Source["prpHesapKod"].Value;
            filtre.islemYapan = pgFiltre.Source["prpSonislemyapan"].Value;

            return filtre;
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            UretimFormu filtre = new UretimFormu();
            filtre = Filtre();

            ObjectArray bilgi = servisTMM.UretimFormuListele(kullanan, filtre);

            List<object> liste = new List<object>();

            foreach (UretimFormu item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    GIRENMUHASEBEKOD = item.girenMuhasebe.kod,
                    GIRENMUHASEBEADI = item.girenMuhasebe.ad,
                    GIRENHARCAMABIRIMKOD = item.girenHarcamaBirimi.kod,
                    GIRENHARCAMABIRIMADI = item.girenHarcamaBirimi.ad,
                    GIRENAMBARKOD = item.girenAmbar.kod,
                    GIRENAMBARADI = item.girenAmbar.ad,
                    CIKANMUHASEBEKOD = item.cikanMuhasebe.kod,
                    CIKANMUHASEBEADI = item.cikanMuhasebe.ad,
                    CIKANHARCAMABIRIMKOD = item.cikanHarcamaBirimi.kod,
                    CIKANHARCAMABIRIMADI = item.cikanHarcamaBirimi.ad,
                    CIKANAMBARKOD = item.cikanAmbar.kod,
                    CIKANAMBARADI = item.cikanAmbar.ad,
                    FISNO = item.fisNo,
                    FISTARIHI = item.fisTarihi.Oku(),
                    DURUM = item.durum,
                    ANAURUNHESAPKOD = item.anaUrun.hesapKod,
                    ANAURUNHESAPAD = item.anaUrun.aciklama,
                    ISLEMYAPAN = item.islemYapan,
                    GIRENTIFFISNO = item.girenFisNo,
                    CIKANTIFFISNO = item.cikanFisNo

                }); ;
            }
            strListe.DataSource = liste;
            strListe.DataBind();

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
                return;
            }
        }

        protected void btnSorguTemizle_Click(object sender, EventArgs e)
        {
            pgFiltre.UpdateProperty("prpBelgeNo1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo2", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
            pgFiltre.UpdateProperty("prpHesapKod", string.Empty);
            pgFiltre.UpdateProperty("prpSonislemyapan", string.Empty);
        }

        protected void btnListeYazdir_Click(object sender, EventArgs e)
        {
            UretimFormu filtre = Filtre();

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;

            string sablonAd = "UretimFormuListesi.xlt";
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("kaynakSatir", ref kaynakSatir, ref sutun);

            ObjectArray bilgiler = servisTMM.UretimFormuListele(kullanan, filtre);

            int satir = 0;
            XLS.HucreAdAdresCoz("baslamaSatir", ref satir, ref sutun);
            foreach (UretimFormu item in bilgiler.objeler)
            {
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                XLS.HucreDegerYaz(satir, 0, item.fisNo);
                XLS.HucreDegerYaz(satir, 1, item.fisTarihi.ToString());
                XLS.HucreDegerYaz(satir, 2, item.anaUrun.hesapKod + "-" + item.anaUrun.aciklama);
                XLS.HucreDegerYaz(satir, 3, item.miktar);
                XLS.HucreDegerYaz(satir, 4, item.girenMuhasebe.kod + "-" + item.girenMuhasebe.ad);
                XLS.HucreDegerYaz(satir, 5, item.girenHarcamaBirimi.kod + "-" + item.girenHarcamaBirimi.ad);
                XLS.HucreDegerYaz(satir, 6, item.girenAmbar.kod + "-" + item.girenAmbar.ad);
                XLS.HucreDegerYaz(satir, 7, item.cikanMuhasebe.kod + "-" + item.cikanMuhasebe.ad);
                XLS.HucreDegerYaz(satir, 8, item.cikanHarcamaBirimi.kod + "-" + item.cikanHarcamaBirimi.ad);
                XLS.HucreDegerYaz(satir, 9, item.cikanAmbar.kod + "-" + item.cikanAmbar.ad);

                XLS.HucreDegerYaz(satir, 10, item.islemYapan);
                XLS.HucreDegerYaz(satir, 11, item.girenFisNo);
                XLS.HucreDegerYaz(satir, 12, item.cikanFisNo);

                satir++;
            }

            XLS.DosyaSaklaTamYol();

            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "UretimFormuListesi", true, "XLSX");
        }

    }
}