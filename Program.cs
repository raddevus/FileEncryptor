using System;

namespace FileEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3){
                Console.WriteLine("Need filename of file to encrypt and password");
                Console.WriteLine("Usage (encrypt): e [filename] [password]");
                Console.WriteLine("Usage (decrypt): d [filename] [password] <file extension>");
                return;
            }
            FileEncryptor fe = new FileEncryptor();
            Console.WriteLine("Using file for cleartext.");
            Console.WriteLine($"args.Length: {args.Length}");
            switch (args[0]){
                case "e":{
                    fe.EncryptFile(args[1], args[2]);
                    break;
                }
                case "d":{
                    String fileExtension = String.Empty;
                    if (args.Length > 3){
                        // trim leading and trailing spaces.
                        fileExtension = args[3].Trim();
                        //trim out . if user supplies it
                        fileExtension = fileExtension.TrimStart(new char[]{'.'});
                        Console.WriteLine(fileExtension);
                    }
                    fe.DecryptFile(args[1], args[2],fileExtension);
                    break;
                }
            }
            
        }
    }
}
