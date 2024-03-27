<%@ Page Language="C#" CodeBehind="SicilNoOzellik.aspx.cs" Inherits="TasinirMal.SicilNoOzellik" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMSCO001 %></title>
    <script language="javascript" type="text/javascript">
        var aktifSatir = 0;

        function AktifSatirAta(a, b, c) {
            aktifSatir = b;
        }

        function filter(a) {
            strModel.filterBy(filterFunction);
        }
        function filterFunction(opRecord) {
            var grid = grdListe;
            index = aktifSatir;
            var deger = grid.store.data.items[index].get('MARKAKOD');
            return opRecord.data.MARKAKOD == deger;
        }

        var aktifPrSicilNo = "";

        var KomutCalistir = function (command, record, row) {
            grdListe.getSelectionModel().selectRow(row);
            var c = grdListe.getColumnModel().getColumnById(command);
            if (command == 'TasinirOzellikEkrani') {

                if (record.data.PRSICILNO == "" || record.data.PRSICILNO == undefined) {
                    extKontrol.Msg.alert("Uyarı", "Listeden özellikleri gösterilecek taşınır seçilmemiş.").Show();
                    return;
                }

                var adres = "TasinirOzellik.aspx?menuYok=1";
                adres += "&muhasebeKod=" + pgFiltre.source["prpMuhasebe"];
                adres += "&harcamaBirimKod=" + pgFiltre.source["prpHarcamaBirimi"];
                adres += "&ambarKod=" + pgFiltre.source["prpAmbar"];
                adres += "&prSicilNo=" + record.data.PRSICILNO;

                window.top.showPopWin(adres, 800, 600, true, null);
            }
            else if (command == 'hesapPlanKod') {

                if (record.data.PRSICILNO == "" || record.data.PRSICILNO == undefined) {
                    extKontrol.Msg.alert("Uyarı", "Listeden özellikleri gösterilecek kayıt seçilmemiş.").Show();
                    return;
                }

                aktifPrSicilNo = record.data.PRSICILNO;
                txtHesapPlanKodEski.setValue(record.data.HESAPPLANKOD);
                lblHesapPlanAdEski.setText(record.data.HESAPPLANAD);
                txtHesapPlanKod.setValue('');
                lblHesapPlanAd.setText('');

                wndHesapPlanKodDegistir.show();
            }
        };

        var wndHesapPlanKodDegistirGizle = function () {
            aktifPrSicilNo = "";
            var store = grdListe.getStore();
            var rowIndex = store.indexOf(grdListe.getSelectionModel().getSelected());
            var satir = store.getAt(rowIndex);
            satir.set("HESAPPLANKOD", txtHesapPlanKod.getValue());
            satir.set("HESAPPLANAD", lblHesapPlanAd.getText());
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="GridKilitle(grdListe,[2,3,4,5,6,7]);" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden runat="server" />
        <ext:Store ID="strListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
            RemotePaging="true" RemoteSort="true" OnRefreshData="strListe_Refresh" RemoteGroup="true" WarningOnDirty="false">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="HESAPPLANKOD" />
                        <ext:RecordField Name="HESAPPLANAD" />
                        <ext:RecordField Name="MARKAKOD" />
                        <ext:RecordField Name="MODELKOD" />
                        <ext:RecordField Name="ACIKLAMASERINO" />
                        <ext:RecordField Name="DISSICILNO" />
                        <ext:RecordField Name="ESERADI" />
                        <ext:RecordField Name="MUZEYEGELISTARIHI" />
                        <ext:RecordField Name="NEREDEBULUNDUGU" />
                        <ext:RecordField Name="CAGI" />
                        <ext:RecordField Name="DURUMUYAPIMMADDESI" />
                        <ext:RecordField Name="ONYUZU" />
                        <ext:RecordField Name="ARKAYUZU" />
                        <ext:RecordField Name="PUANI" />
                        <ext:RecordField Name="MUZEARSIVDEKIYERI" />
                        <ext:RecordField Name="CILTNO" />
                        <ext:RecordField Name="DIL" />
                        <ext:RecordField Name="YAZARCEVIRMENHATTATADI" />
                        <ext:RecordField Name="YAYINBASIMYERI" />
                        <ext:RecordField Name="YAYINBASIMTARIHI" />
                        <ext:RecordField Name="NEREDENGELDIGI" />
                        <ext:RecordField Name="AGIRLIGIBOYUTLARI" />
                        <ext:RecordField Name="BOYUTLARI" />
                        <ext:RecordField Name="SATIRSAYISI" />
                        <ext:RecordField Name="YAPRAKSAYISI" />
                        <ext:RecordField Name="SAYFASAYISI" />
                        <ext:RecordField Name="YERIKONUSU" />
                        <ext:RecordField Name="DISSICILNO2" />
                        <ext:RecordField Name="BISNO" />
                        <ext:RecordField Name="ESKIBISNO1" />
                        <ext:RecordField Name="ESKIBISNO2" />
                        <ext:RecordField Name="RFIDETIKETTURKOD" />
                        <ext:RecordField Name="AMORTISMANYILI" />
                        <ext:RecordField Name="AMORTISMANBITTI" />
                        <ext:RecordField Name="GIAI" />
                        <ext:RecordField Name="EKNO" />
                        <ext:RecordField Name="BULUNDUGUYER" />
                        <ext:RecordField Name="SICILNOOZELLIKYAPISTIR" Type="Int" />
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
        <ext:Store ID="strGosterilecekAlanlar" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strMarka" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strModel" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="MARKAKOD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" Split="true" Border="true" Width="250">
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
                                        <ext:Button ID="btnListele" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
                                <ext:PropertyGridParameter Name="prpMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMSCO046 %>">
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
                                <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMSCO048 %>">
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
                                <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMSCO050 %>">
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
                                <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                                    <Editor>
                                        <ext:SpinnerField runat="server">
                                        </ext:SpinnerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeNoTif" DisplayName="Belge No TİF">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeNoZimmet" DisplayName="Belge No Zimmet">
                                </ext:PropertyGridParameter>
                                <%-- <ext:PropertyGridParameter Name="prpBelgeNo" DisplayName="Belge No ">
                                </ext:PropertyGridParameter>--%>
                                <%--                                <ext:PropertyGridParameter Name="prpListedeGosterilecekler" DisplayName="<%$ Resources:TasinirMal, FRMSCO054 %>">
                                    <Renderer Handler="return PropertyRenderer(strGosterilecekAlanlar,value);" />
                                    <Editor>
                                        <ext:ComboBox runat="server" StoreID="strGosterilecekAlanlar" ValueField="KOD" DisplayField="ADI">
                                        </ext:ComboBox>
                                    </Editor>
                                </ext:PropertyGridParameter>--%>
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
                            Border="true" ClicksToEdit="1" Cls="gridExt">
                            <LoadMask ShowMask="true" Msg="Lütfen Bekleyiniz..." />
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:SplitButton ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />

                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.util.JSON.encode(#{grdListe}.getRowsValues())"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                            <Menu>
                                                <ext:Menu ID="Menu1" runat="server">
                                                    <Items>
                                                        <ext:Checkbox ID="chkKaydet" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMSCO058 %>" />
                                                    </Items>
                                                </ext:Menu>
                                            </Menu>
                                        </ext:SplitButton>
                                        <%-- <ext:ToolbarSeparator runat="server" />
                                        <ext:Button ID="btnSil" runat="server" Text="Sil" Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt özellikleri silinecektir. Onaylıyor musunuz?" />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.util.JSON.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>--%>
                                        <ext:ToolbarSeparator runat="server" />
                                        <ext:Button ID="btnYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTDD028%>"
                                            Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.util.JSON.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>

                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator runat="server" Hidden="true" />
                                        <ext:Button ID="btnSeriNoYukle" runat="server" Text="Seri No Yükle" Icon="Add" Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnSeriNoYukle_Click">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="SATIRLAR" Value="Ext1.util.JSON.encode(#{grdListe}.getRowsValues())"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                    <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator runat="server" Hidden="true" />
                                        <ext:FileUploadField ID="btnDosyaYukle" runat="server" ButtonOnly="true" Icon="PageWhiteAdd"
                                            ButtonText="<%$Resources:TasinirMal,FRMTIG121 %>" Width="90" Hidden="true">
                                        </ext:FileUploadField>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:LinkButton runat="server" Text="Marka Giriş Formu">
                                            <Listeners>
                                                <Click Handler="javascript:showPopWin('TanimMarka.aspx?menuYok=1',620,500,true,null);" />
                                            </Listeners>
                                        </ext:LinkButton>
                                        <ext:ToolbarSeparator runat="server" />
                                        <ext:LinkButton runat="server" Text="Model Giriş Formu">
                                            <Listeners>
                                                <Click Handler="javascript:showPopWin('TanimModel.aspx?menuYok=1',620,500,true,null);" />
                                            </Listeners>
                                        </ext:LinkButton>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn runat="server" />
                                    <ext:Column DataIndex="" Width="25">
                                        <Commands>
                                            <ext:ImageCommand CommandName="TasinirOzellikEkrani" Icon="Magnifier" />
                                        </Commands>
                                    </ext:Column>

                                    <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Width="130" Header="Sicil Numarası" />
                                    <ext:Column ColumnID="DISSICILNO" DataIndex="DISSICILNO" Header="Eski Sicil No">
                                    </ext:Column>
                                    <ext:Column ColumnID="HESAPPLANKOD" DataIndex="HESAPPLANKOD" Width="120" Header="Hesap Plan Kodu">
                                        <Commands>
                                            <ext:ImageCommand Icon="TableEdit" CommandName="hesapPlanKod" ToolTip-Title="Hesap plan kodunu değiştir" />
                                        </Commands>
                                    </ext:Column>
                                    <ext:Column ColumnID="HESAPPLANAD" DataIndex="HESAPPLANAD" Width="120" Header="Hesap Plan Adı" />
                                    <ext:Column ColumnID="BULUNDUGUYER" DataIndex="BULUNDUGUYER" Width="120" Header="Bulunduğu Yer" />
                                    <ext:Column ColumnID="MARKAKOD" DataIndex="MARKAKOD" Header="Marka">
                                        <Renderer Handler="return PropertyRenderer(strMarka,value);" />
                                        <Editor>
                                            <ext:ComboBox ID="ddlMarka" runat="server" StoreID="strMarka" ValueField="KOD" DisplayField="ADI" Editable="true" Mode="Local" TriggerAction="All" ForceSelection="true" SelectOnFocus="true">
                                            </ext:ComboBox>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="MODELKOD" DataIndex="MODELKOD" Header="Model">
                                        <Renderer Handler="return PropertyRenderer(strModel,value);" />
                                        <Editor>
                                            <ext:ComboBox runat="server" StoreID="strModel" ValueField="KOD" DisplayField="ADI" Editable="true" Mode="Local" TriggerAction="All" ForceSelection="true" SelectOnFocus="true">
                                                <Listeners>
                                                    <Expand Fn="filter" />
                                                </Listeners>
                                            </ext:ComboBox>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="GIAI" DataIndex="GIAI" Header="Tanımı" Width="120">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="EKNO" DataIndex="EKNO" Header="Ek No" Width="120">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="ACIKLAMASERINO" DataIndex="ACIKLAMASERINO" Header="Açıklama / Seri No" Width="150">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>

                                    <ext:Column ColumnID="MUZEYEGELISTARIHI" DataIndex="MUZEYEGELISTARIHI" Header="Müzeye Geliş Tarihi">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="MUZEARSIVDEKIYERI" DataIndex="MUZEARSIVDEKIYERI" Header="Müze/Arşivdeki Yeri">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="NEREDEBULUNDUGU" DataIndex="NEREDEBULUNDUGU" Header="Nerede Bulunduğu">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="CAGI" DataIndex="CAGI" Header="Çağı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="AGIRLIGIBOYUTLARI" DataIndex="AGIRLIGIBOYUTLARI" Header="Ağırlığı/Boyutları">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="ONYUZU" DataIndex="ONYUZU" Header="Ön Yüzü">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="ARKAYUZU" DataIndex="ARKAYUZU" Header="Arka Yüzü">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="DURUMUYAPIMMADDESI" DataIndex="DURUMUYAPIMMADDESI" Header="Durumu/Yapım Maddesi">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="PUANI" DataIndex="PUANI" Header="Puanı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>

                                    <ext:Column ColumnID="ESERADI" DataIndex="ESERADI" Header="Eser Adı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="CILTNO" DataIndex="CILTNO" Header="Cilt No">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="DIL" DataIndex="DIL" Header="Dil">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="BOYUTLARI" DataIndex="BOYUTLARI" Header="Boyutları">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="YERIKONUSU" DataIndex="YERIKONUSU" Header="Yeri/Konusu">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="YAZARCEVIRMENHATTATADI" DataIndex="YAZARCEVIRMENHATTATADI" Header="Yazar/Çevirmen/Hattat">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="YAYINBASIMYERI" DataIndex="YAYINBASIMYERI" Header="Yayın/Basım Yeri">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="YAYINBASIMTARIHI" DataIndex="YAYINBASIMTARIHI" Header="Yayın/Basım Tarihi">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="SATIRSAYISI" DataIndex="SATIRSAYISI" Header="Satır Sayısı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="YAPRAKSAYISI" DataIndex="YAPRAKSAYISI" Header="Yaprak Sayısı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="SAYFASAYISI" DataIndex="SAYFASAYISI" Header="Sayfa Sayısı">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="NEREDENGELDIGI" DataIndex="NEREDENGELDIGI" Header="Nereden Geldiği">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="AMORTISMANYILI" DataIndex="AMORTISMANYILI" Header="Amorti Yılı" Width="100">
                                        <%-- <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>--%>
                                    </ext:Column>
                                    <%--<ext:CheckColumn DataIndex="AMORTISMANBITTI" Header="Amortisman Bitti" Editable="true" Width="100" />--%>
                                    <ext:Column ColumnID="SICILNOOZELLIKYAPISTIR" DataIndex="SICILNOOZELLIKYAPISTIR" Header="SICILNOOZELLIKYAPISTIR" Hidden="true" />
                                    <ext:Column ColumnID="PRSICILNO" DataIndex="PRSICILNO" Header="PRSICILNO" Hidden="true" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server">
                                </ext:CheckboxSelectionModel>
                            </SelectionModel>
                            <Listeners>
                                <Command Fn="KomutCalistir" />
                                <RowClick Fn="AktifSatirAta" />
                            </Listeners>
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
                                        <ext:Button ID="btnHesapPlanModal" runat="server" Text="Yapıştır" Icon="PastePlain">
                                            <Listeners>
                                                <Click Handler="javascript:ListeAc('GridYapistir.aspx','grdListe:SICILNOOZELLIKYAPISTIR:PRSICILNO:SICILNO:HESAPPLANKOD:HESAPPLANAD:ACIKLAMASERINO','','');return false;" />
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
        <ext:Window ID="wndHesapPlanKodDegistir" runat="server" Height="200" Width="660" Padding="5"
            Modal="true" AutoShow="false" Hidden="true" Title="Hesap Plan Kodu Değiştir">
            <Listeners>
                <Hide Handler="aktifPrSicilNo = '';" />
            </Listeners>
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnHesapPlanKodDegistir" runat="server" Text="Kaydet" Icon="DiskBlack">
                            <DirectEvents>
                                <Click OnEvent="btnHesapPlanKodDegistir_Click">
                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    <ExtraParams>
                                        <ext:Parameter Name="PRSICILNO" Value="aktifPrSicilNo" Mode="Raw" />
                                    </ExtraParams>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:CompositeField ID="cmpHesapPlanKodEski" runat="server" LabelWidth="135" FieldLabel="Eski Hesap Plan Kodu">
                    <Items>
                        <ext:TriggerField ID="txtHesapPlanKodEski" runat="server" Width="150" ReadOnly="true" />
                        <ext:Label ID="lblHesapPlanAdEski" runat="server" />
                    </Items>
                </ext:CompositeField>
                <ext:CompositeField ID="cmpHesapPlanKodYeni" runat="server" LabelWidth="135" FieldLabel="Yeni Hesap Plan Kodu">
                    <Items>
                        <ext:TriggerField ID="txtHesapPlanKod" runat="server" Width="150">
                            <Triggers>
                                <ext:FieldTrigger Icon="Search" />
                            </Triggers>
                            <Listeners>
                                <TriggerClick Fn="TriggerClick" />
                                <Change Fn="TriggerChange" />
                            </Listeners>
                        </ext:TriggerField>
                        <ext:Label ID="lblHesapPlanAd" runat="server" />
                    </Items>
                </ext:CompositeField>
                <ext:Panel runat="server" StyleSpec="padding:5px;" Border="false" Frame="true">
                    <Content>
                        <div style="font-size: 13px;">
                            <b>Hesap Plan kodu değişikliğinde,</b>
                            <ul>
                                <li>Amortisman işlemi <b>başlatılmış</b> ise, amortisman ömründe değişiklik <b>yapılmayacaktır.</b></li>
                                <li>Amortisman işlemi <b>başlatılmamış</b> ise, amortisman ömrü yeni hesap plan koduna göre <b>değişecektir.</b></li>
                            </ul>
                        </div>
                    </Content>
                </ext:Panel>
            </Items>
        </ext:Window>
    </form>
</body>
</html>
