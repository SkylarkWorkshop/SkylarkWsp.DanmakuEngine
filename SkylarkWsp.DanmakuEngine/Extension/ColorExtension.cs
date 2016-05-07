using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SkylarkWsp.DanmakuEngine.Extension
{
    public static class ColorExtension
    {
        public static Color ToColor(this string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = Convert.ToByte((v >> 24) & 255),
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)
            };
        }
        public static Color ToColor(this long colorName)
        {
            return new Color()
            {
                A = Convert.ToByte((colorName >> 24) & 255),
                R = Convert.ToByte((colorName >> 16) & 255),
                G = Convert.ToByte((colorName >> 8) & 255),
                B = Convert.ToByte((colorName >> 0) & 255)
            };
        }
    }
}
