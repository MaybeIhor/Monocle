using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Image_View
{
    public partial class DontBlurBox : PictureBox
    {
        private bool isDown = false;
        private Point p1;
        private Point p2;
        private Rectangle imageRect;
        private double scale;
        private Bitmap cachedRect;

        public DontBlurBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
            MouseDown += PictureBox_MouseDown;
            MouseUp += PictureBox_MouseUp;
            MouseMove += PictureBox_MouseMove;
        }

        public new Image Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                InvalidateCache();
            }
        }

        public void InvalidateCache()
        {
            cachedRect?.Dispose();
            cachedRect = null;
        }

        private void CalculateImageBounds()
        {
            if (Image == null) return;

            scale = Math.Min((double)Width / Image.Width, (double)Height / Image.Height);
            int w = (int)(Image.Width * scale);
            int h = (int)(Image.Height * scale);
            imageRect = new Rectangle((Width - w) / 2, (Height - h) / 2, w, h);
        }

        private Point ClampToImageBounds(Point pt)
        {
            return new Point(
                Math.Max(imageRect.Left, Math.Min(pt.X, imageRect.Right)),
                Math.Max(imageRect.Top, Math.Min(pt.Y, imageRect.Bottom))
            );
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null) return;

            CalculateImageBounds();

            if (cachedRect == null || cachedRect.Size != Size)
            {
                InvalidateCache();

                PixelFormat cacheFormat = PixelFormat.Format24bppRgb;
                cachedRect = new Bitmap(Width, Height, cacheFormat);

                using (Graphics g = Graphics.FromImage(cachedRect))
                {
                    g.Clear(BackColor);

                    if (Image.Width < 512 && Image.Height < 512)
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                    }
                    else
                    {
                        g.InterpolationMode = InterpolationMode.Bicubic;
                        g.CompositingQuality = CompositingQuality.Default;
                        g.SmoothingMode = SmoothingMode.HighSpeed;
                    }

                    g.DrawImage(Image, imageRect);
                }
            }

            e.Graphics.DrawImageUnscaled(cachedRect, 0, 0);

            if (isDown && !p2.IsEmpty)
            {
                var selRect = new Rectangle(
                    Math.Min(p1.X, p2.X),
                    Math.Min(p1.Y, p2.Y),
                    Math.Abs(p1.X - p2.X),
                    Math.Abs(p1.Y - p2.Y)
                );

                using (var path = new GraphicsPath())
                using (var brush = new SolidBrush(Color.FromArgb(155, 0, 0, 0)))
                {
                    path.AddRectangle(imageRect);
                    path.AddRectangle(selRect);
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (Image == null || e.Button != MouseButtons.Left || Image.Width <= 6 || Image.Height <= 6)
            {
                isDown = false;
                return;
            }

            p1 = ClampToImageBounds(e.Location);
            p2 = Point.Empty;
            isDown = true;
            Cursor.Current = Cursors.Hand;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDown || Image == null) return;

            p2 = ClampToImageBounds(e.Location);
            Invalidate();
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (Image == null || e.Button != MouseButtons.Left) return;

            Cursor.Current = Cursors.Default;
            isDown = false;

            if (!p2.IsEmpty)
            {
                int w = Math.Abs(p1.X - p2.X);
                int h = Math.Abs(p1.Y - p2.Y);

                if (w > 20 && h > 20)
                    Crop();
            }

            Invalidate();
        }

        private void Crop()
        {
            double x1 = (p1.X - imageRect.X) / scale;
            double y1 = (p1.Y - imageRect.Y) / scale;
            double x2 = (p2.X - imageRect.X) / scale;
            double y2 = (p2.Y - imageRect.Y) / scale;

            int w = (int)Math.Abs(x1 - x2);
            int h = (int)Math.Abs(y1 - y2);

            if (h != Image.Height) h++;
            if (w != Image.Width) w++;

            if (w < 6 || h < 6) return;

            var cropRect = new Rectangle((int)Math.Min(x1, x2), (int)Math.Min(y1, y2), w, h);

            try
            {
                Bitmap croppedImage = new Bitmap(w, h, Image.PixelFormat);

                using (Graphics g = Graphics.FromImage(croppedImage))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    
                    g.DrawImage(Image,
                        new Rectangle(0, 0, w, h),
                        cropRect,
                        GraphicsUnit.Pixel);
                }

                Image?.Dispose();
                Image = croppedImage;

            }
            catch {}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InvalidateCache();
            }
            base.Dispose(disposing);
        }
    }
}