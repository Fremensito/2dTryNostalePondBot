using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace _2dTryNostalePondBot
{
    internal class FishDetector
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;

        private Bitmap reference;
        private Bitmap actual;
        private bool referenceCaught;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public void CheckPond(RECT rect, int left, int top, int width, int height, string reference, IntPtr hwnd, string direction)
        {
            int captureX = rect.Left + left; // X coordinate of the capture region
            int captureY = rect.Top + top; // Y coordinate of the capture region
            int captureWidth = width - left; // Width of the capture region
            int captureHeight = height - top; // Height of the capture region
            bool theSame = true;

            using (Bitmap bitmap = new Bitmap(captureWidth, captureHeight, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(captureX, captureY, 0, 0, new Size(captureWidth, captureHeight), CopyPixelOperation.SourceCopy);
                }

                actual = bitmap;
                if(reference != null)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            Color pixel1 = bitmap.GetPixel(x, y);
                            Color pixel2 = this.reference.GetPixel(x, y);

                            if (pixel1 != pixel2)
                            {
                                theSame = false;
                            }
                        }
                    }
                }
                this.reference = actual;

                if (theSame)
                {
                    if (theSame)
                    {
                        switch (direction)
                        {
                            case "left":
                                SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x25, IntPtr.Zero);
                                SendMessage(hwnd, WM_KEYUP, (IntPtr)0x25, IntPtr.Zero);
                                break;
                            case "down":
                                SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x28, IntPtr.Zero);
                                SendMessage(hwnd, WM_KEYUP, (IntPtr)0x28, IntPtr.Zero);
                                break;
                            case "up":
                                SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x26, IntPtr.Zero);
                                SendMessage(hwnd, WM_KEYUP, (IntPtr)0x26, IntPtr.Zero);
                                break;
                            case "right":
                                SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x27, IntPtr.Zero);
                                SendMessage(hwnd, WM_KEYUP, (IntPtr)0x27, IntPtr.Zero);
                                break;
                        }
                    }
                }
            }
        }
    }
}
