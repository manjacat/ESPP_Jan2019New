using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eSPP.Controllers
{
    public class UserGroupController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        static string CurrentUserGroupID = string.Empty;
        static string CurrentUserGroupName = string.Empty;
        // GET: UserGroup
        public ActionResult UserGroup()
        {
            List<IdentityRole> UserRoleList = db.Roles.ToList<IdentityRole>();
            return View(UserRoleList);
        }
        public ActionResult Create()
        {
            return PartialView("_Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserGroupName")] UserGroup userGroup)
        {
            userGroup.Create(userGroup.UserGroupName);
            return RedirectToAction("UserGroup");
        }
        public ActionResult Edit(Guid id)
        {
            List<IdentityRole> RoleList = db.Roles.Where(i => i.Id == id.ToString()).ToList<IdentityRole>();
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(RoleList[0].Id);
            userGroup.UserGroupName = RoleList[0].Name;
            CurrentUserGroupName = userGroup.UserGroupName;
            return PartialView("_Edit", userGroup);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserGroupID,UserGroupName")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                if (userGroup.Edit(CurrentUserGroupName, userGroup.UserGroupName))
                {
                    return RedirectToAction("UserGroup");
                }
            }
            return View(userGroup);
        }
        public ActionResult Delete(Guid id)
        {
            List<IdentityRole> RoleList = db.Roles.Where(i => i.Id == id.ToString()).ToList<IdentityRole>();
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(RoleList[0].Id);
            userGroup.UserGroupName = RoleList[0].Name;
            CurrentUserGroupName = userGroup.UserGroupName;
            return PartialView("_Delete", userGroup);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            List<IdentityRole> RoleList = db.Roles.Where(i => i.Id == id.ToString()).ToList<IdentityRole>();
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(RoleList[0].Id);
            userGroup.UserGroupName = RoleList[0].Name;
            CurrentUserGroupName = userGroup.UserGroupName;
            if (userGroup.Delete(CurrentUserGroupName))
            {
                return RedirectToAction("UserGroup");
            }
            return View(userGroup);
        }
        public ActionResult ManageUser(Guid id)
        {
            List<IdentityUser> userList = db.Users.ToList<IdentityUser>();
            IdentityRole identityRole = db.Roles.Find(id.ToString());
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(identityRole.Id);
            userGroup.UserGroupName = identityRole.Name;
            ViewBag.UserGroup = userGroup;
            List<GroupUser> groupUserList = new List<GroupUser>();
            List<GroupUser> reverseGroupUserList = new List<GroupUser>();
            foreach (IdentityUser identityUser in userList)
            {
                bool bFound = false;
                string UserRoleID = "";
                foreach (IdentityUserRole identityUserRole in identityRole.Users)
                {
                    if (identityUser.Id == identityUserRole.UserId)
                    {
                        UserRoleID = identityUserRole.RoleId;
                        bFound = true;
                    }
                }
                if (bFound)
                {
                    GroupUser groupUser = new GroupUser();
                    groupUser.UserGroupID = new Guid(UserRoleID);
                    groupUser.UserID = new Guid(identityUser.Id);
                    groupUser.UserName = identityUser.UserName;
                    groupUser.Email = identityUser.Email;
                    groupUserList.Add(groupUser);
                }
                else
                {
                    GroupUser groupUser = new GroupUser();
                    groupUser.UserID = new Guid(identityUser.Id);
                    groupUser.UserName = identityUser.UserName;
                    groupUser.Email = identityUser.Email;
                    reverseGroupUserList.Add(groupUser);
                }
            }
            ViewBag.ReverseGroupUserList = reverseGroupUserList;
            return View("ManageUser",groupUserList);
        }
        public ActionResult AddUser(Guid UserGroupID)
        {
            List<IdentityUser> userList = db.Users.ToList<IdentityUser>();
            IdentityRole identityRole = db.Roles.Find(UserGroupID.ToString());
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(identityRole.Id);
            userGroup.UserGroupName = identityRole.Name;
            ViewBag.UserGroup = userGroup;
            List<GroupUser> reverseGroupUserList = new List<GroupUser>();
            foreach (IdentityUser identityUser in userList)
            {
                bool bFound = false;
                string UserRoleID = "";
                foreach (IdentityUserRole identityUserRole in identityRole.Users)
                {
                    if (identityUser.Id == identityUserRole.UserId)
                    {
                        UserRoleID = identityUserRole.RoleId;
                        bFound = true;
                    }
                }
                if (!bFound)
                {
                    GroupUser groupUser = new GroupUser();
                    groupUser.UserID = new Guid(identityUser.Id);
                    groupUser.UserName = identityUser.UserName;
                    groupUser.Email = identityUser.Email;
                    reverseGroupUserList.Add(groupUser);
                }
            }
            return PartialView("_AddUser",reverseGroupUserList);
        }
        public ActionResult AddUserConfirm(Guid UserGroupID, Guid UserID)
        {
            UserGroup userGroupInsert = new UserGroup();
            if (userGroupInsert.AddUserToRole(UserGroupID, UserID))
            {
                return RedirectToAction("ManageUser", new { id = UserGroupID });
            }

            List<IdentityUser> userList = db.Users.ToList<IdentityUser>();
            IdentityRole identityRole = db.Roles.Find(UserGroupID.ToString());
            UserGroup userGroup = new UserGroup();
            userGroup.UserGroupID = new Guid(identityRole.Id);
            userGroup.UserGroupName = identityRole.Name;
            ViewBag.UserGroup = userGroup;
            List<GroupUser> reverseGroupUserList = new List<GroupUser>();
            foreach (IdentityUser identityUser in userList)
            {
                bool bFound = false;
                string UserRoleID = "";
                foreach (IdentityUserRole identityUserRole in identityRole.Users)
                {
                    if (identityUser.Id == identityUserRole.UserId)
                    {
                        UserRoleID = identityUserRole.RoleId;
                        bFound = true;
                    }
                }
                if (!bFound)
                {
                    GroupUser groupUser = new GroupUser();
                    groupUser.UserID = new Guid(identityUser.Id);
                    groupUser.UserName = identityUser.UserName;
                    groupUser.Email = identityUser.Email;
                    reverseGroupUserList.Add(groupUser);
                }
            }
            return RedirectToAction("AddUser", new { UserGroupID = UserGroupID });
        }
        public ActionResult RemoveUser(Guid UserGroupID, Guid UserID)
        {
            GroupUser groupUser = new GroupUser();
            groupUser.UserGroupID = UserGroupID;
            groupUser.UserID = UserID;
            IdentityUser identityUser = db.Users.Find(UserID.ToString());
            groupUser.UserName = identityUser.UserName;
            groupUser.Email = identityUser.Email;
            return View(groupUser);
        }
        [HttpPost, ActionName("RemoveUser")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUserConfirmed(Guid UserGroupID, Guid UserID)
        {
            UserGroup userGroupRemove = new UserGroup();
            if (userGroupRemove.RemoveUserFromRole(UserGroupID, UserID))
            {
                return RedirectToAction("ManageUser", new { id = UserGroupID });
            }
            GroupUser groupUserReturn = new GroupUser();
            groupUserReturn.UserGroupID = UserGroupID;
            groupUserReturn.UserID = UserID;
            IdentityUser identityUserReturn = db.Users.Find(UserID.ToString());
            groupUserReturn.UserName = identityUserReturn.UserName;
            groupUserReturn.Email = identityUserReturn.Email;
            return View(groupUserReturn);
        }
        public ActionResult ManageFeature(Guid UserGroupID)
        {
            CurrentUserGroupID = UserGroupID.ToString();
            ViewBag.UserGroupID = UserGroupID;
            List<GroupFeaturesDisplay> groupFeatureDisplayList = new List<GroupFeaturesDisplay>();
            
            List<GroupFeature> groupFeatureList = db.GroupFeatures.Where(i => i.UserGroupID == UserGroupID).ToList<GroupFeature>();
            List<SystemFeature> systemFeatureList = db.SystemFeatures.ToList<SystemFeature>();
            foreach(SystemFeature systemFeature in systemFeatureList)
            {
                GroupFeaturesDisplay groupFeatureDisplay = new GroupFeaturesDisplay();
                groupFeatureDisplay.UserGroupID = UserGroupID;
                groupFeatureDisplay.SystemFeatureID = systemFeature.SystemFeatureID;
                groupFeatureDisplay.FeatureName = systemFeature.FeatureName;
                groupFeatureDisplay.ControllerName = systemFeature.ControllerName;
                groupFeatureDisplay.ActionName = systemFeature.ActionName;
                foreach(GroupFeature groupFeature in groupFeatureList)
                {
                    if (systemFeature.SystemFeatureID == groupFeature.SystemFeatureID)
                    {
                        groupFeatureDisplay.Selected = true;
                        break;
                    }
                    else
                    {
                        groupFeatureDisplay.Selected = false;
                    }
                }
                groupFeatureDisplayList.Add(groupFeatureDisplay);
            }
            return PartialView("_ManageFeature", groupFeatureDisplayList);
        }
        [HttpPost]
        public ActionResult ManageFeature(string[] SystemFeatureID)
        {
            bool bUpdate = false;
            Guid UserGroupID = new Guid(CurrentUserGroupID);
            if (SystemFeatureID != null)
            {
                GroupFeature groupFeatureUpdate = new GroupFeature();
                groupFeatureUpdate.DeleteGroupFeatureByGroup(UserGroupID);

                foreach (string sSystemFeatureID in SystemFeatureID)
                {
                    Guid gSystemFeatureID = new Guid(sSystemFeatureID);
                    bUpdate = groupFeatureUpdate.InsertGroupFeature(UserGroupID, gSystemFeatureID);
                }
                if (bUpdate)
                {
                    return RedirectToAction("UserGroup");
                }
            }
            ViewBag.UserGroupID = UserGroupID;
            List<GroupFeaturesDisplay> groupFeatureDisplayList = new List<GroupFeaturesDisplay>();

            List<GroupFeature> groupFeatureList = db.GroupFeatures.Where(i => i.UserGroupID == UserGroupID).ToList<GroupFeature>();
            List<SystemFeature> systemFeatureList = db.SystemFeatures.ToList<SystemFeature>();
            foreach (SystemFeature systemFeature in systemFeatureList)
            {
                GroupFeaturesDisplay groupFeatureDisplay = new GroupFeaturesDisplay();
                groupFeatureDisplay.UserGroupID = UserGroupID;
                groupFeatureDisplay.SystemFeatureID = systemFeature.SystemFeatureID;
                groupFeatureDisplay.FeatureName = systemFeature.FeatureName;
                groupFeatureDisplay.ControllerName = systemFeature.ControllerName;
                groupFeatureDisplay.ActionName = systemFeature.ActionName;
                foreach (GroupFeature groupFeature in groupFeatureList)
                {
                    if (systemFeature.SystemFeatureID == groupFeature.SystemFeatureID)
                    {
                        groupFeatureDisplay.Selected = true;
                        break;
                    }
                    else
                    {
                        groupFeatureDisplay.Selected = false;
                    }
                }
                groupFeatureDisplayList.Add(groupFeatureDisplay);
            }
            return View(groupFeatureDisplayList);
        }
    }
}