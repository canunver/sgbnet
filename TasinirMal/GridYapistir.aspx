<%@ Page Language="C#" CodeBehind="GridYapistir.aspx.cs" Inherits="TasinirMal.GridYapistir" EnableViewState="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Grid Yapıştır</title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="javascript" type="text/javascript">
        function YardimAc() {

            var cagiran = hdnCagiran.getValue();
            var bilgiler = cagiran.split(':');

            var yardimDosya = "../App_themes/images/ExceldenYapistirOrnek.png";

            if (bilgiler[1] == "SICILNO" || bilgiler[1] == "PRSICILNO")
                yardimDosya = "../App_themes/images/ExceldenYapistirSicilNoOrnek.png";
            else if (bilgiler[1] == "SICILNOOZELLIKYAPISTIR")
                yardimDosya = "../App_themes/images/ExceldenYapistirSeriNoOrnek.png";


            PopupWin(yardimDosya, 450, 450, 'Yardim');
        }

        function HesapPlanKoduYapistir() {

            var muhasebeKod = "";
            var harcamaBirimiKod = "";

            try {
                muhasebeKod = window.parent.document.getElementById('txtMuhasebe').value;
                harcamaBirimiKod = window.parent.document.getElementById('txtHarcamaBirimi').value;
            } catch (e) {
            }

            var cagiran = hdnCagiran.getValue();
            var bilgiler = cagiran.split(':');
            var grid = parent.extKontrol.getCmp(bilgiler[0]);

            var colKod = "";
            var colAdi = "";
            var colMiktar = "";
            var colOlcu = "";
            var colFiyat = "";

            var colPrSicilNo = "";
            var colSicilNo = "";

            var colAciklama = "";


            if (bilgiler[1] == "TASINIRHESAPKOD" || bilgiler[1] == "hesapPlanKod") {

                if (bilgiler.length > 1) colKod = bilgiler[1];
                if (bilgiler.length > 2) colAdi = bilgiler[2];
                if (bilgiler.length > 3) colOlcu = bilgiler[3];
                if (bilgiler.length > 4) colKDV = bilgiler[4];
                if (bilgiler.length > 5) colMiktar = bilgiler[5];
                if (bilgiler.length > 6) colFiyat = bilgiler[6];

                var kriter = txtHesapPlan.getValue();
                var row = 0;
                var store = grid.getStore();
                //Yapıştırılacak son satırı bul
                for (var i = 0; i < store.data.length; i++) {
                    if (store.data.items[i].data[colKod] == "") {
                        row = i;
                        i = store.data.length + 1;
                    }
                }

                Ext1.net.DirectMethods.HesapPlanKoduYapistir(kriter, {
                    success: function (sonuc) {

                        GridSatirAc(store, sonuc.length);

                        for (var i = 0; i < sonuc.length; i++) {
                            if (colKod != "") store.getAt(row + i).set(colKod, sonuc[i].hesapKod);
                            if (colAdi != "") store.getAt(row + i).set(colAdi, sonuc[i].aciklama);
                            if (colOlcu != "") store.getAt(row + i).set(colOlcu, sonuc[i].olcuBirimAd);

                            if (colMiktar != "") store.getAt(row + i).set(colMiktar, sonuc[i].numara);
                            if (colKDV != "") store.getAt(row + i).set(colKDV, sonuc[i].kdv);
                            if (colFiyat != "") store.getAt(row + i).set(colFiyat, sonuc[i].hesapKodAciklama);
                        }

                        txtHesapPlan.setValue("");
                        parent.hidePopWin();
                    },
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                });
            }
            else if (bilgiler[1] == "PRSICILNO") {

                if (bilgiler.length > 1) colPrSicilNo = bilgiler[1];
                if (bilgiler.length > 2) colSicilNo = bilgiler[2];
                if (bilgiler.length > 3) colKod = bilgiler[3];
                if (bilgiler.length > 4) colAdi = bilgiler[4];
                if (bilgiler.length > 5) colFiyat = bilgiler[5];
                if (bilgiler.length > 6) colBulunduguYer = bilgiler[6];
                if (bilgiler.length > 7) colESicilNo = bilgiler[7];

                var kriter = txtHesapPlan.getValue();
                var row = 0;
                var store = grid.getStore();
                //Yapıştırılacak son satırı bul
                for (var i = 0; i < store.data.length; i++) {
                    if (store.data.items[i].data[colKod] == "") {
                        row = i;
                        i = store.data.length + 1;
                    }
                }

                Ext1.net.DirectMethods.SicilNoYapistir(kriter, muhasebeKod, harcamaBirimiKod, {
                    success: function (sonuc) {

                        GridSatirAc(store, sonuc.length);

                        for (var i = 0; i < sonuc.length; i++) {
                            if (colPrSicilNo != "") store.getAt(row + i).set(colPrSicilNo, sonuc[i].prSicilNo);
                            if (colSicilNo != "") store.getAt(row + i).set(colSicilNo, sonuc[i].sicilNo);

                            if (colKod != "") store.getAt(row + i).set(colKod, sonuc[i].hesapPlanKod);
                            if (colAdi != "") store.getAt(row + i).set(colAdi, sonuc[i].hesapPlanAd);

                            if (colFiyat != "") store.getAt(row + i).set(colFiyat, sonuc[i].fiyat);
                            if (colBulunduguYer != "") store.getAt(row + i).set(colBulunduguYer, sonuc[i].odaAd);
                            if (colESicilNo != "") store.getAt(row + i).set(colESicilNo, sonuc[i].eSicilNo);
                        }

                        txtHesapPlan.setValue("");
                        parent.hidePopWin();
                    },
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                });
            }
            else if (bilgiler[1] == "SICILNO") {

                if (bilgiler.length > 1) colSicilNo = bilgiler[1];
                if (bilgiler.length > 2) colKod = bilgiler[2];
                if (bilgiler.length > 3) colAdi = bilgiler[3];
                if (bilgiler.length > 4) colFiyat = bilgiler[4];

                var kriter = txtHesapPlan.getValue();
                var row = 0;
                var store = grid.getStore();
                //Yapıştırılacak son satırı bul
                for (var i = 0; i < store.data.length; i++) {
                    if (store.data.items[i].data[colKod] == "") {
                        row = i;
                        i = store.data.length + 1;
                    }
                }

                Ext1.net.DirectMethods.SicilNoYapistir(kriter, muhasebeKod, harcamaBirimiKod, {
                    success: function (sonuc) {

                        GridSatirAc(store, sonuc.length);

                        for (var i = 0; i < sonuc.length; i++) {
                            if (colSicilNo != "") store.getAt(row + i).set(colSicilNo, sonuc[i].sicilNo);

                            if (colKod != "") store.getAt(row + i).set(colKod, sonuc[i].hesapPlanKod);
                            if (colAdi != "") store.getAt(row + i).set(colAdi, sonuc[i].hesapPlanAd);

                            if (colFiyat != "") store.getAt(row + i).set(colFiyat, sonuc[i].fiyat);
                        }

                        txtHesapPlan.setValue("");
                        parent.hidePopWin();
                    },
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                });
            }
            else if (bilgiler[1] == "SICILNOOZELLIKYAPISTIR") {
                if (bilgiler.length > 1) colSicilNoOzellikYapistir = bilgiler[1];
                if (bilgiler.length > 2) colPrSicilNo = bilgiler[2];
                if (bilgiler.length > 3) colSicilNo = bilgiler[3];
                if (bilgiler.length > 4) colKod = bilgiler[4];
                if (bilgiler.length > 5) colAdi = bilgiler[5];
                if (bilgiler.length > 6) colAciklama = bilgiler[6];

                var kriter = txtHesapPlan.getValue();
                var data = kriter.split('\n');
                var store = grid.getStore();

                for (var i = 0; i < data.length; i++) {
                    store.getAt(i).set(colAciklama, data[i]);
                }

                txtHesapPlan.setValue("");
                parent.hidePopWin();

            }
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="pnlViewport" runat="server" Region="Center" Border="false" Layout="FitLayout">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnAktar" runat="server" Text="Aktar" Icon="PlayBlue">
                                    <Listeners>
                                        <Click Handler="HesapPlanKoduYapistir();"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button runat="server" Icon="Help" Text="Yardım">
                                    <Listeners>
                                        <Click Handler="YardimAc();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:TextArea ID="txtHesapPlan" runat="server" AutoFocus="true" AutoFocusDelay="100" />
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
