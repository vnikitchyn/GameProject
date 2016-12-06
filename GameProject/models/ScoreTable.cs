using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameProject
{
   public class ScoreTable
    {
        [XmlArray("ScoreList"), XmlArrayItem(typeof(ScoreValues), ElementName = "ScoreValues")]
        public List<ScoreValues> ScoreList { get; set; }
    }
}

