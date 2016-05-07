using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylarkWsp.DanmakuEngine.DanmakuParser.Factory
{
    public interface IDanmakuFactory
    {
        Task<DanmakuModelBase> CreateDanmakuModel(string id);
        Task<DanmakuModelBase> CreateDanmakuModel(string id,string uri);
    }
}
