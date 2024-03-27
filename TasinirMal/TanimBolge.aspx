<%@ Page Language="C#" CodeBehind="TanimBolge.aspx.cs" Inherits="TasinirMal.TanimBolge" EnableEventValidation="false" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Store ID="strListe" runat="server" SortOnLoad="true" PageSize="50">
            <Model>
                <ext:Model ID="Model1" runat="server">
                    <Fields>
                        <ext:ModelField Name="KOD" />
                        <ext:ModelField Name="AD" />
                    </Fields>
                </ext:Model>
            </Model>
            <Sorters>
                <ext:DataSorter Property="AD" Direction="ASC" />
            </Sorters>
        </ext:Store>
        <ext:Model runat="server" Name="mdlMuhasebe" IDProperty="KOD">
            <Fields>
                <ext:ModelField Name="KOD" />
                <ext:ModelField Name="AD" />
            </Fields>
        </ext:Model>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
            <LayoutConfig>
                <ext:VBoxLayoutConfig Align="Stretch" />
            </LayoutConfig>
            <Items>
                <ext:Panel ID="BorderLayoutPanel" runat="server" Flex="1" Layout="BorderLayout" StyleSpec="background-color:white;">
                    <Items>
                        <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" Header="false" TrackMouseOver="true"
                            Border="true" StoreID="strListe" Region="West" Width="300" Split="true" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:TextField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMORT003 %>"
                                            LabelWidth="40" Width="250" EnableKeyEvents="true">
                                            <Listeners>
                                                <KeyUp Handler="ApplyFilterV3('AD');" />
                                                <TriggerClick Handler="ClearFilterV3()" />
                                            </Listeners>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                            </Triggers>
                                        </ext:TextField>
                                        <ext:ToolbarFill />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="AD" DataIndex="AD" Header="Adı"
                                        Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                            <DirectEvents>
                                <CellClick OnEvent="SatirSecildi">
                                    <ExtraParams>
                                        <ext:Parameter Name="GRIDPARAM" Value="Ext.encode(App.grdListe.getRowsValues({selectedOnly:true}))"
                                            Mode="Raw" />
                                    </ExtraParams>
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </CellClick>
                            </DirectEvents>
                            <BottomBar>
                                <ext:PagingToolbar ID="grdPagingToolbar" runat="server" StoreID="strListe" HideRefresh="true">
                                    <Items>
                                        <ext:ComboBox ID="cmbGosterilecekSatir" runat="server" Width="45">
                                            <Items>
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                                <ext:ListItem Text="250" />
                                                <ext:ListItem Text="500" />
                                                <ext:ListItem Text="1000" />
                                            </Items>
                                            <SelectedItems>
                                                <ext:ListItem Value="50" />
                                            </SelectedItems>
                                            <Listeners>
                                                <Select Handler="App.grdListe.store.pageSize = parseInt(this.getValue(), 10); App.grdListe.store.load();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" BodyPadding="5">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMTIC012%>" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAra" runat="server" Text="<%$Resources:TasinirMal,FRMTIC014%>" Icon="Magnifier">
                                            <DirectEvents>
                                                <Click OnEvent="btnAra_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnSil" runat="server" Text="<%$Resources:TasinirMal,FRMTIC013%>" Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt silinecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMTIC015%>"
                                            Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:TextField ID="txtKod" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTBO009 %>" Width="300" LabelWidth="150" />
                                <ext:TextField ID="txtAd" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTBO010 %>" Width="300" LabelWidth="150" />
                                <ext:ItemSelector ID="lstMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTBO011 %>" MsgTarget="Side" LabelWidth="150" Height="200" Buttons="add,remove" ButtonsText='<%# new ItemSelectorButtonsText { Add = "Ekle", Remove = "Çıkart"} %>' AutoDataBind="true">
                                </ext:ItemSelector>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>

