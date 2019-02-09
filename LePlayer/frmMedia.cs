using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using AxWMPLib;

namespace System
{
    public partial class frmMedia : Form
    {
        #region [ Contractor ]

        AxWindowsMediaPlayer ui_media;
        ControlTransparent ui_move;
        IconControl ui_close;
        IconControl ui_minus;
        IconControl ui_list;
        IconControl ui_pause;
        IconControl ui_open;
        IconControl ui_play;
        IconControl ui_arrow_right;
        IconControl ui_arrow_left;
        IconControl ui_repeat;
        IconControl ui_random;
        IconControl ui_edit_cc;
        IconControl ui_cc;
        IconControl ui_star;
        IconControl ui_volum_up;
        IconControl ui_volum_down;
        IconControl ui_stop;

        Panel ui_control;
        
        #endregion

        public frmMedia()
        {
            this.Text = "Player";
            this.Icon = LePlayer.Properties.Resources.icon;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Shown += (se, ev) => LoadControls();

            this.ResizeEnd += (se, ev) =>
            {
            };
        }

        void LoadControls()
        {
            this.Width = 600;
            this.Height = 340;

            // MEDIA
            ui_media = new AxWindowsMediaPlayer()
            {
                Location = new Point(0, 2),
                Width = this.Width,
                Height = this.Height - 7,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            ui_media.Enabled = true;
            ui_media.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(this.f_media_event_PlayStateChange);
            this.Controls.Add(ui_media);
            ui_media.settings.volume = 100;
            ui_media.uiMode = "none";
            ui_media.URL = @"D:\Talk To Miss Lan\Pronunce\Bài 03 Nguyên Âm-Phần 01 Nguyên Âm Đơn_Trim.mp4";
            //ui_media.URL = @"D:\MrThinh\EL\Luyện nghe tiếng Anh qua video VOA-Có phụ đề tiếng anh-Bài 1.mp4";
            //ui_media.URL = @"D:\EL\Social Networking\1. Choosing a Job.mp3";

            ui_move = new ControlTransparent()
            {
                Location = new Point(0, 0),
                Width = this.Width - 5,
                Height = this.Height - 5,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            this.Controls.Add(ui_move);
            ui_move.MouseMove += f_form_move_MouseDown;
            ui_move.MouseClick += (s, v) => {
                if (ui_media.playState == WMPLib.WMPPlayState.wmppsPlaying) {
                    ui_media.Ctlcontrols.pause();
                }
                else {
                    ui_media.Ctlcontrols.play();
                }
            };

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            ui_list = new IconControl(Color.WhiteSmoke, IconType.menu, 11)
            {
                Location = new Point(2, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Black,
            };
            this.Controls.Add(ui_list);

            ui_minus = new IconControl(Color.White, IconType.minus, 11)
            {
                Width = 16,
                Height = 14,
                Location = new Point(this.Width - 32, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Black,
            };
            ui_minus.Click += (s, v) =>
            {
                WindowState = FormWindowState.Minimized;
            };
            this.Controls.Add(ui_minus);

            ui_close = new IconControl(Color.WhiteSmoke, IconType.close, 11)
            {
                Width = 16,
                Height = 14,
                Location = new Point(this.Width - 16, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Black,
            };
            this.Controls.Add(ui_close);
            ui_close.Click += (s, v) =>
            {
                this.Close();
            };
            this.Controls.Add(new Label()
            {
                Width = 36,
                Height = 16,
                Location = new Point(this.Width - 36, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Black,
            });

            ui_open = new IconControl(Color.White, IconType.plus, 11)
            {
                Location = new Point(4, this.Height - 15),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.Black,
            };
            this.Controls.AddRange(new Control[] {
                ui_open,
                new Label() { Width = 16, Height = 18, Location = new Point(1, this.Height - 18), Anchor = AnchorStyles.Bottom | AnchorStyles.Left}
            });
            ui_open.Click += (s, v) =>
            {

            };

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            const int _iconSize = 18;
            const int _iconCounter = 7;
            ui_control = new Panel()
            {
                Padding = new Padding(5, 5, 5, 0),
                Height = _iconSize + 5,
                Width = _iconSize * _iconCounter + 10 + 5 * _iconCounter,
                Location = new Point(this.Width - 5 * _iconCounter - _iconSize * _iconCounter - 10,
                this.Height - _iconSize - 10),
                //BackColor = Color.Blue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };
            this.Controls.Add(ui_control);

            ui_arrow_left = new IconControl(Color.OrangeRed, IconType.arrow_left, _iconSize) { Dock = DockStyle.Left };
            ui_arrow_right = new IconControl(Color.OrangeRed, IconType.arrow_right, _iconSize) { Dock = DockStyle.Left };

            ui_play = new IconControl(Color.OrangeRed, IconType.play, _iconSize) { Dock = DockStyle.Left, Visible = false };
            ui_pause = new IconControl(Color.OrangeRed, IconType.pause, _iconSize) { Dock = DockStyle.Left };
            ui_stop = new IconControl(Color.OrangeRed, IconType.stop, _iconSize) { Dock = DockStyle.Left };

            ui_repeat = new IconControl(Color.OrangeRed, IconType.repeat, _iconSize) { Dock = DockStyle.Left };
            ui_random = new IconControl(Color.OrangeRed, IconType.random, _iconSize) { Dock = DockStyle.Left };
            ui_edit_cc = new IconControl(Color.OrangeRed, IconType.cc, _iconSize) { Dock = DockStyle.Left };
            ui_cc = new IconControl(Color.OrangeRed, IconType.cc, _iconSize) { Dock = DockStyle.Left };
            ui_star = new IconControl(Color.OrangeRed, IconType.star, _iconSize) { Dock = DockStyle.Left };
            ui_volum_down = new IconControl(Color.OrangeRed, IconType.volum_down, _iconSize) { Dock = DockStyle.Left };
            ui_volum_up = new IconControl(Color.OrangeRed, IconType.volum_up, _iconSize) { Dock = DockStyle.Left };

            ui_control.Controls.AddRange(new Control[] {
                //ui_repeat,
                //new Label() { Width = 5, Dock = DockStyle.Left },
                //ui_random,
                ui_repeat,
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_cc,
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_volum_up,
                ui_volum_down,
                new Label() { Width = 5, Dock = DockStyle.Left },
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_arrow_right,
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_play,
                ui_stop,
                //ui_pause,
                new Label() { Width = 5, Dock = DockStyle.Left },
                ui_arrow_left,
            });

            ////////////////////////////////////////////////////////////////////////////
            ui_move.SendToBack();
            ui_media.SendToBack();
        }

        void f_media_event_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            /**** Don't add this if you want to play it on multiple screens***** /
             * 
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
            /********************************************************************/

            if (ui_media.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                //Application.Exit();
            }


            switch (ui_media.playState)
            {
                case WMPLib.WMPPlayState.wmppsTransitioning:

                    break;
                case WMPLib.WMPPlayState.wmppsPlaying:
                    ui_play.Visible = true;
                    ui_stop.Visible = false;
                    break;
                case WMPLib.WMPPlayState.wmppsMediaEnded:
                    break;
                case WMPLib.WMPPlayState.wmppsStopped:
                    ui_play.Visible = false;
                    ui_stop.Visible = true;
                    break;
            }


        }

        #region [ FORM MOVE ]

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void f_form_move_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        #region [ DRAW FORM ]

        private const int cGrip = 5;      // Grip size
        private const int cCaption = 7;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);

            //rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.Black, rc);
        }

        ///
        /// Handling the window messages
        ///
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            else if (m.Msg == 0x85)
            {
                // WM_NCPAINT
                var ncAttr = NativeMethods.DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY;
                var dwAttrPntr = 2;
                NativeMethods.DwmSetWindowAttribute(Handle, ncAttr, ref dwAttrPntr, 4);

                var margins = new NativeMethods.MARGINS()
                {
                    cyBottomHeight = 1,
                    cxLeftWidth = 1,
                    cxRightWidth = 1,
                    cyTopHeight = 1
                };

                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref margins);

            }
            base.WndProc(ref m);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Allows minimizing from taskbar.
        /// </summary>
        private const int WS_MINIMIZEBOX = 0x20000;

        /// <summary>
        /// Paints all descendants of a window in bottom-to-top painting order
        /// using double-buffering.
        /// </summary>
        private const int WS_EX_COMPOSITED = 0x02000000;

        // Modify the parameters of the form's handle (unmanaged code).
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                //aeroShadow = IsDwmCompositionEnabled();

                var cp = base.CreateParams;

                // WS_MINIMIZEBOX   : allows minimizing the software from the taskbar
                // WS_EX_COMPOSITED : paints bottom-to-top. Reduces flicker greatly

                cp.Style = cp.Style | WS_MINIMIZEBOX;
                cp.ExStyle |= WS_EX_COMPOSITED;

                return cp;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        #endregion
    }
}