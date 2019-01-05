using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SystemFeatureModels
    {
        public SystemFeatureModels() { }
        public virtual DbSet<SystemFeature> SystemFeatures { get; set; }
    }
	[Table("SYSTEMFEATURE")]
	public class SystemFeature
    {
        [DisplayName("Feature ID")]
		[Column("SYSTEMFEATUREID")]
		public Guid SystemFeatureID { get; set; }
        [DisplayName("Feature Name")]
		[Column("FEATURENAME")]
		public string FeatureName { get; set; }
        [DisplayName("Controller Name")]
		[Column("CONTROLLERNAME")]
		public string ControllerName { get; set; }
        [DisplayName("Action Name")]
		[Column("ACTIONNAME")]
		public string ActionName { get; set; }
        [DisplayName("Create Date Time")]
		[Column("CREATEDATETIME")]
		public DateTime? CreateDateTime { get; set; }
    }
}