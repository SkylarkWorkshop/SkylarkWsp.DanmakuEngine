﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylarkWsp.DanmakuEngine.Model
{
    public abstract class DanmakuModelBase
    { 
        public abstract List<Danmaku> DanmakuCollection { get; set; }
         
    }
}
