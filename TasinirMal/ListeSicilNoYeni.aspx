<%@ Page Language="C#" CodeBehind="ListeSicilNoYeni.aspx.cs" Inherits="TasinirMal.ListeSicilNoYeni" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">

        var MaskAktar = function () { extKontrol.getBody().mask("Malzemeler listeye aktarılıyor. Bu işlem uzun sürebilir. Lütfen bekleyin..."); }

        function SecKapat() {
            if (grdListe.selModel.selections.items.length == 0) {
                return;
            }

            MaskAktar();
            setTimeout("SecKapatIslem();", 100);
        }

        var liste = [];
        function SecKapatIslem() {
            var bilgi = {}; //, liste = [];

            for (var i = 0; i < grdListe.selModel.selections.items.length; i++) {

                var bilgi = {
                    gorSicilNo: grdListe.selModel.selections.items[i].data.sicilno,
                    hesapPlanKod: grdListe.selModel.selections.items[i].data.kod,
                    hesapPlanAd: grdListe.selModel.selections.items[i].data.ad,
                    kdvOran: grdListe.selModel.selections.items[i].data.kdv,
                    birimFiyat: grdListe.selModel.selections.items[i].data.birimFiyat,
                    miktar: 1,
                    prSicilNo: grdListe.selModel.selections.items[i].data.prSicilNo
                };

                liste.push(bilgi);
            }

            if (liste.length == 0) return;

            if (hdnIliskiliMalzemeEkle.getValue() == "1") {
                Ext1.net.DirectMethods.IliskiliMalzemelerListeyeEklensinMi(Ext1.encode(grdListe.getRowsValues({ selectedOnly: true })), {
                    eventMask: { showMask: true }, timeout: 60000,
                    failure: function (errorMsg) { Ext1.Msg.alert('Hata Oluştu', errorMsg); }
                });
            }
            else
                GrdSatirYaz();
        }

        var GrdSatirYaz = function () {
            var parentText = extKontrol.getCmp("hdnCagiran").getValue();
            if (parentText.indexOf(":") > -1) {
                var bilgiler = parentText.split(':');
                var kontrol = bilgiler[0];
                if (kontrol.indexOf("grd") == 0) {
                    var grid = window.parent.extKontrol.getCmp(bilgiler[0]);
                    var row = parent.gridYazilacakSatirNo; //Number(bilgiler[1]);

                    var adet = liste.length;
                    var store = grid.getStore();

                    GridSatirAc(store, adet);

                    var colKod = bilgiler[1];
                    var colAdi = "";
                    var colSicilNo = "";
                    var colMiktar = "";
                    var colOlcu = "";
                    var colKDV = "";
                    var colFiyat = "";
                    var prSicilNo = "";

                    if (bilgiler.length > 2) colAdi = bilgiler[2];
                    if (bilgiler.length > 3) colSicilNo = bilgiler[3];
                    if (bilgiler.length > 4) colMiktar = bilgiler[4];
                    if (bilgiler.length > 5) colOlcu = bilgiler[5];
                    if (bilgiler.length > 6) colKDV = bilgiler[6];
                    if (bilgiler.length > 7) colFiyat = bilgiler[7];
                    if (bilgiler.length > 8) prSicilNo = bilgiler[8];

                    for (var i = 0; i < adet; i++) {
                        if (colKod != "") store.getAt(row + i).set(colKod, liste[i].hesapPlanKod);
                        if (colAdi != "") store.getAt(row + i).set(colAdi, liste[i].hesapPlanAd);
                        if (colSicilNo != "") store.getAt(row + i).set(colSicilNo, liste[i].gorSicilNo);
                        if (colMiktar != "") store.getAt(row + i).set(colMiktar, liste[i].miktar);
                        if (colOlcu != "") store.getAt(row + i).set(colOlcu, "Adet");
                        if (colKDV != "") store.getAt(row + i).set(colKDV, liste[i].kdvOran);
                        if (colFiyat != "") store.getAt(row + i).set(colFiyat, liste[i].birimFiyat);
                        if (prSicilNo != "") store.getAt(row + i).set(prSicilNo, liste[i].prSicilNo);
                    }
                }
                else {
                    //window.parent.SicilNoBilgiYaz(liste);
                    SecKapatDeger(liste[0].gorSicilNo, "");
                }
            }
            else {
                //window.parent.SicilNoBilgiYaz(liste);
                SecKapatDeger(liste[0].gorSicilNo, liste[0].prSicilNo + "|" + liste[0].hesapPlanAd);
            }

            window.parent.hidePopWin();
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnSeciliSicilNo" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Hidden ID="hdnIliskiliMalzemeEkle" runat="server" />
        <ext:Hidden ID="hdnZimmetTur" runat="server" />
        <ext:Hidden ID="hdnEngelliSicilNolar" runat="server" />
        <ext:Store ID="strListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
            RemotePaging="true" RemoteSort="true" OnRefreshData="strListe_Refresh" RemoteGroup="true">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="tip" />
                        <ext:RecordField Name="prSicilNo" />
                        <ext:RecordField Name="sicilno" />
                        <ext:RecordField Name="kod" />
                        <ext:RecordField Name="ad" />
                        <ext:RecordField Name="kimeGitti" />
                        <ext:RecordField Name="oda" />
                        <ext:RecordField Name="kaynakMuhasebe" />
                        <ext:RecordField Name="kaynakHB" />
                        <ext:RecordField Name="kaynakAmbar" />
                        <ext:RecordField Name="kaynakTC" />
                        <ext:RecordField Name="kaynakOda" />
                        <ext:RecordField Name="eskiSicilNo" />
                        <ext:RecordField Name="kdv" />
                        <ext:RecordField Name="birimFiyat" Type="Float" />
                        <ext:RecordField Name="marka" />
                        <ext:RecordField Name="model" />
                        <ext:RecordField Name="saseNo" />
                        <ext:RecordField Name="islemTipAdi" />
                        <ext:RecordField Name="islemDurumAciklama" />
                        <ext:RecordField Name="amortismanOran" Type="Float" />
                        <ext:RecordField Name="alimTarihi" />
                        <ext:RecordField Name="islemTarih" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <BaseParams>
                <ext:Parameter Name="start" Value="0" Mode="Raw" />
                <ext:Parameter Name="limit" Value="0" Mode="Raw" />
                <ext:Parameter Name="sort" Value="" />
                <ext:Parameter Name="dir" Value="ASC" />
            </BaseParams>
            <Proxy>
                <ext:PageProxy />
            </Proxy>
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
                        <ext:PropertyGridParameter Name="prpBelgeNoTif" DisplayName="Belge No TİF">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNoZimmet" DisplayName="Belge No Zimmet">
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
                        <ext:PropertyGridParameter Name="prpHesapAdi" DisplayName="Hesap Adı" />
                        <ext:PropertyGridParameter Name="prpKisiKod" DisplayName="TC Kimlik No">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpKisiKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpOdaKod" DisplayName="Oda Kodu">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpOdaKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpDurumKod" DisplayName="Durumu">
                            <Renderer Handler="if (value=='1') return 'Ambarda'; else if (value=='2') return 'Zimmette'; else return 'Hepsi';" />
                            <Editor>
                                <ext:ComboBox ID="dllDurum" runat="server" Editable="false">
                                    <Items>
                                        <ext:ListItem Text="Hepsi" Value="" />
                                        <ext:ListItem Text="Ambarda" Value="1" />
                                        <ext:ListItem Text="Zimmette" Value="2" />
                                    </Items>
                                </ext:ComboBox>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonMuhasebe" DisplayName="Hedef Muhasebe">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonMuhasebe',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonHarcamaBirimi" DisplayName="Hedef Harcama Birimi">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonHarcamaBirimi',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpGonAmbar" DisplayName="Hedef Ambara">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpGonAmbar',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpMarkaKod" DisplayName="Marka Adı">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpMarkaKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpModelKod" DisplayName="Model Adı">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpModelKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpSeriNo" DisplayName="Seri No" />
                        <ext:PropertyGridParameter Name="prpPlaka" DisplayName="Plaka" />
                        <ext:PropertyGridParameter Name="prpEserYayinAdi" DisplayName="Eser/Yayın Adı" />
                        <ext:PropertyGridParameter Name="prpEskiSicilNo" DisplayName="Eski Sicil No" />
                    </Source>
                    <Listeners>
                        <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                        <SortChange Handler="this.getStore().sortInfo = undefined;" />
                    </Listeners>
                    <View>
                        <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                    </View>
                </ext:PropertyGrid>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" StoreID="strListe" Margins="5 5 5 0" Split="true"
                    Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnIndir" runat="server" Text="Dosya İndir" Icon="ApplicationGet">
                                    <DirectEvents>
                                        <Click OnEvent="btnIndir_Click" IsUpload="true" Timeout="2400000">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnSecKapat" runat="server" Text="Seç ve Kapat" Icon="Accept">
                                    <Listeners>
                                        <Click Handler="SecKapat();"></Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                            <ExtraParams>
                                                <ext:Parameter Name="RAPORBILGI" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                    Mode="Raw" />
                                            </ExtraParams>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <LoadMask ShowMask="true" Msg="Lütfen Bekleyiniz..." />
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:Column ColumnID="tip" DataIndex="tip" Align="Left" Width="70" Header="Belge Türü" />
                            <ext:Column ColumnID="prSicilNo" DataIndex="prSicilNo" Hidden="true" Align="Left" />
                            <ext:Column ColumnID="sicilno" DataIndex="sicilno" Header="<%$ Resources:TasinirMal, FRMBRK037 %>"
                                Width="160">
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="kod" DataIndex="kod" Header="<%$ Resources:TasinirMal, FRMBRK038 %>"
                                Width="160">
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="ad" DataIndex="ad" Header="<%$ Resources:TasinirMal, FRMBRK039 %>"
                                Width="225">
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="eskiSicilNo" DataIndex="eskiSicilNo" Header="Eski Sicil No"
                                Width="100">
                                <Editor>
                                    <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="kdv" DataIndex="kdv" Header="KDV %" Align="Right" Width="80" />
                            <ext:NumberColumn ColumnID="amortismanOran" DataIndex="amortismanOran" Header="A.%" Align="Right" Width="80" Format="0.000,00/i" />
                            <ext:NumberColumn ColumnID="birimFiyat" DataIndex="birimFiyat" Header="Birim Fiyatı" Align="Right" Format="0.000,00/i"
                                Width="100" />
                            <ext:Column ColumnID="kimeGitti" DataIndex="kimeGitti" Header="<%$ Resources:TasinirMal, FRMBRK040 %>"
                                Width="120" />
                            <ext:Column ColumnID="oda" DataIndex="oda" Header="Oda" Width="100" />
                            <ext:Column ColumnID="marka" DataIndex="marka" Header="Marka" Width="100" />
                            <ext:Column ColumnID="model" DataIndex="model" Header="Model" Width="100" />
                            <ext:Column ColumnID="saseNo" DataIndex="saseNo" Header="Açıklama" Width="150" />
                            <ext:Column ColumnID="alimTarihi" DataIndex="alimTarihi" Header="Alım Tarihi" Width="100" />
                            <ext:Column ColumnID="islemTipAdi" DataIndex="islemTipAdi" Header="Son İşlem" Width="120" />
                            <ext:Column ColumnID="islemTarih" DataIndex="islemTarih" Header="Son İşlem Tarihi" Width="120" />
                            <ext:Column ColumnID="islemDurumAciklama" DataIndex="islemDurumAciklama" Header="Durum Açıklama" Width="200" />

                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:CheckboxSelectionModel runat="server" />
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true"
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
                                    <SelectedItem Value="250" />
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
