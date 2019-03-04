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
        public frmMediaExplorer(IContext context) : base(FORM_TYPE.MEDIA_EXPLORER, context, FORM_STYLE.TEXT_COLOR_BLACK___BG_COLOR_WHITE, 800, 600)
        {
            this.Text = "";
            this.VisiblePanelTransparentToMove = false;
            this.VisibleMenuButton = false;
            this.VisibleMoveButton = true;
            this.EnableResize = false;

            LogBuilder = new StringBuilder();

            //this.Shown += (se, ev) =>
            //{
            //this.Width = 800;
            //this.Height = 480;
            //};

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

            var handler = browser.ResourceHandlerFactory as DefaultResourceHandlerFactory;
            if (handler != null)
            {
                string s = JsonConvert.SerializeObject(DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => new { name = d.Name }));
                handler.RegisterHandler("http://pc/api/drive", ResourceHandler.GetByteArray(s, Encoding.UTF8), "application/json");
            }

            //For legacy biding we'll still have support for
            //CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //browser.RegisterAsyncJsObject("__api", new AsyncJsAPI()); //Standard object rego
            ////browser.RegisterAsyncJsObject("bound", new AsyncJsAPI(), BindingOptions.DefaultBinder); //Use the default binder to serialize values into complex objects
            ////browser.RegisterAsyncJsObject("bound", new AsyncJsAPI(), new BindingOptions { CamelCaseJavascriptNames = false, Binder = new MyCustomBinder() }); //No camelcase of names and specify a default binder
            //browser.RegisterAsyncJsObject("boundAsync", new AsyncBoundObject()); //Standard object rego
            //browser.RegisterAsyncJsObject("bound", new AsyncBoundObject(), BindingOptions.DefaultBinder); //Use the default binder to serialize values into complex objects
            //browser.RegisterAsyncJsObject("bound", new AsyncBoundObject(), new BindingOptions { CamelCaseJavascriptNames = false, Binder = new MyCustomBinder() }); //No camelcase of names and specify a default binder

            //browser.RegisterJsObject("bound", new BoundObject()); //Standard object registration
            //browser.RegisterJsObject("bound", new BoundObject(), BindingOptions.DefaultBinder); //Use the default binder to serialize values into complex objects
            //browser.RegisterJsObject("bound", new BoundObject(), new BindingOptions { CamelCaseJavascriptNames = false, Binder = new MyCustomBinder() }); //No camelcase of names and specify a default binder

            //browser.JavascriptObjectRepository.Register("bound", new BoundObject(), isAsync: false);
            //browser.JavascriptObjectRepository.Register("bound", new BoundObject(), isAsync: false, options: BindingOptions.DefaultBinder);
            //browser.JavascriptObjectRepository.Register("boundAsync", new AsyncBoundObject(), isAsync: true, options: BindingOptions.DefaultBinder);

            ////If you call CefSharp.BindObjectAsync in javascript and pass in the name of an object which is not yet
            ////bound, then ResolveObject will be called, you can then register it
            //browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            //{
            //    var repo = e.ObjectRepository;
            //    if (e.ObjectName == "boundAsync2")
            //    {
            //        repo.Register("boundAsync2", new AsyncBoundObject(), isAsync: true, options: BindingOptions.DefaultBinder);
            //    }
            //};

            ////Firstly you can register an object in a similar fashion to before
            ////For standard object registration (equivalent to RegisterJsObject)
            //browser.JavascriptObjectRepository.Register("bound", new BoundObject(), false);
            ////For async object registration (equivalent to RegisterJsObject)
            ////browser.JavascriptObjectRepository.Register("boundAsync", new BoundObject(), true);

            ////Ability to resolve an object, instead of forcing object registration before the browser has been initialized.
            //browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            //{
            //    var repo = e.ObjectRepository;
            //    if (e.ObjectName == "boundAsync2")
            //    {
            //        //repo.Register("boundAsync2", new AsyncBoundObject(), isAsync: true, options: bindingOptions);
            //    }
            //};
            //browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            //{
            //    var name = e.ObjectName;

            //    Debug.WriteLine($"Object {e.ObjectName} was bound successfully.");
            //};


            ////Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
            //browser.LoadingStateChanged += (sender, args) =>
            //{
            //    //Wait for the Page to finish loading
            //    if (args.IsLoading == false)
            //    {
            //        //Browser.ExecuteJavaScriptAsync("alert('All Resources Have Loaded');");
            //    }
            //};

            ////Wait for the MainFrame to finish loading
            //browser.FrameLoadEnd += (sender, args) =>
            //{
            //    //Wait for the MainFrame to finish loading
            //    if (args.Frame.IsMain)
            //    {
            //        args.Frame.ExecuteJavaScriptAsync("console.log('MainFrame finished loading = " + args.Frame.Url + "');");
            //    }
            //};








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

