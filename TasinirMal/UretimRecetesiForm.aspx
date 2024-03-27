<%@ Page Language="C#" CodeBehind="UretimRecetesiForm.aspx.cs" Inherits="TasinirMal.UretimRecetesiForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function BelgeAc(kod) {
            ddlAnaUrunKod.setValue(kod);
            btnListele.fireEvent("click");
        }

        var HesapPlanKodSelectAnaUrun = function (a, b) {
            lblAnaUrunAdi.setValue(b.data.hesapPlanAd);
        }

        var HesapPlanKodSelect = function (a, b) {
            var store = grdListe.getStore();
            var rowIndex = store.indexOf(grdListe.getSelectionModel().getSelected());
            var satir = store.getAt(rowIndex);
            satir.set("ALTURUNADI", b.data.hesapPlanAd);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="ALTURUNKOD" />
                        <ext:RecordField Name="ALTURUNADI" />
                        <ext:RecordField Name="MIKTAR" />
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
                        <ext:RecordField Name="hesapPlanKod" />
                        <ext:RecordField Name="hesapPlanAd" />
                        <ext:RecordField Name="olcuBirimAd" />
                        <ext:RecordField Name="kdvOran" />
                        <ext:RecordField Name="rfidEtiketKod" />
                        <ext:RecordField Name="markaKod" />
                        <ext:RecordField Name="modelKod" />
                        <ext:RecordField Name="vurgula" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Margins="5 5 0 5" Padding="10" Split="true" CollapseMode="Mini" Collapsible="true" Header="false" LabelWidth="150" Height="90">
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
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>" Icon="ApplicationGo">
                                    <DirectEvents>
                                        <Click OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator />
                                <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:CompositeField runat="server" FieldLabel="Üretilecek Malzeme">
                            <Items>
                                <ext:ComboBox ID="ddlAnaUrunKod" runat="server" DisplayField="hesapPlanKod" ValueField="hesapPlanKod"
                                    TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                    HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                    StoreID="strHesapPlan" Width="180">
                                    <Template runat="server">
                                        <Html>
                                            <tpl for=".">
                                                <div class="search-item">
                                                    <span class="vurgula-{vurgula}">{hesapPlanKod} - {hesapPlanAd}</span>
                                                </div>
                                            </tpl>
                                        </Html>
                                    </Template>
                                    <Listeners>
                                        <Select Fn="HesapPlanKodSelectAnaUrun" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:DisplayField ID="lblAnaUrunAdi" runat="server" HideLabel="true"></ext:DisplayField>
                            </Items>
                        </ext:CompositeField>

                    </Items>
                </ext:FormPanel>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="0 5 5 5" Split="true" ClicksToEdit="1" Cls="gridExt">
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:Column DataIndex="ALTURUNKOD" Header="Kullanılacak Malzeme" Width="180" Sortable="false" Hideable="false">
                                <Editor>
                                    <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="hesapPlanKod" ValueField="hesapPlanKod"
                                        TypeAhead="false" ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20"
                                        HideTrigger="true" ItemSelector="div.search-item" MinChars="2" SelectOnFocus="true"
                                        StoreID="strHesapPlan">
                                        <Template runat="server">
                                            <Html>
                                                <tpl for=".">
                                                    <div class="search-item">
                                                        <span class="vurgula-{vurgula}">{hesapPlanKod} - {hesapPlanAd}</span>
                                                    </div>
                                                </tpl>
                                            </Html>
                                        </Template>
                                        <Listeners>
                                            <Select Fn="HesapPlanKodSelect" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Editor>
                            </ext:Column>
                            <ext:Column Header="Malzeme Adı" DataIndex="ALTURUNADI" Width="200"
                                Sortable="false" Hideable="false">
                            </ext:Column>
                            <ext:Column Header="Miktar" DataIndex="MIKTAR" Width="100" Align="Right"
                                Sortable="false" Hideable="false">
                                <Editor>
                                    <ext:TextField runat="server" SelectOnFocus="true">
                                    </ext:TextField>
                                </Editor>
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server">
                        </ext:RowSelectionModel>
                    </SelectionModel>
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
