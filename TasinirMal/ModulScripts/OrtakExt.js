var gridYazilacakSatirNo;
var sicilListesiAmbarZimmetDurumu;
var sicilListesiKisiDurumu;
var sicilListesiOdaDurumu;
var iliskiMalzemeEklemeDurumu;

var PropertyRenderer = function (store, value) {
    var r = store.getById(value);
    if (!extKontrol.isEmpty(r))
        return r.data.ADI;
    return "";
}

var TarihRenderer = function (value) {
    try {
        if (value != null && value != "")
            return value.format('d.m.Y');
    } catch (e) {

    }
};

function OnayAlExt(msg, fonksiyon, arg1, arg2) {
    msg = msg + " Bu işlemi onaylıyor musunuz?";

    extKontrol.Msg.confirm('Onay', msg, function (btn, text) {
        if (btn == 'yes') {
            window[fonksiyon](arg1, arg2);
        }
    });
}

function AlanIsimleriniYaz() {
    try { txtMuhasebe.fireEvent('change', txtMuhasebe); } catch (e) { }
    try { txtHarcamaBirimi.fireEvent('change', txtHarcamaBirimi); } catch (e) { }
    try { txtAmbar.fireEvent('change', txtAmbar); } catch (e) { }
}

var GridSatirAc = function (store, adet) {
    for (var i = 0; i < adet; i++) {
        store.addRecord({});
    }
    store.commitChanges();
}

var GridSatirAcAraya = function (grid) {
    var store = grid.getStore();
    var rowIndex = store.indexOf(grid.getSelectionModel().getSelected());
    var row = grid.insertRecord(rowIndex, {});
    store.commitChanges();
}

