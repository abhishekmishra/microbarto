using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace microbarto
{
    internal class MbToolbarButton : ToolStripButton
    {
        private static readonly Color defaultButtonBgColor = Color.FromArgb(0x22, 0x22, 0x22);
        private static readonly Color defaultButtonFgColor = Color.FromArgb(0xdd, 0xdd, 0xdd);
        private static readonly Color defaultButtonHoverBgColor = Color.FromArgb(0x44, 0x44, 0x44);
        private static readonly Color defaultButtonHoverBorderColor = Color.FromArgb(0xaa, 0xaa, 0xaa);
        private static readonly Brush defaultButtonHoverBgBrush = new SolidBrush(defaultButtonHoverBgColor);
        private static readonly Brush defaultButtonHoverBorderBrush = new SolidBrush(defaultButtonHoverBorderColor);
        private static readonly Pen defaultButtonHoverBorderPen = new(defaultButtonHoverBorderBrush);
        private static readonly int defaultButtonMargin = 0;
        private static readonly int defaultButtonPadding = 0;

        public Guid Id { get; private set; }
        public Brush ButtonHoverBgBrush { get; private set; } = defaultButtonHoverBgBrush;
        public Pen ButtonHoverBorderPen { get; private set; } = defaultButtonHoverBorderPen;

        public MbToolbarButton() : base("noname")
        {
            InitButton();
        }

        public MbToolbarButton(string text) : base(text)
        {
            InitButton();
        }

        public MbToolbarButton(Image image) : base(image)
        {
            InitButton();
        }

        public MbToolbarButton(string text, Image image) : base(text, image)
        {
            InitButton();
        }

        public MbToolbarButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            InitButton();
        }

        public MbToolbarButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            InitButton();
        }

        private void InitButton()
        {
            Id = Guid.NewGuid();
            BackColor = defaultButtonBgColor;
            ForeColor = defaultButtonFgColor;
            Margin = new System.Windows.Forms.Padding(defaultButtonMargin);
            Padding = new System.Windows.Forms.Padding(defaultButtonPadding);
        }
    }
}
