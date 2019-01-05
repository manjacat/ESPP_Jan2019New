using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class UserGroupModels
    {
        public UserGroupModels() { }
    }
    public class UserGroup
    {
        ApplicationDbContext db = new ApplicationDbContext();

		[Column("USERGROUPID")]
		public Guid UserGroupID { get; set; }
        [DisplayName("Role Name")]
		[Column("USERGROUPNAME")]
		public string UserGroupName { get; set; }

        public bool Create(string UserGroupName)
        {
            bool bReturn = false;
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (!roleManager.RoleExists(UserGroupName))
            {
                IdentityRole identityRole = new IdentityRole();
                identityRole.Name = UserGroupName;
                roleManager.Create(identityRole);
                db.SaveChanges();
                bReturn = true;
            }
            return bReturn;
        }
        public bool Edit(string OldGroupName, string NewGroupName)
        {
            bool bReturn = false;
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (roleManager.RoleExists(OldGroupName))
            {
                IdentityRole identityRole = roleManager.FindByName(OldGroupName);
                identityRole.Name = NewGroupName;
                roleManager.Update(identityRole);
                db.SaveChanges();
                bReturn = true;
            }
            return bReturn;
        }
        public bool Delete(string UserGroupName)
        {
            bool bReturn = false;
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (roleManager.RoleExists(UserGroupName))
            {
                IdentityRole identityRole = roleManager.FindByName(UserGroupName);
                roleManager.Delete(identityRole);
                db.SaveChanges();
                bReturn = true;
            }
            return bReturn;
        }
        public bool AddUserToRole(Guid UserGroupID, Guid UserID)
        {
            bool bReturn = false;
            IdentityRole identityRole = db.Roles.Find(UserGroupID.ToString());
            IdentityUserRole identityUserRole = new IdentityUserRole();
            identityUserRole.RoleId = UserGroupID.ToString();
            identityUserRole.UserId = UserID.ToString();
            identityRole.Users.Add(identityUserRole);
            db.SaveChanges();
            bReturn = true;
            return bReturn;
        }
        public bool RemoveUserFromRole(Guid UserGroupID, Guid UserID)
        {
            bool bReturn = false;
            IdentityRole identityRole = db.Roles.Find(UserGroupID.ToString());
            foreach (IdentityUserRole identityUserRole in identityRole.Users)
            {
                if (identityUserRole.UserId == UserID.ToString())
                {
                    identityRole.Users.Remove(identityUserRole);
                    db.SaveChanges();
                    bReturn = true;
                    break;
                }
            }
            return bReturn;
        }
        public List<Guid> GetGroupsByUser(Guid UserID)
        {
            List<Guid> UserGroupIDList = new List<Guid>();
            List<IdentityRole> identityRoleList = db.Roles.ToList<IdentityRole>();
            foreach (IdentityRole identityRole in identityRoleList)
            {
                List<IdentityUserRole> identityUserRoleList = identityRole.Users.ToList<IdentityUserRole>();
                foreach (IdentityUserRole identityUserRole in identityUserRoleList)
                {
                    if (identityUserRole.UserId == UserID.ToString())
                    {
                        Guid UserGroupID = new Guid(identityUserRole.RoleId);
                        UserGroupIDList.Add(UserGroupID);
                        break;
                    }
                }
            }
            return UserGroupIDList;
        }
        public List<Guid> GetGroupByLoginUser()
        {
            List<Guid> UserGroupIDList = new List<Guid>();
            if (HttpContext.Current.Session["UserLoginID"] != null && HttpContext.Current.Session["UserLoginID"].ToString() != "")
            {
                string UserID = HttpContext.Current.Session["UserLoginID"].ToString();
                List<IdentityRole> identityRoleList = db.Roles.ToList<IdentityRole>();
                foreach (IdentityRole identityRole in identityRoleList)
                {
                    List<IdentityUserRole> identityUserRoleList = identityRole.Users.ToList<IdentityUserRole>();
                    foreach (IdentityUserRole identityUserRole in identityUserRoleList)
                    {
                        if (identityUserRole.UserId == UserID.ToString())
                        {
                            Guid UserGroupID = new Guid(identityUserRole.RoleId);
                            UserGroupIDList.Add(UserGroupID);
                            break;
                        }
                    }
                }
            }
            return UserGroupIDList;
        }
    }
    public class GroupUser
    {
        [DisplayName("Group User ID")]
		[Column("GROUPUSERID")]
		public Guid GroupUserID { get; set; }
        [DisplayName("Role ID")]
		[Column("USERGROUPID")]
		public Guid UserGroupID { get; set; }
        [DisplayName("User ID")]
		[Column("USERID")]
		public Guid UserID { get; set; }
        [DisplayName("User Name")]
		[Column("USERNAME")]
		public string UserName { get; set; }
        [DisplayName("Email")]
		[Column("EMAIL")]
		public string Email { get; set; }
    }
}