var triggerClickKontrolId;
var TriggerClick = function (field, trigger, index) {
    if (trigger.dom.className.indexOf("x-form-clear-trigger") > -1) {
        field.clear();
        field.focus();
        //field.getTrigger(0).hide();
        return;
    }

    if (trigger.dom.className.indexOf("x-form-simplecross-trigger") > -1) {
        ClearFilter();
        return;
    }

    triggerClickKontrolId = field.id;

    if (trigger.dom.className.indexOf("x-form-search-trigger") > -1) {
        if (field.id == "txtMuhasebe")
            ListeAc('ListeMuhasebe.aspx', 'txtMuhasebe', '', 'lblMuhasebeAd');
        else if (field.id == "txtHarcamaBirimi")
            ListeAc('ListeHarcamaBirimi.aspx', 'txtHarcamaBirimi', 'txtMuhasebe', 'lblHarcamaBirimiAd');
        else if (field.id == "txtAmbar")
            ListeAc('ListeAmbar.aspx', 'txtAmbar', 'txtHarcamaBirimi', 'lblAmbarAd');
        else if (field.id == "txtGonMuhasebe")
            ListeAc('ListeMuhasebe.aspx', 'txtGonMuhasebe', '', 'lblGonMuhasebeAd');
        else if (field.id == "txtGonHarcamaBirimi")
            ListeAc('ListeHarcamaBirimi.aspx', 'txtGonHarcamaBirimi', 'txtGonMuhasebe', 'lblGonHarcamaBirimiAd');
        else if (field.id == "txtGonAmbar")
            ListeAc('ListeAmbar.aspx', 'txtGonAmbar', 'txtGonHarcamaBirimi', 'lblGonAmbarAd');
        else if (field.id == "txtGirenMuhasebe")
            ListeAc('ListeMuhasebe.aspx', 'txtGirenMuhasebe', '', 'lblGirenMuhasebeAd');
        else if (field.id == "txtGirenHarcamaBirimi")
            ListeAc('ListeHarcamaBirimi.aspx', 'txtGirenHarcamaBirimi', 'txtGirenMuhasebe', 'lblGirenHarcamaBirimiAd');
        else if (field.id == "txtGirenAmbar")
            ListeAc('ListeAmbar.aspx', 'txtGirenAmbar', 'txtGirenHarcamaBirimi', 'lblGirenAmbarAd');
        else if (field.id == "txtCikanMuhasebe")
            ListeAc('ListeMuhasebe.aspx', 'txtCikanMuhasebe', '', 'lblCikanMuhasebeAd');
        else if (field.id == "txtCikanHarcamaBirimi")
            ListeAc('ListeHarcamaBirimi.aspx', 'txtCikanHarcamaBirimi', 'txtCikanMuhasebe', 'lblCikanHarcamaBirimiAd');
        else if (field.id == "txtCikanAmbar")
            ListeAc('ListeAmbar.aspx', 'txtCikanAmbar', 'txtCikanHarcamaBirimi', 'lblCikanAmbarAd');
        else if (field.id == "txtOda")
            ListeAc('ListeOda.aspx', 'txtOda', 'txtHarcamaBirimi', 'lblOdaAd');
        else if (field.id == "txtNereyeVerildi")
            ListeAc('ListeOda.aspx', 'txtNereyeVerildi', 'txtHarcamaBirimi', 'lblNereyeVerildi');
        else if (field.id == "txtBulunduguYer")
            ListeAc('ListeOda.aspx', 'txtBulunduguYer', '', 'lblBulunduguYer');
        else if (field.id == "txtNereden")
            ListeAc('ListeOda.aspx', 'txtNereden', 'txtHarcamaBirimi', 'lblNereden');
        else if (field.id == "txtMarka")
            ListeAc('ListeMarka.aspx', 'txtMarka', '', 'lblMarkaAd');
        else if (field.id == "txtBolge")
            ListeAc('ListeBolge.aspx', 'txtBolge', '', 'lblBolgeAd');
        else if (field.id == "txtPersonel")
            ListeAc('ListePersonel.aspx', 'txtPersonel', 'txtHarcamaBirimi', 'lblPersonelAd');
        else if (field.id == "txtKimeGitti")
            ListeAc('ListePersonel.aspx', 'txtKimeGitti', 'txtHarcamaBirimi', 'lblKimeGittiAd');
        else if (field.id == "txtKimeVerildi")
            ListeAc('ListePersonel.aspx', 'txtKimeVerildi', 'txtHarcamaBirimi', 'lblKimeVerildi');
        else if (field.id == "txtSicilNo")
            ListeAc('ListeSicilNoYeni.aspx', 'txtSicilNo', '', 'lblSicilNo');
        else if (field.id == "txtNeredenGeldi" || field.id == "txtNereyeGitti") {
            if (hdnFirmaHarcamadanAlma.getValue() == "1")
                adres = "ListeFirma.aspx?menuYok=1&cagiran=ext&";
            else {
                adres = "../HarcamaSurecleri/ListeFirmaKisi.aspx?menuYok=1&per=1&cagiran=ext&";
                adres += "muhasebe=" + txtMuhasebe.getValue();
            }
            showPopWin(adres, 580, 510, true, null);
        }
        else if (field.id == "txtHesapPlanKod")
            ListeAc('ListeHesapPlani.aspx', 'txtHesapPlanKod', '', 'lblHesapPlanAd');
    }
}

