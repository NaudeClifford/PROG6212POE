using System.Security.Cryptography;
using System.Text;

public static class FileEncryptionHelper
{
    // NOTE: Use a secure key and IV in production, don't hardcode!
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("A-32-Byte-Long-Key-For-Encryption!!"); // 32 bytes for AES-256
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("A-16-Byte-IV1234"); // 16 bytes for AES

    public static async Task EncryptFileAsync(Stream inputStream, string outputFilePath)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var cryptoStream = new CryptoStream(
            File.Create(outputFilePath),
            aes.CreateEncryptor(),
            CryptoStreamMode.Write);

        await inputStream.CopyToAsync(cryptoStream);
    }

    public static async Task DecryptFileAsync(string inputFilePath, Stream outputStream)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var cryptoStream = new CryptoStream(
            File.OpenRead(inputFilePath),
            aes.CreateDecryptor(),
            CryptoStreamMode.Read);

        await cryptoStream.CopyToAsync(outputStream);
    }
}
