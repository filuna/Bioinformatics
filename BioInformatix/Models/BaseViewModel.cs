using BioInformatix.Class;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
  public class BaseViewModel
  {
        public string ErrorMessage { get; set; }
    public BaseViewModel() { }
    public UserProject UserProject { get; set; }
  }
}