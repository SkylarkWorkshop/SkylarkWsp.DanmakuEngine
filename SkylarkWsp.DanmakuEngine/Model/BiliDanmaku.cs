using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SkylarkWsp.DanmakuEngine.Model
{
    public class BiliDanmaku:Danmaku
    {
        private BiliDanmaku()
        {
            DanmakuCollection = new ObservableCollection<Danmaku>();
        }

        public static BiliDanmaku CreateBiliDanmaku(XElement doc)
        {
            BiliDanmaku model = new BiliDanmaku();

            model.Chatserver = GetValueFromXElement(doc, elementsName[0]);
            model.Chatid = GetValueFromXElement(doc, elementsName[1]);
            model.Mission = GetValueFromXElement(doc, elementsName[2]);
            model.Maxlimit = GetValueFromXElement(doc, elementsName[3]);
            model.Source = GetValueFromXElement(doc, elementsName[4]);

            foreach (Danmaku item in GetCommentCollection(doc))
            {
                model.DanmakuCollection.Add(item);
            }

            return model;
        }

        private static IEnumerable<Danmaku> GetCommentCollection(XElement doc)
        {
            foreach (XElement item in doc.Elements("d"))
            {
                yield return new Danmaku() {Content= item.Value, PositionData=item.Attribute("p").Value.Split(',') };
            }
        }

        private static string GetValueFromXElement(XElement doc, string elementName) => doc.Element(elementName).Value;

        private static readonly IList<string> elementsName = new List<string> {
            "chatserver", "chatid", "mission", "maxlimit", "source"
        };

        public string Chatserver { get; set; }

        public string Chatid { get; set; }

        public string Mission { get; set; }

        public string Maxlimit { get; set; }

        public string Source { get; set; }

        public ObservableCollection<Danmaku> DanmakuCollection { get; set; }
    }
}
