namespace Web.Healpers.BaseInterfaces
{
    public interface IImageEncryptAndDecrypt
    {
        public Task EncryptImageFile(string inputFilePath, string encryptedFilePath);
        public Task DecryptImageFile(string encryptedFilePath, string decryptedFilePath);
        public Task<IFormFile> DecryptImageToIFormFile(string encryptedFilePath, string fileName);
        public Task<string> DecryptImageToBase64(string encryptedFilePath);
        public string CompressBase64(string base64,int maxWidth,long jpegQuality,bool returnDataUri);
    }
}
