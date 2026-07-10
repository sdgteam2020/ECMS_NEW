$(function () {

    const form = document.getElementById('frmTokenValidate');
    if (!form) return;

    form.addEventListener('submit', async function (event) {
        event.preventDefault();

        if ($(form).valid && !$(form).valid()) {
            return false;
        }

        const result = await SubmitsEncry1();

        if (!result) {
            return false;
        }

        form.submit();
    });

    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });
});

async function SubmitsEncry1() {

    let ArmyNo = document.getElementById('ICNo').value;
    let skey = $('#spnhdns').html();

    if ($('#ICNo').val() == "") {
        alert('Please enter Army No.');
        return false;
    }

    let result = await ChkSfx();

    if (!result) {
        return false; // Stop submission if suffix check fails
    }

    var encryptedArmyNo = encryptData(ArmyNo, skey);
    $('#ICNo').val(encryptedArmyNo);
    return true;

}
async function ChkSfx() {
    // Simulating an asynchronous check (no external fetch needed)
    await new Promise(resolve => resolve()); // The 'await' here is just for structure

    let ArmyNo = document.getElementById('ICNo').value.trim().toUpperCase();

    if (ArmyNo.length < 8 || ArmyNo.length > 9) {
        alert('Invalid Army No.');
        return false;
    }

    const allowedPrefixes = ["IC", "SL", "SS", "WC", "TA"];
    const prefix = ArmyNo.substring(0, 2);

    if (!allowedPrefixes.includes(prefix)) {
        alert('Invalid Army No prefix.');
        return false;
    }

    const regex = /^(IC|SL|SS|WC|TA)\d{5,6}[A-Z]$/;

    if (!regex.test(ArmyNo)) {
        alert('Invalid Army No.');
        return false;
    }
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
        alert("Invalid suffix of Army No.");
        return false;
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