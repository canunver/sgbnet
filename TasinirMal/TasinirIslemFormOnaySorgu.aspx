<%@ Page Language="C#" CodeBehind="TasinirIslemFormOnaySorgu.aspx.cs" Inherits="TasinirMal.TasinirIslemFormOnaySorgu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="YIL" />
                        <ext:RecordField Name="TUR" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="MUHASEBEAD" />
                        <ext:RecordField Name="HARCAMABIRIMIKOD" />
                        <ext:RecordField Name="HARCAMABIRIMIAD" />
                        <ext:RecordField Name="AMBARKOD" />
                        <ext:RecordField Name="AMBARAD" />
                        <ext:RecordField Name="BELGENO" />
                        <ext:RecordField Name="BELGETARIHI" Type="Date" />

                        <ext:RecordField Name="ONAYAGONDERIMTARIHI" Type="Date" />
                        <ext:RecordField Name="ONAYAGONDEREN" />
                        <ext:RecordField Name="BONAYTARIHI" Type="Date" />
                        <ext:RecordField Name="BONAYIVEREN" />
                        <ext:RecordField Name="AONAYTARIHI" Type="Date" />
                        <ext:RecordField Name="AONAYIVEREN" />
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
                                        <Click OnEvent="btnSorguTemizle_Click" Timeout="320000">
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
                        <ext:PropertyGridParameter Name="prpKisi" DisplayName="Kişi">
                        </ext:PropertyGridParameter>
                        <%--<ext:PropertyGridParameter Name="prpOnayTur" DisplayName="Onay Türü">
                            <Editor>
                                <ext:ComboBox runat="server">
                                    <Items>
                                        <ext:ListItem Value="0" Text="Hepsi" />
                                        <ext:ListItem Value="1" Text="A" />
                                        <ext:ListItem Value="2" Text="B" />
                                    </Items>
                                </ext:ComboBox>
                            </Editor>
                        </ext:PropertyGridParameter>--%>
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
                            <ext:Column ColumnID="BELGENO" DataIndex="BELGENO" Header="Belge No" Width="100" />
                            <ext:Column ColumnID="TUR" DataIndex="TUR" Header="Türü" Width="100" />
                            <ext:Column ColumnID="MUHASEBEAD" DataIndex="MUHASEBEAD" Header="Muhasebe" Width="200" Hidden="true" />
                            <ext:Column ColumnID="HARCAMABIRIMIAD" DataIndex="HARCAMABIRIMIAD" Header="Harcama Birimi" Width="200" Hidden="true" />
                            <ext:Column ColumnID="AMBARAD" DataIndex="AMBARAD" Header="Ambar" Width="200" Hidden="true" />

                            <ext:Column ColumnID="ONAYAGONDEREN" DataIndex="ONAYAGONDEREN" Header="Onaya Gönderen" Width="100" />
                            <ext:DateColumn ColumnID="ONAYAGONDERIMTARIHI" DataIndex="ONAYAGONDERIMTARIHI" Header="Onaya Gönderim Tarihi" Width="110" Format="dd.m.Y HH:mm" />

                            <ext:Column ColumnID="BONAYIVEREN" DataIndex="BONAYIVEREN" Header="B Onaylayan" Width="100" />
                            <ext:DateColumn ColumnID="BONAYTARIHI" DataIndex="BONAYTARIHI" Header="B Onay Tarihi" Width="110" Format="dd.m.Y HH:mm" />

                            <ext:Column ColumnID="AONAYIVEREN" DataIndex="AONAYIVEREN" Header="A Onaylayan" Width="100" />
                            <ext:DateColumn ColumnID="AONAYTARIHI" DataIndex="AONAYTARIHI" Header="A Onay Tarihi" Width="110" Format="dd.m.Y HH:mm" />
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
