<%@ Page Language="C#" CodeBehind="GeciciAlindiForm.aspx.cs" Inherits="TasinirMal.GeciciAlindiForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        var KomutCalistir = function (command, record, row) {
            if (command == 'hesapPlaniGoster') {
                gridYazilacakSatirNo = row;
                grdListe.getSelectionModel().selectRow(row);
                window.setTimeout('grdListe.stopEditing();', 10);
                ListeAc('ListeHesapPlani.aspx', 'grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:OLCUBIRIMIKOD:KDVORANI', '', '');
            }
        }

        function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtYil.setValue(yil);
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }

        var HesapPlanKodSelect = function (a, b) {
            var store = grdListe.getStore();
            var rowIndex = store.indexOf(grdListe.getSelectionModel().getSelected());
            var satir = store.getAt(rowIndex);
            satir.set("TASINIRHESAPKOD", b.data.hesapPlanKod);
            satir.set("TASINIRHESAPADI", b.data.hesapPlanAd);
            satir.set("OLCUBIRIMIKOD", b.data.olcuBirimAd);
            satir.set("KDVORANI", b.data.kdvOran);
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[2,4]);AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnFirmaHarcamadanAlma" runat="server" />
        <ext:Hidden ID="hdnHesapKod" runat="server" />
        <ext:Hidden ID="hdnTIFFisNo" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="MIKTAR" Type="Float" />
                        <ext:RecordField Name="OLCUBIRIMIKOD" />
                        <ext:RecordField Name="KDVORANI" Type="Int" />
                        <ext:RecordField Name="BIRIMFIYATI" Type="Float" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
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
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10"
                    Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150"
                    Height="210">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>"
                                    Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="SATIRLAR" Value="Ext1.encode(#{grdListe}.getRowsValues(false, false, false))"
                                                    Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMGAG038%>"
                                    Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnBelgeYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMGAG036%>"
                                    Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnBelgeYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnTifOlustur" runat="server" Text="<%$Resources:TasinirMal,FRMGAG037%>"
                                    Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnTIF_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="TİF oluşturulacaktır. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:DisplayField ID="lblFormDurum" runat="server" />
                                <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMGAG051%>"
                                    Icon="Tick" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnaylaIslem_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="ISLEM" Value="Onay" Mode="Value" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="<%$Resources:TasinirMal,FRMGAG052%>"
                                    Icon="Cross" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnaylaIslem_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onayı kaldırılacaktır. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="ISLEM" Value="OnayKaldir" Mode="Value" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Yıl, Belge Tarihi, Numara">
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>"
                                    Width="60" />
                                <ext:DateField ID="txtBelgeTarihi" runat="server" EmptyText="Belge Tarihi" Width="100" />
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
                                <ext:DisplayField ID="lblTIF" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG025 %>"
                                    Width="120">
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
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG027 %>"
                                    Width="120">
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
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG029 %>"
                                    Width="120">
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
                        <ext:TriggerField ID="txtNeredenGeldi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG031 %>"
                            Width="270">
                            <Triggers>
                                <ext:FieldTrigger Icon="Search" />
                            </Triggers>
                            <Listeners>
                                <TriggerClick Fn="TriggerClick" />
                            </Listeners>
                        </ext:TriggerField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:DateField ID="txtFaturaTarihi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG033 %>"
                                    EmptyText="<%$Resources:TasinirMal,FRMGAG033 %>" Width="100" />
                                <ext:TextField ID="txtNumara" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG034 %>"
                                    EmptyText="<%$Resources:TasinirMal,FRMGAG034 %>" Width="100" />
                            </Items>
                        </ext:CompositeField>
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true"
                    ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column ColumnID="TASINIRHESAPKOD" DataIndex="TASINIRHESAPKOD" Header="<%$Resources:TasinirMal,FRMZFG010 %>"
                                Width="225" Fixed="true" Hideable="false">
                                <Editor>
                                    <ext:ComboBox runat="server" DisplayField="hesapPlanKod" ValueField="hesapPlanKod"
                                        TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                        HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                        StoreID="strHesapPlan">
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                                <tpl for=".">
                                                <div class="search-item">
                                                    <span>{hesapPlanKod} - {hesapPlanAd}</span>
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
                                    <ext:ImageCommand CommandName="hesapPlaniGoster" Icon="Magnifier" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="<%$Resources:TasinirMal,FRMZFG013 %>"
                                Width="200" />
                            <ext:Column ColumnID="MIKTAR" DataIndex="MIKTAR" Header="<%$Resources:TasinirMal,FRMGAG009 %>"
                                Align="Right" Width="80">
                                <Editor>
                                    <ext:TextField runat="server" MaskRe="[,0-9]" StyleSpec="text-align:right" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="OLCUBIRIMIKOD" DataIndex="OLCUBIRIMIKOD" Header="<%$Resources:TasinirMal,FRMGAG010 %>"
                                Width="100" />
                            <ext:Column ColumnID="KDVORANI" DataIndex="KDVORANI" Header="<%$Resources:TasinirMal,FRMGAG011 %>"
                                Align="Right" Width="80">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="BIRIMFIYATI" DataIndex="BIRIMFIYATI" Header="<%$Resources:TasinirMal,FRMZFG015 %>"
                                Align="Right" Width="120" Tooltip="<%$Resources:TasinirMal,FRMZFG015 %>">
                                <Editor>
                                    <ext:TextField runat="server" MaskRe="[,0-9]" StyleSpec="text-align:right" />
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
                    </Listeners>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="500" HideRefresh="true"
                            StoreID="strListe">
                            <Items>
                                <ext:Label ID="Label1" runat="server" Text="Satır sayısı:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                    <Items>
                                        <ext:ListItem Text="50" />
                                        <ext:ListItem Text="100" />
                                        <ext:ListItem Text="250" />
                                        <ext:ListItem Text="500" />
                                        <ext:ListItem Text="1000" />
                                        <ext:ListItem Text="2500" />
                                        <ext:ListItem Text="5000" />
                                    </Items>
                                    <SelectedItem Value="500" />
                                    <Listeners>
                                        <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:Button ID="btnSatirAc" runat="server" Text="Satır Aç" Icon="TableRow">
                                    <Menu>
                                        <ext:Menu runat="server">
                                            <Items>
                                                <ext:MenuItem ID="MenuItem1" runat="server" Text="10" Icon="TableRow" OnClientClick="GridSatirAc(strListe,10)" />
                                                <ext:MenuItem ID="MenuItem3" runat="server" Text="20" Icon="TableRow" OnClientClick="GridSatirAc(strListe,20)" />
                                                <ext:MenuItem ID="MenuItem4" runat="server" Text="50" Icon="TableRow" OnClientClick="GridSatirAc(strListe,50)" />
                                                <ext:MenuItem ID="MenuItem5" runat="server" Text="100" Icon="TableRow" OnClientClick="GridSatirAc(strListe,100)" />
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
                                <ext:Button ID="btnHesapPlanModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                    <Listeners>
                                        <Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:OLCUBIRIMIKOD:KDVORANI:MIKTAR:BIRIMFIYATI','','');return false;" />
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
