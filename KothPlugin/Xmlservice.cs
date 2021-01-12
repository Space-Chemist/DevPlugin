using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace KothPlugin
{
    [Serializable()]
    [XmlRoot(ElementName = "Session")]
    public class session
    {
        [XmlElement("worldscores")]
        public List<WorldScores> Ws { get; set; }
            
    }

    [Serializable()]
    [XmlRoot(ElementName = "worldscores")]
    public class WorldScores
    {
        [XmlElement] 
        public List<WorldDescription> wd { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "worlddescription")]
    public class WorldDescription
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public List<Scores> sc { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "scores")]
    public class Scores
    {
        [XmlElement]
        public List<ScoreDescription> sd { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "scoredescription")]
    public class ScoreDescription
    {
        [XmlElement]
        public long FactionId { get; set; }
        [XmlElement]
        public string FactionName { get; set; }
        [XmlElement]
        public string FactionTag { get; set; }
        [XmlElement]
        public int Points { get; set; }
        [XmlElement]
        public string PlanetId { get; set; }
        [XmlElement]
        public string Gridname { get; set; }
    }
}