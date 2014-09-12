function sendSoapRequest(service_uri, namespace, action, args, callback) {
    var isIE8 = window.XDomainRequest ? true : false;
    var xmlhttp;

    // build SOAP request
    var sr =
        '<?xml version="1.0" encoding="utf-8"?>' +
        '<soap12:Envelope ' +
            'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ' +
            'xmlns:xsd="http://www.w3.org/2001/XMLSchema" ' +
            'xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' +
            '<soap12:Body>' +
                '<' + action + ' xmlns="' + namespace + '">' +
                    OBJtoXML(args) +
                '</' + action + '>' +
            '</soap12:Body>' +
        '</soap12:Envelope>';

    if (isIE8) {
        xmlhttp = new XDomainRequest();
        xmlhttp.onerror = function (e) {
            alert("an error occured");
        };
        xmlhttp.ontimeout = function (e) {
            alert("timeout");
        };
        xmlhttp.onload = function () {
            var xmlObj = textToXML(xmlhttp.responseText)
            if (callback) callback(xmlObj);
        };
        var protocal = service_uri.split('/')[0];
        xmlhttp.open('POST', service_uri.replace(protocal, location.protocol));
        xmlhttp.send(sr);
    } else {
        if (window.XMLHttpRequest) {
            // code for IE7+, Firefox, Chrome, Opera, Safari
            xmlhttp = new XMLHttpRequest();
        } else {
            // code for IE6, IE5
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.open('POST', service_uri, true);
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4) {
                if (xmlhttp.status == 200) {
                    if (callback) callback(xmlhttp.responseXML);
                }
            }
        };
        xmlhttp.setRequestHeader('Content-Type', 'application/soap+xml; charset=utf-8');
        //xmlhttp.setRequestHeader('SOAPAction', namespace + action);
        xmlhttp.send(sr);
    }
}

function OBJtoXML(obj, d) {
    d = (d) ? d : 0;
    var rString = '';
    //var pad = '';
    //for (var i = 0; i < d; i++) {
    //    pad += ' ';
    //}
    if (typeof obj === 'object') {
        if (obj.constructor.toString().indexOf('Array') !== -1) {
            for (i = 0; i < obj.length; i++) {
                rString +=
                    //pad +
                    '<item>' + obj[i] +
                    '</item> ';
            }
            rString = rString.substr(0, rString.length - 1)
        }
        else {
            for (var i in obj) {
                var val = OBJtoXML(obj[i], d + 1);
                if (!val)
                    return false;
                rString +=
                    //((rString === '\n') ? '' : '\n') +
                    //pad +
                    '<' + i + '>' + val +
                    //((typeof obj[i] === 'object') ? '\n' + pad : '') +
                    '</' + i + '>';
            }
        }
    }
    else if (typeof obj === 'string') {
        rString = obj;
    }
    else if (obj.toString) {
        rString = obj.toString();
    }
    else {
        return false;
    }
    return rString;
}

function getReturnValue(responseXML) {
    var soapBody = responseXML.getElementsByTagName('Body')[0];
    if (!soapBody) soapBody = responseXML.getElementsByTagName('soap:Body')[0]
    return XMLtoOBJ(soapBody);
}

function JSONtoXML(json) {
    return eval('OBJtoXML(' + json + ');');
}

function XMLtoOBJ(xml) {
    // http://davidwalsh.name/convert-xml-json
    // Create the return object
    var obj = {};

    if (xml.nodeType == 1) { // element
        // do attributes
        if (xml.attributes.length > 0) {
            obj["@attributes"] = {};
            for (var j = 0; j < xml.attributes.length; j++) {
                var attribute = xml.attributes.item(j);
                obj["@attributes"][attribute.nodeName] = attribute.nodeValue;
            }
        }
    } else if (xml.nodeType == 3) { // text
        obj = xml.nodeValue;
    }

    // do children
    if (xml.hasChildNodes()) {
        for (var i = 0; i < xml.childNodes.length; i++) {
            var item = xml.childNodes.item(i);
            var nodeName = item.nodeName;
            if (typeof (obj[nodeName]) == "undefined") {
                obj[nodeName] = XMLtoOBJ(item);
            } else {
                if (typeof (obj[nodeName].push) == "undefined") {
                    var old = obj[nodeName];
                    obj[nodeName] = [];
                    obj[nodeName].push(old);
                }
                obj[nodeName].push(XMLtoOBJ(item));
            }
        }
    }
    return obj;
};

function textToXML(xmlStr) {
    var parseXml;

    if (window.DOMParser) {
        parseXml = function (xmlStr) {
            return (new window.DOMParser()).parseFromString(xmlStr, "text/xml");
        };
    } else if (typeof window.ActiveXObject != "undefined" && new window.ActiveXObject("Microsoft.XMLDOM")) {
        parseXml = function (xmlStr) {
            var xmlDoc = new window.ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = "false";
            xmlDoc.loadXML(xmlStr);
            return xmlDoc;
        };
    } else {
        parseXml = function () { return null; }
    }

    var xmlDoc = parseXml(xmlStr);
    return xmlDoc;
}