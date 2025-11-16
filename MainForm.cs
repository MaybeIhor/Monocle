using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace Image_View
{
    public partial class form : Form
    {
        private Image originalImage;
        private bool dark;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        public form(string filePath = null)
        {
            InitializeComponent();
            ApplySystemTheme();
            toolStrip.Renderer = new FixedRenderer();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            if (!string.IsNullOrEmpty(filePath))
                LoadImage(filePath);
        }

        private void ApplySystemTheme()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
            {
                if (key?.GetValue("AppsUseLightTheme") is int theme && theme == 0)
                {
                    DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
                    SetDarkTheme();
                }
            }
        }

        private void SetDarkTheme()
        {
            BackColor = Color.FromArgb(255, 25, 25, 25);
            toolStrip.ForeColor = SystemColors.Window;
            themeButton.Text = "⛯";
            dark = true;
        }

        private void SetLightTheme()
        {
            BackColor = SystemColors.Window;
            toolStrip.ForeColor = SystemColors.ControlText;
            themeButton.Text = "⛭";
            dark = false;
        }

        private void LoadImage(string path)
        {
            DisposeImages();

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image tempImage = Image.FromStream(fs))
                {
                    PixelFormat format = HasTransparency(tempImage) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                    originalImage = new Bitmap(tempImage.Width, tempImage.Height, format);
                    using (Graphics g = Graphics.FromImage(originalImage))
                    {
                        g.DrawImage(tempImage, 0, 0, tempImage.Width, tempImage.Height);
                    }
                }

                pictureBox.Image = (Image)originalImage.Clone();
                UpdateTitle(Path.GetFileName(path));
                saveButton.Enabled = true;
                printButton.Enabled = true;
            }
            catch {}
        }

        private bool HasTransparency(Image image)
        {
            if (image.PixelFormat == PixelFormat.Format32bppArgb || image.PixelFormat == PixelFormat.Format32bppPArgb)
            {
                return true;
            }

            if (image.RawFormat.Equals(ImageFormat.Png))
            {
                return true;
            }

            return false;
        }

        private void UpdateTitle(string fileName = null)
        {
            if (originalImage != null)
            {
                string name = fileName ?? "Monocle";
                Text = $"{name}   {originalImage.Width} x {originalImage.Height}";
            }
            else
            {
                Text = "Monocle";
            }
        }

        private void DisposeImages()
        {
            pictureBox.Image?.Dispose();
            originalImage?.Dispose();
            pictureBox.Image = null;
            originalImage = null;
        }

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
                LoadImage(files[0]);
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            if (dark)
                SetDarkTheme();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "All image files (*.png;*.jpg;*.ico;*.jpeg;*.bmp;*.tiff;*.jpe;*.jfif;*.exif;*.gif)|*.png;*.jpg;*.ico;*.jpeg;*.bmp;*.tiff;*.jpe;*.jfif;*.exif;*.gif|PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg;*.jpe;*.jfif;*.exif)|*.jpg;*.jpeg;*.jpe;*.jfif;*.exif|TIFF (*.tiff)|*.tiff|BMP (*.bmp)|*.bmp|ICO (*.ico)|*.ico",
                FilterIndex = 1,
                RestoreDirectory = true
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    LoadImage(dialog.FileName);
            }
        }

        private void MirrorButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Image)originalImage.Clone();
            }
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image?.Dispose();
                pictureBox.Image = (Image)originalImage.Clone();
            }
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            pictureBox.Focus();
        }

        private void ThemeButton_Click(object sender, EventArgs e)
        {
            if (!dark)
                SetDarkTheme();
            else
                SetLightTheme();
            pictureBox.InvalidateCache();
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                pictureBox.Image = (Image)originalImage.Clone();
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image != null && e.Button == MouseButtons.Right)
            {
                toolStrip.Visible = !toolStrip.Visible;
                pictureBox.Invalidate();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            using (SaveFileDialog dialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|BMP (*.bmp)|*.bmp",
                FilterIndex = 1,
                FileName = "Untitled.png",
                RestoreDirectory = true
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string ext = Path.GetExtension(dialog.FileName).ToLower();

                        if (ext == ".jpg" || ext == ".jpeg")
                        {
                            var encoder = GetEncoder(ImageFormat.Jpeg);
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                            pictureBox.Image.Save(dialog.FileName, encoder, encoderParams);
                        }
                        else if (ext == ".bmp")
                        {
                            pictureBox.Image.Save(dialog.FileName, ImageFormat.Bmp);
                        }
                        else
                        {
                            pictureBox.Image.Save(dialog.FileName);
                        }
                    }
                    catch {}
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                RestoreButton_Click(sender, e);
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
            }
        }

        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsImage()) return;

            DisposeImages();

            using (Image clipboardImage = Clipboard.GetImage())
            {
                PixelFormat format = HasTransparency(clipboardImage) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                originalImage = new Bitmap(clipboardImage.Width, clipboardImage.Height, format);
                using (Graphics g = Graphics.FromImage(originalImage))
                {
                    g.DrawImage(clipboardImage, 0, 0, clipboardImage.Width, clipboardImage.Height);
                }
            }

            pictureBox.Image = (Image)originalImage.Clone();
            UpdateTitle();
            saveButton.Enabled = true;
            printButton.Enabled = true;
        }
        
        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (o, args) => args.Graphics.DrawImage(pictureBox.Image, new Point(0, 0));

                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.Document = pd;
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        pd.Print();
                    }
                }
            }
            catch {}
        }
    }
}