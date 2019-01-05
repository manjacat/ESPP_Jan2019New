using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class CreditorMasterModels
    {
        public virtual DbSet<AP_CREDITORMASTER> AP_CREDITORMASTER { get; set; }
    }
    public class AP_CREDITORMASTER
    {
        [Key]
        public string CREDITORCODE { get; set; }
        public string CREDITORNAME { get; set; }
    }
}