var TriggerClickProperty = function (fieldid, trigger) {
    if (trigger.dom.className.indexOf("x-form-search-trigger") > -1) {
        if (fieldid == "prpMuhasebe")
            ListeAcProperty('ListeMuhasebe.aspx', 'pgFiltre:prpMuhasebe');
        else if (fieldid == "prpHarcamaBirimi")
            ListeAcProperty('ListeHarcamaBirimi.aspx', 'pgFiltre:prpHarcamaBirimi:prpMuhasebe');
        else if (fieldid == "prpAmbar")
            ListeAcProperty('ListeAmbar.aspx', 'pgFiltre:prpAmbar:prpHarcamaBirimi:prpMuhasebe');
        else if (fieldid == "prpGonMuhasebe")
            ListeAcProperty('ListeMuhasebe.aspx', 'pgFiltre:prpGonMuhasebe');
        else if (fieldid == "prpGonHarcamaBirimi")
            ListeAcProperty('ListeHarcamaBirimi.aspx', 'pgFiltre:prpGonHarcamaBirimi:prpGonMuhasebe');
        else if (fieldid == "prpGonAmbar")
            ListeAcProperty('ListeAmbar.aspx', 'pgFiltre:prpGonAmbar:prpGonHarcamaBirimi:prpGonMuhasebe');
        else if (fieldid == "prpOdaKod")
            ListeAcProperty('ListeOda.aspx', 'pgFiltre:prpOdaKod:prpHarcamaBirimi');
        else if (fieldid == "prpHesapKod")
            ListeAcProperty('ListeHesapPlani.aspx', 'pgFiltre:prpHesapKod');
        else if (fieldid == "prpSicilNo") {
            sicilListesiAmbarZimmetDurumu = "TEKSECIM";//genel değişken olduğu için var tanımlaması OrtakExt.js de yapıldı
            ListeAcProperty('ListeSicilNoYeni.aspx', 'pgFiltre:prpSicilNo');
        }
        else if (fieldid == "prpKisiKod")
            ListeAcProperty('ListePersonel.aspx', 'pgFiltre:prpKisiKod:prpHarcamaBirimi');
        else if (fieldid == "prpIstekYapanKod")
            ListeAcProperty('ListePersonel.aspx', 'pgFiltre:prpIstekYapanKod:prpHarcamaBirimi');
        else if (fieldid == "prpMarkaKod")
            ListeAcProperty('ListeMarka.aspx', 'pgFiltre:prpMarkaKod');
        else if (fieldid == "prpModelKod")
            ListeAcProperty('ListeModel.aspx', 'pgFiltre:prpModelKod:prpMarkaKod');
        else if (fieldid == "prpBulunduguYer")
            ListeAcProperty('ListeOda.aspx', 'pgFiltre:prpBulunduguYer');

        //else if (field.id == "txtNereyeVerildi")
        //    ListeAc('ListeOda.aspx', 'txtNereyeVerildi', 'txtMuhasebe:txtHarcamaBirimi', 'lblNereyeVerildi');
        //else if (field.id == "txtBolge")
        //    ListeAc('ListeBolge.aspx', 'txtBolge', '', 'lblBolgeAd');
        //else if (field.id == "txtPersonel")
        //    ListeAc('ListePersonel.aspx', 'txtPersonel', 'txtHarcamaBirimi', 'lblPersonelAd');
        //else if (field.id == "txtKimeGitti")
        //    ListeAc('ListePersonel.aspx', 'txtKimeGitti', 'txtHarcamaBirimi', 'lblKimeGittiAd');
        //else if (field.id == "txtKimeVerildi")
        //    ListeAc('ListePersonel.aspx', 'txtKimeVerildi', 'txtHarcamaBirimi', 'lblKimeVerildi');
        //else if (field.id == "txtNeredenGeldi") {
        //    if (hdnFirmaHarcamadanAlma.getValue() == "1")
        //        adres = "ListeFirma.aspx?menuYok=1&cagiran=ext&";
        //    else {
        //        adres = "../HarcamaSurecleri/ListeFirmaKisi.aspx?menuYok=1&per=1&cagiran=ext&";
        //        adres += "muhasebe=" + txtMuhasebe.getValue();
        //    }
        //    showPopWin(adres, 580, 510, true, null);
        //}
        //else if (field.id == "txtNereyeGitti")
        //    ListeAc('ListePersonel.aspx', 'txtNereyeGitti', 'txtHarcamaBirimi', 'lblNereyeGitti');
    }
}


var TriggerChange = function (field) {
    //field.getTrigger(0)[field.getRawValue().toString().length == 0 ? 'hide' : 'show']();

    if (field.id == "txtMuhasebe")
        KodAdGetir('MUHASEBE', 'txtMuhasebe', 'lblMuhasebeAd');
    else if (field.id == "txtHarcamaBirimi")
        KodAdGetir('HARCAMABIRIMI', 'txtMuhasebe:txtHarcamaBirimi', 'lblHarcamaBirimiAd');
    else if (field.id == "txtAmbar")
        KodAdGetir('AMBAR', 'txtMuhasebe:txtHarcamaBirimi:txtAmbar', 'lblAmbarAd');
    else if (field.id == "txtGonMuhasebe")
        KodAdGetir('MUHASEBE', 'txtGonMuhasebe', 'lblGonMuhasebeAd');
    else if (field.id == "txtGonHarcamaBirimi")
        KodAdGetir('HARCAMABIRIMI', 'txtGonMuhasebe:txtGonHarcamaBirimi', 'lblGonHarcamaBirimiAd');
    else if (field.id == "txtGonAmbar")
        KodAdGetir('AMBAR', 'txtGonMuhasebe:txtGonHarcamaBirimi:txtGonAmbar', 'lblGonAmbarAd');
    else if (field.id == "txtPersonel")
        KodAdGetir('PERSONEL', 'txtPersonel:txtMuhasebe:txtHarcamaBirimi', 'lblPersonelAd');
    else if (field.id == "txtKimeVerildi")
        KodAdGetir('PERSONEL', 'txtKimeVerildi:txtMuhasebe:txtHarcamaBirimi', 'lblKimeVerildi');
    else if (field.id == "txtOda")
        KodAdGetir('ODA', 'txtMuhasebe:txtHarcamaBirimi:txtOda', 'lblOdaAd');
    else if (field.id == "txtNereyeVerildi")
        KodAdGetir('ODA', 'txtMuhasebe:txtHarcamaBirimi:txtNereyeVerildi', 'lblNereyeVerildi');
    else if (field.id == "txtBulunduguYer")
        KodAdGetir('ODA', 'pgFiltre:prpMuhasebe:prpHarcamaBirimi:txtBulunduguYer', 'lblBulunduguYer');
    else if (field.id == "txtMarka")
        KodAdGetir('MARKA', 'txtMarka', 'lblMarkaAd');
    else if (field.id == "txtBolge")
        KodAdGetir('BOLGE', 'txtBolge', 'lblBolgeAd');
    else if (field.id == "txtHesapPlanKod")
        KodAdGetir('HESAPPLANI', 'txtHesapPlanKod', 'lblHesapPlanAd');
    else if (field.id == "txtNereyeVerildiMB")
        KodAdGetir('ODA', 'txtNereyeVerildiMB', 'lblHesapPlanAd');
}

