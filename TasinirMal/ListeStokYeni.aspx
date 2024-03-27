<%@ Page Language="C#" CodeBehind="ListeStokYeni.aspx.cs" Inherits="TasinirMal.ListeStokYeni" EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= Resources.TasinirMal.FRMLST001 %></title>
    <script language="JavaScript" type="text/javascript">
        function HesapPlanKoduYapistir() {
            var yapistirmaBilgi = txtHesapPlan.getValue();
            var satirlar = yapistirmaBilgi.split("\n");

            var liste = [];
            var hBilgi = {};

            var hesapKodlari = "";
            for (var i = 0; i < satirlar.length; i++) {
                var bilgi = satirlar[i].split("\t");
                if (bilgi[0] != null && bilgi[0] != "") {

                    if (hesapKodlari != "") hesapKodlari += ";";
                    hesapKodlari += bilgi[0];

                    var miktar = "";
                    if (bilgi.length > 1)
                        miktar = bilgi[1];

                    var hBilgi = {
                        hesapKod: bilgi[0],
                        miktar: miktar
                    };

                    liste.push(hBilgi);
                }
            }


            Ext1.net.DirectMethod.request('Sorgula',
                {
                    success: function (result) {

                        for (var i = 0; i < liste.length; i++) {
                            SecKapatHazirla(liste[i].hesapKod, liste[i].miktar, false);
                        }

                        window.parent.hidePopWin();
                    },

                    failure: function (errorMsg) {
                        Ext1.Msg.alert('Hata', errorMsg);
                    },
                    params: { hesapKod: hesapKodlari },
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                }
            );
        }

        var onGridKomut = function (command, record, row, cellIndex) {
            SecKapat(record.data.HESAPPLANKOD, record.data.HESAPPLANADI);
        };

        function SecKapatHepsi() {

            extKontrol.getBody().mask("Malzemeler listeye aktarılıyor. Bu işlem uzun sürebilir. Lütfen bekleyin...");
            setTimeout("SecKapatHepsiIslem();", 100);
        }

        function SecKapatHepsiIslem() {
            var liste = [];

            for (var i = 0; i < strListe.data.length; i++) {
                ListeyeEkle(liste, strListe, i, strListe.data.items[i].data.MIKTAR);
            }

            CagiranaYaz(liste, true);

            extKontrol.getBody().unmask();
        }

        function SecKapat(hesapPlanKod) {
            Ext1.Msg.prompt("Miktar bilgisi", hesapPlanKod + " hesap koduna ait malzemeden<br>kullanmak istediğiniz miktarı yazınız.",
                function (btn, miktar) {
                    if (btn == 'ok') {
                        SecKapatHazirla(hesapPlanKod, miktar, true);
                    }
                }
            );
        }

        function SecKapatHazirla(hesapPlanKod, miktarGelen, pencereyiKapat) {
            miktarGelen = miktarGelen.replace(/\./g, '');
            miktarGelen = miktarGelen.replace(/,/g, '.');
            var miktar = parseFloat(miktarGelen);

            if (isNaN(miktar) || miktar == 0) {
                alert("Miktar bilgisini hatalı girdiniz.");
                return;
            }

            var liste = [];

            //seçilen hesap kodunu storedan arat. ilk bulunanı al
            //ilk bulunan miktardan kücük veya eşit ise ise bunu listeye yaz ve CagiranaYaz fonk. çağır
            //ilk bulunandan büyükse ilk bulunanı kendi miktarı ile listeye ekle ve kendi milkrarını isten miktardan çıkart ve sıradan liteyi aynı hesap kodu için dön

            for (var i = 0; i < strListe.data.length && miktar >= 0; i++) {

                if (hesapPlanKod.replace(/\./g, '') == strListe.data.items[i].data.HESAPPLANKOD.replace(/\./g, '') && miktar > 0) {

                    if (strListe.data.items[i].data.MIKTAR >= miktar) {
                        ListeyeEkle(liste, strListe, i, miktar)
                        miktar = 0;
                    }
                    if (strListe.data.items[i].data.MIKTAR < miktar) {
                        ListeyeEkle(liste, strListe, i, strListe.data.items[i].data.MIKTAR)
                        miktar = miktar - strListe.data.items[i].data.MIKTAR;
                    }
                }
            }

            CagiranaYaz(liste, pencereyiKapat);
        }

        function ListeyeEkle(liste, strListe, i, miktar) {
            var bilgi = {
                hesapPlanKod: strListe.data.items[i].data.HESAPPLANKOD,
                hesapPlanAd: strListe.data.items[i].data.HESAPPLANADI,
                kdvOran: strListe.data.items[i].data.KDVORAN,
                birimFiyat: strListe.data.items[i].data.BIRIMFIYAT,
                miktar: miktar,
                olcuBirim: strListe.data.items[i].data.OLCUBIRIM
            };

            liste.push(bilgi);
        }

        function CagiranaYaz(liste, pencereyiKapat) {
            if (liste.length == 0) return;

            var parentText = extKontrol.getCmp("hdnCagiran").getValue();
            if (parentText.indexOf(":") > -1) {
                var bilgiler = parentText.split(':');
                var kontrol = bilgiler[0];
                if (kontrol.indexOf("grd") == 0) {
                    var grid = window.parent.extKontrol.getCmp(bilgiler[0]);
                    var row = parent.gridYazilacakSatirNo;//Number(bilgiler[1]);

                    var adet = liste.length;
                    var store = grid.getStore();

                    GridSatirAc(store, adet);

                    var colKod = bilgiler[1];
                    var colAdi = "";
                    var colMiktar = "";
                    var colOlcu = "";
                    var colKDV = "";
                    var colFiyat = "";

                    if (bilgiler.length > 2) colAdi = bilgiler[2];
                    if (bilgiler.length > 3) colMiktar = bilgiler[3];
                    if (bilgiler.length > 4) colOlcu = bilgiler[4];
                    if (bilgiler.length > 5) colKDV = bilgiler[5];
                    if (bilgiler.length > 6) colFiyat = bilgiler[6];

                    for (var i = 0; i < adet; i++) {
                        if (colKod != "") store.getAt(row + i).set(colKod, liste[i].hesapPlanKod);
                        if (colAdi != "") store.getAt(row + i).set(colAdi, liste[i].hesapPlanAd);
                        if (colMiktar != "") store.getAt(row + i).set(colMiktar, liste[i].miktar);
                        if (colOlcu != "") store.getAt(row + i).set(colOlcu, liste[i].olcuBirim);
                        if (colKDV != "") store.getAt(row + i).set(colKDV, liste[i].kdvOran);
                        if (colFiyat != "") store.getAt(row + i).set(colFiyat, liste[i].birimFiyat.replace(",", "."));

                        parent.gridYazilacakSatirNo++;
                    }
                }
            }

            if (pencereyiKapat)
                window.parent.hidePopWin();
        }

        var showTip = function () {
            var rowIndex = grdListe.view.findRowIndex(this.triggerElement),
                cellIndex = grdListe.view.findCellIndex(this.triggerElement),
                record = strListe.getAt(rowIndex),
                fieldName = grdListe.getColumnModel().getDataIndex(cellIndex),
                data = record.get(fieldName);

            this.body.dom.innerHTML = data;
        };
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnSeciliSicilNo" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="HESAPPLANKOD" />
                        <ext:RecordField Name="HESAPPLANADI" />
                        <ext:RecordField Name="HESAPPLANKODADI" />
                        <ext:RecordField Name="MIKTAR" />
                        <ext:RecordField Name="KDVORAN" />
                        <ext:RecordField Name="BIRIMFIYAT" />
                        <ext:RecordField Name="OLCUBIRIM" />
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
                    Border="true" StoreID="strListe" Margins="5 5 5 0" Split="true" AutoExpandColumn="HESAPPLANKODADI" Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnHesapPlanModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                    <Listeners>
                                        <Click Handler="javascript:wndYapistir.show();return false;" />
                                    </Listeners>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnSecKapat" runat="server" Text="<%$ Resources:TasinirMal, FRMLST014 %>" Icon="PageAdd">
                                    <Listeners>
                                        <Click Handler="SecKapatHepsi();" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:ImageCommandColumn runat="server" Header="Seçim" Width="30">
                                <Commands>
                                    <ext:ImageCommand CommandName="SecKapat" Icon="Accept" />
                                </Commands>
                            </ext:ImageCommandColumn>
                            <ext:Column ColumnID="HESAPPLANKODADI" DataIndex="HESAPPLANKODADI" Header="<%$ Resources:TasinirMal, FRMLST002 %>" Sortable="false" Tooltip="{}" />
                            <ext:Column ColumnID="MIKTAR" DataIndex="MIKTAR" Header="<%$ Resources:TasinirMal, FRMLST004 %>" Align="Right" Width="60" Sortable="false" />
                            <ext:Column ColumnID="KDVORAN" DataIndex="KDVORAN" Header="<%$ Resources:TasinirMal, FRMLST005 %>" Align="Center" Width="40" Sortable="false" />
                            <ext:Column ColumnID="BIRIMFIYAT" DataIndex="BIRIMFIYAT" Header="<%$ Resources:TasinirMal, FRMLST006 %>" Align="Right" Width="100" Sortable="false" />
                        </Columns>
                    </ColumnModel>
                    <Listeners>
                        <Command Fn="onGridKomut" />
                    </Listeners>
                    <SelectionModel>
                        <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="250" HideRefresh="true"
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
        <ext:ToolTip
            ID="RowTip"
            runat="server"
            Target="={grdListe.getView().mainBody}"
            Delegate=".x-grid3-cell"
            TrackMouse="true">
            <Listeners>
                <Show Fn="showTip" />
            </Listeners>
        </ext:ToolTip>
        <ext:Window ID="wndYapistir" runat="server" Title="Yapıştır" Width="500" Height="350" Layout="FitLayout" Modal="true" Hidden="true">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnAktar" runat="server" Text="Aktar" Icon="PlayBlue">
                            <Listeners>
                                <Click Handler="HesapPlanKoduYapistir();"></Click>
                            </Listeners>
                        </ext:Button>
                        <ext:ToolbarFill runat="server" />
                        <ext:Button runat="server" Icon="Help" Text="Yardım">
                            <Listeners>
                                <Click Handler="javascript:PopupWin('../App_themes/images/ExceldenYapistirOrnek.png',450,450,'Yardim');" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:TextArea ID="txtHesapPlan" runat="server" AutoFocus="true" AutoFocusDelay="100" />
            </Items>
        </ext:Window>
    </form>
</body>
</html>
