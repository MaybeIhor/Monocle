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
        private string currentFileName;
        private bool dark;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        public form(string filePath = null)
        {
            InitializeComponent();
            ApplySystemTheme();
            toolStrip.Renderer = new FixedRenderer();
            editBox.Renderer = new FixedRenderer();
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
            editBox.BackColor = Color.FromArgb(255, 25, 25, 25);
            toolStrip.ForeColor = SystemColors.Window;
            editBox.ForeColor = SystemColors.Window;
            pictureBox.InvalidateCache();
            themeButton.Text = "⛯";
            dark = true;
        }

        private void SetLightTheme()
        {
            BackColor = SystemColors.Window;
            editBox.BackColor = SystemColors.Window;
            toolStrip.ForeColor = SystemColors.ControlText;
            editBox.ForeColor = SystemColors.ControlText;
            pictureBox.InvalidateCache();
            themeButton.Text = "⛭";
            dark = false;
        }

        private void LoadImage(string path)
        {
            try
            {
                pictureBox.Image?.Dispose();
                pictureBox.Image = null;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
                using (Image tempImage = Image.FromStream(fs, false, false))
                {
                    PixelFormat format = HasTransparency(tempImage) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                    Image temp = new Bitmap(tempImage.Width, tempImage.Height, format);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        g.DrawImage(tempImage, 0, 0, tempImage.Width, tempImage.Height);
                    }
                    pictureBox.Image = temp;
                    pictureBox.ResetCrop();
                    currentFileName = path;
                    UpdateTitle();
                }
            }
            catch { }
        }
        private bool HasTransparency(Image img) => img.PixelFormat == PixelFormat.Format32bppArgb || img.PixelFormat == PixelFormat.Format32bppPArgb || img.RawFormat.Equals(ImageFormat.Png);

        private void UpdateTitle()
        {
            Rectangle? crop = pictureBox.GetCrop();

            int width = crop.HasValue ? crop.Value.Width : pictureBox.Image.Width;
            int height = crop.HasValue ? crop.Value.Height : pictureBox.Image.Height;

            Text = pictureBox.Image != null ? $"{width} x {height}   {Path.GetFileName(currentFileName)}" : "Monocle";
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

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            pictureBox.Focus();
        }

        private void ThemeButton_Click(object sender, EventArgs e)
        {
            if (dark) SetLightTheme();
            else SetDarkTheme();
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            pictureBox.ResetCrop();
            UpdateTitle();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            editBox.Visible = false;

            if (pictureBox.Image != null && e.Button == MouseButtons.Right)
            {
                toolStrip.Visible = !toolStrip.Visible;
                pictureBox.Invalidate();
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

        private int PickExtension(string ext) 
        {
            if (ext == ".jpg" || ext == ".jpeg")
                return 2;
            if (ext == ".bmp")
                return 3;
            return 1;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            using (SaveFileDialog dialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|BMP (*.bmp)|*.bmp",
                FilterIndex = PickExtension(Path.GetExtension(currentFileName).ToLower()),
                FileName = Path.GetFileName(currentFileName),
                RestoreDirectory = false
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Image imageToSave = pictureBox.GetVisible())
                        {
                            string ext = Path.GetExtension(dialog.FileName).ToLower();

                            if (ext == ".jpg" || ext == ".jpeg")
                            {
                                using (var encoderParams = new EncoderParameters(1))
                                {
                                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                                    imageToSave.Save(dialog.FileName, GetEncoder(ImageFormat.Jpeg), encoderParams);
                                }
                            }
                            else if (ext == ".bmp")
                            {
                                imageToSave.Save(dialog.FileName, ImageFormat.Bmp);
                            }
                            else
                            {
                                imageToSave.Save(dialog.FileName);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
                RestoreButton_Click(sender, e);
            else if (e.Control && e.KeyCode == Keys.V)
                PasteFromClipboard();
            else if (e.Control && e.KeyCode == Keys.G)
                GridButton_Click(sender, e);
        }

        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsImage()) return;

            using (Image clipboardImage = Clipboard.GetImage())
            {
                PixelFormat format = HasTransparency(clipboardImage) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                Image temp = new Bitmap(clipboardImage.Width, clipboardImage.Height, format);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    g.DrawImage(clipboardImage, 0, 0, clipboardImage.Width, clipboardImage.Height);
                }
                pictureBox.Image = temp;
                currentFileName = "Untitled.png";
                pictureBox.ResetCrop();
                UpdateTitle();
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            try
            {
                Image imageToPrint = pictureBox.GetVisible();

                PrintDocument pd = new PrintDocument();

                pd.PrintPage += (o, args) =>
                {
                    Rectangle printArea = args.MarginBounds;

                    float scaleX = (float)printArea.Width / imageToPrint.Width;
                    float scaleY = (float)printArea.Height / imageToPrint.Height;
                    float scale = Math.Min(scaleX, scaleY);

                    int scaledWidth = (int)(imageToPrint.Width * scale);
                    int scaledHeight = (int)(imageToPrint.Height * scale);

                    int x = printArea.Left + (printArea.Width - scaledWidth) / 2;
                    int y = printArea.Top;

                    args.Graphics.DrawImage(imageToPrint, x, y, scaledWidth, scaledHeight);
                };

                pd.EndPrint += (o, args) =>
                {
                    imageToPrint?.Dispose();
                };

                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.Document = pd;
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        pd.Print();
                    }
                    else
                    {
                        imageToPrint?.Dispose();
                    }
                }
            }
            catch { }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            editBox.Visible = !editBox.Visible;
        }

        private void Rotate90Button_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox.InvalidateBoth();
            pictureBox.Crop90();
            UpdateTitle();
        }

        private void Rotate270Button_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            pictureBox.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            pictureBox.InvalidateBoth();
            pictureBox.Crop270();
            UpdateTitle();
        }

        private void MirrorButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox.InvalidateBoth();
            pictureBox.CropMirror();
            UpdateTitle();
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null) return;

            UpdateTitle();
        }

        private void GridButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;
            pictureBox.isGridding = !pictureBox.isGridding;
            pictureBox.Invalidate();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
                RestoreButton_Click(sender, e);
            else if (e.Control && e.KeyCode == Keys.V)
                PasteFromClipboard();
            else if (e.Control && e.KeyCode == Keys.G)
                GridButton_Click(sender, e);
        }

        private void RedirectButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(currentFileName));

                using (Image imageToEdit = pictureBox.GetVisible())
                {
                    imageToEdit.Save(tempPath, ImageFormat.Png);
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "rundll32.exe",
                    Arguments = $"shell32.dll,OpenAs_RunDLL {tempPath}",
                    UseShellExecute = false
                };
                System.Diagnostics.Process.Start(startInfo);
            }
            catch { }
        }
    }
}