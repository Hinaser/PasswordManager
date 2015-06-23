using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

//Temporary
using System.IO;
using System.Security.Cryptography;
//Temporary

namespace PasswordManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var password = Utility.GetHash("test");
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            RijndaelManaged rijndael = new RijndaelManaged
            {
                Key = password,
                Mode = CipherMode.CBC,
                BlockSize = 128,
                KeySize = 256,
                Padding = PaddingMode.PKCS7
            };
            //rijndael.Key = password;

            byte[] encryptedData;
            byte[] decryptedData;

            SymmetricAlgorithm algorithm = rijndael;

            ICryptoTransform encryptor = algorithm.CreateEncryptor();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    encryptedData = ms.ToArray();
                }
            }

            ICryptoTransform decryptor = algorithm.CreateDecryptor();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(encryptedData, 0, encryptedData.Length);
                    cs.FlushFinalBlock();
                    decryptedData = ms.ToArray();
                }
            }

            return;
            //
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
