using System;
using OrtakClass;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;
using System.IO;

namespace TasinirMal
{
    /// <summary>
    /// Uygulamanýn veri tabanýna eriþip istenilen SQL cümlelerinin çalýþtýrýlabildiði sayfa
    /// Bu sayfa uygulama ile ilgili teknik iþler yapan insanlarýn kullanýmý için yapýlmýþtýr.
    /// Bu nedenle sadece stratek kullanýcýsý bu sayfada iþlem yapabilir, yetki kontrolü bu yöndedir.
    /// </summary>
    public partial class Listele : System.Web.UI.Page
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Ýþlemi yapan kullanýcýya ait bilgileri tutan nesne
        /// </summary>
        Kullanici kullanan;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir
        ///     varsa ve sayfadaki yetki kontrolünü geçerse sayfa yüklenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                !(kullanan.kullaniciKodu == "stratek" || kullanan.kullaniciKodu == "cunver" ||
                kullanan.kullaniciKodu == "39228" ||
                kullanan.kullaniciKodu == "29319"))
                GenelIslemler.SayfayaGirmesin(false, Resources.TasinirMal.FRMSQL001);

            if (!IsPostBack)
            {
            }
        }

        /// <summary>
        /// Çalýþtýr tuþuna basýlýnca çalýþan olay metodu
        /// Sayfadaki txtSQL TextBox kontrolüne yazýlan SQL cümlesini çalýþtýrýlmak üzere
        /// sunucudaki SQLCalistir yordamýna gönderir ve dönen sonucu ekrana yazar.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        [DirectMethod]
        public void SqlCalistir(string sqlGel, string baglanti, int excel)
        {
            if (string.IsNullOrWhiteSpace(baglanti))
                baglanti = "BaglantiSatiriTMM";

            string sql = File.ReadAllText(Server.MapPath("~") + "/Listele.txt");

            int yer1 = sql.IndexOf("/*");
            while (yer1 > -1)
            {
                int yer2 = sql.IndexOf("*/");
                if (yer2 > 0)
                    sql = sql.Substring(0, yer1) + sql.Substring(yer2 + 2);
                else
                {
                    pnlSonuc.Html = Resources.TasinirMal.FRMSQL003;
                    return;
                }
                yer1 = sql.IndexOf("/*");
            }
            if (excel == 2)
                ExcelUret(sql, baglanti);
            else
                pnlSonuc.Html = servisTMM.SQLCalistir(kullanan, baglanti, sql);
        }

        private void ExcelUret(string sql, string baglanti)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(dosyaAd);

            TNS.UZY.VTSorgula vts = new TNS.UZY.VTSorgula(sql, baglanti, 20000);

            int sat = 0;
            XLS.HucreDegerYaz(sat, 0, baglanti);
            XLS.HucreDegerYaz(sat, 1, sql);
            XLS.HucreBirlestir(sat, 1, sat, 5);
            sat++;
            Type[] tipler = null;
            int kolonSay = 0;
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;
                if (sat == 1)
                {
                    kolonSay = row.ItemArray.Length;
                    tipler = new Type[kolonSay];
                    for (int i = 0; i < kolonSay; i++)
                    {
                        tipler[i] = typeof(System.DBNull);
                        XLS.HucreDegerYaz(sat, i, row.Table.Columns[i].ColumnName);
                    }
                    sat++;
                }
                for (int i = 0; i < kolonSay; i++)
                {
                    Type t = row.ItemArray[i].GetType();
                    if (tipler != null)
                    {
                        if (tipler[i] == typeof(System.DBNull))
                            tipler[i] = t;
                        if (IntMi(t))
                            XLS.HucreDegerYaz(sat, i, vts.IntAl(i));
                        else if (DblMi(t))
                            XLS.HucreDegerYaz(sat, i, vts.DoubleAl(i));
                        else if (TarihMi(t))
                            XLS.HucreDegerYaz(sat, i, vts.DTAl(i));
                        else
                            XLS.HucreDegerYaz(sat, i, vts.StringAl(i));
                    }
                }
                sat++;
            }
            if (sat == 1)
            {
                XLS.HucreDegerYaz(sat, 0, vts.hata);
            }
            else
            {
                for (int i = 0; i < kolonSay; i++)
                {
                    if (tipler != null)
                    {
                        if (TarihMi(tipler[i]))
                            XLS.HucreFormatla(2, i, sat, i, "dd.mm.yyyy hh:mm");
                    }
                }
            }
            XLS.DosyaSaklaTamYol();
            DosyaIslem.DosyaGonderX(dosyaAd, "Sonuc_" + DateTime.Now.Ticks.ToString() + ".xlsx", true, "xlsx");
        }

        private bool TarihMi(Type type)
        {
            if (type == typeof(DateTime))
                return true;
            return false;
        }

        private bool DblMi(Type type)
        {
            if (type == typeof(double) || type == typeof(decimal))
                return true;
            return false;
        }

        private bool IntMi(Type type)
        {
            if (type == typeof(int))
                return true;
            return false;
        }
    }
}