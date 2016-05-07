using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SkylarkWsp.DanmakuEngine
{
    public sealed partial class DanmakuPlayer : UserControl
    {
        List<Danmaku> danmakus;
        #region Events declarations
        public delegate void DanmakuExceptionRoutedEventHandler(object sender, DanmakuExceptionRoutedEventArgs e);

        public event RoutedEventHandler SeekCompleted;
        public event RoutedEventHandler VolumeChanged;
        public event RoutedEventHandler BufferingProgressChanged;
        public event RoutedEventHandler CurrentStateChanged;
        public event RoutedEventHandler DownloadProgressChanged;
        public event RoutedEventHandler MediaEnded;
        public event ExceptionRoutedEventHandler MediaFailed;
        public event RoutedEventHandler MediaOpened;
        public event RateChangedRoutedEventHandler RateChanged;
        public event RoutedEventHandler DanmakuLoadStart;
        public event RoutedEventHandler DanmakuLoadComplete;
        public event DanmakuExceptionRoutedEventHandler DanmakuLoadFailed;
        private void mediaEle_SeekCompleted(object sender, RoutedEventArgs e)
        {
            if (SeekCompleted != null)
            {
                SeekCompleted(this, e);
            }
            if(danmakus!=null)
            {
                foreach (var i in danmakus)
                {
                    i.IsShowed = false;
                }
                this.danmakuPres.Clear();
            }
        }

        private void mediaEle_VolumeChanged(object sender, RoutedEventArgs e)
        {
            if (VolumeChanged != null)
            {
                VolumeChanged(this, e);
            }
        }
        private void mediaEle_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            if(BufferingProgressChanged!=null)
            {
                BufferingProgressChanged(this, e);
            }
        }
        private void mediaEle_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if(CurrentStateChanged!=null)
            {
                CurrentStateChanged(this, e);
            }
        }
        private void mediaEle_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            if(DownloadProgressChanged!=null)
            {
                DownloadProgressChanged(this, e);
            }
        }
        private void mediaEle_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (MediaEnded != null)
                MediaEnded(this, e);
        }

        private void mediaEle_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (MediaFailed != null)
                MediaFailed(this, e);
        }

        private void mediaEle_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaOpened != null)
                MediaOpened(this, e);
        }
        private void mediaEle_RateChanged(object sender, RateChangedRoutedEventArgs e)
        {
            if (RateChanged != null)
                RateChanged(this, e);
        }
        #endregion
        public DanmakuPlayer()
        {
            this.InitializeComponent();            
        }
        /// <summary>
        /// Set the damaku source
        /// </summary>
        /// <param name="source"></param>
        public void SetDanmakuSource(ObservableCollection<Danmaku> source)
        {
            try
            {
                if (source != null)
                {
                    danmakus = source.OrderBy(o => o.Time).ToList();
                    this.mediaEle.Markers.Clear();
                    foreach (var i in source)
                    {
                        this.mediaEle.Markers.Add(new TimelineMarker() { Time = TimeSpan.FromSeconds(i.Time) });
                    }                    
                }
            }
            catch(Exception ex)
            {
                if (DanmakuLoadFailed != null)
                {
                    DanmakuLoadFailed(this, new DanmakuExceptionRoutedEventArgs(ex));
                }
            }
        }
        /// <summary>
        /// Load danmakus from the specified source
        /// </summary>
        /// <param name="danmakuId"></param>
        /// <param name="source"></param>
        public async void LoadDanmaku(string danmakuId,DanmakuSource source)
        {
            try
            {
                if (DanmakuLoadStart != null)
                {
                    DanmakuLoadStart(this, new RoutedEventArgs());
                }
                switch (source)
                {
                    case DanmakuSource.AcFun:
                        var accol = (await DanmakuParser.DanmakuParser.GetAcDanmaku(danmakuId)).DanmakuCollection;
                        SetDanmakuSource(accol);
                        if (DanmakuLoadComplete != null)
                        {
                            DanmakuLoadComplete(this, new RoutedEventArgs());
                        }
                        break;
                    case DanmakuSource.Bilibili:
                        var bilicol = (await DanmakuParser.DanmakuParser.GetBiliDanmaku(danmakuId)).DanmakuCollection;
                        SetDanmakuSource(bilicol);
                        if (DanmakuLoadComplete != null)
                        {
                            DanmakuLoadComplete(this, new RoutedEventArgs());
                        }
                        break;
                    default:
                        if (DanmakuLoadFailed != null)
                        {
                            DanmakuLoadFailed(this, new DanmakuExceptionRoutedEventArgs("Unkown source"));
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                if (DanmakuLoadFailed != null)
                {
                    DanmakuLoadFailed(this, new DanmakuExceptionRoutedEventArgs(ex));
                }
            }
        }
        public void Play()
        {
            this.mediaEle.Play();
            this.danmakuPres.Resume();
        }
        public void Stop()
        {
            this.mediaEle.Stop();
        }
        public void Pause()
        {
            this.mediaEle.Pause();
            this.danmakuPres.Pause();
        }
        public void WriteLine(string msg)
        {
            this.logger.WriteLine(msg);
        }
        /// <summary>
        /// Set the danmakus count limit
        /// </summary>
        /// <param name="count"></param>
        public void SetLimit(int count)
        {
            this.danmakuPres.SetLimit(count);
        }
        private void play_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.mediaEle.Play();      
        }

        private async void mediaEle_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            try
            {
                if (this.danmakus != null)
                {
                    var res = this.danmakus.Where(d => ((int)d.Time == (int)(e.Marker.Time.TotalSeconds) && d.IsShowed == false));
                    this.danmakuPres.AddDanmaku(res.FirstOrDefault());
                    this.danmakus.Where(d => ((int)d.Time == (int)(e.Marker.Time.TotalSeconds) && d.IsShowed == false)).OrderBy(o => o.Time).FirstOrDefault().IsShowed = true;
                    await Task.Delay(100);
                }
            }
            catch(Exception ex)
            {
                if (DanmakuLoadFailed != null)
                {
                    DanmakuLoadFailed(this, new DanmakuExceptionRoutedEventArgs(ex));
                }
            }            
        }
        #region DependencyProperty declarations
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set
            {
                SetValue(SourceProperty, value);
                this.mediaEle.Source = value;
            }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(DanmakuEngine.DanmakuPlayer), new PropertyMetadata(null));



        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set
            {
                SetValue(VolumeProperty, value);
                this.mediaEle.Volume = Volume;
            }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(DanmakuPlayer), new PropertyMetadata(1));



        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set
            {
                SetValue(IsMutedProperty, value);
                this.mediaEle.IsMuted = IsMuted;
            }
        }

        // Using a DependencyProperty as the backing store for IsMuted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(DanmakuPlayer), new PropertyMetadata(false));



        public bool ShowLogger
        {
            get { return (bool)GetValue(ShowLoggerProperty); }
            set
            {
                if(value==false)
                {
                    this.logger.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.logger.Visibility = Visibility.Visible;
                }
                SetValue(ShowLoggerProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ShowLogger.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLoggerProperty =
            DependencyProperty.Register("ShowLogger", typeof(bool), typeof(DanmakuPlayer), new PropertyMetadata(true));



        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
                mediaEle.Position = Position;
            }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimeSpan), typeof(DanmakuPlayer), new PropertyMetadata(new TimeSpan(0,0,10)));


        public Duration NaturalDuration
        {
            get { return mediaEle.NaturalDuration; } 
        }
        //todo: implement more properties and events
        #endregion
    }
    public class DanmakuExceptionRoutedEventArgs: RoutedEventArgs
    {
        public DanmakuExceptionRoutedEventArgs(string message)
        {
            this.Ex = new Exception(message);
            this.Message = message;
        }
        public DanmakuExceptionRoutedEventArgs(Exception ex)
        {
            this.Ex = ex;
            this.Message = ex.Message;
        }
        public string Message { get; set; }
        public Exception Ex { get; set; }
    }
}
