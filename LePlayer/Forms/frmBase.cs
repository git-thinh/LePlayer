using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using AxWMPLib;
using System;
using System.IO;
using System.Diagnostics;

namespace System
{
    public partial class frmBase : Form, IForm
    {
        #region [ Contractor ]
        ControlTransparent ui_move;

        IconControl ui_close;
        IconControl ui_minus;

        IconControl ui_list;
        IconControl ui_arrow_right;
        IconControl ui_arrow_left;

        Panel ui_control;

        #endregion

        public frmBase(IContext context)
        {
            this.Context = context;

            this.Icon = LePlayer.Properties.Resources.icon;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Shown += (se, ev) => LoadControls();
        }

        void LoadControls()
        {
            this.Width = 600;
            this.Height = 340;
            
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
            
            ////////////////////////////////////////////////////////////////////////////
            ui_move.SendToBack();
        }



        public IContext Context { get; private set; }
        public string URL_NEXT { get; set; }

        public virtual void Go(string url)
        {
            //Browser.Stop();
            //Browser.Load(url);
        }

        public virtual void RaiseEventMenuBrowser(int menuCode)
        {
            //switch (menuCode)
            //{
            //    case 27004: // Open log request resource
            //        File.WriteAllText("request.txt", LogBuilder.ToString());
            //        Process.Start("request.txt");
            //        break;
            //    case 27005: // clear log 
            //        this.ClearLog();
            //        break;
            //    case 27100: // exit application
            //        this.CrossThreadCalls(() => this.CloseForm());
            //        break;
            //}
        }

        public virtual void CloseForm()
        {
            this.Close();
        }

        public virtual void ClearLog() {
            //LogBuilder.Clear();
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