<%@ Page Language="C#" CodeBehind="TanimHesapPlaniBarkod.aspx.cs" Inherits="TasinirMal.TanimHesapPlaniBarkod" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function DalYukle(node, coklu) {
            gvBarkod.getSelectionModel().clearSelections();
            var zaman = new Date();
            Ext1.net.DirectMethod.request(
                'DalYukle',
                {
                    eventMask: { showMask: true },
                    success: function (result) {
                        if (result == "")
                            return;
                        var data = eval("(" + result + ")");
                        node.loadNodes(data);
                    },

                    failure: function (errorMsg) {
                        Ext1.Msg.alert('Failure', errorMsg);
                    },
                    specifier: 'static',
                    params: { hesapKod: node.id, coklu: coklu, zaman: zaman.toString() }
                }
            );
        }

        function NodeKodSec(node) {
            hdnSeciliKod.setValue(node.id.toString());
            Ext1.net.DirectMethods.Yukle(node.id.toString());
        }

        var secilenKodlariGetir = function () {
            var selectedRowsValues = gvBarkod.getRowsValues({ selectedOnly: true });
            var kodlar = "";
            Ext1.each(selectedRowsValues, function (item) {
                kodlar += item.barkodID + ";";
            });
            return kodlar;
        }

        var startEditing = function (e) {
            if (e.getKey() === e.ENTER) {
                var grid = gvBarkod,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var afterEdit = function (e) {
            Stratek.AfterEdit(e.record.id, e.field, e.originalValue, e.value, e.record.data);
        };

        var addBarkod = function () {
            var str = "";
            if (hdnHarcamaBirimi.getValue() == "")
                str = "Lütfen harcama birimi giriniz";
            else if (hdnMuhasebeBirimi.getValue() == "")
                str = "Lütfen muhasebe birimi giriniz";

            if (str != "") {
                wndGiris.show();
                return;
            }
            var grid = gvBarkod;
            grid.getRowEditor().stopEditing();

            grid.insertRecord(0, {
                barkod: ""
            });

            grid.getView().refresh();
            grid.getSelectionModel().selectRow(0);
            grid.getRowEditor().startEditing(0);
        }

        var YeniKayit = function () {
            Stratek.AfterEdit(reKayit.record.id, null, null, reKayit.record.data.barkod, null);
        };

    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnSeciliKod" runat="server" />
        <ext:Hidden ID="hdnMuhasebeBirimi" runat="server" />
        <ext:Hidden ID="hdnHarcamaBirimi" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="barkodID">
                    <Fields>
                        <ext:RecordField Name="barkodID" />
                        <ext:RecordField Name="hesapNo" />
                        <ext:RecordField Name="barkod" />
                        <ext:RecordField Name="muhasebeKod" />
                        <ext:RecordField Name="harcamaBirimKod" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:TreePanel ID="trvHesap" runat="server" Width="350" Region="West" Split="true" Header="false" AutoScroll="true" Border="true">
                            <Listeners>
                                <BeforeClick Fn="NodeKodSec" />
                            </Listeners>
                        </ext:TreePanel>
                        <ext:GridPanel ID="gvBarkod" runat="server" StoreID="strListe" Border="true" StripeRows="true"
                            Region="Center">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnSil" runat="server" Text="Sil" Icon="Delete" IconAlign="Left"
                                            Width="50">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Seçilen barkodlar silinecektir.<br>Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="secilenKodlar" Mode="Raw" Value="secilenKodlariGetir()" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator>
                                        </ext:ToolbarSeparator>
                                        <ext:Button ID="btnBarkodEkle" runat="server" Text="Barkod Ekle" Icon="Add">
                                            <Listeners>
                                                <Click Fn="addBarkod" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarFill ID="tbfill1" runat="server">
                                        </ext:ToolbarFill>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="barkodID" DataIndex="barkodID" Hidden="true" Align="Left" Width="0">
                                    </ext:Column>
                                    <ext:Column ColumnID="barkod" DataIndex="barkod" Header="Barkod" Align="Left" Width="160">
                                        <Editor>
                                            <ext:TextField ID="txtEditBarkod" runat="server" MaxLength="20" />
                                        </Editor>
                                    </ext:Column>
                                </Columns>
                            </ColumnModel>
                            <Listeners>
                                <KeyDown Fn="startEditing" />
                                <AfterEdit Fn="afterEdit" />
                            </Listeners>
                            <Plugins>
                                <ext:RowEditor ID="reKayit" runat="server" CancelText="Vazgeç" SaveText="Kaydet"
                                    MinButtonWidth="50px">
                                    <Listeners>
                                        <AfterEdit Fn="YeniKayit" />
                                    </Listeners>
                                </ext:RowEditor>
                            </Plugins>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
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
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="wndGiris" runat="server" Height="140" Width="480" Padding="5" Title="Muhasebe Birimi Seç"
            Modal="true" AutoShow="false" Hidden="true">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnKapat" runat="server" Text="Kapat" Icon="Decline">
                            <DirectEvents>
                                <Click OnEvent="btnKapat_Click">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:Container runat="server" Layout="ToolbarLayout">
                    <Items>
                        <ext:TriggerField ID="txtMuhasebe" runat="server" MaxLength="5" FieldLabel="<%$ Resources:TasinirMal, FRMBRK006 %>"
                            Width="260" LabelWidth="130">
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
                </ext:Container>
                <ext:Container runat="server" Layout="ToolbarLayout">
                    <Items>
                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" MaxLength="15" FieldLabel="<%$ Resources:TasinirMal, FRMBRK008 %>"
                            Width="260" LabelWidth="130">
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
                </ext:Container>
            </Items>
        </ext:Window>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
