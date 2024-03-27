<%@ Page Language="C#" CodeBehind="TasinirIslemDosya.aspx.cs" Inherits="TasinirMal.TasinirIslemDosya" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Dosya İşlemleri</title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="javascript" type="text/javascript">
        var DosyaSil = function (id) {
            Ext1.net.DirectMethods.SatirSil(id, { eventMask: { showMask: true, msg: "Lütfen Bekleyin..." } });
        }

        var KomutCalistir = function (command, record, row) {
            if (command == 'SatirSil') {
                OnayAlExt('Seçili dosya silinecektir.', 'DosyaSil', record.data.DOSYAID, '')

            }
            if (command == 'Indir') {
                Ext1.net.DirectMethods.DosyaIndir(record.data.DOSYAID);
            }
            if (command == 'Goster') {
                showPopWin3("DosyaOnizleme.aspx?menuYok=1&dosyaID=" + record.data.DOSYAID, 840, 600, true, null, null);
            }
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Hidden ID="hdnYil" runat="server" />
        <ext:Hidden ID="hdnMuhasebeKod" runat="server" />
        <ext:Hidden ID="hdnHarcamaBirimKod" runat="server" />
        <ext:Hidden ID="hdnFisNo" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="DOSYAID" />
                        <ext:RecordField Name="YIL" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="HARCAMABIRIMKOD" />
                        <ext:RecordField Name="FISNO" />
                        <ext:RecordField Name="SIRANO" />
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
                <ext:Panel ID="pnlDis" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Layout="FitLayout">
                    <Items>
                        <ext:GridPanel ID="grdResimListe" runat="server" Region="East" Split="true" StripeRows="true" StoreID="strListe"
                            Border="true" Cls="gridExt" Width="400">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:FileUploadField ID="fileDosya" runat="server" ButtonOnly="true" Icon="Attach"
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
