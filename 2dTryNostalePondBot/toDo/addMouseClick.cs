using System;
using System.Runtime.InteropServices;

class Program
{
    // Import Windows API functions
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    static void Main()
    {
        // Find the handle of the target window
        IntPtr hWnd = FindWindow(null, "Target Window Title"); // Replace "Target Window Title" with the title of the window you want to send a mouse click to

        // Check if the window handle is valid
        if (hWnd != IntPtr.Zero)
        {
            // Specify the coordinates where you want to simulate the mouse click
            int x = 100; // Replace with the desired x-coordinate
            int y = 200; // Replace with the desired y-coordinate

            // Calculate the lParam value for the mouse click message
            int lParam = (y << 16) + x;

            // Send a left mouse button down message
            SendMessage(hWnd, 0x201, 1, lParam); // 0x201 is the WM_LBUTTONDOWN message

            // Send a left mouse button up message
            SendMessage(hWnd, 0x202, 0, lParam); // 0x202 is the WM_LBUTTONUP message
        }
        else
        {
            Console.WriteLine("Window not found.");
        }
    }
}
