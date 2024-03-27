<%@ Page Language="C#" CodeBehind="SicilNoDegerArtisSorgu.aspx.cs" Inherits="TasinirMal.SicilNoDegerArtisSorgu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            parent.tabPanelAna.setActiveTab("panelIslem");
            parent.panelIslem.getBody().BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo);
        }

        function TarihceGoster(yil, muhasebe, harcamaBirimi, belgeNo) {
            try { document.getElementById('lblHata').innerHTML = ""; }
            catch (e) { }

            showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo, 500, 350, true, null);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="HARCAMABIRIMIKOD" />
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="HARCAMABIRIMI" />
                        <ext:RecordField Name="BELGENO" />
                        <ext:RecordField Name="BELGETARIHI" Type="Date" />
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="GORSICILNO" />
                        <ext:RecordField Name="TUTAR" />
                        <ext:RecordField Name="GEREKCE" />
                        <ext:RecordField Name="DURUM" />
                        <ext:RecordField Name="MALZEMEADI" />
                        <ext:RecordField Name="TUR" />
                        <ext:RecordField Name="TURADI" />
                        <ext:RecordField Name="AMORTISMAN" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strDurum" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strTur" runat="server">
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
                <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" ForceFit="true" Collapsible="true"
                    Width="250" Margins="5 0 5 5" Split="true" AutoRender="false" Header="false">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnSorguTemizle_Click">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>"
                                    Icon="ApplicationGo">
                                    <DirectEvents>
                                        <Click OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Source>
                        <ext:PropertyGridParameter Name="prpMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMBRK006 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpMuhasebe',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpHarcamaBirimi',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpTur" DisplayName="Tür">
                            <Renderer Handler="return PropertyRenderer(strTur,value);" />
                            <Editor>
                                <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                    Editable="false" StoreID="strTur" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                    ListWidth="200" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNo1" DisplayName="<%$ Resources:TasinirMal, FRMZFS031 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNo2" DisplayName="<%$ Resources:TasinirMal, FRMZFS032 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMZFS033 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMZFS034 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpSicilNo" DisplayName="<%$ Resources:TasinirMal, FRMBRK018 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpSicilNo',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpEskiSicilNo" DisplayName="Eski Sicil No">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpDurum" DisplayName="<%$ Resources:TasinirMal, FRMTIS014 %>">
                            <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                            <Editor>
                                <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                    Editable="false" StoreID="strDurum" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                    ListWidth="200" />
                            </Editor>
                        </ext:PropertyGridParameter>
                    </Source>
                    <Listeners>
                        <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                        <SortChange Handler="this.getStore().sortInfo = undefined;" />
                    </Listeners>
                    <View>
                        <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                    </View>
                </ext:PropertyGrid>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="5 5 5 0" Split="true" Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:Button ID="btnYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMGAS018 %>" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill />
                                <ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayaGonder_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Seçilen belgeler onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                    Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBelgeIptal" runat="server" Text="İptal Et" Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnIptal_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Seçilen belgeler iptal edilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                    Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:TemplateColumn ColumnID="BELGENO" DataIndex="BELGENO" Header="Belge No"
                                Align="Center" Width="80" Groupable="false" Fixed="true" Hideable="false">
                                <Template runat="server">
                                    <Html>
                                        <a href="javascript:BelgeAc('{YIL}','{MUHASEBEKOD}','{HARCAMABIRIMIKOD}','{BELGENO}')">{BELGENO}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:DateColumn runat="server" ColumnID="BELGETARIHI" DataIndex="BELGETARIHI" Header="Belge Tarihi" Width="80" Format="dd.m.Y" />
                            <ext:Column ColumnID="MUHASEBE" DataIndex="MUHASEBE" Header="Muhasebe" Width="200" />
                            <ext:Column ColumnID="HARCAMABIRIMI" DataIndex="HARCAMABIRIMI" Header="Harcama Birimi" Width="200" />
                            <ext:Column ColumnID="MALZEMEADI" DataIndex="MALZEMEADI" Header="Malzeme Adı" Width="200" />
                            <ext:Column runat="server" ColumnID="TUTAR" DataIndex="TUTAR" Header="Tutar" Align="Right" Width="80" />
                            <ext:Column ColumnID="AMORTISMAN" DataIndex="AMORTISMAN" Header="Amortisman" Align="Right" Width="80" />
                            <ext:Column ColumnID="TURADI" DataIndex="TURADI" Header="Tür" Width="140" />
                            <ext:Column ColumnID="DURUM" DataIndex="DURUM" Header="Durum" Width="100" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:CheckboxSelectionModel runat="server" />
                    </SelectionModel>
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
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
