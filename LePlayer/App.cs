﻿using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Windows.Forms;

namespace LePlayer
{
    static class App
    {
        //static App()
        //{
        //    AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
        //    {
        //        Assembly asm = null;
        //        string comName = ev.Name.Split(',')[0];

        //        string resourceName = @"dll\" + comName + ".dll";
        //        var assembly = Assembly.GetExecutingAssembly();
        //        resourceName = typeof(App).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
        //        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        //            //using (Stream stream = File.OpenRead("dll/" + comName + ".dll"))
        //            {
        //            if (stream == null)
        //            {
        //                    //Debug.WriteLine(resourceName);
        //                }
        //            else
        //            {
        //                byte[] buffer = new byte[stream.Length];
        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    int read;
        //                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        //                        ms.Write(buffer, 0, read);
        //                    buffer = ms.ToArray();
        //                }
        //                asm = Assembly.Load(buffer);
        //            }
        //        }
        //        return asm;
        //    };
        //}

        /// <summary>
        /// Helps us to ensure only one instance runs at a time.
        /// </summary>
        //static Mutex mutex = new Mutex(true, "{0fbc294c-f089-4009-9b1a-ab757739483f}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (mutex.WaitOne(TimeSpan.Zero, true))
            //{
            //    try
            //    {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //OwinServer.Start();
            Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: "http://*:10101/");

            Application.Run(new MainContext());
            CefSharp.Cef.Shutdown();

            //    }
            //    finally
            //    {
            //        mutex.ReleaseMutex();
            //    }
            //}
        }

        public class ValuesController : ApiController
        {
            public IEnumerable<string> Get()
            {
                return new List<string> { "ASP.NET", "Docker", "Windows Containers" };
            }
        }

        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                HttpConfiguration config = new HttpConfiguration();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
                app.UseWebApi(config);
            }
        }













    }
}
