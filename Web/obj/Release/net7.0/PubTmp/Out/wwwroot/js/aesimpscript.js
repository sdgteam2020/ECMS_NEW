function SubmitsEncry() {
    var txtpassword = $('#Password').val();
    var skey = $('#hdns').val();

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
    }
}
function CreateHash() {

    var txtnewpassword = $('#Password').val();
    var txtconfpassword = $('#ConfirmPassword').val();
    var skey = $('#hdns').val();
    if (txtconfpassword == "") {
        alert('Please enter Password');
        return false;
    }
    else {
        var key = CryptoJS.enc.Utf8.parse(skey);
        var iv = CryptoJS.enc.Utf8.parse(skey);
        var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtconfpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });

        $('#Password').val(encryptedpassword);
        $('#ConfirmPassword').val(encryptedpassword);

    }
}
function CrNwHsh() {

    var txtnewpassword = $('#Password').val();
    var txtconfpassword = $('#ConfirmPassword').val();
    var skey = $('#hdns').val();
    if (txtconfpassword == "") {
        alert('Please enter Password');
        return false;
    }
    else {
        var key = CryptoJS.enc.Utf8.parse(skey);
        var iv = CryptoJS.enc.Utf8.parse(skey);
        var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtnewpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        var encryptedconfpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtconfpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        $('#Password').val(encryptedpassword);
        $('#ConfirmPassword').val(encryptedconfpassword);

    }
}
function CrOldHsh() {

    var txtnewpassword = $('#Password').val();
    var txtconfpassword = $('#ConfirmPassword').val();
    var txtoldpassword = $('#OldPassword').val();
    var skey = $('#hdns').val();
    if (txtconfpassword == "") {
        alert('Please enter Password');
        return false;
    }
    else {
        var key = CryptoJS.enc.Utf8.parse(skey);
        var iv = CryptoJS.enc.Utf8.parse(skey);
        var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtnewpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        var encryptedconfpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtconfpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        var encryptedoldpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtoldpassword), key,
            { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
        $('#Password').val(encryptedpassword);
        $('#ConfirmPassword').val(encryptedconfpassword);
        $('#OldPassword').val(encryptedoldpassword);
    }
}