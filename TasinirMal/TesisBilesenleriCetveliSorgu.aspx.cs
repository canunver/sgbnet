﻿using System;
using OrtakClass;
using TNS.KYM;
using System.Collections.Generic;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    public partial class TesisBilesenleriCetveliSorgu : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                DurumDoldur();

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpDurum", "0");
            }
        }

        private TNS.TMM.TesisBilesenleri Filtre()
        {
            TNS.TMM.TesisBilesenleri filtre = new TNS.TMM.TesisBilesenleri();

            filtre.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value, 0);
            filtre.muhasebe = new TNS.TMM.Muhasebe();
            if (!string.IsNullOrEmpty(OrtakFonksiyonlar.ConvertToStr(pgFiltre.Source["prpMuhasebe"].Value)))
                filtre.muhasebe.kod = OrtakFonksiyonlar.ConvertToStr(pgFiltre.Source["prpMuhasebe"].Value) + "";

            filtre.harcamaBirimi = new HarcamaBirimi();
            if (!string.IsNullOrEmpty(OrtakFonksiyonlar.ConvertToStr(pgFiltre.Source["prpMuhasebe"].Value)))
                filtre.harcamaBirimi.kod = OrtakFonksiyonlar.ConvertToStr(pgFiltre.Source["prpHarcamaBirimi"].Value) + "";
            else
                filtre.harcamaBirimi.kod = "";

            return filtre;
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            TNS.TMM.TesisBilesenleri filtre = new TNS.TMM.TesisBilesenleri();
            filtre = Filtre();

            TasinirGenel.DegiskenleriKaydet(kullanan, filtre.muhasebe.kod, filtre.harcamaBirimi.kod, "");

            ObjectArray bilgi = servisTMM.TesisBilesenleriListele(kullanan, filtre);

            List<object> liste = new List<object>();

            foreach (TNS.TMM.TesisBilesenleri item in bilgi.objeler)
            {
                int belgeNo = OrtakFonksiyonlar.ConvertToInt(item.belgeNo, 0);
                liste.Add(new
                {
                    REFID = item.refId,
                    MUHASEBEKOD = item.muhasebe.kod,
                    MUHASEBEADI = item.muhasebe.ad,
                    HARCAMABIRIMKOD = item.harcamaBirimi.kod,
                    HARCAMABIRIMADI = item.harcamaBirimi.ad,
                    AMBARKOD = item.ambar.kod,
                    AMBARADI = item.ambar.ad,
                    BELGENO = belgeNo.ToString("d6"),
                    BELGETARIHI = item.belgeTarihi.Oku(),
                    SICILNO = item.sicilNo,
                    DURUM = TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_" + item.durum.ToString("d3")),
                });
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
            pgFiltre.UpdateProperty("muhasebe", string.Empty);
            pgFiltre.UpdateProperty("harcamaBirimi", string.Empty);
        }

        protected void btnListeYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.TesisBilesenleri filtre = Filtre();

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;

            string sablonAd = "TesisBilesenleriListesi.xlt";
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("kaynakSatir", ref kaynakSatir, ref sutun);

            ObjectArray bilgiler = servisTMM.TesisBilesenleriListele(kullanan, filtre);

            int satir = 0;
            XLS.HucreAdAdresCoz("baslamaSatir", ref satir, ref sutun);
            foreach (TNS.TMM.TesisBilesenleri item in bilgiler.objeler)
            {
                XLS.SatirAc(satir, 1);

                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);

                XLS.HucreDegerYaz(satir, 0, item.belgeNo.ToString("d6"));
                XLS.HucreDegerYaz(satir, 1, item.belgeTarihi.ToString());
                XLS.HucreDegerYaz(satir, 2, item.muhasebe.kod + "-" + item.muhasebe.ad);
                XLS.HucreDegerYaz(satir, 3, item.harcamaBirimi.kod + "-" + item.harcamaBirimi.ad);
                XLS.HucreDegerYaz(satir, 4, item.ambar.kod + "-" + item.ambar.ad);
                XLS.HucreDegerYaz(satir, 5, item.sicilNo);
                XLS.HucreDegerYaz(satir, 6, TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_" + item.durum.ToString("d3")));

                satir++;
            }

            XLS.DosyaSaklaTamYol();

            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "TesisBilesenleriListe", true, "XLSX");
        }

        private void DurumDoldur()
        {
            List<object> liste = new List<object>();
            liste.Add(new { KOD = 0, ADI = TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_000") });
            liste.Add(new { KOD = (int)ENUMTesisBilesenDurum.KAYITEDILDI, ADI = TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_001") });
            liste.Add(new { KOD = (int)ENUMTesisBilesenDurum.IPTAL, ADI = TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_009") });

            strDurum.DataSource = liste;
            strDurum.DataBind();
        }
    }
}