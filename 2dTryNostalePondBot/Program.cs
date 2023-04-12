using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AForge.Imaging;
using AForge.Imaging.Filters;

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

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
                                     IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);


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

            List<Object> obj = new List<Object>();
            obj.Add(hwnd);
            obj.Add(rect);
            Thread thread = new Thread(ThreadMethod);
            thread.Start(obj);

            while (true)
            {
                Pond(349, 370, 421, 431, "../../../leftPond.png", hwnd, "left");
                Pond(456, 429, 537, 490, "../../../downPond.png", hwnd, "down");
                Pond(488, 323, 577, 382, "../../../upPond.png", hwnd, "up");
                Pond(604, 378, 709, 434, "../../../rightPond.png", hwnd, "right");

                //GetMousePosition(rect);
                Thread.Sleep(50);
            }
        }
        else
        {
            Console.WriteLine("Window with title '" + targetWindowTitle + "' not found.");
        }
    }

    static void Pond(int left, int top, int width, int height, string reference, IntPtr hwnd, string direction)
    {
        int captureX = left; // X coordinate of the capture region
        int captureY = top; // Y coordinate of the capture region
        int captureWidth = width - left; // Width of the capture region
        int captureHeight = height - top; // Height of the capture region
        bool theSame = true;

        Bitmap imageReference = new Bitmap(reference);

        using (Graphics windowGraphics = Graphics.FromHwnd(hwnd))
        {
            // Get the device context (DC) of the window
            IntPtr hdcWindow = GetWindowDC(hwnd);

            // Create a bitmap to hold the captured fragment
            Bitmap bitmap = new Bitmap(captureWidth, captureHeight);

            // Create a graphics object for the bitmap
            using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
            {
                // Capture the fragment from the window
                BitBlt(bitmapGraphics.GetHdc(), 0, 0, bitmap.Width, bitmap.Height,
                        hdcWindow, captureX, captureY, 0x00CC0020); // Change the coordinates (100, 100) and the size (200x150) as needed
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

    static void Bonus(int left, int top, int width, int height, string reference, IntPtr hwnd, string direction)
    {
        int captureX = left; // X coordinate of the capture region
        int captureY = top; // Y coordinate of the capture region
        int captureWidth = width - left; // Width of the capture region
        int captureHeight = height - top; // Height of the capture region

        Bitmap imageReference = new Bitmap(reference);

        TemplateMatch[] matches;

        using (Graphics windowGraphics = Graphics.FromHwnd(hwnd))
        {
            // Get the device context (DC) of the window
            IntPtr hdcWindow = GetWindowDC(hwnd);

            // Create a bitmap to hold the captured fragment
            Bitmap bitmap = new Bitmap(captureWidth, captureHeight);

            // Create a graphics object for the bitmap
            using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
            {
                // Capture the fragment from the window
                BitBlt(bitmapGraphics.GetHdc(), 0, 0, bitmap.Width, bitmap.Height,
                        hdcWindow, captureX, captureY, 0x00CC0020); // Change the coordinates (100, 100) and the size (200x150) as needed
            }
            // Convert the images to grayscale
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayLarge = grayFilter.Apply(bitmap);
            Bitmap graySmall = grayFilter.Apply(imageReference);

            // Instantiate the template matching class
            ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.9f);

            // Find the smaller image within the larger image
            matches = templateMatching.ProcessImage(grayLarge, graySmall);


            if (matches.Length > 0)
            {
                Console.WriteLine("Bonus");
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
            /*using (Bitmap bitmap = new Bitmap(captureWidth, captureHeight, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(captureX, captureY, 0, 0, new Size(captureWidth, captureHeight), CopyPixelOperation.SourceCopy);
                }
                /*string fileName = "screenshot.png"; // File name of the screenshot
                bitmap.Save(fileName, ImageFormat.Png);
                Console.WriteLine("Screenshot saved as: " + fileName);

                // Convert the images to grayscale
                Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayLarge = grayFilter.Apply(bitmap);
                Bitmap graySmall = grayFilter.Apply(imageReference);

                // Instantiate the template matching class
                ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.9f);

                // Find the smaller image within the larger image
                matches = templateMatching.ProcessImage(grayLarge, graySmall);


                if (matches.Length > 0)
                {
                    Console.WriteLine("Bonus");
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
            }*/
        }
    }
    static void ThreadMethod(Object obj)
    {
        List<Object> objects = (List<Object>)obj;
        IntPtr hwnd = (IntPtr)objects[0];
        while (true)
        {
            Bonus(327, 374, 759, 462, "../../../leftBonus.png", hwnd, "left");
            Bonus(327, 374, 759, 462, "../../../downBonus.png", hwnd, "down");
            Bonus(327, 374, 759, 462, "../../../upBonus.png", hwnd, "up");
            Bonus(327, 374, 759, 462, "../../../rightBonus.png", hwnd, "right");
            Thread.Sleep(50);
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


