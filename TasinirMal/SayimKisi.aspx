<%@ Page Language="C#" CodeBehind="SayimKisi.aspx.cs" Inherits="TasinirMal.SayimKisi" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="javascript" type="text/javascript" src="ModulScripts/SayimGiris.js?v=1"></script>
    <script type="text/javascript">
        var wndSayimKisiDetayKapat = function () {
            Ext1.net.DirectMethods.SayimKisiListele(hdnSayimKod.getValue());
        }

        var SatirIslemiYap = function (command, record, rowIndex) {
            if (command == "SayimDetayAc") {
                var sayimKod = record.data.SAYIMKOD;
                var kisiKod = record.data.KISIKOD;
                var kisiAd = record.data.KISIAD;
                var odaKod = record.data.ODAKOD;
                var odaAd = record.data.ODAAD;
                var muhasebeKod = txtMuhasebe.getValue();
                var harcamaBirimKod = txtHarcamaBirimi.getValue();
                harcamaBirimKod = harcamaBirimKod.replace(/\./g, "")

                if (kisiKod == "") { kisiKod = odaKod; kisiAd = odaAd; }

                SayimKisiDetayPencereAc(record.data.KOD, sayimKod, kisiKod, kisiAd, muhasebeKod, harcamaBirimKod);
            }
        }

        function SayimKisiDetayPencereAc(kod, sayimKod, kisiKod, kisiAd, muhasebeKod, harcamaBirimKod) {
            showPopWin('SayimKisiDetayPencere.aspx?menuYok=1&sayimDetayKod=' + kod +'&sayimKod=' + sayimKod + '&kisiKod=' + kisiKod + '&kisiAd=' + kisiAd + '&muhasebeKod=' + muhasebeKod + '&harcamaBirimKod=' + harcamaBirimKod, 1100, 600, true, null);
        }

    </script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Hidden ID="hdnAktifSayimKod" runat="server" />
        <ext:Hidden ID="hdnSayimKod" runat="server" />
        <ext:Store ID="storeSayim" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="BASLAMATARIH" Type="Date" />
                        <ext:RecordField Name="BITISTARIH" Type="Date" />
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="ISLEMYAPANAD" />
                        <ext:RecordField Name="ISLEMYAPANKOD" />
                        <ext:RecordField Name="ISLEMTARIHI" />
                        <ext:RecordField Name="DURUM" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="storeSayimKisi" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="SAYIMKOD" />
                        <ext:RecordField Name="KISIKOD" />
                        <ext:RecordField Name="KISIAD" />
                        <ext:RecordField Name="ODAKOD" />
                        <ext:RecordField Name="ODAAD" />
                        <ext:RecordField Name="ISLEMYAPANAD" />
                        <ext:RecordField Name="ISLEMYAPANKOD" />
                        <ext:RecordField Name="ISLEMTARIHI" Type="Date" />
                        <ext:RecordField Name="DURUM" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="storeKisi" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="storeOda" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
            <Items>
                <ext:BorderLayout ID="BorderLayout2" runat="server">
                    <Center>
                        <ext:Panel ID="tabPanelAna" runat="server" StyleSpec="background-color:white;padding:10px"
                            Margins="104 20 10 20" Border="false">
                            <Items>
                                <ext:BorderLayout ID="BorderLayout1" runat="server">
                                    <West Split="true" UseSplitTips="true" CollapsibleSplitTip="Kapatmak için çift tıklayın"
                                        ExpandableSplitTip="Açmak için çift tıklayın">
                                        <ext:GridPanel ID="grdSayim" runat="server" Border="true" StoreID="storeSayim" Width="620">
                                            <TopBar>
                                                <ext:Toolbar ID="Toolbar1" runat="server">
                                                    <Items>
                                                        <ext:Button ID="btnBirimSec" runat="server" Text="Muhasebe Birimi Seç" Icon="House">
                                                            <Listeners>
                                                                <Click Handler="wndBirim.show();" />
                                                            </Listeners>
                                                        </ext:Button>
                                                        <ext:Button ID="btnSayimBaslat" runat="server" Text="Sayım Başlat" Icon="PlayGreen">
                                                            <Listeners>
                                                                <Click Handler="wndSayimBaslat.show();" />
                                                            </Listeners>
                                                        </ext:Button>
                                                        <ext:Button ID="btnSayimBitir" runat="server" Text="Sayım Bitir" Icon="StopRed">
                                                            <Listeners>
                                                                <Click Handler="wndSayimBitir.show();" />
                                                            </Listeners>
                                                        </ext:Button>
                                                        <ext:ToolbarFill runat="server" />
                                                        <ext:DisplayField ID="lblSayimBirimAdi" runat="server">
                                                        </ext:DisplayField>
                                                    </Items>
                                                </ext:Toolbar>
                                            </TopBar>
                                            <ColumnModel>
                                                <Columns>
                                                    <%-- <ext:CommandColumn ID="ImageCommandColumn2" runat="server" Width="25">
                                                    <Commands>
                                                        <ext:GridCommand CommandName="Duzenle" Icon="ApplicationEdit">
                                                            <ToolTip Text="Düzenle" />
                                                        </ext:GridCommand>
                                                    </Commands>
                                                    <listeners>
                                        <Command Fn="GridIslem" />
                                                    </listeners>
                                                </ext:CommandColumn>--%>
                                                    <ext:Column Header="Durum" DataIndex="DURUM" Width="100" />
                                                    <ext:DateColumn Header="Başlama Tarihi" DataIndex="BASLAMATARIH" Width="90" Format="dd.m.Y" />
                                                    <ext:DateColumn Header="Bitiş Tarihi" DataIndex="BITISTARIH" Width="90" Format="dd.m.Y" />
                                                    <ext:Column Header="Adı" DataIndex="ADI" Width="200" />
                                                    <ext:Column Header="İşlem Yapan Adı" DataIndex="ISLEMYAPANAD" />
                                                </Columns>
                                            </ColumnModel>
                                            <SelectionModel>
                                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                                    <DirectEvents>
                                                        <RowSelect OnEvent="SayimSecildi">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="Kod" Value="this.getSelected().data.KOD" Mode="Raw" />
                                                            </ExtraParams>
                                                        </RowSelect>
                                                    </DirectEvents>
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                        </ext:GridPanel>
                                    </West>
                                    <Center>
                                        <ext:GridPanel ID="GridPanel1" runat="server" Border="true" StoreID="storeSayimKisi"
                                            Width="620">
                                            <TopBar>
                                                <ext:Toolbar runat="server">
                                                    <Items>
                                                        <ext:Button ID="btnYeniKisi" runat="server" Text="Yeni Kişi Say" Icon="UserAdd" Hidden="true">
                                                            <Listeners>
                                                                <Click Handler="wndYeniKisiSay.show();" />
                                                            </Listeners>
                                                        </ext:Button>
                                                        <ext:Button ID="btnRapor" runat="server" Text="Rapor Hazırla" Icon="PageExcel">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnRapor_Click" IsUpload="true">
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:Button>
                                                        <ext:Button ID="btnKisiRapor" runat="server" Text="Sayılmayan Kişiler Raporu" Icon="PageExcel">
                                                            <DirectEvents>
                                                                <Click OnEvent="btnKisiRapor_Click" IsUpload="true">
                                                                </Click>
                                                            </DirectEvents>
                                                        </ext:Button>
                                                        <ext:ToolbarFill runat="server" />
                                                        <ext:Button runat="server">
                                                        </ext:Button>
                                                        <ext:Label ID="lblSayimAdi" runat="server" Icon="Package">
                                                        </ext:Label>
                                                    </Items>
                                                </ext:Toolbar>
                                            </TopBar>
                                            <ColumnModel>
                                                <Columns>
                                                    <ext:RowNumbererColumn Width="20" />
                                                    <ext:CommandColumn Width="30" Align="Center">
                                                        <Commands>
                                                            <ext:GridCommand Icon="Pencil" CommandName="SayimDetayAc" ToolTip-Text="Sayım detayı">
                                                            </ext:GridCommand>
                                                        </Commands>
                                                    </ext:CommandColumn>
                                                    <ext:Column Header="Kişi Adı" DataIndex="KISIAD" Width="150" />
                                                    <ext:Column Header="Oda Adı" DataIndex="ODAAD" Width="110" />
                                                    <ext:DateColumn Header="İşlem Tarihi" DataIndex="ISLEMTARIHI" Width="90" Format="dd.m.Y" />
                                                    <ext:Column Header="İşlem Yapan Adı" DataIndex="ISLEMYAPANAD" />
                                                    <ext:Column Header="Durum" DataIndex="DURUM" Width="80" />
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Fn="SatirIslemiYap" />
                                            </Listeners>
                                            <SelectionModel>
                                                <ext:RowSelectionModel ID="RowSelectionModel2" runat="server">
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                        </ext:GridPanel>
                                    </Center>
                                </ext:BorderLayout>
                            </Items>
                        </ext:Panel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
        <ext:Window ID="wndSayimBaslat" runat="server" Title="Sayım Başlat" Width="500" Height="150"
            Modal="true" Hidden="true" Padding="5">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnBaslat" runat="server" Text="Sayım Başlat" Icon="PlayBlue">
                            <DirectEvents>
                                <Click OnEvent="btnBaslat_Click" Timeout="50000">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Yeni sayım başlatılacak. Onaylıyor musunuz?" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:TextField ID="txtSayimAdi" runat="server" FieldLabel="Sayım Adı" Width="450" />
            </Items>
        </ext:Window>
        <ext:Window ID="wndSayimBitir" runat="server" Title="Sayım Bitir" Width="500" Height="150"
            Modal="true" Hidden="true" Padding="5">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnBitir" runat="server" Text="Sayım Bitir" Icon="StopRed">
                            <DirectEvents>
                                <Click OnEvent="btnBitir_Click" Timeout="50000">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Sayım sonladırılacak. Onaylıyor musunuz?" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:DateField ID="txtBitisTarih" runat="server" FieldLabel="Tarih" Width="200" />
            </Items>
        </ext:Window>
        <ext:Window ID="wndYeniKisiSay" runat="server" Title="Yeni Kişi Say" Width="500"
            Height="150" Modal="true" Hidden="true" Padding="5">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnYeniKisiSayimBaslat" runat="server" Text="Kişi Sayım" Icon="Accept">
                            <DirectEvents>
                                <Click OnEvent="btnYeniKisiSayimBaslat_Click" Timeout="50000">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kişi sayımı başlatılacak. Onaylıyor musunuz?" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:ComboBox ID="ddlKisi" runat="server" FieldLabel="Kişi Adı" ValueField="KOD"
                    StoreID="storeKisi" DisplayField="AD" EmptyText="Kişi Seçiniz" Width="400" Mode="Local"
                    ForceSelection="true" TriggerAction="All">
                    <Triggers>
                        <ext:FieldTrigger Icon="Clear" />
                    </Triggers>
                    <Listeners>
                        <TriggerClick Fn="TriggerClick" />
                    </Listeners>
                </ext:ComboBox>
                <ext:ComboBox ID="ddlOda" runat="server" FieldLabel="Oda Adı" ValueField="KOD" StoreID="storeOda"
                    DisplayField="AD" EmptyText="Oda Seçiniz" Width="400" Mode="Local" ForceSelection="true"
                    TriggerAction="All">
                    <Triggers>
                        <ext:FieldTrigger Icon="Clear" />
                    </Triggers>
                    <Listeners>
                        <TriggerClick Fn="TriggerClick" />
                    </Listeners>

                </ext:ComboBox>
            </Items>
        </ext:Window>
        <ext:Window ID="wndBirim" runat="server" Title="Muhasebe Birim Seçimi" Width="500"
            Height="150" Modal="true" Hidden="true" Padding="5" Closable="false">
            <TopBar>
                <ext:Toolbar ID="Toolbar3" runat="server">
                    <Items>
                        <ext:Button ID="btnMuhasebeSecim" runat="server" Text="Seç" Icon="Accept">
                            <DirectEvents>
                                <Click OnEvent="btnMuhasebeSecim_Click" Timeout="50000">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:CompositeField ID="cf1" runat="server">
                    <Items>
                        <ext:TriggerField ID="txtMuhasebe" runat="server" Width="120" MaxLength="5" FieldLabel="Muhasebe">
                            <Triggers>
                                <ext:FieldTrigger Icon="Search" />
                            </Triggers>
                            <Listeners>
                                <TriggerClick Fn="TriggerClick" />
                                <Change Fn="TriggerChange" />
                            </Listeners>
                        </ext:TriggerField>
                        <ext:Label ID="lblMuhasebeAd" runat="server">
                        </ext:Label>
                    </Items>
                </ext:CompositeField>
                <ext:CompositeField ID="CompositeField1" runat="server">
                    <Items>
                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" MaxLength="15" Width="120"
                            FieldLabel="Harcama Birimi">
                            <Triggers>
                                <ext:FieldTrigger Icon="Search" />
                            </Triggers>
                            <Listeners>
                                <TriggerClick Fn="TriggerClick" />
                                <Change Fn="TriggerChange" />
                            </Listeners>
                        </ext:TriggerField>
                        <ext:Label ID="lblHarcamaBirimiAd" runat="server">
                        </ext:Label>
                    </Items>
                </ext:CompositeField>
            </Items>
        </ext:Window>
    </form>
</body>
</html>
