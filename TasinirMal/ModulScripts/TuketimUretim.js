function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('txtBelgeNo').value = belgeNo;
    document.getElementById('btnListele').click();
}

var hesapPlaniPenceresi = null;

function NodeSecildiYerel(node) {
    FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 2);
}

var kdvYaz = true;

function StokSicilListesindenGrideYazKDVYazma(deger) {
    kdvYaz = deger;
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow() + satirNo;

    ListeyeDegerYaz(fp, satir, 0, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 2, miktar, true);
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

function HesapPlaniGoster() {
    if (document.getElementById('ddlTur').selectedIndex == 0) {
        StokListesiAc();
        return;
    }
    else {
        if (hesapPlaniPenceresi == null || hesapPlaniPenceresi.closed) {
            hesapPlaniPenceresi = window.open("ListeHesapPlani.aspx?menuYok=1", "hesapPlani", "resizable,status,width=480,height=410");
            hesapPlaniPenceresi.focus();
        }
        else
            hesapPlaniPenceresi.focus();

        hesapPlaniPenceresi.moveTo(screen.width - 505, 30);
    }
}

function HesapPlaniKapat() {
    if (hesapPlaniPenceresi != null)
        hesapPlaniPenceresi.close();
}

function FPDegerAta_HesapPlanListe(node, fp, sutun, sutunAciklama) {
    var kodAd = node.text.split('-');
    var satir = fp.GetActiveRow();

    fp.SetValue(satir, sutun, kodAd[0], false);

    var hucre = fp.GetCellByRowCol(satir, sutunAciklama);
    hucre.removeAttribute("FpCellType");
    fp.SetValue(satir, sutunAciklama, kodAd[1], false);
    hucre.setAttribute("FpCellType", "readonly");
}

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 2)
        ParaNoktaKoy(control, null);
    else if (col == 0) {
        var deger = control.GetValue(row, col);
        if (deger != "") {
            TasinirBilgiGetir(control, row, col, 3, 4);
        }
        else {
            var hucre = control.GetCellByRowCol(row, 3);
            hucre.removeAttribute("FpCellType");
            control.SetValue(row, 3, "");
            hucre.setAttribute("FpCellType", "readonly");
            hucre = control.GetCellByRowCol(row, 4);
            hucre.removeAttribute("FpCellType");
            control.SetValue(row, 4, "");
            hucre.setAttribute("FpCellType", "readonly");
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
        ListeyeDegerYaz(liste, i, 4, "", true);
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