var OrgBirimDoldur = function (node) {
    var orgKod = ddlBirim.getValue();
    if (orgKod == "")
        orgKod = "0";

    var orgBirim = { kod: orgKod, tasinirMi: 1, ambarMi: 1 };
    nodeLoadOrgBirimParam(node, orgBirim);
};

function MuhasebeHarcamaAmbarBul(muhasebe, harcamaBirimi, ambar, orgBirim) {
    orgBirim = Ext1.util.JSON.decode(orgBirim);
    if (muhasebe != null)
        document.getElementById(muhasebe).value = PadLeft(orgBirim.muhasebekodu, '0', 5);
    if (harcamaBirimi != null)
        document.getElementById(harcamaBirimi).value = orgBirim.kurumsalkod + "." + orgBirim.muhasebebirim;
    if (ambar != null)
        document.getElementById(ambar).value = orgBirim.ambarKod;
}

function ListeAc(adres, parentText, parentCriteria, parentLabel) {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var genislik = 500;


    if (parentText != '') {
        var flagAmbar = 0;
        var flagHarcama = 0;
        var flagOda = 0;
        var flagPersonel = 0;
        var flagMarka = 0;
        var flagModel = 0;

        if (adres == "ListeAmbar.aspx")
            flagAmbar = 1;
        if (adres == "ListeHarcamaBirimi.aspx")
            flagHarcama = 1;
        if (adres == "ListeOda.aspx")
            flagOda = 1;
        if (adres == "ListePersonel.aspx") {
            flagPersonel = 1;
            genislik = 620;
        }
        if (adres == "ListeMarka.aspx")
            flagMarka = 1;
        if (adres == "ListeModel.aspx")
            flagModel = 1;

        adres += "?menuYok=1&cagiran=" + parentText + "&cagiranLabel=" + parentLabel;

        if (flagAmbar == 1 || flagOda == 1 || flagPersonel == 1) {
            //Dağıtım/İade belgesi için sadece ambar gösteriliyor. Muh ve Hb üstteki olmalı
            if (parentText == "txtGonAmbar" && parentCriteria == "txtGonHarcamaBirimi") {
                if (document.getElementById(parentCriteria).value == "")
                    parentCriteria = "txtHarcamaBirimi";
            }

            if (parentCriteria.xtype != "nettrigger") {
                if (document.getElementById(parentCriteria).value == "" && flagPersonel != 1) {
                    alert(res_FRMJSC012);
                    return false;
                }
                adres += "&hb=" + document.getElementById(parentCriteria).value;
            }
            else {
                if (parentCriteria.getName() == "txtHarcamaBirimi")
                    if (txtHarcamaBirimi.getRawValue() == "" && flagPersonel != 1) {
                        alert(res_FRMJSC012);
                        return false;
                    }

                adres += "&hb=" + txtHarcamaBirimi.getRawValue();
            }

            var muhDeger = "";
            if (parentCriteria == "txtGonHarcamaBirimi")
                muhDeger = document.getElementById('txtGonMuhasebe').value;
            else
                muhDeger = document.getElementById('txtMuhasebe').value;

            if (muhDeger == "" && flagPersonel != 1) {
                alert(res_FRMJSC013);
                return false;
            }

            adres += "&mb=" + muhDeger;

            try {
                adres += "&ab=" + document.getElementById('txtAmbar').value;
            }
            catch (e) { }
        }

        if (flagHarcama == 1 && parentCriteria.xtype != "nettrigger") {
            if (document.getElementById(parentCriteria).value == "") {
                alert(res_FRMJSC013);
                return false;
            }
            adres += "&mb=" + document.getElementById(parentCriteria).value;
        }
        else if (flagHarcama == 1 && parentCriteria.xtype == "nettrigger") {
            if (parentCriteria.getName() == "txtMuhasebe")
                if (txtMuhasebe.getRawValue() == "") {
                    alert(res_FRMJSC013);
                    return false;
                }
            adres += "&mb=" + txtMuhasebe.getRawValue();
        }

        if (flagModel == 1) {
            if (document.getElementById(parentCriteria).value == "") {
                alert(res_FRMJSC011);
                return false;
            }
            adres += "&marka=" + document.getElementById(parentCriteria).value;
        }
    }

    showPopWin(adres, genislik, 420, true, null);
}

function Sec(kod, childText) {
    document.getElementById(childText).value = kod;
}

function SecKapat(childText) {
    var parentText = document.getElementById('hdnCagiran').value;
    window.parent.document.getElementById(parentText).value = document.getElementById(childText).value;
    window.parent.hidePopWin();
}

