using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using Newtonsoft.Json;

namespace LePlayer
{
    public struct CallbackResponseStruct
    {
        public string Response;

        public CallbackResponseStruct(string response)
        {
            Response = response;
        }
    }

    public class BoundObject
    {
        public string storeGetDirves()
        {
            string s = "{}";
            try
            {
                s = JsonConvert.SerializeObject(DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => new { name = d.Name }));
            }
            catch { }
            return s;
        }
        public void showMessage(string msg)
        {
            MessageBox.Show(msg);
        }
        public void TestCallback(IJavascriptCallback javascriptCallback)
        {
            const int taskDelay = 1500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);
                using (javascriptCallback)
                {
                    //NOTE: Classes are not supported, simple structs are
                    var response = new CallbackResponseStruct("This callback from C# was delayed " + taskDelay + "ms");
                    await javascriptCallback.ExecuteAsync(response);
                }
            });
        }
    }

}