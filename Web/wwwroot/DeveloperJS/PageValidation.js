$(document).ready(function () {


    document.addEventListener('DOMContentLoaded', function () {
        // Push a new state to the browser history
        history.pushState(null, null, location.href);

        // Listen for the popstate event
        window.addEventListener('popstate', function (event) {
            // Prevent the back button action by pushing the state again
            history.pushState(null, null, location.href);
            alert('Back navigation is disabled!');
        });
    });


});
function SubmitsEncry1() {
  
    let txtpassword = $('#Password').val();
    let skey = $('#spnhdns').html();

    if (txtpassword == "") {
        alert('Please enter Password');
        return false;
    }
    else {
        var key = CryptoJS.enc.Utf8.parse(skey);
        var iv = CryptoJS.enc.Utf8.parse(skey);
        var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });

        $('#Password').val(encryptedpassword);
        return true;
    }
}