using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageSizeReducer
{
    class MyService
    {
        public void Start()
        {
            // write code here that runs when the Windows Service starts up.  
            //File.AppendAllText(@"c:\temp\MyService.txt", String.Format("{0} started{1}", DateTime.Now, Environment.NewLine));
            Watcher.imagenesReingreso();
        }
        public void Stop()
        {
            // write code here that runs when the Windows Service stops.  
            //File.AppendAllText(@"c:\temp\MyService.txt", String.Format("{0} stopped{1}", DateTime.Now, Environment.NewLine));
        }

        static class Constants
        {
            public const string FOLDERPATH = @"C:\Reingreso\";
            public const int IMAGEQUALITY = 50;
            public static readonly ReadOnlyCollection<string> ALLOWEDIMAGETYPES = new ReadOnlyCollection<string>(new[] { ".JPG", ".JPE", ".JPEG", ".BMP", ".GIF", ".PNG" });
        }

        public class Watcher
        {
            public static void imagenesReingreso()
            {
                try
                {
                    var watcher = new FileSystemWatcher(@"C:\Reingreso");

                    watcher.NotifyFilter = NotifyFilters.DirectoryName
                                        | NotifyFilters.FileName
                                        | NotifyFilters.Size;

                    watcher.Created += OnCreated;

                    watcher.Filter = "*.*";
                    watcher.IncludeSubdirectories = true;
                    watcher.EnableRaisingEvents = true;
                    Console.WriteLine("Esta registrando los movimientos de la carpeta de reingreso por este servicio.");
                }
                catch (System.Exception)
                {

                }
            }

            private static void OnCreated(object sender, FileSystemEventArgs e)
            {
                string value = $"Created: {e.FullPath}";
                Console.WriteLine(value + " NAME " + e.Name);
                //if (Regex.IsMatch(e.FullPath, @"\.png|\.jpg", RegexOptions.IgnoreCase)){
                if (!Regex.IsMatch(e.Name, @"^F1", RegexOptions.IgnoreCase))
                {
                    //here Process the image file 
                    Console.WriteLine("es imagen " + Path.GetExtension(e.FullPath));
                    File.AppendAllText(@"c:\temp\MyService.txt", String.Format("{0} started{1}", DateTime.Now+ " - es imagen " + Path.GetExtension(e.FullPath), Environment.NewLine));
                    //Conv(Path.GetExtension(e.FullPath), e.FullPath, "50", @"C:\Reingreso");
                    RunImageOptimizer(e.FullPath);
                }
            }

            static void RunImageOptimizer(string file)
            {
                try{
                    Thread.Sleep(10000);
                    //var dateTimeNow = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
                    //var fileList = Directory.GetFiles(Constants.FOLDERPATH).ToList();
                    //var destinationPath = new StringBuilder(Constants.FOLDERPATH + @"\" + dateTimeNow + @"\").ToString();

                    var fileExtension = Path.GetExtension(file).ToUpperInvariant().ToString();
                    if (Constants.ALLOWEDIMAGETYPES.Contains(fileExtension))
                    {
                        // process image
                        //string fileSavePath = new StringBuilder(destinationPath + file.Replace(Constants.FOLDERPATH, "").Replace("WhatsApp Image ", "").Replace("PHOTO-", "")).ToString();
                        string fileSavePath = new StringBuilder(Constants.FOLDERPATH + "F1" + file.Replace(Constants.FOLDERPATH, "").Replace("WhatsApp Image ", "").Replace("PHOTO-", "")).ToString();
                        CompressImage(Image.FromFile(file), fileExtension, Constants.IMAGEQUALITY, fileSavePath);

                        File.Delete(file);
                    }
                }catch(Exception e){
                    File.AppendAllText(@"c:\temp\MyService.txt", String.Format("{0} started{1}", DateTime.Now + " - error " + e, Environment.NewLine));
                }
                //Directory.CreateDirectory(destinationPath);
                /*
                foreach (var file in fileList)
                {
                    var fileExtension = Path.GetExtension(file).ToUpperInvariant().ToString();
                    if (Constants.ALLOWEDIMAGETYPES.Contains(fileExtension))
                    {
                        // process image
                        //string fileSavePath = new StringBuilder(destinationPath + file.Replace(Constants.FOLDERPATH, "").Replace("WhatsApp Image ", "").Replace("PHOTO-", "")).ToString();
                        string fileSavePath = new StringBuilder(Constants.FOLDERPATH + "F" + file.Replace(Constants.FOLDERPATH, "").Replace("WhatsApp Image ", "").Replace("PHOTO-", "")).ToString();
                        CompressImage(Image.FromFile(file), fileExtension, Constants.IMAGEQUALITY, fileSavePath);

                        File.Delete(file);
                    }
                }
                */
                }

            private static void CompressImage(Image sourceImage, string fileExtension, int imageQuality, string savePath)
            {
                try
                {
                    //Create an ImageCodecInfo-object for the codec information
                    ImageCodecInfo jpegCodec = null;
                    //Set quality factor for compression
                    EncoderParameter imageQualitysParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, imageQuality);
                    //List all avaible codecs (system wide)
                    ImageCodecInfo[] allCodecs = ImageCodecInfo.GetImageEncoders();
                    EncoderParameters codecParameter = new EncoderParameters(1);
                    codecParameter.Param[0] = imageQualitysParameter;

                    //Find and choose JPEG codec
                    for (int i = 0; i < allCodecs.Length; i++)
                    {
                        if (allCodecs[i].MimeType == "image/jpeg")
                        {
                            jpegCodec = allCodecs[i];
                            break;
                        }
                    }

                    //Save compressed image
                    sourceImage.Save(savePath, jpegCodec, codecParameter);

                    sourceImage.Dispose();
                    //Force garbage collection.
                    GC.Collect();
                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception e)
                {
                    File.AppendAllText(@"c:\temp\MyService.txt", String.Format("{0} started{1}", DateTime.Now + " - error " + e, Environment.NewLine));
                    //throw e;
                }
            }


        }
        }

    
}
