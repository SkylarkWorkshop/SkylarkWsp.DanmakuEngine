using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylarkWsp.DanmakuEngine.Model;
using Windows.Data.Json;
using Windows.Web.Http;
using System.Collections.ObjectModel;
using SkylarkWsp.DanmakuEngine.Extension;

namespace SkylarkWsp.DanmakuEngine.DanmakuParser.Factory
{
    class AcDanmakuFactory : IDanmakuFactory
    {
        private static string ACFUNDANMAKUURI = "http://static.comment.acfun.tv/{0}/";
        /// <summary>
        /// Create the danmaku model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DanmakuModelBase> CreateDanmakuModel(string id)
        {
            AcDanmakuModel model = new AcDanmakuModel();

            await ParseToModelCollection(model, id);

            return model;
        }
        /// <summary>
        /// Create the danmaku model with the specified source uri
        /// </summary>
        /// <param name="id"></param>
        /// <param name="acfunUri">The danmaku server uri</param>
        /// <returns></returns>
        public async Task<DanmakuModelBase> CreateDanmakuModel(string id,string acfunUri)
        {
            AcDanmakuModel model = new AcDanmakuModel();
            ACFUNDANMAKUURI = acfunUri;
            await ParseToModelCollection(model, id);
            return model;
        }
        static Uri GetDanmakuCompleteUri(string id) => new Uri(string.Format(ACFUNDANMAKUURI, id));

        async static Task<string> GetStringFromInternet(Uri uri)
        {
            using (HttpClient client = new HttpClient())
            {
                var res = await client.GetStringAsync(uri);
                return res;
            }
        }

        static JsonArray ParseJsonArray(string response)
        {
            JsonArray array;
            JsonArray.TryParse(response, out array);
            if (array != null) return array;
            throw new InvalidCastException("Cannot cast to json array................");
        }

        async Task ParseToModelCollection(AcDanmakuModel model, string id)
        {
            model.DanmakuCollection = new ObservableCollection<Model.Danmaku>();

            string response = await GetStringFromInternet(GetDanmakuCompleteUri(id));


            JsonArray jsonArray = ParseJsonArray(response);

            var commentArray = jsonArray[0].GetArray().Concat(jsonArray[1].GetArray()).Concat(jsonArray[2].GetArray());

            foreach (var item in commentArray)
            {
                string[] posData = item.GetObject().GetNamedString("c").Split(',');
                DanmakuMode mode;
                switch (Convert.ToInt32(posData[2]))
                {
                    case 1:
                        mode = DanmakuMode.Scroll;
                        break;
                    case 7:
                        mode = DanmakuMode.Advanced;
                        break;
                    case 4:
                        mode = DanmakuMode.Bottom;
                        break;
                    case 5:
                        mode = DanmakuMode.Top;
                        break;
                        //todo: add support of 'advanced' danmaku
                    default:
                        mode = DanmakuMode.Scroll;
                        break;
                }
                model.DanmakuCollection.Add(new Danmaku() { Content = item.GetObject().GetNamedString("m"), PositionData = posData, Time = Convert.ToDouble(posData[0]), ForegroundColor = (Convert.ToInt32(posData[1]) | 0xFF000000).ToColor(),Mode= mode,Size= Convert.ToInt32(posData[3]) });
            }
        }
    }
}


