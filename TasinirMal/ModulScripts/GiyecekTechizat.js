function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('txtBelgeNo').value = belgeNo;
    document.getElementById('btnListele').click();
}

var hesapPlaniPenceresi = null;

function NodeSecildiYerel(node) {
    FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 3);
}

var kdvYaz = true;

function StokSicilListesindenGrideYazKDVYazma(deger) {
    kdvYaz = deger;
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow() + satirNo;

    ListeyeDegerYaz(fp, satir, 3, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 5, miktar, true);
}

function PersonelListesindenGrideYaz(sicilNo, adSoyad) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow();

    ListeyeDegerYaz(fp, satir, 0, sicilNo, false);
    ListeyeDegerYaz(fp, satir, 2, adSoyad, true);
}

function StokListesiAc() {
    var hesapPlanKod = document.getElementById('fpL').GetValue(document.getElementById('fpL').GetActiveRow(), 0); //FireFox
    var adres = "ListeStok.aspx?menuYok=1&hpk=" + hesapPlanKod;
    var yil = document.getElementById('ddlYil').value;
    adres += "&yil=" + yil;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hbk=" + harcamaBirimKod;
    var ambarKod = document.getElementById('txtAmbar').value;
    adres += "&ak=" + ambarKod;

    showPopWin(adres, 800, 500, true, null);
}

function PersonelListesiAc() {
    var adres = "ListePersonel.aspx?menuYok=1&cagiran=GRID";
    var yil = document.getElementById('ddlYil').value;
    adres += "&yil=" + yil;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hb=" + harcamaBirimKod;

    showPopWin(adres, 500, 420, true, null);
}

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 5)
        ParaNoktaKoy(control, null);
    else if (col == 0) {
        var deger = control.GetValue(row, col);
        if (deger != "") {
            PersonelBilgiGetir(control, row, col, 2);
        }
    }
    else if (col == 3) {
        var deger = control.GetValue(row, col);
        if (deger != "") {
            TasinirBilgiGetir(control, row, col, 6, 7);
        }
    }
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
        ListeyeDegerYaz(liste, i, 0, "", true);
        ListeyeDegerYaz(liste, i, 2, "", true);
        ListeyeDegerYaz(liste, i, 3, "", true);
        ListeyeDegerYaz(liste, i, 5, "", true);
        ListeyeDegerYaz(liste, i, 6, "", true);
        ListeyeDegerYaz(liste, i, 7, "", true);
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