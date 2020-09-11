using System;

namespace FileEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            FileEncryptor fe = new FileEncryptor();
            // String clearText = Convert.ToBase64String(new byte[]{97},0,1);
            // Console.WriteLine($"clearText: {clearText}");
            Console.WriteLine("Using file for cleartext.");
            fe.EncryptFile("text.cleartext", "a");
        }
    }
}
