﻿using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VP_Project
{
    public partial class mainForm : Form
    {
        string imagePath;
        string textFilePath;
        string textFile;
        string messageToHide, messageToHideInBinary, decryptedMessageInBinary = "";
        double textFileSize, imgFileSize;
        int size;
        OpenFileDialog opf;
        Bitmap bmpImage;
        public mainForm()
        {
            InitializeComponent();
            opf = new OpenFileDialog();
            readTextFile();
        }

        public void readTextFile()
        {
            // Default folder  
            textFile = @"D:\BUIC\ImageSteganoraphy\ImagesAndText\textForEncryption.txt";
            if (File.Exists(textFile))
            {
                messageToHide = File.ReadAllText(textFile);
                byte[] bytes = Encoding.ASCII.GetBytes(messageToHide);
                foreach (byte by in bytes)
                {
                    messageToHideInBinary += Convert.ToString(by, 2);
                }
  
                size = messageToHideInBinary.Length;
            }
        }
        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Decryption has started. Please Wait!", "Processing....", MessageBoxButtons.OK, MessageBoxIcon.Information);
            int i, j, R, G, B, count = 0;
            string r, g, b;

            for (i = 0; i < bmpImage.Width; i++)
            {
                for (j = 0; j < bmpImage.Height; j++)
                {
                   
                    Color pixels = bmpImage.GetPixel(i, j);
                    R = pixels.R;
                    G = pixels.G;
                    B = pixels.B; 
                    r = Convert.ToString(R, 2).PadLeft(8, '0');
                    g = Convert.ToString(G, 2).PadLeft(8, '0');
                    b = Convert.ToString(B, 2).PadLeft(8, '0'); //converting R, G, B values to binary
   
                    if(count < size)
                    {
                        if(count < size)
                        {
                            decryptedMessageInBinary += r[r.Length - 1]; //extracting and saving least significant bit
                            count++;
                        }
                        if(count < size)
                        {
                            decryptedMessageInBinary += g[g.Length - 1];
                            count++;
                        }
                        if(count < size)
                        {
                            decryptedMessageInBinary += b[b.Length - 1];
                            count++;
                        }
                    }
                }
            }

            if(decryptedMessageInBinary == messageToHideInBinary)
            {
                rtbTextFile.Text = messageToHide;
                File.WriteAllText(@"D:\BUIC\ImageSteganoraphy\EncryptedImageAndDecryptedMessage\decryptedMessage.txt", messageToHide);
                MessageBox.Show("Decryption has been finished successfully and Decrypted Message is saved successfully in Text File.", "Decryption Successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Decryption has failed! Please try again.", "Decryption Fail!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            tbImgBrowse.Text = "";
            tbFileBrowse.Text = "";
            rtbTextFile.Text = "";
            pbDisplayImage.Image = null;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void BtnImgBrowse_Click(object sender, EventArgs e)
        {
            if (opf.ShowDialog() == DialogResult.OK)
            {
                imagePath = opf.FileName;
                pbDisplayImage.Image = Image.FromFile(imagePath);
                pbDisplayImage.SizeMode = PictureBoxSizeMode.StretchImage;
                tbImgBrowse.Text = imagePath;
                bmpImage = new Bitmap(pbDisplayImage.Image);
                double bytes = new FileInfo(imagePath).Length;
                imgFileSize = bytes;
                double kb = Math.Round(bytes / 1024f, 0);
                lbHeight.Text = bmpImage.Height.ToString();
                lbWidth.Text = bmpImage.Width.ToString();
                lbSize.Text = kb + " KB";
                int i, j, numberOfPixels = 0;
                for (i = 0; i < bmpImage.Width; i++)
                {
                    for (j = 0; j < bmpImage.Height; j++)
                    {
                        Color pixels = bmpImage.GetPixel(i, j);
                        numberOfPixels++;
                    }
                }
                lbPixels.Text = numberOfPixels.ToString();
            }
        }

        private void BtnFileBrowse_Click(object sender, EventArgs e)
        {
            if(opf.ShowDialog() ==  DialogResult.OK)
            {
                textFilePath = opf.FileName;
                tbFileBrowse.Text = textFilePath;
                rtbTextFile.Text = File.ReadAllText(textFilePath);
                FileInfo fi = new FileInfo(opf.FileName);
                textFileSize = fi.Length;
            }
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if(textFileSize > imgFileSize)
            {
                MessageBox.Show("Sorry choose larger image to hide this text file.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(pbDisplayImage.Image == null || rtbTextFile.Text == String.Empty)
            {
                MessageBox.Show("Please choose image or text file to complete encryption!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Encryption has started. Please Wait!", "Processing....", MessageBoxButtons.OK, MessageBoxIcon.Information);
   
                int i, j, R, G, B, charIndex = 0;
                string r, g, b;
                int dR, dG, dB; //variables declaration
                for (i = 0; i < bmpImage.Width; i++) //looping through each pixel
                {
                    for (j = 0; j < bmpImage.Height; j++)
                    {
                        Color pixels = bmpImage.GetPixel(i, j);
                        R = pixels.R; 
                        G = pixels.G;
                        B = pixels.B; //getting R, G, B values in decimal
 
                        r = Convert.ToString(R, 2).PadLeft(8, '0');
                        g = Convert.ToString(G, 2).PadLeft(8, '0');
                        b = Convert.ToString(B, 2).PadLeft(8, '0'); //converting R, G, B values to binary
  
                        if (charIndex < size) //charIndex should not be greater than size
                        {
                            //removing LSB and replacing with binary of text message
                            if(charIndex < size)
                            {
                                r = r.Remove(r.Length - 1, 1) + messageToHideInBinary[charIndex];
                                charIndex++;
                            }
                            if(charIndex < size)
                            {
                                g = g.Remove(g.Length - 1, 1) + messageToHideInBinary[charIndex];
                                charIndex++;
                            }
                            if(charIndex < size)
                            {
                                b = b.Remove(b.Length - 1, 1) + messageToHideInBinary[charIndex];
                                charIndex++;
                            }
                        }
                        dR = Convert.ToInt32(r, 2); 
                        dG = Convert.ToInt32(g, 2); //converting modified binary to decimal
                        dB = Convert.ToInt32(b, 2);
                        bmpImage.SetPixel(i, j, Color.FromArgb(dR, dG, dB)); //setting in image pixels
                    }
                }
                bmpImage.Save(@"C:\Users\malik\OneDrive\Documents\GitHub\VP-Project\EncryptedImageAndDecryptedMessage\encryptedImage.bmp"); //saving data encrypted image
                MessageBox.Show("Encryption has been finished successfully and Encrypted Image is saved successfully.", "Encryption Successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
