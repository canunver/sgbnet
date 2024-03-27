function KomisyonYaz() {
    KomisyonYazYeni();
}

function KomisyonYazYeni() {
    var hedefListe = parent.grdListe;
    var hedefStore = hedefListe.getStore();
    var komisyon = $("#txtRakam").val();
    var ekle = true;
    var kdvDahil = false;

    if (document.getElementById('optSecim_1').checked)
        ekle = false;

    if (document.getElementById('chkKDV').checked)
        kdvDahil = true;

    for (var j = 0; j < 3; j++) { komisyon = komisyon.replace(/\./, ''); }
    for (var j = 0; j < 3; j++) { komisyon = komisyon.replace(/\,/, '\.'); }

    if (!(komisyon > 0))
        alert('Rakam alanında hesaplanacak bilgi olmadığı için işlem yapılamadı.');

    var satirSayisi = hedefStore.data.length;
    var toplamFiyat = 0;
    var lmiktar = new Array();
    var lkdv = new Array();
    var lfiyat = new Array();

    for (var i = 0; i < satirSayisi; i++) {
        var miktar = hedefStore.data.items[i].data.miktar;
        var kdv = hedefStore.data.items[i].data.kdvOran;
        var fiyat = hedefStore.data.items[i].data.birimFiyat;

        //for (var j = 0; j < 3; j++) fiyat = fiyat.replace(/\./, '')
        //for (var j = 0; j < 3; j++) fiyat = fiyat.replace(/\,/, '\.')

        //for (var j = 0; j < 3; j++) miktar = miktar.replace(/\./, '')
        //for (var j = 0; j < 3; j++) miktar = miktar.replace(/\,/, '\.')

        lmiktar[i] = miktar;
        lkdv[i] = kdv;
        lfiyat[i] = fiyat;

        if (kdvDahil)
            toplamFiyat = toplamFiyat + (fiyat * miktar * (kdv / 100 + 1));
        else
            toplamFiyat = toplamFiyat + (fiyat * miktar);
    }

    var hTutar = 0;
    for (var i = 0; i < satirSayisi; i++) {
        var miktar = lmiktar[i];
        var kdv = lkdv[i];
        var fiyat = lfiyat[i];

        var hesapla = 0;
        if (kdvDahil)
            hesapla = ((fiyat * (kdv / 100 + 1) * komisyon) / toplamFiyat);
        else
            hesapla = ((fiyat * komisyon) / toplamFiyat);

        if (i != satirSayisi - 1)
            hTutar = hTutar + hesapla * miktar;
        else
            hesapla = (komisyon - hTutar) / miktar;

        if (ekle)
            hesapla = hesapla + Number(fiyat);
        else
            hesapla = Number(fiyat) - hesapla;

        if (hesapla > 0) {
            hedefStore.getAt(i).set("birimFiyat", hesapla);
        }
    }

    parent.hidePopWin();
}

function formatla6Hane(num, kurusEkle) {
    var isNegative = false;

    if (isNaN(num)) {
        num = "0";
    }
    if (num < 0) {
        num = Math.abs(num);
        isNegative = true;
    }
    sakliNum = num;

    cents = Math.floor((num * 1000000 + 0.5) % 1000000);
    num = Math.floor((num * 1000000 + 0.5) / 1000000).toString();

    //6 karakterden küçük ise başına o kadar sıfır koy
    centsYazi = cents + "";
    if (centsYazi.length < 6) {
        for (i = 0; i < 6 - centsYazi.length; i++) {
            cents = "0" + cents;
        }
    }

    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++) {
        num = num.substring(0, num.length - (4 * i + 3)) + '.' + num.substring(num.length - (4 * i + 3));
    }

    var result = 0;
    if (kurusEkle == true)
        result = num + ',' + cents;
    else
        result = num;
    if (isNegative) {
        result = "-" + result;
    }
    return result;
}
function KomisyonYazEski() {
    var hedefListe = parent.document.getElementById('fpL');
    var komisyon = document.getElementById('txtRakam').value;
    var ekle = true;
    var kdvDahil = false;

    if (document.getElementById('optSecim_1').checked)
        ekle = false;

    if (document.getElementById('chkKDV').checked)
        kdvDahil = true;

    for (var j = 0; j < 3; j++) { komisyon = komisyon.replace(/\./, ''); }
    for (var j = 0; j < 3; j++) { komisyon = komisyon.replace(/\,/, '\.'); }

    if (!(komisyon > 0))
        alert('Rakam alanında hesaplanacak bilgi olmadığı için işlem yapılamadı.');

    var satirSayisi = hedefListe.GetRowCount();
    var toplamFiyat = 0;
    var lmiktar = new Array();
    var lkdv = new Array();
    var lfiyat = new Array();

    for (var i = 0; i < satirSayisi; i++) {
        var miktar = hedefListe.GetValue(i, 4);
        var kdv = hedefListe.GetValue(i, 6);
        var fiyat = hedefListe.GetValue(i, 7);

        for (var j = 0; j < 3; j++) fiyat = fiyat.replace(/\./, '')
        for (var j = 0; j < 3; j++) fiyat = fiyat.replace(/\,/, '\.')

        for (var j = 0; j < 3; j++) miktar = miktar.replace(/\./, '')
        for (var j = 0; j < 3; j++) miktar = miktar.replace(/\,/, '\.')

        lmiktar[i] = miktar;
        lkdv[i] = kdv;
        lfiyat[i] = fiyat;

        if (kdvDahil)
            toplamFiyat = toplamFiyat + (fiyat * miktar * (kdv / 100 + 1));
        else
            toplamFiyat = toplamFiyat + (fiyat * miktar);
    }

    var hTutar = 0;
    for (var i = 0; i < satirSayisi; i++) {
        var miktar = lmiktar[i];
        var kdv = lkdv[i];
        var fiyat = lfiyat[i];

        var hesapla = 0;
        if (kdvDahil)
            hesapla = ((fiyat * (kdv / 100 + 1) * komisyon) / toplamFiyat);
        else
            hesapla = ((fiyat * komisyon) / toplamFiyat);

        if (i != satirSayisi - 1)
            hTutar = hTutar + hesapla * miktar;
        else
            hesapla = (komisyon - hTutar) / miktar;

        if (ekle)
            hesapla = hesapla + Number(fiyat);
        else
            hesapla = Number(fiyat) - hesapla;

        if (hesapla > 0) {
            hedefListe.SetValue(i, 7, formatla6Hane(hesapla, true), true);
        }
    }

    parent.hidePopWin();
}