function SecKapatDeger(deger, aciklama) {
    var parentText = document.getElementById('hdnCagiran').value;
    if (parentText == "ext") {
        window.parent.SecKapatDeger(deger, aciklama);
    }
    else {
        var parentLabel = document.getElementById('hdnCagiranLabel').value;
        window.parent.document.getElementById(parentText).value = deger;
        if (parentLabel != "" && parentLabel == 'lblMuhasebeAdHesapPlani')
            window.parent.document.getElementById('hdnMuhasebeBirimi').value = deger;
        if (parentLabel != "" && parentLabel == 'lblHarcamaBirimiAdHesapPlani')
            window.parent.document.getElementById('hdnHarcamaBirimi').value = deger;
        if (parentLabel != "")
            window.parent.document.getElementById(parentLabel).innerHTML = aciklama;
    }
    window.parent.hidePopWin();
}

function TasinirBilgiGetir(control, satir, sutun, hesapSutun, olcuSutun) {
    param = new Object;
    param.control = control;
    param.satir = satir;
    param.sutun = sutun;
    param.hesapSutun = hesapSutun;
    param.olcuSutun = olcuSutun;

    var deger = control.GetValue(satir, sutun);
    var adres = "TasinirBilgiXML.aspx?HesapPlanKod=" + deger;

    var c1 = new HttpCagir(adres, TasinirBilgiYaz, param);
    delete (c1);
}

function TasinirBilgiYaz(cevap, param) {
    var olcuBirimAd = "";
    var hesapPlanAd = "";

    try {
        var elem = document.createElement(cevap);
        olcuBirimAd = elem.getAttribute("OLCUBIRIMAD");
        hesapPlanAd = elem.getAttribute("HESAPPLANAD");
        delete (elem);
    }
    catch (err) {
    }

    if (param.olcuSutun != "-1" && param.olcuSutun != "") {
        document.getElementById('fpL').GetCellByRowCol(param.satir, param.olcuSutun).removeAttribute("FpCellType");
        if (olcuBirimAd != null)
            param.control.SetValue(param.satir, param.olcuSutun, olcuBirimAd, true);
        document.getElementById('fpL').GetCellByRowCol(param.satir, param.olcuSutun).setAttribute("FpCellType", "readonly");
    }
    document.getElementById('fpL').GetCellByRowCol(param.satir, param.hesapSutun).removeAttribute("FpCellType");
    if (hesapPlanAd != null)
        param.control.SetValue(param.satir, param.hesapSutun, hesapPlanAd, true);
    document.getElementById('fpL').GetCellByRowCol(param.satir, param.hesapSutun).setAttribute("FpCellType", "readonly");

    delete (param);
}

function PersonelBilgiGetir(control, satir, sutun, adSoyadSutun) {
    param = new Object;
    param.control = control;
    param.satir = satir;
    param.sutun = sutun;
    param.adSoyadSutun = adSoyadSutun;

    var deger = control.GetValue(satir, sutun);
    var adres = "TasinirBilgiXML.aspx?tcKimlikNo=" + deger;

    var c1 = new HttpCagir(adres, PersonelBilgiYaz, param);
    delete (c1);
}

function PersonelBilgiYaz(cevap, param) {
    var adSoyad = "";

    try {
        var elem = document.createElement(cevap);
        adSoyad = elem.getAttribute("ADSOYAD");
        delete (elem);
    }
    catch (err) {
    }

    if (param.adSoyadSutun != "-1") {
        document.getElementById('fpL').GetCellByRowCol(param.satir, param.adSoyadSutun).removeAttribute("FpCellType");
        if (adSoyad != null)
            param.control.SetValue(param.satir, param.adSoyadSutun, adSoyad, true);
        document.getElementById('fpL').GetCellByRowCol(param.satir, param.adSoyadSutun).setAttribute("FpCellType", "readonly");
    }

    delete (param);
}

function OnayAl(islemTip, kontrol) {
    var deger = true;

    if (islemTip != '') {
        deger = BelgeOnayAl(islemTip);

        if (deger && islemTip != "Yazdir")
            ShowProgress();

        if (deger && kontrol != null) {
            var theForm = document.forms[0];
            var dugme = document.getElementById(kontrol);
            dugme.disabled = true;
            dugme.value = res_FRMJSC014;
            this.__doPostBack(kontrol, '');
        }
    }
    return deger;
}

function OdemeEmriGoster() {
    var yil_ = document.getElementById('hdnYil');
    var belgeNo_ = document.getElementById('hdnBelgeNo');

    var yil = 0;
    var belgeNo = "";

    if (yil_ != null && yil_.value != null && belgeNo_ != null && belgeNo_.value != null) {
        yil = yil_.value;
        belgeNo = belgeNo_.value;

        if (Number(yil) > 0 && belgeNo.length < 13 && belgeNo != "") {
            var sayfa = "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?menuYok=1&aramaYok=1&yil=" + yil + "&belgeNo=" + belgeNo;
            window.open(sayfa, "odemeEmri", "resizable,status");
        }
        else
            alert("Ödeme Emri belge numarası okunamadı.");
    }
}

