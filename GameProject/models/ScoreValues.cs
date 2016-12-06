using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameProject
{
    [XmlRoot("ScoreTable")]
    public class ScoreValues
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
