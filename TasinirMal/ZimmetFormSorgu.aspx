<%@ Page Language="C#" CodeBehind="ZimmetFormSorgu.aspx.cs" Inherits="TasinirMal.ZimmetFormSorgu" %>

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
            var belgeTur = document.getElementById('hdnBelgeTur').value;
            showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo + "&belgeTur=" + belgeTur + "&tarihceTur=zimmet", 500, 350, true, null);
        }

        function IslemYap(islemTip) {

            var yil = "";
            var muhasebeKod = "";
            var harcamaBirimKod = "";
            var belgeNo = "";
            var belgeTur = "";

            var kontrol = grdListe.getSelectionModel().events.rowdeselect.obj.selections.items;

            if (kontrol) {
                for (var i = 0; i < kontrol.length; i++) {
                    if (yil != "") yil = yil + ";";
                    yil += kontrol[i].data.YIL;

                    if (muhasebeKod != "") muhasebeKod = muhasebeKod + ";";
                    muhasebeKod += kontrol[i].data.MUHASEBEKOD;

                    if (harcamaBirimKod != "") harcamaBirimKod = harcamaBirimKod + ";";
                    harcamaBirimKod += kontrol[i].data.HARCAMABIRIMIKOD;

                    if (belgeNo != "") belgeNo = belgeNo + ";";
                    belgeNo += kontrol[i].data.BELGENO;
                }
            }

            var params = { isUpload: false, eventMask: { showMask: true, msg: "Lütfen Bekleyin..." } }
            if (islemTip == "Yazdir") {
                params = { isUpload: true }
            }

            if (belgeNo == "") {
                extKontrol.Msg.alert("Uyarı", "Listeden işlem yapılacak belge seçilmemiş.").Show();
                return;
            }

            Ext1.net.DirectMethods.Islem(yil, muhasebeKod, harcamaBirimKod, belgeNo, islemTip, params);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnBelgeTur" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="YIL" />
                        <ext:RecordField Name="BELGENO" />
                        <ext:RecordField Name="BELGETARIHI" Type="Date" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="HARCAMABIRIMIKOD" />
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="HARCAMABIRIMI" />
                        <ext:RecordField Name="TIPI" />
                        <ext:RecordField Name="TURU" />
                        <ext:RecordField Name="AMBAR" />
                        <ext:RecordField Name="DURUM" />
                        <ext:RecordField Name="KIMEVERILDI" />
                        <ext:RecordField Name="NEREYEVERILDI" />
                        <ext:RecordField Name="SONISLEMTARIHI" Type="Date" />
                        <ext:RecordField Name="SONISLEMYAPAN" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strDurum" runat="server">
            <Reader>
                <ext:JsonReader runat="server" IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strBelgeTuru" runat="server">
            <Reader>
                <ext:JsonReader runat="server" IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strBelgeTipi" runat="server">
            <Reader>
                <ext:JsonReader runat="server" IDProperty="KOD">
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
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>" Icon="ApplicationGo">
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
                        <ext:PropertyGridParameter Name="prpDurum" DisplayName="<%$ Resources:TasinirMal, FRMZFS054 %>">
                            <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlDurum" runat="server" ForceSelection="true" StoreID="strDurum"
                                    ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTuru" DisplayName="<%$ Resources:TasinirMal, FRMZFS029 %>">
                            <Renderer Handler="return PropertyRenderer(strBelgeTuru,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlBelgeTuru" runat="server" StoreID="strBelgeTuru" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTipi" DisplayName="<%$ Resources:TasinirMal, FRMZFS030 %>">
                            <Renderer Handler="return PropertyRenderer(strBelgeTipi,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlBelgeTipi" runat="server" StoreID="strBelgeTipi" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
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
                        <ext:PropertyGridParameter Name="prpKisiKod" DisplayName="TC Kimlik No">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpKisiKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpOdaKod" DisplayName="Oda Kodu">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpOdaKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpSonislemyapan" DisplayName="<%$ Resources:TasinirMal, FRMGAS016 %>">
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
                        <ext:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <ext:Button ID="btnListeYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS043%>" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnListeYazdir_Click" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnIslemler" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS042%>"
                                    Icon="TableGear">
                                    <Menu>
                                        <ext:Menu ID="mnuIslem" runat="server">
                                            <Items>
                                                <ext:MenuItem ID="btnBelgeYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMZFS045%>"
                                                    Icon="PageExcel">
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();IslemYap('Yazdir');return false;"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnOnayla" runat="server" Text="<%$ Resources:TasinirMal, FRMZFS046%>"
                                                    Icon="Tick">
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();OnayAlExt('Seçili belgeler onaylanacaktır.', 'IslemYap', 'Onay', '');return false;"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnOnayKaldir" runat="server" Text="<%$ Resources:TasinirMal, FRMZFS047%>"
                                                    Icon="Cross">
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();OnayAlExt('Seçili belgelerin onayı kaldırılacaktır.', 'IslemYap', 'OnayKaldir', '');return false;"></Click>
                                                    </Listeners>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnIptal" runat="server" Text="<%$ Resources:TasinirMal, FRMZFS048%>"
                                                    Icon="Delete">
                                                    <Listeners>
                                                        <Click Handler="mnuIslem.hide();OnayAlExt('Seçili belgeler iptal edilecektir.', 'IslemYap', 'İptal', '');return false;"></Click>
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
                            <ext:TemplateColumn ColumnID="BELGENO" DataIndex="BELGENO" Header="Belge No" Align="Center" Width="80" Groupable="false" Fixed="true" Hideable="false">
                                <Template runat="server">
                                    <Html>
                                        <a href="javascript:BelgeAc('{YIL}','{MUHASEBEKOD}','{HARCAMABIRIMIKOD}','{BELGENO}')">{BELGENO}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:Column ColumnID="YIL" DataIndex="YIL" Header="<%$ Resources:TasinirMal, FRMZFS021 %>" Width="50" Hidden="true" />
                            <ext:DateColumn runat="server" ColumnID="BELGETARIHI" DataIndex="BELGETARIHI" Header="<%$ Resources:TasinirMal, FRMZFS050 %>" Width="80" Format="dd.m.Y" />
                            <ext:Column ColumnID="TIPI" DataIndex="TIPI" Header="<%$ Resources:TasinirMal, FRMZFS030 %>" Hidden="true" Width="80" />
                            <ext:Column ColumnID="TURU" DataIndex="TURU" Header="<%$ Resources:TasinirMal, FRMZFS029 %>" Width="80" />
                            <ext:Column ColumnID="MUHASEBE" DataIndex="MUHASEBE" Header="<%$ Resources:TasinirMal, FRMZFS051 %>" Width="200" />
                            <ext:Column ColumnID="HARCAMABIRIMI" DataIndex="HARCAMABIRIMI" Header="<%$ Resources:TasinirMal, FRMZFS052 %>" Width="200" />
                            <ext:Column ColumnID="AMBAR" DataIndex="AMBAR" Header="<%$ Resources:TasinirMal, FRMZFS053 %>" Width="200" />
                            <ext:Column ColumnID="KIMEVERILDI" DataIndex="KIMEVERILDI" Header="Kime Verildi" Width="150" />
                            <ext:Column ColumnID="NEREYEVERILDI" DataIndex="NEREYEVERILDI" Header="Nereye Verildi" Width="150" />
                            <ext:TemplateColumn ColumnID="DURUM" DataIndex="DURUM" Header="<%$ Resources:TasinirMal, FRMZFS054 %>" Align="Center" Width="80" Groupable="false" Fixed="true" Hideable="false">
                                <Template runat="server">
                                    <Html>
                                        <a href="javascript:TarihceGoster('{YIL}','{MUHASEBEKOD}','{HARCAMABIRIMIKOD}','{BELGENO}')">{DURUM}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:DateColumn runat="server" ColumnID="SONISLEMTARIHI" DataIndex="SONISLEMTARIHI" Header="<%$ Resources:TasinirMal, FRMZFS055 %>" Width="80" Format="dd.m.Y" />
                            <ext:Column ColumnID="SONISLEMYAPAN" DataIndex="SONISLEMYAPAN" Header="<%$ Resources:TasinirMal, FRMZFS056 %>" Width="100" />
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