var KodAdGetir = function (kod, veriAlani, hedefKontrol) {

    var alanlar = veriAlani.split(':');
    var alanDeger = "";
    for (var i = 0; i < alanlar.length; i++) {

        if (alanDeger != "") alanDeger += "|";

        if ((alanlar[i] != "" && alanlar[0] != "pgFiltre") || alanlar[i].indexOf("txt") > -1) {
            alanDeger += extKontrol.getCmp(alanlar[i]).getValue();
        }
        else {//MUHASEBE VE HARCAMA PROPERTY GRIDDEN SECILDIGINDE 
            if (i != 0 && i != 3)
                alanDeger += extKontrol.getCmp(alanlar[0]).getSource()[alanlar[i]];
            else if (i != 0) alanDeger += alanlar[3];
        }
    }

    if (alanDeger == "") {
        extKontrol.getCmp(hedefKontrol).setText("");
        return;
    }

    $.ajax({
        url: "../TasinirMal/AdGetir.ashx",
        type: 'GET',
        dataType: 'text',
        contentType: 'application/text; charset=utf-8',
        data: {
            kod: kod,
            alanDeger: alanDeger
        },
        success: function (result) {
            extKontrol.getCmp(hedefKontrol).setText(result);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });


}

function ListeAc(adres, parentText, parentCriteria, parentLabel) {
    var genislik = 500;
    var yukseklik = 420;

    var flagAmbar = 0;
    var flagHarcama = 0;
    var flagOda = 0;
    var flagPersonel = 0;
    var flagMarka = 0;
    var flagModel = 0;
    var flagSicilNo = 0;
    var flagHesapPlan = 0;
    var flagStok = 0;
    var flagGridYapistir = 0;

    if (parentText != '') {

        if (adres == "ListeAmbar.aspx")
            flagAmbar = 1;
        if (adres == "ListeHarcamaBirimi.aspx")
            flagHarcama = 1;
        if (adres == "ListeOda.aspx") {
            flagOda = 1;
            genislik = 720;
        }
        if (adres == "ListePersonel.aspx") {
            flagPersonel = 1;
            genislik = 620;
        }
        if (adres == "ListeSicilNoYeni.aspx") {
            flagSicilNo = 1;
            genislik = 800;
            yukseklik = 500;
            if (parentLabel == "TasinirTransfer") {
                genislik = 700;
                yukseklik = 440;
            }
        }
        if (adres == "ListeStokYeni.aspx") {
            flagStok = 1;
            genislik = 800;
            yukseklik = 500;
        }
        if (adres == "ListeMarka.aspx")
            flagMarka = 1;
        if (adres == "ListeModel.aspx")
            flagModel = 1;
        if (adres == "ListeHesapPlani.aspx")
            flagHesapPlan = 1;
        if (adres == "GridYapistir.aspx")
            flagGridYapistir = 1;
        if (adres == "ListeOdaMB.aspx") {
            flagOda = 1;
            genislik = 620;
        }

        adres += "?menuYok=1&cagiran=" + parentText + "&cagiranLabel=" + parentLabel;

        if (parentText.indexOf("GonMuhasebe") > -1 || parentText.indexOf("GonHarcama") > -1 || parentText.indexOf("GonAmbar") > -1)
            adres += "&butun=1";

        if (flagHesapPlan == 1) {

            try {
                if (hdnEkranTip.getValue() == "" && txtAmbar.getValue() == "51") {
                    adres += "&ekranTip=YZ";
                }
                else if (hdnEkranTip.getValue() == "" && txtAmbar.getValue() == "50") {
                    adres += "&ekranTip=GM";
                }
                else
                    adres += "&ekranTip=" + hdnEkranTip.getValue();
            } catch (e) { }

            HesapPlaniPenceresiAc(adres, 480, 400, ($(window).width() - 505), 30, null, "Taşınır Hesap Planı")
            return;
        }

        if (flagSicilNo == 1) {
            var yil = "";
            var muhasebeKod = txtMuhasebe.getRawValue();
            var harcamaBirimKod = txtHarcamaBirimi.getRawValue();
            var ambarKod = txtAmbar.getRawValue();
            var hesapKod = "";
            try {
                hesapKod = hdnHesapKod.getValue();
            } catch (e) { }

            adres += "&yil=" + yil;
            adres += "&mb=" + muhasebeKod;
            adres += "&hb=" + harcamaBirimKod;
            adres += "&ak=" + ambarKod;
            adres += "&hk=" + hesapKod;
            adres += "&kk=" + sicilListesiKisiDurumu;
            adres += "&ok=" + sicilListesiOdaDurumu;
            adres += "&listeTur=" + sicilListesiAmbarZimmetDurumu;
            adres += "&iliskiliMalzemeEkle=" + iliskiMalzemeEklemeDurumu;
            //adres += "&sicilNoListe=" + parentCriteria;
        }

        if (flagOda == 1)
            adres += "&kk=" + sicilListesiKisiDurumu;


        if (flagStok == 1) {
            var yil = 0;

            try {
                yil = txtYil.getValue();
            } catch (e) {
                yil = ddlYil.getValue();
            }

            var muhasebeKod = txtMuhasebe.getRawValue();
            var harcamaBirimKod = txtHarcamaBirimi.getRawValue();
            var ambarKod = txtAmbar.getRawValue();

            adres += "&yil=" + yil;
            adres += "&mb=" + muhasebeKod;
            adres += "&hb=" + harcamaBirimKod;
            adres += "&ak=" + ambarKod;
            adres += "&listeTur=" + sicilListesiAmbarZimmetDurumu;
        }

        if (flagAmbar == 1 || flagOda == 1 || flagPersonel == 1) {
            //Dağıtım/İade belgesi için sadece ambar gösteriliyor. Muh ve Hb üstteki olmalı
            if (parentText == "txtGonAmbar" && parentCriteria == "txtGonHarcamaBirimi") {
                if (extKontrol.getCmp(parentCriteria).getValue() == "")
                    parentCriteria = "txtHarcamaBirimi";
            }

            if (parentCriteria.xtype != "nettrigger") {
                if (parentCriteria.indexOf(":") > -1) {
                    var parentCriteria2 = parentCriteria.split(':');

                    if (extKontrol.getCmp(parentCriteria2[0]).getSource()[parentCriteria2[1]] == "" && flagPersonel != 1 && flagOda != 1) {
                        alert("Harcama birimi boş bırakılmış. Lütfen Harcama Birimi giriniz.");
                        return false;
                    }

                    adres += "&hb=" + extKontrol.getCmp(parentCriteria2[0]).getSource()[parentCriteria2[1]];
                }
                else {
                    if (extKontrol.getCmp(parentCriteria).getValue() == "" && flagPersonel != 1 && flagOda != 1) {
                        alert("Harcama birimi boş bırakılmış. Lütfen Harcama Birimi giriniz.");
                        return false;
                    }
                    adres += "&hb=" + extKontrol.getCmp(parentCriteria).getValue();
                }
            }
            else {
                if (parentCriteria.getName() == "txtHarcamaBirimi")
                    if (txtHarcamaBirimi.getRawValue() == "" && flagPersonel != 1 && flagOda != 1) {
                        alert("Harcama birimi boş bırakılmış. Lütfen Harcama Birimi giriniz.");
                        return false;
                    }

                adres += "&hb=" + txtHarcamaBirimi.getRawValue();
            }

            var muhDeger = "";
            if (parentCriteria == "txtGonHarcamaBirimi")
                muhDeger = extKontrol.getCmp("txtGonMuhasebe").getValue();
            else if (parentCriteria == "txtGirenHarcamaBirimi")
                muhDeger = extKontrol.getCmp("txtGirenMuhasebe").getValue();
            else if (parentCriteria == "txtCikanHarcamaBirimi")
                muhDeger = extKontrol.getCmp("txtCikanMuhasebe").getValue();
            else {
                if (parentCriteria.indexOf(":") > -1) {
                    muhDeger = extKontrol.getCmp(parentCriteria2[0]).getSource()["prpMuhasebe"];
                }
                else {
                    muhDeger = extKontrol.getCmp("txtMuhasebe").getValue();
                }
            }

            if (muhDeger == "" && flagPersonel != 1 && flagOda != 1) {
                alert("Muhasebe birimi boş bırakılmış. Lütfen Muhasebe Birimi giriniz.");
                return false;
            }

            adres += "&mb=" + muhDeger;

            try {
                adres += "&ab=" + extKontrol.getCmp("txtAmbar").getValue();
            }
            catch (e) { }
        }

        if (flagHarcama == 1 && parentCriteria.xtype != "nettrigger") {
            if (extKontrol.getCmp(parentCriteria).getValue() == "") {
                alert("Muhasebe birimi boş bırakılmış. Lütfen Muhasebe Birimi giriniz.");
                return false;
            }
            adres += "&mb=" + extKontrol.getCmp(parentCriteria).getValue();
        }
        else if (flagHarcama == 1 && parentCriteria.xtype == "nettrigger") {
            if (parentCriteria.getName() == "txtMuhasebe")
                if (txtMuhasebe.getRawValue() == "") {
                    alert("Muhasebe birimi boş bırakılmış. Lütfen Muhasebe Birimi giriniz.");
                    return false;
                }
            adres += "&mb=" + txtMuhasebe.getRawValue();
        }

        if (flagModel == 1) {
            if (extKontrol.getCmp(parentCriteria).getValue() == "") {
                alert("Marka boş bırakılmış. Lütfen Marka bilgisini giriniz.");
                return false;
            }
            adres += "&marka=" + extKontrol.getCmp(parentCriteria).getValue();
        }

    }

    showPopWin(adres, genislik, yukseklik, true, null);
}

function ListeAcProperty(adres, alanBilgi) {
    var genislik = 500;
    var yukseklik = 420;

    //Read property
    //propertyGrid.getSource()["propertyName"]
    //Set property
    //propertyGrid.setProperty(propName, value)
    //Add new property
    //propertyGrid.setProperty(propName, value, true)
    //Remove property
    //propertyGrid.removeProperty(propName)

    var propertyGrid = "";
    var parentText = "";
    var parentCriteria = "";
    if (alanBilgi.indexOf(":") > -1) {
        kontrolTuru = "propertyGrid";

        var bilgiler = alanBilgi.split(':');
        propertyGrid = bilgiler[0];
        parentText = bilgiler[1];
        if (bilgiler.length > 1)
            parentCriteria = bilgiler[2];

        extKontrol.getCmp(propertyGrid).stopEditing(false);
    }
    else
        return;

    var flagAmbar = 0;
    var flagHarcama = 0;
    var flagOda = 0;
    var flagPersonel = 0;
    var flagMarka = 0;
    var flagModel = 0;
    var flagSicilNo = 0;
    var flagHesapPlan = 0;

    if (parentText != '') {
        if (adres == "ListeAmbar.aspx")
            flagAmbar = 1;
        if (adres == "ListeHarcamaBirimi.aspx")
            flagHarcama = 1;
        if (adres == "ListeOda.aspx") {
            flagOda = 1;
            genislik = 720;
        }
        if (adres == "ListePersonel.aspx") {
            flagPersonel = 1;
            genislik = 620;
        }
        if (adres == "ListeSicilNoYeni.aspx") {
            flagSicilNo = 1;
            genislik = 800;
            yukseklik = 500;
        }
        if (adres == "ListeMarka.aspx")
            flagMarka = 1;
        if (adres == "ListeModel.aspx")
            flagModel = 1;
        if (adres == "ListeHesapPlani.aspx")
            flagHesapPlan = 1;

        adres += "?menuYok=1&cagiran=" + alanBilgi + "&cagiranLabel=";

        if (flagHesapPlan == 1) {

            HesapPlaniPenceresiAc(adres, 480, 400, ($(window).width() - 505), 30, null, "Taşınır Hesap Planı")
            return;
        }

        if (flagSicilNo == 1) {
            var yil = pgFiltre.source["prpYil"];
            var muhasebeKod = pgFiltre.source["prpMuhasebe"];
            var harcamaBirimKod = pgFiltre.source["prpHarcamaBirimi"];
            var ambarKod = pgFiltre.source["prpAmbar"];
            try {
                if (ambarKod == undefined) ambarKod = txtAmbar.getRawValue();
            } catch (e) { ambarKod = ""; }

            adres += "&yil=" + yil;
            adres += "&mb=" + muhasebeKod;
            adres += "&hb=" + harcamaBirimKod;
            adres += "&ak=" + ambarKod;
            adres += "&vermeDusme=-1";
            adres += "&listeTur=" + sicilListesiAmbarZimmetDurumu;
        }

        if (flagAmbar == 1 || flagOda == 1 || flagPersonel == 1) {
            //Dağıtım/İade belgesi için sadece ambar gösteriliyor. Muh ve Hb üstteki olmalı
            if (parentText == "prpGonAmbar" && parentCriteria == "prpGonHarcamaBirimi") {
                if (extKontrol.getCmp(propertyGrid).source[parentCriteria] == "")
                    parentCriteria = "prpHarcamaBirimi";
            }

            if (extKontrol.getCmp(propertyGrid).source[parentCriteria] == "" && flagPersonel != 1 && flagOda != 1) {
                alert("Harcama birimi boş bırakılmış. Lütfen Harcama Birimi giriniz.");
                return false;
            }
            adres += "&hb=" + extKontrol.getCmp(propertyGrid).source[parentCriteria];

            var muhDeger = "";
            if (parentCriteria == "txtGonHarcamaBirimi")
                muhDeger = extKontrol.getCmp(propertyGrid).source["prpGonMuhasebe"];
            else
                muhDeger = extKontrol.getCmp(propertyGrid).source["prpMuhasebe"];

            if (muhDeger == "" && flagPersonel != 1 && flagOda != 1) {
                alert("Muhasebe birimi boş bırakılmış. Lütfen Muhasebe Birimi giriniz.");
                return false;
            }

            adres += "&mb=" + muhDeger;

            try {
                adres += "&ab=" + extKontrol.getCmp(propertyGrid).source["prpAmbar"];
            }
            catch (e) { }
        }

        if (flagHarcama == 1) {
            if (extKontrol.getCmp(propertyGrid).source[parentCriteria] == "") {
                alert("Muhasebe birimi boş bırakılmış. Lütfen Muhasebe Birimi giriniz.");
                return false;
            }
            adres += "&mb=" + extKontrol.getCmp(propertyGrid).source[parentCriteria];
        }

        if (flagModel == 1) {
            if (extKontrol.getCmp(propertyGrid).source[parentCriteria] == "") {
                alert("Marka boş bırakılmış. Lütfen Marka bilgisini giriniz.");
                return false;
            }
            adres += "&marka=" + extKontrol.getCmp(propertyGrid).source[parentCriteria];
        }
    }

    showPopWin(adres, genislik, yukseklik, true, null);
}

function HesapPlaniKapat() {
    if (hesapPlaniPenceresi != null)
        hesapPlaniPenceresi.close();
}

function Sec(kod, childText) {
    extKontrol.getCmp(childText).setValue(kod);
}

function SecKapat(childText) {
    var parentText = extKontrol.getCmp("hdnCagiran").getValue();
    window.parent.extKontrol.getCmp(parentText).setValue(extKontrol.getCmp(childText).getValue());
    window.parent.hidePopWin();
}

function SecKapatDeger(deger, aciklama) {
    var parentText = extKontrol.getCmp("hdnCagiran").getValue();

    if (parentText.indexOf(":") > -1) {

        var bilgiler = parentText.split(':');
        propertyGrid = bilgiler[0];
        propName = bilgiler[1];

        window.parent.extKontrol.getCmp(propertyGrid).setProperty(propName, deger);
    }
    else if (parentText == "ext") {
        window.parent.SecKapatDeger(deger, aciklama);
    }
    else {
        var parentLabel = extKontrol.getCmp("hdnCagiranLabel").getValue();

        try {
            window.parent.extKontrol.getCmp(parentText).setValue(deger);
        } catch (e) {
            window.parent.document.getElementById(parentText).value = deger;
        }

        if (parentLabel != "" && parentLabel == 'lblMuhasebeAdHesapPlani') {

            try {
                window.parent.extKontrol.getCmp("hdnMuhasebeBirimi").setValue(deger);
            } catch (e) {
                window.parent.document.getElementById('hdnMuhasebeBirimi').value = deger;
            }

        }
        else if (parentLabel != "" && parentLabel == 'lblHarcamaBirimiAdHesapPlani') {

            try {
                window.parent.extKontrol.getCmp("hdnHarcamaBirimi").setValue(deger);
            } catch (e) {
                window.parent.document.getElementById('hdnHarcamaBirimi').value = deger;
            }

        }
        else if (parentLabel != "" && parentLabel == 'lblSicilNo') {

            try {
                degerler = aciklama.split('|');

                window.parent.extKontrol.getCmp("hdnPrSicilNo").setValue(degerler[0]);
                window.parent.extKontrol.getCmp(parentLabel).setText(degerler[1]);

            } catch (e) {
                window.parent.document.getElementById('hdnPrSicilNo').value = aciklama;
            }

        }
        else if (parentLabel != "") {

            try {
                window.parent.extKontrol.getCmp(parentLabel).setText(aciklama);
            } catch (e) {
                window.parent.document.getElementById(parentLabel).innerHTML = aciklama;
            }
        }
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
            dugme.value = "Gönderiliyor...";
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
        alert("Lütfen ilk önce hesap kodunu seçin.");
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
        alert("Lütfen ilk önce hesap kodunu seçin.");
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
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    var hdnFirmaHarcamadanAlma = document.getElementById('hdnFirmaHarcamadanAlma').value;
    firmaSecimYeri = kontrol;
    if (hdnFirmaHarcamadanAlma == "1") {
        showPopWin("ListeFirma.aspx", 580, 510, true, null);
    }
    else {
        showPopWin("../HarcamaSurecleri/ListeFirmaKisi.aspx?menuYok=1&per=1&muhasebe=" + muhasebeKod, 580, 510, true, null);
    }
}

function FirmaBilgisiGoster(firma) {
    try {
        document.getElementById(triggerClickKontrolId).value = firma.ad;
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

var winHesapPlani = null;
function HesapPlaniPenceresiAc(url, width, height, x, y, beforeCloseFunc, title) {
    var windowConfig = {
        id: "WindowCommonHesapPlani",
        autoShow: false,
        modal: false,
        width: width,
        height: height,
        initCenter: false,
        x: x,
        y: y,
        resizable: true,
        minimizable: false,
        maximizable: false,
        collapsible: true,
        title: title,
        hidden: false,
        closeAction: "close",
        listeners: {
            hide: function () {
                if (beforeCloseFunc != null)
                    eval(beforeCloseFunc);
            }
        },
        autoLoad: {
            url: url,
            mode: "iframe",
            showMask: true,
            maskMsg: "Lütfen Bekleyiniz...",
            nocache: true,
            triggerEvent: "show",
            reloadOnEvent: true
        }
    };

    winHesapPlani = extKontrol.getCmp(windowConfig.id);
    if (winHesapPlani) {
        winHesapPlani.show();
    }
    else
        winHesapPlani = new extKontrol.Window(windowConfig);
}

var GridKilitle = function (grd, colIndexlist) {
    $("#styleGridKilit").remove();
    var style = "";
    var index = 0;
    Ext1.each(grd.getColumnModel().columns, function (col) {
        if (col.id != "") {
            var kilitle = false;
            Ext1.each(colIndexlist, function (colIndex) {
                if (colIndex == index)
                    kilitle = true;
            });

            if (kilitle) {
                if (style != "")
                    style += ", ";
                style += ".x-grid3-body .x-grid3-td-" + col.id;
                if (col.editable)
                    col.editable = false;
            }
            else if (col.editable != null && col.editable == false)
                col.editable = true;
        }
        index++;
    });

    $('<style id="styleGridKilit">' + style + '{background-color: #FAFAD2;}</style>').appendTo('body');
}
