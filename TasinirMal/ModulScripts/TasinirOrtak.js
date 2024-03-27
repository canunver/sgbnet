var HesapPlaniAdParcala = function (h) {
    if (h != null)
        return h.match(/(?=\S)[^-]+?(?=\s*(-|$))/g);
    return null;
}

function VeriGosterFX(value, funcName, ek) {
    FX(value, funcName, ek);
    if (ek == 'parent.') { parent.hidePopWin(); }
}

function FX(value, funcName, ek) {
    var func = new Function(ek + funcName + "('" + value + "')");
    return func();
}

function GosterGizle(element1, element2, e, eValue, eValueG) {
    element1 = document.getElementById(element1);
    if (element2 != "")
        element2 = document.getElementById(element2);

    if (element1.style.display == "none") {
        element1.style.display = "block";
        if (element2 != "")
            element2.style.display = "none";
        e.innerHTML = eValueG;
        try { document.getElementById('hdnGosterGizleDurum').value = "1" } catch (e) { }
    }
    else {
        element1.style.display = "none";
        if (element2 != "")
            element2.style.display = "block";
        e.innerHTML = eValue;
        try { document.getElementById('hdnGosterGizleDurum').value = "" } catch (e) { }
    }
}

var ApplyFilter = function (field) {
    var store = grdListe.getStore();
    store.suspendEvents();
    store.filterBy(getRecordFilter(field));
    store.resumeEvents();
    grdListe.getView().refresh(false);
};

var ClearFilter = function () {
    txtFiltre.reset();
    var store = grdListe.getStore();
    store.clearFilter();
}

var ApplyFilterV3 = function (field) {
    var store = App.grdListe.getStore();
    store.suspendEvents();
    store.filterBy(GetRecordFilterV3(field));
    store.resumeEvents();
    App.grdListe.getView().refresh(false);
};

var ClearFilterV3 = function () {
    App.txtFiltre.reset();
    var store = App.grdListe.getStore();
    store.clearFilter();
}

var GetRecordFilterV3 = function (field) {
    var f = [];
    if (field == undefined)
        field = "AD";

    f.push({
        filter: function (record) {
            return filterString(App.txtFiltre.getValue(), field, record);
        }
    });

    var len = f.length;

    return function (record) {
        for (var i = 0; i < len; i++) {
            if (!f[i].filter(record)) {
                return false;
            }
        }
        return true;
    };
};


var filterString = function (value, dataIndex, record) {
    var val = record.get(dataIndex);

    if (typeof val != "string") {
        return value.length == 0;
    }

    return val.toLowerCase().indexOf(value.toLowerCase()) > -1;
};

var getRecordFilter = function (field) {
    var f = [];
    if (field == undefined)
        field = "AD";

    f.push({
        filter: function (record) {
            return filterString(txtFiltre.getValue(), field, record);
        }
    });

    var len = f.length;

    return function (record) {
        for (var i = 0; i < len; i++) {
            if (!f[i].filter(record)) {
                return false;
            }
        }
        return true;
    };
};

