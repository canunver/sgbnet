<%@ Page Language="C#" CodeBehind="SayimGiris.aspx.cs" Inherits="TasinirMal.SayimGiris" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        var KomutCalistir = function (command, record, row) {
            if (command == 'hesapPlaniGoster') {
                gridYazilacakSatirNo = row;
                grdListe.getSelectionModel().selectRow(row);
                window.setTimeout('grdListe.stopEditing();', 10);
                ListeAc('ListeHesapPlani.aspx', 'grdListe:hesapPlanKod:hesapPlanAd:olcuBirimAd', '', '');
            }
        }

        var AfterEdit = function (grdField) {
            if (grdField.field == "hesapPlanKod") {
                var data = grdField.record.data;
                var kodAd = HesapPlaniAdParcala(data.hesapPlanKod);
                if (kodAd != null) {
                    data.hesapPlanKod = kodAd[0];
                    data.hesapPlanAd = (kodAd.length > 1 ? kodAd[1] : "");
                    data.olcuBirimAd = (kodAd.length > 2 ? kodAd[2] : "");
                    grdField.record.commit();
                }
            }
        }

        var BelgeAc = function (yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
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
                <DocumentReady Handler="GridKilitle(grdListe,[2,3,6]);AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="siraNo">
                    <Fields>
                        <ext:RecordField Name="siraNo" />
                        <ext:RecordField Name="hesapPlanKod" />
                        <ext:RecordField Name="hesapPlanAd" />
                        <ext:RecordField Name="olcuBirimAd" />
                        <ext:RecordField Name="ambarMiktar" Type="Float" />
                        <ext:RecordField Name="ortakMiktar" Type="Float" />
                        <ext:RecordField Name="kayitKisiMiktar" Type="Float" />
                        <ext:RecordField Name="aciklama" />
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
                        <ext:RecordField Name="KOD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="170">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>" Icon="Disk">
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
                                <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMGAG038%>" Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnTemizle_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnIslemler" runat="server" Text="İşlemler" Icon="ApplicationViewList">
                                    <Menu>
                                        <ext:Menu ID="Menu1" runat="server">
                                            <Items>
                                                <ext:MenuItem ID="btnAmbarAktar" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG044%>"
                                                    Icon="PageGo">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnAmbarAktar_Click" Timeout="320000">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="islem" Value="LISTELE" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnAmbarAktarKaydet" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG051%>"
                                                    Icon="PageSave">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnAmbarAktar_Click" Timeout="320000">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="islem" Value="KAYDET" Mode="Value" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnSayimTutanak" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG041%>"
                                                    Icon="PageExcel">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnSayimTutanak_Click" IsUpload="true" />
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnAmbarDevirTutanak" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG042%>"
                                                    Icon="PageExcel">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnAmbarDevirTutanak_Click" IsUpload="true" />
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnTIFNoksan" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG045%>"
                                                    Icon="PageWhiteDelete">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnTIFNoksan_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnTIFFazla" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG046%>"
                                                    Icon="PageWhiteAdd">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnTIFFazla_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:MenuItem>
                                                <ext:MenuItem ID="btnTerminalOku" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG047%>"
                                                    Hidden="true" Icon="ApplicationXpTerminal" OnClientClick="showPopWin('TerminalOku.aspx?menuYok=1&tur=sayim', 600, 400, true, null);">
                                                </ext:MenuItem>
                                            </Items>
                                        </ext:Menu>
                                    </Menu>
                                </ext:Button>
                                <ext:ToolbarFill />
                                <ext:Button ID="btnSil" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG040 %>"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnSil_Click">
                                            <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt silinecektir. Onaylıyor musunuz?" />
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Yıl, Sayım Tarihi, Numara">
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:DateField ID="txtSayimTarihi" runat="server" EmptyText="Sayım Tarihi" Width="100" />
                                <ext:TriggerField ID="txtBelgeNo" runat="server" EmptyText="Sayım No" Width="100">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <DirectEvents>
                                        <TriggerClick OnEvent="btnListele_Click" Timeout="3200000">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </TriggerClick>
                                        <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TriggerField>
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
                        <ext:Label ID="lblBilgi" runat="server" Text="<%$ Resources:TasinirMal, FRMSYG050 %>"
                            Icon="Information" AnchorHorizontal="100%" HideLabel="true" />
                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true" ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column ColumnID="hesapPlanKod" DataIndex="hesapPlanKod" Header="<%$ Resources:TasinirMal, FRMSYG019 %>" Width="225" Fixed="true" Hideable="false">
                                <Editor>
                                    <ext:ComboBox runat="server" DisplayField="KOD" ValueField="KOD" TypeAhead="false"
                                        ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20" HideTrigger="true"
                                        ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true" StoreID="strHesapPlan">
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                                <tpl for=".">
                                                <div class="search-item">
                                                    <span>{KOD}</span>
                                                </div>
                                            </tpl>
                                            </Html>
                                        </Template>
                                    </ext:ComboBox>
                                </Editor>
                                <Commands>
                                    <ext:ImageCommand CommandName="hesapPlaniGoster" Icon="Magnifier" />
                                </Commands>
                            </ext:Column>
                            <ext:Column ColumnID="hesapPlanAd" DataIndex="hesapPlanAd" Header="<%$ Resources:TasinirMal, FRMSYG020 %>" Width="200" />
                            <ext:Column ColumnID="olcuBirimAd" DataIndex="olcuBirimAd" Header="<%$ Resources:TasinirMal, FRMSYG021 %>" Width="70" />
                            <ext:Column ColumnID="ambarMiktar" DataIndex="ambarMiktar" Header="<%$ Resources:TasinirMal, FRMSYG022 %>" Width="100" Align="Right">
                                <%--<Renderer Handler="return Ext1.util.Format.number(value, '0.0/i');" />--%>
                                <Editor>
                                    <ext:NumberField runat="server" SelectOnFocus="true" AllowNegative="false" DecimalPrecision="2" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="ortakMiktar" DataIndex="ortakMiktar" Header="<%$ Resources:TasinirMal, FRMSYG023 %>" Width="100" Align="Right">
                                <%--<Renderer Handler="return Ext1.util.Format.number(value, '0.0/i');" />--%>
                                <Editor>
                                    <ext:NumberField runat="server" SelectOnFocus="true" AllowNegative="false" DecimalPrecision="2" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="kayitKisiMiktar" DataIndex="kayitKisiMiktar"
                                Header="<%$ Resources:TasinirMal, FRMSYG024 %>" Width="100" Fixed="true" Align="Right"
                                Editable="false">
                                <%--<Renderer Handler="return Ext1.util.Format.number(value, '0.0/i');" />--%>
                                <Editor>
                                    <ext:NumberField runat="server" SelectOnFocus="true" AllowNegative="false" DecimalPrecision="2" />
                                </Editor>
                            </ext:Column>
                            <ext:Column ColumnID="aciklama" DataIndex="aciklama" Header="<%$ Resources:TasinirMal, FRMSYG025 %>">
                                <Editor>
                                    <ext:TextArea runat="server" />
                                </Editor>
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <View>
                        <ext:GridView ID="GridView1" runat="server" AutoFill="true" ForceFit="true" />
                    </View>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server">
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <Listeners>
                        <Command Fn="KomutCalistir" />
                        <AfterEdit Fn="AfterEdit" />
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
