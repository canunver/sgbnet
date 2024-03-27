<%@ Page Language="C#" CodeBehind="SayimKisiDetayPencere.aspx.cs" Inherits="TasinirMal.SayimKisiDetayPencere" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="javascript" type="text/javascript" src="ModulScripts/SayimGiris.js?v=1"></script>
    <script type="text/javascript">
        function Yenile() {
            var sayimDetayKod = hdnSayimDetaySayimDetayKod.getValue();
            var sayimKod = hdnSayimDetaySayimKod.getValue();
            var kisiKod = hdnSayimDetayKisiKod.getValue();

            Ext1.net.DirectMethods.KisiDetayEkraniHazirla(sayimDetayKod,sayimKod, kisiKod,
                {
                    eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                }
            );
        }

        var SayimKisiDetayKapat = function () {
            parent.wndSayimKisiDetayKapat();
            parent.hidePopWin();
        }

        var SatirIslemiYap = function (command, record, rowIndex) {
            if (command == "FazlaSil") {
                var kod = record.data.KOD;
                Ext1.net.DirectMethods.FazlaSil(kod,
                    {
                        eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                    }
                );
            }

            if (command == "IslemYap") {
                var sicilNo = record.data.SICILNO;
                var okundu = record.data.OKUNDU;
                Ext1.net.DirectMethods.SicilNoOkutuldu(sicilNo, okundu,
                    {
                        eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                    }
                );
                Yenile();
            }
        }

        var SicilOkundu = function (kontrol, key) {
            if (key.keyCode == 13) {
                var sicilNo = txtSicil.getValue();
                Ext1.net.DirectMethods.SicilNoOkutuldu(sicilNo, 0,
                    {
                        eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
                    }
                );
            }
        }

        var satirRenklendir = function (record, rowIndex, rp, ds) {
            if (record.get('OKUNDU') > 0) {
                return 'Okundu';
            }
            else {
                return 'OkunduSifir';
            }
        }

    </script>
    <style type="text/css">
        .Okundu {
            color: red;
        }

        .OkunduSifir {
            color: Black;
        }
    </style>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Hidden ID="hdnAktifSayimKod" runat="server" />
        <ext:Hidden ID="hdnSayimKod" runat="server" />
        <ext:Hidden ID="hdnMuhasebeKod" runat="server" />
        <ext:Hidden ID="hdnHarcamaBirimKod" runat="server" />
        <ext:Store ID="storeZimmet" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="SAYIMKOD" />
                        <ext:RecordField Name="KISIKOD" />
                        <ext:RecordField Name="PRSICILNO" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="MALZEMEAD" />
                        <ext:RecordField Name="OKUNDU" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="storeZimmetFazla" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="MALZEMEAD" />
                        <ext:RecordField Name="FAZLAKISIAD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Hidden ID="hdnSayimDetayKisiKod" runat="server" />
        <ext:Hidden ID="hdnSayimDetaySayimKod" runat="server" />
        <ext:Hidden ID="hdnSayimDetaySayimDetayKod" runat="server" />
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;"
            Layout="FitLayout">
            <Items>
                <ext:Panel runat="server" Region="Center" Layout="BorderLayout" Border="false">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:Button ID="btnYenile" runat="server" Text="Yenile" Icon="Reload" OnClientClick="Yenile();">
                                </ext:Button>
                                <ext:Button ID="btnRapor" runat="server" Text="Rapor Hazırla" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnRapor_Click" IsUpload="true">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarSeparator runat="server" />
                                <ext:Checkbox ID="chkYenile" runat="server" FieldLabel="Her Okutmada Yenile">
                                </ext:Checkbox>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnKapat" runat="server" Text="Kapat" Icon="Decline" OnClientClick="SayimKisiDetayKapat();">
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:GridPanel ID="grdZimmet" runat="server" Region="West" StoreID="storeZimmet"
                            Title="Üzerinde zimmetli olan malzemeler" AutoExpandColumn="MALZEMEAD" Width="500"
                            Split="true">
                            <ColumnModel>
                                <Columns>
                                    <ext:RowNumbererColumn Width="20" />
                                    <ext:Column Header="Sicil No" DataIndex="SICILNO" Width="160" />
                                    <ext:Column Header="Malzeme Adı" DataIndex="MALZEMEAD" />
                                    <ext:Column Header="Okundu" DataIndex="OKUNDU" Hidden="true" />
                                    <ext:CommandColumn Width="25" Align="Center">
                                        <Commands>
                                            <ext:GridCommand Icon="Pencil" CommandName="IslemYap" ToolTip-Text="Okut">
                                            </ext:GridCommand>
                                        </Commands>
                                    </ext:CommandColumn>
                                </Columns>
                            </ColumnModel>
                            <Listeners>
                                <Command Fn="SatirIslemiYap" />
                            </Listeners>
                            <View>
                                <ext:GridView runat="server">
                                    <GetRowClass Fn="satirRenklendir" />
                                </ext:GridView>
                            </View>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="3000" HideRefresh="true">
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                        <ext:Panel ID="pnlGenel" runat="server" Layout="BorderLayout" Region="Center" Border="false">
                            <Items>
                                <ext:Panel Region="Center" Layout="BorderLayout" runat="server" Border="false">
                                    <Items>
                                        <ext:Panel ID="pnlKisi" runat="server" Region="Center" Padding="5">
                                            <Items>
                                                <ext:ProgressBar ID="prgDurum" runat="server" Width="400">
                                                </ext:ProgressBar>
                                                <ext:DisplayField runat="server" Height="24" />
                                                <ext:TextField ID="txtSicil" runat="server" FieldLabel="Sicil No" Width="400" EnableKeyEvents="true"
                                                    AutoFocus="true" AutoFocusDelay="100">
                                                    <Listeners>
                                                        <KeyPress Fn="SicilOkundu" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Items>
                                        </ext:Panel>
                                    </Items>
                                </ext:Panel>
                                <ext:GridPanel runat="server" Region="South" Width="310" Height="430" AutoExpandColumn="MALZEMEAD"
                                    Title="Üzerinde zimmetli olamayan malzemeler" StoreID="storeZimmetFazla">
                                    <ColumnModel>
                                        <Columns>
                                            <ext:RowNumbererColumn Width="20" />
                                            <ext:Column Header="Sicil No" DataIndex="SICILNO" Width="170" />
                                            <ext:Column Header="Malzeme Adı" DataIndex="MALZEMEAD" />
                                            <ext:Column Header="Kimde" DataIndex="FAZLAKISIAD" Width="160" />
                                            <ext:CommandColumn Width="25" Align="Center">
                                                <Commands>
                                                    <ext:GridCommand Icon="Decline" CommandName="FazlaSil" ToolTip-Text="Sil">
                                                    </ext:GridCommand>
                                                </Commands>
                                            </ext:CommandColumn>
                                        </Columns>
                                    </ColumnModel>
                                    <Listeners>
                                        <Command Fn="SatirIslemiYap" />
                                    </Listeners>
                                    <BottomBar>
                                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="3000" HideRefresh="true">
                                        </ext:PagingToolbar>
                                    </BottomBar>
                                </ext:GridPanel>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
