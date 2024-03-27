using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Ge�ici al�nd� fi�i bilgilerinin sorgulama ve yazd�rma i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemFormOnaySorgu : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "B/A Onay� Sorgulama";

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                pgFiltre.UpdateProperty("prpBelgeTarihi1", DateTime.Now.AddDays(-30).ToShortDateString());
                pgFiltre.UpdateProperty("prpBelgeTarihi2", DateTime.Now.ToShortDateString());
            }
        }

        /// <summary>
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplan�r ve sunucuya g�nderilir
        /// ve ge�ici al�nd� fi�i bilgileri sunucudan al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r,
        /// yoksa gelen ge�ici al�nd� fi�i bilgileri gvBelgeler GridView kontrol�ne doldurulur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            BAOnay kriter = new BAOnay();

            kriter.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kriter.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            kriter.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kriter.bOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.aOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            kriter.aOnayTarih.AddDays(1);
            kriter.aOnaylayan = pgFiltre.Source["prpKisi"].Value;

            TasinirGenel.DegiskenleriKaydet(kullanan, kriter.muhasebeKod.Trim(), kriter.harcamaKod.Trim(), kriter.ambarKod.Trim());

            ObjectArray bilgi = servisTMM.BAOnayiSorgulama(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (BAOnay onay in bilgi.objeler)
            {
                string tur = "Fi�";
                if (onay.fisNo.StartsWith("A")) tur = "Amortisman";
                else if (onay.fisNo.StartsWith("B")) tur = "De�er D�zeltme";

                liste.Add(new
                {
                    YIL = onay.yil,
                    TUR = tur,
                    MUHASEBEKOD = onay.muhasebeKod,
                    HARCAMABIRIMIKOD = onay.harcamaKod,
                    AMBARKOD = onay.harcamaAd,
                    MUHASEBEAD = "",
                    HARCAMABIRIMIAD = "",
                    AMBARAD = "",
                    BELGENO = onay.fisNo,
                    ONAYAGONDEREN = onay.gonderenKisi,
                    ONAYAGONDERIMTARIHI = onay.gonderimTarih.Oku(),
                    BONAYIVEREN = onay.bOnaylayan,
                    BONAYTARIHI = onay.bOnayTarih.Oku(),
                    AONAYIVEREN = onay.aOnaylayan,
                    AONAYTARIHI = onay.aOnayTarih.Oku(),
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Liste Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplan�r, sunucuya g�nderilir
        /// ve ge�ici al�nd� fi�i bilgileri sunucudan al�n�r. Hata varsa ekrana hata bilgisi
        /// yaz�l�r, yoksa gelen ge�ici al�nd� fi�i bilgilerini i�eren excel raporu �retilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            BAOnay kriter = new BAOnay();

            kriter.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kriter.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            kriter.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kriter.bOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.aOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            kriter.aOnayTarih.AddDays(1);
            kriter.aOnaylayan = pgFiltre.Source["prpKisi"].Value;

            ObjectArray bilgi = servisTMM.BAOnayiSorgulama(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int yazSatir = 0;

            string sablonAd = "BAOnayListesi.xlt";
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref yazSatir, ref sutun);

            foreach (BAOnay onay in bilgi.objeler)
            {
                string tur = "Fi�";
                if (onay.fisNo.StartsWith("A")) tur = "Amortisman";
                else if (onay.fisNo.StartsWith("B")) tur = "De�er Art���";

                XLS.SatirAc(yazSatir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, yazSatir, sutun);

                XLS.HucreDegerYaz(yazSatir, sutun, onay.fisNo);
                XLS.HucreDegerYaz(yazSatir, sutun + 1, tur);
                XLS.HucreDegerYaz(yazSatir, sutun + 2, onay.gonderenKisi);
                XLS.HucreDegerYaz(yazSatir, sutun + 3, onay.gonderimTarih.ToString());
                XLS.HucreDegerYaz(yazSatir, sutun + 4, onay.bOnaylayan);
                XLS.HucreDegerYaz(yazSatir, sutun + 5, onay.bOnayTarih.ToString());
                XLS.HucreDegerYaz(yazSatir, sutun + 6, onay.aOnaylayan);
                XLS.HucreDegerYaz(yazSatir, sutun + 7, onay.aOnayTarih.ToString());

                yazSatir++;
            }

            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
            pgFiltre.UpdateProperty("prpKisi", string.Empty);
            //pgFiltre.UpdateProperty("prpOnayTur", string.Empty);
        }

    }
}