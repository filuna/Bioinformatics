using BioInformatix.Class;
using BioInformatix.Models;
using Kendo.Mvc.UI;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformatix.Services
{
  public interface IBioInformatixService
  {
    void SaveSequenceData(IList<CazyItem> items, UserProject userProject);
    IList<CazyItem> GetSequenceData(ObjectId projectId, bool fromBlast = false);
    void DeleteSequencesForProject(string projectId, bool fromBlast);
    IList<SequenceProject> GetProjects(ObjectId userId);
    void UpdateSequenceData(IList<Sequence> items);
    void SaveProject(SequenceProject project, ObjectId creatorId);

    void GenerateClustalAligment(ObjectId projectId, string title, string path);
    void GenerateTree(ObjectId projectId, string alignmentGuid, string path);
    void GenerateBlastSearch(ObjectId projectId, Models.BlastModels.BlastViewModel model);

    bool GetClustalResult(SequenceProject project, string alignmentId);
    bool GetSimplePhyloResult(SequenceProject project, string alignmentId);
    bool GetBlastSearchResult(SequenceProject project, string path, UserProject userProject);

    IDictionary<string, int> GetLineage(ObjectId projectId, bool fromBlast);
    LineageNode GetLineageTree(ObjectId projectId);
    IDictionary<int, IList<string>> GetLineageForView(ObjectId projectId, bool fromBlast);
    DataSourceResult GetSequenceForProject([DataSourceRequest] DataSourceRequest request, string projectId, string[] taxons, bool isBlast);
    DataSourceResult GetAlignmentForProject([DataSourceRequest] DataSourceRequest request, string projectId);

    IList<Sequence> GetSequenceForProject(string projectId, string[] taxons, bool isBlast);
    Sequence GetSequence(string seqId);
    bool UpdateSequence(Sequence seq);
    bool SelUnSelAllSequence(string projectId, bool sel, bool isBlast);
    IList<ServiceConnector.BlastConnector.wsParameterDetails> GetCompleteBlastParams();
  }
}
