using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylarkWsp.DanmakuEngine.Model
{
   public class BiliDanmakuModel : DanmakuModelBase
    {
        public BiliDanmakuModel()
        {
            DanmakuCollection = new List<Danmaku>();
        }

        public string Chatserver { get; set; }

        public string Chatid { get; set; }

        public string Mission { get; set; }

        public string Maxlimit { get; set; }

        public string Source { get; set; }

        public override List<Danmaku> DanmakuCollection { get; set; }
    }
}
