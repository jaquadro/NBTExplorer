using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class About : Form
    {
        public About ()
        {
            InitializeComponent();

            int len = linkLabel1.Text.Length;
            System.Version version = typeof(About).Assembly.GetName().Version;

            linkLabel1.Text = linkLabel1.Text.Replace("{ver}", version.Major + "." + version.Minor + "." + version.Build);

            int adj = linkLabel1.Text.Length - len;
            linkLabel1.LinkArea = new LinkArea(linkLabel1.LinkArea.Start + adj, linkLabel1.LinkArea.Length);
        }

        private void linkLabel1_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jaquadro/NBTExplorer");
        }
    }
}
