using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LePlayer
{
    public class frmDictionary : frmBase
    {
        //public IWinFormsWebBrowser Browser { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }
        //const string URL = "https://gitter.im/";
        const string URL = "https://translate.google.com/#view=home&op=translate&sl=en&tl=vi";
        //const string URL = "file:///G:/_EL/Document/forME/Vietnamese/Cau_truc_tieng_anh_The_Windy.pdf";
        //const string URL = "https://www.rong-chang.com/easyspeak/es/school01.htm";
        //const string URL = "http://root/texttospeech/index.html";
        //const string URL = "localfolder://cefsharp/home.html";
        //const string URL = "http://hook/base.js";
        //const string URL = "https://www.eslfast.com/";
        //const string URL = "https://dictionary.cambridge.org/";
        //const string URL = "https://youtube.com";
        //const string URL = "https://youtube.com";
        readonly StringBuilder LogBuilder;
        public frmDictionary(IContext context) : base(context)
        {
            LogBuilder = new StringBuilder();
            this.Text = "";


            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);

            this.Shown += (se, ev) =>
            {
                this.Top = 0;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - 1030;
                this.Width = 1030;// Screen.PrimaryScreen.WorkingArea.Width / 2;
                //this.Height = 610; 
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;

                Browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                BackColor = Color.Black;
                Browser.SendToBack();
            };

            var browser = new ChromiumWebBrowser(URL)
            {
                Location = new System.Drawing.Point(3, 7),
                Width = 1024,
                //Height = 600,
                Height = Screen.PrimaryScreen.WorkingArea.Height - 10,
                Dock = DockStyle.None,
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled,
                    WebSecurity = CefState.Disabled,
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled,
                    //ApplicationCache = CefState.Disabled
                }
            };
            browser.IsBrowserInitializedChanged += (se, ev) =>
            {
                browser.ShowDevTools();
            };
            browser.RequestHandler = new RequestHandler(this);
            browser.MenuHandler = new MenuHandler(this);
            ////browser.JsDialogHandler = new JsDialogHandler();
            ////var requestResource = new RequestResourceHandlerFactory();
            ////requestResource.OnEventUrlArrived += (string url) =>
            ////{
            ////    LogBuilder.AppendLine(Environment.NewLine);
            ////    LogBuilder.AppendLine(url);

            ////    Console.WriteLine(Environment.NewLine);
            ////    Console.WriteLine(url);
            ////};
            ////browser.ResourceHandlerFactory = requestResource;
            ////browser.LifeSpanHandler = new BrowserLifeSpanHandler();
            this.Controls.Add(browser);
            this.Browser = browser;


        }
        ~frmDictionary()
        {
            this.Browser.Dispose();
        }
                
        public override void Go(string url)
        {
            Browser.Stop();
            Browser.Load(url);
        }

        public override void RaiseEventMenuBrowser(int menuCode)
        {
            switch (menuCode)
            {
                case 27004: // Open log request resource
                    File.WriteAllText("request.txt", LogBuilder.ToString());
                    Process.Start("request.txt");
                    break;
                case 27005: // clear log 
                    this.ClearLog();
                    break;
                case 27100: // exit application
                    this.CrossThreadCalls(() => this.CloseForm());
                    break;
            }
        }

        public override void CloseForm()
        {
            this.Close();
        }

        public override void ClearLog() { LogBuilder.Clear(); }
    }
}

