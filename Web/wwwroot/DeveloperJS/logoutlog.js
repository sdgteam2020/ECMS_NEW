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
    let ArmyNo = document.getElementById('ICNo').value.trim().toUpperCase();

    // Validate Army No length (7 to 9 characters)
    if (ArmyNo.length < 7 || ArmyNo.length > 9) {
        toastr.error("Army No must be between 7 and 9 characters.");
        return false;
    }

    // Allowed Army No prefixes
    const allowedPrefixes = [
        "IC", "SS", "TA", "SL", "WC", "DR",
        "LC", "MR", "MS", "NR", "NS",
        "NTR", "NTS", "PC", "RC", "SC",
        "SCO", "ST", "DS", "GC", "TC",
        "V", "VS", "WS"
    ];

    // Extract prefix, numeric part and suffix
    const match = ArmyNo.match(/^([A-Z]{1,3})([0-9]{5,6})([A-Z])$/);

    if (!match) {
        toastr.error("Please enter a valid Army No. Examples: V12345X, IC123456L, NTR12345X.");
        return false;
    }

    const prefix = match[1];
    const numericPart = match[2];
    const actualSuffix = match[3];

    // Validate prefix
    if (!allowedPrefixes.includes(prefix)) {
        toastr.error("Invalid Army No Prefix.");
        return false;
    }

    // Three-letter prefix must contain exactly 5 digits
    if (prefix.length === 3 && numericPart.length !== 5) {
        toastr.error("Three-letter Army No Prefix requires exactly 5 digits.");
        return false;
    }

    // Calculate expected suffix using Modulo 11
    let length = numericPart.length;
    let multiplier = length + 1;
    let sum = 0;

    for (let i = 0; i < length; i++) {

        let digit = Number(numericPart[i]);

        sum += digit * multiplier;
        multiplier--;
    }

    let remainder = sum % 11;

    // Suffix mapping
    const suffixMap = {
        0: "A",
        1: "F",
        2: "H",
        3: "K",
        4: "L",
        5: "M",
        6: "N",
        7: "P",
        8: "W",
        9: "X",
        10: "Y"
    };

    const expectedSuffix = suffixMap[remainder];

    // Validate suffix
    if (actualSuffix !== expectedSuffix) {
        toastr.error("Invalid Army No Suffix.");
        return false;
    }

    // Army No is valid
    return true;
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