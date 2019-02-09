﻿using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace System
{
    public class IconControl : Label
    {
        public int FontSize { set; get; }
        private static byte[] bufferFont;
        public IconControl(Color normalColor, IconType type = IconType.play, int fontSize = 24)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitialiseFont();

            FontSize = fontSize;
            Width = fontSize;
            Height = fontSize;

            IconType = type;
            NormalColor = normalColor;
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        /// <summary>
        /// Loads the icon font from the resources.
        /// </summary>
        private static void InitialiseFont()
        {
            if (bufferFont == null || bufferFont.Length == 0)
            {
                bufferFont = LePlayer.Properties.Resources.line_awesome;

                try
                {
                    unsafe
                    {
                        fixed (byte* pFontData = bufferFont)
                        {
                            uint dummy = 0;
                            Fonts.AddMemoryFont((IntPtr)pFontData, bufferFont.Length);
                            AddFontMemResourceEx((IntPtr)pFontData, (uint)bufferFont.Length, IntPtr.Zero, ref dummy);
                        }
                    }
                }
                catch
                {
                    // log?
                }
            }
        }

        private Brush IconBrush { get; set; } // brush currently in use
        public Color _normalColor;
        public Color NormalColor
        {
            get { return _normalColor; }
            set
            {
                _normalColor = value;
                IconBrush = new SolidBrush(_normalColor);
                Invalidate();
            }
        }

        private string IconChar { get; set; }
        private IconType _iconType = IconType.play;
        public IconType IconType
        {
            get { return _iconType; }
            set
            {
                _iconType = value;
                // see http://fortawesome.github.io/Font-Awesome/cheatsheet/ for a list of hex values
                IconChar = char.ConvertFromUtf32((int)value);
                Invalidate();
            }
        }

        private Font IconFont { get; set; }

        private static readonly PrivateFontCollection Fonts = new PrivateFontCollection();
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            // Set best quality
            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            //graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // is the font ready to go?
            if (IconFont == null)
                IconFont = new Font(Fonts.Families[0], FontSize, GraphicsUnit.Point);

            //// center icon
            int left = -1 * (int)Math.Round(FontSize * 0.39766233f, MidpointRounding.AwayFromZero); //-26
            int top = -1 * (int)Math.Round(FontSize * 0.17584415f, MidpointRounding.AwayFromZero); //-12

            // Fill in Background (for efficiency only the area that has been clipped)
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)),
            ////e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
            //0, 0, FontSize, FontSize);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), 0, 0, FontSize, FontSize);
            //graphics.FillRectangle(Brushes.Red, new Rectangle(10, 10, 200, 200));

            // Draw string to screen.
            graphics.DrawString(IconChar, IconFont, IconBrush, new PointF(left, top));

            base.OnPaint(e);

            if (Focused)
            {
                var rc = this.ClientRectangle;
                rc.Inflate(-2, -2);
                ControlPaint.DrawFocusRectangle(e.Graphics, rc);
            }
        }
    }
}
