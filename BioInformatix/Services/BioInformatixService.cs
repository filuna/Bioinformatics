using BioInformatix.Class;
using BioInformatix.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MongoBaseRepository;
using MongoDB.Bson;
using ServiceConnector.Classes.Ifaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BioInformatix.Services
{
	public class BioInformatixService : IBioInformatixService
	{
		IMongoRepository db;
		IClustalOmegaConnectorClient clustalService;
		ISimplePhylologyConnectorClient phylologyService;
		IBlastConnectorClient blastService;

		public BioInformatixService(IMongoRepository _db = null, IClustalOmegaConnectorClient _clService = null, ISimplePhylologyConnectorClient _plService = null, IBlastConnectorClient _bService = null)
		{
			db = _db ?? DependencyResolver.Current.GetService<IMongoRepository>();
			clustalService = _clService ?? DependencyResolver.Current.GetService<IClustalOmegaConnectorClient>();
			phylologyService = _plService ?? DependencyResolver.Current.GetService<ISimplePhylologyConnectorClient>();
			blastService = _bService ?? DependencyResolver.Current.GetService<IBlastConnectorClient>();
		}

		#region Sequence

		public void SaveSequenceData(IList<CazyItem> items, UserProject userProject)
		{
			db.AddItems<Sequence>(items.Select(x => new Sequence(x) { ProjectId = userProject.ProjectId }).ToList(), userProject.UserId);
		}

		public void UpdateSequenceData(IList<Sequence> items)
		{
			foreach (var item in items) db.UpdateItem(item);
		}

		public Sequence GetSequence(string seqId)
		{
			ObjectId id = ObjectId.Empty;
			if (ObjectId.TryParse(seqId, out id))
			{
				return db.GetItem<Sequence>(x => x._id == id);
			}
			return null;
		}

		public bool SelUnSelAllSequence(string projectId, bool sel, bool isBlast)
		{
			ObjectId id = ObjectId.Empty;
			if (ObjectId.TryParse(projectId, out id))
			{
				var querySeq = isBlast ? db.GetCollection<Sequence>(x => x.ProjectId == id && !String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList() :
						db.GetCollection<Sequence>(x => x.ProjectId == id && String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList();
				querySeq.ForEach(x => x.Selected = sel);
				SequenceProject project = db.GetItem<SequenceProject>(x => x._id == id);
				project.ActualTaxons = new List<string>();
				db.UpdateItem(project);
				UpdateSequenceData(querySeq);
			}
			return false;
		}

		public bool UpdateSequence(Sequence seq)
		{
			return db.UpdateItem(seq);
		}

		public IList<Sequence> GetSequenceForProject(string projectId, string[] taxons, bool isBlast = false)
		{
			ObjectId id = ObjectId.Empty;
			if (ObjectId.TryParse(projectId, out id))
			{
				IQueryable<Sequence> querySeq = null;
				if (taxons != null)
				{
					querySeq = db.GetCollection<Sequence>(x => x.ProjectId == id && x.Lineage != null).AsQueryable();
					IList<Sequence> sequences = querySeq.ToList();
					IList<Sequence> hlpSequences = new List<Sequence>();
					foreach (string taxon in taxons)
					{
						hlpSequences.AddRange(sequences.Where(x => x.Lineage.Contains(taxon)).ToList());
					}
					querySeq = hlpSequences.AsQueryable();
				}
				else
				{
					querySeq = db.GetCollection<Sequence>(x => x.ProjectId == id).AsQueryable();
				}
				if (isBlast) querySeq = querySeq.Where(x => !String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList().AsQueryable();
				else querySeq = querySeq.Where(x => String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList().AsQueryable();
				return querySeq.ToList();
			}
			return new List<Sequence>();
		}

		public void DeleteSequencesForProject(string projectId, bool fromBlast)
		{
			foreach (Sequence seq in GetSequenceForProject(projectId, null, fromBlast))
			{
				db.RemoveItem<Sequence>(x => x._id == seq._id);
			}
		}

		public DataSourceResult GetSequenceForProject([DataSourceRequest] DataSourceRequest request, string projectId, string[] taxons, bool isBlast = false)
		{
			foreach (Kendo.Mvc.SortDescriptor sd in request.Sorts) sd.Member = ConvertSequenceViewModel(sd.Member);
			foreach (Kendo.Mvc.FilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.FilterDescriptor))
			{
				fd.Member = ConvertSequenceViewModel(fd.Member);
			}
			foreach (
				Kendo.Mvc.CompositeFilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor))
				ConvertSequenceFilters(fd);
			ObjectId id = ObjectId.Empty;
			if (ObjectId.TryParse(projectId, out id))
			{
				IQueryable<Sequence> querySeq = null;
				if (taxons != null)
				{
					querySeq = db.GetCollection<Sequence>(x => x.ProjectId == id && x.Lineage != null).AsQueryable();
					IList<Sequence> sequences = querySeq.ToList();
					IList<Sequence> hlpSequences = new List<Sequence>();
					foreach (string taxon in taxons)
					{
						hlpSequences.AddRange(sequences.Where(x => x.Lineage.Contains(taxon)).ToList());
					}
					SequenceProject project = db.GetItem<SequenceProject>(x => x._id == id);
					project.ActualTaxons = taxons.ToList();
					db.UpdateItem(project);
					querySeq = hlpSequences.AsQueryable();
				}
				else
				{
					querySeq = db.GetCollection<Sequence>(x => x.ProjectId == id).AsQueryable();
				}
				if (isBlast) querySeq = querySeq.Where(x => !String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList().AsQueryable();
				else querySeq = querySeq.Where(x => String.IsNullOrEmpty(x.BlastAlignedSequence)).ToList().AsQueryable();

				return querySeq.ToDataSourceResult(request);
			}
			return new DataSourceResult();
		}

		public IList<CazyItem> GetSequenceData(ObjectId projectId, bool fromBlast = false)
		{
			IList<Sequence> sequences = new List<Sequence>();
			if (projectId == ObjectId.Empty)
			{
				//sequences = db.GetCollection<Sequence>(x => fromBlast ? x.BlastAlignedSequence != null : x.BlastAlignedSequence == null);
			}
			else sequences = db.GetCollection<Sequence>(x => x.ProjectId == projectId).Where(x=> fromBlast ? x.BlastAlignedSequence != null : x.BlastAlignedSequence == null).ToList();
			return sequences.Select(x => x.GetItemFromSequence()).ToList();
		}

		private void ConvertSequenceFilters(Kendo.Mvc.CompositeFilterDescriptor filter)
		{
			if (filter != null)
			{
				foreach (Kendo.Mvc.FilterDescriptor fd in filter.FilterDescriptors.Where(x => x is Kendo.Mvc.FilterDescriptor)) fd.Member = ConvertSequenceViewModel(fd.Member);
				foreach (Kendo.Mvc.CompositeFilterDescriptor cfd in filter.FilterDescriptors.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor)) ConvertSequenceFilters(cfd);
			}
		}

		private string ConvertSequenceViewModel(string member)
		{
			return member;
		}

		#endregion

		#region Alignment

		public DataSourceResult GetAlignmentForProject([DataSourceRequest] DataSourceRequest request, string projectId)
		{
			foreach (Kendo.Mvc.SortDescriptor sd in request.Sorts) sd.Member = ConvertSequenceViewModel(sd.Member);
			foreach (Kendo.Mvc.FilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.FilterDescriptor))
			{
				fd.Member = ConvertSequenceViewModel(fd.Member);
			}
			foreach (
				Kendo.Mvc.CompositeFilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor))
				ConvertSequenceFilters(fd);
			ObjectId id = ObjectId.Empty;
			if (ObjectId.TryParse(projectId, out id))
			{
				SequenceProject project = db.GetItem<SequenceProject>(x => x._id == id);
				IQueryable<AlignmentSequence> querySeq = project != null && project.Alligments != null ? project.Alligments.AsQueryable() : new List<AlignmentSequence>().AsQueryable();
				return querySeq.ToDataSourceResult(request);
			}
			return new DataSourceResult();
		}

		#endregion

		#region LineageTree

		public IDictionary<string, int> GetLineage(ObjectId projectId, bool fromBlast)
		{
			int lastTaxon = fromBlast ? 12 : 6;
			IDictionary<string, int> lineage = new Dictionary<string, int>();
			IList<CazyItem> items = GetSequenceData(projectId, fromBlast);
			foreach (CazyItem item in items.Where(x => x.Lineage != null))
			{
				int index = 0;
				while (!item.Lineage[index].EndsWith("idae") && item.Lineage.Count - 1 > index && index < lastTaxon)
				{
					if (!lineage.Keys.Contains(item.Lineage[index])) lineage.Add(item.Lineage[index], index);
					index += 1;
				}
			}
			return lineage;
		}

		public IDictionary<int, IList<string>> GetLineageForView(ObjectId projectId, bool fromBlast)
		{
			IDictionary<string, int> lineage = GetLineage(projectId, fromBlast);
			IDictionary<int, IList<string>> treeLineage = new Dictionary<int, IList<string>>();
			foreach (int i in lineage.Values.Distinct())
			{
				treeLineage.Add(i, lineage.Where(x => x.Value == i).Select(x => x.Key).ToList());
			}
			return treeLineage;
		}

		public LineageNode GetLineageTree(ObjectId projectId)
		{
			LineageNode lineage = new LineageNode();
			IList<CazyItem> items = GetSequenceData(projectId);
			foreach (CazyItem item in items.Where(x => x.Lineage != null))
			{
				int index = 0;
				IList<string> hlpLineage = new List<string>();
				while (!item.Lineage[index].EndsWith("idea") && item.Lineage.Count - 1 > index)
				{
					hlpLineage.Add(item.Lineage[index++]);
				}
				AddLineageTree(lineage, hlpLineage);
			}
			return lineage;
		}

		private void AddLineageTree(LineageNode node, IList<string> lineage)
		{
			int index = 0;
			LineageNode lastNode = node;
			while (CheckLineageTree(node, lineage[index]) && lineage.Count - 1 > index) index++;
			if (index == 0) node.Name = lineage[0];
			else
			{
				lastNode = GetLastKnownNode(node, lineage[index - 1]);
				if (lastNode.Items == null) lastNode.Items = new List<LineageNode>();
				if (!lastNode.Items.Select(x => x.Name).Contains(lineage[index])) lastNode.Items.Add(new LineageNode() { Name = lineage[index], BranchNo = index });
			}
			if (index < lineage.Count - 1) AddLineageTree(node, lineage);

		}

		private bool CheckLineageTree(LineageNode node, string val)
		{
			bool result = false;
			if (node.Name == val) return true;
			else
			{
				if (node.Items != null && node.Items.Any())
				{
					foreach (LineageNode n in node.Items)
					{
						if (!result)
						{
							result = CheckLineageTree(n, val);
						}
					}
				}
				else return false;
			}
			return result;
		}

		private LineageNode GetLastKnownNode(LineageNode node, string val)
		{
			if (node.Name == val) return node;
			else
			{
				if (node.Items != null && node.Items.Any())
				{
					foreach (LineageNode n in node.Items)
					{
						node = GetLastKnownNode(n, val);
					}
				}
			}
			return node;
		}

		#endregion

		#region Project
		public void SaveProject(SequenceProject project, ObjectId creatorId)
		{
			if (project._id == ObjectId.Empty)
			{
				db.AddItem(project, creatorId);
			}
			else
			{
				db.UpdateItem(project);
			}
		}

		public IList<SequenceProject> GetProjects(ObjectId userId)
		{
			return db.GetCollection<SequenceProject>(x => x.CreatorId == userId);
		}

		#endregion

		#region Clustal
		/// <summary>
		/// Založení sekvenčního zarovnání v programu ClustalOmega
		/// </summary>
		/// <param name="projectId"></param>
		public void GenerateClustalAligment(ObjectId projectId, string title, string path)
		{
			IList<Sequence> sequences = db.GetCollection<Sequence>(x => x.ProjectId == projectId && x.Selected == true && String.IsNullOrEmpty(x.BlastAlignedSequence));
			string seq = String.Empty;
			foreach (Sequence s in sequences) seq += s.FastaProtein;
			clustalService.SetParams(new ServiceConnector.ClustalConnector.InputParameters() { sequence = seq, outfmt = "vienna" });
			var project = db.GetItem<SequenceProject>(x => x._id == projectId);
			if (project != null)
			{
				if (project.Alligments == null) project.Alligments = new List<AlignmentSequence>();
				AlignmentSequence allign = new AlignmentSequence()
				{
					Created = DateTime.Now,
					CreatorId = project.CreatorId,
					Description = title,
					IsFinished = false,
					_id = ObjectId.GenerateNewId(),
					Source = "Cazy"
				};
				allign.AlignmentGuid = clustalService.Run(title);
				allign.Taxons = project.ActualTaxons;
				allign.AlignmentFilename = path + allign.AlignmentGuid;
				project.Alligments.Add(allign);
				db.UpdateItem(project);
				SelUnSelAllSequence(project.IdString, false, false);
			}
		}

		public void GenerateTree(ObjectId projectId, string alignmentId, string path)
		{
			SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projectId);
			if (project != null)
			{
				AlignmentSequence aligment = project.Alligments.FirstOrDefault(x => x.IdString == alignmentId);
				if (aligment != null)
				{
					string seq = String.Empty;
					foreach (AlignRecord rec in aligment.AllignedSequences) seq += ">" + rec.Organism.Replace(" ", "_") + "\n" + rec.AlignedSequence + "\n";
					phylologyService.SetParams(new ServiceConnector.PhylogenyConnector.InputParameters() { sequence = seq, tree = "phylip" });
					aligment.PhylogeneticGuid = phylologyService.Run(aligment.Description);
					aligment.PhylogeneticFilename = path + aligment.PhylogeneticGuid;
					db.UpdateItem(project);
				}
			}
		}

		private void GetLineage(Sequence item, WebClient client)
		{
			try
			{
				string json = client.DownloadString("https://www.ebi.ac.uk/ena/data/taxonomy/v1/taxon/scientific-name/" + item.Organism);
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

		public bool GetClustalResult(SequenceProject project, string alignmentId)
		{
			IList<string> records = new List<string>();
			if (project != null && project.Alligments != null)
			{
				AlignmentSequence alignment = project.Alligments.First(x => x._id == ObjectId.Parse(alignmentId));
				string result = alignment.AlignmentFilename + ".vienna.aln-vienna.vienna";
				if (alignment != null)
				{
					if (!File.Exists(result))
					{
						result = clustalService.GetResults(alignment.AlignmentGuid, alignment.AlignmentFilename);
						if (String.IsNullOrEmpty(result)) return false;
						result = result + ".aln-vienna.vienna";
					}
					if (!String.IsNullOrEmpty(result))
					{
						using (StreamReader sr = new StreamReader(result))
						{
							StringBuilder sb = new StringBuilder();
							while (!sr.EndOfStream)
							{
								string hlpString = sr.ReadLine();
								sb.AppendLine(hlpString);
								records.Add(hlpString);
							}
							alignment.AllignedSequences = GetRecordsFromResult(project._id, records);
							alignment.IsFinished = true;
							db.UpdateItem(project);
							return true;
						}
					}
				}
			}
			return false;
		}

		private IList<AlignRecord> GetRecordsFromResult(ObjectId projectId, IList<string> records)
		{
			IList<AlignRecord> alignRecords = new List<AlignRecord>();
			SequenceProject sProject = db.GetItem<SequenceProject>(x => x._id == projectId);
			if (sProject != null)
			{
				IList<Sequence> sequences = GetSequenceForProject(projectId.ToString(), null);

				int index = 0;
				while (index < records.Count)
				{
					if (records[index].StartsWith(">"))
					{
						AlignRecord record = new AlignRecord();
						record.AlignedSequence = records[index + 1];
						record.SequenceHeader = records[index];
						Sequence findedSeq = sequences.FirstOrDefault(x => x.FastaProtein.StartsWith(records[index]));
						index += 2;
						if (findedSeq != null)
						{
							record.SequenceId = findedSeq._id;
							record.SequenceName = findedSeq.Name;
							record.Organism = findedSeq.Organism;
							alignRecords.Add(record);
						}
					}
				}
			}
			return alignRecords;
		}

		public bool GetSimplePhyloResult(SequenceProject project, string alignmentId)
		{
			IList<string> records = new List<string>();
			if (project != null && project.Alligments != null)
			{
				AlignmentSequence alignment = project.Alligments.First(x => x._id == ObjectId.Parse(alignmentId));
				string result = alignment.PhylogeneticFilename + ".tree.ph";
				if (alignment != null)
				{
					if (!File.Exists(result))
					{
						result = phylologyService.GetResults(alignment.PhylogeneticGuid, alignment.PhylogeneticFilename);
						if (String.IsNullOrEmpty(result)) return false;
						result = result + ".tree.ph";
					}
					if (!String.IsNullOrEmpty(result))
					{
						using (StreamReader sr = new StreamReader(result))
						{
							StringBuilder sb = new StringBuilder();
							while (!sr.EndOfStream)
							{
								string hlpString = sr.ReadLine();
								sb.AppendLine(hlpString);
								records.Add(hlpString);
							}
							foreach (string s in records) alignment.PhylogeneticTree += s;
							db.UpdateItem(project);
							return true;
						}
					}
				}
			}
			return false;
		}

		#endregion

		#region Blast

		public IList<ServiceConnector.BlastConnector.wsParameterDetails> GetCompleteBlastParams()
		{
			var blastParams = blastService.GetParams();
			IList<ServiceConnector.BlastConnector.wsParameterDetails> details = new List<ServiceConnector.BlastConnector.wsParameterDetails>();
			foreach (string parName in blastParams)
			{
				details.Add(blastService.GetParamValues(parName));
			}
			return details;
		}

		public void GenerateBlastSearch(ObjectId projectId, Models.BlastModels.BlastViewModel model)
		{
			SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projectId);
			if (project != null)
			{
				blastService.SetParams(new ServiceConnector.BlastConnector.InputParameters() { stype = "protein", program = "blastp", matrix = "BLOSUM62", sequence = model.Sequence, database = new string[] { model.DatabaseParam }, scores = Int32.Parse(model.ScoresParam) });
				project.BlastSearchGuid = blastService.Run("BlastSearch");
				db.UpdateItem(project);
			}
		}

		public bool GetBlastSearchResult(SequenceProject project, string path, UserProject userProject)
		{
			if (project != null)
			{
				//XmlResult
				string result = path + project.BlastSearchGuid + ".xml.xml";
				if (!File.Exists(result))
				{
					result = blastService.GetResults(project.BlastSearchGuid, result);
					if (String.IsNullOrEmpty(result)) return false;
				}
				if (!String.IsNullOrEmpty(result))
				{
					using (StreamReader sr = new StreamReader(result))
					{
						StringBuilder sb = new StringBuilder();
						XmlReader reader = XmlReader.Create(sr);
						EBIApplicationResult ebiResult = GenerateSequencesFromXml(reader);
						if (ebiResult != null && ebiResult.SequenceSimilaritySearchResult != null)
						{
							IList<Sequence> sequencies = CreateBlastSequencies(ebiResult.SequenceSimilaritySearchResult.hits, project, userProject);
							if (project.Alligments == null) project.Alligments = new List<AlignmentSequence>();
							if (sequencies != null && sequencies.Any())
							{
								AlignmentSequence allign = new AlignmentSequence()
								{
									Created = DateTime.Now,
									CreatorId = project.CreatorId,
									Description = "BlastAlignment",
									IsFinished = false,
									_id = ObjectId.GenerateNewId(),
									Source = "Blast",
									AllignedSequences = new List<AlignRecord>()
								};
								foreach (Sequence seq in sequencies)
								{
									allign.AllignedSequences.Add(new AlignRecord()
									{
										AlignedSequence = seq.BlastAlignedSequence,
										Organism = seq.Organism,
										SequenceId = seq._id,
										SequenceName = seq.Name,
										SequenceHeader = seq.Description
									});
								}
								project.Alligments.Add(allign);
								db.UpdateItem(project);
								return true;
							}

						}
					}
				}
			}
			return false;
		}

		private EBIApplicationResult GenerateSequencesFromXml(XmlReader reader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(EBIApplicationResult));
			EBIApplicationResult result = (EBIApplicationResult)serializer.Deserialize(reader);
			return result;
		}

		private IList<Sequence> CreateBlastSequencies(hits hits, SequenceProject project, UserProject userProject)
		{
			IList<Sequence> sequencies = new List<Sequence>();
			if (hits != null && hits.seqHits != null)
			{
				WebClient webClient = new WebClient();
				foreach (hit hit in hits.seqHits)
				{
					if (IsReadyForUpdate(hit.alignmets))
					{
						Sequence sequence = new Sequence();
						sequence.ProjectId = userProject.ProjectId;
						//výsledek zarovnání blast
						sequence.BlastAlignedSequence = hit.alignmets.alignment[0].patternSeq.matchSeqValue;
						//fasta organismu
						sequence.FastaProtein = hit.alignmets.alignment[0].matchSeq.matchSeqValue;
						sequence.Created = DateTime.Now;
						sequence.Description = hit.description;
						PopulateLineageBlast(sequence, webClient);
						if (!String.IsNullOrEmpty(sequence.Organism) && sequence.Lineage != null && sequence.Lineage.Any())
						{
							if (!sequence.Name.Contains("Fragment"))
							{
								if (!sequencies.Select(x => x.Organism).Contains(sequence.Organism))
								{
									sequencies.Add(sequence);
								}
							}

						}

					}
				}
				db.AddItems(sequencies, userProject.UserId);
			}
			return sequencies;
		}

		private void PopulateLineageBlast(Sequence sequence, WebClient client)
		{
			IList<string> separateDescription = sequence.Description.Split(new string[] { "OS=", "OX" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			if (separateDescription != null && separateDescription.Count > 1)
			{
				sequence.Name = separateDescription[0].TrimEnd(new char[] { ' ' });
				sequence.Organism = separateDescription[1].TrimEnd(new char[] { ' ' });
				GetLineage(sequence, client);
			}
		}

		private bool IsReadyForUpdate(alignments alig)
		{
			if (alig != null)
			{
				if (alig.alignment != null && alig.alignment.Any()) return true;
			}
			return false;
		}

		#endregion
	}
}