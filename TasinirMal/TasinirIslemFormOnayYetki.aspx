<%@ Page Language="C#" CodeBehind="TasinirIslemFormOnayYetki.aspx.cs" Inherits="TasinirMal.TasinirIslemFormOnayYetki" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        var secStore;
        var KullaniciEkle = function (store) {
            secStore = store;
            var adres = "ListePersonel.aspx?menuYok=1&cagiran=ext" + "&mb=" + txtMuhasebe.getValue() + "&hb=" + txtHarcamaBirimi.getValue();
            showPopWin(adres, 620, 420, true, null);
        }


        //Not: Değer mernis gelmiyor. Sicil no geliyor.
        var SecKapatDeger = function (deger, aciklama) {
            secStore.addRecord({ muhasebeKod: "", harcamaBirimKod: "", mernis: deger, adSoyad: aciklama, onayTur: "" });
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="AlanIsimleriniYaz();" />
        </Listeners>
    </ext:ResourceManager>
    <ext:Store ID="strListeA" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="muhasebeKod" />
                    <ext:RecordField Name="harcamaBirimKod" />
                    <ext:RecordField Name="mernis" />
                    <ext:RecordField Name="adSoyad" />
                    <ext:RecordField Name="onayTur" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="start" Value="0" Mode="Raw" />
            <ext:Parameter Name="limit" Value="250" Mode="Raw" />
        </BaseParams>
    </ext:Store>
    <ext:Store ID="strListeB" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="muhasebeKod" />
                    <ext:RecordField Name="harcamaBirimKod" />
                    <ext:RecordField Name="mernis" />
                    <ext:RecordField Name="adSoyad" />
                    <ext:RecordField Name="onayTur" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="start" Value="0" Mode="Raw" />
            <ext:Parameter Name="limit" Value="250" Mode="Raw" />
        </BaseParams>
    </ext:Store>
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;"
        Layout="BorderLayout">
        <Items>
            <ext:Panel ID="pnlAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                <Items>
                    <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Padding="10" LabelWidth="150"
                        Height="100">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar3" runat="server">
                                <Items>
                                    <ext:Button ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                                        <DirectEvents>
                                            <Click OnEvent="btnKaydet_Click">
                                                <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                <ExtraParams>
                                                    <ext:Parameter Name="jsonA" Value="Ext1.encode(grdListeA.getRowsValues())" Mode="Raw" />
                                                    <ext:Parameter Name="jsonB" Value="Ext1.encode(grdListeB.getRowsValues())" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnListele" runat="server" Text="Listele" Icon="ApplicationGo">
                                        <DirectEvents>
                                            <Click OnEvent="btnListele_Click">
                                                <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:CompositeField ID="CompositeField1" runat="server">
                                <Items>
                                    <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM011 %>"
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
                            <ext:CompositeField ID="CompositeField2" runat="server">
                                <Items>
                                    <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM013 %>"
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
                        </Items>
                    </ext:FormPanel>
                    <ext:Panel ID="pnlListe" runat="server" Layout="ColumnLayout" Region="Center" StyleSpec="background-color:white;padding-top:5px">
                        <Items>
                            <ext:GridPanel ID="grdListeA" runat="server" Region="Center" Title="A Onay Yetkisi"
                                Layout="FormLayout" StripeRows="true" TrackMouseOver="true" Border="true" StoreID="strListeA"
                                ForceFit="true" Width="400" Margins="5 0 5 5" Split="true" Cls="gridExt" StyleSpec="padding:5px;">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="btnKullaniciEkleA" runat="server" Icon="UserAdd" Text="Kullanici Ekle"
                                                OnClientClick="KullaniciEkle(strListeA);" />
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:RowNumbererColumn />
                                        <ext:Column ColumnID="adSoyad" DataIndex="adSoyad" Header="Ad Soyad">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="Sil">
                                                    <ToolTip Text="Çıkar" />
                                                </ext:ImageCommand>
                                            </Commands>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" CheckOnly="true" runat="server" />
                                </SelectionModel>
                                <Listeners>
                                    <Command Handler="strListeA.remove(record)" />
                                </Listeners>
                                <View>
                                    <ext:GridView ID="GridView1" runat="server" ForceFit="true" AutoFill="true" />
                                </View>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
                                        StoreID="strListeA" />
                                </BottomBar>
                            </ext:GridPanel>
                            <ext:GridPanel ID="grdListeB" runat="server" Region="East" Title="B Onay Yetkisi"
                                StripeRows="true" TrackMouseOver="true" Border="true" StoreID="strListeB" ForceFit="true"
                                Width="400" Margins="5 5 5 0" Split="true" Cls="gridExt" StyleSpec="padding-top:5px;padding-right:5px;padding-bottom:5px;padding-left:0px;">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Button ID="btnKullaniciEkle2" runat="server" Icon="UserAdd" Text="Kullanici Ekle"
                                                OnClientClick="KullaniciEkle(strListeB);" />
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:RowNumbererColumn />
                                        <ext:Column ColumnID="adSoyad" DataIndex="adSoyad" Header="Ad Soyad">
                                            <Commands>
                                                <ext:ImageCommand Icon="Delete" CommandName="Sil">
                                                    <ToolTip Text="Çıkar" />
                                                </ext:ImageCommand>
                                            </Commands>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" CheckOnly="true" runat="server" />
                                </SelectionModel>
                                <Listeners>
                                    <Command Handler="strListeB.remove(record)" />
                                </Listeners>
                                <View>
                                    <ext:GridView ID="GridView2" runat="server" ForceFit="true" AutoFill="true" />
                                </View>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="100" HideRefresh="true"
                                        StoreID="strListeB" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
