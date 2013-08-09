using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class CancelSearchForm : Form
    {
        public CancelSearchForm ()
        {
            InitializeComponent();
        }

        public string SearchPathLabel
        {
            get { return _searchPathLabel.Text; }
            set { _searchPathLabel.Text = value; }
        }
    }
}
