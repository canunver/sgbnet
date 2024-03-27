<%@ Page Language="C#" CodeBehind="TopluIslem.aspx.cs" Inherits="TasinirMal.TopluIslem" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript">
        function ListeOlustur() {
            var adres = "../TasinirMal/ListeSicilNoYeni.aspx?menuYok=1";
            showPopWin(adres, 800, 550, true, null);
        }

        function GrupIslemGoster(kod, aciklama) {
            txtGrupKod.setValue(kod);
            txtAciklama.setValue(aciklama);
            btnDosyaYukleExt.reset();
            wndGrupBilgi.show();
        }

        function IslemListesiGoster() {
            frmBilgi.hide();
            grdListe.show();
        }

        function Islemler(komut, satir, satirNo, sutun) {
            var grupKod = satir.data.grupKod;
            var aciklama = satir.data.aciklama;
            hdnGrupKod.setValue(grupKod);

            if (komut == "edit") {
                GrupIslemGoster(grupKod, aciklama);
            }
            else if (komut == "delete") {
                Ext1.Msg.confirm("Uyarı", "Grup İşlem bilgisi silinecek. Onaylıyor musunuz?",
                function (btn) {
                    if (btn == "yes") {
                        Ext1.net.DirectMethods.GrupIslemSil(grupKod, { eventMask: { showMask: true} });
                    }
                });
            }
            else if (komut == "download") {
                Ext1.net.DirectMethods.GrupIslemYazdir();
            }
            else if (komut == "islem") {
                Ext1.net.DirectMethods.GrupIslemBaslat();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Hidden ID="hdnGrupKod" runat="server" />
    <ext:Store ID="listeStore" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="grupKod">
                <Fields>
                    <ext:RecordField Name="grupKod" />
                    <ext:RecordField Name="aciklama" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" StoreID="listeStore"
        Border="true" Title="İşlem Listesi" Height="300" AutoExpandColumn="aciklama">
        <TopBar>
            <ext:Toolbar runat="server">
                <Items>
                    <ext:Button ID="btnYeni" runat="server" Icon="Page" Text="Yeni İşlem" OnClientClick="GrupIslemGoster('','');">
                    </ext:Button>
                    <ext:ToolbarFill runat="server" />
                    <ext:Button ID="btnListeYap" runat="server" Text="Liste Oluştur" Icon="Table">
                        <Listeners>
                            <Click Handler="ListeOlustur();" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <ColumnModel runat="server">
            <Columns>
                <ext:RowNumbererColumn />
                <ext:ImageCommandColumn Width="90">
                    <Commands>
                        <ext:ImageCommand CommandName="delete" Icon="PageWhiteDelete">
                            <ToolTip Text="Sil" />
                        </ext:ImageCommand>
                        <ext:ImageCommand CommandName="edit" Icon="PageWhiteEdit">
                            <ToolTip Text="Grup Bilgisini Düzenle" />
                        </ext:ImageCommand>
                        <ext:ImageCommand CommandName="download" Icon="PageWhiteExcel">
                            <ToolTip Text="İşlem Dosyasını İndir" />
                        </ext:ImageCommand>
                        <ext:ImageCommand CommandName="islem" Icon="PageWhiteGear">
                            <ToolTip Text="İşlemler Ekranına Git" />
                        </ext:ImageCommand>
                    </Commands>
                </ext:ImageCommandColumn>
                <ext:Column ColumnID="grupKod" DataIndex="grupKod" Width="70" Header="Grup Kodu">
                </ext:Column>
                <ext:Column ColumnID="aciklama" DataIndex="aciklama" Align="Left" Header="Açıklama">
                </ext:Column>
            </Columns>
        </ColumnModel>
        <Listeners>
            <Command Fn="Islemler" />
        </Listeners>
    </ext:GridPanel>
    <ext:Panel ID="frmBilgi" runat="server" Title="İşlem Sayfası" Padding="5" Hidden="true"
        LabelWidth="150">
        <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <ext:Button ID="btnGeri" runat="server" Text="Liste Sayfasına Dön" Icon="ArrowLeft"
                        OnClientClick="IslemListesiGoster();">
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:Panel ID="pnlDugmeler" runat="server" Layout="ColumnLayout" Padding="5" Border="false">
                <Items>
                    <ext:Button ID="btnZimmetDus" runat="server" Text="1. Zimmet Düşme İşlemi" Icon="CartRemove"
                        IconAlign="Top" Width="160">
                        <DirectEvents>
                            <Click OnEvent="btnZimmetDus_Click">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Label ID="Label2" runat="server" Width="5" />
                    <ext:Button ID="btnBirimDevirVer" runat="server" Text="2. Birim Devir Verme İşlemi"
                        Icon="TableEdit" IconAlign="Top" Width="160">
                        <DirectEvents>
                            <Click OnEvent="btnBirimDevirVer_Click">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Label ID="Label3" runat="server" Width="5" />
                    <ext:Button ID="btnBirimDevirAl" runat="server" Text="3. Birim Devir Alma İşlemi"
                        Icon="TableEdit" IconAlign="Top" Width="160">
                        <DirectEvents>
                            <Click OnEvent="btnBirimDevirAl_Click">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Label ID="Label4" runat="server" Width="5" />
                    <ext:Button ID="btnZimmetVer" runat="server" Text="4. Zimmet Verme İşlemi" Icon="CartPut"
                        IconAlign="Top" Width="160">
                        <DirectEvents>
                            <Click OnEvent="btnZimmetVer_Click">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Panel>
            <ext:Panel ID="pnlIzleme" runat="server" Padding="5">
                <Items>
                    <ext:Label ID="lblToplamEvrak" runat="server" FieldLabel="İşlem yapılacak" Text="0" />
                    <ext:Label ID="lblAktarilanEvrak" runat="server" FieldLabel="İşlem Yapılan" Text="0" />
                    <ext:Label ID="lblKalanEvrak" runat="server" FieldLabel="Kalan " Text="0" />
                    <ext:Label runat="server" Html="&nbsp;" Height="15" />
                    <ext:ProgressBar ID="Progress1" runat="server" Width="430" FieldLabel="Durum" />
                    <ext:Label runat="server" Html="&nbsp;" Height="15" />
                    <ext:Panel ID="pnlIslemSonuc" runat="server" Height="250" AutoScroll="true" Header="false">
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Panel>
    <ext:TaskManager ID="TaskManager1" runat="server">
        <Tasks>
            <ext:Task TaskID="IslemGostergec" Interval="4000" AutoRun="false">
                <DirectEvents>
                    <Update OnEvent="RefreshProgress" />
                </DirectEvents>
            </ext:Task>
        </Tasks>
    </ext:TaskManager>
    <ext:Window ID="wndGrupBilgi" runat="server" Title="Grup Bilgisi" Width="500" Height="200"
        Hidden="true" Modal="true" Padding="5">
        <TopBar>
            <ext:Toolbar ID="Toolbar3" runat="server">
                <Items>
                    <ext:Button ID="btnKaydet" runat="server" Text="Kaydet" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnKaydet_Click" Timeout="500000">
                                <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:ToolbarFill runat="server" />
                    <ext:Button ID="btnKapat" runat="server" Text="Kapat" Icon="Decline" OnClientClick="wndGrupBilgi.hide();">
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:TextField ID="txtGrupKod" runat="server" FieldLabel="Grup Kodu" Width="200"
                ReadOnly="true" StyleSpec="background-color:red" />
            <ext:TextField ID="txtAciklama" runat="server" FieldLabel="Açıklama" Width="450" />
            <ext:FileUploadField ID="btnDosyaYukleExt" runat="server" Icon="PageWhiteExcel" FieldLabel="İşlem Dosyası"
                ButtonText="Yükle" EmptyText="Dosya Seçiniz" ButtonOnly="true" >
            </ext:FileUploadField>
            <ext:Label Icon="Lightbulb" ID="lblBilgi" runat="server" Html="* Grup kodunu sistem otomatik verir<br>* Sadece açıklama bilgisini (dosya yüklemeden) değiştirebilirsiniz<br>* Grup bilgisine istenilen zamanda işlem listesi kayıt edilebilir. Yeni yüklenen liste eskisini silecektir.">
            </ext:Label>
        </Items>
    </ext:Window>
    </form>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
</body>
</html>
