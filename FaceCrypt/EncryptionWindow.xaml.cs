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
using System.Security.Cryptography;
using System.IO;
namespace FaceCrypt
{
    /// <summary>
    /// Interaction logic for EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {

        internal static byte[] encKey;
        AesCryptoServiceProvider msp = new AesCryptoServiceProvider();
        ICryptoTransform enc;
        ICryptoTransform dec;
        public EncryptionWindow()
        {
            InitializeComponent();
            msp.Key = encKey;
            msp.IV = new byte[16];
            msp.Padding = PaddingMode.None;
            enc = msp.CreateEncryptor();
            dec = msp.CreateDecryptor();
        }

        private void encryptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] me = Convert.FromBase64String(textInput.Text);
                dec.TransformBlock(me, 0, me.Length, me, 0);
                textInput.Text = new BinaryReader(new MemoryStream(me)).ReadString();
            }catch(Exception er)
            {
                MemoryStream mstream = new MemoryStream();
                BinaryWriter mwriter = new BinaryWriter(mstream);
                mwriter.Write(textInput.Text);
                byte[] aligned = new byte[mstream.Length+(16-(mstream.Length % 16))];
                byte[] src = mstream.ToArray();
                Buffer.BlockCopy(src, 0, aligned, 0, src.Length);
                enc.TransformBlock(aligned, 0, aligned.Length, aligned, 0);
                textInput.Text = Convert.ToBase64String(aligned);
            }
        }
    }
}
