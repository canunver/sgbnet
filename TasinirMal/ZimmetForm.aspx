<%@ Page Language="C#" CodeBehind="ZimmetForm.aspx.cs" Inherits="TasinirMal.ZimmetForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        var KomutCalistir = function (command, record, row) {
            if (command == 'SicilNoListesiAc') {
                var vermeDusme = ddlBelgeTuru.getValue();
                if (vermeDusme == "1")
                    sicilListesiAmbarZimmetDurumu = "ambar"; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                else {
                    sicilListesiAmbarZimmetDurumu = "zimmet"; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                    sicilListesiAmbarZimmetDurumu += hdnBelgeTur.getValue() == "2" ? "ORT" : "";
                }

                sicilListesiKisiDurumu = txtKimeVerildi.getRawValue(); //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                sicilListesiOdaDurumu = txtNereyeVerildi.getRawValue(); //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı

                gridYazilacakSatirNo = row; //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                grdListe.getSelectionModel().selectRow(row);

                var store = grdListe.getStore();

                var sicilNolar = "";

                for (var i = 0; i < store.data.items.length; i++) {
                    var sicilNo = store.data.items[i].data.SICILNO;

                    if (sicilNo != "" && sicilNo != undefined) {
                        if (sicilNolar != "") sicilNolar += ",";
                        sicilNolar += "'" + store.data.items[i].data.SICILNO + "'";
                    }
                }


                Ext1.net.DirectMethods.EklenenSicilNolar(sicilNolar, {
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                });

                iliskiMalzemeEklemeDurumu = 1; //ilişkili malzemeleri listeye eklemek için var tanımlaması OrtakExt.js de yapıldı              
                ListeAc('ListeSicilNoYeni.aspx', 'grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:SICILNO:::KDVORANI:BIRIMFIYATI', '', '');
            }
        }

        var NereyeVerildiPenceresi = function (field, trigger, index) {

            var vermeDusme = ddlBelgeTuru.getValue();

            sicilListesiKisiDurumu = "";
            if (vermeDusme != "1") {
                sicilListesiKisiDurumu = txtKimeVerildi.getRawValue(); //genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
                ListeAc('ListeOda.aspx', 'txtNereyeVerildi', 'txtHarcamaBirimi', 'lblNereyeVerildi');
            }
            else {
                ListeAc('ListeOdaMB.aspx', 'txtNereyeVerildi', 'txtHarcamaBirimi', 'lblNereyeVerildi');
            }
        }

        function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtYil.setValue(yil);
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }

        function BarkodEkraniAc() {
            var yil = txtYil.getValue();
            var muhasebeKod = txtMuhasebe.getValue();
            var harcamaBirimKod = txtHarcamaBirimi.getValue();
            var ambarKod = txtAmbar.getValue();
            var belgeNo = PadLeft(txtBelgeNo.getValue(), '0', 6);
            var belgeTur = hdnBelgeTur.getValue() == "1" ? "ZIM" : "ORT";

            var adres = "BarkodYazdir.aspx?bTur=" + belgeTur + "&y=" + yil + "&m=" + muhasebeKod + "&h=" + harcamaBirimKod + "&a=" + ambarKod + "&b=" + belgeNo + "&menuYok=1";
            showPopWin(adres, 800, 500, true, null);
        }

        function TransferEkraniAc() {
            var belgeTur = hdnBelgeTur.getValue();

            showPopWin("ZimmetTransfer.aspx?menuYok=1&Tur=" + belgeTur, 960, 560, true, null);
        }
        function LocalIsimleriYaz() {
            KodAdGetir('PERSONEL', 'txtKimeVerildi:txtMuhasebe:txtHarcamaBirimi', 'lblKimeVerildi');
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[2,3,4,5]);AlanIsimleriniYaz();LocalIsimleriYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnBelgeTur" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="ACIKLAMA" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="KDVORANI" Type="Int" />
                        <ext:RecordField Name="BIRIMFIYATI" Type="Float" />
                        <ext:RecordField Name="TESLIMEDILMEANINDADURUMU" />
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
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10"
                    Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150"
                    Height="260">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>"
                                    Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click">
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
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMZFG051%>"
                                    Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:SplitButton ID="btnBelgeYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMZFG050%>"
                                    Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnBelgeYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                    <Menu>
                                        <ext:Menu ID="Menu1" runat="server">
                                            <Items>
                                                <ext:Checkbox ID="chkResim" runat="server" BoxLabel="Resim ile yazdır" />
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:SplitButton>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnBarkodYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMZFG052%>"
                                    Icon="TagBlue">
                                    <Listeners>
                                        <Click Handler="BarkodEkraniAc();return false;"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnTransfer" runat="server" Text="Transfer" Icon="PackageGo">
                                    <Listeners>
                                        <Click Handler="TransferEkraniAc();"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMZFS046%>"
                                    Icon="Tick">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayla_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS047%>"
                                    Icon="Cross">
                                    <DirectEvents>
                                        <Click OnEvent="btnOnayKaldir_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belgenin onayı kaldırılacaktır. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator runat="server" />
                                <ext:Button ID="btnBelgeIptal" runat="server" Text="<%$Resources:TasinirMal,FRMZFS048%>"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnIptal_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge iptal edilecektir. Bu işlemi onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Yıl, Belge Tarihi, Belge No">
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>"
                                    Width="60" />
                                <ext:DateField ID="txtBelgeTarihi" runat="server" EmptyText="Belge Tarihi" Width="100" />
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
                        <ext:CompositeField runat="server">
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
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTIM031 %>"
                                    Width="120">
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
                        <ext:ComboBox ID="ddlBelgeTuru" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMZFG043 %>"
                            Width="150" StoreID="strBelgeTuru" ValueField="KOD" DisplayField="ADI" />
                        <ext:ComboBox ID="ddlBelgeTipi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMZFG044 %>"
                            Width="150" StoreID="strBelgeTipi" ValueField="KOD" DisplayField="ADI" />
                        <ext:CompositeField runat="server" FieldLabel="<%$Resources:TasinirMal,FRMZFG045 %>">
                            <Items>
                                <ext:TriggerField ID="txtKimeVerildi" runat="server" Width="150">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblKimeVerildi" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel="<%$Resources:TasinirMal,FRMZFG047 %>">
                            <Items>
                                <ext:TriggerField ID="txtNereyeVerildi" runat="server" Width="150">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="NereyeVerildiPenceresi" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblNereyeVerildi" runat="server" />
                            </Items>
                        </ext:CompositeField>
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true"
                    Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Header="<%$Resources:TasinirMal,FRMZFG011 %>"
                                Width="190">
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                                <Commands>
                                    <ext:ImageCommand CommandName="SicilNoListesiAc" Icon="Magnifier" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="TASINIRHESAPKOD" DataIndex="TASINIRHESAPKOD" Header="<%$Resources:TasinirMal,FRMZFG010 %>"
                                Width="150" />
                            <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="<%$Resources:TasinirMal,FRMZFG013 %>"
                                Width="200" />
                            <ext:Column ColumnID="KDVORANI" DataIndex="KDVORANI" Header="<%$Resources:TasinirMal,FRMZFG014 %>"
                                Align="Right" Width="80" />
                            <ext:Column ColumnID="BIRIMFIYATI" DataIndex="BIRIMFIYATI" Header="<%$Resources:TasinirMal,FRMZFG015 %>"
                                Align="Right" Width="120" Tooltip="<%$Resources:TasinirMal,FRMZFG015 %>" />
                            <ext:Column ColumnID="ACIKLAMA" DataIndex="ACIKLAMA" Header="<%$Resources:TasinirMal,FRMZFG012 %>"
                                Width="200">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="TESLIMEDILMEANINDADURUMU" DataIndex="TESLIMEDILMEANINDADURUMU"
                                Header="<%$Resources:TasinirMal,FRMZFG016 %>" Width="200">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
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
                                <ext:Button ID="btnHesapPlanModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                    <Listeners>
                                        <%--<Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:hesapPlanKod:hesapPlanAd:olcuBirimAd:kdvOran:miktar:birimFiyat','','');return false;" />--%>
                                        <Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:SICILNO:TASINIRHESAPKOD:TASINIRHESAPADI:BIRIMFIYATI','','');return false;" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1"></iframe>
    </form>
</body>
</html>
