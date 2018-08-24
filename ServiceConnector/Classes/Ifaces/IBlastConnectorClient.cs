using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnector.Classes.Ifaces
{
  public interface IBlastConnectorClient
  {

    void SetParams(BlastConnector.InputParameters parameters);
    IList<string> GetParams();
    BlastConnector.wsParameterDetails GetParamValues(string paramName);
    string Run(string title);
    string GetResults(string guid, string fileName);
    string GetSvgResults(string guid, string fileName);
  }
}
