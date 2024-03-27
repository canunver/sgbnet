using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using TNS.KYM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr hesap planý listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeHesapPlani : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        static ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa adresinde gelen hesapKod ve cokluSecim girdi dizgilerini kullanarak
        ///     taþýnýr hesap planlarýný aðaç þeklinde hiyerarþik olarak listeler.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                ResourceManager1.RegisterIcon(Icon.Folder);

                Ext1.Net.TreeNode root = new Ext1.Net.TreeNode(Resources.TasinirMal.FRMLHP002);
                root.Expanded = true;
                trvHesap.Root.Add(root);

                HesapPlaniYukle(root);
                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["cokluSecim"], 0) == 1)
                {
                    btnSec.Visible = true;
                    trvHesap.Listeners.BeforeClick.Fn = string.Empty;
                }
                hdnBilgi.Text = GenelIslemlerIstemci.RequestBul(this, "bilgi", "");
            }
        }

        /// <summary>
        /// Dals the yukle.
        /// </summary>
        /// <param name="hesapKod">The hesap kod.</param>
        /// <returns></returns>
        [Ext1.Net.DirectMethod(ClientProxy = Ext1.Net.ClientProxy.Ignore)]
        public static string DalYukle(string hesapKod, bool coklu)
        {
            string[] hesapKirilimlar = hesapKod.Split('.');

            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.hesapKod = hesapKod;
            if (hesapKod == "")
                hs.seviye = 1;
            //else if (hesapKod.Length < 3)
            //    hs.seviye = 1;
            else
                hs.seviye = hesapKirilimlar.Length + 1;

            ObjectArray hv = new ObjectArray();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    hv = servisTMM.HesapPlaniListele(new Kullanici(), hs, new Sayfalama());
                    if (hv.objeler.Count == 0)
                        hs.seviye++;
                    else
                        break;
                }
            }
            catch (Exception exc)
            {
                hv = new ObjectArray();
                hv.sonuc = new Sonuc(false, exc.Message, "");
            }
            Ext1.Net.TreeNodeCollection nodes = new Ext1.Net.TreeNodeCollection();
            foreach (HesapPlaniSatir h in hv.objeler)
            {
                Ext1.Net.AsyncTreeNode asyncNode = new Ext1.Net.AsyncTreeNode();
                if (h.olcuBirim == 0)
                    asyncNode.Text = h.hesapKod + " - " + h.aciklama;
                else
                    asyncNode.Text = h.hesapKod + " - " + h.aciklama + " - " + h.olcuBirimAd;

                asyncNode.Cls = h.hesapKod + "|" + h.aciklama + "|" + h.olcuBirimAd + "|" + h.kdv + "|" + h.rfidEtiketKod + "|" + h.markaKod + "|" + h.modelKod;

                asyncNode.NodeID = h.hesapKod;
                if (h.detay)
                {
                    asyncNode.Leaf = true;
                    if (coklu)
                        asyncNode.Checked = ThreeStateBool.False;
                }
                else
                    asyncNode.Leaf = false;
                nodes.Add(asyncNode);
            }

            return nodes.ToJson();
        }

        /// <summary>
        /// Dals the yukle arama.
        /// </summary>
        /// <returns></returns>
        [DirectMethod]
        public string DalYukleArama()
        {
            Ext1.Net.TreeNodeCollection nodes = new TreeNodeCollection();

            HesapPlaniSatir h = new HesapPlaniSatir();

            if (tabPanelGenel.ActiveTabIndex == 1)
            {
                h.hesapKod = txtHesapKod.Text.Trim();
                h.aciklama = txtHesapAd.Text.Trim();

                if (h.hesapKod.Trim() == "" && h.aciklama.Trim() == "") return "";
            }
            else if (tabPanelGenel.ActiveTabIndex == 2)
            {
                h.hesapKoduBarkod.barkod = txtHesapKodBarkod.Text.Trim();
                if (h.hesapKoduBarkod.barkod.Trim() == "") return "";
            }

            ObjectArray hv = servisTMM.HesapPlaniListele(new Kullanici(), h, new Sayfalama());
            if (hv.sonuc.islemSonuc)
            {
                Ext1.Net.TreeNode node = new Ext1.Net.TreeNode("", "Hesap Planý", Icon.None);
                node.Expanded = true;
                nodes.Add(node);

                foreach (HesapPlaniSatir hesapPlani in hv.objeler)
                {
                    string ad = hesapPlani.hesapKod + " - " + hesapPlani.aciklama;
                    if (hesapPlani.olcuBirim != 0)
                        ad += " - " + hesapPlani.olcuBirimAd;
                    Ext1.Net.TreeNode yeniDal = new Ext1.Net.TreeNode(hesapPlani.hesapKod, ad, hesapPlani.detay ? Icon.None : Icon.Folder);
                    Ext1.Net.TreeNode anaNode = AnaNodeBul(nodes, hesapPlani.hesapKod);

                    yeniDal.Cls = hesapPlani.hesapKod + "|" + hesapPlani.aciklama + "|" + hesapPlani.olcuBirimAd + "|" + hesapPlani.kdv + "|" + hesapPlani.rfidEtiketKod + "|" + hesapPlani.markaKod + "|" + hesapPlani.modelKod;

                    if (anaNode != null)
                        anaNode.Nodes.Add(yeniDal);
                    else
                        node.Nodes.Add(yeniDal);
                }
            }

            return nodes.ToJson();
        }

        private Ext1.Net.TreeNode AnaNodeBul(Ext1.Net.TreeNodeCollection nodes, string hesapKod)
        {
            if (nodes == null || nodes.Count <= 0)
                return null;

            for (int i = 0; i < nodes.Count; i++)
            {
                Ext1.Net.TreeNode node = (Ext1.Net.TreeNode)nodes[i];
                int l = hesapKod.LastIndexOf('.');
                if (l <= 0)
                    l = hesapKod.Length;
                if (node.NodeID == hesapKod.Substring(0, l))
                    return node;
                else
                {
                    Ext1.Net.TreeNode yeniNode = AnaNodeBul(((Ext1.Net.TreeNode)node).Nodes, hesapKod);
                    if (yeniNode != null)
                        return yeniNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Parametre olarak verilen seciliNode dalýna sunucudan çekilen
        /// ilgili taþýnýr hesap planlarý yeni dal olarak eklenir.
        /// </summary>
        /// <param name="root">Seçili taþýnýr hesap planýna ait dal</param>
        private void HesapPlaniYukle(TreeNode root)
        {
            TNS.TMM.HesapPlaniSatir hs = new TNS.TMM.HesapPlaniSatir();
            hs.seviye = 1;
            string ekranTip = "";

            ObjectArray hv;
            try
            {
                ekranTip = Request.QueryString["ekranTip"] + "";
                if (ekranTip == "YZ")
                    hs.hesapKod = "094";
                else if (ekranTip == "GM")
                    hs.hesapKodAciklama = "@097 098";

                hv = servisTMM.HesapPlaniListele(new TNS.KYM.Kullanici(), hs, new Sayfalama());
            }
            catch (Exception exc)
            {
                hv = new ObjectArray();
                hv.sonuc = new Sonuc(false, exc.Message, "");
            }

            if (hv.sonuc.islemSonuc)
            {
                Ext1.Net.AsyncTreeNode node;
                for (int i = 0; i < hv.ObjeSayisi; i++)
                {
                    hs = (TNS.TMM.HesapPlaniSatir)hv[i];

                    if (ekranTip == "" && TNS.TMM.Arac.MerkezBankasiKullaniyor() && (hs.hesapKod.StartsWith("094") || hs.hesapKod.StartsWith("097") || hs.hesapKod.StartsWith("098")))
                        continue;

                    string ad = hs.hesapKod + " - " + hs.aciklama;
                    if (hs.olcuBirim != 0)
                        ad += " - " + hs.olcuBirimAd;

                    node = new AsyncTreeNode(hs.hesapKod, ad);
                    node.Cls = hs.hesapKod + "|" + hs.aciklama + "|" + hs.olcuBirimAd + "|" + hs.kdv + "|" + hs.rfidEtiketKod + "|" + hs.markaKod + "|" + hs.modelKod;

                    root.Nodes.Add(node);
                }

                string coklu = "false";
                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["cokluSecim"], 0) == 1)
                    coklu = "true";
                trvHesap.Listeners.BeforeLoad.Handler = "DalYukle(node, " + coklu + ")";
            }
        }
    }
}