using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace KothPlugin
{
    [Serializable()]
    [XmlRoot(ElementName = "Session")]
    public class session
    {
        [XmlElement("PlanetScores")] public List<PlanetScores> PlanetScores { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "PlanetScores")]
    public class PlanetScores
    {
        [XmlElement] public List<PlanetDescription> PlanetDescription { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "PlanetDescription")]
    public class PlanetDescription
    {
        [XmlElement] public string Name { get; set; }
        [XmlElement] public List<Scores> Scores { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "Scores")]
    public class Scores
    {
        [XmlElement] public List<ScoreDescription> ScoreDescription { get; set; }
    }

    [Serializable()]
    [XmlRoot(ElementName = "ScoreDescription")]
    public class ScoreDescription
    {
        [XmlElement] public long FactionId { get; set; }
        [XmlElement] public string FactionName { get; set; }
        [XmlElement] public string FactionTag { get; set; }
        [XmlElement] public int Points { get; set; }
        [XmlElement] public string PlanetId { get; set; }
        [XmlElement] public string Gridname { get; set; }
    }
}