using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections.Generic;
using LePlayer;
using System.Collections.Concurrent;

namespace System
{
    public partial class frmBase : Form, IForm
    {
        static ConcurrentDictionary<FORM_TYPE, object> storeForm = new ConcurrentDictionary<FORM_TYPE, object>() { };
        public static void createAndShowByFormType(IContext context, FORM_TYPE formType)
        {
            if (storeForm.ContainsKey(formType))
            {
                Form f = (Form)storeForm[formType];
                if (f.IsDisposed) {
                    object val = null;
                    storeForm.TryRemove(formType, out val);
                    createAndShowByFormType(context, formType);
                }
                else f.Show();
            }
            else
            {
                switch (formType)
                {
                    case FORM_TYPE.BROWSER:
                        var w = new frmBrowser(context);
                        storeForm.TryAdd(formType, w);
                        w.Show();
                        break;
                    case FORM_TYPE.DICTIONARY:
                        var dic = new frmDictionary(context);
                        storeForm.TryAdd(formType, dic);
                        dic.Show();
                        break;
                    case FORM_TYPE.MEDIA_VIDEO:
                        var video = new frmMediaVideo(context);
                        storeForm.TryAdd(formType, video);
                        video.Show();
                        break;
                }
            }
        }

        #region [ Contractor ]

        //ControlTransparent ui_panel_transparent;
        ControlTransparent ui_panel_transparent;

        Label ui_title;
        IconControl ui_close;
        IconControl ui_minus;

        IconControl ui_move;
        IconControl ui_menu;

        Panel ui_control;

        const int _TITLE_HEIGHT = 17;

        #endregion

        public FORM_STYLE FormStyle = FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE;
        public FORM_TYPE FormType { get; private set; }
        public bool VisibleMoveButton { get; set; }
        public bool VisibleMenuButton { get; set; }
        public bool VisiblePanelTransparentToMove { get; set; }

        public frmBase(FORM_TYPE formType, IContext context, FORM_STYLE formStyle = FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE)
        {
            this.FormType = formType;
            this.FormStyle = formStyle;
            this.Context = context;

            ui_control = new Panel()
            {
                Padding = new Padding(0),
                Height = 800,
                Width = 600,
                //Location = new Point(0, _TITLE_HEIGHT),
                //BackColor = Color.Blue,
            };
            this.Controls.Add(ui_control);

            this.Icon = LePlayer.Properties.Resources.icon;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Shown += (se, ev) => LoadControls();

            switch (this.FormStyle)
            {
                case FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE:
                    this.BackColor = Color.White;
                    break;
                case FORM_STYLE.TEXT_COLOR_GRAY___BG_COLOR_BLACK:
                case FORM_STYLE.TEXT_COLOR_WHITE___BG_COLOR_BLACK:
                    this.BackColor = Color.Black;
                    break;
            }
        }

        void LoadControls()
        {
            this.Width = 600;
            this.Height = 340;
            Color TEXT_COLOR = Color.Black;
            Color BG_COLOR = Color.White;

            Label ui_box_close = null;
            switch (this.FormStyle)
            {
                case FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE:
                    TEXT_COLOR = Color.Black;
                    BG_COLOR = Color.White;
                    break;
                case FORM_STYLE.TEXT_COLOR_WHITE___BG_COLOR_BLACK:
                    TEXT_COLOR = Color.White;
                    BG_COLOR = Color.Black;
                    break;
                case FORM_STYLE.TEXT_COLOR_GRAY___BG_COLOR_BLACK:
                    TEXT_COLOR = Color.Gray;
                    BG_COLOR = Color.Black;
                    break;
            }

            //////////////////////////////////////////////////////////////////////////// 
            ui_title = new Label() { ForeColor = TEXT_COLOR, BackColor = BG_COLOR };
            ui_panel_transparent = new ControlTransparent()
            {
                Location = new Point(0, 0),
                Height = this.Height - 5,
                Width = this.Width,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                Visible = this.VisiblePanelTransparentToMove,
                //BackColor = Color.Red,
            };
            ui_move = new IconControl(this.FormStyle, ICON_TYPE.arrow_alt, 11)
            {
                Location = new Point(2, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Visible = this.VisibleMoveButton,
            };
            ui_menu = new IconControl(this.FormStyle, ICON_TYPE.menu, 11)
            {
                Location = new Point(4, this.Height - 15),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Visible = this.VisibleMenuButton,
            };
            ui_minus = new IconControl(this.FormStyle, ICON_TYPE.minus, 11)
            {
                Width = 16,
                Height = 14,
                Location = new Point(this.Width - 32, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            ui_close = new IconControl(this.FormStyle, ICON_TYPE.close, 11)
            {
                Width = 16,
                Height = 14,
                Location = new Point(this.Width - 16, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            ui_box_close = new Label()
            {
                BackColor = BG_COLOR,
                Width = 36,
                Height = 17,
                Location = new Point(this.Width - 36, -1),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            //////////////////////////////////////////////////////////////////////////// 
            ui_title.Height = _TITLE_HEIGHT;
            ui_title.Padding = new Padding(_TITLE_HEIGHT, 2, 0, 0);
            ui_title.Text = Guid.NewGuid().ToString();
            ui_title.Dock = DockStyle.Top;
            ui_title.MouseMove += f_form_move_MouseDown;
            ui_move.MouseMove += f_form_move_MouseDown;
            ui_panel_transparent.MouseMove += f_form_move_MouseDown;

            //////////////////////////////////////////////////////////////////////////// 
            ui_title.Visible = false;
            ui_control.Location = new Point(0, 0);
            ui_control.Height = this.Height - 5;
            switch (this.FormStyle)
            {
                case FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE:
                    break;
                case FORM_STYLE.TEXT_COLOR_WHITE___BG_COLOR_BLACK:
                    //ui_move.Visible = false;
                    //ui_control.Location = new Point(0, _TITLE_HEIGHT);
                    //ui_control.Height = this.Height - _TITLE_HEIGHT - 5;
                    break;
            }
            //////////////////////////////////////////////////////////////////////////// 
            this.Controls.Add(ui_title);
            this.Controls.Add(ui_move);
            this.Controls.Add(ui_minus);
            this.Controls.Add(ui_close);
            this.Controls.Add(ui_box_close);
            this.Controls.Add(ui_menu);
            this.Controls.Add(ui_panel_transparent);

            ui_minus.Click += (s, v) =>
            {
                WindowState = FormWindowState.Minimized;
            };
            ui_close.Click += (s, v) =>
            {
                //this.Close();
                this.Hide();
            };

            //////////////////////////////////////////////////////////////////////////// 
            ui_control.Width = this.Width;
            ui_control.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;

            ui_panel_transparent.SendToBack();
            ui_title.SendToBack();
            ui_control.SendToBack();
        }

        public void AddControl(Control control)
        {
            this.ui_control.Controls.Add(control);
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
            //this.Close();
        }

        public virtual void ClearLog()
        {
            //LogBuilder.Clear();
        }

        #region [ FORM MOVE ]

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public void f_form_move_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    cyBottomHeight = 0,
                    cxLeftWidth = 0,
                    cxRightWidth = 0,
                    cyTopHeight = 0
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