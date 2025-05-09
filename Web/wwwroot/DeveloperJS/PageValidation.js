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
function ChkSfx() {
    let ArmyNo = document.getElementById('ICNo').value;
    const isFirstTwoAlpha = /^[A-Za-z]{2}/.test(ArmyNo);

    if (ArmyNo.length < 7 || isFirstTwoAlpha == false) {
        alert('Invalid Army No.');
        return false;
    }
    var armyno = ArmyNo.replace(/[A-Za-z]/g, '');

    var txt = ArmyNo.slice(-1);
    // Get last character
    const lastChar = ArmyNo.slice(-1);

    // Check if it is an alphabet
    const isAlpha = /^[A-Za-z]$/.test(lastChar);
    if (txt == "" || isAlpha == false) {
        alert('Invalid Army No.');
        return false;
    }
    var vlength = armyno.length;
    var NumMulti = parseInt(vlength) + 1;
    var vMulti = 0;
    var vSum = 0;
    var Sfx;
    for (var i = 0; i < vlength; i++) {
        vMulti = parseInt(armyno.charAt(i)) * parseInt(NumMulti);
        vSum = parseInt(vSum) + parseInt(vMulti);
        NumMulti = parseInt(NumMulti) - 1;

    }

    var Reminder = parseInt(vSum) % 11;
    switch (Reminder) {
        case (0):
            {
                Sfx = "A"
                break;
            }
        case (1):
            {
                Sfx = "F"
                break;
            }
        case (2):
            {
                Sfx = "H"
                break;
            }
        case (3):
            {
                Sfx = "K"
                break;
            }
        case (4):
            {
                Sfx = "L"
                break;
            }
        case (5):
            {
                Sfx = "M"
                break;
            }
        case (6):
            {
                Sfx = "N"
                break;
            }
        case (7):
            {
                Sfx = "P"
                break;
            }
        case (8):
            {
                Sfx = "W"
                break;
            }
        case (9):
            {
                Sfx = "X"
                break;
            }
        case (10):
            {
                Sfx = "Y"
                break;
            }
    }
    //var txtcalsfx = document.getElementById('ICNo');
    //txtcalsfx.value = Sfx;
    if (txt.toUpperCase() == Sfx) {
        return true;
    }
    else {
        alert("Suffix Mismatch.Expected suffix is " + Sfx);
        return false;
    }
}
function SubmitsEncry1(result) {
   
    if (result) {
        // Get the password value
        let txtpassword = $("#Password").val();
        let skey = $('#spnhdns').html();

        if (txtpassword == "" || $('#ConfirmPassword').val() == "" || $('#ICNo').val() == "") {
            alert('Please enter Army No / Password / ConfirmPassword.');
            return false;
        }
        if (!ChkSfx()) {
            return false; // Stop submission if suffix check fails
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
            if (!ChkSfx()) {
                return false; // Stop submission if suffix check fails
            }
            var key = CryptoJS.enc.Utf8.parse(skey);
            var iv = CryptoJS.enc.Utf8.parse(skey);
            var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

                { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });

            $('#Password').val(encryptedpassword);
            //$('#ConfirmPassword').val(encryptedpassword);
            return true;
        }
    }

}