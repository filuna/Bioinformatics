using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnector.Classes.Ifaces
{
  public interface ISimplePhylologyConnectorClient
  {
    void SetParams(PhylogenyConnector.InputParameters parameters);
    string Run(string title);
    string GetResults(string guid, string fileName);
  }
}
