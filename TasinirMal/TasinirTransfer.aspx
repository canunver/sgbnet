<%@ Page Language="C#" CodeBehind="TasinirTransfer.aspx.cs" Inherits="TasinirMal.TasinirTransfer" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Transfer İşlemi</title>
    <script language="JavaScript" type="text/javascript">
        function KimePenceresiAc() {
            var kimden = txtPersonel.getValue();
            if (kimden == "") {
                extKontrol.Msg.alert('Uyarı', "Kimden alanı boş bırakılamaz.");
                return;
            }

            wndTransfer.show();
        }

        var HesapPlanKodSelect = function (a, b) {
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
        }

        var KomutCalistir = function (command, record, row) {
            grdListe.getSelectionModel().selectRow(row);

            if (command == 'SicilNoListesiAc') {
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


                //Ext1.net.DirectMethods.EklenenSicilNolar(sicilNolar, {
                //    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                //});

                iliskiMalzemeEklemeDurumu = 1; //ilişkili malzemeleri listeye eklemek için var tanımlaması OrtakExt.js de yapıldı              
                ListeAc('ListeSicilNoYeni.aspx', 'grdListe:TASINIRHESAPKOD:TASINIRHESAPADI:SICILNO:::FISNO', '', 'TasinirTransfer');
            }
        };
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[3]);" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnBelgeTur" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="FIYAT" />
                        <ext:RecordField Name="GONMUHASEBEKOD" />
                        <ext:RecordField Name="GONHARACAMABIRIMKOD" />
                        <ext:RecordField Name="GONAMBARKOD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strHesapPlan" runat="server" AutoLoad="false" OnRefreshData="HesapStore_Refresh">
            <Proxy>
                <ext:PageProxy />
            </Proxy>
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="TASINIRHESAPKOD" />
                        <ext:RecordField Name="TASINIRHESAPADI" />
                        <ext:RecordField Name="FIYAT" />
                        <ext:RecordField Name="GONMUHASEBEKOD" />
                        <ext:RecordField Name="GONHARACAMABIRIMKOD" />
                        <ext:RecordField Name="GONAMBARKOD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="190">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                                    <DirectEvents>
                                        <Click OnEvent="btnKaydet_Click" Timeout="320000">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="json" Value="Ext1.encode(grdListe.getRowsValues())" Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:DateField ID="txtBelgeTarihi" runat="server" FieldLabel="Belge Tarihi" Width="100" />
                        <ext:CompositeField runat="server">
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
                        <ext:CompositeField runat="server">
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
                        <ext:CompositeField runat="server">
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
                        <ext:TextField ID="txtAciklama" runat="server" FieldLabel="Açıklama" AnchorHorizontal="90%" />
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="0 5 5 5" ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column ColumnID="PRSICILNO" DataIndex="PRSICILNO" Header="Pr Sicil No" Hidden="true" />
                            <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Header="<%$Resources:TasinirMal,FRMZFG011 %>" Width="190">
                                <Editor>
                                    <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="SICILNO" ValueField="SICILNO"
                                        TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                        HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                        StoreID="strHesapPlan">
                                        <Template runat="server">
                                            <Html>
                                                <tpl for=".">
                                                <div class="search-item">
                                                    <span>{TASINIRHESAPADI}<p>Sicil No:<b>{SICILNO}</b></p><p>Eski Sicil No:<b>{SICILNO}</b></p></span>
                                                </div>
                                            </tpl>
                                            </Html>
                                        </Template>
                                        <Listeners>
                                            <Select Fn="HesapPlanKodSelect" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Editor>
                                <%--  <Commands>
                                    <ext:ImageCommand Icon="Magnifier" CommandName="SicilNoListesiAc" />
                                </Commands>--%>
                            </ext:Column>
                            <ext:Column ColumnID="TASINIRHESAPADI" DataIndex="TASINIRHESAPADI" Header="<%$Resources:TasinirMal,FRMZFG013 %>" Width="200" />

                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server">
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="5000" HideRefresh="true"
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
                    <Listeners>
                        <Command Fn="KomutCalistir" />
                    </Listeners>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <iframe id="frmBelgeYazdir" frameborder="0" scrolling="no" width="1" height="1"></iframe>
    </form>
</body>
</html>
