function DivYonet() {
    if (document.getElementById('rblIslemTur').document.activeElement.value == "1") {
        document.getElementById('dvAmortismanTur').style.display = "";
        document.getElementById('dvDegerlemeOran').style.display = "none";
    }
    else if (document.getElementById('rblIslemTur').document.activeElement.value == "2") {
        document.getElementById('dvAmortismanTur').style.display = "none";
        document.getElementById('dvDegerlemeOran').style.display = "";
    }
}

function SicilNoListesiAc() {
    var adres = "ListeSicilNo.aspx?menuYok=1&hpk=" + document.getElementById('txtHesapPlanKod').value;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hbk=" + harcamaBirimKod;
    var ambarKod = document.getElementById('txtAmbar').value;
    adres += "&ak=" + ambarKod;

    showPopWin(adres, 800, 500, true, null);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    document.getElementById('txtSicilNo').value = sicilNo;
}

function ListeSec() {
    var kontrol = document.getElementById('dgListe');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}
