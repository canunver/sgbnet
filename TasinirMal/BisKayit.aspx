<%@ Page Language="C#" CodeBehind="BisKayit.aspx.cs" Inherits="TasinirMal.BisKayit" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        var SicilNoAc = function () {
            gridYazilacakSatirNo = strListe.data.length;

            var yil = pgFiltre.source["prpYil"];
            var muhasebeKod = pgFiltre.source["prpMuhasebe"];
            var harcamaBirimKod = pgFiltre.source["prpHarcamaBirimi"];
            var ambarKod = pgFiltre.source["prpAmbar"];
            var sicilNo = pgFiltre.source["prpSicilNo"];

            var adres = "ListeSicilNoYeni.aspx?menuYok=1&cagiran=grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:SICILNO:::::PRSICILNO";
            adres += "&yil=" + yil;
            adres += "&mb=" + muhasebeKod;
            adres += "&hb=" + harcamaBirimKod;
            adres += "&ak=" + ambarKod;
            adres += "&vermeDusme=-1";
            adres += "&listeTur=TEKSECIM";
            adres += "&sicilNoListe=" + sicilNo;

            showPopWin(adres, 800, 500, true, null);
        }

        var SecKapatDeger = function (deger) {
            alert(deger);
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" Split="true" Border="true"
                            Width="290">
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
                                        <ext:Button ID="btnListe" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click" Timeout="2400000">
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
                            </Source>
                            <Listeners>
                                <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                <SortChange Handler="this.getStore().sortInfo = undefined;" />
                            </Listeners>
                            <View>
                                <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                            </View>
                        </ext:PropertyGrid>
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" StoreID="strListe"
                            Border="true" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click" Before="strListe.commitChanges();">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.encode(#{grdListe}.getRowsValues(false, false, false))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnMalzemeEkle" runat="server" Text="Malzeme Ekle" Icon="Add">
                                            <Listeners>
                                                <Click Handler="SicilNoAc();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:ImageCommandColumn Width="22" Fixed="true">
                                        <Commands>
                                            <ext:ImageCommand Icon="Delete" CommandName="Sil">
                                                <ToolTip Text="Çıkar" />
                                            </ext:ImageCommand>
                                        </Commands>
                                    </ext:ImageCommandColumn>
                                    <ext:Column ColumnID="PRSICILNO" DataIndex="PRSICILNO" Header="PrSicil No" Width="100"
                                        Fixed="true" Hidden="true" />
                                    <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Header="Sicil No" Width="150"
                                        Fixed="true" />
                                    <ext:Column ColumnID="TASINIRHESAPKOD" DataIndex="TASINIRHESAPKOD" Header="Hesap Planı Kodu"
                                        Width="150" Fixed="true" />
                                    <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="Hesap Planı Adı" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                            <View>
                                <ext:GridView runat="server" ForceFit="true" AutoFill="true" />
                            </View>
                            <Listeners>
                                <Command Handler="strListe.remove(record)" />
                            </Listeners>
                            <BottomBar>
                                <ext:PagingToolbar ID="grdPagingToolbar" runat="server" PageSize="100" HideRefresh="true"
                                    StoreID="strListe">
                                    <Items>
                                        <ext:Button ID="btnSatirSil" runat="server" Text="Satır Sil" Icon="TableRowDelete">
                                            <Listeners>
                                                <Click Handler="#{grdListe}.deleteSelected();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
