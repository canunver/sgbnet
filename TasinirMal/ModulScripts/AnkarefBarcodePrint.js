function PrintingTag(values, units) {
    for (var z = 0; z < values[0].length; z++) {
        Ext1.net.Mask.show({ msg: 'Etiket yazdırılıyor. Lütfen bekleyiniz...' });
        OnGetSelectedFieldValuesForFixref(values[0][z], units[z]);
    }
}

function OnGetSelectedFieldValuesForFixref(selectedValues, units) {
    var ekrandakiYazi = "Demirbaş için etiket yazdırlıyor...";
    var yazilacakBilgi = '{ "jobName": "' + ekrandakiYazi + '"';
    yazilacakBilgi += ', "template": ""'; // normal veya metal
    yazilacakBilgi += ', "pageCount": 1';
    yazilacakBilgi += ', "common": {}';
    yazilacakBilgi += ', "pages":[';

    var prSiciller = "";
    var isInvalidOccured = false;

    for (var i = 0; i < 1; i++) {

        if (!selectedValues[i][0]) {
            Ext1.Msg.show({ title: "Uyarı", msg: "RFID değeri boş olduğundan etiket basım işlemi gerçekleşmedi.", buttons: Ext1.Msg.OK })
            continue;
        }
        var rfid = selectedValues[0].toString(); //epc
        var serialNumber = selectedValues[1] == null ? "" : selectedValues[1].toString();
        var usage = selectedValues[2] == null ? "" : selectedValues[2].toString();
        var barkod = selectedValues[3] == null ? "" : selectedValues[3].toString();
        var description = selectedValues[4] == null ? "" : selectedValues[4].toString();
        var birim = selectedValues[5] == null ? "" : units.split('|')[0].toString();

        if (rfid != null) {
            prSiciller += "{";
            prSiciller += ' "rfid": "' + pad(rfid, 24) + '"';
            //prSiciller += ', "usageType":"' + usage + '"';
            //prSiciller += ', "epc":"' + rfid + '"';
            //prSiciller += ', "serialNumber":"' + serialNumber + '"';
            prSiciller += ', "description":"' + description + '"';
            prSiciller += ', "kill":"77777777"';
            prSiciller += ', "access":"77777777"';
            prSiciller += ', "unit":"' + birim + '"';
            prSiciller += ', "barcode": "' + barkod + '"';
            prSiciller += ' }';
        }
        else {
            isInvalidOccured = true;
        }
    }
    yazilacakBilgi += prSiciller + ']}';
    RFIDYaziciCagir(Base64.encode(yazilacakBilgi));
    if (isInvalidOccured) {
        Ext1.Msg.show({ title: "Uyarı", msg: "Geçersiz barkod numarası tespit edildi!", buttons: Ext1.Msg.OK })
    };
    return false;
}

function InventoryPointLabelPrint(selectedValues) {
    Ext1.net.Mask.show({ msg: 'Etiket yazdırılıyor. Lütfen bekleyiniz...' });
    var ekrandakiYazi = selectedValues.length + " malzeme için etiket yazdırılıyor...";
    var yazilacakBilgi = '{ "jobName": "' + ekrandakiYazi + '"';
    yazilacakBilgi += ', "template": ""'; // normal veya metal
    yazilacakBilgi += ', "pageCount": 1';
    yazilacakBilgi += ', "common": {}';
    yazilacakBilgi += ', "pages":[';

    var prSiciller = "";
    var isInvalidOccured = false;

    for (var i = 0; i < selectedValues.length; i++) {

        if (prSiciller != "") prSiciller += ",";

        var bastirilacakBilgi = selectedValues[i];

        if (!bastirilacakBilgi[0]) {
            Ext1.Msg.show({ title: "Uyarı", msg: "RFID değeri boş olduğundan etiket basım işlemi gerçekleşmedi.", buttons: Ext1.Msg.OK })
            continue;
        }
        var rfid = bastirilacakBilgi[0].toString(); //epc
        var barkod = bastirilacakBilgi[1] == null ? "" : bastirilacakBilgi[1].toString();
        var description = bastirilacakBilgi[2] == null ? "" : bastirilacakBilgi[2].toString();
        var birim = "";

        if (rfid != null) {
            prSiciller += "{";
            prSiciller += ' "rfid": "' + pad(rfid, 24) + '"';
            prSiciller += ', "usageType":""';
            prSiciller += ', "epc":"' + rfid + '"';
            prSiciller += ', "serialNumber":""';
            prSiciller += ', "description":"' + description + '"';
            prSiciller += ', "brand":""';
            prSiciller += ', "kill":"77777777"';
            prSiciller += ', "access":"77777777"';
            prSiciller += ', "unit":"' + birim + '"';
            prSiciller += ', "barcode": "' + barkod + '"';
            prSiciller += ' }';
        }
        else {
            isInvalidOccured = true;
        }
    }
    yazilacakBilgi += prSiciller + ']}';
    RFIDYaziciCagir(Base64.encode(yazilacakBilgi));
    if (isInvalidOccured) {
        Ext1.Msg.show({ title: "Uyarı", msg: "Geçersiz barkod numarası tespit edildi!", buttons: Ext1.Msg.OK })
    };
    return false;
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function RFIDYaziciCagir(yazilacakBilgi) {
    var yaziciURL = txtYaziciAdres.getValue();
    var url = location.href.substring(0, location.href.lastIndexOf("/") + 1);
    url = url + "TanimDemirbasRFIDBarkodYazdir.aspx?yaziciURL=" + yaziciURL + "&data=" + yazilacakBilgi;

    $.ajax({
        type: 'GET',
        url: url,
        success: function (data) {
            Ext1.Msg.show({ title: "Bilgi", msg: "Etiket Bastırıldı.", buttons: Ext1.Msg.OK })

            Ext1.net.Mask.hide();
        },
        error: function (param1, data, param2) {
            //    console.log(param1.status);
            if (param1.status == 0) {
                //Ext1.Msg.show({ title: "Uyarı", msg: "Yazıcıya bağlanılamadı.", buttons: Ext1.Msg.OK })
            }
            else if (param1.status == 200) {
                Ext1.Msg.show({ title: "Bilgi", msg: "Etiket Bastırıldı.", buttons: Ext1.Msg.OK })
            }
            else {
                Ext1.Msg.show({ title: "Bilgi", msg: "Status:" + param1.status + "<br>Status Text:" + param1.statusText, buttons: Ext1.Msg.OK })
            }
            Ext1.net.Mask.hide();
        }
    });
    //var fileref = document.createElement('script');
    //fileref.setAttribute("type", "text/javascript");
    ////fileref.setAttribute("src", "http://172.16.4.53:7890/" + yazilacakBilgi);
    //fileref.setAttribute("src", "http://" + window.yaziciURL + "/" + yazilacakBilgi);
    //if (typeof fileref != "undefined") {
    //    console.log("before");
    //    document.getElementsByTagName("head")[0].appendChild(fileref);
    //    console.log("end");
    //}
}

var Base64 = {
    // private property 
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

    // public method for encoding 
    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);
        while (i < input.length) {
            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
                this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
                this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
        }
        return output;
    },

    // public method for decoding 
    decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
        while (i < input.length) {
            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);
            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }
        }
        output = Base64._utf8_decode(output);
        return output;
    },
    // private method for UTF-8 encoding 
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";
        for (var n = 0; n < string.length; n++) {
            var c = string.charCodeAt(n);
            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }
        }
        return utftext;
    },
    // private method for UTF-8 decoding 
    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;
        while (i < utftext.length) {
            c = utftext.charCodeAt(i);
            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }
        }
        return string;
    }
}