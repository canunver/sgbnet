<%@ Page Language="C#" Inherits="TasinirMal.MuhasebeBilgisi " CodeBehind="MuhasebeBilgisi.aspx.cs" %>

<%@ Register TagPrefix="ext" Namespace="Ext1.Net" Assembly="Ext1.Net" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Muhasebe İşlemleri</title>
    <script type="text/javascript">
        var JsonGoster = function (islemRefNo) {
            var jsonStr = "{\"muhasebeIslemiList\": [ " + strMuhasebe.data.get(islemRefNo).data.JSON + "]}";
            var jsonObj = JSON.parse(jsonStr);
            var json = JSON.stringify(jsonObj, null, '\t');
            $("pre").text(json);
            winJson.show();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <%--<DocumentReady Handler="btnDurumGuncelle.fireEvent('click');" />--%>
            </Listeners>
        </ext:ResourceManager>
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
        <ext:Viewport ID="Viewport1" runat="server" Layout="BorderLayout" StyleSpec="background-color: transparent;">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Region="Center" Layout="BorderLayout" Margins="104 20 10 20">
                    <Items>

                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" ForceFit="true" Collapsible="true"
                            Width="280" Margins="5 0 5 5" Split="true" AutoRender="false" Header="false">
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
                                        <ext:Button ID="btnListele" runat="server" Text="Listele"
                                            Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnOku_Click" Timeout="900000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="raporTur" Value="3" Mode="Value" />
                                                    </ExtraParams>
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
                                <ext:PropertyGridParameter Name="prpDurum" DisplayName="<%$ Resources:TasinirMal, FRMTIS014 %>">
                                    <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                                    <Editor>
                                        <ext:ComboBox runat="server" TriggerAction="All" ForceSelection="true"
                                            Editable="false" StoreID="strDurum" DisplayField="ADI" ValueField="KOD" Resizable="true"
                                            ListWidth="200" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpIslemRefNo" DisplayName="İşlem Referans No">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpIslemCinsi" DisplayName="İşlem Cinsi">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
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
                            TrackMouseOver="true" Border="true" Margins="5 5 5 0" Split="true"
                            Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="Muhasebe Servisine Gönder"
                                            Icon="ServerGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Timeout="900000">
                                                    <EventMask ShowMask="true" Msg="" />
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Seçilen kayıtlar muhasebe servisine gönderilecek. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Muhasebe kaydı gönderiliyor..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly: true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnDurumGuncelle" runat="server" Text="Durum Sorgula ve Güncelle"
                                            Icon="TableRefresh">
                                            <DirectEvents>
                                                <Click OnEvent="btnDurumGuncelle_Click" Timeout="900000">
                                                    <%--<Confirmation ConfirmRequest="true" Title="Onay" Message="Muhasebe servisinden durum sorgulama işlemi yapılacak. Onaylıyor musunuz?" />--%>
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Muhasebe durumu sorgulanıyor..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly: true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>

                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server" ID="CheckboxSelectionModel1"></ext:CheckboxSelectionModel>
                            </SelectionModel>
                            <Store>
                                <ext:Store runat="server" ID="strMuhasebe">
                                    <Reader>
                                        <ext:JsonReader IDProperty="ISLEMREFNO">
                                            <Fields>
                                                <ext:RecordField Name="ISLEMREFNO" />
                                                <ext:RecordField Name="ISLEMCINSI" />
                                                <ext:RecordField Name="SERVIS" />
                                                <ext:RecordField Name="DURUM" />
                                                <ext:RecordField Name="ISLEMYAPAN" />
                                                <ext:RecordField Name="ISLEMTARIH" Type="Date" />
                                                <ext:RecordField Name="JSON" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:TemplateColumn ColumnID="ISLEMREFNO" DataIndex="ISLEMREFNO" Header="İşlem Referans No" Align="Center"
                                        Width="170" Groupable="false" Fixed="true" Hideable="false">
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                                <a href="javascript:JsonGoster('{ISLEMREFNO}')">{ISLEMREFNO}</a>
                                            </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:Column DataIndex="ISLEMCINSI" ColumnID="ISLEMCINSI" Header="İşlem Cinsi" Width="180" />
                                    <ext:Column DataIndex="SERVIS" ColumnID="SERVIS" Header="Servis" Width="60" />
                                    <ext:Column DataIndex="DURUM" ColumnID="DURUM" Header="Durum" Width="175">
                                        <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                                    </ext:Column>
                                    <ext:Column DataIndex="ISLEMYAPAN" ColumnID="ISLEMYAPAN" Header="İşlem Yapan" Width="155" />
                                    <ext:DateColumn DataIndex="ISLEMTARIH" ColumnID="ISLEMTARIH" Header="İşlem Tarihi" Width="120" Format="dd.m.Y HH:mm:ss" Fixed="true" />
                                </Columns>
                            </ColumnModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="500" HideRefresh="true"
                                    StoreID="strDurum">
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
                            <View>
                                <ext:GridView ID="GridView1" runat="server" />
                            </View>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
        <ext:Window ID="winJson" runat="server" Title="JSON" Width="600" Height="550"
            Modal="true" Hidden="true" Padding="5" Layout="FitLayout" PaddingSummary="20" AutoScroll="true">
            <Content>
                <pre id="json"></pre>
            </Content>
        </ext:Window>
    </form>
</body>
</html>
