function SeciliKodBul() {
    var kodlar = "";
    var seciliNodelar = trvOda.getChecked();

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
    var tree = trvOda;
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

    var kod = node.attributes.cls.split("|");
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
            params: { odaKod: kod[2] }
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
            if (bilgiler[1] == "hepsi") {
                parent.YerleskeHepsiniDoldur(kodAd[0], kodAd[1]);
            }
            else {
                var grid = window.parent.extKontrol.getCmp(bilgiler[0]);
                var row = parent.gridYazilacakSatirNo;//Number(bilgiler[1]);
                var colKod = bilgiler[1];
                var colAdi = "";

                if (bilgiler.length > 2) colAdi = bilgiler[2];

                var store = grid.getStore();
                if (colKod != "") store.getAt(row).set(colKod, kodAd[0]);
                if (colAdi != "") store.getAt(row).set(colAdi, kodAd[1]);
            }
            window.parent.hidePopWin();
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