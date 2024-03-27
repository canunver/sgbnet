<%@ Page Language="C#" CodeBehind="TasinirIslemFormSorgu.aspx.cs" Inherits="TasinirMal.TasinirIslemFormSorgu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            parent.tabPanelAna.setActiveTab("panelIslem");
            parent.panelIslem.getBody().BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo);
        }

        function TarihceGoster(yil, muhasebe, harcamaBirimi, belgeNo) {
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
                        <ext:RecordField Name="fisNo" />
                        <ext:RecordField Name="yil" Type="Int" />
                        <ext:RecordField Name="muhasebe" />
                        <ext:RecordField Name="harcamaBirimi" />
                        <ext:RecordField Name="harcamaBirimiAd" />
                        <ext:RecordField Name="ambar" />
                        <ext:RecordField Name="fisTarihi" Type="Date" />
                        <ext:RecordField Name="islemTipi" Type="Int" />
                        <ext:RecordField Name="durum" />
                        <ext:RecordField Name="islemTarih" Type="Date" />
                        <ext:RecordField Name="islemYapan" />
                        <ext:RecordField Name="kod" />
                        <ext:RecordField Name="faturaTarihi" Type="Date" />
                        <ext:RecordField Name="faturaNo" />
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
        <ext:Store ID="strIslemTipi" runat="server">
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
                        <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                            <Editor>
                                <ext:SpinnerField runat="server">
                                </ext:SpinnerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpDurum" DisplayName="<%$ Resources:TasinirMal, FRMTIS014 %>">
                            <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlDurum" runat="server" TriggerAction="All" ForceSelection="true"
                                    Editable="false" StoreID="strDurum" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                    ListWidth="200" />
                            </Editor>
                        </ext:PropertyGridParameter>
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
                        <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpAmbar',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNo1" DisplayName="<%$ Resources:TasinirMal, FRMTIS021 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNo2" DisplayName="<%$ Resources:TasinirMal, FRMTIS022 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMTIS023 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMTIS024 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpDurumTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMTIS025 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpDurumTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMTIS026 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpFaturaNo" DisplayName="Fatura No">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpNereye" DisplayName="<%$ Resources:TasinirMal, FRMTIS027 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpKime" DisplayName="<%$ Resources:TasinirMal, FRMTIS028 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpHesapKod" DisplayName="Hesap Kodu">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpHesapKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpNereden" DisplayName="<%$ Resources:TasinirMal, FRMTIS030 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpIslemTipi" DisplayName="<%$ Resources:TasinirMal, FRMTIS031 %>">
                            <Renderer Handler="return PropertyRenderer(strIslemTipi,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlIslemTipi" runat="server" TriggerAction="All" ForceSelection="true"
                                    Editable="false" StoreID="strIslemTipi" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                    ListWidth="200" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpIslemYapan" DisplayName="<%$ Resources:TasinirMal, FRMTIS032 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMBRK006 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonMuhasebe',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
                            <Editor>
                                <ext:TriggerField runat="server" MaxLength="15">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonHarcamaBirimi',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                            <Editor>
                                <ext:TriggerField runat="server" MaxLength="3" EmptyText="">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonAmbar',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                          <ext:PropertyGridParameter Name="prpAciklama" DisplayName="Açıklama">
                        </ext:PropertyGridParameter>
                    </Source>
                    <Listeners>
                        <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                        <SortChange Handler="this.getStore().sortInfo = undefined;" />
                    </Listeners>
                    <View>
                        <ext:GridView ID="GridView2" ForceFit="true" runat="server" />
                    </View>
                </ext:PropertyGrid>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" StoreID="strListe" Margins="5 5 5 0" Split="true"
                    Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <ext:Button ID="btnListeYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS043%>"
                                    Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnListeYazdir_Click" Json="true" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnIslemler" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS042%>"
                                    Icon="TableGear">
                                    <Menu>
                                        <ext:Menu ID="mnuIslem" runat="server">
                                            <Items>
                                                <ext:MenuItem ID="btnOnayla" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS044%>"
                                                    Icon="Accept">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnIslem_Click" Timeout="9600000">
                                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="islem" Value="Onay" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnOnayKaldir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS045%>"
                                                    Icon="Delete">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnIslem_Click" Timeout="9600000">
                                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="islem" Value="OnayKaldir" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnOnayaGonder" runat="server" Text="Onaya Gönder" Icon="LightningGo"
                                                    Hidden="true">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnIslem_Click">
                                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="islem" Value="OnayaGonder" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnBelgeYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS043%>"
                                                    Icon="PageExcel">
                                                    <DirectEvents>
                                                        <Click Json="true" IsUpload="true" OnEvent="btnIslem_Click" Timeout="9600000">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="islem" Value="Yazdir" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnIslemIptal" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS046%>"
                                                    Icon="PageWhiteDelete">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnIslem_Click" Timeout="9600000">
                                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="json" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="islem" Value="İptal" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:TemplateColumn ColumnID="fisNo" DataIndex="fisNo" Header="Belge No" Align="Center"
                                Width="100" Groupable="false" Fixed="true" Hideable="false">
                                <Template ID="Template1" runat="server">
                                    <Html>
                                        <a href="javascript:BelgeAc('{yil}','{muhasebe}','{harcamaBirimi}','{fisNo}')">{fisNo}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:Column ColumnID="yil" DataIndex="yil" Header="Yıl" Width="80" Fixed="true" />
                            <ext:DateColumn ColumnID="fisTarihi" DataIndex="fisTarihi" Header="Belge Tarihi"
                                Width="100" Fixed="true" Format="dd.m.Y" />
                            <ext:Column ColumnID="muhasebe" DataIndex="muhasebe" Header="Muhasebe" Width="100"
                                Fixed="true" />
                            <ext:Column ColumnID="harcamaBirimi" DataIndex="harcamaBirimi" Header="Harcama Birimi" />
                            <ext:Column ColumnID="harcamaBirimiAd" DataIndex="harcamaBirimiAd" Header="Harcama Birimi Adı" />
                            <ext:Column ColumnID="ambar" DataIndex="ambar" Header="Ambar" />
                            <ext:Column ColumnID="islemTipi" DataIndex="islemTipi" Header="İşlem Tipi">
                                <Renderer Handler="return PropertyRenderer(strIslemTipi,value);" />
                            </ext:Column>
                            <ext:DateColumn ColumnID="faturaTarihi" DataIndex="faturaTarihi" Header="Fatura Tarihi"
                                Width="100" Fixed="true" Format="dd.m.Y" />
                            <ext:TemplateColumn ColumnID="durum" DataIndex="durum" Header="Durum" Align="Left"
                                Width="100" Groupable="false" Fixed="true" Hideable="false">
                                <Template ID="Template2" runat="server">
                                    <Html>
                                        <a href="javascript:TarihceGoster('{yil}','{muhasebe}','{harcamaBirimi}','{fisNo}')">{durum}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:Column ColumnID="faturaNo" DataIndex="faturaNo" Header="Fatura No" Hidden="true" />
                            <ext:DateColumn ColumnID="islemTarih" DataIndex="islemTarih" Header="Kayıt Tarihi"
                                Format="dd.m.Y" />
                            <ext:Column ColumnID="islemYapan" DataIndex="islemYapan" Header="Kayıt Eden" />
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
