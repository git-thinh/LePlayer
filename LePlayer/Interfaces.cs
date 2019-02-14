﻿using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public interface IContext
    {
    }

    public interface ICrawler
    {
        IContext Context { get; }
        string URL_NEXT { get; set; }
        void Go(string url);
    }

    public interface IForm
    {
        FORM_TYPE FormType { get; }
        IContext Context { get; }

        void RaiseEventMenuBrowser(int menuCode);
        void CloseForm();
        void ClearLog();

        string URL_NEXT { get; set; }
        void Go(string url);
    }

    public interface IForm_Browser
    {
    }
    public interface IForm_Dictionary
    {
    }
    public interface IForm_MediaVideo
    {
    }
    public interface IForm_MediaShortcut
    {
    }
    public interface IForm_MediaList
    {
    }

    public enum FORM_TYPE
    {
        DICTIONARY = 0,
        BROWSER = 1,

        MEDIA_VIDEO = 2,
        MEDIA_LIST = 3,
        MEDIA_SHORTCUT = 4,
        MEDIA_YOUTUBE_SEARCH = 5,
        MEDIA_YOUTUBE_VIDEO = 6,

        GITTER_CHAT = 10,
    }

    public enum FORM_STYLE
    {
        TEXT_COLOR_BLACK___BG_COLOR_WHITE = 0,
        TEXT_COLOR_WHITE___BG_COLOR_BLACK = 1,
    }

    public enum ICON_TYPE
    {
        arrow_alt = 0xf124,
        close = 0xf191,
        minus = 0xf28e,
        play = 0xf2be,
        stop = 0xf323,
        pause = 0xf2ad,
        search = 0xf2eb,
        history = 0xf236,
        plus = 0xf2c2,
        menu = 0xf131,
        arrow_right = 0xf112,
        arrow_left = 0xf111,
        random = 0xf2d0,
        volum_down = 0xf373,
        volum_up = 0xf375,
        star = 0xf318,
        repeat = 0xf2d9,//f2dc
        cc = 0xf1a9,
        setting = 0xf19a,
        trash = 0xf34c,
    }
}
