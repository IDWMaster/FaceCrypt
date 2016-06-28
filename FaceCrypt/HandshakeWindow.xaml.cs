using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Threading;
using System.IO;
namespace FaceCrypt
{
    /// <summary>
    /// Interaction logic for HandshakeWindow.xaml
    /// </summary>
    public partial class HandshakeWindow : Window
    {
        RSACryptoServiceProvider msp;
        public HandshakeWindow()
        {
            InitializeComponent();
            instructions.Text = "Welcome to FaceCrypt, a program to allow for secure communications with your friends over Facebook Chat (and other IM applications).\nThis program is open-source, so you can (AND SHOULD) validate that we didn't put any backdoors in it (we reside in the US, so we may be required by law to install backdoors)\nThe only way to protect against backdoors is to analyze the code yourself.\nThis software makes use of public key cryptography to securely exchange encryption keys between two people over an insecure connection (such as Facebook), which enables future communications over the same medium.\nThis software relies on a secondary out-of-band message channel (other than Facebook), to verify your friend's identity.\nTo start, we have generated public/private key combination.\n You should NEVER give your private key to anyone, but give your public key to your friend, so they can verify your identity.\nWe will start by exchanging public keys. Your public key has been copied to your clipboard. Paste this key into the Facebook chat window, and send it.\n\n\n Copy and paste your friend's public key into this window.";
            msp = new RSACryptoServiceProvider();
            Clipboard.SetText(Convert.ToBase64String(msp.ExportCspBlob(false)));
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            ApplicationCommands.Paste.CanExecuteChanged += Paste_CanExecuteChanged;
            
        }
        private void Paste_CanExecuteChanged(object sender, EventArgs e)
        {

            if(Clipboard.ContainsText())
            {
                try
                {
                    using(RSACryptoServiceProvider msp = new RSACryptoServiceProvider())
                    {
                        byte[] pubkey = Convert.FromBase64String(Clipboard.GetText());
                        msp.ImportCspBlob(pubkey);
                        using (SHA512CryptoServiceProvider hasher = new SHA512CryptoServiceProvider())
                        {
                            byte[] hash = hasher.ComputeHash(pubkey);
                            Guid remote = HashToString(hash);
                            Guid local = HashToString(hasher.ComputeHash(this.msp.ExportCspBlob(false)));
                            if (remote != local)
                            {
                                ApplicationCommands.Paste.CanExecuteChanged -= Paste_CanExecuteChanged;
                                instructions.Text = "Your friend's identity is: " + remote + "\nYour identity is: " + local+"\n\nPlease verify with your friend that their identity matches the information displayed on the screen.\nDo this over something like a phone call, where you can recognize your friend's voice.\n\nIf a single character does NOT match, CLOSE THIS WINDOW AND DO NOT PROCEED!";
                                if(remote.CompareTo(local)>0)
                                {
                                    instructions.Text += "\n\nNext, an AES session key, encrypted with your friend's public key\nhas been copied to your clipboard.\nPlease paste this key in your Facebook messenger\nand tell them to copy it into this program.";
                                    using(AesCryptoServiceProvider mep = new AesCryptoServiceProvider())
                                    {
                                        mep.KeySize = 256;
                                        mep.GenerateKey();
                                        Clipboard.SetText(Convert.ToBase64String(msp.Encrypt(mep.Key, true)));
                                        File.WriteAllBytes("key",mep.Key);
                                        DialogResult = true;
                                    }
                                }else
                                {
                                    instructions.Text += "\n\nYour friend will be sending you a key soon.\nPlease copy and paste the key into this application when it is received.";
                                    ApplicationCommands.Paste.CanExecuteChanged += Paste_CanExecuteChanged1;
                                }
                            }
                        }
                    }
                }catch(Exception er)
                {

                }
            }
        }

        private void Paste_CanExecuteChanged1(object sender, EventArgs e)
        {
            //Clipboard should contain an AES session key
            try
            {
                byte[] key = msp.Decrypt(Convert.FromBase64String(Clipboard.GetText()), true);
                File.WriteAllBytes("key", key);
                DialogResult = true;
                Close();
                MessageBox.Show("Key exchange was successful. You can now exchange messages securely with your friend. Please have your friend close the key exchange window.");
            }
            catch (Exception er)
            {
            }
        }

        private static Guid HashToString(byte[] hash)
        {
            byte[] truncated = new byte[16];
            Buffer.BlockCopy(hash, 0, truncated, 0, 16);
            return new Guid(truncated);
        }
    }
}
