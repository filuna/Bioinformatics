using ServiceConnector.Classes.Ifaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnector.Classes
{
  public class BlastConnectorClient: IBlastConnectorClient
  {
    private NcbiBlastClient client;
    public BlastConnectorClient()
    {
      client = new NcbiBlastClient() { Email = Properties.Settings.Default.Email };
    }

    public IList<string> GetParams()
    {
      return client.GetParams().ToList();
    }

    public BlastConnector.wsParameterDetails GetParamValues(string paramName)
    {
      return client.GetParamDetail(paramName);
    }

    public void SetParams(BlastConnector.InputParameters parameters)
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
        client.GetResults(guid, "xml", fileName);
        return client.OutFile;
      }
      return String.Empty;
    }

        public string GetSvgResults(string guid, string fileName)
        {
            if (client.GetStatus(guid) == "FINISHED")
            {
                client.GetResults(guid, "complete-visual-svg", fileName);
                return client.OutFile;
            }
            return String.Empty;
        }
    }
}
