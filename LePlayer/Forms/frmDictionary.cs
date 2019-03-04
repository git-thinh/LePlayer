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
    public class frmDictionary : frmMonitor, IForm_Dictionary
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
        public frmDictionary(IContext context) : base(FORM_TYPE.DICTIONARY, context, FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE)
        {
            this.Text = "";
            this.VisiblePanelTransparentToMove = false;
            this.VisibleMenuButton = true;
            this.VisibleMoveButton = true;

            LogBuilder = new StringBuilder();


            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);

            this.Shown += (se, ev) =>
            {
                //this.Top = 0;
                this.Left = 0;// Screen.PrimaryScreen.WorkingArea.Width - 1030;
                this.Width = 450;// Screen.PrimaryScreen.WorkingArea.Width / 2;
                this.Height = 610;
                //this.Height = Screen.PrimaryScreen.WorkingArea.Height;

                //Browser.Location = new System.Drawing.Point(1, 1);
                //Browser.Width = this.Width - 16;
                //Browser.Height = this.Height - 10;
                //Browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                //BackColor = Color.Black;
                //Browser.SendToBack();
            };

            var browser = new ChromiumWebBrowser(URL)
            {
                //Location = new System.Drawing.Point(3, 7),
                //Width = 1024,
                //Height = 800,
                ////Height = Screen.PrimaryScreen.WorkingArea.Height - 10,
                //Dock = DockStyle.None,
                Dock = DockStyle.Fill,
                BrowserSettings =
                {
                    DefaultEncoding = "UTF-8",
                    WebGl = CefState.Disabled,
                    //For Origin file:// is not allowed by Access-Control-Allow-Origin.) error
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled,
                    WebSecurity = CefState.Disabled,
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
            var requestResource = new DictionaryRequestResourceHandlerFactory();
            requestResource.OnEventUrlArrived += (string url) =>
            {
                //LogBuilder.AppendLine(Environment.NewLine);
                //LogBuilder.AppendLine(url);

                //Console.WriteLine(Environment.NewLine);
                //Console.WriteLine(url);
            };
            requestResource.OnEventUrlVoiceCallback += (string url) =>
            {

            };
            browser.ResourceHandlerFactory = requestResource;
            ////browser.LifeSpanHandler = new BrowserLifeSpanHandler();
            //this.Controls.Add(browser);
            this.AddControl(browser);
            this.Browser = browser;

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //browser.JavascriptObjectRepository.Register("bound", new BoundObject(), isAsync: false);
            browser.JavascriptObjectRepository.Register("boundAsync", new BoundObject(), true);

        }

        private void jsCall()
        {
            string EvaluateJavaScriptResult;
            var frame = this.Browser.GetMainFrame();
            var task = frame.EvaluateScriptAsync("(function() { return document.getElementById('searchInput').value; })();", null);

            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    EvaluateJavaScriptResult = response.Success ? (response.Result.ToString() ?? "null") : response.Message;
                    MessageBox.Show(EvaluateJavaScriptResult);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
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

    public class DictionaryRequestResourceHandlerFactory : IResourceHandlerFactory
    {
        /// <summary>
        /// Raised when a request resource event arrives.
        /// </summary>
        public event Action<string> OnEventUrlArrived;
        public event Action<string> OnEventUrlVoiceCallback;

        bool IResourceHandlerFactory.HasHandlers
        {
            get { return true; }
        }

        void SendLogUrl(string url)
        {
            if (url.Contains("chrome-devtools") || url.Contains("chrome-extension")) return;
            if (OnEventUrlArrived != null) OnEventUrlArrived(url);
        }

        IResourceHandler IResourceHandlerFactory.GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            string url = request.Url;

            //if (url.Contains("facebook")
            //    //|| url.Contains("google") 
            //    || url.Contains("google-analytics")
            //    || url.Contains("googlesyndication")
            //    || url.Contains("adservice.google.com")
            //    || url.Contains("googletagservices")
            //    || url.Contains("click")
            //    || url.Contains("sharethis")
            //    || url.Contains("counter")
            //    || url.Contains("adserver")
            //    || url.Contains("reach")
            //    || url.Contains("visitor"))
            //{
            //    SendLogUrl("##> " + request.Method + ": " + url);
            //    return new RequestResourceCanceler();
            //}

            if (url.Contains("translate_tts"))
            {
                Console.WriteLine(Environment.NewLine + "!!!!>: " + url);
                if (OnEventUrlVoiceCallback != null) OnEventUrlVoiceCallback(url);
                //return new RequestResourceCanceler();
            }

            //SendLogUrl("--> " + request.Method + ": " + url);
            return null;
        }
    }
}

