namespace Image_View
{
    partial class form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeImages();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form));
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.themeButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.printButton = new System.Windows.Forms.ToolStripButton();
            this.restoreButton = new System.Windows.Forms.ToolStripButton();
            this.rotateButton = new System.Windows.Forms.ToolStripButton();
            this.mirrorButton = new System.Windows.Forms.ToolStripButton();
            this.pictureBox = new Image_View.DontBlurBox();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // openFile
            // 
            this.openFile.AutoUpgradeEnabled = false;
            // 
            // toolStrip
            // 
            this.toolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.toolStrip.AutoSize = false;
            this.toolStrip.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.themeButton,
            this.saveButton,
            this.printButton,
            this.restoreButton,
            this.rotateButton,
            this.mirrorButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip.ShowItemToolTips = false;
            this.toolStrip.Size = new System.Drawing.Size(467, 30);
            this.toolStrip.TabIndex = 2;
            // 
            // openButton
            // 
            this.openButton.AutoSize = false;
            this.openButton.AutoToolTip = false;
            this.openButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.openButton.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.openButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Margin = new System.Windows.Forms.Padding(3, 2, 2, 3);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(36, 26);
            this.openButton.Text = "☰";
            this.openButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // themeButton
            // 
            this.themeButton.AutoSize = false;
            this.themeButton.AutoToolTip = false;
            this.themeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.themeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.themeButton.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.themeButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.themeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.themeButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.themeButton.Name = "themeButton";
            this.themeButton.Size = new System.Drawing.Size(36, 26);
            this.themeButton.Text = "⛭";
            this.themeButton.Click += new System.EventHandler(this.ThemeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.AutoSize = false;
            this.saveButton.AutoToolTip = false;
            this.saveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveButton.Enabled = false;
            this.saveButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(64, 26);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // printButton
            // 
            this.printButton.AutoSize = false;
            this.printButton.AutoToolTip = false;
            this.printButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.printButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.printButton.Enabled = false;
            this.printButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.printButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(64, 26);
            this.printButton.Text = "Print";
            this.printButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // restoreButton
            // 
            this.restoreButton.AutoSize = false;
            this.restoreButton.AutoToolTip = false;
            this.restoreButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.restoreButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.restoreButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(64, 26);
            this.restoreButton.Text = "Restore";
            this.restoreButton.Click += new System.EventHandler(this.RestoreButton_Click);
            // 
            // rotateButton
            // 
            this.rotateButton.AutoSize = false;
            this.rotateButton.AutoToolTip = false;
            this.rotateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rotateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.rotateButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rotateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(64, 26);
            this.rotateButton.Text = "Rotate";
            this.rotateButton.Click += new System.EventHandler(this.RotateButton_Click);
            // 
            // mirrorButton
            // 
            this.mirrorButton.AutoSize = false;
            this.mirrorButton.AutoToolTip = false;
            this.mirrorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mirrorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mirrorButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mirrorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mirrorButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 3);
            this.mirrorButton.Name = "mirrorButton";
            this.mirrorButton.Size = new System.Drawing.Size(64, 26);
            this.mirrorButton.Text = "Mirror";
            this.mirrorButton.Click += new System.EventHandler(this.MirrorButton_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox.CausesValidation = false;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.ErrorImage = null;
            this.pictureBox.Image = null;
            this.pictureBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox.InitialImage = null;
            this.pictureBox.Location = new System.Drawing.Point(0, 30);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(467, 368);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
            this.pictureBox.MouseEnter += new System.EventHandler(this.PictureBox_MouseEnter);
            // 
            // form
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(467, 398);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.toolStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(290, 275);
            this.Name = "form";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Monocle";
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton themeButton;
        private System.Windows.Forms.ToolStripButton rotateButton;
        private DontBlurBox pictureBox;
        private System.Windows.Forms.ToolStripButton mirrorButton;
        private System.Windows.Forms.ToolStripButton restoreButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton printButton;
    }
}

