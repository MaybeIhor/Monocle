using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Image_View
{
    public partial class DontBlurBox : PictureBox
    {
        private bool isDown;
        private Point p1, p2;
        private Rectangle imageRect;
        private double scale;
        private Rectangle? crop;
        private Bitmap cachedImage;
        private readonly Timer resizeTimer;
        private bool isResizing;
        public bool isGridding;

        public DontBlurBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            resizeTimer = new Timer { Interval = 300 };
            resizeTimer.Tick += (s, e) => { resizeTimer.Stop(); isResizing = false; InvalidateBoth(); };

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        public new Image Image
        {
            get => base.Image;
            set
            {
                if (base.Image != value)
                {
                    base.Image = value;
                    InvalidateCache();
                }
            }
        }

        public void InvalidateCache()
        {
            cachedImage?.Dispose();
            cachedImage = null;
        }

        public void InvalidateBoth()
        {
            InvalidateCache();
            Invalidate();
        }

        public void ResetCrop()
        {
            crop = null;
            InvalidateBoth();
        }

        public Rectangle? GetCrop() => crop;

        public void Crop90()
        {
            if (!crop.HasValue || Image == null) return;

            Rectangle c = crop.Value;
            crop = new Rectangle(Image.Width - c.Y - c.Height, c.X, c.Height, c.Width);
        }

        public void Crop270()
        {
            if (!crop.HasValue || Image == null) return;

            Rectangle c = crop.Value;
            crop = new Rectangle(c.Y, Image.Height - c.X - c.Width, c.Height, c.Width);
        }

        public void CropMirror()
        {
            if (!crop.HasValue || Image == null) return;

            Rectangle c = crop.Value;
            crop = new Rectangle(Image.Width - c.X - c.Width, c.Y, c.Width, c.Height);
        }

        public Image GetVisible()
        {
            Rectangle sourceRect = crop ?? new Rectangle(0, 0, Image.Width, Image.Height);
            Bitmap croppedImage = new Bitmap(sourceRect.Width, sourceRect.Height, Image.PixelFormat);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(Image,
                    new Rectangle(0, 0, sourceRect.Width, sourceRect.Height),
                    sourceRect,
                    GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

      

        private void CalculateImageBounds()
        {
            if (Image == null) return;

            var imgAspect = crop.HasValue ? (double)crop.Value.Width / crop.Value.Height : (double)Image.Width / Image.Height;
            var ctrlAspect = (double)Width / Height;

            int w, h;
            if (imgAspect > ctrlAspect)
            {
                w = Width;
                h = (int)(Width / imgAspect);
            }
            else
            {
                h = Height;
                w = (int)(Height * imgAspect);
            }
            imageRect = new Rectangle((Width - w) / 2, (Height - h) / 2, w, h);
            scale = crop.HasValue ? Math.Min((double)w / crop.Value.Width, (double)h / crop.Value.Height) : Math.Min((double)w / Image.Width, (double)h / Image.Height);
        }

        private Point ClampToImage(Point pt) => new Point(
            Math.Max(imageRect.Left, Math.Min(imageRect.Right, pt.X)),
            Math.Max(imageRect.Top, Math.Min(imageRect.Bottom, pt.Y))
        );

        protected override void OnResize(EventArgs e)
        {
            isResizing = true;
            resizeTimer.Stop();
            resizeTimer.Start();
            Invalidate();
        }

        private static readonly SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(155, 0, 0, 0));
        private static readonly Pen crossPen = new Pen(Color.FromArgb(155, 155, 155, 155), 1);

        private void DrawGrid(Graphics g)
        {
            if (!isGridding || Image == null) return;

            int left = imageRect.Left;
            int right = imageRect.Right;
            int top = imageRect.Top;
            int bottom = imageRect.Bottom;
            int width = imageRect.Width;
            int height = imageRect.Height;

            int centerX = left + width / 2;
            int centerY = top + height / 2;
            g.DrawLine(crossPen, centerX, top, centerX, bottom);
            g.DrawLine(crossPen, left, centerY, right, centerY);

            int quarterX1 = left + width / 4;
            int quarterX2 = left + 3 * width / 4;
            int quarterY1 = top + height / 4;
            int quarterY2 = top + 3 * height / 4;

            g.DrawLine(crossPen, quarterX1, top, quarterX1, bottom);
            g.DrawLine(crossPen, quarterX2, top, quarterX2, bottom);
            g.DrawLine(crossPen, left, quarterY1, right, quarterY1);
            g.DrawLine(crossPen, left, quarterY2, right, quarterY2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null) return;

            CalculateImageBounds();

            if (cachedImage == null || cachedImage.Size != Size)
            {
                InvalidateCache();
                cachedImage = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

                using (var g = Graphics.FromImage(cachedImage))
                {
                    g.Clear(BackColor);
                    g.PixelOffsetMode = PixelOffsetMode.Half;

                    bool useFastMode = isResizing || (Image.Width < 512 && Image.Height < 512) || (crop.HasValue && crop.Value.Width < 512 && crop.Value.Height < 512);

                    g.InterpolationMode = useFastMode ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = useFastMode ? CompositingQuality.HighSpeed : CompositingQuality.HighQuality;

                    if (crop.HasValue)
                        g.DrawImage(Image, imageRect, crop.Value, GraphicsUnit.Pixel);
                    else
                        g.DrawImage(Image, imageRect);
                }
            }

            e.Graphics.DrawImageUnscaled(cachedImage, 0, 0);
            DrawGrid(e.Graphics);

            if (isDown && !p2.IsEmpty)
            {
                var rect = new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));

                Region region = new Region(imageRect);
                region.Exclude(rect);
                e.Graphics.FillRegion(overlayBrush, region);
                region.Dispose();
                Cursor.Current = Cursors.Hand;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (Image == null || e.Button != MouseButtons.Left)
            {
                isDown = false;
                return;
            }

            int minDim = crop.HasValue ? Math.Min(crop.Value.Width, crop.Value.Height) : Math.Min(Image.Width, Image.Height);

            if (minDim <= 6)
            {
                isDown = false;
                return;
            }

            p1 = SnapToPixel(ClampToImage(e.Location));
            p2 = Point.Empty;
            isDown = true;
        }

        private Point SnapToPixel(Point screenPoint)
        {
            double pixelX = (screenPoint.X - imageRect.X) / scale + 1;
            double pixelY = (screenPoint.Y - imageRect.Y) / scale + 1;

            int snappedPixelX = (int)Math.Floor(pixelX);
            int snappedPixelY = (int)Math.Floor(pixelY);

            int screenX = imageRect.X + (int)Math.Round(snappedPixelX * scale);
            int screenY = imageRect.Y + (int)Math.Round(snappedPixelY * scale);

            return new Point(screenX, screenY);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDown || Image == null) return;
            p2 = ClampToImage(e.Location);
            Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (Image == null || e.Button != MouseButtons.Left)
            {
                isDown = false;
                return;
            }

            Cursor.Current = Cursors.Default;
            isDown = false;

            if (!p2.IsEmpty && Math.Abs(p1.X - p2.X) > 20 && Math.Abs(p1.Y - p2.Y) > 20)
                ApplyCrop();

            Invalidate();
        }

        private void ApplyCrop()
        {
            int x1 = (int)Math.Floor((p1.X - imageRect.X) / scale);
            int y1 = (int)Math.Floor((p1.Y - imageRect.Y) / scale);
            int x2 = (int)Math.Floor((p2.X - imageRect.X) / scale);
            int y2 = (int)Math.Floor((p2.Y - imageRect.Y) / scale);

            if (crop.HasValue)
            {
                x1 += crop.Value.X;
                y1 += crop.Value.Y;
                x2 += crop.Value.X;
                y2 += crop.Value.Y;
            }

            int minX = Math.Max(0, Math.Min(x1, x2));
            int minY = Math.Max(0, Math.Min(y1, y2));
            int maxX = Math.Min(Image.Width, Math.Max(x1, x2));
            int maxY = Math.Min(Image.Height, Math.Max(y1, y2));

            int w = maxX - minX;
            int h = maxY - minY;

            if (w >= 6 && h >= 6)
            {
                crop = new Rectangle(minX, minY, w, h);
                InvalidateBoth();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InvalidateCache();
                resizeTimer?.Dispose();
                base.Image?.Dispose();
                base.Image = null;
            }
            base.Dispose(disposing);
        }
    }
}