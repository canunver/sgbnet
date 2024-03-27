var hesapPlaniPenceresi = null;
var kdvYaz = true;

function BarkodEkraniAc() {
    var yil = document.getElementById('ddlYil').value;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    var ambarKod = document.getElementById('txtAmbar').value;
    var belgeNo = PadLeft(document.getElementById('txtBelgeNo').value, '0', 6);
    var belgeTur = document.getElementById('hdnBelgeTur').value == "1" ? "ZIM" : "ORT";

    var adres = "BarkodYazdir.aspx?bTur=" + belgeTur + "&y=" + yil + "&m=" + muhasebeKod + "&h=" + harcamaBirimKod + "&a=" + ambarKod + "&b=" + belgeNo + "&menuYok=1";
    parent.showPopWin(adres, 800, 500, true, null);
}

function NodeSecildiYerel(node) {
    var satirHesap = FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 7, "Hesap", 0, false, true);
}

function HesapPlaniGoster() {
    if (hesapPlaniPenceresi == null || hesapPlaniPenceresi.closed) {
        hesapPlaniPenceresi = window.open("ListeHesapPlani.aspx?menuYok=1", "hesapPlani", "resizable,status,width=480,height=410");
        hesapPlaniPenceresi.focus();
    }
    else
        hesapPlaniPenceresi.focus();

    hesapPlaniPenceresi.moveTo(screen.width - 505, 30);
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

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 0) {
        var deger = control.GetValue(row, col);
        TasinirBilgiGetir(control, row, col, 4, -1);
    }
}

function BelgeYazdirGoster() {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var yil = document.getElementById('ddlYil').value;
    var muhasebe = document.getElementById('txtMuhasebe').value;
    var harcama = document.getElementById('txtHarcamaBirimi').value;
    var fisNo = document.getElementById('txtBelgeNo').value;
    var belgeTur = document.getElementById('hdnBelgeTur').value;
    var resim = document.getElementById('chkResim').checked;
    var resimLink = "";
    if (resim == true)
        resimLink = "&resim=1";

    document.getElementById("frmBelgeYazdir").src = "ZimmetFormYazdir.aspx?yil=" + yil + "&harcama=" + harcama + "&fisNo=" + fisNo + "&muhasebe=" + muhasebe + "&belgeTur=" + belgeTur + resimLink;
}

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('txtBelgeNo').value = belgeNo;
    document.getElementById('btnListele').click();
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

    var belgeTuru = document.getElementById('hdnBelgeTur').value;
    var kisi = document.getElementById('txtKimeGitti').value;
    var oda;
    if (belgeTuru == "2") //Dayanıklı Taşınırlar Listesi
        oda = document.getElementById('txtNereyeGitti').value;
    else
        oda = "";

    var zimmetTipi = document.getElementById('ddlZimmetVermeDusme').value;

    if (zimmetTipi == "2" && kisi == "") {
        alert(res_FRMJSC008);
        return;
    }
    if (belgeTuru == "2" && zimmetTipi == "2" && oda == "") {
        alert(res_FRMJSC009);
        return;
    }

    adres += "&kisi=" + kisi;
    adres += "&oda=" + oda;
    adres += "&vermeDusme=" + zimmetTipi;
    adres += "&belgeTur=" + belgeTuru;

    showPopWin(adres, 800, 500, true, null);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow() + satirNo;

    ListeyeDegerYaz(fp, satir, 0, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 1, sicilNo, true);
    //    ListeyeDegerYaz(fp,satir,3,miktar,true); Zimmetten miktar kalktı çünkü 1
    ListeyeDegerYaz(fp, satir, 5, kdvOran, true);
    ListeyeDegerYaz(fp, satir, 6, birimFiyat, true);
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

function ListeTemizle() {
    var liste = document.getElementById('fpL');
    var rg = liste.GetSelectedRange();

    for (var i = rg.row; i < rg.row + rg.rowCount; i++) {
        ListeyeDegerYaz(liste, i, 0, "", true);
        ListeyeDegerYaz(liste, i, 1, "", true);
        ListeyeDegerYaz(liste, i, 3, "", true);
        ListeyeDegerYaz(liste, i, 4, "", true);
        ListeyeDegerYaz(liste, i, 5, "", true);
        ListeyeDegerYaz(liste, i, 6, "", true);
    }
}

function StokSicilListesindenGrideYazKDVYazma(deger) {
    kdvYaz = deger;
}

function ListeAcTerminal(adres) {
    adres += "?menuYok=1&tur=ortakAlan";
    showPopWin(adres, 600, 400, true, null);
}

function TerminalListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kolon) {
    StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, 1, 0, 0);
}