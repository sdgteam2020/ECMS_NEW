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
function SubmitsEncry1(result) {
   
    if (result) {
        // Get the password value
        let txtpassword = $("#Password").val();
        let skey = $('#spnhdns').html();

        if (txtpassword == "" || $('#ConfirmPassword').val() == "" || $('#ICNo').val() == "") {
            alert('Please enter Army No / Password / ConfirmPassword.');
            return false;
        }
        if ($('#Password').val() == $('#ConfirmPassword').val()) {

            // Regular expression to check password rules
            var passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,15}$/;

            // Validate password against the regular expression
            if (passwordPattern.test(txtpassword)) {
                var key = CryptoJS.enc.Utf8.parse(skey);
                var iv = CryptoJS.enc.Utf8.parse(skey);
                var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

                    { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });

                $('#Password').val(encryptedpassword);
                $('#ConfirmPassword').val(encryptedpassword);
                return true;
            }
            else {
                alert('Password must be 8-15 characters long, contain at least one uppercase letter, one digit, and one special character.');
                return false;
            }
        }
        else {
            alert('Password and confirmation password do not match.');
            return false;
        }
    }
    else {
        let txtpassword = $('#Password').val();
        let skey = $('#spnhdns').html();

        if (txtpassword == "" || $('#ICNo').val() == "") {
            alert('Please enter Army No / Password.');
            return false;
        }
        else {
            var key = CryptoJS.enc.Utf8.parse(skey);
            var iv = CryptoJS.enc.Utf8.parse(skey);
            var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

                { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });

            $('#Password').val(encryptedpassword);
            $('#ConfirmPassword').val(encryptedpassword);
            return true;
        }
    }

}