using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SkylarkWsp.DanmakuEngine
{
    public class Logger:StackPanel
    {
        /// <summary>
        /// Write the specified message to the screen logger
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            this.Children.Add(new TextBlock() { Text = message, Foreground = new SolidColorBrush(Colors.White),VerticalAlignment=Windows.UI.Xaml.VerticalAlignment.Bottom });
        }
        /// <summary>
        /// Clear the screen logger
        /// </summary>
        public void Clear()
        {
            this.Children.Clear();
        }
        /// <summary>
        /// Write the specified message to the debug logger
        /// </summary>
        /// <param name="message"></param>
        public void WriteToDebug(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
