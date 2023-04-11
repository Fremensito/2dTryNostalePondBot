using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    static void Main()
    {
        string targetWindowTitle = "Nostale"; // Target window title

        IntPtr hwnd = FindWindow(null, targetWindowTitle);
        if (hwnd != IntPtr.Zero)
        {
            RECT rect;
            GetWindowRect(hwnd, out rect);

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            Console.WriteLine("Window Position:");
            Console.WriteLine("Left: " + rect.Left);
            Console.WriteLine("Top: " + rect.Top);
            Console.WriteLine("Width: " + windowWidth);
            Console.WriteLine("Height: " + windowHeight);

            while (true)
            {
                Pond(rect, 349, 370, 421, 431, "../../../leftPond.png", hwnd, "left");
                Pond(rect, 456, 429, 537, 490, "../../../downPond.png", hwnd, "down");
                Pond(rect, 488, 323, 577, 382, "../../../upPond.png", hwnd, "up");
                Pond(rect, 604, 378, 709, 434, "../../../rightPond.png", hwnd, "right");
                //GetMousePosition(rect);
                Thread.Sleep(50);
            }
        }
        else
        {
            Console.WriteLine("Window with title '" + targetWindowTitle + "' not found.");
        }
    }

    static void Pond(RECT rect, int left, int top, int width, int height, string reference, IntPtr hwnd, string direction)
    {
        int captureX = rect.Left + left; // X coordinate of the capture region
        int captureY = rect.Top + top; // Y coordinate of the capture region
        int captureWidth = width - left; // Width of the capture region
        int captureHeight = height - top; // Height of the capture region
        bool theSame = true;

        Bitmap imageReference = new Bitmap(reference);

        using (Bitmap bitmap = new Bitmap(captureWidth, captureHeight, PixelFormat.Format32bppArgb))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(captureX, captureY, 0, 0, new Size(captureWidth, captureHeight), CopyPixelOperation.SourceCopy);
            }

            /*string fileName = "screenshot.png"; // File name of the screenshot
            bitmap.Save(fileName, ImageFormat.Png);
            Console.WriteLine("Screenshot saved as: " + fileName);*/

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel1 = bitmap.GetPixel(x, y);
                    Color pixel2 = imageReference.GetPixel(x, y);

                    if (pixel1 != pixel2)
                    {
                        theSame = false;
                    }
                }
            
            }
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
    static void GetMousePosition(RECT rect)
    {
        POINT mousePosition;
        if (GetCursorPos(out mousePosition))
        {
            Console.Write("Mouse Position:");
            Console.Write(" X: " + (mousePosition.X - rect.Left) + " ");
            Console.WriteLine("Y: " + (mousePosition.Y - rect.Top));
        }
    }
}


