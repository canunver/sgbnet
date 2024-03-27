function IstekTarihceAc(prSicilNo) {
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    var harcamaKod = document.getElementById('txtHarcamaBirimi').value;
    var adres = "AcikPazarIstekTarihce.aspx?menuYok=1&muhasebeKod=" + muhasebeKod + "&harcamaKod=" + harcamaKod + "&prSicilNo=" + prSicilNo;
    showPopWin(adres, 700, 400, true, null);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow() + satirNo;

    ListeyeDegerYaz(fp, satir, 2, sicilNo, false);
    ListeyeDegerYaz(fp, satir, 4, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 6, kdvOran, true);
    ListeyeDegerYaz(fp, satir, 7, birimFiyat, true);

    TasinirBilgiGetir(fp, satir, 5, hesapPlanKod);
}

function TasinirBilgiGetir(control, satir, sutun, deger) {
    param = new Object;
    param.control = control;
    param.satir = satir;
    param.sutun = sutun;

    var adres = "TasinirBilgiXML.aspx?HesapPlanKod=" + deger;

    var c1 = new HttpCagir(adres, TasinirBilgiYaz, param);
    delete (c1);
}

function TasinirBilgiYaz(cevap, param) {
    var hesapPlanAd = "";

    try {
        var elem = document.createElement(cevap);
        hesapPlanAd = elem.getAttribute("HESAPPLANAD");
        delete (elem);
    }
    catch (err) {
    }

    if (hesapPlanAd != null)
        ListeyeDegerYaz(param.control, param.satir, param.sutun, hesapPlanAd, true);

    delete (param);
}

function SicilNoListesiAc() {
    var adres = "ListeSicilNo.aspx?menuYok=1";
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hbk=" + harcamaBirimKod;

    showPopWin(adres, 850, 500, true, null);
}

function BosSatirAc() {
    document.getElementById('fpL').CallBack("bossatirekle");
}

function ArayaSatirEkle() {
    document.getElementById('fpL').CallBack("arayasatirekle");
}

function SatirSil() {
    document.getElementById('fpL').CallBack("satirsil");
}

function ListeTemizle() {
    var liste = document.getElementById('fpL');
    var rg = liste.GetSelectedRange();

    for (var i = rg.row; i < rg.row + rg.rowCount; i++) {
        ListeyeDegerYaz(liste, i, 2, "", false);
        ListeyeDegerYaz(liste, i, 4, "", false);
        ListeyeDegerYaz(liste, i, 5, "", true);
        ListeyeDegerYaz(liste, i, 6, "", true);
        ListeyeDegerYaz(liste, i, 7, "", true);
        ListeyeDegerYaz(liste, i, 8, "", true);
        ListeyeDegerYaz(liste, i, 9, "", true);
    }
}

function ListeyeDegerYaz(kontrol, row, col, deger, noEvent) {
    var hucre = kontrol.GetCellByRowCol(row, col);
    if (hucre) {
        var durum = hucre.getAttribute("FpCellType");
        hucre.removeAttribute("FpCellType");
        kontrol.SetValue(row, col, deger, noEvent);
        if (durum != "")
            hucre.setAttribute("FpCellType", durum);
    }
}