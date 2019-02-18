using CefSharp;
using CefSharp.Handler;
using CefSharp.Internals;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LePlayer
{
    public class ContextUrl : IContextUrl
    {
        public ContextUrl(IContext context, string url, ContextOption option)
        {
            this.Option = option;
            this.Context = context;
            this.Url = url;
        }
        public IContext Context { get; private set; }
        public ContextOption Option { get; set; }
        public string Url { get; set; }
    }

    public class CrawlerHtml
    {
        static RequestContext requestContext;
        static BrowserSettings browserSettings;
        static ChromiumWebBrowser browser;
        static IContextUrl contextUrl;

        public static async void getSourceAsync(string url, ContextOption option)
        {
            if (browser == null) return;
            contextUrl.Url = url;
            contextUrl.Option = option;
            await LoadPageAsync(browser, url);
        }

        public static async void Start(IContext context_)
        {
            contextUrl = new ContextUrl(context_, "about:blank", null);
            browserSettings = new BrowserSettings()
            {
                WindowlessFrameRate = 1,
                Javascript = CefState.Disabled,
                ApplicationCache = CefState.Disabled
            };
            var requestContextSettings = new RequestContextSettings { CachePath = "cache-offscreen" };
            requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser("about:blank", browserSettings, requestContext);

            browser.RequestHandler = new CrawlerRequestHandler(contextUrl);
            browser.ResourceHandlerFactory = new CrawlerRequestResourceHandlerFactory(contextUrl);

            await LoadPageAsync(browser);
        }

        static Task LoadPageAsync(IWebBrowser browser, string address = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
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
    }

    public class CrawlerRequestHandler : DefaultRequestHandler
    {
        readonly IContextUrl Crawler;
        public CrawlerRequestHandler(IContextUrl crawler) : base() { this.Crawler = crawler; }

        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if (frame.IsMain) this.Crawler.Url = request.Url;
            return false;
        }

        private Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();
        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var url = request.Url;
            if (url == this.Crawler.Url)
            {
                var dataFilter = new MemoryStreamResponseFilter(null);
                responseDictionary.Add(request.Identifier, dataFilter);
                return dataFilter;
            }
            return null;
        }

        public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            var url = request.Url;
            if (url == this.Crawler.Url)
            {
                MemoryStreamResponseFilter filter;
                if (responseDictionary.TryGetValue(request.Identifier, out filter))
                {
                    var data = filter.Data;
                    var dataLength = filter.Data.Length;
                    browser.StopLoad();
                    var html = Encoding.UTF8.GetString(data);
                    //Console.WriteLine(html);
                    this.Crawler.Context.crawler_callbackResultStore(this.Crawler.Url, html);
                }
            }
        }
    }

    public class CrawlerRequestResourceHandlerFactory : IResourceHandlerFactory
    {
        readonly IContextUrl Crawler;
        public CrawlerRequestResourceHandlerFactory(IContextUrl crawler) : base() { this.Crawler = crawler; }
        bool IResourceHandlerFactory.HasHandlers { get { return true; } }
        IResourceHandler IResourceHandlerFactory.GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            string url = request.Url;
            //Console.WriteLine(Environment.NewLine + "!!!!-> " + url);
            if (url == this.Crawler.Url) return null;
            return new RequestResourceCanceler();
        }
    }
}
