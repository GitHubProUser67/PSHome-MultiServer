using MultiServerLibrary.Extension;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RemoteControl
{
    public static class Utils
    {
        public static string OpenExecutableFile(this TextBox textBox)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Please select an exe file.",
                InitialDirectory = Program.currentDir,
                Filter = "Executable files (*.exe)|*.exe"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;

                if (Encoding.ASCII.GetString(FileSystemUtils.TryReadFileChunck(fileName, 2, FileShare.ReadWrite)) == "MZ")
                {
                    textBox.Text = fileName;
                    return fileName;
                }
                else
                    MessageBox.Show("File is not a valid Executable!", "Error");
            }

            return null;
        }

        public static string UpdateConfigurationFile(this TextBox textBox, string exePath)
        {
            if (string.IsNullOrEmpty(exePath))
                return null;

            string result = Path.GetDirectoryName(exePath) + $"/static/{Path.GetFileNameWithoutExtension(exePath)}.json";

            textBox.Text = result;

            return result;
        }

        public static void ShowNoProcessMessageBox()
        {
            MessageBox.Show("No Process running for this server!", "Error");
        }
    }
}
