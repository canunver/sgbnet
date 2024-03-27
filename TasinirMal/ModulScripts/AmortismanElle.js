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

function HucreDegisti(control) {
    var col = event.cell.cellIndex;
    if (col == 5 || col == 6)
        deger = ParaNoktaKoy(control, null);
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
    for (var i = rg.row; i < rg.row + rg.rowCount; i++)
        for (var j = 0; j <= 5; j++)
            ListeyeDegerYaz(liste, i, j, "", false);
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
