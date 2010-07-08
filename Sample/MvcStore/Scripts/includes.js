var scriptTags = "";
var cssTags = "";
function includeJs(jsFile) {
    var scriptTag = '<scr' + 'ipt type="text/javascript" language="javascript" src="' + jsFile + '"></script>';
    scriptTags += scriptTag + "\r\n";
}

//Parsing current controller and page name for page specific script and css injection
var lastIndexOfViews = document.location.href.lastIndexOf("Views/");
var lastIndexOfDot = document.location.href.lastIndexOf(".");
var controllerPageName = document.location.href.substring(lastIndexOfViews + 6, lastIndexOfDot);

var indexOfSlash = controllerPageName.lastIndexOf("/");
document.controllerName = controllerPageName.substring(0, indexOfSlash);
document.pageName = controllerPageName.substring(indexOfSlash + 1, controllerPageName.length);

includeJs("/Scripts/jquery-1.4.1.min.js");
includeJs("/Scripts/jquery.validate.min.js");
includeJs("/Scripts/MicrosoftAjax.js");
includeJs("/Scripts/MicrosoftMvcAjax.js");
includeJs("/Scripts/MicrosoftMvcValidation.js");
includeJs("/Scripts/Views/" + controllerPageName + ".js");

