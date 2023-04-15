using System.Runtime.InteropServices;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;
class Program
{
    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;
    public const uint MOUSEEVENTF_MOVE = 0x0001;


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
    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    static void Main()
    {
        bool test = true;
        bool playing = false;
        int coupons = 0;
        int points = 0;

        while (test)
        {
            Console.Write("How much points: ");
            points = Convert.ToInt32(Console.ReadLine());
            Console.Write("How many coupons: ");
            coupons = Convert.ToInt32(Console.ReadLine());
            if (coupons >= 0 && points % 100 == 0)
                test = false;
            else
                Console.WriteLine("Incorrect input, try agian (only values for lv 5 reward)");
        }


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
                if (!playing)
                {
                    Console.WriteLine("Points: " + points);
                    Console.WriteLine("Coupons: " + coupons);
                    playing = true;
                    Click(hwnd, 433, 453);
                    Thread.Sleep(1000);
                    Click(hwnd, 405, 564);
                    Thread.Sleep(1000);
                    Click(hwnd, 518, 576);
                }
                if (!Pond(445, 213, 462, 235, "../../../score.png", hwnd, "void"))
                {
                    if (GetTheScore(rect, hwnd) != "Color [A=255, R=1, G=1, B=1]")
                    {
                        Pond(349, 370, 421, 431, "../../../leftPond.png", hwnd, "left");
                        Pond(456, 429, 537, 490, "../../../downPond.png", hwnd, "down");
                        Pond(488, 323, 577, 382, "../../../upPond.png", hwnd, "up");
                        Pond(604, 378, 709, 434, "../../../rightPond.png", hwnd, "right");
                    }
                    else
                    {
                        Bonus(327, 374, 759, 462, "../../../leftBonus.png", hwnd, "left");
                        Bonus(327, 374, 759, 462, "../../../downBonus.png", hwnd, "down");
                        Bonus(327, 374, 759, 462, "../../../upBonus.png", hwnd, "up");
                        Bonus(327, 374, 759, 462, "../../../rightBonus.png", hwnd, "right");
                    }
                    //TakePicture(445, 213, 462, 235, rect, hwnd);
                }
                else
                {
                    Thread.Sleep(15000);
                    Click(hwnd, 641, 464);
                    Thread.Sleep(1000);
                    Click(hwnd, 646, 464);
                    points -= 100;
                    if (points != 0)
                    {
                        Thread.Sleep(1000);
                        Click(hwnd, 452, 491);
                        Thread.Sleep(1000);
                        Click(hwnd, 518, 576);
                    }
                    Console.WriteLine("Points: " + points);
                    Console.WriteLine("Coupons: " + coupons);
                }
                if (points == 0 && coupons != 0)
                {
                    Thread.Sleep(1000);
                    SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x1B, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, (IntPtr)0x1B, IntPtr.Zero);
                    Thread.Sleep(1000);
                    Click(hwnd, 568, 493);
                    Thread.Sleep(1000);
                    SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x54, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, (IntPtr)0x54, IntPtr.Zero);
                    Thread.Sleep(1000);
                    SendMessage(hwnd, WM_KEYDOWN, (IntPtr)0x0D, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, (IntPtr)0x0D, IntPtr.Zero);
                    points += 500;
                    coupons--;
                    playing = false;
                }
                if (points == 0 && coupons == 0)
                    break;
                Thread.Sleep(100);
            }
        }
        else
        {
            Console.WriteLine("Window with title '" + targetWindowTitle + "' not found.");
        }
    }

    static void Hover(IntPtr hwnd, int x, int y)
    {
        while (true)
        {
            IntPtr lParam = new IntPtr((y << 16) | x);
            SendMessage(hwnd, 0x0200, IntPtr.Zero, lParam);
            Thread.Sleep(50);
        }
    }
    static void Click(IntPtr hwnd, int x, int y)
    {
        y = y - 16;
        IntPtr lParam = new IntPtr((uint)y << 16| (uint)x);
        IntPtr wParam = new IntPtr(MOUSEEVENTF_MOVE);

        SendMessage(hwnd, 0x200, IntPtr.Zero, lParam);

        SendMessage(hwnd, 0x201, new IntPtr(0x0001), lParam); // 0x201 is the WM_LBUTTONDOWN message

        //Send a left mouse button up message
        SendMessage(hwnd, 0x202, new IntPtr(0x0001), lParam); // 0x202 is the WM_LBUTTONUP message

    }
    static bool Pond(int left, int top, int width, int height, string reference, IntPtr hwnd, string direction)
    {
        int captureX = left; // X coordinate of the capture region
        int captureY = top; // Y coordinate of the capture region
        int captureWidth = width - left; // Width of the capture region
        int captureHeight = height - top; // Height of the capture region
        bool theSame = false;

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

                ReleaseDC(hwnd, hdcWindow);
            }

            /*string fileName = "screenshot.png"; // File name of the screenshot
            bitmap.Save(fileName, ImageFormat.Png);
            Console.WriteLine("Screenshot saved as: " + fileName);*/
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayLarge = grayFilter.Apply(bitmap);
            Bitmap graySmall = grayFilter.Apply(imageReference);

            // Instantiate the template matching class
            ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.999f);

            // Find the smaller image within the larger image
            matches = templateMatching.ProcessImage(grayLarge, graySmall);

            if (matches.Length > 0)
                theSame = true;

            if (theSame && direction != "void")
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
        return theSame;
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
                ReleaseDC(hwnd, hdcWindow);
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
    static string GetTheScore(RECT rect, IntPtr hwnd)
    {
        /*POINT mousePosition;
        if (GetCursorPos(out mousePosition))
        {
            Console.Write("Mouse Position:");
            Console.Write(" X: " + (mousePosition.X - rect.Left) + " ");
            Console.WriteLine("Y: " + (mousePosition.Y - rect.Top));
        }*/

        using (Graphics windowGraphics = Graphics.FromHwnd(hwnd))
        {
            // Get the device context (DC) of the window
            IntPtr hdcWindow = GetWindowDC(hwnd);

            // Create a bitmap to hold the captured fragment
            Bitmap bitmap = new Bitmap(1, 1);

            // Create a graphics object for the bitmap
            using (Graphics bitmapGraphics = Graphics.FromImage(bitmap))
            {
                // Capture the fragment from the window
                BitBlt(bitmapGraphics.GetHdc(), 0, 0, bitmap.Width, bitmap.Height,
                        hdcWindow, 735, 407, 0x00CC0020); // Change the coordinates (100, 100) and the size (200x150) as needs
                ReleaseDC(hwnd, hdcWindow);
            }
            Color pixel = bitmap.GetPixel(0, 0);
            return pixel.ToString();
        }
    }

    /*static void TakePicture(int left, int top, int width, int height, RECT rect, IntPtr hwnd)
    {
        int captureX = left; // X coordinate of the capture region
        int captureY = top; // Y coordinate of the capture region
        int captureWidth = width - left; // Width of the capture region
        int captureHeight = height - top; // Height of the capture region

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
                        hdcWindow, captureX, captureY, 0x00CC0020); // Change the coordinates (100, 100) and the size (200x150) as needs
                ReleaseDC(hwnd, hdcWindow);
            }

            string fileName = "screenshot.png"; // File name of the screenshot
            bitmap.Save(fileName, ImageFormat.Png);
            Console.WriteLine("Screenshot saved as: " + fileName);
        }
    }*/
}


