using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    class WatermarkTextBox : TextBox
    {
        public WatermarkTextBox ()
        {
            WatermarkText = "Type here";
            WatermarkActive = true;

            this.Text = WatermarkText;
            this.ForeColor = Color.Gray;

            GotFocus += (source, e) => {
                RemoveWatermak();
            };

            LostFocus += (source, e) => {
                ApplyWatermark();
            };
        }

        public string WatermarkText { get; set; }

        public bool WatermarkActive { get; set; }

        public void RemoveWatermak ()
        {
            if (this.WatermarkActive) {
                this.WatermarkActive = false;
                this.Text = "";
                this.ForeColor = Color.Black;
            }
        }

        public void ApplyWatermark ()
        {
            if (!this.WatermarkActive && string.IsNullOrEmpty(this.Text) || ForeColor == Color.Gray) {
                this.WatermarkActive = true;
                this.Text = WatermarkText;
                this.ForeColor = Color.Gray;
            }
        }

        public void ApplyWatermark (string newText)
        {
            Text = "";
            WatermarkText = newText;
            ApplyWatermark();
        }

        public void ApplyText (string text)
        {
            RemoveWatermak();
            Text = text;
        }
    }
}
