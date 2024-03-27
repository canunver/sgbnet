<%@ Page Language="C#" CodeBehind="ListeOdaMB.aspx.cs" Inherits="TasinirMal.ListeOdaMB" EnableViewState="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%= Resources.TasinirMal.FRMLOD001 %></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ListeOdaMB.js?v=2"></script>
    <script language="JavaScript" type="text/javascript">
        var onGridKomut = function (command, record, row, cellIndex) {
            if ($("#hdnCagiran").val().indexOf("grd") > -1) {
                SecKapat(record);
            }
            else
                SecKapatDeger(record.data.KOD, record.data.ADI);
        };


        function SecKapat(record) {
            GrdSatirYaz(record);
        }

        var GrdSatirYaz = function (record) {
            var parentText = extKontrol.getCmp("hdnCagiran").getValue();
            if (parentText.indexOf(":") > -1) {
                var bilgiler = parentText.split(':');
                var kontrol = bilgiler[0];
                if (kontrol.indexOf("grd") == 0) {
                    var grid = window.parent.extKontrol.getCmp(bilgiler[0]);
                    var row = parent.gridYazilacakSatirNo; //Number(bilgiler[1]);

                    var store = grid.getStore();

                    store.getAt(row).set("yerleskeYeri", record.data.KOD);
                    store.getAt(row).set("yerleskeYeriAd", record.data.ADI);
                }
            }

            window.parent.hidePopWin();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Hidden ID="hdnBilgi" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout" AutoScroll="true">
            <Items>
                <ext:TreePanel ID="trvOda" runat="server" AutoWidth="true" Border="false">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:TriggerField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMLOD003 %>"
                                    LabelWidth="40" Width="200" EnableKeyEvents="true" AutoFocus="true">
                                    <Listeners>
                                        <TriggerClick Handler="DalYukleArama($('#txtFiltre').val())"></TriggerClick>
                                    </Listeners>
                                    <Listeners>
                                        <KeyDown Handler="if (e.getKey() == Ext1.EventObject.ENTER) { e.preventDefault(); #{txtFiltre}.fireEvent('triggerClick', this); }" />
                                    </Listeners>
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                </ext:TriggerField>
                                <ext:ToolbarSeparator runat="server" />
                                <ext:RadioGroup runat="server">
                                    <Items>
                                        <ext:Radio ID="rdMuhasebe" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMLPR004 %>" />
                                        <ext:Radio ID="rdHarcamaBirimi" runat="server" Checked="false" BoxLabel="Harcama Birimindeki" Width="190" />
                                        <ext:Radio ID="rdKurum" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMLPR006 %>" />
                                    </Items>
                                </ext:RadioGroup>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Listeners>
                        <BeforeClick Fn="NodeKodSecKapat" />
                    </Listeners>
                </ext:TreePanel>

                <%--     <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Region="Center" AutoExpandColumn="ADI" Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:TriggerField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMLOD003 %>"
                                    LabelWidth="40" Width="200" EnableKeyEvents="true" AutoFocus="true">
                                    <DirectEvents>
                                        <TriggerClick OnEvent="btnAra_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </TriggerClick>
                                    </DirectEvents>
                                    <Listeners>
                                        <KeyDown Handler="if (e.getKey() == Ext1.EventObject.ENTER) { e.preventDefault(); #{txtFiltre}.fireEvent('triggerClick', this); }" />
                                    </Listeners>
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                </ext:TriggerField>
                                <ext:ToolbarSeparator runat="server" />
                                <ext:RadioGroup runat="server">
                                    <Items>
                                        <ext:Radio ID="rdMuhasebe" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMLPR004 %>" />
                                        <ext:Radio ID="rdHarcamaBirimi" runat="server" Checked="false" BoxLabel="Harcama Birimindeki" Width="190" />
                                        <ext:Radio ID="rdKurum" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMLPR006 %>" />
                                    </Items>
                                </ext:RadioGroup>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:ImageCommandColumn runat="server" Header="Seçim" Width="30">
                                <Commands>
                                    <ext:ImageCommand CommandName="Sec" Icon="Accept" />
                                </Commands>
                            </ext:ImageCommandColumn>
                            <ext:Column ColumnID="KOD" DataIndex="KOD" Header="<%$ Resources:TasinirMal, FRMLMH005 %>"
                                Width="150" />
                            <ext:TemplateColumn ColumnID="ADI" DataIndex="ADI" Header="Adı">
                                <Template ID="Template1" runat="server">
                                    <Html>
                                        <a href="javascript:SecKapatDeger('{KOD}','{ADI}')">{ADI}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>--%>
                <%--<ext:Column ColumnID="ADI" DataIndex="ADI" Header="<%$ Resources:TasinirMal, FRMLMH006 %>" />--%>
                <%--   </Columns>
                    </ColumnModel>
                    <Listeners>
                        <Command Fn="onGridKomut" />
                    </Listeners>
                    <SelectionModel>
                        <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
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
                                    <SelectedItem Value="100" />
                                    <Listeners>
                                        <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                    </Listeners>
                                </ext:ComboBox>
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>--%>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