function OdemeEmriLinktenGoster(yil, belgeNo) {
    document.getElementById('hdnYil').value = yil;
    document.getElementById('hdnBelgeNo').value = belgeNo;

    OdemeEmriGoster();
}

function OdemeHesapKodListele() {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var yil = document.getElementById('hdnYil').value;

    showPopWin("ListeTertip.aspx?menuYok=1&cagiran=txtHesapKod&yil=" + yil + "&kurum=" + "" + "&muhasebe=&birim=&hesapKod=", 600, 360, true, null);
}

function TertipGoster() {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var sutun = document.getElementById('fpL').GetActiveCol() - 1;

    var yil = document.getElementById('hdnYil').value;
    var muhasebe = document.getElementById('hdnMuhasebe').value;
    var birim = document.getElementById('hdnBirim').value;
    var eko = "";
    var kur = document.getElementById('hdnKur').value;

    var satir = document.getElementById('fpL').GetActiveRow();
    var hesapKod = document.getElementById('fpL').GetValue(satir, 0);
    var ekoKodListe = hesapKod.split(' ');
    if (ekoKodListe.length > 1)
        hesapKod = ekoKodListe[0];

    if (hesapKod == "")
        alert(res_FRMJSC016);
    else {
        showPopWin("../ButceMuhasebe/ListeTertip.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&birim=" + birim + "&hedefKontrol=fpL&kolon=" + sutun + "&hesapKod=" + hesapKod + "&eko=" + "" + "&kur=" + kur + "&tur=butce&otoKapat=1&secKapat=1", 580, 350, true, null);
    }

}

function TertipGosterToText() {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var yil = document.getElementById('hdnYil').value;
    var muhasebe = document.getElementById('hdnMuhasebe').value;
    var birim = document.getElementById('hdnBirim').value;
    var eko = "";
    var kur = document.getElementById('hdnKur').value;

    var hesapKod = document.getElementById('txtHesapKod').value;
    var ekoKodListe = hesapKod.split(' ');
    if (ekoKodListe.length > 1)
        hesapKod = ekoKodListe[0];

    if (hesapKod == "")
        alert(res_FRMJSC016);
    else {
        showPopWin("../ButceMuhasebe/ListeTertip.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&birim=" + birim + "&hedefKontrol=txtOdemeTertip&hesapKod=" + hesapKod + "&eko=" + "" + "&kur=" + kur + "&tur=butce&otoKapat=1&secKapat=1", 580, 350, true, null);
    }

}

var firmaSecimYeri = "";
function AltHesapGosterGoster(kontrol) {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    firmaSecimYeri = kontrol;
    var muhasebeKod = document.getElementById('hdnMuhasebe').value;
    showPopWin("../HarcamaSurecleri/ListeFirmaKisi.aspx?menuYok=1&per=1&muhasebe=" + muhasebeKod, 580, 510, true, null);
}


function FirmaGoster() {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    var hdnFirmaHarcamadanAlma = document.getElementById('hdnFirmaHarcamadanAlma').value;
    if (hdnFirmaHarcamadanAlma == "1") {
        showPopWin("ListeFirma.aspx", 580, 510, true, null);
    }
    else {
        showPopWin("../HarcamaSurecleri/ListeFirmaKisi.aspx?menuYok=1&per=1&muhasebe=" + muhasebeKod, 580, 510, true, null);
    }
}

function FirmaBilgisiGoster(firma) {
    try {
        document.getElementById('txtNeredenGeldi').value = firma.ad;
    }
    catch (e) {
        if (firmaSecimYeri == "txtAltHesapKod") {
            document.getElementById('txtAltHesapKod').value = firma.vergiNo;

            try {
                document.getElementById('txtIlgiliAd').value = firma.ad;
                document.getElementById('txtIlgiliNo').value = firma.vergiNo;
                document.getElementById('txtIlgiliVD').value = firma.vergiDairesi;
                document.getElementById('txtIlgiliBankaAd').value = firma.banka;
                document.getElementById('txtIlgiliBankaNo').value = firma.hesapNo;
            } catch (e) {
            }
        }
        else {
            document.getElementById('txtIlgiliAd').value = firma.ad;
            document.getElementById('txtIlgiliNo').value = firma.vergiNo;
            document.getElementById('txtIlgiliVD').value = firma.vergiDairesi;
            document.getElementById('txtIlgiliBankaAd').value = firma.banka;
            document.getElementById('txtIlgiliBankaNo').value = firma.hesapNo;
        }
    }
}

function AdresAc(adres) {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }
    showPopWin(adres, 500, 420, true, null);
}

function isIE() {
    var ua = window.navigator.userAgent;

    // Test values; Uncomment to check result …

    // IE 10
    // ua = 'Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)';

    // IE 11
    // ua = 'Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko';

    // Edge 12 (Spartan)
    // ua = 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0';

    // Edge 13
    // ua = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586';

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}