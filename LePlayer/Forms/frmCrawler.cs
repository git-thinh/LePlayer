using CefSharp;
using CefSharp.Internals;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LePlayer
{
    public partial class frmCrawler : Form
    { 
        public frmCrawler()
        { 
            MainAsync("cache", 1.0d); 
        }

        private static async void MainAsync(string cachePath, double zoomLevel)
        {
            //const string URL = "https://www.rong-chang.com/easyspeak/es/school01.htm";
            //const string URL = "http://root/texttospeech/index.html";
            //const string URL = "localfolder://cefsharp/home.html";
            //const string URL = "http://hook/base.js";
            //const string URL = "https://www.eslfast.com/";
            const string URL = "https://dictionary.cambridge.org/";

            var browserSettings = new BrowserSettings();
            //Reduce rendering speed to one frame per second so it's easier to take screen shots
            browserSettings.WindowlessFrameRate = 1;
            var requestContextSettings = new RequestContextSettings { CachePath = cachePath };
            // RequestContext can be shared between browser instances and allows for custom settings
            // e.g. CachePath
            using (var requestContext = new RequestContext(requestContextSettings))
            using (var browser = new ChromiumWebBrowser(URL, browserSettings, requestContext))
            {
                if (zoomLevel > 1)
                {
                    browser.FrameLoadStart += (s, argsi) =>
                    {
                        var b = (ChromiumWebBrowser)s;
                        if (argsi.Frame.IsMain)
                        {
                            b.SetZoomLevel(zoomLevel);
                        }
                    };
                }
                await LoadPageAsync(browser);
                //Check preferences on the CEF UI Thread
                await Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    var preferences = requestContext.GetAllPreferences(true);
                    //Check do not track status
                    var doNotTrack = (bool)preferences["enable_do_not_track"];
                    Console.WriteLine("DoNotTrack:" + doNotTrack);
                });
                var onUi = Cef.CurrentlyOnThread(CefThreadIds.TID_UI);
                //// For Google.com pre-pupulate the search text box
                //await browser.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");
                //// Wait for the screenshot to be taken,
                //// if one exists ignore it, wait for a new one to make sure we have the most up to date

                //await LoadPageAsync(browser, "http://github.com");
                //browser.GetBrowser().MainFrame.ViewSource();

                string htm = browser.GetBrowser().MainFrame.GetSourceAsync().Result;
                Console.WriteLine(htm);

                //string message = "You did not enter a server name. Cancel this operation?";
                //string caption = "Error Detected in Input";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result;
                //// Displays the MessageBox.
                //result = MessageBox.Show(message, caption, buttons);
                ////Gets a wrapper around the underlying CefBrowser instance
                //var cefBrowser = browser.GetBrowser();
                //// Gets a warpper around the CefBrowserHost instance
                //// You can perform a lot of low level browser operations using this interface
                //var cefHost = cefBrowser.GetHost();

                ////You can call Invalidate to redraw/refresh the image
                //cefHost.Invalidate(PaintElementType.View);
                //// Wait for the screenshot to be taken.

            }
        }
        public static Task LoadPageAsync(IWebBrowser browser, string address = null)
        {
            //If using .Net 4.6 then use TaskCreationOptions.RunContinuationsAsynchronously
            //and switch to tcs.TrySetResult below - no need for the custom extension method
            var tcs = new TaskCompletionSource<bool>();
            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    //This is required when using a standard TaskCompletionSource
                    //Extension method found in the CefSharp.Internals namespace
                    tcs.TrySetResultAsync(true);
                }
            };
            browser.LoadingStateChanged += handler;
            if (!string.IsNullOrEmpty(address))
            {
                browser.Load(address);
            }
            return tcs.Task;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //////////////////Cef.Initialize();
            string message = "You did not enter a server name. Cancel this operation?";
            string caption = "Error Detected in Input";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            // Displays the MessageBox.
            result = MessageBox.Show(message, caption, buttons);
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }
    }
}