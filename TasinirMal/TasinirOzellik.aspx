<%@ Page Language="C#" CodeBehind="TasinirOzellik.aspx.cs" Inherits="TasinirMal.TasinirOzellik" EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Özellikler</title>
    <style id="styleDiv" type="text/css">
        @media print {
            body * {
                visibility: hidden;
            }

            #divYazdir * {
                visibility: visible;
            }
        }

        .tablo {
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            font-size: 9pt;
            border-collapse: collapse;
            width: 100%;
        }

            .tablo td, .tablo th {
                border: 1px solid #ddd;
                padding: 8px;
            }

            .tablo tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .tablo tr:hover {
                background-color: #ddd;
            }

            .tablo th {
                padding-top: 12px;
                padding-bottom: 12px;
                text-align: left;
                background-color: #4CAF50;
                color: white;
            }
    </style>
    <script type="text/javascript">
        function BilgileriYaz() {
            Ext1.net.DirectMethods.Listele({
                success: function (sonuc) {

                    for (var i = 0; i <= sonuc.length; i++) {
                        $("#divYazdir").html(function () {
                            if (i == 38) {
                                $("#tblTarihce tbody").append(sonuc[i]);
                            }
                            else
                                return $(this).html().replace("{" + i + "}", sonuc[i]);
                        });
                    }
                },
                eventMask: { showMask: true, msg: "Lütfen Bekleyin..." }
            });
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="BilgileriYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Toolbar runat="server">
            <Items>
                <ext:Button runat="server" Text="Yazdır" Icon="Printer">
                    <Listeners>
                        <Click Handler="$('#txtTarih').show(); window.print(); $('#txtTarih').hide();"></Click>
                    </Listeners>
                </ext:Button>
                <ext:Button ID="btnIndir" runat="server" Text="İndir" Icon="PageExcel">
                    <DirectEvents>
                        <Click OnEvent="btnIndir_Click" IsUpload="true">
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Items>
        </ext:Toolbar>

        <div id="divYazdir" style="padding: 10px;">
            <div id="txtTarih" runat="server" class="tablo" style="text-align: right; display: none;"></div>
            <table id="tblBilgi" class="tablo">
                <tr>
                    <td width="150px"><b>Demirbaş ve Bis No</b></td>
                    <td>{0}</td>
                    <td width="150px"><b>Eski Dem. ve Bis No</b></td>
                    <td>{1}</td>
                </tr>
                <tr>
                    <td><b>Tanımı</b></td>
                    <td style="max-width: 150px; text-wrap: none;">{2}</td>

                    <td><b>Daha Eski Dem. ve Bis No</b></td>
                    <td>{3}</td>
                </tr>
                <tr>
                    <td><b>Ek No</b></td>
                    <td style="max-width: 150px; text-wrap: none;">{4}</td>

                    <td><b>Malzeme Kodu</b></td>
                    <td>{5}</td>
                </tr>
                <tr>
                    <td><b>Alış Tarihi</b></td>
                    <td>{6}</td>

                    <td><b>Hesap Tarihi</b></td>
                    <td>{7}</td>
                </tr>
                <tr>
                    <td><b>Yer Kodu</b></td>
                    <td>{8}</td>

                    <td><b>Müdürlük Kodu</b></td>
                    <td>{9}</td>
                </tr>
                <tr>
                    <td><b>Harmoni Lokasyon</b></td>
                    <td>{34}</td>

                    <td><b>Harmoni Zimmet</b></td>
                    <td>{35}</td>
                </tr>
                <tr>
                    <td><b>Bütçe Kodu</b></td>
                    <td>{10}</td>

                    <td><b>Blokaj Kodu</b></td>
                    <td>{11}</td>
                </tr>
                <tr>
                    <td><b>Fiş No</b></td>
                    <td>{30}</td>

                    <td><b>Açıklama / Seri No</b></td>
                    <td style="max-width: 150px; text-wrap: none;">{31}</td>
                </tr>
                <tr>
                    <td><b>Marka</b></td>
                    <td>{32}</td>

                    <td><b>Model</b></td>
                    <td>{33}</td>
                </tr>
                <tr>
                    <td><b>İlk Amorti. Yüzdesi</b></td>
                    <td>{12}</td>

                    <td><b>Bul. Amorti. Yüzdesi</b></td>
                    <td>{13}</td>
                </tr>
                <tr>
                    <td><b>İlk Bedel</b></td>
                    <td>{14}</td>

                    <td><b>Biriken Amorti.</b></td>
                    <td>{15}</td>
                </tr>
                <tr>
                    <td><b>Bedel Düz. Farkı</b></td>
                    <td>{16}</td>

                    <td><b>Amort Düz. Farkı</b></td>
                    <td>{17}</td>
                </tr>
                <tr>
                    <td><b>Son Bedel</b></td>
                    <td>{18}</td>

                    <td><b>Düz. Biriken Amort.</b></td>
                    <td>{19}</td>
                </tr>
                <tr>
                    <td><b>Ayr. Son Dönem Amort.</b></td>
                    <td>{20}</td>

                    <td><b>Geçmiş Dönemler Amort.</b></td>
                    <td>{21}</td>
                </tr>
                <%--<tr>
                    <td><b>Geldiği Şube</b></td>
                    <td>{22}</td>
                    <td><b></b></td>
                    <td></td>
               <td><b>Şube Mut. Tarihi</b></td>
                    <td>{25}</td>
                </tr>
               <tr>
                    <td><b>Mut. Tarihi</b></td>
                    <td>{22}</td>

                    <td><b>Mut. No</b></td>
                    <td>{23}</td>
                </tr>
                <tr>
                    <td><b>Şube Mut. No 1</b></td>
                    <td>{26}</td>

                    <td><b>Şube Mut. No 2</b></td>
                    <td>{27}</td>
                </tr>
                <tr>
                    <td><b>Şube Mut. No 3</b></td>
                    <td>{28}</td>

                    <td><b>Şube Mut. No 4</b></td>
                    <td>{29}</td>
                </tr>--%>

                <%--<tr>
                    <td><b>Şase No</b></td>
                    <td>{34}</td>

                    <td><b>Motor No</b></td>
                    <td>{35}</td>
                </tr>
                <tr>
                    <td><b>Plaka</b></td>
                    <td>{36}</td>

                    <td><b>Garanti Bitiş Tarihi</b></td>
                    <td>{37}</td>
                </tr>--%>
            </table>
            <br />
            <table id="tblTarihce" class="tablo">
                <thead>
                    <tr>
                        <th>Tarih</th>
                        <th>Muhasebe Birimi</th>
                        <th>Harcama Birimi</th>
                        <th>Ambar</th>
                        <th>Belge No</th>
                        <th>İşlem</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>

