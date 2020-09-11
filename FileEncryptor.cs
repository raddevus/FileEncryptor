using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace FileEncryptor{
    public class FileEncryptor{
        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    //var fakekey = "ca978112ca1bbdcafac231b39a23dc4da786eff8147c4e72b9807785afee48bb";
                    // byte [] keyBytes = new byte[AES.KeySize/8];
                    // for (int i = 0; i < AES.KeySize/8;i++){
                    //     keyBytes[i] = passwordBytes[i];
                    // }
                    
                    Console.WriteLine($"passwordBytes : {passwordBytes.Length}");
                    AES.Key = passwordBytes;
                    var ivBlockSize = AES.BlockSize / 8;
                    
                    byte [] keyBytes = new byte[ivBlockSize];
                    for (int i = 0; i < ivBlockSize;i++){
                        keyBytes[i] = passwordBytes[i];
                    }
                    AES.IV = keyBytes;
                    //AES.Key = key.GetBytes(AES.KeySize / 8);
                    
                    Console.WriteLine($"ivBlockSize : {ivBlockSize}");
                    //AES.IV = key.GetBytes(ivBlockSize);

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

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

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

            //byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] bytesToBeEncrypted = {97}; // used to test specific bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            var encryptedBytesHexEncoded = BytesToHex(bytesEncrypted);
            Console.WriteLine(encryptedBytesHexEncoded);
            String b64Encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptedBytesHexEncoded),0,encryptedBytesHexEncoded.Length);
            //String b64Encrypted = Convert.ToBase64String(bytesEncrypted,0,bytesEncrypted.Length);

            string fileEncrypted = clearTextFile +  ".enc";
            Console.WriteLine(b64Encrypted);
            Console.WriteLine($"wrote {fileEncrypted}");
            File.WriteAllText(fileEncrypted,b64Encrypted);
            //File.WriteAllBytes(fileEncrypted, bytesEncrypted);
            
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