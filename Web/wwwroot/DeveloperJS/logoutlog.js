$(function () {
var dataToSend = {
    domainName: $("#spnlogoutDomainId").html(),
    appName: $("#spnlogoutAppName").html(),
    appRoleName: $("#spnlogoutRoleName").html(),
    flexible: ''
};

// Create a query string from the data object
var queryString = Object.keys(dataToSend).map(function (key) {
    return key + '=' + dataToSend[key];
}).join('&');

// Make an AJAX request with the data in the URL
$.ajax({
    url: 'iam2.army.mil/IAM/singleAppConfirmLoginResponse.htm?' + queryString,
    type: 'GET',
    dataType: 'html',
    success: function (response) {
        // Display the response in the 'result' div
        $('#result').html(response);
    },
    error: function (xhr, status, error) {
        console.error(xhr.responseText);
    }
});
});