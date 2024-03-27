using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Oda listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeOdaMB : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        static ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir varsa sayfa yüklenir.
        ///     Sayfa adresinde gelen mb, hb ve ab girdi dizgileri kullanýlarak odalar cevap (response) olarak döndürülür.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                //string kisiKimlikNo = Request.QueryString["kk"] + "";

                //if (kisiKimlikNo != "") KisininOdalari(kisiKimlikNo);

                ResourceManager1.RegisterIcon(Icon.Folder);

                Ext1.Net.TreeNode root = new Ext1.Net.TreeNode("Oda Listesi");
                root.Expanded = true;
                trvOda.Root.Add(root);

                OdaYukle(root);
                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["cokluSecim"], 0) == 1)
                {
                    //btnSec.Visible = true;
                    trvOda.Listeners.BeforeClick.Fn = string.Empty;
                }
                hdnBilgi.Text = GenelIslemlerIstemci.RequestBul(this, "bilgi", "");
            }
        }


        /// <summary>
        /// Dals the yukle.
        /// </summary>
        /// <param name="odaKod">The hesap kod.</param>
        /// <returns></returns>
        [Ext1.Net.DirectMethod(ClientProxy = Ext1.Net.ClientProxy.Ignore)]
        public static string DalYukle(string odaKod)
        {
            //string[] odaKirilimlar = odaKod.Split('.');

            Oda hs = new Oda();
            if (odaKod == "")
                hs.seviye = 1;
            else
                hs.ustKod = OrtakFonksiyonlar.ConvertToInt(odaKod, 0);

            hs.durum = (int)ENUMOdaDurum.AKTIF;

            ObjectArray hv = new ObjectArray();
            try
            {
                //for (int i = 0; i < 10; i++)
                //{
                hv = servisTMM.OdaListele(new TNS.KYM.Kullanici(), hs);
                //if (hv.objeler.Count == 0)
                //hs.seviye++;
                //    else
                //        break;
                //}
            }
            catch (Exception exc)
            {
                hv = new ObjectArray();
                hv.sonuc = new Sonuc(false, exc.Message, "");
            }
            Ext1.Net.TreeNodeCollection nodes = new Ext1.Net.TreeNodeCollection();
            foreach (Oda h in hv.objeler)
            {
                Ext1.Net.AsyncTreeNode asyncNode = new Ext1.Net.AsyncTreeNode();
                asyncNode.Text = h.kod + " - " + h.ad;

                asyncNode.Cls = h.kod + "|" + h.ad + "|" + h.ustKod;

                asyncNode.NodeID = h.kod;

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

            Oda h = new Oda();
            string muhasebeKod = Request.QueryString["mb"] + "";
            string harcamaBirimKod = Request.QueryString["hb"] + "";

            if (rdKurum.Checked)
            {
                h.muhasebeKod = string.Empty;
                h.harcamaBirimKod = string.Empty;
            }
            else if (rdMuhasebe.Checked)
            {
                h.muhasebeKod = muhasebeKod;
                h.harcamaBirimKod = string.Empty;
            }
            else if (rdHarcamaBirimi.Checked)
            {
                h.muhasebeKod = muhasebeKod;
                h.harcamaBirimKod = harcamaBirimKod;
            }
            else
            {
                h.muhasebeKod = muhasebeKod;
                h.harcamaBirimKod = harcamaBirimKod;
            }
            h.harcamaBirimKod = harcamaBirimKod;
            h.ad = txtFiltre.Text;

            //if (tabPanelGenel.ActiveTabIndex == 1)
            //{
            //    h.hesapKod = txtHesapKod.Text.Trim();
            //    h.aciklama = txtHesapAd.Text.Trim();

            //    if (h.hesapKod.Trim() == "" && h.aciklama.Trim() == "") return "";
            //}
            //else if (tabPanelGenel.ActiveTabIndex == 2)
            //{
            //    h.hesapKoduBarkod.barkod = txtHesapKodBarkod.Text.Trim();
            //    if (h.hesapKoduBarkod.barkod.Trim() == "") return "";
            //}

            ObjectArray hv = servisTMM.OdaListele(kullanan, h);
            if (hv.sonuc.islemSonuc)
            {
                Ext1.Net.TreeNode node = new Ext1.Net.TreeNode("", "Oda Listesi", Icon.None);
                node.Expanded = true;
                nodes.Add(node);

                foreach (Oda oda in hv.objeler)
                {
                    string ad = oda.kod + " - " + oda.ad;
                    Ext1.Net.TreeNode yeniDal = new Ext1.Net.TreeNode(oda.kod, ad, Icon.Folder);
                    Ext1.Net.TreeNode anaNode = AnaNodeBul(nodes, oda.kod);

                    yeniDal.Cls = oda.kod + "|" + oda.ad + "|" + oda.ustKod;

                    if (anaNode != null)
                        anaNode.Nodes.Add(yeniDal);
                    else
                        node.Nodes.Add(yeniDal);
                }
            }

            return nodes.ToJson();
        }

        private Ext1.Net.TreeNode AnaNodeBul(Ext1.Net.TreeNodeCollection nodes, string odaKod)
        {
            if (nodes == null || nodes.Count <= 0)
                return null;

            for (int i = 0; i < nodes.Count; i++)
            {
                Ext1.Net.TreeNode node = (Ext1.Net.TreeNode)nodes[i];
                int l = odaKod.LastIndexOf('.');
                if (l <= 0)
                    l = odaKod.Length;
                if (node.NodeID == odaKod.Substring(0, l))
                    return node;
                else
                {
                    Ext1.Net.TreeNode yeniNode = AnaNodeBul(((Ext1.Net.TreeNode)node).Nodes, odaKod);
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
        private void OdaYukle(TreeNode root)
        {
            Oda oda = new Oda();
            oda.muhasebeKod = Request.QueryString["mb"] + "";
            oda.harcamaBirimKod = Request.QueryString["hb"] + "";
            oda.seviye = 1;
            oda.durum = (int)ENUMOdaDurum.AKTIF;

            ObjectArray bilgi;
            try
            {
                bilgi = servisTMM.OdaListele(kullanan, oda);
            }
            catch (Exception exc)
            {
                bilgi = new ObjectArray();
                bilgi.sonuc = new Sonuc(false, exc.Message, "");
            }

            if (bilgi.sonuc.islemSonuc)
            {
                Ext1.Net.AsyncTreeNode node;
                for (int i = 0; i < bilgi.ObjeSayisi; i++)
                {
                    oda = (Oda)bilgi[i];

                    string ad = oda.kod + " - " + oda.ad;

                    node = new AsyncTreeNode(oda.kod, ad);
                    node.Cls = oda.kod + "|" + oda.ad + "|" + oda.ustKod;

                    root.Nodes.Add(node);
                }

                string coklu = "false";
                trvOda.Listeners.BeforeLoad.Handler = "DalYukle(node, " + coklu + ")";
            }
        }

        private void KisininOdalari(string kisiKimlikNo)
        {
            Ext1.Net.TreeNodeCollection nodes = new TreeNodeCollection();
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.muhasebeKod = Request.QueryString["mb"] + "";
            shBilgi.harcamaBirimKod = Request.QueryString["hb"] + "";
            shBilgi.kimeGitti = kisiKimlikNo;

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());

            List<object> liste = new List<object>();
            Hashtable htOda = new Hashtable();
            foreach (SicilNoHareket dt in bilgi.objeler)
            {
                if (htOda[dt.odaKod] != null)
                    continue;

                htOda[dt.odaKod] = dt.odaAd;

                liste.Add(new
                {
                    KOD = dt.odaKod,
                    ADI = dt.odaAd,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        [DirectMethod]
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Oda oda = new Oda();
            string muhasebeKod = Request.QueryString["mb"] + "";
            string harcamaBirimKod = Request.QueryString["hb"] + "";

            if (rdKurum.Checked)
            {
                oda.muhasebeKod = string.Empty;
                oda.harcamaBirimKod = string.Empty;
            }
            else if (rdMuhasebe.Checked)
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = string.Empty;
            }
            else if (rdHarcamaBirimi.Checked)
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = harcamaBirimKod;
            }
            else
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = harcamaBirimKod;
            }
            oda.harcamaBirimKod = harcamaBirimKod;
            oda.ad = txtFiltre.Text;
            ObjectArray bilgi = servisTMM.OdaListele(kullanan, oda);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLOD002);

            List<object> liste = new List<object>();
            foreach (Oda o in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = o.kod,
                    ADI = o.ad,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}