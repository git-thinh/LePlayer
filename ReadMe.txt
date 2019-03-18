


https://github.com/filipw/WebApi.ActionInjector
https://github.com/tugberkugurlu/DotNetDoodle.Owin.Dependencies
https://www.strathweb.com/2014/07/dependency-injection-directly-actions-asp-net-web-api/


https://github.com/simpleinjector/SimpleInjector
https://github.com/simpleinjector/SimpleInjector/issues/138
https://weblog.west-wind.com/posts/2016/dec/12/loading-net-assemblies-out-of-seperate-folders
https://stackoverflow.com/questions/10988749/self-hosting-webapi-application-referencing-controller-from-different-assembly/24434526#24434526
https://stackoverflow.com/questions/48652339/making-owin-use-assemblyresolved-assemblies

https://exceptionnotfound.net/custom-validation-in-asp-net-web-api-with-fluentvalidation/
https://github.com/JeremySkinner/FluentValidation/issues/157
https://fluentvalidation.net/start
https://stackoverflow.com/questions/9984144/what-is-the-correct-way-to-register-fluentvalidation-with-simpleinjector
https://stackoverflow.com/questions/10940988/how-to-configure-simple-injector-ioc-to-use-ravendb
https://simpleinjector.readthedocs.io/en/latest/windowsformsintegration.html





https://groups.google.com/forum/#!topic/cefglue/EhskGZ9OndY


Re: [CefGlue] Re: Async Javascript to Native C# with Params ???
Dịch thông báo sang Tiếng Việt  

Thanks Andrew! Based on your post I created the following proof of concept app. I found the performance OK. I passed a 5MB image, which took 1sec to transform into base64 string in the c# code, and plus 0,5sec to pass to the js code. It'd be great to have typed arrays support for this reason. If you don't pass around a lot of data, then you're more than fine I guess. How's your experiences?

My CefApp:
using System.Drawing;

using System.Drawing.Imaging;

using System.IO;

using System.Threading;

using System;



namespace Xilium.CefGlue.Client

{

    internal sealed class DemoApp : CefApp

    {

        private CefRenderProcessHandler renderProcessHandler = new DemoRenderProcessHandler();



        protected override CefRenderProcessHandler GetRenderProcessHandler()

        {

            return renderProcessHandler;

        }

    }



    internal class DemoRenderProcessHandler : CefRenderProcessHandler

    {

        MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();



        protected override void OnWebKitInitialized()

        {

            base.OnWebKitInitialized();



            var nativeFunction = @"nativeImplementation = function(onSuccess) {

                native function MyNativeFunction(onSuccess);

                return MyNativeFunction(onSuccess);

            };";



            CefRuntime.RegisterExtension("myExtension", nativeFunction, myCefV8Handler);

        }

    }



    internal class MyCustomCefV8Handler : CefV8Handler

    {

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue,

            out string exception)

        {

            //Debugger.Launch();



            var context = CefV8Context.GetCurrentContext();

            var taskRunner = CefTaskRunner.GetForCurrentThread();

            var callback = arguments[0];

            new Thread(() =>

            {

                //Sleep a bit: to test whether the app remains responsive

                Thread.Sleep(3000);

                taskRunner.PostTask(new CefCallbackTask(context, callback));

            }).Start();



            returnValue = CefV8Value.CreateBool(true);

            exception = null;

            return true;

        }

    }



    internal class CefCallbackTask : CefTask

    {

        private readonly CefV8Context context;

        private readonly CefV8Value callback;



        public CefCallbackTask(CefV8Context context, CefV8Value callback)

        {

            this.context = context;

            this.callback = callback;

        }



        protected override void Execute()

        {

            var callbackArguments = CreateCallbackArguments();

            callback.ExecuteFunctionWithContext(context, null, callbackArguments);

        }



        private CefV8Value[] CreateCallbackArguments()

        {

            var imageInBase64EncodedString = LoadImage(@"C:\hamb.jpg");



            context.Enter();



            var imageV8String = CefV8Value.CreateString(imageInBase64EncodedString);

            var featureV8Object = CefV8Value.CreateObject(null);

            var listOfFeaturesV8Array = CefV8Value.CreateArray(1);



            featureV8Object.SetValue("name", CefV8Value.CreateString("V8"), CefV8PropertyAttribute.None);

            featureV8Object.SetValue("isEnabled", CefV8Value.CreateInt(0), CefV8PropertyAttribute.None);

            featureV8Object.SetValue("isFromJSCode", CefV8Value.CreateBool(false), CefV8PropertyAttribute.None);



            listOfFeaturesV8Array.SetValue(0, featureV8Object);



            context.Exit();



            return new [] { listOfFeaturesV8Array, imageV8String };

        }



        private string LoadImage(string fileName)

        {

            using (var memoryStream = new MemoryStream())

            {

                var image = Bitmap.FromFile(fileName);

                image.Save(memoryStream, ImageFormat.Png);

                byte[] imageBytes = memoryStream.ToArray();

                return Convert.ToBase64String(imageBytes);

            }

        }

    }

}

The html file, that I loaded at the first place:

<!DOCTYPE html>



<html lang="en" xmlns="http://www.w3.org/1999/xhtml">

    <head>

        <meta charset="utf-8" />

        <title>C# and JS experiments</title>

        <script src="index.js"></script>

    </head>

    <body>

        <h1>C# and JS are best friends</h1>

        <div id="features">

        </div>

        

        <div id="image">

        </div>

    </body>

</html>

The javascript code:

function Browser() {

}



Browser.prototype.ListAllFeatures = function (onSuccess) {

    return nativeImplementation(onSuccess);

}



function App(browser) {

    this.browser = browser;

}



App.prototype.Run = function () {

    var beforeRun = new Date().getTime();



    this.browser.ListAllFeatures(function(features, imageInBase64EncodedString) {

        var feautersListString = '';

        for (var i = 0; i < features.length; i++) {

            var f = features[i];

            feautersListString += ('<p>' + 'Name: ' + f.name + ', is enabled: ' + f.isEnabled + ', is called from js code: ' + f.isFromJSCode + '</p>');

        }



        feautersListString += '<p> The image: </p>';

        feautersListString += '<p>' + imageInBase64EncodedString + '</p>';



        document.getElementById("features").innerHTML = feautersListString;

        var afterRun = new Date().getTime();



        document.getElementById("image").innerHTML = '<img src="data:image/png;base64,' + imageInBase64EncodedString + '" />';

        var afterLoadedImage = new Date().getTime();



        console.log("ELAPSED TIME - INSIDE LIST ALL FEATURES: " + (afterRun - beforeRun));

        console.log("ELAPSED TIME - IMAGE IS LOADED TO THE <img> TAG: " + (afterLoadedImage - beforeRun));

    });

}



window.onload = function () {

    var browser = new Browser();

    var application = new App(browser);



    //Lets measure

    var beforeRun = new Date().getTime();

    application.Run();

    var afterRun = new Date().getTime();

    console.log("ELAPSED TIME - INSIDE ONLOAD: " + (afterRun - beforeRun));