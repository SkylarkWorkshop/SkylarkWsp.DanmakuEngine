//Partly referenced to https://github.com/wspl/DanmakuChi-Client-CSharp/blob/master/DanmakuChi/Danmaku.cs (original author:wspl)
using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Graphics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Effects;
using System.Threading;
using System.Diagnostics;

namespace SkylarkWsp.DanmakuEngine
{
    public sealed class DanmakuManager
    {
        private Grid _container;
        private int _lineHeight = 36;
        private int _paddingTop = 4;
        private bool[] _isOccupied;
        private bool[] _isOccupiedTop;
        private bool[] _isOccupiedBottom;
        private bool _isPaused = false;
        private int _lines;
        private int _limitCount = -1;
        private List<Storyboard> _currentStoryboards = new List<Storyboard>();
        /// <summary>
        /// Clear all the lines and mark them as unoccupied
        /// </summary>
        private void ClearAllLine()
        {
            for (int i = 0; i < _lines; i++)
            {
                _isOccupied[i] = false;
            }
        }
        /// <summary>
        /// Get the number of available line and mark this line as occupied
        /// </summary>
        /// <returns></returns>
        private int GetAvailableLine()
        {
            for (int i = 0; i < _lines; i++)
            {
                if (!_isOccupied[i])
                {
                    _isOccupied[i] = true;
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Get the number of available line and mark this line as occupied
        /// </summary>
        /// <returns></returns>
        private int GetAvailableLineForTopDanmaku()
        {
            for (int i = 0; i < _lines; i++)
            {
                if (!_isOccupiedTop[i])
                {
                    _isOccupiedTop[i] = true;
                    return i;
                }
            }
            if(_isOccupiedTop[0]==true&&_isOccupiedTop[_lines-1] ==true)
            {
                _isOccupiedTop = new bool[_lines];
                return GetAvailableLineForTopDanmaku();
            }
            return -1;
        }
        /// <summary>
        /// Get the number of available line and mark this line as occupied
        /// </summary>
        /// <returns></returns>
        private int GetAvailableLineForBottomDanmaku()
        {
            for (int i = 0; i < _lines; i++)
            {
                if (!_isOccupiedBottom[i])
                {
                    _isOccupiedBottom[i] = true;
                    return i;
                }
            }
            if (_isOccupiedBottom[0] == true && _isOccupiedBottom[_lines - 1] == true)
            {
                _isOccupiedBottom = new bool[_lines];
                return GetAvailableLineForBottomDanmaku();
            }
            return -1;
        }
        /// <summary>
        /// Get the Y coordinate in the specified line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int GetY(int line)
        {
            return (line * _lineHeight) + _paddingTop;
        }
        /// <summary>
        /// Init Danmaku manager and use the specified grid as Danmaku container
        /// </summary>
        /// <param name="container"></param>
        public DanmakuManager(Grid container)
        {
            this._container = container;
            this._lines = (int)(_container.RenderSize.Height / _lineHeight) - 1;
            _isOccupied = new bool[_lines];
            _isOccupiedTop = new bool[_lines];
            _isOccupiedBottom = new bool[_lines];
            this._container.SizeChanged += _container_SizeChanged;
        }

        private void _container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this._lines = (int)(_container.RenderSize.Height / _lineHeight) - 1;
            _isOccupied = new bool[_lines];
            _isOccupiedTop = new bool[_lines];
            _isOccupiedBottom = new bool[_lines];
        }

        /// <summary>
        /// Add a scrollable danmaku
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        /// <param name="size"></param>
        public void AddScrollableDanmaku(string text, Color foreground, double size, bool shadow)
        {
            var line = GetAvailableLine();
            if (line == -1)
            {
                ClearAllLine();
                line = GetAvailableLine();
            }
            Grid item = new Grid() { Margin = new Thickness(0, GetY(line), 0, 0) };
            TextBlock tbk = new TextBlock() { Margin = new Thickness(0, 0, 0, 0), Text = text, Foreground = new SolidColorBrush(foreground), FontSize = GetDefaultFontSize(size) };
            if (shadow)
            {
                TextBlock shadowTbk = new TextBlock() { Margin = new Thickness(1, 1, 0, 0), Text = text, Foreground = new SolidColorBrush(Colors.Black), FontSize = tbk.FontSize };
                item.Children.Add(shadowTbk);
                item.Children.Add(tbk);
            }
            else
            {
                item.Children.Add(tbk);
            }
            if (_limitCount!=-1&&_container.Children.Count>=_limitCount)
            {
                return;
            }
            else
            {
                _container.Children.Add(item);
            }
            var da = new DoubleAnimation();
            da.From = this._container.RenderSize.Width;
            da.To = -item.DesiredSize.Width - 1600;
            da.SpeedRatio = item.DesiredSize.Width > 80 ?
                (.05 * (item.DesiredSize.Width / 1500 + 1)) :
                (.1 * ((100 - item.DesiredSize.Width) / 100 + 1));
            TranslateTransform trans = new TranslateTransform();
            item.RenderTransform = trans;
            //remove the textblock from the grid after presenting it
            da.Completed += ((sender, e) =>
            {
                if (_container.Children.Contains(item))
                {
                    _container.Children.Remove(item);
                }
            });
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += ((sener, e) =>
            {
                Point relativePoint = item.TransformToVisual(_container).TransformPoint(new Point(0, 0));
                if (relativePoint.X < _container.ActualWidth - item.DesiredSize.Width - 50)
                {
                    timer.Stop();
                    if (line < _lines && line != -1)
                    {
                        _isOccupied[line] = false;
                    }
                }
            });
            timer.Start();
            Storyboard.SetTarget(da, trans);
            Storyboard.SetTargetProperty(da, "(TranslateTransform.X)");
            Storyboard sb = new Storyboard();
            _currentStoryboards.Add(sb);
            sb.Children.Add(da);
            sb.Begin();
            if (_isPaused)
            {
                sb.Pause();
            }
            sb.Completed += (sender, e) => 
            {
                if (_currentStoryboards.Contains(sb))
                {
                    _currentStoryboards.Remove(sb);
                }
            };
        }
        /// <summary>
        /// Add a scrollable danmaku
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        /// <param name="size"></param>
        public void AddScrollableDanmaku(string text, Color foreground, double size, int speed,bool shadow)
        {
            var line = GetAvailableLine();
            if (line == -1)
            {
                ClearAllLine();
                line = GetAvailableLine();
            }
            Grid item = new Grid() { Margin = new Thickness(0, GetY(line), 0, 0) };
            TextBlock tbk = new TextBlock() { Margin = new Thickness(0,0,0,0), Text = text, Foreground = new SolidColorBrush(foreground), FontSize = GetDefaultFontSize(size) };
            if (shadow)
            {
                TextBlock shadowTbk = new TextBlock() { Margin = new Thickness(1, 1, 0, 0),Text=text,Foreground= new SolidColorBrush(Colors.Black),FontSize= tbk.FontSize };
                item.Children.Add(shadowTbk);
                item.Children.Add(tbk);
            }
            else
            {
                item.Children.Add(tbk);
            }

            if (_limitCount != -1 && _container.Children.Count >= _limitCount)
            {
                return;
            }
            else
            {
                _container.Children.Add(item);
            }
            var da = new DoubleAnimation();
            da.From = this._container.RenderSize.Width;
            da.To = -item.DesiredSize.Width - 1600;
            da.SpeedRatio = item.DesiredSize.Width > 80 ?
                (.05 * (item.DesiredSize.Width / 1500 + 1)) :
                (.1 * ((100 - item.DesiredSize.Width) / speed + 1));
            TranslateTransform trans = new TranslateTransform();
            item.RenderTransform = trans;
            //remove the textblock from the grid after presenting it
            da.Completed += ((sender, e) =>
            {
                if (_container.Children.Contains(item))
                {
                    _container.Children.Remove(item);
                }
            });
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += ((sener, e) =>
            {
                Point relativePoint = item.TransformToVisual(_container).TransformPoint(new Point(0, 0));
                if (relativePoint.X < _container.ActualWidth - item.DesiredSize.Width - 50)
                {
                    timer.Stop();
                    if (line < _lines && line != -1)
                    {
                        _isOccupied[line] = false;
                    }
                }
            });
            timer.Start();
            Storyboard.SetTarget(da, trans);
            Storyboard.SetTargetProperty(da, "(TranslateTransform.X)");
            Storyboard sb = new Storyboard();
            _currentStoryboards.Add(sb);
            sb.Children.Add(da);
            sb.Begin();
            if (_isPaused)
            {
                sb.Pause();
            }
            sb.Completed += (sender, e) => 
            {
                if (_currentStoryboards.Contains(sb))
                {
                    _currentStoryboards.Remove(sb);
                }
            };
            
        }
        /// <summary>
        /// Add a motionless danmaku on the top of the presenter
        /// </summary>
        /// <param name="text"></param>
        /// <param name="line"></param>
        /// <param name="foreground"></param>
        /// <param name="size"></param>
        public async void AddTopMotionlessDanmaku(string text, Color foreground, double size,bool shadow)
        {
            var line = GetAvailableLineForTopDanmaku();
            Grid item = new Grid() { Margin = new Thickness(0, GetY(line), 0, 0),HorizontalAlignment=HorizontalAlignment.Center };
            TextBlock tbk = new TextBlock() { Margin = new Thickness(0, 0, 0, 0), Text = text, Foreground = new SolidColorBrush(foreground), FontSize = GetDefaultFontSize(size) };
            if (shadow)
            {
                TextBlock shadowTbk = new TextBlock() { Margin = new Thickness(1, 1, 0, 0), Text = text, Foreground = new SolidColorBrush(Colors.Black), FontSize = tbk.FontSize };
                item.Children.Add(shadowTbk);
                item.Children.Add(tbk);
            }
            else
            {
                item.Children.Add(tbk);
            }

            if (_limitCount != -1 && _container.Children.Count >= _limitCount)
            {
                return;
            }
            else
            {
                _container.Children.Add(item);
            }
            await Task.Delay(5000);
            if(_isPaused)
            {
                while(_isPaused)
                {
                    await Task.Delay(5000);
                }
            }
            if(_container.Children.Contains(item))
            {
                _container.Children.Remove(item);
            }
            if (line < _lines && line != -1)
            {
                _isOccupiedTop[line] = false;
            }

        }
        /// <summary>
        /// Add a motionless danmaku on the foot of the presenter
        /// </summary>
        /// <param name="text"></param>
        /// <param name="line"></param>
        /// <param name="foreground"></param>
        /// <param name="size"></param>
        public async void AddBottomMotionlessDanmaku(string text, Color foreground, double size,bool shadow)
        {
            var line = GetAvailableLineForBottomDanmaku();
            Grid item = new Grid() { Margin = new Thickness(0, 0, 0, GetY(line)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom };
            TextBlock tbk = new TextBlock() { Margin = new Thickness(0, 0, 0, 0), Text = text, Foreground = new SolidColorBrush(foreground), FontSize = GetDefaultFontSize(size) };
            if (shadow)
            {
                TextBlock shadowTbk = new TextBlock() { Margin = new Thickness(1, 1, 0, 0), Text = text, Foreground = new SolidColorBrush(Colors.Black), FontSize = tbk.FontSize };
                item.Children.Add(shadowTbk);
                item.Children.Add(tbk);
            }
            else
            {
                item.Children.Add(tbk);
            }

            if (_limitCount != -1 && _container.Children.Count >= _limitCount)
            {
                return;
            }
            else
            {
                _container.Children.Add(item);
            }
            await Task.Delay(5000);
            if (_isPaused)
            {
                while (_isPaused)
                {
                    await Task.Delay(5000);
                }
            }
            if (_container.Children.Contains(item))
            {
                _container.Children.Remove(item);
            }
            if (line < _lines && line != -1)
            {
                _isOccupiedBottom[line] = false;
            }
        }
        /// <summary>
        /// Add a danmaku
        /// </summary>
        /// <param name="text"></param>
        /// <param name="line"></param>
        /// <param name="foreground"></param>
        /// <param name="size"></param>
        /// <param name="mode"></param>
        public void AddDanmaku(string text, Color foreground, double size,DanmakuMode mode,bool shadow,int speed=5000)
        {
            switch(mode)
            {
                case DanmakuMode.Top:
                    AddTopMotionlessDanmaku(text, foreground, size, shadow);
                    break;
                case DanmakuMode.Bottom:
                    AddBottomMotionlessDanmaku(text, foreground, size, shadow);
                    break;
                case DanmakuMode.Scroll:
                    AddScrollableDanmaku(text, foreground, size,speed, shadow);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Hide/Show the presenter
        /// </summary>
        public void SwitchVisibility()
        {
            this._container.Visibility = this._container.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Pause the player
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            _currentStoryboards.ForEach(s => s.Pause());
        }
        /// <summary>
        /// Resume the player
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            _currentStoryboards.ForEach(s => s.Resume());
        }
        /// <summary>
        /// Remove all danmakus
        /// </summary>
        public void Clear()
        {
            if(this._container!=null&&_currentStoryboards!=null&&_isOccupied!=null&&_isOccupiedTop!=null&&_isOccupiedBottom!=null)
            {
                _currentStoryboards.ForEach(s=>s.Stop());
                _currentStoryboards.Clear();
                this._container.Children.Clear();
                _isOccupied = new bool[_lines];
                _isOccupiedTop = new bool[_lines];
                _isOccupiedBottom = new bool[_lines];
            }            
        }
        /// <summary>
        /// Set the danmakus count limit
        /// </summary>
        /// <param name="count"></param>
        public void SetLimit(int count)
        {
            if(count!=-1)
             _limitCount = count;
        }
        private double GetDefaultFontSize(double size)
        {
            //not sure?
            return size-5;
        }
    }
    public enum DanmakuMode
    {
        Top,
        Scroll,
        ScrollFromLeft,
        Bottom,
        Advanced,
        Script
    }
    public enum DanmakuSource
    {
        AcFun,
        Bilibili,
        Niconico,
        Other
    }
}

