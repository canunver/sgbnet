<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SayimTutanagiKayit.aspx.cs"
    Inherits="TasinirMal.SayimTutanagiKayit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" rel="stylesheet" href="../Script/GridExt.css" />
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=1"></script>
    <script language="javascript" type="text/javascript" src="ModulScripts/SayimGiris.js?v=1"></script>
    <script type="text/javascript">
        function SatirEkle() {
            for (i = 0; i < 10; i++) {
                var rowIndex = grdListe.addRecord();
                grdListe.getView().focusRow(rowIndex);
                grdListe.startEditing(rowIndex, 0);
            }
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
        <input type="hidden" id="hdnSayimNo" runat="server" />
        <ext:Store runat="server" ID="StoreListe">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="HESAPPLANKOD" />
                        <ext:RecordField Name="HESAPPLANAD" />
                        <ext:RecordField Name="OLCUBIRIMAD" />
                        <ext:RecordField Name="AMBARMIKTAR" />
                        <ext:RecordField Name="ORTAKMIKTAR" />
                        <ext:RecordField Name="KAYITKISIMIKTAR" />
                        <ext:RecordField Name="ACIKLAMA" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport2" runat="server" StyleSpec="background-color: #DFE8F6">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <North Split="true" UseSplitTips="true" CollapsibleSplitTip="Kapatmak için çift tıklayın"
                        ExpandableSplitTip="Açmak için çift tıklayın">
                        <ext:Panel ID="Panel3" runat="server" Collapsible="true" Title="Alanlar" StyleSpec="background-color: #DFE8F6"
                            Padding="7" Height="190" Border="false" BodyStyle="background-color: #DFE8F6"
                            Layout="FormLayout" Frame="false" LabelWidth="120">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG039 %>"
                                            Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <EventMask ShowMask="true" Msg="Lütfen Bekleyin..." />
                                                    <Confirmation Message="kayıt(lar) kaydedilecektir. Onaylıyor musunuz?" ConfirmRequest="true"
                                                        Title="Onay" />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="DirtyGridRows" Value="Ext1.encode(#{grdListe}.getRowsValues({dirtyRowsOnly:false}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnSil" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG040 %>"
                                            Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                    <Confirmation Message="kayıt(lar) silinecektir. Onaylıyor musunuz?" ConfirmRequest="true"
                                                        Title="Onay" />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG043 %>"
                                            Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                    <Confirmation Message="Alan(lar) temizlenecektir. Onaylıyor musunuz?" ConfirmRequest="true"
                                                        Title="Onay" />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnSayimTutanak" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG041 %>"
                                            Icon="Table">
                                            <DirectEvents>
                                                <Click OnEvent="btnSayimTutanak_Click" Timeout="240000" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnAmbarDevirTutanak" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG042 %>"
                                            Icon="Table">
                                            <DirectEvents>
                                                <Click OnEvent="btnAmbarDevirTutanak_Click" Timeout="240000" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnAmbarAktar" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG044 %>"
                                            Icon="TableRelationship">
                                            <DirectEvents>
                                                <Click OnEvent="btnAmbarAktar_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnAmbarAktarKaydet" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG051 %>"
                                            Icon="TextRuler">
                                            <DirectEvents>
                                                <Click OnEvent="btnAmbarAktarKaydet_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnTIFNoksan" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG045 %>"
                                            Icon="TableDelete">
                                            <DirectEvents>
                                                <Click OnEvent="btnTIFNoksan_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnTIFFazla" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG046 %>"
                                            Icon="TableAdd">
                                            <DirectEvents>
                                                <Click OnEvent="btnTIFFazla_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnTerminalAc" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG047 %>"
                                            Icon="Connect">
                                            <Listeners>
                                                <Click Handler="ListeAcTerminal('TerminalOku.aspx');return false;" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Button ID="btnListele" runat="server" Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnListele_Click" Timeout="240000">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:Container ID="Container9" runat="server" Layout="FormLayout" LabelWidth="125">
                                    <Items>
                                        <ext:ComboBox ID="ddlYil" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG030 %>"
                                            Width="80">
                                        </ext:ComboBox>
                                        <ext:CompositeField ID="cf1" runat="server">
                                            <Items>
                                                <ext:TriggerField ID="txtMuhasebe" runat="server" MaxLength="5" FieldLabel="<%$ Resources:TasinirMal,FRMSYL003 %>">
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
                                                <ext:TriggerField ID="txtHarcamaBirimi" runat="server" MaxLength="15" FieldLabel="<%$ Resources:TasinirMal,FRMSYL005 %>">
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
                                        <ext:CompositeField ID="CompositeField2" runat="server">
                                            <Items>
                                                <ext:TriggerField ID="txtAmbar" runat="server" MaxLength="3" FieldLabel="<%$ Resources:TasinirMal,FRMSYL007 %>">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Fn="TriggerClick" />
                                                        <Change Fn="TriggerChange" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblAmbarAd" runat="server">
                                                </ext:Label>
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:DropDownField ID="ddlBirim" runat="server" Width="300" TriggerIcon="Ellipsis"
                                            Hidden="true" FieldLabel="<%$ Resources:TasinirMal,FRMORG001 %>" Mode="ValueText">
                                            <Component>
                                                <ext:Panel ID="Panel2" runat="server" Width="400">
                                                    <Items>
                                                        <ext:TreePanel ID="TreePanel1" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                            Border="false" RootVisible="false">
                                                            <Root>
                                                                <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                                            </Root>
                                                            <Listeners>
                                                                <BeforeLoad Fn="OrgBirimDoldur" />
                                                                <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', \'txtAmbar\');');" />
                                                            </Listeners>
                                                        </ext:TreePanel>
                                                    </Items>
                                                    <Listeners>
                                                        <Show Handler="try { TreePanel1.getRootNode().findChild('id', ddlBirim.getValue(), true).select(); } catch (e) { }" />
                                                    </Listeners>
                                                </ext:Panel>
                                            </Component>
                                        </ext:DropDownField>
                                        <ext:DateField ID="txtSayimTarihi" runat="server" Width="90" FieldLabel="<%$ Resources:TasinirMal,FRMSYG037 %>">
                                        </ext:DateField>
                                    </Items>
                                </ext:Container>
                            </Items>
                        </ext:Panel>
                    </North>
                    <Center>
                        <ext:FormPanel ID="FormPanel1" runat="server" StyleSpec="background-color: #DFE8F6"
                            Layout="FitLayout" Border="false" BodyStyle="background-color: #DFE8F6" Height="400">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnSatirEkle" runat="server" Text="Satır Ekle" Icon="Add">
                                            <Listeners>
                                                <Click Handler="SatirEkle();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Button ID="btnDelete" runat="server" Text="Seçili Satırları Sil" Icon="Delete">
                                            <Listeners>
                                                <Click Handler="#{grdListe}.deleteSelected();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:GridPanel ID="grdListe" runat="server" StoreID="StoreListe" AutoExpandColumn="HESAPPLANAD"
                                    Layout="FormLayout">
                                    <ColumnModel ID="ColumnModel1" runat="server">
                                        <Columns>
                                            <ext:RowNumbererColumn />
                                            <ext:Column DataIndex="HESAPPLANKOD" ColumnID="HESAPPLANKOD" Header="Hesap Plan Kod"
                                                Width="130" Groupable="false">
                                                <Editor>
                                                    <ext:TriggerField ID="txtHesapPlanKod" runat="server" MaxLength="40">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="Hesap Kodu" />
                                                        </Triggers>
                                                        <TabMenu>
                                                            <ext:Menu runat="server">
                                                                <Items>
                                                                    <ext:Label ID="lbl" Icon="Zoom" runat="server">
                                                                    </ext:Label>
                                                                </Items>
                                                            </ext:Menu>
                                                        </TabMenu>
                                                        <Listeners>
                                                            <TriggerClick Fn="TriggerClick" />
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:Column>
                                            <ext:Column DataIndex="HESAPPLANAD" ColumnID="HESAPPLANAD" Header="Hesap Plan Adı"
                                                Editable="false" Width="120">
                                            </ext:Column>
                                            <ext:Column DataIndex="OLCUBIRIMAD" ColumnID="OLCUBIRIMAD" Header="Ölçü Birimi Adı"
                                                Editable="false" Width="120">
                                            </ext:Column>
                                            <ext:Column DataIndex="AMBARMIKTAR" ColumnID="AMBARMIKTAR" Header="Ambarda" Width="100">
                                                <Editor>
                                                    <ext:NumberField ID="txtAmbarMiktar" runat="server">
                                                    </ext:NumberField>
                                                </Editor>
                                            </ext:Column>
                                            <ext:Column DataIndex="ORTAKMIKTAR" ColumnID="ORTAKMIKTAR" Header="Ortak Alan" Align="Left"
                                                Width="100">
                                                <Editor>
                                                    <ext:NumberField ID="txtOrtakMiktar" runat="server">
                                                    </ext:NumberField>
                                                </Editor>
                                            </ext:Column>
                                            <ext:Column DataIndex="KAYITKISIMIKTAR" ColumnID="KAYITKISIMIKTAR" Header="Zimmette"
                                                Align="Left" Width="100">
                                            </ext:Column>
                                            <ext:Column DataIndex="ACIKLAMA" ColumnID="ACIKLAMA" Header="Açıklama" Align="Left"
                                                Width="100">
                                            </ext:Column>
                                        </Columns>
                                    </ColumnModel>
                                    <LoadMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                    <BottomBar>
                                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
                                            StoreID="StoreListe">
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
                                                        <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                            </Items>
                                        </ext:PagingToolbar>
                                    </BottomBar>
                                    <View>
                                        <ext:GridView ID="GridView1" ForceFit="true" runat="server">
                                        </ext:GridView>
                                    </View>
                                    <SelectionModel>
                                        <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                                    </SelectionModel>
                                </ext:GridPanel>
                            </Items>
                            <BottomBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Label Icon="Lightbulb" ID="lblBilgi" runat="server" Text="<%$ Resources:TasinirMal,FRMSYG050 %>">
                                        </ext:Label>
                                        <ext:DisplayField ID="dfBilgi" runat="server" Height="30">
                                        </ext:DisplayField>
                                    </Items>
                                </ext:Toolbar>
                            </BottomBar>
                        </ext:FormPanel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
