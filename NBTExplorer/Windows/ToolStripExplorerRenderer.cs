using System.Drawing;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    class ToolStripExplorerRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderSplitButtonBackground (ToolStripItemRenderEventArgs e)
        {
            base.OnRenderSplitButtonBackground(e);

            ToolStripSplitButton button = e.Item as ToolStripSplitButton;
            Graphics g = e.Graphics;

            if (button == null)
                return;

            Rectangle bounds = new Rectangle(Point.Empty, button.Size);
            Rectangle arrowButtonBounds = new Rectangle(button.DropDownButtonBounds.X + 1, button.DropDownButtonBounds.Y + 1,
                button.DropDownButtonBounds.Width - 2, button.DropDownButtonBounds.Height - 2);

            using (Brush brush = new SolidBrush(ColorTable.ToolStripGradientEnd)) {
                g.FillRectangle(brush, arrowButtonBounds);
            }

            if (button.Pressed)
                DrawArrow(new ToolStripArrowRenderEventArgs(g, button, arrowButtonBounds, SystemColors.ControlText, ArrowDirection.Down));
            else
                DrawArrow(new ToolStripArrowRenderEventArgs(g, button, arrowButtonBounds, SystemColors.ControlText, ArrowDirection.Right));
        }

        protected override void OnRenderToolStripBackground (ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown || e.ToolStrip is MenuStrip || e.ToolStrip is StatusStrip)
                base.OnRenderToolStripBackground(e);
            else {
                using (Brush brush = new SolidBrush(ColorTable.ToolStripGradientEnd)) {
                    e.Graphics.FillRectangle(brush, e.ToolStrip.Bounds);
                }
            }
        }

        protected override void OnRenderToolStripBorder (ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown || e.ToolStrip is MenuStrip || e.ToolStrip is StatusStrip)
                base.OnRenderToolStripBorder(e);
        }
    }
}
