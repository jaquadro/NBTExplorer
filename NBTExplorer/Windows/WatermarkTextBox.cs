using System.Drawing;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    internal class WatermarkTextBox : TextBox
    {
        public WatermarkTextBox()
        {
            WatermarkText = "Type here";
            WatermarkActive = true;

            Text = WatermarkText;
            ForeColor = Color.Gray;

            GotFocus += (source, e) => { RemoveWatermak(); };

            LostFocus += (source, e) => { ApplyWatermark(); };
        }

        public string WatermarkText { get; set; }

        public bool WatermarkActive { get; set; }

        public void RemoveWatermak()
        {
            if (WatermarkActive)
            {
                WatermarkActive = false;
                Text = "";
                ForeColor = Color.Black;
            }
        }

        public void ApplyWatermark()
        {
            if (!WatermarkActive && string.IsNullOrEmpty(Text) || ForeColor == Color.Gray)
            {
                WatermarkActive = true;
                Text = WatermarkText;
                ForeColor = Color.Gray;
            }
        }

        public void ApplyWatermark(string newText)
        {
            Text = "";
            WatermarkText = newText;
            ApplyWatermark();
        }

        public void ApplyText(string text)
        {
            RemoveWatermak();
            Text = text;
        }
    }
}