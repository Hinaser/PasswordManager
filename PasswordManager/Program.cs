using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            var encryptedData = Utility.Encrypt(data, password);
            var decryptedData = Utility.Decrypt(encryptedData, password);

            return;
            //
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
