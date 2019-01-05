#region Using

using eSPP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static eSPP.Controllers.AccountController;

#endregion

namespace eSPP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: home/index
        public ActionResult Index(string getuserid, ManageMessageId? message)
        {
			ViewBag.StatusMessage =
			message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
			: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
			: message == ManageMessageId.Error ? "An error has occurred."
			: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
			: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
			: message == ManageMessageId.ResetPassword ? "Katalaluan Baru Telah Dihantar Ke Emel"
			: "";

			ApplicationDbContext db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();
            var userList = db.Users.SingleOrDefault(s => s.Id == user);

            if (userList == null)
            {
                userList = new ApplicationUser();
            }

            getuserid = user;
            ViewBag.id = getuserid;
            DateTime currentDate = DateTime.Now;
            DateTime futureDate = Convert.ToDateTime(userList.PasswordUpdate).AddMonths(3); 
            DateTime futureDate1 = currentDate.AddMonths(-3);
            DateTime startDate = Convert.ToDateTime(userList.PasswordUpdate);
            DateTime endDate = Convert.ToDateTime(userList.PasswordUpdate).AddDays(3);
            int date = ((futureDate - Convert.ToDateTime(userList.PasswordUpdate)).Days);
            if(futureDate <= DateTime.Now)
			{
				return RedirectToAction("ChangePassword", "Manage", new { getuserid = getuserid, Message = ManageMessageId.UpdatePassword });
                //ViewBag.message = "Kata Laluan Anda Sudah Menghampiri 3 Bulan. Sila Tukar Kata Laluan";
            }

            List<EventsModels> Activity = SenaraiAktiviti("A").ToList();
            ViewBag.Activity = Activity;
            return View();
        }

        public ActionResult Social()
        {
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Aktiviti(HR_ACARA aktiviti, HR_TARIKH_ACARA tarikhaktiviti, string HR_KATEGORI, string HR_JENIS)
        {
            var txtMsg = "";
            if(HR_JENIS == "Tambah")
            {
                txtMsg = "ditambah";
            }
            else if(HR_JENIS == "Edit")
            {
                txtMsg = "diubah";
            }
            else
            {
                txtMsg = "dipadam";
            }
           if (ModelState.IsValid)
            {
                if (HR_KATEGORI == "A")
                {
                    if(HR_JENIS == "Padam")
                    {
                        HR_TARIKH_ACARA mTarikh = db.HR_TARIKH_ACARA.Where(s => s.HR_KOD_TARIKH_ACARA == tarikhaktiviti.HR_KOD_TARIKH_ACARA).FirstOrDefault();
                        db.HR_TARIKH_ACARA.Remove(mTarikh);
                        db.SaveChanges();
                    }
                    else
                    {
                        HR_ACARA mAktiviti = db.HR_ACARA.OrderByDescending(s => s.HR_KOD_ACARA).FirstOrDefault();
                        if (mAktiviti == null)
                        {
                            mAktiviti = new HR_ACARA();
                        }

                        int LastID2 = 0;
                        if (aktiviti.HR_KOD_ACARA == null)
                        {
                            if (mAktiviti.HR_KOD_ACARA != null)
                            {
                                var ListID = new string(mAktiviti.HR_KOD_ACARA.SkipWhile(x => x == 'A' || x == '0').ToArray());
                                LastID2 = Convert.ToInt32(ListID);
                            }

                            var Increment = LastID2 + 1;
                            var kod = Convert.ToString(Increment).PadLeft(4, '0');
                            aktiviti.HR_KOD_ACARA = "A" + kod;
                            aktiviti.HR_AKTIF_IND = "Y";
                            db.HR_ACARA.Add(aktiviti);
                            db.SaveChanges();
                        }
                        else
                        {
                            HR_ACARA aktiviti2 = db.HR_ACARA.FirstOrDefault(s => s.HR_KOD_ACARA == aktiviti.HR_KOD_ACARA);
                            aktiviti2.HR_ICON = aktiviti.HR_ICON;
                            aktiviti2.HR_WARNA = aktiviti.HR_WARNA;
                            db.Entry(aktiviti2).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        HR_TARIKH_ACARA mTarikh = db.HR_TARIKH_ACARA.Where(s => s.HR_KOD_TARIKH_ACARA == tarikhaktiviti.HR_KOD_TARIKH_ACARA).FirstOrDefault();
                        if (mTarikh == null)
                        {
                            HR_TARIKH_ACARA semakTarikh = db.HR_TARIKH_ACARA.OrderByDescending(s => s.HR_KOD_TARIKH_ACARA).FirstOrDefault();
                            if (semakTarikh == null)
                            {
                                semakTarikh = new HR_TARIKH_ACARA();
                            }
                            int LastKod2 = 0;
                            if (semakTarikh.HR_KOD_TARIKH_ACARA != null)
                            {
                                var LastKod = new string(semakTarikh.HR_KOD_TARIKH_ACARA.SkipWhile(x => x == 'T' || x == '0').ToArray());
                                LastKod2 = Convert.ToInt32(LastKod);
                            }

                            var Inc = LastKod2 + 1;
                            var kod2 = Convert.ToString(Inc).PadLeft(4, '0');
                            tarikhaktiviti.HR_KOD_TARIKH_ACARA = "T" + kod2;
                            tarikhaktiviti.HR_KOD_ACARA = aktiviti.HR_KOD_ACARA;
                            db.HR_TARIKH_ACARA.Add(tarikhaktiviti);
                        }
                        else
                        {
                            mTarikh.HR_TARIKH = tarikhaktiviti.HR_TARIKH;
                            mTarikh.HR_WAKTU_MULA = tarikhaktiviti.HR_WAKTU_MULA;
                            mTarikh.HR_WAKTU_AKHIR = tarikhaktiviti.HR_WAKTU_AKHIR;
                            db.Entry(mTarikh).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    
                }

                if (HR_KATEGORI == "C")
                {
                    DateTime tarikh = Convert.ToDateTime(tarikhaktiviti.HR_KOD_TARIKH_ACARA);
                    if (HR_JENIS == "Padam")
                    {
                        HR_CUTI_UMUM cutiUmum = db.HR_CUTI_UMUM.Where(s => s.HR_TARIKH == tarikh).FirstOrDefault();
                        if (cutiUmum != null)
                        {
                            db.HR_CUTI_UMUM.Remove(cutiUmum);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var LastID2 = 0;
                        HR_CUTI mCuti = db.HR_CUTI.OrderByDescending(s => s.HR_KOD_CUTI).FirstOrDefault();
                        if (mCuti == null)
                        {
                            mCuti = new HR_CUTI();
                        }

                        if (aktiviti.HR_KOD_ACARA == null)
                        {
                            if (mCuti.HR_KOD_CUTI != null)
                            {
                                var ListID = new string(mCuti.HR_KOD_CUTI.SkipWhile(x => x == 'C' || x == '0').ToArray());
                                LastID2 = Convert.ToInt32(ListID);
                            }

                            mCuti = new HR_CUTI();

                            var Increment = LastID2 + 1;
                            var kod = Convert.ToString(Increment).PadLeft(4, '0');
                            mCuti.HR_KOD_CUTI = "C" + kod;
                            mCuti.HR_AKTIF_IND = "Y";
                            db.HR_CUTI.Add(mCuti);
                            db.SaveChanges();
                        }
                        else
                        {
                            mCuti = db.HR_CUTI.FirstOrDefault(s => s.HR_KOD_CUTI == aktiviti.HR_KOD_ACARA);
                            mCuti.HR_ICON = aktiviti.HR_ICON;
                            mCuti.HR_COLOR = aktiviti.HR_WARNA;
                            db.Entry(mCuti).State = EntityState.Modified;
                        }

                        HR_CUTI_UMUM semakCutiUmum = db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == mCuti.HR_KOD_CUTI && s.HR_TARIKH == tarikhaktiviti.HR_TARIKH).FirstOrDefault();
                        if(semakCutiUmum != null)
                        {
                            return Json(new { error = false, msg = "Maaf tarikh cuti telah wujud" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            HR_CUTI_UMUM cutiUmum = db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == mCuti.HR_KOD_CUTI && s.HR_TARIKH == tarikh).FirstOrDefault();
                            if (cutiUmum != null)
                            {
                                db.HR_CUTI_UMUM.Remove(cutiUmum);
                                db.SaveChanges();
                            }

                            cutiUmum = new HR_CUTI_UMUM();
                            cutiUmum.HR_KOD_CUTI_UMUM = aktiviti.HR_KOD_ACARA;
                            cutiUmum.HR_TARIKH = Convert.ToDateTime(tarikhaktiviti.HR_TARIKH);
                            cutiUmum.HR_CATATAN = tarikhaktiviti.HR_KETERANGAN;
                            db.HR_CUTI_UMUM.Add(cutiUmum);
                            db.SaveChanges();
                        }
                        
                    }
                    
                }

                return Json(new { error = false, msg = "Data berjaya " + txtMsg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = true, msg = "Data tidak berjaya " + txtMsg }, JsonRequestBehavior.AllowGet);
        }

        // GET: home/inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/widgets
        public ActionResult Widgets()
        {
            return View();
        }

        // GET: home/chat
        public ActionResult Chat()
        {
            return View();
        }

        public List<EventsModels> SenaraiAktiviti(string category)
        {
            List<EventsModels> events = new List<EventsModels>();
            if(category == "C" || category == "ALL")
            {
                string cutiInd = "C";
                //if(category == "C")
                //{
                //    cutiInd = "B";
                //}
                List<HR_CUTI> cuti = db.HR_CUTI.Where(s => s.HR_AKTIF_IND == "Y" && s.HR_CUTI_IND == cutiInd).ToList();

                foreach (HR_CUTI item in cuti)
                {
                    List<HR_CUTI_UMUM> cutiumum = db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == item.HR_KOD_CUTI).OrderBy(s => s.HR_TARIKH).ToList();
                    if(category == "ALL")
                    {
                        foreach (HR_CUTI_UMUM item2 in cutiumum)
                        {
                            EventsModels aktiviti = new EventsModels();

                            aktiviti.id = item2.HR_TARIKH.ToString();
                            aktiviti.title = item.HR_KETERANGAN;
                            aktiviti.description = item2.HR_CATATAN;
                            aktiviti.icon = item.HR_ICON;
                            aktiviti.className = item.HR_COLOR == null ? "bg-color-blue" : item.HR_COLOR;
                            aktiviti.start = Convert.ToDateTime(item2.HR_TARIKH);
                            aktiviti.startHour = 0;
                            aktiviti.startMinute = 0;
                            //aktiviti.startSecond = Convert.ToInt16(waktuMula[2]);
                            aktiviti.end = Convert.ToDateTime(item2.HR_TARIKH);
                            aktiviti.endHour = 0;
                            aktiviti.endMinute = 0;
                            //aktiviti.endSecond = Convert.ToInt16(waktuAkhir[2]);
                            aktiviti.category = "C";
                            aktiviti.kod = item.HR_KOD_CUTI.ToString();
                            aktiviti.allDay = true;
                            events.Add(aktiviti);
                        }
                    }
                    else
                    {
                        EventsModels aktiviti = new EventsModels();

                        aktiviti.kod = item.HR_KOD_CUTI.ToString();
                        aktiviti.title = item.HR_KETERANGAN;
                        //aktiviti.description = item.HR_CATATAN;
                        aktiviti.icon = item.HR_ICON;
                        aktiviti.className = item.HR_COLOR == null ? "bg-color-blue" : item.HR_COLOR;
                        aktiviti.category = "C";
                        aktiviti.allDay = true;
                        events.Add(aktiviti);
                    }
                    
                }
            }
            

            if (category == "A" || category == "ALL")
            {
                List<HR_ACARA> sAktiviti = db.HR_ACARA.Where(s => s.HR_AKTIF_IND == "Y").ToList();
                foreach (HR_ACARA item3 in sAktiviti)
                {
                    if (category == "ALL")
                    {
                        List<HR_TARIKH_ACARA> tAktiviti = db.HR_TARIKH_ACARA.Where(s => s.HR_KOD_ACARA == item3.HR_KOD_ACARA).ToList();
                        foreach (HR_TARIKH_ACARA item4 in tAktiviti)
                        {
                            EventsModels aktiviti = new EventsModels();

                            if (item4.HR_WAKTU_MULA == null)
                            {
                                item4.HR_WAKTU_MULA = "00:00";
                            }

                            if (item4.HR_WAKTU_AKHIR == null)
                            {
                                item4.HR_WAKTU_AKHIR = "00:00";
                            }
                            string[] waktuMula = item4.HR_WAKTU_MULA.Split(':');
                            string[] waktuAkhir = item4.HR_WAKTU_AKHIR.Split(':');

                            aktiviti.id = item4.HR_KOD_TARIKH_ACARA;
                            aktiviti.title = item3.HR_TAJUK;
                            aktiviti.description = item4.HR_KETERANGAN;
                            aktiviti.icon = item3.HR_ICON;
                            aktiviti.className = item3.HR_WARNA == null ? "bg-color-blue" : item3.HR_WARNA;
                            aktiviti.start = Convert.ToDateTime(item4.HR_TARIKH);
                            aktiviti.startHour = Convert.ToInt16(waktuMula[0]);
                            aktiviti.startMinute = Convert.ToInt16(waktuMula[1]);
                            //aktiviti.startSecond = Convert.ToInt16(waktuMula[2]);
                            aktiviti.end = Convert.ToDateTime(item4.HR_TARIKH);
                            aktiviti.endHour = Convert.ToInt16(waktuAkhir[0]);
                            aktiviti.endMinute = Convert.ToInt16(waktuAkhir[1]);
                            //aktiviti.endSecond = Convert.ToInt16(waktuAkhir[2]);
                            aktiviti.category = "A";
                            aktiviti.kod = item3.HR_KOD_ACARA;
                            aktiviti.allDay = false;
                            events.Add(aktiviti);
                        }
                    }
                    else
                    {
                        EventsModels aktiviti = new EventsModels();

                        aktiviti.kod = item3.HR_KOD_ACARA;
                        aktiviti.title = item3.HR_TAJUK;
                        //aktiviti.description = item3.HR_KETERANGAN;
                        aktiviti.icon = item3.HR_ICON;
                        aktiviti.className = item3.HR_WARNA == null ? "bg-color-blue" : item3.HR_WARNA;
                        aktiviti.category = "A";
                        aktiviti.allDay = false;
                        events.Add(aktiviti);
                    }
                    
                }
            }
            return events;
        }

        public JsonResult ViewEventKod(string category, string kod)
        {
            List<EventsModels> events = SenaraiAktiviti(category);
            EventsModels araca = events.FirstOrDefault(s => s.kod == kod);
            if(araca == null)
            {
                araca = new EventsModels();
            }

            return Json(araca, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ViewEvent(string category)
        {
            List<EventsModels> events = SenaraiAktiviti(category);
            return Json(events, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult CariCuti(HR_ACARA aktiviti, HR_TARIKH_ACARA tarikhaktiviti, string HR_KATEGORI, string HR_JENIS)
        {
            if(HR_KATEGORI == "C")
            {
                EventsModels events = SenaraiAktiviti("ALL").FirstOrDefault(s => s.id == tarikhaktiviti.HR_TARIKH.ToString() && s.kod == aktiviti.HR_KOD_ACARA);
                if(events != null)
                {
                    return Json(new { error = true, msg = "Maaf tarikh cuti telah wujud"}, JsonRequestBehavior.AllowGet);
                }
            }
            
            return Json(new { error = false, msg = "" }, JsonRequestBehavior.AllowGet);

        }
    }
}