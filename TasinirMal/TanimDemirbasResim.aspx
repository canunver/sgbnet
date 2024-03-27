<%@ Page Language="C#" CodeBehind="TanimDemirbasResim.aspx.cs" Inherits="TasinirMal.TanimDemirbasResim" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        var DosyaSil = function (id) {
            Ext1.net.DirectMethods.SatirSil(id, { eventMask: { showMask: true, msg: "Lütfen Bekleyin..." } });
        }

        var KomutCalistir = function (command, record, row) {
            if (command == 'SatirSil') {
                if (record.data.RESIMID != "") {
                    OnayAlExt('Seçili dosya silinecektir.', 'DosyaSil', record.data.RESIMID, '')
                }
                else {
                    extKontrol.Msg.alert("Uyarı", "Bu dosya silinemez.").Show();
                }
            }
            if (command == 'Indir') {
                Ext1.net.DirectMethods.DosyaIndir(record.data.RESIMID, record.data.DOSYAID);
            }
            if (command == 'Goster') {
                if (record.data.RESIMID != "") {
                    showPopWin("DosyaOnizleme.aspx?menuYok=1&resimID=" + record.data.RESIMID, 840, 600, true, null);
                }
                else if (record.data.DOSYAID != "") {
                    showPopWin("DosyaOnizleme.aspx?menuYok=1&dosyaID=" + record.data.DOSYAID, 840, 600, true, null);
                }
            }
        }

        var saveData = function () {
            hdnGridData.setValue(Ext1.encode(grdListe.getRowsValues({ selectedOnly: false })));
        };

    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnSeciliSicilNo" runat="server" />
        <ext:Hidden ID="hdnSeciliResim" runat="server" />
        <ext:Hidden ID="hdnGridData" runat="server" />
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
                        <ext:RecordField Name="epc" />
                        <ext:RecordField Name="dosyaVar" />
                        <ext:RecordField Name="zimmetoda" />
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
        <ext:Store ID="strResimListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="DOSYAID" />
                        <ext:RecordField Name="RESIMID" />
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="BOYUTU" />
                        <ext:RecordField Name="EKLEYENKISIAD" />
                        <ext:RecordField Name="EKLEYENKISIKOD" />
                        <ext:RecordField Name="EKLEMETARIHI" />
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
                                        <ext:Button ID="btnListe" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
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
                                <ext:PropertyGridParameter Name="prpHesapAd" DisplayName="Hesap Adı">
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
                                <%-- <ext:PropertyGridParameter Name="prpBelgeNo" DisplayName="Belge No (TİF, Zimmet)">
                                </ext:PropertyGridParameter>--%>
                                <ext:PropertyGridParameter Name="prpDosyaVarYok" DisplayName="Dosya Var/Yok">
                                    <Renderer Handler="if (value=='1') return 'Var'; else if (value=='2') return 'Yok'; else return 'Hepsi';" />
                                    <Editor>
                                        <ext:ComboBox ID="ddlDosyaVarYok" runat="server" Editable="false">
                                            <Items>
                                                <ext:ListItem Text="Hepsi" Value="" />
                                                <ext:ListItem Text="Var" Value="1" />
                                                <ext:ListItem Text="Yok" Value="2" />
                                            </Items>
                                        </ext:ComboBox>
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
                            Border="true" Cls="gridExt">
                            <LoadMask ShowMask="true" Msg="Lütfen Bekleyiniz..." />

                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:Button ID="btnXls" runat="server" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                                            <Listeners>
                                                <Click Fn="saveData" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>

                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column ColumnID="tip" DataIndex="tip" Align="Left" Width="70" Header="Belge Türü" Hidden="true" />
                                    <ext:Column ColumnID="prSicilNo" DataIndex="prSicilNo" Hidden="true" Align="Left" />
                                    <ext:Column ColumnID="sicilno" DataIndex="sicilno" Header="<%$ Resources:TasinirMal, FRMBRK037 %>" Width="160" />
                                    <ext:Column ColumnID="kod" DataIndex="kod" Header="<%$ Resources:TasinirMal, FRMBRK038 %>" Width="160" />
                                    <ext:Column ColumnID="ad" DataIndex="ad" Header="<%$ Resources:TasinirMal, FRMBRK039 %>" Width="225" />
                                    <ext:Column ColumnID="kimeGitti" DataIndex="kimeGitti" Header="<%$ Resources:TasinirMal, FRMBRK040 %>" Width="160" Hidden="true" />
                                    <ext:Column ColumnID="dosyaVar" DataIndex="dosyaVar" Header="Dosya?" Width="80" />
                                    <ext:Column ColumnID="zimmetoda" DataIndex="zimmetoda" Header="<%$ Resources:TasinirMal, FRMLOD003 %>" Width="160" Hidden="true" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" SingleSelect="true">
                                    <DirectEvents>
                                        <RowSelect OnEvent="SatirSecildi" Buffer="100">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                            <ExtraParams>
                                                <ext:Parameter Name="prSicilNo" Value="this.getSelected().data.prSicilNo" Mode="Raw" />
                                            </ExtraParams>
                                        </RowSelect>
                                    </DirectEvents>
                                </ext:RowSelectionModel>
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
                        <ext:GridPanel ID="grdResimListe" runat="server" Region="East" Split="true" StripeRows="true" StoreID="strResimListe"
                            Border="true" Cls="gridExt" Width="400">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:FileUploadField ID="fileResim" runat="server" ButtonOnly="true" Icon="Attach"
                                            ButtonText="Dosya Ekle">
                                            <DirectEvents>
                                                <FileSelected IsUpload="true" OnEvent="ResimKayit">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </FileSelected>
                                            </DirectEvents>
                                        </ext:FileUploadField>
                                        <ext:ToolbarFill />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn runat="server" />
                                    <ext:ImageCommandColumn Width="50">
                                        <Commands>
                                            <ext:ImageCommand CommandName="Indir" Icon="DiskDownload">
                                                <ToolTip Text="İndir" />
                                            </ext:ImageCommand>
                                            <ext:ImageCommand CommandName="Goster" Icon="Magnifier">
                                                <ToolTip Text="Göster" />
                                            </ext:ImageCommand>
                                        </Commands>
                                    </ext:ImageCommandColumn>
                                    <ext:Column ColumnID="ADI" DataIndex="ADI" Width="150" Header="Adı" />
                                    <ext:Column ColumnID="BOYUTU" DataIndex="BOYUTU" Width="70" Header="Boyutu" Hidden="true" />
                                    <ext:Column ColumnID="EKLEYENKISIAD" DataIndex="EKLEYENKISIAD" Header="Ekleyen Kisi" Width="120" />
                                    <ext:Column ColumnID="EKLEMETARIHI" DataIndex="EKLEMETARIHI" Header="Ekleme Tarihi" Width="90" Hidden="true" />
                                    <ext:ImageCommandColumn Width="20">
                                        <Commands>
                                            <ext:ImageCommand CommandName="SatirSil" Icon="Delete">
                                                <ToolTip Text="Sil" />
                                            </ext:ImageCommand>
                                        </Commands>
                                    </ext:ImageCommandColumn>
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                </ext:RowSelectionModel>
                            </SelectionModel>
                            <Listeners>
                                <Command Fn="KomutCalistir" />
                            </Listeners>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
