using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace FileEncryptor{
    public class FileEncryptor{
        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var ivBlockSize = AES.BlockSize / 8;

                    Console.WriteLine($"passwordBytes : {passwordBytes.Length}");
                    AES.Key = passwordBytes;
                    
                    
                    byte [] keyBytes = new byte[ivBlockSize];
                    for (int i = 0; i < ivBlockSize;i++){
                        keyBytes[i] = passwordBytes[i];
                    }
                    AES.IV = keyBytes;
                    
                    Console.WriteLine($"ivBlockSize : {ivBlockSize}");

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var ivBlockSize = AES.BlockSize / 8;

                    AES.Key = passwordBytes;
                     byte [] keyBytes = new byte[ivBlockSize];
                    for (int i = 0; i < ivBlockSize;i++){
                        keyBytes[i] = passwordBytes[i];
                    }
                    AES.IV = keyBytes;

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }


        public void EncryptFile(string clearTextFile, string password)
        {
            string file = clearTextFile;

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            //byte[] bytesToBeEncrypted = {97,10}; // used to test specific bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            String b64Encrypted = Convert.ToBase64String(bytesEncrypted,0,bytesEncrypted.Length);

            string fileEncrypted = clearTextFile +  ".enc";
            Console.WriteLine(b64Encrypted);
            Console.WriteLine($"wrote {fileEncrypted}");
            File.WriteAllText(fileEncrypted,b64Encrypted);
            //File.WriteAllBytes(fileEncrypted, bytesEncrypted);
            
        }

        public void DecryptFile(string encryptedFile, string password, string decryptedFileExtension)
        {
            string base64EncryptedString = File.ReadAllText(encryptedFile);
            Console.WriteLine(base64EncryptedString);
            byte[] bytesToBeDecrypted = Convert.FromBase64String(base64EncryptedString);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            
            string decryptedFile = 
                $"{Path.GetDirectoryName(Path.GetFullPath(encryptedFile))}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(encryptedFile)}.dec";
            if (decryptedFileExtension != String.Empty){
                decryptedFile += $".{decryptedFileExtension}";
            }
            Console.WriteLine($"wrote {decryptedFile}");
            File.WriteAllBytes(decryptedFile,decryptedBytes);
        }


    private string BytesToHex(byte[] bytes) 
    { 
    //return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))); 
    return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))); 
    }

        public void DecryptFile(string cipherTextFile, string password)
        {
            string fileEncrypted = cipherTextFile;
            
            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            
            string file = Path.GetFileNameWithoutExtension(fileEncrypted);
            file = Path.Combine(Path.GetDirectoryName(fileEncrypted), file);
            Console.WriteLine(file);
            File.WriteAllBytes(file, bytesDecrypted);
        }

    }
}