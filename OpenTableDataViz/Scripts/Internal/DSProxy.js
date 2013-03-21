var DSProxy = function () //constructor for the proxy
{
	var doAjax = function(urlSegment, requestType, data, successCallback, errorCallback, argumentsToSuccessCallback, async) {
		var fullUrl;

		if (!data) {
			data = { };
		}

		fullUrl = getFullUrl(urlSegment);

		if (!errorCallback) errorCallback = globalOnAjaxError;

		jQuery.support.cors = true;
		$.ajax({
			async: async,
			type: requestType,
			url: fullUrl,
			data: data,
			contentType: "application/json; charset=utf-8",
			cache: false,
			timeout: 5000,
			headers: {
								
			},
			success: function(returnData) {
				if (typeof successCallback !== "undefined" && successCallback !== null) {
					successCallback(returnData, argumentsToSuccessCallback);
				}
			},
			error: function(jqXhr, textStatus, errorThrown) {
				if (typeof errorCallback !== "undefined" && errorCallback !== null) {
					errorCallback(jqXhr, textStatus);
				}
			}
		});
	},
	getFullUrl = function (urlSegment) {
		var sBaseUrl = window.location.protocol + "//" + window.location.host;
		return sBaseUrl + "/" + urlSegment;
	},
	globalOnAjaxError = function (oResponse, sErrorType) {
		// Global Handler for AJAX errors
		// Possible values for sErrorType are - null, timeout, error, abort, parseerror
		try {
			var sErrorMessage = '';
			// Custom process response uner following conditions - 
			// 2 - Request is fully loaded
			// 3 - Request is waiting for user interaction
			// 4 - Request is complete
			if (oResponse.readyState == 2 || oResponse.readyState == 3 || oResponse.readyState == 4) {
				// Get the Error object
				var oError = jQuery.parseJSON(oResponse.responseText);
				if (oError.ErrorMessages != null && oError.ErrorMessages.length > 0)
					console.log(oError.ErrorMessages[0]);
			}
				// When readystate is any of the following, just log the url and the statusMessage in reponse since responseText won't be available
				// 0 - Object is uninitialized
				// 1 - Request is loading    
			else {
				sErrorMessage = "URL:" + window.location;
				sErrorMessage += "AJAX Response StatusCode:" + oResponse.status;
				sErrorMessage += "Status text:" + oResponse.statusText;
				console.log(sErrorMessage);
			}
		} catch(e) {
			// Global On AJAX error - if some error happens in this logic, don't throw or log to avoid infinite loop.
			console.log(e);
		}
	};

	return {
		DoAjax: doAjax,
		globalOnAjaxError: globalOnAjaxError
	};
}();

// Catches all uncaught javascript exceptions
window.onerror = function (msg, url, linenumber) {
	var sErrorMessage = String.format("Error:{0}. Url:{1}. LineNumber:{2}", msg, url, linenumber);
	console.log(sErrorMessage);
};