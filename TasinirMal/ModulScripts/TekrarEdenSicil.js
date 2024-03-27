function SicilNoTarihceAc(prSicilNo) {
    var adres = "SicilNoTarihce.aspx?menuYok=1";
    adres += "&prSicilNo=" + prSicilNo;
    showPopWin(adres, 900, 550, true, null);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    document.getElementById('txtSicilNo').value = sicilNo;
}