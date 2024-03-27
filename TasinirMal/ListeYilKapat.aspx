<%@ Page Language="C#" Inherits="TasinirMal.ListeYilKapat" CodeBehind="ListeYilKapat.aspx.cs" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%= Resources.TasinirMal.FRMLYK001 %></title>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="HARCAMABIRIMI" />
                        <ext:RecordField Name="AMBAR" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Region="Center" AutoExpandColumn="AMBAR" Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:TriggerField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMORT003 %>"
                                    LabelWidth="40" Width="250" EnableKeyEvents="true">
                                    <Listeners>
                                        <KeyUp Handler="ApplyFilter('AMBAR');" />
                                        <TriggerClick Fn="TriggerClick" />
                                    </Listeners>
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Clear" />
                                    </Triggers>
                                </ext:TriggerField>
                                <ext:ToolbarFill />
                                <ext:RadioGroup runat="server">
                                    <Items>
                                        <ext:Radio ID="rdAcik" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMLYK006 %>" Width="300" />
                                        <ext:Radio ID="rdKapali" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMLYK007 %>" Width="300" />
                                    </Items>
                                </ext:RadioGroup>
                                <ext:ToolbarSeparator runat="server"></ext:ToolbarSeparator>
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal, FRMLYK008 %>" Icon="Magnifier">
                                    <DirectEvents>
                                        <Click OnEvent="btnListe_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:Column ColumnID="MUHASEBE" DataIndex="MUHASEBE" Header="<%$ Resources:TasinirMal, FRMLYK009 %>" Width="150" />
                            <ext:Column ColumnID="HARCAMABIRIMI" DataIndex="HARCAMABIRIMI" Header="<%$ Resources:TasinirMal, FRMLYK010 %>" Width="200" />
                            <ext:Column ColumnID="AMBAR" DataIndex="AMBAR" Header="<%$ Resources:TasinirMal, FRMLYK011 %>" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
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
                                    <SelectedItem Value="100" />
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
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
