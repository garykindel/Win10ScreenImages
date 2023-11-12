using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Win10ScreenImages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string wPath = (string) FindResource("ImageActualPath");
            this.Images.Children.Clear();
            if (Directory.Exists(wPath))
            {
                string[] files = Directory.GetFiles(wPath);
                foreach (string file in files) 
                { 
                    var wFileInfo = new FileInfo(file);
                    double fileSizeInKilobytes = wFileInfo.Length / 1024.0;
                    if (fileSizeInKilobytes>=100)
                    {
                        var imageControl = new System.Windows.Controls.Image();
                        imageControl.Width = 300; // Set the desired width
                        imageControl.Height = 300; // Set the desired height
                        byte[] byteArray = File.ReadAllBytes(file);
                        BitmapImage bitmapImage = ByteArrayToBitmapImage(byteArray);
                        imageControl.Source = bitmapImage;
                        this.Images.Children.Add(imageControl);
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid path");
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        private void SaveImageToFile(System.Windows.Controls.Image image, string filePath)
        {
            try
            {
                // Create a JpegBitmapEncoder
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                // Create a MemoryStream to store the encoded JPEG data
                using (MemoryStream stream = new MemoryStream())
                {
                    // Create a BitmapFrame from the BitmapImage
                    BitmapFrame bitmapFrame = BitmapFrame.Create((image.Source as BitmapImage));

                    // Add the BitmapFrame to the encoder
                    encoder.Frames.Add(bitmapFrame);

                    // Save the encoded JPEG data to the MemoryStream
                    encoder.Save(stream);

                    // Write the MemoryStream to a file
                    File.WriteAllBytes(filePath, stream.ToArray());
                }

                MessageBox.Show($"Image saved to {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string wPath = (string)FindResource("SaveFolderPath");
            DateTime currentDate = DateTime.Now;
            string newFileName = $"image_{currentDate:yyyyMMdd_HHmmss}";
            newFileName = newFileName + "_{0}.png";
            if (Directory.Exists(wPath))
            {                             
                Int32 wCount = 1;
                foreach (var child in this.Images.Children)
                {
                    if (child is System.Windows.Controls.Image)
                    {                                            
                        SaveImageToFile((child as System.Windows.Controls.Image), System.IO.Path.Combine(wPath, string.Format(newFileName, wCount)));
                    }
                    wCount++;
                }
            }
            else
            {
                MessageBox.Show("Invalid path");
            }

        }
    }
}