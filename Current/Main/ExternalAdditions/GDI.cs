﻿using System;
using System.Drawing;
using TranslationHelper.ExternalAdditions;

namespace TranslationHelper
{
    public static class GDI
    {
        private const UInt32 SRCCOPY = 0x00CC0020;

        public static void CopyGraphics(Graphics g, Rectangle bounds, Graphics bufferedGraphics, Point p)
        {
            if (bufferedGraphics == null || g == null)
            {
                return;
            }

            IntPtr hdc1 = g.GetHdc();
            IntPtr hdc2 = bufferedGraphics.GetHdc();

            NativeMethods.BitBlt(hdc1, bounds.X, bounds.Y,
                bounds.Width, bounds.Height, hdc2, p.X, p.Y, SRCCOPY);

            g.ReleaseHdc(hdc1);
            bufferedGraphics.ReleaseHdc(hdc2);
        }
    }
}
