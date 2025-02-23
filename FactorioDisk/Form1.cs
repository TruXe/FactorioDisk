using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioDisk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string DIR_BLUEPRINTS = "Blueprints";

        private void Form1_Load( object sender, EventArgs e )
        {
            FileAssociation.RegisterFcdFileAssociation();
            FileAssociation.RefreshIcons();
            string version = Globals.version;

            title_lbl.Text = "Factorio Disk" + $" v{version}" ;
            this.Text = "Factorio Disk" + $" v{version}";

            if(!File.Exists("setuped"))
            {
                File.Create( "setuped" ).Close();
                FileAssociation.RegisterFcdFileAssociation();
                FileAssociation.RefreshIcons();
            }

            if(!Directory.Exists(DIR_BLUEPRINTS))
            {
                Directory.CreateDirectory( DIR_BLUEPRINTS );
               // File.Create( Path.Combine( DIR_BLUEPRINTS, "Test.fcd" ) ).Close();
                //MessageBox.Show( "Blueprints directory created", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
                LoadBlueprints();
            }
            else
            {
                LoadBlueprints();
            }
        }

        private void LoadBlueprints()
        {
            // Build the full path to the "Blueprints" folder in your application's directory
            string blueprintsDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Blueprints" );

            // Check if the directory exists
            if (Directory.Exists( blueprintsDir ))
            {
                // Get all files with the .fcd extension
                string[] files = Directory.GetFiles( blueprintsDir, "*.fcd" );
                listBoxBlueprints.Items.Clear();

                // Add each file (just the file name) to the ListBox
                foreach (string file in files)
                {
                    listBoxBlueprints.Items.Add( Path.GetFileName( file ) );
                }
            }
            else
            {
                //MessageBox.Show( "Blueprints directory not found!" );
            }
        }

        private void button4_Click( object sender, EventArgs e )
        {
            LoadBlueprints();
        }

        private void listBoxBlueprints_SelectedIndexChanged( object sender, EventArgs e )
        {
            // Ensure that an item is selected
            if (listBoxBlueprints.SelectedIndex != -1)
            {
                // Get the selected file name
                string fileName = listBoxBlueprints.SelectedItem.ToString();
                // Build the full path to the selected file
                string filePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Blueprints", fileName );

                // Check if the file exists before trying to read it
                if (File.Exists( filePath ))
                {
                    // Read the file content and display it in the Multiline TextBox
                    Blueprint_txt.Text = File.ReadAllText( filePath );
                }
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            if(string.IsNullOrWhiteSpace( NewBluepirnt_txt.Text ))
            {
                MessageBox.Show( "Please enter a blueprint name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            else
            {

            
                string BlueprintName = NewBluepirnt_txt.Text + ".fcd";
            if(!File.Exists( Path.Combine( DIR_BLUEPRINTS, BlueprintName ) ))
            {
                File.Create( Path.Combine( DIR_BLUEPRINTS, BlueprintName ) ).Close();
                //MessageBox.Show( "Blueprint created", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
                LoadBlueprints();
            }
            else
            {
                MessageBox.Show( "Blueprint already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            }
        }

        private void button5_Click( object sender, EventArgs e )
        {
            if (listBoxBlueprints.SelectedIndex != -1)
            {
                // Get the selected file name
                string fileName = listBoxBlueprints.SelectedItem.ToString();
                // Build the full path to the selected file
                string filePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Blueprints", fileName );

                // Check if the file exists before attempting to save
                if (File.Exists( filePath ))
                {
                    try
                    {
                        // Write the content from Blueprint_txt into the file
                        File.WriteAllText( filePath, Blueprint_txt.Text );
                        //MessageBox.Show( "Blueprint saved successfully!" );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show( "Error saving blueprint: " + ex.Message );
                    }
                }
                else
                {
                    MessageBox.Show( "File not found!" );
                }
            }
            else
            {
                MessageBox.Show( "Please select a blueprint to save." );
            }
        }

        private void button7_Click( object sender, EventArgs e )
        {
            if(listBoxBlueprints.SelectedIndex != -1)
            {
                string fileName = listBoxBlueprints.SelectedItem.ToString();
                string filePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Blueprints", fileName );
                if (File.Exists( filePath ))
                {
                    File.Delete( filePath );
                    MessageBox.Show( "Blueprint deleted", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    LoadBlueprints();
                }
                else
                {
                    MessageBox.Show( "File not found!" );
                }
            }
            else
            {
                MessageBox.Show( "Please select a blueprint to delete." );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // Check if the TextBox contains any text
            if (!string.IsNullOrWhiteSpace( Blueprint_txt.Text ))
            {
                // Copy the text from Blueprint_txt to the Clipboard
                Clipboard.SetText( Blueprint_txt.Text );
                //MessageBox.Show( "Text copied to clipboard!" );
            }
            else
            {
                //MessageBox.Show( "There is no text to copy." );
            }
        }

        private void button3_Click( object sender, EventArgs e )
        {
            Environment.Exit( 0 );
        }

        private void button6_Click( object sender, EventArgs e )
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void panel1_MouseDown( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left)
            {
                // Start dragging: store initial cursor and form locations
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            if (dragging)
            {
                // Calculate the new form location based on the cursor movement
                Point diff = Point.Subtract( Cursor.Position, new Size( dragCursorPoint ) );
                this.Location = Point.Add( dragFormPoint, new Size( diff ) );
            }
        }

        private void panel1_MouseUp( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left)
            {
                // Stop dragging when the left mouse button is released
                dragging = false;
            }
        }
    }
}
