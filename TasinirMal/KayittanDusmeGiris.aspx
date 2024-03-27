<%@ Page Language="C#" CodeBehind="KayittanDusmeGiris.aspx.cs" Inherits="TasinirMal.KayittanDusmeGiris" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        var KomutCalistir = function (command, record, row) {
            if (command == 'StokListesiAc') {
                gridYazilacakSatirNo = row;//genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                grdListe.getSelectionModel().selectRow(row);
                ListeAc('ListeStokYeni.aspx', 'grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:MIKTAR:OLCUBIRIMIKOD:KDVORANI:BIRIMFIYATI', '', '');
            }
            if (command == 'SicilNoListesiAc') {
                gridYazilacakSatirNo = row;//genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                grdListe.getSelectionModel().selectRow(row);
                sicilListesiAmbarZimmetDurumu = "ambar";//genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                iliskiMalzemeEklemeDurumu = 1; //ilişkili malzemeleri listeye eklemek için var tanımlaması OrtakExt.js de yapıldı              
                ListeAc('ListeSicilNoYeni.aspx', 'grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:SICILNO:MIKTAR:OLCUBIRIMIKOD:KDVORANI:BIRIMFIYATI', '', '');
            }
        }

        function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtYil.setValue(yil);
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[3,4,5,6,7]);AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnTutanakNo" runat="server" />
        <ext:Hidden ID="hdnHesapKod" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="MIKTAR" Type="Float" />
                        <ext:RecordField Name="OLCUBIRIMIKOD" />
                        <ext:RecordField Name="KDVORANI" Type="Int" />
                        <ext:RecordField Name="BIRIMFIYATI" Type="Float" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="160">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>" Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click" Timeout="2400000">
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
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMGAG038%>" Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnBelgeYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMGAG036%>" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnBelgeYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnTifOlustur" runat="server" Text="<%$Resources:TasinirMal,FRMGAG037%>" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnTIF_Click" Timeout="2400000">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill />
                                <ext:Button ID="btnSil" runat="server" Text="<%$Resources:TasinirMal,FRMKDG030%>" Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnSil_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt silinecektir. Onaylıyor musunuz?" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Yıl, Tutanak Tarihi, Numara">
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:DateField ID="txtTutanakTarihi" runat="server" EmptyText="Tutanak Tarihi" Width="100" />
                                <ext:TriggerField ID="txtBelgeNo" runat="server" Width="100" EmptyText="Belge No">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <DirectEvents>
                                        <TriggerClick OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </TriggerClick>
                                        <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TriggerField>
                                <ext:Label ID="lblFormDurum" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG021 %>" Width="120">
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
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG023 %>" Width="120">
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
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMKDG025 %>" Width="120">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblAmbarAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true" ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column ColumnID="TASINIRHESAPKOD" DataIndex="TASINIRHESAPKOD" Header="<%$Resources:TasinirMal,FRMZFG010 %>" Width="150">
                                <Commands>
                                    <ext:ImageCommand CommandName="StokListesiAc" Icon="Magnifier" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Header="<%$Resources:TasinirMal,FRMKDG007 %>" Width="190">
                                <Commands>
                                    <ext:ImageCommand CommandName="SicilNoListesiAc" Icon="Magnifier" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="<%$Resources:TasinirMal,FRMZFG013 %>" Width="200" />
                            <ext:Column ColumnID="MIKTAR" DataIndex="MIKTAR" Header="<%$Resources:TasinirMal,FRMKDG008 %>" Align="Right" Width="80" />
                            <ext:Column ColumnID="OLCUBIRIMIKOD" DataIndex="OLCUBIRIMIKOD" Header="<%$Resources:TasinirMal,FRMKDG009 %>" Width="100" />
                            <ext:Column ColumnID="KDVORANI" DataIndex="KDVORANI" Header="<%$Resources:TasinirMal,FRMKDG010 %>" Align="Right" Width="80" />
                            <ext:Column ColumnID="BIRIMFIYATI" DataIndex="BIRIMFIYATI" Header="<%$Resources:TasinirMal,FRMZFG015 %>" Align="Right" Width="120" Tooltip="<%$Resources:TasinirMal,FRMZFG015 %>" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server">
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <Listeners>
                        <Command Fn="KomutCalistir" />
                    </Listeners>
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
                                <ext:Button ID="btnSatirAc" runat="server" Text="Satır Aç" Icon="TableRow">
                                    <Menu>
                                        <ext:Menu runat="server">
                                            <Items>
                                                <ext:MenuItem ID="MenuItem1" runat="server" Text="10" Icon="TableRow" OnClientClick="GridSatirAc(strListe,10)" />
                                                <ext:MenuItem ID="MenuItem3" runat="server" Text="20" Icon="TableRow" OnClientClick="GridSatirAc(strListe,20)" />
                                                <ext:MenuItem ID="MenuItem4" runat="server" Text="50" Icon="TableRow" OnClientClick="GridSatirAc(strListe,50)" />
                                                <ext:MenuItem ID="MenuItem5" runat="server" Text="100" Icon="TableRow" OnClientClick="GridSatirAc(strListe,100)" />
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:Button>
                                <ext:Button ID="btnSatirAcAraya" runat="server" Text="Araya Satır Aç" Icon="TableCell">
                                    <Listeners>
                                        <Click Handler="GridSatirAcAraya(grdListe);" />
                                    </Listeners>
                                </ext:Button>
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
        </ext:Viewport>
    </form>
</body>
</html>
