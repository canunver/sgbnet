<%@ Page Language="C#" Inherits="TasinirMal.HesapPlani" CodeBehind="TanimHesapPlani.aspx.cs" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        var DosyaYukleAc = function () {
            if (txtKod.getValue() != "") {
                Ext1.Msg.confirm('Onay', 'Dosyayı Yüklemek İstiyor musunuz?', function (btn, text) {
                    if (btn == 'yes') {
                        winDosyaYukle.iframe.dom.contentWindow.document.getElementById("fuDosya-file").click();
                    }
                });
            }
            else
                Ext1.Msg.show({ title: "Uyarı", msg: "Lütfen öncelikle Hesap Konudu giriniz.", icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
        }

        var DosyaYukle = function (dosyaAd, tmpDosya) {
            Ext1.net.DirectMethods.DosyaYukle(dosyaAd, tmpDosya, { eventMask: { showMask: true } });
        }

        function KaydetKontrollu() {
            Ext1.net.DirectMethods.KaydetKontrollu({ eventMask: { showMask: true }, timeout: 50000 });
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Hidden ID="hdnSeciliKod" runat="server" />
        <ext:Hidden ID="hdnSecKapat" runat="server" />
        <ext:Hidden ID="hdnZorla" runat="server" />

        <ext:Store ID="strOlcubirim" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strMarka" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strModel" runat="server" AutoLoad="false" OnRefreshData="ModelDoldur">
            <DirectEventConfig>
                <EventMask ShowMask="false" />
            </DirectEventConfig>
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <Listeners>
                <Load Handler="if(ddlModel.getValue() != ''){#{ddlModel}.setValue(ddlModel.getValue());} else {#{ddlModel}.setValue(#{ddlModel}.store.getAt(0).get('KOD'));}" />
            </Listeners>
        </ext:Store>
        <ext:Store ID="strRFIDEtiket" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" Header="false" TrackMouseOver="true"
                            Border="true" Region="West" Width="400" Split="true" AutoExpandColumn="ACIKLAMA"
                            Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMORT003 %>"
                                            LabelWidth="40" Width="250" EnableKeyEvents="true">
                                            <Listeners>
                                                <KeyUp Handler="ApplyFilter('ACIKLAMA');" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="SimpleCross" />
                                            </Triggers>
                                        </ext:TriggerField>
                                        <ext:ToolbarFill />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="KOD" DataIndex="KOD" Header="<%$Resources:TasinirMal,FRMTHP010 %>" />
                                    <ext:Column ColumnID="ACIKLAMA" DataIndex="ACIKLAMA" Header="<%$Resources:TasinirMal,FRMTHP011 %>" />
                                    <ext:Column ColumnID="OLCUBIRIMKOD" DataIndex="OLCUBIRIMKOD" Header="<%$Resources:TasinirMal,FRMTHP012 %>"
                                        Hidden="true" />
                                    <ext:Column ColumnID="KDV" DataIndex="KDV" Header="<%$Resources:TasinirMal,FRMTHP013 %>"
                                        Hidden="true" />
                                    <ext:Column ColumnID="AMORTIYIL" DataIndex="AMORTIYIL" Header="<%$Resources:TasinirMal,FRMTHP026 %>"
                                        Hidden="true" />
                                    <ext:Column ColumnID="VURGULA" DataIndex="VURGULA" Header="Vurgula"
                                        Hidden="true" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                            <DirectEvents>
                                <CellClick OnEvent="SatirSecildi">
                                    <ExtraParams>
                                        <ext:Parameter Name="GRIDPARAM" Value="Ext1.encode(grdListe.getRowsValues({selectedOnly:true}))"
                                            Mode="Raw" />
                                    </ExtraParams>
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </CellClick>
                            </DirectEvents>
                            <Store>
                                <ext:Store ID="strListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
                                    RemotePaging="true" RemoteSort="true" OnRefreshData="StoreListe_Refresh" RemoteGroup="true">
                                    <Reader>
                                        <ext:JsonReader>
                                            <Fields>
                                                <ext:RecordField Name="KOD" />
                                                <ext:RecordField Name="ACIKLAMA" />
                                                <ext:RecordField Name="SEVIYE" />
                                                <ext:RecordField Name="DETAY" />
                                                <ext:RecordField Name="OLCUBIRIMKOD" />
                                                <ext:RecordField Name="KDV" />
                                                <ext:RecordField Name="KULLANILMIYOR" />
                                                <ext:RecordField Name="GUNCELLEME" />
                                                <ext:RecordField Name="NUMARA" />
                                                <ext:RecordField Name="AMORTIYIL" />
                                                <ext:RecordField Name="RFIDETIKETKOD" />
                                                <ext:RecordField Name="MARKAKOD" />
                                                <ext:RecordField Name="MODELKOD" />
                                                <ext:RecordField Name="KISTAMORTISMAN" />
                                                <ext:RecordField Name="VURGULA" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                    <BaseParams>
                                        <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        <ext:Parameter Name="limit" Value="0" Mode="Raw" />
                                        <ext:Parameter Name="sort" Value="KOD" />
                                        <ext:Parameter Name="dir" Value="ASC" />
                                    </BaseParams>
                                    <Proxy>
                                        <ext:PageProxy />
                                    </Proxy>
                                </ext:Store>
                            </Store>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
                                    StoreID="strListe">
                                    <Items>
                                        <ext:Label ID="Label1" runat="server" Text="Sayfada gösterilecek satır sayısı:" />
                                        <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                        <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                            <Items>
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                                <ext:ListItem Text="250" />
                                                <ext:ListItem Text="500" />
                                                <ext:ListItem Text="1000" />
                                            </Items>
                                            <SelectedItem Value="100" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();"
                                                    Delay="240000" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMTIC012%>"
                                            Icon="Disk">
                                            <Listeners>
                                                <Click Handler="KaydetKontrollu();" />
                                            </Listeners>
                                            <%--<DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Timeout="640000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>--%>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAra" runat="server" Text="<%$Resources:TasinirMal,FRMTIC014%>"
                                            Icon="Magnifier">
                                            <DirectEvents>
                                                <Click OnEvent="btnAra_Click" Timeout="200000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnSil" runat="server" Text="<%$Resources:TasinirMal,FRMTIC013%>"
                                            Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click" Timeout="200000">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt silinecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMTIC015%>"
                                            Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click" Timeout="200000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnDetayGuncelle" runat="server" Text="Detay Güncelle" Icon="ApplicationEdit">
                                            <DirectEvents>
                                                <Click OnEvent="btnDetayGuncelle_Click" Timeout="200000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Checkbox ID="chkSonDuzeyGoster" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMTHP022 %>" />
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAmortiAta" runat="server" Text="Amortisman Ömrü Ata" Icon="ApplicationFormAdd">
                                            <DirectEvents>
                                                <Click OnEvent="btnAmortiAta_Click" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:TextField ID="txtKod" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP010 %>"
                                    Width="300" />
                                <ext:TextField ID="txtAciklama" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP011 %>"
                                    Width="300" />
                                <ext:ComboBox ID="ddlOlcuBirim" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP012 %>"
                                    StoreID="strOlcubirim" ValueField="KOD" DisplayField="ADI" QueryMode="Local"
                                    Width="100" />
                                <ext:TextField ID="txtAmortismanYili" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP026 %>"
                                    Width="100" />
                                <ext:Checkbox ID="chkKist" runat="server" FieldLabel="Kıst Amortisman" />
                                <ext:ComboBox ID="ddlKdv" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP013 %>"
                                    QueryMode="Local" Width="100" />
                                <ext:ComboBox ID="ddlMarka" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP027 %>"
                                    StoreID="strMarka" ValueField="KOD" DisplayField="ADI" QueryMode="Local" Width="200">
                                    <Listeners>
                                        <Select Handler="#{ddlModel}.clearValue(); #{strModel}.reload();" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:ComboBox ID="ddlModel" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP028 %>"
                                    StoreID="strModel" ValueField="KOD" DisplayField="ADI" QueryMode="Local" Width="200" />
                                <ext:ComboBox ID="ddlRFIDEtiket" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHP029 %>"
                                    StoreID="strRFIDEtiket" ValueField="KOD" DisplayField="ADI" QueryMode="Local"
                                    Width="200" />
                                <ext:CompositeField runat="server" FieldLabel="Dosya" Width="400">
                                    <Items>
                                        <ext:Button ID="btnDosyaYukle" runat="server" Text="Dosya Yükle" Icon="Attach" OnClientClick="DosyaYukleAc();" />
                                        <ext:Button ID="btnEkliDosyaSil" runat="server" Icon="Delete" ToolTip="Dosya Sil"
                                            Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnEkliDosyaSil_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Ekli dosya silinecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:DisplayField ID="lblEkliDosyaAd" runat="server" Flex="1" />
                                        <ext:Button ID="btnEkliDosyaIndir" runat="server" Icon="DiskDownload" ToolTip="Dosya İndir"
                                            Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnEkliDosyaIndir_Click" IsUpload="true" />
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:CompositeField>
                                <ext:Checkbox ID="chkKullanilmiyor" runat="server" MarginSpec="0 0 0 105" BoxLabel="<%$Resources:TasinirMal,FRMTHP014 %>" />
                                <ext:Checkbox ID="chkGuncelleme" runat="server" MarginSpec="0 0 0 105" BoxLabel="<%$Resources:TasinirMal,FRMTHP015 %>" />
                                <ext:Checkbox ID="chkVurgula" runat="server" MarginSpec="0 0 0 105" BoxLabel="Vurgula" />
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
        <ext:Window ID="winDosyaYukle" runat="server" Width="500" Height="200" Modal="false"
            Hidden="true">
            <AutoLoad ShowMask="true" MaskMsg="Yükleniyor..." Mode="IFrame" Url="DosyaYukle.aspx?menuYok=1"
                NoCache="true" />
        </ext:Window>
    </form>
</body>
</html>
