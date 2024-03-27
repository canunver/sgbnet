<%@ Page Language="C#" CodeBehind="ZimmetTransfer.aspx.cs" Inherits="TasinirMal.ZimmetTransfer" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Transfer İşlemi</title>
    <script language="JavaScript" type="text/javascript">
        function SicilNoAlaniGoster() {

            $(".birimBazli").hide();
            fldSicilNoBazli.show();
            hdnIslemTur.setValue("1");
        }

        function KimePenceresiAc() {
            var kimden = txtPersonel.getValue();
            var islemTuru = hdnIslemTur.getValue();

            if (islemTuru == "1") {
                var sicilNolar = txtSicilNolar.getValue();
                if (sicilNolar == "") {
                    extKontrol.Msg.alert('Uyarı', "Sicil Nolar alanı boş bırakılamaz.");
                    return;
                }
            }
            else {
                if (kimden == "") {
                    extKontrol.Msg.alert('Uyarı', "Kimden alanı boş bırakılamaz.");
                    return;
                }
            }

            wndTransfer.show();
        }

        var SicilNoSelect = function (a, b) {
            var store = grdListe.getStore();
            var rowIndex = store.indexOf(grdListe.getSelectionModel().getSelected());
            var satir = store.getAt(rowIndex);
            satir.set("SICILNO", b.data.SICILNO);
            satir.set("PRSICILNO", b.data.PRSICILNO);
            satir.set("TASINIRHESAPKOD", b.data.TASINIRHESAPKOD);
            satir.set("FIYAT", b.data.FIYAT);
            satir.set("TASINIRHESAPADI", b.data.TASINIRHESAPADI);
            satir.set("GONMUHASEBEKOD", b.data.GONMUHASEBEKOD);
            satir.set("GONHARACAMABIRIMKOD", b.data.GONHARACAMABIRIMKOD);
            satir.set("GONAMBARKOD", b.data.GONAMBARKOD);
            satir.set("BIRIMFIYATI", b.data.BIRIMFIYAT);
            satir.set("BULUNDUGUYER", b.data.BULUNDUGUYER);
            satir.set("DISSICILNO", b.data.ESICILNO);
        }

        var KomutCalistir = function (command, record, row) {
            if (command == "SatirSil") {
                grdListe.deleteRecord(record);
            }
        };

        function LocalIsimleriYaz() {
            KodAdGetir('PERSONEL', 'txtPersonel:txtMuhasebe:txtHarcamaBirimi', 'lblPersonelAd');
            KodAdGetir('PERSONEL', 'txtKimeVerildi:txtMuhasebe:txtHarcamaBirimi', 'lblKimeVerildi');
        }

        function IslemAyarla() {
            if (chkSadeceDusme.checked) {
                cmpNereye.hide();
                cmpKime.hide();
            }
            else {
                cmpNereye.show();
                cmpKime.show();
            }
        }

        var NereyeVerildiPenceresi = function (field, trigger, index) {
            ListeAc('ListeOdaMB.aspx', 'txtNereyeVerildi', 'txtHarcamaBirimi', 'lblNereyeVerildi');
        }

        var NeredenPenceresi = function (field, trigger, index) {
            ListeAc('ListeOdaMB.aspx', 'txtNereden', 'txtHarcamaBirimi', 'lblNereden');
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[2,4,5,6]);AlanIsimleriniYaz();LocalIsimleriYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnBelgeTur" runat="server" />
        <ext:Hidden ID="hdnIslemTur" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="FISNO" />
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="ACIKLAMA" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="KDVORANI" Type="Int" />
                        <ext:RecordField Name="BIRIMFIYATI" Type="Float" />
                        <ext:RecordField Name="TESLIMEDILMEANINDADURUMU" />
                        <ext:RecordField Name="DISSICILNO" />
                        <ext:RecordField Name="BULUNDUGUYER" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strSicilNo" runat="server" AutoLoad="false" OnRefreshData="SicilNoStore_Refresh">
            <Proxy>
                <ext:PageProxy />
            </Proxy>
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="ESICILNO" />
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="FIYAT" />
                        <ext:RecordField Name="GONMUHASEBEKOD" />
                        <ext:RecordField Name="GONHARACAMABIRIMKOD" />
                        <ext:RecordField Name="GONAMBARKOD" />
                        <ext:RecordField Name="BIRIMFIYAT" />
                        <ext:RecordField Name="BULUNDUGUYER" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="240">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnSonrakiAdim" runat="server" Text="Sonraki Adım" Icon="PlayGreen">
                                    <Listeners>
                                        <Click Handler="KimePenceresiAc();" />
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarFill runat="server"></ext:ToolbarFill>
                                <ext:Button ID="btnSicilNoBazli" runat="server" Text="Sicil No Bazlı" Icon="Application">
                                    <Listeners>
                                        <Click Handler="SicilNoAlaniGoster();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:DateField ID="txtBelgeTarihi" runat="server" FieldLabel="BelgeTarihi" Width="100" />
                        <ext:CompositeField runat="server" ItemCls="birimBazli">
                            <Items>
                                <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM011 %>" Width="120">
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
                        <ext:CompositeField runat="server" ItemCls="birimBazli">
                            <Items>
                                <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM013 %>" Width="120">
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
                        <ext:CompositeField runat="server" ItemCls="birimBazli">
                            <Items>
                                <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTIM031 %>" Width="120">
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
                        <ext:CompositeField runat="server" FieldLabel="Kimden" ItemCls="birimBazli">
                            <Items>
                                <ext:TriggerField ID="txtPersonel" runat="server" Width="150">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblPersonelAd" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel="Nereden" ItemCls="birimBazli">
                            <Items>
                                <ext:TriggerField ID="txtNereden" runat="server" Width="150">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="NeredenPenceresi" />
                                        <Change Fn="TriggerChange" />
                                    </Listeners>
                                </ext:TriggerField>
                                <ext:Label ID="lblNereden" runat="server" />
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel="Hesap Kodu" ItemCls="birimBazli">
                            <Items>
                                <ext:TextField ID="txtHesapKodu" runat="server" Width="150">
                                </ext:TextField>
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel=" " LabelSeparator=" " ItemCls="birimBazli">
                            <Items>
                                <ext:Button ID="btnZimmetListele" runat="server" Icon="Magnifier" Text="Kişi Üzerindeki Malzemeleri Listele">
                                    <DirectEvents>
                                        <Click OnEvent="btnZimmetListele_Click" Timeout="1200000">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:CompositeField>
                        <ext:CompositeField runat="server" FieldLabel="Sicil Nolar" ID="fldSicilNoBazli" Hidden="true">
                            <Items>
                                <ext:TextArea ID="txtSicilNolar" runat="server" Height="170" Width="470">
                                </ext:TextArea>
                            </Items>
                        </ext:CompositeField>
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="0 5 5 5" ClicksToEdit="1" Cls="gridExt birimBazli">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:ImageCommandColumn Width="22" Fixed="true">
                                <Commands>
                                    <ext:ImageCommand Icon="Delete" CommandName="SatirSil">
                                        <ToolTip Text="Çıkar" />
                                    </ext:ImageCommand>
                                </Commands>
                            </ext:ImageCommandColumn>
                            <ext:Column ColumnID="PRSICILNO" DataIndex="PRSICILNO" Header="Pr Sicil No" Hidden="true" />
                            <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Header="<%$Resources:TasinirMal,FRMZFG011 %>" Width="150">
                                <Editor>
                                    <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="SICILNO" ValueField="SICILNO"
                                        TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                        HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                        StoreID="strSicilNo">
                                        <Template runat="server">
                                            <Html>
                                                <tpl for=".">
                                                    <div class="search-item">
                                                        <span>
                                                            {TASINIRHESAPADI}<p>Sicil No:<b>{SICILNO}</b></p>
                                                            <p>Eski Sicil No:<b>{ESICILNO}</b></p>
                                                        </span>
                                                    </div>
                                                </tpl>
                                            </Html>
                                        </Template>
                                        <Listeners>
                                            <Select Fn="SicilNoSelect" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="DISSICILNO" DataIndex="DISSICILNO" Header="Eski Sicil No" Width="100" />
                            <ext:Column ColumnID="TASINIRHESAPKOD" DataIndex="TASINIRHESAPKOD" Header="<%$Resources:TasinirMal,FRMZFG010 %>" Width="150" Hidden="true" />
                            <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="<%$Resources:TasinirMal,FRMZFG013 %>" Width="200" />
                            <ext:Column ColumnID="BIRIMFIYATI" DataIndex="BIRIMFIYATI" Header="<%$Resources:TasinirMal,FRMZFG015 %>" Align="Right" Width="120" Tooltip="<%$Resources:TasinirMal,FRMZFG015 %>">
                                <Renderer Handler="return Ext1.util.Format.number(value, '0.000,00/i');" />
                            </ext:Column>
                            <ext:Column ColumnID="BULUNDUGUYER" DataIndex="BULUNDUGUYER" Header="Bulunduğu Yer" Width="200" />
                            <ext:Column ColumnID="ACIKLAMA" DataIndex="ACIKLAMA" Header="<%$Resources:TasinirMal,FRMZFG012 %>" Width="200">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="TESLIMEDILMEANINDADURUMU" DataIndex="TESLIMEDILMEANINDADURUMU" Header="<%$Resources:TasinirMal,FRMZFG016 %>" Width="150">
                                <Editor>
                                    <ext:TextField runat="server" />
                                </Editor>
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" />
                    </SelectionModel>
                    <Listeners>
                        <Command Fn="KomutCalistir" />
                    </Listeners>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="5000" HideRefresh="true"
                            StoreID="strListe">
                            <Items>
                                <ext:Button ID="btnSicilNoModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                    <Listeners>
                                        <Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:PRSICILNO:SICILNO:TASINIRHESAPKOD:TASINIRHESAPADI:BIRIMFIYATI:BULUNDUGUYER:DISSICILNO','','');return false;" />
                                        <%--<Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:OLCUBIRIMIKOD:KDVORANI:MIKTAR:BIRIMFIYATI','','');return false;" />--%>
                                    </Listeners>
                                </ext:Button>
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
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="wndTransfer" runat="server" Height="140" Width="480" Padding="5"
            Modal="true" AutoShow="false" Hidden="true" Title="Transfer Edilecek Kişi Seçimi ">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnBitir" runat="server" Text="İşlemi Tamamla" Icon="PlayBlue">
                            <DirectEvents>
                                <Click OnEvent="btnKaydet_Click" Timeout="1200000">
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <ExtraParams>
                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.encode(grdListe.getRowsValues())"
                                            Mode="Raw" />
                                    </ExtraParams>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:ToolbarFill runat="server" />
                        <ext:Checkbox ID="chkSadeceDusme" runat="server" BoxLabel="Sadece Düşme İşlemi Yap">
                            <Listeners>
                                <Check Handler="IslemAyarla();" />
                            </Listeners>
                        </ext:Checkbox>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:CompositeField ID="cmpKime" runat="server" FieldLabel="Kime Verilecek">
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
                <ext:CompositeField ID="cmpNereye" runat="server" FieldLabel="Nereye Verilecek">
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
        </ext:Window>
        <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1"></iframe>
    </form>
</body>
</html>
