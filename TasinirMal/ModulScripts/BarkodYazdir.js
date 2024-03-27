function BarkodYaz() {

    var yazdirmaTur = hdnYazdirmaTur.getValue();
    var aktifSatir = 0;
    var bEn = txtGenislik.getValue();
    var bBoy = txtYukseklik.getValue();
    var bSol = txtSolBosluk.getValue();
    var bUst = txtUstBosluk.getValue();
    var bBoyut = "0";
    var barkodAciklama = "";
    var data = "";
    var gidecekString = "";

    if (rdCokKucuk.checked)
        bBoyut = "1";
    else if (rdKucuk.checked)
        bBoyut = "2";
    else if (rdNormal.checked)
        bBoyut = "3";
    else if (rdBuyuk.checked)
        bBoyut = "4";

    barkodAciklama = txtAciklama.getValue();

    var obj = null;
    if (yazdirmaTur == "")
        obj = new ActiveXObject("barkod.BarkodYazdir");

    var seciliSatirlar = grdListe.getRowsValues({ selectedOnly: true });

    var sayac = 0;

    for (var i = 0; i < seciliSatirlar.length; i++) {

        var entry = seciliSatirlar[i];

        var muhasebeKod = pgFiltre.source["prpMuhasebe"];
        var muhasebeAd = entry.MUHASEBEADI;
        var harcamaKod = pgFiltre.source["prpHarcamaBirimi"];
        var harcamaAd = entry.HARCAMABIRIMADI;
        var ambarKod = pgFiltre.source["prpAmbar"];
        var ambarAd = entry.AMBARADI;
        var sicilNo = entry.SICILNO;
        var hesapPlanAd = entry.HESAPPLANADI;
        var zimmetKisi = "";

        var ekBilgi = cmbEkBilgi.getValue();

        if (ekBilgi == "2")
            zimmetKisi = entry.ZIMMETKISI;
        else if (ekBilgi == "3")
            zimmetKisi = entry.ESERBILGISI;
        else if (ekBilgi == "4")
            zimmetKisi = barkodAciklama;
        else if (ekBilgi == "5")
            zimmetKisi = "SeriNo:" + entry.SASENO;

        if (yazdirmaTur == "")
            obj.Yazdir(muhasebeKod, muhasebeAd, harcamaKod, harcamaAd, ambarKod, ambarAd, sicilNo, hesapPlanAd, zimmetKisi, bBoyut, bBoy, bEn, bUst, bSol, data, "");
        else {
            gidecekString += muhasebeKod + "|" + muhasebeAd + "|" + harcamaKod + "|" + harcamaAd + "|" + ambarKod + "|" + ambarAd + "|" + sicilNo + "|" + hesapPlanAd + "|" + zimmetKisi + "|" + bBoyut + "|" + bBoy + "|" + bEn + "|" + bUst + "|" + bSol + "|" + data + "\n";
        }
    }

    if (gidecekString != "") {
        gidecekString = Base64.encode(gidecekString);
        YaziciCagir(gidecekString);
    }

    return false;
}

function YaziciCagir(yazilacakBilgi) {

    var url = "http://localhost:10018?" + yazilacakBilgi;;

    $.ajax({
        type: 'GET',
        url: url,
        data: "",
        success: function (data) {
            Ext1.net.Notification.show({ iconCls: 'icon-information', title: "Bilgi", html: "Etiket Bastırıldı.", hideDelay: 1000, })

        },
        error: function (param1, data, param2) {
            //    console.log(param1.status);
            if (param1.status == 0) {
                Ext1.net.Notification.show({ iconCls: 'icon-information', title: "Bilgi", html: "Etiket Bastırıldı.", hideDelay: 1000, })
            }
            else if (param1.status == 200) {
                Ext1.net.Notification.show({ iconCls: 'icon-information', title: "Bilgi", html: "Etiket Bastırıldı.", hideDelay: 1000, })
            }
            else {
                Ext1.Msg.show({ title: "Bilgi", msg: "Status:" + param1.status + "<br>Status Text:" + param1.statusText, buttons: Ext1.Msg.OK })
            }
        }
    });


    //var fileref = document.createElement('script');
    //fileref.setAttribute("type", "text/javascript");
    //fileref.setAttribute("src", "http://localhost:10018?" + yazilacakBilgi);

    //if (typeof fileref != "undefined")
    //    document.getElementsByTagName("head")[0].appendChild(fileref);
}