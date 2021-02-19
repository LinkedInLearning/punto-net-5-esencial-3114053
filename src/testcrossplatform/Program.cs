using System;

namespace testcrossplatform
{
    class Program
    {
        static void Main(string[] args)
        {
            if (OperatingSystem.IsWindows()){
                var x = Microsoft.Win32.Registry.CurrentUser.GetValue("Microsoft");
            }
            
        }
    }
}
