using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ClobbModels
    {
        public ClobbModels() { }
        public virtual DbSet<Clobb> CLOBB { get; set; }
    }
	[Table("CLOBB")]
	public class Clobb
    {
		[Key]
        [DisplayName("Content")]
		[Column("CONTENT")]
		public Guid Content { get; set; }
    }
}