using SkylarkWsp.DanmakuEngine.DanmakuParser.Factory;
using SkylarkWsp.DanmakuEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylarkWsp.DanmakuEngine.DanmakuParser
{
    public class DanmakuParser
    {
        /// <summary>
        /// Get danmakus from Bilibili
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<BiliDanmakuModel> GetBiliDanmaku(string id)
        {
            IDanmakuFactory fac = new BiliDanmakuFactory();
            return (BiliDanmakuModel)await fac.CreateDanmakuModel(id);
        }
        /// <summary>
        /// Get danmakus from Bilibili with the specified source uri
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri">The danmaku server uri</param>
        /// <returns></returns>
        public static async Task<BiliDanmakuModel> GetBiliDanmaku(string id,string uri)
        {
            IDanmakuFactory fac = new BiliDanmakuFactory();
            return (BiliDanmakuModel)await fac.CreateDanmakuModel(id,uri);
        }
        /// <summary>
        /// Get danmakus from AcFun
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>   
        public static async Task<AcDanmakuModel> GetAcDanmaku(string id)
        {
            IDanmakuFactory fac = new AcDanmakuFactory();
            return (AcDanmakuModel)await fac.CreateDanmakuModel(id);
        }
        /// <summary>
        /// Get danmakus from AcFun with the specified source uri
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri">The danmaku server uri</param>
        /// <returns></returns>
        public static async Task<AcDanmakuModel> GetAcDanmaku(string id,string uri)
        {
            IDanmakuFactory fac = new AcDanmakuFactory();
            return (AcDanmakuModel)await fac.CreateDanmakuModel(id,uri);
        }
    }
}
