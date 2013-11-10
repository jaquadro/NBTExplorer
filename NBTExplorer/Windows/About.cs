using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class About : Form
    {
        public About ()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jaquadro/NBTExplorer");
        }
    }
}
