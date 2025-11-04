namespace Web.Healpers.BaseInterfaces
{
    public interface IImageEncryptAndDecrypt
    {
        Task EncryptImageFile(string inputFilePath, string encryptedFilePath);
        Task DecryptImageFile(string encryptedFilePath, string decryptedFilePath);
        Task<IFormFile> DecryptImageToIFormFile(string encryptedFilePath, string fileName);
        Task<string> DecryptImageToBase64(string encryptedFilePath);
        public string CompressBase64(string base64,int maxWidth,long jpegQuality,bool returnDataUri);
    }
}
