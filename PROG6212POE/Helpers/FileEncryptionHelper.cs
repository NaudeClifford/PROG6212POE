using System.Security.Cryptography;
using System.Text;

public static class FileEncryptionHelper
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"); // 32 characters = 32 bytes
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ABCDEF0123456789"); // 16 bytes

    public static async Task EncryptFileAsync(Stream inputStream, string outputFilePath)
    {

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var fileStream = File.Create(outputFilePath);

        using var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

        await inputStream.CopyToAsync(cryptoStream);

        //flush the block
        await cryptoStream.FlushAsync();

        cryptoStream.FlushFinalBlock();
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
