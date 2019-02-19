using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using AxWMPLib;
using System;

namespace LePlayer
{
    public partial class frmMediaShortcut : frmBase, IForm_MediaVideo
    {
        #region [ Contractor ]

        ControlTransparent ui_move;

        IconControl ui_arrow_right;
        IconControl ui_arrow_left;

        Panel ui_control;

        Label ui_text;
        Label ui_pronunce;
        Label ui_mean_vi;

        #endregion

        public frmMediaShortcut(IContext context) : base(FORM_TYPE.MEDIA_VIDEO, context, FORM_STYLE.TEXT_COLOR_GRAY___BG_COLOR_BLACK)
        {
            this.VisiblePanelTransparentToMove = true;
            this.VisibleMenuButton = false;
            this.VisibleMoveButton = false;

            this.Text = "Player";
            this.Shown += (se, ev) => LoadControls();
        }

        void LoadControls()
        {
            this.Width = 236;
            this.Height = 99;

            Font font = new Font("Arial", 19f, FontStyle.Regular);
            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString("Websites where you talk about yourself", font);
            this.Width = (int)size.Width;
            this.Height = 69;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;

            ui_move = new ControlTransparent()
            {
                Location = new Point(0, 0),
                Width = this.Width - 5,
                Height = this.Height - 5,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            this.Controls.Add(ui_move);
            ui_move.MouseMove += this.f_form_move_MouseDown;
            ui_move.MouseClick += (s, v) =>
            {
            };

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            const int _iconSize = 11;
            const int _iconCounter = 2;
            ui_control = new Panel()
            {
                Padding = new Padding(5, 5, 5, 0),
                Height = _iconSize + 5,
                Width = _iconSize * _iconCounter + 15,
                Location = new Point(0, -3),
                BackColor = Color.Black,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };
            this.AddControl(ui_control);

            ui_arrow_left = new IconControl(this.FormStyle, ICON_TYPE.arrow_left, _iconSize) { Dock = DockStyle.Left };
            ui_arrow_right = new IconControl(this.FormStyle, ICON_TYPE.arrow_right, _iconSize) { Dock = DockStyle.Left };

            ui_control.Controls.AddRange(new Control[] {
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_arrow_right,
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_arrow_left,
            });

            ////////////////////////////////////////////////////////////////////////////


            ui_pronunce = new Label()
            {
                Text = "/ˈkɒm.ən.li/",
                TextAlign = ContentAlignment.BottomRight,
                ForeColor = Color.Gray,
                //BackColor = Color.Red,
                Font = new Font("Arial", 15f, FontStyle.Bold),
                Location = new Point(0, this.Height - 25),
                Height = 25,
                Width = this.Width - 7,
                Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom
            };
            this.Controls.Add(ui_pronunce);

            ui_text = new Label() {
                //Text = "Use Common Commonly",
                Text = "Websites where you talk about yourself",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                //BackColor = Color.Orange,
                Font = new Font("Arial", 19f, FontStyle.Regular),
                Location = new Point(0,11),
                Height = this.Height - 36,
                Width = this.Width,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom
            };
            this.AddControl(ui_text);

            ui_pronunce.MouseMove += this.f_form_move_MouseDown;
            ui_move.SendToBack();
            ui_pronunce.BringToFront();
        }

    }
}