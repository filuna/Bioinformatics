using ServiceConnector.Classes.Ifaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnector.Classes
{
  public class ClustalOmegaConnectorClient : IClustalOmegaConnectorClient
  {
    private ClustalOClient client;
    public ClustalOmegaConnectorClient()
    {
      client = new ClustalOClient() { Email = Properties.Settings.Default.Email };
    }

    public void SetParams(ClustalConnector.InputParameters parameters)
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
        client.GetResults(guid, "aln-vienna", fileName);
        return client.OutFile;
      }
      return String.Empty;
    }
  }
}
