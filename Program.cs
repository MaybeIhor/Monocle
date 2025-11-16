using System;
using System.Windows.Forms;

namespace Image_View
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                string filePath = args[0];
                Application.Run(new form(filePath));
            }
            else
            {
                Application.Run(new form());
            }
        }
    }
    public class FixedRenderer : ToolStripSystemRenderer
    {
        public FixedRenderer() { }
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }
}
