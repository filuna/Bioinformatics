using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace BioInformatix.Services
{

    [XmlRoot("EBIApplicationResult", Namespace = "http://www.ebi.ac.uk/schema"), Serializable]
    public class EBIApplicationResult
    {
        [XmlElement("SequenceSimilaritySearchResult")]
        public SequenceSimilaritySearchResult SequenceSimilaritySearchResult { get; set; }
    }

    public class SequenceSimilaritySearchResult
    {
        [XmlElement("hits")]
        public hits hits { get; set; }
    }

    public class hits
    {
        [XmlAttribute("total")]
        public int total { get; set; }
        [XmlElement("hit")]
        public List<hit> seqHits { get; set; }

    }

    public class hit
    {
        [XmlAttribute("description")]
        public string description { get; set; }
        [XmlAttribute("id")]
        public string id { get; set; }
        [XmlAttribute("database")]
        public string database { get; set; }
        [XmlElement("alignments")]
        public alignments alignmets { get; set; }
    }

    public class alignments
    {
        [XmlAttribute("total")]
        public int total { get; set; }
        [XmlElement("alignment")]
        public List<alignment> alignment { get; set; }

    }

    public class alignment
    {
        [XmlElement("matchSeq")]
        public matchSeq matchSeq { get; set; }
        [XmlElement("pattern")]
        public matchSeq patternSeq { get; set; }

        [XmlElement("querySeq")]
        public matchSeq querySeq { get; set; }
    }

    public class matchSeq
    {
        [XmlText]
        public string matchSeqValue { get; set; }

    }

    public class querySeq
    {
        [XmlText]
        public string querySeqValue { get; set; }

    }

    public class patternSeq
    {
        [XmlText]
        public string patternSeqValue { get; set; }

    }
}