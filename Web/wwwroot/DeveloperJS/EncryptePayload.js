function encryptPayloadData(plainText) {
    const secretKey = $("#hiddenSa").val();
    if (!secretKey) return "";

    // 🔥 convert int → string
    const text = plainText.toString();

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.padEnd(16, '0').substring(0, 16));

    const encrypted = CryptoJS.AES.encrypt(text, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return encrypted.toString();
}