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
    public class frmMediaExplorer : frmMonitor
    {
        //public IWinFormsWebBrowser Browser { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }

        const string URL = "http://root/explorer/index.html";
        readonly StringBuilder LogBuilder;
        public frmMediaExplorer(IContext context) : base(FORM_TYPE.MEDIA_EXPLORER, context, FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE)
        {
            this.Text = "";
            this.VisiblePanelTransparentToMove = false;
            this.VisibleMenuButton = false;
            this.VisibleMoveButton = true;
            this.EnableResize = false;

            LogBuilder = new StringBuilder();

            this.Shown += (se, ev) =>
            {
                this.Width = 800;
                this.Height = 480;
            };

            var browser = new ChromiumWebBrowser(URL)
            {
                Dock = DockStyle.Fill,
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled,
                    WebSecurity = CefState.Disabled,
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled,
                    ApplicationCache = CefState.Disabled
                }
            };
            browser.IsBrowserInitializedChanged += (se, ev) =>
            {
                browser.ShowDevTools();
            };
            browser.MenuHandler = new MenuHandler(this);
            this.AddControl(browser);
            this.Browser = browser;
        }
        ~frmMediaExplorer()
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

