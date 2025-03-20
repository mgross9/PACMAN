using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
//using Reprise;

namespace PACMAN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //private IntPtr handle = IntPtr.Zero;

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    if (!CheckValidLicense())
        //    {
        //        Application.Current.Shutdown();
        //    }
        //}
        //protected override void OnExit(ExitEventArgs e)
        //{
        //    if (handle != IntPtr.Zero)
        //    {
        //        // Clean up the handle
        //        RLM.rlm_close(handle);
        //    }
        //    //do your things
        //    base.OnExit(e);
        //}
        //private bool CheckValidLicense()
        //{
        //    try
        //    {
        //        string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        //        handle = RLM.rlm_init(".", appPath, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("Unable Load licensing software:\n\n" + ex.Message, "PACMAN", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return false;
        //    }
        //    int stat = RLM.rlm_stat(handle);
        //    if (stat != 0)
        //    {
        //        System.Windows.MessageBox.Show("Unable Load licensing software:\n\nReturn Code: " + stat, "PACMAN", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return false;
        //    }
        //    else
        //    {

        //        // Check out a license
        //        IntPtr license = checkout(handle, "PACMAN", "1.0", 1);
        //        if (license != IntPtr.Zero)
        //        {
        //            return true;
        //        }
        //        else
        //        {
                    
        //            return false;
        //        }
        //    }
        //}

        //private static IntPtr checkout(IntPtr handle, String prod, String ver, int count)
        //{
        //    IntPtr license = RLM.rlm_checkout(handle, prod, ver, count);
        //    int stat = RLM.rlm_license_stat(license);
        //    if (stat != 0)
        //    {
        //        System.Windows.MessageBox.Show("Checkout of PACMAN failed: " +
        //        RLM.marshalToString(RLM.rlm_errstring(
        //         license, handle, new byte[RLM.RLM_ERRSTRING_MAX])), "PACMAN",  MessageBoxButton.OK, MessageBoxImage.Error);
        //        license = IntPtr.Zero;
        //    }
        //    return license;
        //}
    }
}
