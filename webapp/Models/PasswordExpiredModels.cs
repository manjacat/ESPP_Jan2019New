using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PasswordExpiredModels
    {
        public bool Expire(string user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userList = db.Users.SingleOrDefault(s => s.Id == user);
            DateTime futureDate = Convert.ToDateTime(userList.PasswordUpdate).AddMonths(3);
            if (futureDate <= DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}