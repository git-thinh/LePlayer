using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IForm
    {
        IContext Context { get; }
        void RaiseEventMenuBrowser(int menuCode);
        void CloseForm();
        void ClearLog();

        string URL_NEXT { get; set; }
        void Go(string url);
    }

    public interface IContext
    {
    }
}
