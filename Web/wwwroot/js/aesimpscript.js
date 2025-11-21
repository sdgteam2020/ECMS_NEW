function SubmitsEncry() {
    var txtpassword = $('#Password').val();
    var skey = $('#hdns').val();

    if (txtpassword == "") {
        alert('Please enter Password');
        return false;
    }
    else {
        var encryptedpassword = encryptData(txtpassword, skey);
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

        var encryptedpassword = encryptData(txtconfpassword, skey);

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

        var encryptedpassword = encryptData(txtnewpassword, skey);
        var encryptedconfpassword = encryptData(txtconfpassword, skey);
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
        var encryptedpassword = encryptData(txtnewpassword, skey);
        var encryptedconfpassword = encryptData(txtconfpassword, skey);
        var encryptedoldpassword = encryptData(txtoldpassword, skey);

        $('#Password').val(encryptedpassword);
        $('#ConfirmPassword').val(encryptedconfpassword);
        $('#OldPassword').val(encryptedoldpassword);
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