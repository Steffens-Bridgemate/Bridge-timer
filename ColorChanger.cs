using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace BridgeTimer
{
    public class ColorChanger
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public void ChangeLogoColor(Color newForeColor,Color newBackground,string fileName)
        {
            var baseFilename = Path.Combine(App.Path, App.BaseLogoFile);
            try
            {
                using (var bitmap = new Bitmap(baseFilename))
                {
                    var bg = Color.FromArgb(255, 255, 255);

                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            Color originalColor = bitmap.GetPixel(x, y);
                            Color changedColor;

                            if (originalColor == bg)
                            {
                                changedColor = newBackground;
                            }
                            else
                            {
                                changedColor = newForeColor;
                            }
                            bitmap.SetPixel(x, y, changedColor);
                        }
                    }

                    bitmap.Save(App.GetFullAppDataPath(fileName), System.Drawing.Imaging.ImageFormat.Png); // location of your new image
                }
            }
            catch ( Exception ex)
            {
                Logger.Error(ex);
                Logger.Error(baseFilename);
                throw;
            }

        }

        public static Color Convert(System.Windows.Media.Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }
    }
}
