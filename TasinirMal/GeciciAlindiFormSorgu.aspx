﻿<%@ Page Language="C#" CodeBehind="GeciciAlindiFormSorgu.aspx.cs" Inherits="TasinirMal.GeciciAlindiFormSorgu" %>

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
                        <ext:RecordField Name="YIL" />
                        <ext:RecordField Name="BELGENO" />
                        <ext:RecordField Name="BELGETARIHI" Type="Date" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="HARCAMABIRIMIKOD" />
                        <ext:RecordField Name="AMBARKOD" />
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="HARCAMABIRIMI" />
                        <ext:RecordField Name="AMBAR" />
                        <ext:RecordField Name="MUHASEBEAD" />
                        <ext:RecordField Name="HARCAMABIRIMIAD" />
                        <ext:RecordField Name="AMBARAD" />
                        <ext:RecordField Name="SONISLEMTARIHI" Type="Date" />
                        <ext:RecordField Name="SONISLEMYAPAN" />
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
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:Button ID="btnYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMGAS018 %>" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill />
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
                            <ext:Column ColumnID="YIL" DataIndex="YIL" Header="Yıl" Width="50" Hidden="true" />
                            <ext:DateColumn runat="server" ColumnID="BELGETARIHI" DataIndex="BELGETARIHI" Header="Belge Tarihi" Width="80" Format="dd.m.Y" />
                            <ext:Column ColumnID="MUHASEBE" DataIndex="MUHASEBE" Header="Muhasebe" Width="200" />
                            <ext:Column ColumnID="HARCAMABIRIMI" DataIndex="HARCAMABIRIMI" Header="Harcama Birimi" Width="200" />
                            <ext:Column ColumnID="AMBAR" DataIndex="AMBAR" Header="Ambar" Width="200" />
                            <ext:DateColumn runat="server" ColumnID="SONISLEMTARIHI" DataIndex="SONISLEMTARIHI" Header="İşlem Tarihi" Width="80" Format="dd.m.Y" />
                            <ext:Column ColumnID="SONISLEMYAPAN" DataIndex="SONISLEMYAPAN" Header="İşlem Yapan" Width="100" />
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