using System;
using System.Runtime.InteropServices;
class MessageBox
{
    public enum Options { OK }
    public static void Show(string Content, string Title = null, Options Options = Options.OK)
    {
        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWind, string text, string caption, int options);
        MessageBox(IntPtr.Zero, Content, Title, Convert.ToInt32(Options));
    }
}
