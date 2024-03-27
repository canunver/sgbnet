function SeciliKodBul() {
    var kodlar = "";
    var seciliNodelar = trvHesap.getChecked();

    Ext1.each(seciliNodelar, function (node) {
        if (kodlar.length > 0) {
            if (hdnBilgi.getValue() == "1")
                kodlar += " @ ";
            else
                kodlar += "-";
        }
        if (hdnBilgi.getValue() == "1")
            kodlar += node.text;
        else
            kodlar += node.id;
    });

    SecKapat(kodlar, "-");
}

function DalYukleArama(deger) {
    var deger = deger;
    if (deger == 'arama')
        var tree = trvHesapArama;
    else if (deger == 'barkod')
        var tree = trvHesapAramaBarkod;
    Ext1.net.DirectMethods.DalYukleArama({
        success: function (result) {
            var nodes = eval(result);
            if (nodes.length > 0) {
                tree.initChildren(nodes);
            }
            else {
                tree.getRootNode().removeChildren();
            }
        }
    });
}

function DalYukle(node, coklu) {
    var zaman = new Date();
    Ext1.net.DirectMethod.request(
        'DalYukle',
        {
            success: function (result) {
                if (result == "")
                    return;
                var data = eval("(" + result + ")");
                node.loadNodes(data);
            },

            failure: function (errorMsg) {
                Ext1.Msg.alert('Failure', errorMsg);
            },
            specifier: 'static',
            params: { hesapKod: node.id, coklu: coklu, zaman: zaman.toString() }
        }
    );
}

function NodeKodSecKapat(node) {
    var parentText = extKontrol.getCmp("hdnCagiran").getValue();
    var kodAd = node.attributes.cls.split('|');

    if (parentText.indexOf(":") > -1) {

        var bilgiler = parentText.split(':');
        var kontrol = bilgiler[0];

        if (kontrol.indexOf("grd") == 0) {
            var grid = window.parent.extKontrol.getCmp(bilgiler[0]);
            var row = parent.gridYazilacakSatirNo;//Number(bilgiler[1]);
            var colKod = bilgiler[1];
            var colAdi = "";
            var colOlcu = "";
            var colKDV = "";
            var colRfidEtiketKod = "";
            var colMarkaKod = "";
            var colModelKod = "";

            if (bilgiler.length > 2) colAdi = bilgiler[2];
            if (bilgiler.length > 3) colOlcu = bilgiler[3];
            if (bilgiler.length > 4) colKDV = bilgiler[4];
            if (bilgiler.length > 5) colRfidEtiketKod = bilgiler[5];
            if (bilgiler.length > 6) colMarkaKod = bilgiler[6];
            if (bilgiler.length > 7) colModelKod = bilgiler[7];

            var store = grid.getStore();
            if (colKod != "") store.getAt(row).set(colKod, kodAd[0]);
            if (colAdi != "") store.getAt(row).set(colAdi, kodAd[1]);
            if (colOlcu != "") store.getAt(row).set(colOlcu, kodAd[2]);
            if (colKDV != "") store.getAt(row).set(colKDV, kodAd[3]);
            if (colRfidEtiketKod != "") store.getAt(row).set(colRfidEtiketKod, kodAd[4]);
            if (colMarkaKod != "") store.getAt(row).set(colMarkaKod, kodAd[5]);
            if (colModelKod != "") store.getAt(row).set(colModelKod, kodAd[6]);
        }
        else {
            var propertyGrid = bilgiler[0];
            var propName = bilgiler[1];
            try {
                window.parent.extKontrol.getCmp(propertyGrid).setProperty(propName, kodAd[0]);
            } catch (e) {
                parent.extKontrol.getCmp(propertyGrid).setProperty(propName, kodAd[0]);
            }
        }

        return;
    }
    else if (parentText.indexOf("txt") > -1) {
        SecKapatDeger(kodAd[0], kodAd[1]);
        return;
    }
    try {
        opener.NodeSecildiYerel(node);
        return false;
    }
    catch (e) {
        parent.NodeSecildiYerel(node); return false;
    }
}

function SecKapat(secili, ayrac) {
    try {
        opener.NodeSecildiYerelCoklu(secili);
        opener.focus();
    }
    catch (e) {
        parent.NodeSecildiYerelCoklu(secili);
    }
}