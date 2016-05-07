using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http;
using SkylarkWsp.DanmakuEngine.Model;
using SkylarkWsp.DanmakuEngine.Extension;

namespace SkylarkWsp.DanmakuEngine.DanmakuParser.Factory
{
    public class BiliDanmakuFactory : IDanmakuFactory
    {
        private static string BILIDANMAKUURI = "http://comment.bilibili.tv/{0}.xml";
        /// <summary>
        /// Create the danmaku model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DanmakuModelBase> CreateDanmakuModel(string id)
        {

            BiliDanmakuModel model = new BiliDanmakuModel();
            XElement doc = await CreateXDoc(id);
            model.Chatserver = GetValueFromXElement(doc, elementsName[0]);
            model.Chatid = GetValueFromXElement(doc, elementsName[1]);
            model.Mission = GetValueFromXElement(doc, elementsName[2]);
            model.Maxlimit = GetValueFromXElement(doc, elementsName[3]);
            model.Source = GetValueFromXElement(doc, elementsName[4]);

            foreach (Model.Danmaku item in GetCommentCollection(doc))
            {
                model.DanmakuCollection.Add(item);
            }

            return model;
        }
        /// <summary>
        /// Create the danmaku model with the specified source uri
        /// </summary>
        /// <param name="id"></param>
        /// <param name="biliUri">The danmaku server uri</param>
        /// <returns></returns>
        public async Task<DanmakuModelBase> CreateDanmakuModel(string id,string biliUri)
        {

            BiliDanmakuModel model = new BiliDanmakuModel();
            BILIDANMAKUURI = biliUri;
            XElement doc = await CreateXDoc(id);
            model.Chatserver = GetValueFromXElement(doc, elementsName[0]);
            model.Chatid = GetValueFromXElement(doc, elementsName[1]);
            model.Mission = GetValueFromXElement(doc, elementsName[2]);
            model.Maxlimit = GetValueFromXElement(doc, elementsName[3]);
            model.Source = GetValueFromXElement(doc, elementsName[4]);

            foreach (Model.Danmaku item in GetCommentCollection(doc))
            {
                model.DanmakuCollection.Add(item);
            }            
            return model;
        }
        private static IEnumerable<Model.Danmaku> GetCommentCollection(XElement doc)
        {
            foreach (XElement item in doc.Elements("d"))
            {
                string[] posData = item.Attribute("p").Value.Split(',');
                DanmakuMode mode;
                switch (Convert.ToInt32(posData[1]))
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
                        //todo: add support of 'script', 'advanced', 'scrollfromleft' modes
                    default:
                        mode = DanmakuMode.Scroll;
                        break;
                }
                yield return new Danmaku() { Content= item.Value,PositionData= posData,Time = Convert.ToDouble(posData[0]), ForegroundColor = (Convert.ToInt32(posData[3]) | 0xFF000000).ToColor(), Mode = mode, Size = Convert.ToInt32(posData[2]),UserId=posData[6],DanmakuId=posData[7] };
            }
        }

        private static string GetValueFromXElement(XElement doc, string elementName) => doc.Element(elementName).Value;

        private static readonly IList<string> elementsName = new List<string> {
            "chatserver", "chatid", "mission", "maxlimit", "source"
        };

        private static Uri CreateDanmakuUri(string id) => new Uri(string.Format(BILIDANMAKUURI, id));

        private static async Task<XElement> CreateXDoc(string id) => XElement.Parse(await GetDanmakuContent(id));

        private static async Task<string> GetDanmakuContent(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(CreateDanmakuUri(id));
            }
        }
    }
}
