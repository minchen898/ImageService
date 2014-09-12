var servicePath = 'http://localhost:11202/';

(function () {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = servicePath + 'js/soap.js';
    //script.charset = 'utf-8';
    head.appendChild(script);
})();

function uploadImage(file_content, cutSetting, sizeSetting, processOption, callback) {
    var args = {};
    args['image'] = file_content;
    if (cutSetting) args['cutSetting'] = cutSetting;
    if (sizeSetting) args['sizeSetting'] = sizeSetting;
    args['processOption'] = (processOption ? processOption : 'CutFirst');

    sendSoapRequest(
        servicePath + 'UploadImage.asmx',
        'http://tempuri.org/',
        'UploadFile',
        args,
        function (responseXML) {
            if (callback) {
                callback(servicePath + 'LoadImage.aspx?id=' +
                    getReturnValue(responseXML).UploadFileResponse.UploadFileResult['#text']);
            }
        }
    );
}

function uploadImageByUrl(url, cutSetting, sizeSetting, processOption, callback) {
    var args = {};
    args['url'] = encodeURIComponent(url);
    if (cutSetting) args['cutSetting'] = cutSetting;
    if (sizeSetting) args['sizeSetting'] = sizeSetting;
    args['processOption'] = (processOption ? processOption : 'CutFirst');

    sendSoapRequest(
        servicePath + 'UploadImage.asmx',
        'http://tempuri.org/',
        'UploadImageByUrl',
        args,
        function (responseXML) {
            if (callback) {
                callback(servicePath + 'LoadImage.aspx?id=' +
                    getReturnValue(responseXML).UploadImageByUrlResponse.UploadImageByUrlResult['#text']);
            }
        }
    );
}

function calculateResizeRate(imageUrl, newWidth, newHeigth, option, callback) {
    var args = {};
    args['imageUrl'] = encodeURIComponent(imageUrl);
    args['size'] = { 'Width': newWidth, 'Height': newHeigth };
    args['resizeOption'] = option;

    sendSoapRequest(
        servicePath + 'ProcessImage.asmx',
        'http://tempuri.org/',
        'CalculateResizeRate',
        args,
        function (responseXML) {
            if (callback) callback(parseFloat(getReturnValue(responseXML).CalculateResizeRateResponse.CalculateResizeRateResult['#text']));
        }
    );
}

function generateTextImage(text, font, fontSize, fontStyle, color, size, callback) {
    var args = {};
    args['text'] = text;
    args['stringFont'] = font;
    args['fontSize'] = fontSize;
    if (fontStyle) args['fontStyles'] = { 'FontStyle': fontStyle };
    args['stringColor'] = color;
    if (size) args['imageSize'] = size;
    args['isVertical'] = false;

    sendSoapRequest(
        servicePath + 'ProcessImage.asmx',
        'http://tempuri.org/',
        'GenerateTextImage',
        args,
        function (responseXML) {
            if (callback) callback(getReturnValue(responseXML).GenerateTextImageResponse.GenerateTextImageResult['#text']);
        }
    );
}

function generateVerticalTextImage(text, font, fontSize, fontStyle, color, size, callback) {
    var args = {};
    args['text'] = text;
    args['stringFont'] = font;
    args['fontSize'] = fontSize;
    if (fontStyle) args['fontStyles'] = { 'FontStyle': fontStyle };
    args['stringColor'] = color;
    if (size) args['imageSize'] = size;
    args['isVertical'] = true;

    sendSoapRequest(
        servicePath + 'ProcessImage.asmx',
        'http://tempuri.org/',
        'GenerateTextImage',
        args,
        function (responseXML) {
            if (callback) callback(getReturnValue(responseXML).GenerateTextImageResponse.GenerateTextImageResult['#text']);
        }
    );
}

function generateCutSetting(width, height, position, pointX, pointY, previewRate) {
    var setting = {};
    setting['RegionSize'] = { 'Width': width, 'Height': height };
    setting['Position'] =position;
    if (position == 'Custom') {
        setting['LeftTopPosition'] = { 'X': pointX, 'Y': pointY };
        setting['PreviewRate'] = previewRate;
    }
    return setting;
}

function generateSizeSetting(width, height, option) {
    var setting = {};
    setting['FrameSize'] = { 'Width': width, 'Height': height };
    setting['Option'] = option;
    return setting;
}