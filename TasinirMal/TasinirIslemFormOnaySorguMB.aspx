<%@ Page Language="C#" CodeBehind="TasinirIslemFormOnaySorguMB.aspx.cs" Inherits="TasinirMal.TasinirIslemFormOnaySorguMB" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        var RendererOnayDurum = function (a, b, c) {
            return "<a href=\"javascript:TarihceGoster('" + c.data.yil + "','" + c.data.muhasebe + "','" + c.data.harcamaBirimi + "','" + c.data.fisNo + "')\">" + strOnayDurum.getById(a).data.ADI + "</a>";
        }

        var RendererIslemTip = function (a, b, c) {
            var kayit = strIslemTipi.getById(a);
            if (kayit != null)
                return strIslemTipi.getById(a).data.ADI;
            return "";
        }

        var RendererOnayTur = function (a, b, c) {
            var kayit = strOnayDurum.getById(a);
            if (kayit != null)
                return strOnayDurum.getById(a).data.ADI;
            return "";
        }

        var prepareCommandBilgi = function (grid, command, record, row) {
            if (command.command == 'onayAciklama' && (record.data.onayAciklama == null || record.data.onayAciklama == "")) {
                command.hidden = true;
                command.hideMode = 'display';
            }
        };

        var SatirSec = function (grid, rowIndex, columnIndex, e) {
            if (columnIndex == 1) {
                grid.getView().focusRow(rowIndex);
                grid.getSelectionModel().selectRow(rowIndex);
                var bilgi = grid.getStore().getAt(rowIndex);
                DosyaGoruntule(bilgi.data.yil, bilgi.data.donem, bilgi.data.islemTipi, bilgi.data.harcamaBirimi, bilgi.data.muhasebe, bilgi.data.fisNo, 0, bilgi.data.ambar);
            }
            return false;
        }

        var DosyaGoruntule = function (yil, donem, islemTipi, harcamaBirimi, muhasebe, fisNo, tamEkran, ambarKod) {
            Ext1.net.DirectMethods.DosyaGoruntule(yil, donem, islemTipi, harcamaBirimi, muhasebe, fisNo, tamEkran, ambarKod, { isUpload: false, eventMask: { showMask: true, customTarget: "pnlOnIzleme" }, timeout: 180000, success: function () { }, failure: function (errorMsg) { Ext1.Msg.alert('Hata Oluştu', errorMsg); } });
        }

        var listeCommand = function (command, record, rowIndex) {
            if (command == "onayAciklama") {
                lblAciklama.setText(record.data.onayAciklama);
                winAciklama.setTitle("Açıklama / " + record.data.fisNo);
                winAciklama.show();
            }
        };

        var TarihceGoster = function (yil, muhasebe, harcamaBirimi, belgeNo) {
            showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo, 500, 350, true, null);
        }

        var TamEkranAc = function () {
            var store = grdListe.getStore();
            var sm = grdListe.getSelectionModel();
            if (sm.getSelections().length == 1) {
                var rowIndex = store.indexOf(sm.getSelected());
                if (rowIndex > -1) {
                    var bilgi = store.getAt(rowIndex);
                    DosyaGoruntule(bilgi.data.yil, bilgi.data.donem, bilgi.data.islemTipi, bilgi.data.harcamaBirimi, bilgi.data.muhasebe, bilgi.data.fisNo, 1, bilgi.data.ambar);
                }
            }
        }

        var MuhasebeOnIzleme = function (yil, donem, islemTipi, muhasebeKod, harcamaKod, ambarKod, fisNo) {
            parent.parent.showPopWin("MuhasebeBilgisiOnIzleme.aspx?menuYok=1&yil=" + yil + "&donem=" + donem + "&islemTipi=" + islemTipi + "&muhasebeKod=" + muhasebeKod + "&harcamaKod=" + harcamaKod + "&ambarKod=" + ambarKod + "&fisNo=" + fisNo, 760, 620, true, null);
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" AjaxTimeout="720000" />
        <ext:Hidden runat="server"></ext:Hidden>
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="kod">
                    <Fields>
                        <ext:RecordField Name="fisNo" />
                        <ext:RecordField Name="yil" Type="Int" />
                        <ext:RecordField Name="donem" Type="Int" />
                        <ext:RecordField Name="muhasebe" />
                        <ext:RecordField Name="harcamaBirimi" />
                        <ext:RecordField Name="harcamaBirimiAd" />
                        <ext:RecordField Name="ambar" />
                        <ext:RecordField Name="fisTarihi" />
                        <ext:RecordField Name="islemTipi" Type="Int" />
                        <ext:RecordField Name="durum" />
                        <ext:RecordField Name="islemTarih" />
                        <ext:RecordField Name="islemYapan" />
                        <ext:RecordField Name="kod" />
                        <ext:RecordField Name="onayDurum" />
                        <ext:RecordField Name="onayAciklama" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <BaseParams>
                <ext:Parameter Name="start" Value="0" Mode="Raw" />
                <ext:Parameter Name="limit" Value="250" Mode="Raw" />
            </BaseParams>
        </ext:Store>
        <ext:Store ID="strOnayDurum" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIslemTipi" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="TUR" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <SortInfo Field="ADI" Direction="ASC" />
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;"
            Layout="BorderLayout">
            <Items>
                <ext:Panel ID="pnlAna" runat="server" Region="Center" StyleSpec="background-color:white"
                    Border="false" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" ForceFit="true" Collapsible="true"
                            Width="250" Margins="5 0 5 5" Split="true" AutoRender="false" Header="false">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnSorguTemizle_Click" Timeout="320000">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="Button1" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>"
                                            Icon="ApplicationGo">
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
                                <ext:PropertyGridParameter Name="prpBelgeTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMZFS033 %>">
                                    <Renderer Fn="TarihRenderer" />
                                    <Editor>
                                        <ext:DateField runat="server" Format="dd.m.Y" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMZFS034 %>">
                                    <Renderer Fn="TarihRenderer" />
                                    <Editor>
                                        <ext:DateField runat="server" Format="dd.m.Y" />
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <%-- <ext:PropertyGridParameter Name="prpKisi" DisplayName="Kişi">
                                </ext:PropertyGridParameter>--%>
                                <ext:PropertyGridParameter Name="prpOnayTur" DisplayName="Onay Türü">
                                    <Renderer Fn="RendererOnayTur" />
                                    <Editor>
                                        <ext:ComboBox runat="server" StoreID="strOnayDurum" ValueField="KOD" DisplayField="ADI">
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
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                            TrackMouseOver="true" Border="true" StoreID="strListe" ForceFit="true"
                            Split="true" Cls="gridExt" AutoExpandColumn="islemYapan">
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:TemplateColumn ColumnID="fisNo" DataIndex="fisNo" Header="Belge No" Align="Center"
                                        Width="60" Groupable="false" Fixed="true" Hideable="false">
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                                <a href="#">{fisNo}</a>
                                            </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:Column ColumnID="yil" DataIndex="yil" Header="Yıl" Width="40" Fixed="true" Hidden="true" />
                                    <ext:Column ColumnID="fisTarihi" DataIndex="fisTarihi" Header="Belge Tarihi" Width="70"
                                        Fixed="true" />
                                    <ext:Column ColumnID="onayDurum" DataIndex="onayDurum" Header="Onay Durumu" Align="Left"
                                        Width="100" Fixed="true" Hideable="false">
                                        <Renderer Fn="RendererOnayDurum" />
                                    </ext:Column>
                                    <ext:Column ColumnID="muhasebe" DataIndex="muhasebe" Header="Muhasebe" Width="80"
                                        Fixed="true" Hidden="true" />
                                    <ext:Column ColumnID="harcamaBirimi" DataIndex="harcamaBirimi" Header="Harcama Birimi"
                                        Width="100" Fixed="true" Hidden="true" />
                                    <ext:Column ColumnID="harcamaBirimiAd" DataIndex="harcamaBirimiAd" Header="Harcama Birimi Adı" />
                                    <ext:Column ColumnID="ambar" DataIndex="ambar" Header="Ambar" Hidden="true" />
                                    <ext:Column ColumnID="islemTipi" DataIndex="islemTipi" Header="İşlem Tipi">
                                        <Renderer Fn="RendererIslemTip" />
                                    </ext:Column>
                                    <ext:TemplateColumn ColumnID="durum" DataIndex="durum" Header="Belge Durumu" Align="Left"
                                        Width="100" Hidden="true">
                                        <Template ID="Template2" runat="server">
                                            <Html>
                                                <a href="javascript:TarihceGoster('{yil}','{muhasebe}','{harcamaBirimi}','{fisNo}')">{durum}</a>
                                            </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:Column ColumnID="islemTarih" DataIndex="islemTarih" Header="En Son İşlem Tarihi"
                                        Hidden="true" />
                                    <ext:TemplateColumn ColumnID="islemYapan" DataIndex="islemYapan" Header="Muhasebe" Align="Left"
                                        Width="120">
                                        <Template ID="Template4" runat="server">
                                            <Html>
                                                <a href="javascript:MuhasebeOnIzleme('{yil}','{donem}','{islemTipi}','{muhasebe}','{harcamaBirimi}','{ambar}','{fisNo}')">Muhasebe Ön İzleme</a>
                                            </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <%--<ext:Column ColumnID="islemYapan" DataIndex="islemYapan" Header="En Son İşlemi Yapan" />--%>
                                    <ext:TemplateColumn ColumnID="islemYapan" DataIndex="islemYapan" Header="En Son İşlemi Yapan"
                                        Align="Left" Groupable="false">
                                        <Template ID="Template3" runat="server">
                                            <Html>    
                                                {islemYapan}
                                           </Html>
                                        </Template>
                                        <Commands>
                                            <ext:ImageCommand CommandName="onayAciklama" Icon="Note" Style="margin-left: 5px !important;">
                                                <ToolTip Text="Açıklama" />
                                            </ext:ImageCommand>
                                        </Commands>
                                        <PrepareCommand Fn="prepareCommandBilgi" />
                                    </ext:TemplateColumn>
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" CheckOnly="true" runat="server" />
                            </SelectionModel>
                            <Listeners>
                                <RowClick Fn="SatirSec" />
                                <CellClick Fn="SatirSec" />
                                <Command Fn="listeCommand" />
                            </Listeners>
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
                        </ext:GridPanel>
                        <ext:FormPanel ID="pnlOnIzleme" runat="server" Region="East" Split="true" Layout="BorderLayout"
                            Title="Ön İzleme" Width="360">
                            <Tools>
                                <ext:Tool Handler="TamEkranAc();" Type="Maximize" />
                            </Tools>
                            <Items>
                                <ext:Panel ID="pnlDokumanBilgi" runat="server" Region="North" BodyBorder="false"
                                    Border="false" Height="60" Padding="10">
                                    <Items>
                                        <ext:Container ID="cDokumanBilgi" runat="server" Layout="Column" Height="155" Hidden="true">
                                            <Items>
                                                <ext:Container ID="Container1" runat="server" Layout="Form" ColumnWidth=".5">
                                                    <Items>
                                                        <ext:DisplayField ID="lblYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR006 %>"
                                                            LabelStyle="font-weight: bold;" />
                                                        <ext:DisplayField ID="lblBelgeNo" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR007 %>"
                                                            LabelStyle="font-weight: bold;" />
                                                    </Items>
                                                </ext:Container>
                                                <ext:Container ID="Container2" runat="server" Layout="Form" ColumnWidth=".5">
                                                    <Items>
                                                        <ext:DisplayField ID="lblMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR008 %>"
                                                            LabelStyle="font-weight: bold;" />
                                                        <ext:DisplayField ID="lblHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMBTR009 %>"
                                                            LabelStyle="font-weight: bold;" />
                                                    </Items>
                                                </ext:Container>
                                            </Items>
                                        </ext:Container>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="pnlDokuman" runat="server" Region="Center" Border="false" Hidden="false"
                                    Layout="FitLayout">
                                    <AutoLoad ShowMask="true" MaskMsg="Yükleniyor..." Mode="IFrame" NoCache="true" />
                                </ext:Panel>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="winTamEkran" runat="server" Maximized="true" Maximizable="false"
            Layout="FitLayout" ConstrainHeader="true" Minimizable="false" Closable="true"
            CloseAction="Hide" Padding="5" Title="Ön İzleme" Hidden="true">
            <Items>
                <ext:Panel ID="pnlDokumanTamEkran" runat="server" Region="Center" Border="false"
                    Hidden="false" Layout="FitLayout">
                    <AutoLoad ShowMask="true" MaskMsg="Yükleniyor..." Mode="IFrame" NoCache="true" />
                </ext:Panel>
            </Items>
            <Listeners>
                <Hide Handler="pnlDokuman.show();" />
                <Show Handler="pnlDokuman.hide();" />
            </Listeners>
        </ext:Window>
        <ext:Window ID="winAciklama" runat="server" Layout="FitLayout" Modal="true" ConstrainHeader="true" Closable="true"
            CloseAction="Hide" Padding="20" Title="Açıklama" Width="400" Height="220" Hidden="true">
            <Items>
                <ext:Label ID="lblAciklama" runat="server" Text="" />
            </Items>
            <Listeners>
                <Hide Handler="pnlDokuman.show();" />
                <Show Handler="pnlDokuman.hide();" />
            </Listeners>
        </ext:Window>
    </form>
</body>
</html>
