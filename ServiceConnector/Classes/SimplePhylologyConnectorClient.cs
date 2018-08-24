using ServiceConnector.Classes.Ifaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnector.Classes
{
  public class SimplePhylologyConnectorClient: ISimplePhylologyConnectorClient
  {
    private PhylologyClient client;
    public SimplePhylologyConnectorClient()
    {
      client = new PhylologyClient() { Email = Properties.Settings.Default.Email };
    }

    public void SetParams(PhylogenyConnector.InputParameters parameters)
    {
      client.InParams = parameters;
    }

    public string Run(string title)
    {
      return client.RunApp(client.Email, title, client.InParams);
    }

    public string GetResults(string guid, string fileName)
    {
      if (client.GetStatus(guid) == "FINISHED")
      {
        client.GetResults(guid, "aln-phylip", fileName);
        return client.OutFile;
      }
      return String.Empty;
    }
  }
}
