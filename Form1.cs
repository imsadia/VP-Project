﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VP_Project
{
    public partial class mainForm : Form
    {
        string imagePath;
        string textFilePath;
        string messageToHide;
        double textFileSize, imgFileSize;
        OpenFileDialog opf;
        Bitmap bmpImage;
        public mainForm()
        {
            InitializeComponent();
            opf = new OpenFileDialog();
        }
        public enum State
        {
            hiding,
            fillingWithZeros,
        };
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
                messageToHide = rtbTextFile.Text;
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
                MessageBox.Show("Please choose image or text file to complete encryption", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Encryption has started", "Processing....", MessageBoxButtons.OK, MessageBoxIcon.Information);
                State state = State.hiding;
                int i, j, k = 0;
                int charIndex = 0;
                long pixelElementIndex = 0;
                int zeros = 0;
                int charValue = 0;
                int R, G, B;
                for (i = 0; i < bmpImage.Height; i++)
                {
                    for (j = 0; j < bmpImage.Width; j++)
                    {
                        Color pixels = bmpImage.GetPixel(j, i);
                        R = pixels.R - pixels.R % 2;
                        G = pixels.G - pixels.G % 2;
                        B = pixels.B - pixels.B % 2;
                        for (int n = 0; n < 3; n++)
                        {
                            if (pixelElementIndex % 8 == 0)
                            {
                                if (state == State.fillingWithZeros && zeros == 8)
                                {
                                    if ((pixelElementIndex - 1) % 3 < 2)
                                    {
                                        bmpImage.SetPixel(j, i, Color.FromArgb(R, G, B));
                                    }
                                }
                                if (charIndex >= textFileSize)
                                {
                                    state = State.fillingWithZeros;
                                }
                                else
                                {
                                    charValue = messageToHide[charIndex++];
                                }
                            }
                        }
                        if(pixelElementIndex % 3 == 0)
                        {
                            if(state == State.hiding)
                            {
                                R += charValue % 2;
                                charValue /= 2;
                            }
                        }
                        else if(pixelElementIndex % 3 == 1)
                        {
                            if (state == State.hiding)
                            {
                                G += charValue % 2;
                                charValue /= 2;
                            }
                        }
                        else if (pixelElementIndex % 3 == 2)
                        {
                            if (state == State.hiding)
                            {
                                B += charValue % 2;
                                charValue /= 2;
                            }
                            bmpImage.SetPixel(j, i, Color.FromArgb(R, G, B));
                        }
                        pixelElementIndex++;
                        if (state == State.fillingWithZeros)
                        {
                            zeros++;
                        }
                    }
                }
                bmpImage.Save(@"C:\Users\malik\OneDrive\Documents\GitHub\VP-Project\img.bmp");
                MessageBox.Show("Encryption has been finished.", "Encryption Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
