using System;

namespace FileEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2){
                Console.WriteLine("Need filename of file to encrypt and password");
                Console.WriteLine("Usage: [filename] [password]");
                return;
            }
            FileEncryptor fe = new FileEncryptor();
            // String clearText = Convert.ToBase64String(new byte[]{97},0,1);
            // Console.WriteLine($"clearText: {clearText}");
            Console.WriteLine("Using file for cleartext.");

            fe.EncryptFile(args[0], args[1]);
        }
    }
}
