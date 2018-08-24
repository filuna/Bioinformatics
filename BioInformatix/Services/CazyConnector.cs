using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections;
using BioInformatix.Models;
using Newtonsoft.Json;

namespace BioInformatix.Services
{
  public class CazyConnector : ICazyConnector
  {
    private IList<CazyItem> items = new List<CazyItem>();
    private WebClient webClient { get; set; }
    public string BaseUrl { get; set; }
        public string Pattern { get; set; }
        private string resTable = "pos_onglet";
        private string resLinks = "pages";
        private string uniprotLink = "http://www.uniprot.org/uniprot/";
        private string taxonomyLink = "https://www.ebi.ac.uk/ena/data/taxonomy/v1/taxon/scientific-name/";

    public CazyConnector()
    {
      webClient = new WebClient();
    }

    public IList<CazyItem> GetCazyItems(string url = null, bool ismain = true)
    {
      String strResult = String.Empty;
      WebRequest objRequest = HttpWebRequest.Create(url ?? BaseUrl);

      if (Properties.Settings.Default.Proxy)
      {
        IWebProxy proxy = new WebProxy(Properties.Settings.Default.ProxyHost);
        proxy.Credentials = new NetworkCredential(Properties.Settings.Default.ProxyUser,
          Properties.Settings.Default.ProxyPass);
        objRequest.Proxy = proxy;
      }
      WebResponse objResponse = objRequest.GetResponse();
      using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
      {
        strResult = sr.ReadToEnd();
        sr.Close();
      }
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml(strResult);
      IList<HtmlNode> nodes = new List<HtmlNode>();
      GetBodyNodeById(doc.DocumentNode, nodes, resTable);
      if (nodes.Count > 0)
      {
        HtmlNode rootUl = nodes[0];
        nodes = new List<HtmlNode>();
        foreach (var n in rootUl.ChildNodes.Where(x => x.Name != "#text").Where(x => x.Id != "line_titre").Where(x => x.Id != "royaume"))
        {
          RecItem(n);
        }
        if (ismain)
        {
          IList<string> links = GetSublinks(rootUl);
          foreach (string s in links)
          {
            GetCazyItems("http://www.cazy.org/" + s, false);
          }
        }
      }
      return items;
    }

    private IList<string> GetSublinks(HtmlNode node)
    {
      IList<HtmlNode> subNodes = new List<HtmlNode>();
      IList<string> links = new List<string>();
      GetBodyNodeByClass(node, subNodes, resLinks);
      if (subNodes.Any())
      {
        HtmlNode lastNode = subNodes[0].ChildNodes.Where(x => x.Name == "a").Last();
        if (lastNode != null)
        {
          HtmlAttribute hrefAttr = lastNode.Attributes.FirstOrDefault(x => x.Name == "href");
          if (hrefAttr != null)
          {
            string[] linkParts = hrefAttr.Value.Split(new[] { '=', '#' }, StringSplitOptions.RemoveEmptyEntries);
            if (linkParts.Length == 3)
            {
              int last = 0;
              if (Int32.TryParse(linkParts[1], out last))
              {
                for (int index = 100; index <= last; index = index + 100)
                {
                  links.Add(linkParts[0] + "=" + index.ToString() + "#" + linkParts[2]);
                }
              }
            }
          }
        }
      }
      return links;
    }

        private void RecItem(HtmlNode node)
        {
            CazyItem item = new CazyItem();

            if (node.Name == "tr")
            {
                int index = 0;
                foreach (var n in node.ChildNodes.Where(x => x.Name != "#text"))
                {
                    if (n.Name == "td")
                    {
                        switch (index)
                        {
                            case 0: item.ProteinName = n.InnerText.Replace("&nbsp;", ""); break;
                            case 2: item.Organism = ParseOrganismName(n.InnerText); break;
                            case 4: item.UniprotUrl = n.InnerText.Replace("&nbsp;", ""); break;

                        }
                        index++;
                    }

                }
                if (!String.IsNullOrEmpty(item.UniprotUrl))
                    if (!items.Select(x => x.ProteinName).Contains(item.ProteinName) && !items.Select(x => x.Organism).Contains(item.Organism))
                        if (!String.IsNullOrEmpty(Pattern))
                        {
                            GetFastaFormat(item);
                            GetLineage(item);
                            if (item.ProteinName.ToUpper().Contains(Pattern.ToUpper())) if (item.FastaProtein != null) items.Add(item);
                        }
                        else
                        {
                            GetFastaFormat(item);
                            GetLineage(item);
                            if (item.FastaProtein != null) items.Add(item);
                        }
            }

        }

    private void GetLineage(CazyItem item)
    {
      try
      {
        string json = webClient.DownloadString(taxonomyLink + item.Organism);
        if (json != "No results.")
        {
          dynamic jsonDynamic = JsonConvert.DeserializeObject<dynamic>(json);
          string lineage = jsonDynamic[0].lineage;
          string[] lineageSplit = lineage.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          if (lineageSplit != null && lineageSplit.Any())
          {
            item.Lineage = lineageSplit.Select(x => x.Replace(" ", "")).Where(x => !String.IsNullOrEmpty(x)).ToList();
          }
        }
      }
      catch (Exception e)
      {
      }
    }

    private void GetFastaFormat(CazyItem item)
    {
      try
      {
        item.FastaProtein = webClient.DownloadString(uniprotLink + item.UniprotUrl + ".fasta");
      }
      catch (Exception e)
      {
        item.FastaProtein = null;
      }
    }

    private string ParseOrganismName(string name)
    {
      string hlpName = RemoveCharacters(name.Replace("&nbsp;", ""));
      string[] partNames = hlpName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      if (partNames.Length >= 2) hlpName = partNames[0] + " " + partNames[1];
      return hlpName;
    }


    private string RemoveCharacters(string text)
    {
      text = text.Replace("\n", "");
      text = text.Replace("\t", "");
      text = text.Replace("\r", "");
      return text;
    }

    private HtmlNode DeleteTextNode(HtmlNode html)
    {
      for (int i = 0; i < html.ChildNodes.Count; i++)
      {
        if (html.ChildNodes[i].Name.StartsWith("#")) html.RemoveChild(html.ChildNodes[i--]);
        else DeleteTextNode(html.ChildNodes[i]);
      }
      return html;
    }

    private void GetBodyNodeById(HtmlNode html, IList<HtmlNode> nodes, string nameNode)
    {
      if (html.Id == nameNode) nodes.Add(html);
      foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeById(nod, nodes, nameNode);
    }

    private void GetBodyNodeByClass(HtmlNode html, IList<HtmlNode> nodes, string className)
    {
      if (html.HasAttributes)
      {
        var attrClass = html.Attributes.FirstOrDefault(x => x.Name == "class");
        if (attrClass != null)
        {
          if (attrClass.Value == className) nodes.Add(html);
        }
      }
      foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeByClass(nod, nodes, className);
    }

    private void GetBodyNodeByTag(HtmlNode html, IList<HtmlNode> nodes, string nameNode)
    {
      if (html.Name == nameNode) nodes.Add(html);
      foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeByTag(nod, nodes, nameNode);
    }
  }
}
