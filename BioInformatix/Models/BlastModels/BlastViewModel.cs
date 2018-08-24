using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Models.BlastModels
{
    public class BlastViewModel: BaseViewModel
    {
        private const string dbParam = "Database";
        private const string scoreParam = "Scores";
        public IDictionary<string, IList<SelectListItem>> Parameters { get; set; }

        public IDictionary<int, IList<string>> LineageForView { get; set; }

        public string Sequence { get; set; }

        public string DatabaseParam { get; set; }

        public string ScoresParam { get; set; }

        public string Description { get; set; }

        public string ProjectId { get; set; }

        public SequenceProject Project { get; set; }

		public bool IsRedirect { get; set; }


        public void Init(Services.IBioInformatixService bioService)
        {
            IList<ServiceConnector.BlastConnector.wsParameterDetails> paramDetails = bioService.GetCompleteBlastParams();
            ServiceConnector.BlastConnector.wsParameterDetails dBases = paramDetails.FirstOrDefault(x => x.name == dbParam);

            Parameters = new Dictionary<string, IList<SelectListItem>>();
            if (dBases != null)
            {
                var selBases = dBases.values.Select(x => x.value).Where(y => y.StartsWith("uniprot")).ToList();
                if (selBases != null && selBases.Any())
                {
                    Parameters.Add(dbParam, selBases.Select(x => new SelectListItem() { Text = x, Value = x, Selected = selBases.IndexOf(x) == 0 }).ToList());
                }
            }
            ServiceConnector.BlastConnector.wsParameterDetails scores = paramDetails.FirstOrDefault(x => x.name == scoreParam);
            if (scores != null)
            {
                Parameters.Add(scoreParam, scores.values.Select(x => new SelectListItem() { Text = x.value, Value = x.value }).ToList());
            }
        }
    }
}