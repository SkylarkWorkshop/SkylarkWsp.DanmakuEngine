using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SkylarkWsp.DanmakuEngine.Model
{
    public class Danmaku
    {
        public string Content { get; set; }
        public IList<string> PositionData { get; set; }
        public double Size { get; set; }
        public int Speed { get; set; }
        public double Time { get; set; }
        public DanmakuMode Mode { get; set; }
        public Color ForegroundColor { get; set; }
        public string UserId { get; set; }
        public string DanmakuId { get; set; }
        /// <summary>
        /// Get or set a bool value indicates if the danmaku has already been showed
        /// </summary>
        public bool IsShowed
        {
            get; set;
        }
    }
}
