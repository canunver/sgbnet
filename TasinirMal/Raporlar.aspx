<%@ Page Language="C#" CodeBehind="Raporlar.aspx.cs" Inherits="TasinirMal.Raporlar" %>


<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="JavaScript" type="text/javascript">
        function IlceDoldur() {
            var ilKod = App.ddlIl.getValue();
            App.direct.IlceDoldur(ilKod);
        }

        function IlkRaporuSec(rapor) {
            grdListe.getSelectionModel().selectRow(strListe.find("KOD", rapor));
        }

        var hesapPlaniCokluSecimAc = function () {
            var adres = "ListeHesapPlani.aspx?menuYok=1&cagiran=" + 'txtHesapPlanKod2' + "&cagiranLabel=" + 'lblHesapPlanAd2' + "&cokluSecim=1";
            HesapPlaniPenceresiAc(adres, 480, 400, ($(window).width() - 505), 30, null, "Taşınır Hesap Planı")
        }

        var NodeSecildiYerelCoklu = function (secili) {
            txtHesapPlanKod2.setValue(secili);
            if (secili.indexOf('-') == -1)
                KodAdGetir('HESAPPLANI', 'txtHesapPlanKod2', 'lblHesapPlanAd2');
            else
                lblHesapPlanAd2.setText('');
        }

        function RaporListesiAcKapa() {

            if (document.getElementById("chk2Duzey").checked)
                rdbRaporListesi.hide();
            else
                rdbRaporListesi.show();
        }

        var basSure = null;
        var sayacBaslat = function () {
            basSure = new Date().getTime() + 2 * 60 * 60 * 1000;
            raporIcinGecenSure.setText("");
            raporIcinGecenSure.show();
        }

        var sayacGuncelle = function () {
            bitSure = new Date().getTime();
            raporIcinGecenSure.setText(new Date(bitSure - basSure).dateFormat('H:i:s'));
        }


    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Hidden ID="hdnFirmaHarcamadanAlma" runat="server"></ext:Hidden>
        <ext:ResourceManager ID="ResourceManager2" runat="server">
        </ext:ResourceManager>
        <ext:Hidden runat="server" ID="hdnRaporKod" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                        <ext:RecordField Name="DOSYAAD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIslemTipi" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIl" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIlce" runat="server" OnRefreshData="IlceDoldur">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strDonem" runat="server" OnRefreshData="IlceDoldur">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strGMTur" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <Center MarginsSummary="104 20 10 20">
                        <ext:Panel ID="tabPanelAna" runat="server" StyleSpec="background-color:white;padding:10px">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                            <Listeners>
                                                <Click Handler="sayacBaslat();" />
                                            </Listeners>
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" IsUpload="true" Timeout="9600000">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="RAPORBILGI" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Label ID="raporIcinGecenSure" runat="server" Icon="TimeRed" Hidden="true" />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:BorderLayout ID="BorderLayout2" runat="server">
                                    <Center>
                                        <ext:FormPanel ID="frmPanel" runat="server" Frame="true" AutoScroll="true" Padding="5"
                                            Title="Rapor Kriter Alanları" LabelWidth="140">
                                            <Items>
                                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" Hidden="true" />
                                                <ext:ComboBox ID="ddlAy" runat="server" Hidden="true" FieldLabel="Ay" Width="80">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                    <Items>
                                                        <ext:ListItem Value="1" Text="1" />
                                                        <ext:ListItem Value="2" Text="2" />
                                                        <ext:ListItem Value="3" Text="3" />
                                                        <ext:ListItem Value="4" Text="4" />
                                                        <ext:ListItem Value="5" Text="5" />
                                                        <ext:ListItem Value="6" Text="6" />
                                                        <ext:ListItem Value="7" Text="7" />
                                                        <ext:ListItem Value="8" Text="8" />
                                                        <ext:ListItem Value="9" Text="9" />
                                                        <ext:ListItem Value="10" Text="10" />
                                                        <ext:ListItem Value="11" Text="11" />
                                                        <ext:ListItem Value="12" Text="12" />
                                                    </Items>
                                                </ext:ComboBox>
                                                <ext:CompositeField ID="cmpMuhasebe" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG025 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblMuhasebeAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpHarcamaBirimi" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG027 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpAmbar" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTIM031 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblAmbarAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:ComboBox ID="ddlIl" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMH009%>" Width="120" StoreID="strIl" ValueField="KOD" DisplayField="ADI" QueryMode="Local">
                                                    <Listeners>
                                                        <Select Handler="#{strIlce}.reload(); #{ddlIlce}.setValue(null);" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="ddlIlce" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMH010%>" Width="120" StoreID="strIlce" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                                                <ext:ComboBox ID="ddlDonem" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMECK022%>" Width="120" StoreID="strDonem" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                                                <ext:CompositeField ID="cmpKimeVerildi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMZFG045 %>">
                                                    <Items>
                                                        <ext:TriggerField ID="txtKimeVerildi" runat="server" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblKimeVerildi" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpNereyeVerildi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTSR016 %>">
                                                    <Items>
                                                        <ext:TriggerField ID="txtNereyeVerildi" runat="server" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblNereyeVerildi" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpHesapPlan" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtHesapPlanKod" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV062 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblHesapPlanAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:TextField ID="txtBelgeNo" runat="server" Width="100" EmptyText="Belge No" FieldLabel="Belge No" Hidden="true" />
                                                <ext:CompositeField ID="cmpHesapPlan2" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtHesapPlanKod2" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV062 %>" Width="160">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="hesapPlaniCokluSecimAc" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblHesapPlanAd2" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:ComboBox ID="ddlHesapPlanKodIslemDurumu" runat="server" Hidden="true" FieldLabel="Taşınır Hesap Kodu İşlem" Width="160">
                                                    <Items>
                                                        <ext:ListItem Value="1" Text="Sadece Seçilenleri Getir" />
                                                        <ext:ListItem Value="2" Text="Seçilenleri Dahil Etme" />
                                                    </Items>
                                                    <SelectedItem Value="1" />
                                                </ext:ComboBox>

                                                <ext:RadioGroup ID="rdGrup" runat="server" ColumnsNumber="3" Hidden="true">
                                                    <Items>
                                                        <ext:Radio ID="rdMuhasebeBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD020 %>" />
                                                        <ext:Radio ID="rdHarcamaBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD021 %>" />
                                                        <ext:Radio ID="rdAmbarBazinda" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMTDD022 %>" />
                                                        <ext:Radio ID="rdIlBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD023 %>" />
                                                        <ext:Radio ID="rdKurumBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD024 %>" />
                                                    </Items>
                                                </ext:RadioGroup>
                                                <ext:Checkbox ID="chkYilIci" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD025 %>" Hidden="true" />
                                                <ext:Checkbox ID="chkYilDevri" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD026 %>" Checked="true" Hidden="true" />
                                                <ext:Checkbox ID="chkMevcut" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD027 %>" Hidden="true" />
                                                <ext:ComboBox ID="ddlIslemTipi" runat="server" Editable="false" FieldLabel="<%$Resources:TasinirMal,FRMTMT011 %>" StoreID="strIslemTipi" ValueField="KOD" DisplayField="ADI" SelectOnFocus="true" Width="120" Hidden="true" />
                                                <ext:ComboBox ID="ddlGirisCikis" runat="server" Width="120" Editable="false" FieldLabel="Giriş/Çıkış" Hidden="true">
                                                    <Items>
                                                        <ext:ListItem Text="Hepsi" Value="0" />
                                                        <ext:ListItem Text="Giriş" Value="-1" />
                                                        <ext:ListItem Text="Çıkış" Value="-2" />
                                                    </Items>
                                                    <SelectedItem Value="0" />
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="ddlOnayDurum" runat="server" Width="120" Editable="false" FieldLabel="Onay Durum" Hidden="true">
                                                    <Items>
                                                        <ext:ListItem Text="Hepsi" Value="0" />
                                                        <ext:ListItem Text="Onaysız" Value="1" />
                                                        <ext:ListItem Text="Onaylı" Value="5" />
                                                        <ext:ListItem Text="İptal" Value="9" />
                                                    </Items>
                                                    <SelectedItem Value="0" />
                                                </ext:ComboBox>
                                                <ext:TextField ID="txtBirimFiyat" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTSR020 %>" Width="120" />
                                                <ext:CompositeField ID="cmpTarih" runat="server" FieldLabel="Tarih" Hidden="true">
                                                    <Items>
                                                        <ext:DateField ID="txtBaslangicTarih" runat="server" Width="100" Note="Başlangıç">
                                                        </ext:DateField>
                                                        <ext:DateField ID="txtBitisTarih" runat="server" Width="100" Note="Bitiş">
                                                        </ext:DateField>
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:Checkbox ID="chkDetay" runat="server" BoxLabel="<%$Resources:TasinirMal,FRMTMC021 %>" Hidden="true" />
                                                <ext:Checkbox ID="chk2Duzey" runat="server" BoxLabel="<%$Resources:TasinirMal,FRMTHU014 %>" Hidden="true">
                                                    <Listeners>
                                                        <Check Handler="RaporListesiAcKapa();" />
                                                    </Listeners>
                                                </ext:Checkbox>
                                                <ext:CompositeField ID="cmpGonMuhasebe" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtGonMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT017 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblGonMuhasebeAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpGonHarcamaBirimi" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtGonHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT018 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblGonHarcamaBirimiAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpGonAmbar" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtGonAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT019 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />

                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblGonAmbarAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:CompositeField ID="cmpKimeGitti" runat="server" Hidden="true">
                                                    <Items>
                                                        <ext:TriggerField ID="txtKimeGitti" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT020 %>" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblKimeGittiAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:TriggerField ID="txtNeredenGeldi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT022 %>" Width="120">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>

                                                <ext:TriggerField ID="txtNereyeGitti" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMT024  %>" Width="120" Hidden="true">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>

                                                <ext:Checkbox ID="chkKisiDahilEt" runat="server" BoxLabel="<%$Resources:TasinirMal,FRMZOA011 %>" Hidden="true" />
                                                <ext:ComboBox ID="ddlGayrimenkulTuru" runat="server" Editable="true" TypeAhead="true" Mode="Local"
                                                    ForceSelection="true" FieldLabel="Gayrimenkül Türü" Width="120" StoreID="strGMTur" DisplayField="AD"
                                                    ValueField="KOD" SelectOnFocus="true">
                                                </ext:ComboBox>
                                                <ext:RadioGroup ID="rblKHKDurumu" runat="server" ColumnsNumber="3" Hidden="true" FieldLabel="KHK Durumu">
                                                    <Items>
                                                        <ext:Radio ID="rdKHKDahil" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMTDD029 %>" />
                                                        <ext:Radio ID="rdKHKSiz" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD030 %>" />
                                                        <ext:Radio ID="rdSadeceKHK" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTDD031 %>" />
                                                    </Items>
                                                </ext:RadioGroup>
                                                <ext:RadioGroup ID="rdbRaporListesi" runat="server" ColumnsNumber="3" Hidden="true" FieldLabel="Rapor Listesi">
                                                    <Items>
                                                        <ext:Radio ID="rHGenel" runat="server" Checked="true" BoxLabel="Taşınır Hurda Raporu (Genel)" />
                                                        <ext:Radio ID="rHKodlu" runat="server" BoxLabel="Taşınır Koda Göre Gruplanmış" />
                                                        <ext:Radio ID="rHBirim" runat="server" BoxLabel="Birimler İtibari İle" />
                                                        <ext:Radio ID="rHBakanlik" runat="server" BoxLabel="Bakanlık Düzeyi" />
                                                        <ext:Radio ID="rHIller" runat="server" BoxLabel="İller İtibari İle" />
                                                    </Items>
                                                </ext:RadioGroup>
                                                <ext:TextField ID="txtTCVergiNo" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTOE042 %>" Width="120" Hidden="true" />
                                                <ext:Checkbox ID="chkBinekOtoveSanatEseri" runat="server" FieldLabel="Binek Oto ve Sanat Eserleri Hariç Mi?" Hidden="true" />
                                                <ext:TextField ID="txtKisiSayisi" runat="server" FieldLabel="Kişi Sayısı" Width="300" Hidden="true" Note="Harcama birimindeki yıl bazlı kişi sayılarını ',', ';' karakterler ile ayırarak girmelisiniz. Bulunduğunuz yıldan itibaren 3 yıl geriye dönük yıllara ait kişi sayılarını girmelisiniz. Örnek: (133,122,144,155)" NoteAlign="Down" />
                                                <ext:ComboBox ID="ddlCiktiTur" runat="server" Width="100" Editable="false" FieldLabel="Çıktı Türü"
                                                    AllowBlank="false" Hidden="true">
                                                    <Items>
                                                        <ext:ListItem Text="Excel" Value="1" />
                                                        <ext:ListItem Text="Pdf" Value="2" />
                                                    </Items>
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="cmbSiralama" runat="server" Width="100" Editable="false" FieldLabel="Siralama"
                                                    AllowBlank="true" Hidden="true">
                                                    <Items>
                                                        <ext:ListItem Text="Hesap Planı Kodu" Value="HESAPPLANKOD" />
                                                        <ext:ListItem Text="Sıra No" Value="SIRANO" />
                                                    </Items>
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="ddlIhracTur2" runat="server" Width="160" Editable="false" FieldLabel="İhraç Şekli"
                                                    AllowBlank="false" Hidden="true" SelectedIndex="0">
                                                    <Items>
                                                        <ext:ListItem Text="İhraç Edilmiş" Value="1" />
                                                        <ext:ListItem Text="İhraç Edilecek" Value="2" />
                                                        <ext:ListItem Text="İhraç Edilmiş (Geçici)" Value="3" />
                                                        <ext:ListItem Text="İhraç Edilecek (Geçici)" Value="4" />
                                                    </Items>
                                                    <Listeners>
                                                        <Select Handler="if(this.value=='3'){ddlIhracEdilmislerDurum.show();} else {ddlIhracEdilmislerDurum.hide();} if(this.value=='1'){chkIhracSatislariDahilEt.show();} else {chkIhracSatislariDahilEt.hide();}" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="ddlIhracEdilmislerDurum" runat="server" Width="160" Editable="false" FieldLabel="İhraç Edilmisler"
                                                    AllowBlank="false" Hidden="true" SelectedIndex="1">
                                                    <Items>
                                                        <ext:ListItem Text="Göster" Value="A" />
                                                        <ext:ListItem Text="Gösterme" Value="B" />
                                                    </Items>
                                                </ext:ComboBox>
                                                <ext:Checkbox ID="chkIhracSatislariDahilEt" runat="server" FieldLabel="Satış Yapılanları Dahil Et" Checked="true" Hidden="true" />
                                            </Items>
                                        </ext:FormPanel>
                                    </Center>
                                    <West Split="true">
                                        <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" TrackMouseOver="true"
                                            StoreID="strListe" Border="true" AutoExpandColumn="AD" Layout="FitLayout" Width="300"
                                            Title="Raporlar" HideHeaders="true">
                                            <ColumnModel ID="ColumnModel1" runat="server">
                                                <Columns>
                                                    <ext:RowNumbererColumn />
                                                    <ext:Column DataIndex="AD" MenuDisabled="true" Sortable="false" />
                                                </Columns>
                                            </ColumnModel>
                                            <SelectionModel>
                                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" SingleSelect="true">
                                                    <DirectEvents>
                                                        <RowSelect OnEvent="SatirSecildi" Buffer="100">
                                                            <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="GRIDPARAM" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                            </ExtraParams>
                                                        </RowSelect>
                                                    </DirectEvents>
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                        </ext:GridPanel>
                                    </West>
                                </ext:BorderLayout>
                            </Items>
                        </ext:Panel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
        <ext:TaskManager ID="TaskManager1" runat="server">
            <Tasks>
                <ext:Task>
                    <Listeners>
                        <Update Handler="sayacGuncelle();" />
                    </Listeners>
                </ext:Task>
            </Tasks>
        </ext:TaskManager>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
