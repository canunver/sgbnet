<%@ Page Language="C#" CodeBehind="TasinirIslemFormMB.aspx.cs" Inherits="TasinirMal.TasinirIslemFormMB"
    EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?v=1"></script>
    <script language="JavaScript" type="text/javascript">
        var editorCancel = false;
        var hesapKodEditorYok = false;

        var BirimFiyatFormatla = function (value, b, c) {
            if (hdnKurumTur.getValue() == 'MerkezBanka')
                return Ext1.util.Format.number(value, '0.0,00/i');
            else
                return value;
        }

        var HesapPlaniGoster = function (row) {
            var islemTur = ddlIslemTipi.value.split('*')[1];

            if (Number(islemTur) >= 50 || islemTur == "-1") {
                StokListesiAc(row, islemTur);
            }
            else {
                gridYazilacakSatirNo = row;
                grdListe.getSelectionModel().selectRow(row);
                window.setTimeout('grdListe.stopEditing();', 10);
                ListeAc('ListeHesapPlani.aspx', 'grdListe:hesapPlanKod:hesapPlanAd:olcuBirimAd:kdvOran:rfidEtiketTurKod:markaKod:modelKod', '', '');
            }
        }

        var StokListesiAc = function (row, islemTur) {
            gridYazilacakSatirNo = row; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
            grdListe.getSelectionModel().selectRow(row);

            var store = grdListe.getStore();

            var sicilNolar = "";

            for (var i = 0; i < store.data.items.length; i++) {
                var sicilNo = store.data.items[i].data.SICILNO;

                if (sicilNo != "" && sicilNo != undefined) {
                    if (sicilNolar != "") sicilNolar += ",";
                    sicilNolar += store.data.items[i].data.SICILNO;
                }
            }

            ListeAc('ListeStokYeni.aspx', 'grdListe:hesapPlanKod:hesapPlanAd:miktar:olcuBirimAd:kdvOran:birimFiyat', sicilNolar, '');
        }

        var SicilNoListesiAc = function (row) {
            var islemTur = ddlIslemTipi.value.split('*')[1];

            if ((Number(islemTur) >= 0 && Number(islemTur) <= 49) || islemTur == "50") {
                Ext1.Msg.show({ title: "Uyarı", msg: "Bu işlem tipi için sicil numarası seçilemez", icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
            }
            else {
                sicilListesiAmbarZimmetDurumu = "ambar"; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                gridYazilacakSatirNo = row; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                grdListe.getSelectionModel().selectRow(row);

                var store = grdListe.getStore();
                var sicilNolar = "";

                for (var i = 0; i < store.data.items.length; i++) {
                    var sicilNo = store.data.items[i].data.gorSicilNo;

                    if (sicilNo != "" && sicilNo != undefined) {
                        sicilNolar += store.data.items[i].data.gorSicilNo + ",";
                    }
                }

                iliskiMalzemeEklemeDurumu = 1;
                ListeAc('ListeSicilNoYeni.aspx', 'grdListe:hesapPlanKod:hesapPlanAd:gorSicilNo:miktar:olcuBirimAd:kdvOran:birimFiyat', sicilNolar, '');
            }
        }

        var YerleskeYeriListesiAc = function (row) {
            gridYazilacakSatirNo = row; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
            grdListe.getSelectionModel().selectRow(row);

            ListeAc('ListeOdaMB.aspx', 'grdListe:yerleskeYeri:yerleskeYeriAd', 'txtHarcamaBirimi', '');
        }

        var YerleskeHepsiniDoldur = function (kod, ad) {
            var i = 0;
            Ext1.each(grdListe.getStore().data.items, function (item) {
                if (item.data["hesapPlanKod"] != null) {
                    item.set("yerleskeYeri", kod);
                    item.set("yerleskeYeriAd", ad);
                }
                i++;
            });
        }

        var ddlIslemTipiSec = function (alan, deger) {
            Ext1.each(ddlIslemTipi.getStore().data.items, function (item) {
                if (item.data[alan] == deger) {
                    ddlIslemTipi.setValue(item.data.KODTUR);
                }
            });
        }

        var YazdirKontrol = function () {
            if (txtBelgeNo.getValue() == "") {
                Ext1.Msg.show({ title: "Uyarı", msg: "Belge No boş olamaz.", icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
                return false;
            }
            return true;
        }

        var KomisyonGoster = function () {
            showPopWin("TasinirIslemKomisyon.htm?menuYok=1", 320, 210, true, null);
        }

        var BelgeOzellikGoster = function () {
            var adres = "SicilNoOzellik.aspx?menuYok=1";
            adres += "&yil=" + txtYil.getValue();
            adres += "&mb=" + txtMuhasebe.getValue();
            adres += "&hbk=" + txtHarcamaBirimi.getValue();
            adres += "&ak=" + txtAmbar.getValue();
            adres += "&belgeNo=" + txtBelgeNo.getValue();
            adres += "&tur=" + hdnBelgeTur.getValue();

            showPopWin(adres, 800, 500, true, null);
        }

        var BarkodEkraniAc = function () {
            var adres = "BarkodYazdir.aspx?bTur=TIF&menuYok=1";
            adres += "&y=" + txtYil.getValue();
            adres += "&m=" + txtMuhasebe.getValue();
            adres += "&h=" + txtHarcamaBirimi.getValue();
            adres += "&a=" + txtAmbar.getValue();
            adres += "&b=" + txtBelgeNo.getValue();

            showPopWin(adres, 800, 500, true, null);
        }

        var DevirListesiAc = function () {
            var islemTur = ddlIslemTipi.value.split('*')[1];

            if (Number(islemTur) != 5 && Number(islemTur) != 8)//5-Devir Giriş 8-Dağıtım/İade Belgesi (Giriş);
            {
                Ext1.Msg.show({ title: "Uyarı", msg: res_FRMJSC007, icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
            }
            else {
                var adres = "ListeDevir.aspx?menuYok=1";
                adres += "&yil=" + txtYil.getValue();
                adres += "&mb=" + txtMuhasebe.getValue();
                adres += "&hbk=" + txtHarcamaBirimi.getValue();
                adres += "&ak=" + txtAmbar.getValue();
                adres += "&b=" + txtBelgeNo.getValue();
                adres += "&is=" + islemTur;

                showPopWin(adres, 900, 400, true, null);
            }
        }

        var BelgeAc = function (yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtYil.setValue(yil);
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }

        var isValid = function () {
            if (pnlTanim.getForm().isValid())
                return true;
            Ext1.Msg.show({ title: "Uyarı", msg: "Form üzerinde hatalı girilmiş bilgiler var. Lütfen kontrol ediniz.", icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
            return false;
        }

        var KomutCalistir = function (command, record, row) {
            grdListe.getSelectionModel().selectRow(row);
            var c = grdListe.getColumnModel().getColumnById(command);
            if (command == 'hesapPlanKod' && c.editable) {
                HesapPlaniGoster(row);
                editorCancel = true;
            }
            else if (command == 'gorSicilNo' && c.editable) {
                SicilNoListesiAc(row);
                editorCancel = true;
            }
            else if (command == 'yerleskeYeriAd') {
                YerleskeYeriListesiAc(row);
            }
        };

        var BeforeEdit = function (e) {
            if (editorCancel || (hesapKodEditorYok && e.field == "hesapPlanKod")) {
                e.cancel = true;
                editorCancel = false;
            }
        }

        var AfterEdit = function (e) {
            if (e.field == "markaKod") {
                e.record.set("modelKod", 0);
            }
        }

        var HesapPlanKodSelect = function (a, b) {
            var store = grdListe.getStore();
            var rowIndex = store.indexOf(grdListe.getSelectionModel().getSelected());
            var satir = store.getAt(rowIndex);
            satir.set("hesapPlanKod", b.data.hesapPlanKod);
            satir.set("hesapPlanAd", b.data.hesapPlanAd);
            satir.set("olcuBirimAd", b.data.olcuBirimAd);
            satir.set("kdvOran", b.data.kdvOran);
            satir.set("rfidEtiketTurKod", b.data.rfidEtiketKod);
            satir.set("markaKod", b.data.markaKod);
            satir.set("modelKod", b.data.modelKod);
        }

        var ToplamHesapla = function () {
            var fisToplami = 0;

            Ext1.each(grdListe.getStore().data.items, function (item) {
                var birimFiyat = item.data["birimFiyat"]
                var miktar = item.data["miktar"]
                var kdv = item.data["kdvOran"]
                if (birimFiyat != "" || miktar != "") {
                    birimFiyat = Number(ScriptParaCevir(birimFiyat));
                    miktar = Number(ScriptParaCevir(miktar));

                    kdv = 1 + (Number(kdv) / 100);
                    fisToplami += (birimFiyat * miktar * kdv);
                }
            });

            Ext1.Msg.show({ title: "Toplam", msg: ParayaCevir(fisToplami), icon: Ext1.Msg.INFO, buttons: Ext1.Msg.OK });
        }

        var ParayaCevir = function (sayi) {
            sayi = sayi.toString().replace(/\$|\,/g, '');
            if (isNaN(sayi))
                sayi = "0";
            isaret = (sayi == (sayi = Math.abs(sayi)));
            sayi = Math.floor(sayi * 100 + 0.50000000001);
            kurus = sayi % 100;
            sayi = Math.floor(sayi / 100).toString();
            if (kurus < 10)
                kurus = "0" + kurus;
            for (var i = 0; i < Math.floor((sayi.length - (1 + i)) / 3); i++)
                sayi = sayi.substring(0, sayi.length - (4 * i + 3)) + '.' + sayi.substring(sayi.length - (4 * i + 3));
            return (((isaret) ? '' : '-') + sayi + ',' + kurus);
        }

        var DosyaGoster = function () {
            if (txtBelgeNo.getValue() != "" && txtBelgeNo.getValue() != undefined) {
                var adres = "TasinirIslemDosya.aspx?menuYok=1";
                adres += "&yil=" + txtYil.getValue();
                adres += "&mb=" + txtMuhasebe.getValue();
                adres += "&hb=" + txtHarcamaBirimi.getValue();
                adres += "&belgeNo=" + txtBelgeNo.getValue();

                showPopWin(adres, 800, 500, true, null);
            }
            else {
                extKontrol.Msg.alert("Uyarı", "Belge No Boş Bırakılamaz.").Show();
            }
        }

        var TarihceGoster = function (yil, muhasebe, harcamaBirimi, belgeNo) {
            showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo, 500, 350, true, null);
        }

        function filter(a) {
            strModel.filterBy(filterFunction);
        }
        function filterFunction(opRecord) {
            var grid = grdListe;
            var rowIndex = grid.store.indexOf(grdListe.getSelectionModel().getSelected());
            var deger = grid.store.data.items[rowIndex].get('markaKod');
            return opRecord.data.MARKAKOD == deger;
        }

        function TransferEkraniAc() {
            var belgeTur = hdnBelgeTur.getValue();

            showPopWin("TasinirTransfer.aspx?menuYok=1", 800, 500, true, null);
        }

        var OnayaGonder = function () {
            Ext1.Msg.show({
                title: 'Onay',
                msg: "Belge B/A onayına gönderilecektir. Bu işlemi onaylıyor musunuz?<br><br><b>Açıklama:</b>",
                width: 450,
                closable: false,
                buttons: Ext1.Msg.YESNO,
                buttonText:
                {
                    yes: 'Evet',
                    no: 'Hayır'
                },
                multiline: true,
                fn: function (buttonValue, inputText, showConfig) {
                    if (buttonValue == "yes") {
                        Ext1.net.DirectMethods.OnayaGonder(inputText, { eventMask: { showMask: true } });
                    }
                },
                icon: Ext1.Msg.QUESTION
            });
        }

        var DemirbasYazilimBilgisiYaz = function (img) {
            $('.x-panel-body').css('background', 'url("../App_Themes/images/' + img + '.png") no-repeat fixed center 100px');
        }

    </script>
    <style>
        .vurgula-0 {
        }

        .vurgula-1 {
            color: red
        }
    </style>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden runat="server" ID="hdnMutemmim" />
        <ext:Hidden runat="server" ID="hdnKod" />
        <ext:Hidden runat="server" ID="hdnHesapKod" />
        <ext:Hidden runat="server" ID="hdnBelgeTur" />
        <ext:Hidden runat="server" ID="hdnFirmaHarcamadanAlma" />
        <ext:Hidden runat="server" ID="hdnKurumTur" />
        <ext:Hidden runat="server" ID="hdnEkranTip" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="siraNo">
                    <Fields>
                        <ext:RecordField Name="siraNo" />
                        <ext:RecordField Name="hesapPlanKod" />
                        <ext:RecordField Name="gorSicilNo" />
                        <ext:RecordField Name="miktar" Type="Float" />
                        <ext:RecordField Name="olcuBirimAd" />
                        <ext:RecordField Name="kdvOran" />
                        <ext:RecordField Name="birimFiyat" Type="Float" />
                        <ext:RecordField Name="hesapPlanAd" />
                        <ext:RecordField Name="disSicilNo" />
                        <ext:RecordField Name="ciltNo" />
                        <ext:RecordField Name="dil" />
                        <ext:RecordField Name="yazarAdi" />
                        <ext:RecordField Name="adi" />
                        <ext:RecordField Name="yayinYeri" />
                        <ext:RecordField Name="yayinTarihi" />
                        <ext:RecordField Name="gelisTarihi" />
                        <ext:RecordField Name="neredenGeldi" />
                        <ext:RecordField Name="neredeBulundu" />
                        <ext:RecordField Name="cagi" />
                        <ext:RecordField Name="boyutlari" />
                        <ext:RecordField Name="satirSayisi" />
                        <ext:RecordField Name="yaprakSayisi" />
                        <ext:RecordField Name="sayfaSayisi" />
                        <ext:RecordField Name="ciltTuru" />
                        <ext:RecordField Name="cesidi" />
                        <ext:RecordField Name="durumuMaddesi" />
                        <ext:RecordField Name="onYuz" />
                        <ext:RecordField Name="arkaYuz" />
                        <ext:RecordField Name="puan" />
                        <ext:RecordField Name="yeriKonusu" />
                        <ext:RecordField Name="eAciklama" />
                        <ext:RecordField Name="eSicilNo" />
                        <ext:RecordField Name="eAlimTarihi" Type="Date" />
                        <ext:RecordField Name="eTedarikSekli" />
                        <ext:RecordField Name="garantiBitisTarihi" Type="Date" />
                        <ext:RecordField Name="giai" />
                        <ext:RecordField Name="rfidEtiketTurKod" Type="Int" />
                        <ext:RecordField Name="markaKod" Type="Int" />
                        <ext:RecordField Name="modelKod" Type="Int" />
                        <ext:RecordField Name="yerleskeYeri" />
                        <ext:RecordField Name="yerleskeYeriAd" />
                        <ext:RecordField Name="satisBedeli" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strHesapPlan" runat="server" AutoLoad="false" OnRefreshData="HesapStore_Refresh">
            <Proxy>
                <ext:PageProxy />
            </Proxy>
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="hesapPlanKod" />
                        <ext:RecordField Name="hesapPlanAd" />
                        <ext:RecordField Name="olcuBirimAd" />
                        <ext:RecordField Name="kdvOran" />
                        <ext:RecordField Name="rfidEtiketKod" />
                        <ext:RecordField Name="markaKod" />
                        <ext:RecordField Name="modelKod" />
                        <ext:RecordField Name="vurgula" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strMarka" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strModel" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="MARKAKOD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strRFIDEtiket" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Layout="ColumnLayout"
                    Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true"
                    Header="false" LabelWidth="150" Height="227">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>"
                                    Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click" Before="return isValid()">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="json" Value="Ext1.encode(grdListe.getRowsValues())" Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMTIG114 %>"
                                    Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG112%>"
                                    Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" Before="return YazdirKontrol();">
                                            <ExtraParams>
                                                <ext:Parameter Name="islem" Value="belge" Mode="Value" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnIslemler" runat="server" Text="İşlemler" Icon="TableGear">
                                    <Menu>
                                        <ext:Menu runat="server">
                                            <Items>
                                                <ext:MenuItem ID="btnKomisyon" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG115 %>"
                                                    Icon="PageMagnify" OnClientClick="KomisyonGoster();">
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnOzellik" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG116 %>"
                                                    Icon="ApplicationFormMagnify" OnClientClick="BelgeOzellikGoster();">
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnSicilYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG118%>"
                                                    Icon="PrinterColor">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnYazdir_Click" Before="return YazdirKontrol();">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="islem" Value="TIFSicil" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnBarkodYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG117%>"
                                                    Icon="PrinterMono" OnClientClick="BarkodEkraniAc();" />
                                                <ext:MenuItem ID="btnIhracYazdir" runat="server" Text="İhraç Yazdır" Icon="Printer" Hidden="true">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnIhracYazdir_Click">
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:Button>
                                <ext:Button ID="btnDosyaIslemleri" runat="server" Text="Dosya İşlemleri" Icon="DiskDownload">
                                    <Listeners>
                                        <Click Handler="DosyaGoster();"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnTransfer" runat="server" Text="Transfer" Icon="PackageGo" Hidden="true">
                                    <Listeners>
                                        <Click Handler="TransferEkraniAc();"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMZFS046%>"
                                    Icon="Tick">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayla_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS047%>"
                                    Icon="Cross" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayKaldir_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator runat="server" />
                                <ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayaGonder_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBelgeIptal" runat="server" Text="<%$Resources:TasinirMal,FRMZFS048%>"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnIptal_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge iptal edilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:Container runat="server" Layout="Form" ColumnWidth=".5">
                            <Defaults>
                                <ext:Parameter Name="LabelStyle" Value="color:#B40404;" Mode="Value" />
                            </Defaults>
                            <Items>
                                <ext:CompositeField runat="server" FieldLabel="Yıl, Belge Tarihi, Belge No">
                                    <Items>
                                        <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>"
                                            Width="60" />
                                        <ext:DateField ID="txtBelgeTarih" runat="server" EmptyText="Belge Tarihi" Width="100" />
                                        <ext:TriggerField ID="txtBelgeNo" runat="server" Width="100" EmptyText="Belge No">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <DirectEvents>
                                                <TriggerClick OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </TriggerClick>
                                                <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TriggerField>
                                        <ext:DisplayField ID="lblFormDurum" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG031%>"
                                            Width="140">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblMuhasebeAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG033%>"
                                            Width="140">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG035 %>"
                                            Width="140">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblAmbarAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="ddlIslemTipi" runat="server" FieldLabel="İşlem Tipi" Editable="true"
                                    Mode="Local" TriggerAction="All" ForceSelection="true" AllowBlank="false" SelectOnFocus="true"
                                    DisplayField="AD" ValueField="KODTUR" Width="160">
                                    <Store>
                                        <ext:Store ID="strIslemTipi" runat="server">
                                            <Reader>
                                                <ext:JsonReader IDProperty="KOD">
                                                    <Fields>
                                                        <ext:RecordField Name="AD" />
                                                        <ext:RecordField Name="KOD" />
                                                        <ext:RecordField Name="TUR" />
                                                        <ext:RecordField Name="KODTUR" />
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="AD" Direction="ASC" />
                                        </ext:Store>
                                    </Store>
                                    <DirectEvents>
                                        <Select OnEvent="ddlIslemTipiSelect">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Select>
                                    </DirectEvents>
                                </ext:ComboBox>
                                <ext:CompositeField runat="server" ID="pnlMutemmim" Hidden="true" FieldLabel="Mütemmim">
                                    <Items>
                                        <ext:Label ID="lblMutemmim" Text="Açık" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <%-- <ext:ComboBox ID="ddlMutemmim" runat="server" FieldLabel="Mütemmim" Editable="false"
                                    SelectOnFocus="true" Width="70" Hidden="true">
                                    <Items>
                                        <ext:ListItem Text="Hayır" Value="0" />
                                        <ext:ListItem Text="Evet" Value="1" />
                                    </Items>
                                    <SelectedItem Text="Hayır" Value="0" />
                                    <DirectEvents>
                                        <Select OnEvent="ddlMutemmimSelect">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Select>
                                    </DirectEvents>
                                </ext:ComboBox>--%>
                                <ext:ComboBox ID="ddlDoviz" runat="server" FieldLabel="Para Birimi" Editable="false"
                                    SelectOnFocus="true" Width="100" />
                                <ext:CompositeField ID="fcDayanak" runat="server" Layout="HBoxLayout" FieldLabel="Dayanak Belgenin"
                                    AnchorHorizontal="90%" LabelStyle="color:#686868;">
                                    <Items>
                                        <ext:DateField ID="txtDayanakTarih" runat="server" EmptyText="Belge Tarihi" Width="100" />
                                        <ext:TextField ID="txtDayanakNo" runat="server" EmptyText="Numarası" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                            </Items>
                        </ext:Container>
                        <ext:Container runat="server" Layout="Form" ColumnWidth=".5">
                            <Items>
                                <ext:CompositeField ID="cfNeredenGeldi" runat="server" FieldLabel="Nereden Geldiği"
                                    AnchorHorizontal="90%" LabelStyle="color:#B40404;">
                                    <Items>
                                        <ext:TriggerField ID="txtNeredenGeldi" runat="server" Width="205">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblNeredenGeldi" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="fcFatura" runat="server" Layout="HBoxLayout" FieldLabel="Fatura"
                                    AnchorHorizontal="90%">
                                    <Items>
                                        <ext:DateField ID="txtFaturaTarih" runat="server" EmptyText="Fatura Tarihi" Width="100" />
                                        <ext:TextField ID="txtFaturaNo" runat="server" EmptyText="Numarası" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="fcMuayene" runat="server" Layout="HBoxLayout" FieldLabel="Muayene Kont. Komisyon"
                                    AnchorHorizontal="90%">
                                    <Items>
                                        <ext:DateField ID="txtMuayeneTarih" runat="server" EmptyText="Mua. Kont. Kom. Tarihi"
                                            Width="100" />
                                        <ext:TextField ID="txtMuayeneNo" runat="server" EmptyText="Rapor Numarası" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cfNereyeGitti" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTIG105 %>"
                                    Hidden="true" AnchorHorizontal="90%">
                                    <Items>
                                        <ext:TriggerField ID="txtNereyeGitti" runat="server" EmptyText="Nereye Verildiği"
                                            Width="205">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblNereyeGitti" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cfKimeGitti" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMTIG106 %>">
                                    <Items>
                                        <ext:TriggerField ID="txtKimeGitti" runat="server" Width="205">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblKimeGittiAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:FieldSet ID="fsGonderenBirim" runat="server" Title="<%$ Resources:TasinirMal,FRMTIG108%>"
                                    Collapsible="false" Layout="form" AnchorHorizontal="90%" LabelWidth="130">
                                    <Items>
                                        <ext:Button ID="btnDevirListesi2" runat="server" Text="Devir Listesi Aç" Icon="PageWhiteMagnify"
                                            OnClientClick="DevirListesiAc();" Hidden="true" />
                                        <ext:CompositeField ID="cfGonMuhasebe" runat="server" FieldLabel="Muhasebe Birimi Kodu"
                                            Hidden="true">
                                            <Items>
                                                <ext:TriggerField ID="txtGonMuhasebe" runat="server" FieldLabel="Muhasebe Birimi Kodu"
                                                    Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Fn="TriggerChange" />
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonMuhasebeAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:CompositeField ID="cfGonHarcamaBirimi" runat="server" FieldLabel="Harcama Birimi Kodu"
                                            Hidden="true">
                                            <Items>
                                                <ext:TriggerField ID="txtGonHarcamaBirimi" runat="server" Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Fn="TriggerChange" />
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonHarcamaBirimiAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:CompositeField ID="cfGonAmbar" runat="server" FieldLabel="Taşınır Ambar Kodu"
                                            Hidden="true">
                                            <Items>
                                                <ext:TriggerField ID="txtGonAmbar" runat="server" EnableKeyEvents="true" Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Fn="TriggerChange" />
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonAmbarAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                    </Items>
                                    <Tools>
                                        <ext:Tool Handler="DevirListesiAc();" Type="Gear" Qtip="Devir Listesi Aç" />
                                    </Tools>
                                </ext:FieldSet>
                                <ext:CompositeField ID="fcGonYilBelgeNo" runat="server" Layout="HBoxLayout" FieldLabel="Gelen Belgenin Yılı, Nosu"
                                    Hidden="true">
                                    <Items>
                                        <ext:SpinnerField ID="txtGonYil" runat="server" Width="60" />
                                        <ext:TriggerField ID="txtGonBelgeNo" runat="server" Width="110">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <DirectEvents>
                                                <TriggerClick OnEvent="btnGonListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </TriggerClick>
                                                <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnGonListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TriggerField>
                                    </Items>
                                </ext:CompositeField>
                                <ext:TextArea ID="txtAciklama" runat="server" FieldLabel="Açıklama" AnchorHorizontal="90%"
                                    LabelWidth="120" Height="50" />
                            </Items>
                        </ext:Container>
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true"
                    ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:RowNumbererColumn DataIndex="siraNo" />
                            <ext:Column ColumnID="hesapPlanKod" runat="server" DataIndex="hesapPlanKod" Header="<%$ Resources:TasinirMal, FRMTIG017 %>"
                                Width="150" Hideable="false">
                                <Editor>
                                    <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="hesapPlanKod" ValueField="hesapPlanKod"
                                        TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                        HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                        StoreID="strHesapPlan">
                                        <Template runat="server">
                                            <Html>
                                                <tpl for=".">
                                                    <div class="search-item">
                                                        <span class="vurgula-{vurgula}">{hesapPlanKod} - {hesapPlanAd}</span>
                                                    </div>
                                                </tpl>
                                            </Html>
                                        </Template>
                                        <Listeners>
                                            <Select Fn="HesapPlanKodSelect" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Editor>
                                <Commands>
                                    <ext:ImageCommand Icon="Magnifier" CommandName="hesapPlanKod" />
                                </Commands>
                            </ext:Column>
                            <ext:CommandColumn ColumnID="hesapPlanKodCommand" runat="server" Width="25" Hidden="true">
                                <Commands>
                                    <ext:GridCommand Icon="Magnifier" CommandName="hesapPlanKod" />
                                </Commands>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="gorSicilNo" runat="server" DataIndex="gorSicilNo" Header="<%$ Resources:TasinirMal, FRMTIG018 %>"
                                Width="190">
                                <Commands>
                                    <ext:ImageCommand Icon="Magnifier" CommandName="gorSicilNo" />
                                </Commands>
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                            </ext:Column>
                            <ext:CommandColumn ColumnID="gorSicilNoCommand" runat="server" Width="25" Hidden="true">
                                <Commands>
                                    <ext:GridCommand Icon="Magnifier" CommandName="gorSicilNo" />
                                </Commands>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="miktar" runat="server" DataIndex="miktar" Header="<%$ Resources:TasinirMal, FRMTIG019 %>"
                                Width="80" Align="Right">
                                <Editor>
                                    <ext:NumberField ID="NumberField3" runat="server" DecimalPrecision="4" SelectOnFocus="true"
                                        AllowNegative="false" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="olcuBirimAd" runat="server" DataIndex="olcuBirimAd" Header="<%$ Resources:TasinirMal, FRMTIG020 %>"
                                Width="70">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="kdvOran" runat="server" DataIndex="kdvOran" Header="<%$ Resources:TasinirMal, FRMTIG021 %>"
                                Width="65">
                                <Renderer Handler="return Ext1.util.Format.number(value, '%00/i');" />
                                <Editor>
                                    <ext:NumberField ID="NumberField1" runat="server" SelectOnFocus="true" MaxValue="99"
                                        AllowNegative="false" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="birimFiyat" runat="server" DataIndex="birimFiyat" Header="<%$Resources:TasinirMal,FRMZFG015 %>"
                                Align="Right" Width="120" Tooltip="<%$Resources:TasinirMal,FRMZFG015 %>">
                                <Renderer Fn="BirimFiyatFormatla" />
                                <Editor>
                                    <ext:NumberField runat="server" DecimalPrecision="12" SelectOnFocus="true" AllowNegative="false" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="hesapPlanAd" runat="server" DataIndex="hesapPlanAd" Header="<%$ Resources:TasinirMal, FRMTIG023 %>"
                                Width="180" Wrap="false">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="disSicilNo" runat="server" DataIndex="disSicilNo" Header="<%$ Resources:TasinirMal, FRMTIG024 %>"
                                Width="180" Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="ciltNo" runat="server" DataIndex="ciltNo" Header="<%$ Resources:TasinirMal, FRMTIG025 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="dil" runat="server" DataIndex="dil" Header="<%$ Resources:TasinirMal, FRMTIG026 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yazarAdi" runat="server" DataIndex="yazarAdi" Header="<%$ Resources:TasinirMal, FRMTIG027 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="adi" runat="server" DataIndex="adi" Header="<%$ Resources:TasinirMal, FRMTIG028 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yayinYeri" runat="server" DataIndex="yayinYeri" Header="<%$ Resources:TasinirMal, FRMTIG029 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yayinTarihi" runat="server" DataIndex="yayinTarihi" Header="<%$ Resources:TasinirMal, FRMTIG030 %>"
                                Hidden="true">
                                <Renderer Fn="TarihRenderer" />
                                <Editor>
                                    <ext:DateField runat="server" Format="dd.m.Y" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="gelisTarihi" runat="server" DataIndex="gelisTarihi" Header="<%$ Resources:TasinirMal, FRMTIG041 %>"
                                Hidden="true">
                                <Renderer Fn="TarihRenderer" />
                                <Editor>
                                    <ext:DateField runat="server" Format="dd.m.Y" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="neredenGeldi" runat="server" DataIndex="neredenGeldi" Header="<%$ Resources:TasinirMal, FRMTIG031 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="neredeBulundu" runat="server" DataIndex="neredeBulundu" Header="<%$ Resources:TasinirMal, FRMTIG043 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="cagi" runat="server" DataIndex="cagi" Header="<%$ Resources:TasinirMal, FRMTIG044 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="boyutlari" runat="server" DataIndex="boyutlari" Header="<%$ Resources:TasinirMal, FRMTIG032 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="satirSayisi" runat="server" DataIndex="satirSayisi" Header="<%$ Resources:TasinirMal, FRMTIG033 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yaprakSayisi" runat="server" DataIndex="yaprakSayisi" Header="<%$ Resources:TasinirMal, FRMTIG034 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="sayfaSayisi" runat="server" DataIndex="sayfaSayisi" Header="<%$ Resources:TasinirMal, FRMTIG035 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="ciltTuru" runat="server" DataIndex="ciltTuru" Header="<%$ Resources:TasinirMal, FRMTIG036 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="cesidi" runat="server" DataIndex="cesidi" Header="<%$ Resources:TasinirMal, FRMTIG037 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="durumuMaddesi" runat="server" DataIndex="durumuMaddesi" Header="<%$ Resources:TasinirMal, FRMTIG046 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="onYuz" runat="server" DataIndex="onYuz" Header="<%$ Resources:TasinirMal, FRMTIG047 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="arkaYuz" runat="server" DataIndex="arkaYuz" Header="<%$ Resources:TasinirMal, FRMTIG048 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="puan" runat="server" DataIndex="puan" Header="<%$ Resources:TasinirMal, FRMTIG049 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yeriKonusu" runat="server" DataIndex="yeriKonusu" Header="<%$ Resources:TasinirMal, FRMTIG038 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="yerleskeYeriAd" runat="server" DataIndex="yerleskeYeriAd" Header="Yerleşke Yeri"
                                Width="190" Hidden="true">
                                <Commands>
                                    <ext:ImageCommand Icon="Magnifier" CommandName="yerleskeYeriAd" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="eAciklama" runat="server" DataIndex="eAciklama" Header="<%$ Resources:TasinirMal, FRMTIG051 %>">
                                <Editor>
                                    <ext:TextArea runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="eSicilNo" runat="server" DataIndex="eSicilNo" Header="<%$ Resources:TasinirMal, FRMTIG052 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="eAlimTarihi" runat="server" DataIndex="eAlimTarihi" Header="<%$ Resources:TasinirMal, FRMTIG053 %>"
                                Hidden="true">
                                <Renderer Fn="TarihRenderer" />
                                <Editor>
                                    <ext:DateField runat="server" Format="dd.m.Y" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="eTedarikSekli" runat="server" DataIndex="eTedarikSekli" Header="<%$ Resources:TasinirMal, FRMTIG054 %>"
                                Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="garantiBitisTarihi" runat="server" DataIndex="garantiBitisTarihi"
                                Header="Garanti Bitiş Tarihi" Hidden="true">
                                <Renderer Fn="TarihRenderer" />
                                <Editor>
                                    <ext:DateField runat="server" Format="dd.m.Y" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="giai" runat="server" DataIndex="giai" Header="K.Demirbaş No (GIAI)"
                                Width="120" Hidden="true">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="rfidEtiketTurKod" runat="server" DataIndex="rfidEtiketTurKod"
                                Header="RFID Etiket Türü" Hidden="true">
                                <Renderer Handler="return PropertyRenderer(strRFIDEtiket,value);" />
                                <Editor>
                                    <ext:ComboBox ID="ddlRfidEtiketTur" runat="server" DisplayField="ADI" ValueField="KOD"
                                        StoreID="strRFIDEtiket" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="markaKod" runat="server" DataIndex="markaKod" Header="Marka"
                                Hidden="true">
                                <Renderer Handler="return PropertyRenderer(strMarka,value);" />
                                <Editor>
                                    <ext:ComboBox runat="server" DisplayField="ADI" ValueField="KOD" StoreID="strMarka" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="modelKod" runat="server" DataIndex="modelKod" Header="Model"
                                Hidden="true">
                                <Renderer Handler="return PropertyRenderer(strModel,value);" />
                                <Editor>
                                    <ext:ComboBox runat="server" StoreID="strModel" ValueField="KOD" DisplayField="ADI"
                                        TypeAhead="false" ForceSelection="true" Editable="false" SelectOnFocus="true">
                                        <Listeners>
                                            <Expand Fn="filter" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Editor>
                            </ext:Column>

                            <ext:Column ColumnID="satisBedeli" runat="server" DataIndex="satisBedeli" Header="Satış Bedeli"
                                Align="Right" Width="120" Hidden="true">
                                <Editor>
                                    <ext:NumberField runat="server" DecimalPrecision="12" SelectOnFocus="true" AllowNegative="false" />
                                </Editor>
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server">
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <Listeners>
                        <Command Fn="KomutCalistir" />
                        <BeforeEdit Fn="BeforeEdit" />
                        <AfterEdit Fn="AfterEdit" />
                    </Listeners>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true" PageSize="250">
                            <Items>
                                <ext:Button ID="btnSatirAc" runat="server" Text="Satır Aç" Icon="TableRow">
                                    <Menu>
                                        <ext:Menu runat="server">
                                            <Items>
                                                <ext:MenuItem ID="MenuItem1" runat="server" Text="10" Icon="TableRow" OnClientClick="GridSatirAc(strListe,10);" />
                                                <ext:MenuItem ID="MenuItem3" runat="server" Text="20" Icon="TableRow" OnClientClick="GridSatirAc(strListe,20);" />
                                                <ext:MenuItem ID="MenuItem4" runat="server" Text="50" Icon="TableRow" OnClientClick="GridSatirAc(strListe,50);" />
                                                <ext:MenuItem ID="MenuItem5" runat="server" Text="100" Icon="TableRow" OnClientClick="GridSatirAc(strListe,100);" />
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:Button>
                                <ext:Button ID="btnSatirAcAraya" runat="server" Text="Araya Satır Aç" Icon="TableCell">
                                    <Listeners>
                                        <Click Handler="GridSatirAcAraya(grdListe);" />
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnSatirSil" runat="server" Text="Satır Sil" Icon="TableRowDelete">
                                    <Listeners>
                                        <Click Handler="#{grdListe}.deleteSelected();" />
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnToplamHesapla" runat="server" Text="Toplam Hesapla" Icon="Sum">
                                    <Listeners>
                                        <Click Fn="ToplamHesapla" />
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnHesapPlanModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                    <Listeners>
                                        <Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:hesapPlanKod:hesapPlanAd:olcuBirimAd:kdvOran:miktar:birimFiyat','','');return false;" />
                                    </Listeners>
                                </ext:Button>
                                <ext:FileUploadField ID="btnDosyaYukle" runat="server" ButtonOnly="true" Icon="PageWhiteAdd"
                                    ButtonText="Malzeme Yükle">
                                    <DirectEvents>
                                        <FileSelected OnEvent="btnDosyaYukle_Change">
                                            <EventMask Msg="Lütfen bekleyin...." ShowMask="true" />
                                        </FileSelected>
                                    </DirectEvents>
                                </ext:FileUploadField>
                                <ext:Button ID="btnDosyaSakla" runat="server" Text="Malzeme Sakla" Icon="DiskDownload">
                                    <DirectEvents>
                                        <Click OnEvent="btnDosyaSakla_Click">
                                            <ExtraParams>
                                                <ext:Parameter Name="json" Value="Ext1.encode(grdListe.getRowsValues())" Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnYerleskeSec" runat="server" Text="Yerleşke Seç" Icon="FolderHome" Hidden="true">
                                    <Listeners>
                                        <Click Handler="javascript:ListeAc('ListeOdaMB.aspx', 'grdListe:hepsi', 'txtHarcamaBirimi', '');return false;" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
