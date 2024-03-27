using System;
using OrtakClass;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;
using System.IO;

namespace TasinirMal
{
    /// <summary>
    /// Uygulaman�n veri taban�na eri�ip istenilen SQL c�mlelerinin �al��t�r�labildi�i sayfa
    /// Bu sayfa uygulama ile ilgili teknik i�ler yapan insanlar�n kullan�m� i�in yap�lm��t�r.
    /// Bu nedenle sadece stratek kullan�c�s� bu sayfada i�lem yapabilir, yetki kontrol� bu y�ndedir.
    /// </summary>
    public partial class Listele : System.Web.UI.Page
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// ��lemi yapan kullan�c�ya ait bilgileri tutan nesne
        /// </summary>
        Kullanici kullanan;

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir
        ///     varsa ve sayfadaki yetki kontrol�n� ge�erse sayfa y�klenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
        /// �al��t�r tu�una bas�l�nca �al��an olay metodu
        /// Sayfadaki txtSQL TextBox kontrol�ne yaz�lan SQL c�mlesini �al��t�r�lmak �zere
        /// sunucudaki SQLCalistir yordam�na g�nderir ve d�nen sonucu ekrana yazar.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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