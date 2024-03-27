var hesapPlaniPenceresi = null;
var kdvYaz = true;

function NodeSecildiYerel(node) {
    var satirHesap = FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 8, "Hesap", 0, false, true);
}

function HesapPlaniGoster(hesapKod) {
    var islemTur = document.getElementById('ddlIslemTipi').value.split('*')[1];

    if (Number(islemTur) >= 50 || islemTur == "-1") {
        StokListesiAc(islemTur);
        return;
    }
    else {
        if (hesapPlaniPenceresi == null || hesapPlaniPenceresi.closed) {
            hesapPlaniPenceresi = window.open("ListeHesapPlani.aspx?menuYok=1&hesapKod=" + hesapKod, "hesapPlani", "resizable,status,width=480,height=410");
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

function FPDegerAta_HesapPlanListe(node, fp, sutun, sutunAciklama, tertipTur, ekoUzun, noEventKod, noEventAciklama) {
    var kodAd = node.text.split('-');
    var satir = fp.GetActiveRow();

    ListeyeDegerYaz(fp, satir, 0, kodAd[0], noEventKod);
}

function BosSatirAc() {
    document.getElementById('fpL').CallBack("bossatirekle");
}

function BosSatirAc50() {
    document.getElementById('fpL').CallBack("bossatirekle50");
}

function BosSatirAc100() {
    document.getElementById('fpL').CallBack("bossatirekle100");
}

function BosSatirAc250() {
    document.getElementById('fpL').CallBack("bossatirekle250");
}

function BosSatirAc500() {
    document.getElementById('fpL').CallBack("bossatirekle500");
}

function ArayaSatirEkle() {
    document.getElementById('fpL').CallBack("arayasatirekle");
}

function SatirSil() {
    document.getElementById('fpL').CallBack("satirsil");
}

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 0) {
        var deger = control.GetValue(row, col);
        TasinirBilgiGetir(control, row, col, kdvYaz);
    }
    if (col == 7) {
        deger = ParaNoktaKoy(control, null);
    }
}

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('hdnTutanakNo').value = belgeNo;
    document.getElementById('btnListele').click();
}

function ToplamHesapla() {
    var liste = document.getElementById('fpL');

    var satirSayisi = liste.GetRowCount();

    var fisToplami = 0;

    for (var row = 0; row < satirSayisi; row++) {
        var birimFiyat = liste.GetValue(row, 7);
        var miktar = liste.GetValue(row, 4);
        var kdv = liste.GetValue(row, 6);

        if (birimFiyat != "" || miktar != "") {
            birimFiyat = Number(ScriptParaCevir(birimFiyat));
            miktar = Number(ScriptParaCevir(miktar));

            kdv = 1 + (Number(kdv) / 100);
            fisToplami += (birimFiyat * miktar * kdv);
        }
    }

    alert(parayaCevir(fisToplami));
}

function parayaCevir(sayi) {
    sayi = sayi.toString().replace(/\$|\,/g, '');
    if (isNaN(sayi))
        sayi = "0";
    isaret = (sayi == (sayi = Math.abs(sayi)));
    sayi = Math.floor(sayi * 100 + 0.50000000001);
    kurus = sayi % 100;
    sayi = Math.floor(sayi / 100).toString();
    if (kurus < 10)
        kurus = "0" + kurus;
    for (var i = 0; i < Math.floor((sayi.length - (1 + i)) / 3); i++)
        sayi = sayi.substring(0, sayi.length - (4 * i + 3)) + '.' +
            sayi.substring(sayi.length - (4 * i + 3));
    return (((isaret) ? '' : '-') + sayi + ',' + kurus);
}

function ListeTemizle() {
    var liste = document.getElementById('fpL');
    var rg = liste.GetSelectedRange();

    for (var i = rg.row; i < rg.row + rg.rowCount; i++) {
        ListeyeDegerYaz(liste, i, 0, "", true);
        ListeyeDegerYaz(liste, i, 2, "", true);
        ListeyeDegerYaz(liste, i, 4, "", true);
        ListeyeDegerYaz(liste, i, 5, "", true);
        ListeyeDegerYaz(liste, i, 6, "", true);
        ListeyeDegerYaz(liste, i, 7, "", true);
        ListeyeDegerYaz(liste, i, 8, "", true);
        ListeyeDegerYaz(liste, i, 9, "", true);
        ListeyeDegerYaz(liste, i, 10, "", true);
        ListeyeDegerYaz(liste, i, 11, "", true);
        ListeyeDegerYaz(liste, i, 12, "", true);
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

function SicilNoListesiAc() {
    var hesapPlanKod = document.getElementById('fpL').GetValue(document.getElementById('fpL').GetActiveRow(), 0); //FireFox
    var adres = "ListeSicilNo.aspx?menuYok=1&hpk=" + hesapPlanKod;
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

function StokListesiAc(islemTur) {
    var adres = "ListeStok.aspx?menuYok=1";
    var yil = document.getElementById('ddlYil').value;
    adres += "&yil=" + yil;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hbk=" + harcamaBirimKod;
    var ambarKod = document.getElementById('txtAmbar').value;
    adres += "&ak=" + ambarKod;
    adres += "&it=" + islemTur;

    showPopWin(adres, 800, 500, true, null);
}

function TasinirBilgiGetir(control, satir, sutun, kdvYaz) {
    param = new Object;
    param.control = control;
    param.satir = satir;
    param.sutun = sutun;
    param.kdvYaz = kdvYaz;

    var deger = control.GetValue(satir, sutun);
    var adres = "TasinirBilgiXML.aspx?HesapPlanKod=" + deger;

    var c1 = new HttpCagir(adres, TasinirBilgiYaz, param);
    delete (c1);
}

function TasinirBilgiYaz(cevap, param) {
    var olcuBirimAd = "";
    var hesapPlanAd = "";
    var kdv = "";

    try {
        var elem = document.createElement(cevap);
        olcuBirimAd = elem.getAttribute("OLCUBIRIMAD");
        hesapPlanAd = elem.getAttribute("HESAPPLANAD");
        kdv = elem.getAttribute("KDV");
        delete (elem);
    }
    catch (err) {
    }

    if (olcuBirimAd != null)
        ListeyeDegerYaz(param.control, param.satir, 5, olcuBirimAd, true);
    if (kdv != null && param.kdvYaz)
        ListeyeDegerYaz(param.control, param.satir, 6, kdv, true);
    if (hesapPlanAd != null)
        ListeyeDegerYaz(param.control, param.satir, 8, hesapPlanAd, true);

    delete (param);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow() + satirNo;

    ListeyeDegerYaz(fp, satir, 0, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 2, sicilNo, true);
    ListeyeDegerYaz(fp, satir, 4, miktar, true);
    ListeyeDegerYaz(fp, satir, 6, kdvOran, true);
    ListeyeDegerYaz(fp, satir, 7, birimFiyat, true);
}

function StokSicilListesindenGrideYazKDVYazma(deger) {
    kdvYaz = deger;
}
