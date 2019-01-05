using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
	public class GroupFeatureModels
	{
		public GroupFeatureModels() { }
		public virtual DbSet<GroupFeature> GroupFeatures { get; set; }
	}
	[Table("GROUPFEATURES")]
	public class GroupFeature
	{
		[DisplayName("Group Features ID")]
		[Column("GROUPFEATUREID")]
		public Guid GroupFeatureID { get; set; }
		[DisplayName("Role")]
		[Column("USERGROUPID")]
		public Guid UserGroupID { get; set; }
		[DisplayName("System Feature")]
		[Column("SYSTEMFEATUREID")]
		public Guid SystemFeatureID { get; set; }
		[DisplayName("Create Date Time")]
		[Column("CREATEDATETIME")]
		public DateTime? CreateDateTime { get; set; }
		public bool DeleteGroupFeatureByGroup(Guid UserGroupID)
		{
			bool bReturn = false;
			ApplicationDbContext db = new ApplicationDbContext();
			List<GroupFeature> groupFeatureList = db.GroupFeatures.Where(i => i.UserGroupID == UserGroupID).ToList<GroupFeature>();
			foreach (GroupFeature groupFeature in groupFeatureList)
			{
				db.GroupFeatures.Remove(groupFeature);
				db.SaveChanges();
				bReturn = true;
			}
			return bReturn;
		}
		public bool InsertGroupFeature(Guid UserGroupID, Guid SystemFeatureID)
		{
			bool bReturn = false;
			ApplicationDbContext db = new ApplicationDbContext();

			if (UserGroupID == null || UserGroupID.ToString() == "") { return false; }
			if (SystemFeatureID == null || SystemFeatureID.ToString() == "") { return false; }

			GroupFeature groupFeature = new GroupFeature();
			groupFeature.GroupFeatureID = Guid.NewGuid();
			groupFeature.UserGroupID = UserGroupID;
			groupFeature.SystemFeatureID = SystemFeatureID;
			groupFeature.CreateDateTime = DateTime.Now;
			db.GroupFeatures.Add(groupFeature);
			db.SaveChanges();
			bReturn = true;

			return bReturn;
		}
		public List<GroupFeaturesDisplay> GetFeatureListByGroup(List<Guid> UserGroupIDList)
		{
			ApplicationDbContext db = new ApplicationDbContext();
			List<GroupFeaturesDisplay> groupFeatureDisplayList = new List<GroupFeaturesDisplay>();

			if (UserGroupIDList == null || UserGroupIDList.Count <= 0) return new List<GroupFeaturesDisplay>();

			foreach (Guid userGroupID in UserGroupIDList)
			{
				List<GroupFeature> groupFeatureList = db.GroupFeatures.Where(i => i.UserGroupID == userGroupID).ToList<GroupFeature>();
				List<SystemFeature> systemFeatureList = db.SystemFeatures.ToList<SystemFeature>();

				foreach (SystemFeature systemFeature in systemFeatureList)
				{
					bool bFound = false;
					GroupFeaturesDisplay groupFeatureDisplay = new GroupFeaturesDisplay();
					groupFeatureDisplay.SystemFeatureID = systemFeature.SystemFeatureID;
					groupFeatureDisplay.FeatureName = systemFeature.FeatureName;
					groupFeatureDisplay.ControllerName = systemFeature.ControllerName;
					groupFeatureDisplay.ActionName = systemFeature.ActionName;
					foreach (GroupFeature groupFeature in groupFeatureList)
					{
						if (groupFeature.SystemFeatureID == systemFeature.SystemFeatureID)
						{
							groupFeatureDisplay.UserGroupID = groupFeature.UserGroupID;
							bFound = true;
							break;
						}
					}
					if (bFound)
					{
						groupFeatureDisplay.Selected = true;
					}
					else
					{
						groupFeatureDisplay.Selected = false;
					}
					groupFeatureDisplayList.Add(groupFeatureDisplay);
				}
			}
			return groupFeatureDisplayList;
		}
		public bool ShowFeature(List<Guid> UserGroupIDList, List<GroupFeaturesDisplay> groupFeatureDisplayList, string FeatureName, string ControllerName, string ActionName)
		{
			bool bReturn = false;
			foreach (GroupFeaturesDisplay groupFeatureDisplay in groupFeatureDisplayList)
			{
				foreach (Guid userGroupID in UserGroupIDList)
				{
					if (userGroupID == groupFeatureDisplay.UserGroupID)
					{
						if (groupFeatureDisplay.Selected == true && groupFeatureDisplay.FeatureName == FeatureName && groupFeatureDisplay.ControllerName == ControllerName && groupFeatureDisplay.ActionName == ActionName)
						{
							bReturn = true;
							break;
						}
					}
				}
			}
			return bReturn;
		}
	}
	public class GroupFeaturesDisplay
	{
		public bool Selected { get; set; }
		public Guid UserGroupID { get; set; }
		public Guid SystemFeatureID { get; set; }
		[DisplayName("Feature Name")]
		public string FeatureName { get; set; }
		[DisplayName("Controller Name")]
		public string ControllerName { get; set; }
		[DisplayName("Action Name")]
		public string ActionName { get; set; }
	}
}