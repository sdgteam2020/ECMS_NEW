$(function () {
    const btn = document.getElementById('btnProceed');
    if (!btn) return;

    btn.addEventListener('click', function (event) {
        const isNewUser = btn.dataset.isNewUser === "true";
        handleSubmit(event, isNewUser);
    });

    const pwd = document.getElementById("Password");
    if (!pwd) return;

    pwd.addEventListener("paste", function (e) {
        e.preventDefault();
    });

    pwd.addEventListener("copy", function (e) {
        e.preventDefault();
    });

    pwd.addEventListener("cut", function (e) {
        e.preventDefault();
    });

    if ($('#ConfirmPassword').length) {
        const confirmpwd = document.getElementById("ConfirmPassword");
        if (!confirmpwd) return;

        confirmpwd.addEventListener("paste", function (e) {
            e.preventDefault();
        });

        confirmpwd.addEventListener("copy", function (e) {
            e.preventDefault();
        });

        confirmpwd.addEventListener("cut", function (e) {
            e.preventDefault();
        });
    }

    // Push a new state to the browser history
    history.pushState(null, null, location.href);

    // Listen for the popstate event
    window.addEventListener('popstate', function (event) {
        // Prevent the back button action by pushing the state again
        history.pushState(null, null, location.href);
        alert('Back navigation is disabled!');
    });
    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });
});
async function handleSubmit(event, isNewUser) {
    try {
        const result = await SubmitsEncry1(isNewUser);
        if (!result) {
            event.preventDefault();  // Prevent form submission if the result is false
            return false; // Return false to ensure the button does not trigger the default form submit action
        }
    } catch (error) {
        console.error("Error in submission:", error);
        event.preventDefault();  // Prevent form submission in case of an error
        return false; // Handle the error by preventing the form submission
    }
}
async function ChkSfx() {
    // Simulating an asynchronous check (no external fetch needed)
    await new Promise(resolve => resolve()); // The 'await' here is just for structure

    let ArmyNo = document.getElementById('ICNo').value;

    if (ArmyNo.length < 8 || ArmyNo.length > 9) {
        alert('Army number must be 8-9 characters long.');
        return false;
    }
    else {
        const regex = /^[A-Z]{2}\d{5,6}[A-Z]$/;

        if (!regex.test(ArmyNo)) {
            alert('Invalid format. Must be: 2 letters + 5-6 digits + 1 letter (e.g., IC12345X, SC123456P).');
            return false;
        }
        else {
            var armyno = ArmyNo.replace(/[A-Za-z]/g, '');

            var txt = ArmyNo.slice(-1);

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
            if (txt.toUpperCase() === Sfx) {
                return true;
            }
            else {
                alert("Invalid Army No.");
                return false;
            }
        }


    }

}

async function SubmitsEncry1(result) {
   
    if (result) {
        // Get the password value
        let txtpassword = $("#Password").val();
        let ArmyNo = document.getElementById('ICNo').value;
        let skey = $('#spnhdns').html();

        if (txtpassword == "" || $('#ConfirmPassword').val() == "" || $('#ICNo').val() == "") {
            alert('Please enter Army No / Password / ConfirmPassword.');
            return false;
        }

        let result = await ChkSfx();

        if (!result) {
            return false; // Stop submission if suffix check fails
        }

        if ($('#Password').val() == $('#ConfirmPassword').val()) {

            // Regular expression to check password rules
            var passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,15}$/;

            // Validate password against the regular expression
            if (passwordPattern.test(txtpassword)) {

                var encryptedpassword = encryptData(txtpassword, skey);
                var encryptedArmyNo = encryptData(ArmyNo, skey);
                $('#ICNo').val(encryptedArmyNo);
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
        let ArmyNo = document.getElementById('ICNo').value;
        let skey = $('#spnhdns').html();

        if (txtpassword == "" || $('#ICNo').val() == "") {
            alert('Please enter Army No / Password.');
            return false;
        }
        else {
            let result = await ChkSfx();
            if (!result) {
                return false; // Stop submission if suffix check fails
            }

            var encryptedpassword = encryptData(txtpassword, skey);
            var encryptedArmyNo = encryptData(ArmyNo, skey);
            $('#ICNo').val(encryptedArmyNo);

            $('#Password').val(encryptedpassword);
            return true;
        }
    }

}
function encryptData(plainText, secretKey) {
    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16)); // 16 bytes

    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return encrypted.toString();   // Base64 output
}
function decryptData(cipherText, secretKey) {
    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16));

    return CryptoJS.AES.decrypt(cipherText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    }).toString(CryptoJS.enc.Utf8);
}