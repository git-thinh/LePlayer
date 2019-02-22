using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using AxWMPLib;
using System;

namespace LePlayer
{
    public partial class frmMediaVideo : frmMonitor, IForm_MediaVideo
    {
        #region [ Contractor ]

        AxWindowsMediaPlayer ui_media;
        ControlTransparent ui_move;
        //IconControl ui_close;
        //IconControl ui_minus;
        //IconControl ui_list;
        //IconControl ui_open;

        IconControl ui_pause;
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

        public frmMediaVideo(IContext context): base(FORM_TYPE.MEDIA_VIDEO, context, FORM_STYLE.TEXT_COLOR_WHITE___BG_COLOR_BLACK)
        {
            this.VisiblePanelTransparentToMove = true;
            this.VisibleMenuButton = true;
            this.VisibleMoveButton = false;

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
            this.Height = 345;

            // MEDIA
            ui_media = new AxWindowsMediaPlayer()
            {
                //Location = new Point(0, 2),
                //Width = this.Width,
                //Height = this.Height - 7,
                Dock = DockStyle.Fill,
                //Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            ui_media.Enabled = true;
            ui_media.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(this.f_media_event_PlayStateChange);
            //this.Controls.Add(ui_media);
            this.AddControl(ui_media);
            ui_media.settings.volume = 100;
            ui_media.uiMode = "none";
            //ui_media.URL = @"https://translate.google.com/translate_tts?ie=UTF-8&q=common&tl=en&total=1&idx=0&textlen=6&tk=202366.361594&client=webapp&prev=input";
            ui_media.URL = @"C:\test\Bài 03 Nguyên Âm-Phần 01 Nguyên Âm Đơn_Trim.mp4";
            //ui_media.URL = @"D:\Talk To Miss Lan\Pronunce\Bài 03 Nguyên Âm-Phần 01 Nguyên Âm Đơn_Trim.mp4";
            ////////ui_media.URL = @"D:\MrThinh\EL\Luyện nghe tiếng Anh qua video VOA-Có phụ đề tiếng anh-Bài 1.mp4";
            ////////ui_media.URL = @"D:\EL\Social Networking\1. Choosing a Job.mp3";

            ui_move = new ControlTransparent()
            {
                Location = new Point(0, 0),
                Width = this.Width - 5,
                Height = this.Height - 5,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            this.Controls.Add(ui_move);
            ui_move.MouseMove += this.f_form_move_MouseDown;
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

            //////ui_list = new IconControl(this._FormStyle, IconType.menu, 11)
            //////{
            //////    Location = new Point(2, 2),
            //////    Anchor = AnchorStyles.Top | AnchorStyles.Left,
            //////    BackColor = Color.Black,
            //////};
            //////this.Controls.Add(ui_list);

            //////ui_minus = new IconControl(this._FormStyle, IconType.minus, 11)
            //////{
            //////    Width = 16,
            //////    Height = 14,
            //////    Location = new Point(this.Width - 32, 2),
            //////    Anchor = AnchorStyles.Top | AnchorStyles.Right,
            //////    BackColor = Color.Black,
            //////};
            //////ui_minus.Click += (s, v) =>
            //////{
            //////    WindowState = FormWindowState.Minimized;
            //////};
            //////this.Controls.Add(ui_minus);

            //////ui_close = new IconControl(this._FormStyle, IconType.close, 11)
            //////{
            //////    Width = 16,
            //////    Height = 14,
            //////    Location = new Point(this.Width - 16, 2),
            //////    Anchor = AnchorStyles.Top | AnchorStyles.Right,
            //////    BackColor = Color.Black,
            //////};
            //////this.Controls.Add(ui_close);
            //////ui_close.Click += (s, v) =>
            //////{
            //////    this.Close();
            //////};
            //////this.Controls.Add(new Label()
            //////{
            //////    Width = 36,
            //////    Height = 16,
            //////    Location = new Point(this.Width - 36, 0),
            //////    Anchor = AnchorStyles.Top | AnchorStyles.Right,
            //////    BackColor = Color.Black,
            //////});

            //////ui_open = new IconControl(this._FormStyle, IconType.plus, 11)
            //////{
            //////    Location = new Point(4, this.Height - 15),
            //////    Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            //////    BackColor = Color.Black,
            //////};
            //////this.Controls.AddRange(new Control[] {
            //////    ui_open,
            //////    new Label() { Width = 16, Height = 18, Location = new Point(1, this.Height - 18), Anchor = AnchorStyles.Bottom | AnchorStyles.Left}
            //////});
            //////ui_open.Click += (s, v) =>
            //////{

            //////};

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

            ui_arrow_left = new IconControl(this.FormStyle, ICON_TYPE.arrow_left, _iconSize) { Dock = DockStyle.Left };
            ui_arrow_right = new IconControl(this.FormStyle, ICON_TYPE.arrow_right, _iconSize) { Dock = DockStyle.Left };

            ui_play = new IconControl(this.FormStyle, ICON_TYPE.play, _iconSize) { Dock = DockStyle.Left, Visible = false };
            ui_pause = new IconControl(this.FormStyle, ICON_TYPE.pause, _iconSize) { Dock = DockStyle.Left };
            ui_stop = new IconControl(this.FormStyle, ICON_TYPE.stop, _iconSize) { Dock = DockStyle.Left };

            ui_repeat = new IconControl(this.FormStyle, ICON_TYPE.repeat, _iconSize) { Dock = DockStyle.Left };
            ui_random = new IconControl(this.FormStyle, ICON_TYPE.random, _iconSize) { Dock = DockStyle.Left };
            ui_edit_cc = new IconControl(this.FormStyle, ICON_TYPE.cc, _iconSize) { Dock = DockStyle.Left };
            ui_cc = new IconControl(this.FormStyle, ICON_TYPE.cc, _iconSize) { Dock = DockStyle.Left };
            ui_star = new IconControl(this.FormStyle, ICON_TYPE.star, _iconSize) { Dock = DockStyle.Left };
            ui_volum_down = new IconControl(this.FormStyle, ICON_TYPE.volum_down, _iconSize) { Dock = DockStyle.Left };
            ui_volum_up = new IconControl(this.FormStyle, ICON_TYPE.volum_up, _iconSize) { Dock = DockStyle.Left };

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

    }
}