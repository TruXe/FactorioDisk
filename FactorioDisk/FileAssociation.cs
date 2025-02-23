using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class FileAssociation
{
    public static void RegisterFcdFileAssociation()
    {
        // Define the extension, ProgID, description, and paths
        string extension = ".fcd";
        string progId = "FactorioDisk.FCD";
        string description = "FactorioDisk file icon";
        string iconPath = @"C:\blueprint.ico"; // Replace with your icon path
        string appPath = Application.ExecutablePath; // Your application's path

        using(WebClient webClient = new WebClient())
        {
            try
            {
                webClient.DownloadFile( "https://raw.githubusercontent.com/TruXe/FactorioDisk/refs/heads/master/FactorioDisk/icons/Blueprint.ico", iconPath );
            }
            catch (Exception ex)
            {
                MessageBox.Show( "Error downloading icons: " + ex.Message );
            }
        }


        try
        {
            // Associate the extension with the ProgID (this affects only .fcd files)
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey( extension ))
            {
                key.SetValue( "", progId );
            }

            // Create the ProgID key and set its properties
            using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey( progId ))
            {
                progIdKey.SetValue( "", description );

                // Set the default icon
                using (RegistryKey defaultIconKey = progIdKey.CreateSubKey( "DefaultIcon" ))
                {
                    defaultIconKey.SetValue( "", $"{iconPath},0" );
                }

                // Set the command to open the file
                using (RegistryKey shellKey = progIdKey.CreateSubKey( @"Shell\Open\Command" ))
                {
                    shellKey.SetValue( "", $"\"{appPath}\" \"%1\"" );
                }
            }

           // MessageBox.Show( "File association registered successfully for .fcd files!" );
        }
        catch (Exception ex)
        {
            return;
            //MessageBox.Show( "Error registering file association: " + ex.Message );
        }
    }

    // Constants for SHChangeNotify
    private const uint SHCNE_ASSOCCHANGED = 0x08000000;
    private const uint SHCNF_IDLIST = 0x0000;

    [DllImport( "shell32.dll" )]
    private static extern void SHChangeNotify( uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2 );

    public static void RefreshIcons()
    {
        // Notify the shell that file associations have changed
        SHChangeNotify( SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero );
    }
}
