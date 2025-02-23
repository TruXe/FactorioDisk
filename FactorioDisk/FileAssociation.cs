using Microsoft.Win32;
using System;
using System.Net;
using System.Windows.Forms;

public class FileAssociation
{
    public static void RegisterFcdFileAssociation()
    {
        // Define the extension, ProgID, description, and paths
        string extension = ".fcd";
        string progId = "MyApp.FCD";
        string description = "My FCD File";
        string iconPath = @"C:\Path\To\MyIcon.ico"; // Replace with your icon path
        string appPath = Application.ExecutablePath; // Your application's path

        using(WebClient webClient = new WebClient())
        {
            try
            {
                webClient.DownloadFile( "", "" );
            }
            catch (Exception ex)
            {
                MessageBox.Show( "Error downloading files: " + ex.Message );
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

            MessageBox.Show( "File association registered successfully for .fcd files!" );
        }
        catch (Exception ex)
        {
            MessageBox.Show( "Error registering file association: " + ex.Message );
        }
    }
}
