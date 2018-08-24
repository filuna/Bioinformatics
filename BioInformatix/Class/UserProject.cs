using MongoBaseRepository.Classes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Class
{
  public class UserProject: BaseEntity
  {
    public ObjectId UserId { get; set; }
    public ObjectId ProjectId { get; set; }
        public string ProjectString { get; set; }
  }
}