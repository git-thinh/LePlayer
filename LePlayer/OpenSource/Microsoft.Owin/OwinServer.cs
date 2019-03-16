using Fleck;
using FluentValidation.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace System
{
    public class OwinServer
    {
        public static string PATH_ROOT = @"./";
        static AutoResetEvent _TERMINAL = new AutoResetEvent(false);

        public static void Start(string baseAddress = "http://127.0.0.1:12345/", string pathRoot = "./")
        {
            PATH_ROOT = pathRoot;

            Task.Factory.StartNew(() =>
            {
                WebApp.Start<Startup>(new StartOptions(baseAddress)
                {
                    ServerFactory = "Microsoft.Owin.Host.HttpListener"
                });

                var server = new WebSocketServer("ws://0.0.0.0:8181");
                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        //Console.WriteLine("Open!");
                        File.WriteAllText("log.txt", string.Empty);
                    };
                    socket.OnClose = () =>
                    {
                        //Console.WriteLine("Close!");
                    };
                    socket.OnMessage = message =>
                    {
                        //socket.Send(message);
                        Console.WriteLine();
                        Console.WriteLine(message);
                        Console.WriteLine();
                        using (StreamWriter w = File.AppendText("log.txt"))
                        {
                            w.WriteLine(message);
                        }
                    };
                });

                _TERMINAL.WaitOne();
            });
        }

        public static void Stop()
        {
            _TERMINAL.Set();
        }
    }
      
    class Startup
    {
        public AppFunc MyMiddleWare(AppFunc next)
        {
            AppFunc appFunc = async (IDictionary<string, object> context) =>
            {
                var url = context["owin.RequestPath"] as string;
                if (url.Contains("test.html"))
                {
                    string UserAgent = string.Empty;
                    var requestHeaders = context["owin.RequestHeaders"] as IDictionary<string, string[]>;
                    if (requestHeaders != null && requestHeaders.ContainsKey("User-Agent"))
                        UserAgent = (requestHeaders["User-Agent"] as string[])[0];

                    //string root = File.ReadAllText("path.txt");
                    //string file = Path.Combine(root, @"home\js\interface\interface.js");
                    if (true)
                    {
                        string js = "console.log('LOGGGGGGGGGGGGGGGGGGGGGGGG')";

                        //// Do something with the incoming request:
                        var responseHeaders = context["owin.ResponseHeaders"] as IDictionary<string, string[]>;
                        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(js);
                        //responseHeaders.Add("Content-Type", new[] { "application/javascript" });
                        responseHeaders.Add("Content-Type", new[] { "text/plain" });
                        //responseHeaders["Content-Type"] = new string[] { "text/plain" };
                        responseHeaders["Content-Length"] = new string[] { responseBytes.Length.ToString(CultureInfo.InvariantCulture) };

                        var responseStream = context["owin.ResponseBody"] as Stream;
                        await responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }

                // Call the next Middleware in the chain:
                await next.Invoke(context);
            };
            return appFunc;
        }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.MessageHandlers.Add(new CustomHeaderHandler());

            FluentValidationModelValidatorProvider.Configure(config);
            //------------------------------------------------------------------------


            //Enable Cors
            app.UseCors(CorsOptions.AllowAll);

            var middleware = new Func<AppFunc, AppFunc>(MyMiddleWare);
            app.Use(middleware);

            //Disable cache
            //app.Use(typeof(DisableCacheMiddleWare));

            // A middleware Disable browser cache for all the request
            //app.Use((ctx, next) =>
            //{
            //    ctx.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            //    ctx.Response.Headers["Pragma"] = "no-cache";
            //    ctx.Response.Headers["Expires"] = "-1"; 
            //    return next();
            //});

            //====== Run WebApi by config ============
            app.UseWebApi(config);

            //var physicalFileSystem = new PhysicalFileSystem(@"./www");
            var physicalFileSystem = new PhysicalFileSystem(OwinServer.PATH_ROOT);
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                RequestPath = new PathString(""),
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
            };

            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            //options.DefaultFilesOptions.DefaultFileNames = new[]
            //{
            //    "index.html",
            //    "home.html",
            //}; 
            app.UseFileServer(options);
        }
    }

    public class CustomHeaderHandler : DelegatingHandler
    {
        //using System.Net.Http; using System.Threading;using System.Threading.Tasks;
        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            response.Headers.Add("CACHE-CONTROL", "NO-CACHE");
            return response;
        }
    }
}
