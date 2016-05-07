using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SkylarkWsp.DanmakuEngine
{
    public sealed partial class DanmakuPresenter : UserControl
    {
        DanmakuManager dm;
        bool _loaded=false;
        public event EventHandler OnPause;
        public event EventHandler OnResume;
        public DanmakuPresenter()
        {
            this.InitializeComponent();
        }        
        public void AddScrollableDanmaku(string text, Color foreground, double size,bool shadow,int speed=5000)
        {
            if (_loaded == true)
            {
                if (dm == null)
                {
                    dm = new DanmakuManager(danmakuPres);
                }

                dm.AddScrollableDanmaku(text, foreground, size,speed,true);
            }
            else
            {
                throw new InvalidOperationException("Danmaku presenter haven't loaded yet.");
            }
        }
        public void AddTopMotinlessDanmaku(string text, Color foreground, double size,bool shadow)
        {
            if (_loaded == true)
            {
                if (dm == null)
                {
                    dm = new DanmakuManager(danmakuPres);
                }

                dm.AddTopMotionlessDanmaku(text, foreground, size,true);
            }
            else
            {
                throw new InvalidOperationException("Danmaku presenter haven't loaded yet.");
            }
        }
        public void AddBottomMotinlessDanmaku(string text, Color foreground, double size,bool shadow)
        {
            if (_loaded == true)
            {
                if (dm == null)
                {
                    dm = new DanmakuManager(danmakuPres);
                }

                dm.AddBottomMotionlessDanmaku(text, foreground, size,true);
            }
            else
            {
                throw new InvalidOperationException("Danmaku presenter haven't loaded yet.");
            }
        }
        public void AddDanmaku(Danmaku danmaku)
        {
            if (_loaded == true)
            {
                if (dm == null)
                {
                    dm = new DanmakuManager(danmakuPres);
                }
                switch (danmaku.Mode)
                {
                    case DanmakuMode.Top:
                        AddTopMotinlessDanmaku(danmaku.Content, danmaku.ForegroundColor, danmaku.Size,true);
                        break;
                    case DanmakuMode.Bottom:
                        AddBottomMotinlessDanmaku(danmaku.Content, danmaku.ForegroundColor, danmaku.Size,true);
                        break;
                    case DanmakuMode.Scroll:
                        AddScrollableDanmaku(danmaku.Content, danmaku.ForegroundColor, danmaku.Size,true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                throw new InvalidOperationException("Danmaku presenter haven't loaded yet.");
            }
        }
        public void SetLimit(int count)
        {
            if (_loaded == true)
            {
                if (dm == null)
                {
                    dm = new DanmakuManager(danmakuPres);
                }

                dm.SetLimit(count);
            }
            else
            {
                throw new InvalidOperationException("Danmaku presenter haven't loaded yet.");
            }
        }
        private void danmakuPres_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true; 
        }
        public void SwitchPresenterVisibility()
        {
            if (dm != null)
            {
                dm.SwitchVisibility();
            }
        }
        public void Pause()
        {
            if (dm != null)
            {
                dm.Pause();
                if(OnPause!=null)
                    OnPause(this, new PauseResumeEventArgs(true));
            }
        }
        public void Resume()
        {
            if (dm != null)
            {
                dm.Resume();
                if (OnResume != null)
                    OnResume(this, new PauseResumeEventArgs(false));
            }
        }
        public void Clear()
        {
            if(dm!=null)
            {
                dm.Clear();
            }
        }
        private class PauseResumeEventArgs : EventArgs
        {
            public bool IsPaused { get; private set; }
            public PauseResumeEventArgs(bool ispaused)
            {
                IsPaused = ispaused;
            }
        }
    }
}
