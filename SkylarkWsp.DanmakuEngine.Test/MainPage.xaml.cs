using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkylarkWsp.DanmakuEngine.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
            this.player.DanmakuLoadComplete += Player_DanmakuLoadComplete;
            this.player.DanmakuLoadFailed += Player_DanmakuLoadFailed;
            this.player.DanmakuLoadStart += Player_DanmakuLoadStart;
        }

        private void Player_DanmakuLoadStart(object sender, RoutedEventArgs e)
        {
            this.player.WriteLine("Loading");
        }

        private void Player_DanmakuLoadFailed(object sender, DanmakuExceptionRoutedEventArgs e)
        {
            this.player.WriteLine(e.Message);
        }

        private void Player_DanmakuLoadComplete(object sender, RoutedEventArgs e)
        {
            this.player.WriteLine("Load complete");
        }

        private void load_Btn_Click(object sender, RoutedEventArgs e)
        {
            //this.player.SetLimit(20);  set limit
            this.player.LoadDanmaku("7318843", DanmakuSource.Bilibili);
            //this.testPres.AddDanmaku(new Danmaku() { Content = "aaaa", Size = 25, ForegroundColor = Colors.White, Speed = 5000, Mode = DanmakuMode.Scroll  });  add a danmaku manually
        }

        private void pause_Copy1_Click(object sender, RoutedEventArgs e)
        {
            this.player.Pause();
        }

        private void play_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.player.Play();
        }
        bool _updating;
        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if(!_updating)
            {
                _updating = true;
                TimeSpan duration = player.NaturalDuration.TimeSpan;
                player.Position = new TimeSpan(0, 0, (int)(duration.TotalSeconds * slider.Value));
                _updating = false;
            }
        }

        private void logger_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.player.ShowLogger = !(this.player.ShowLogger);           
        }
    }
}
