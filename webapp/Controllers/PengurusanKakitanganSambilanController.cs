using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Data.Entity;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.tool.xml;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Core.Objects;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Office.Interop.Word;
using ClosedXML.Excel;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using eSPP.App_Helpers.ExcelHelper;

namespace eSPP.Controllers
{
    public class PengurusanKakitanganSambilanController : Controller
    {
        Entities db2 = new Entities();
        Entities1 db3 = new Entities1();

        // GET: PengurusanKakitanganSambilan
        public ActionResult Index()
        {
            //Peribadi("", "");
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            return View(mPeribadi);
        }

        public ActionResult ListTransaksiSambilan(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.Muktamad ? "Data Berjaya Dimuktamadkan. Tiada Lagi Sebarang Perubahan Boleh Dilakukan."
              : "";

            List<AgreementModels> agreelist = new List<AgreementModels>();
            return View(agreelist);
        }

        public PartialViewResult ListTransaksiPartial(ManageMessageId? message, string bulantahunbekerja, string bulantahundibayar)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.Muktamad ? "Data Berjaya Dimuktamadkan. Tiada Lagi Sebarang Perubahan Boleh Dilakukan."
              : "";

            ApplicationDbContext db = new ApplicationDbContext();
            List<AgreementModels> agreelist = new List<AgreementModels>();

            ViewBag.bulantahunbekerja = bulantahunbekerja;
            ViewBag.bulantahundibayar = bulantahundibayar;

            try
            {
                string[] cbulantahunbekerja = bulantahunbekerja.Split('/');
                string[] cbulantahundibayar = bulantahundibayar.Split('/');

                int[] inserttahunbulannotis = Array.ConvertAll(cbulantahunbekerja, int.Parse);
                int[] inserttahunbulanbonus = Array.ConvertAll(cbulantahundibayar, int.Parse);

                for (int i = 0; i < inserttahunbulannotis.Length; i++)
                {
                    var select1 = inserttahunbulannotis.ElementAt(i);
                    var select2 = inserttahunbulanbonus.ElementAt(i);

                    var bulantahunbulannotis = inserttahunbulannotis[0];
                    var tahuntahunbulannotis = inserttahunbulannotis[1];
                    var bulandari = inserttahunbulanbonus.ElementAt(0);
                    var tahundari = inserttahunbulanbonus.ElementAt(1);

                    var tarikhdari = "01/" + bulantahunbulannotis + "/" + tahuntahunbulannotis;
                    var tarikhhingga = "01/" + bulandari + "/" + tahundari;
                    var datedari = Convert.ToDateTime(tarikhdari);
                    var datehingga = Convert.ToDateTime(tarikhhingga);

                    var diff = ((datehingga.Year - datedari.Year) * 12) + datehingga.Month - datedari.Month;

                    var dates = new List<DateTime>();

                    for (var dt = datedari; dt <= datehingga; dt = dt.AddMonths(1))
                    {
                        dates.Add(dt);
                    }

                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                        .Where(s => s.HR_KOD_IND == "G"
                        //&& (Convert.ToDateTime("01/" + s.HR_BULAN_BEKERJA + "/" + s.HR_TAHUN_BEKERJA) >= datedari) 
                        //&& (Convert.ToDateTime("01/" + s.HR_BULAN_BEKERJA + "/" + s.HR_TAHUN_BEKERJA) <= datehingga)).ToList();
                        && s.HR_BULAN_BEKERJA == datedari.Month
                        && s.HR_TAHUN_BEKERJA == datedari.Year
                        && s.HR_BULAN_DIBAYAR == datehingga.Month
                        && s.HR_TAHUN == datehingga.Year)
                        .ToList();

                        //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_BULAN_BEKERJA == bulantahunbulannotis && s.HR_TAHUN_BEKERJA == tahuntahunbulannotis && s.HR_BULAN_DIBAYAR == bulandari && s.HR_TAHUN == tahundari && s.HR_KOD_IND == "G").ToList();

                    foreach (var item in sambilan)
                    {
                        HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();

                        AgreementModels agree = new AgreementModels();
                        agree.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                        agree.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;
                        agree.HR_NO_KPBARU = peribadi.HR_NO_KPBARU;
                        agree.LIST_HR_BULAN_BEKERJA = bulantahunbulannotis;
                        agree.LIST_HR_TAHUN = tahuntahunbulannotis;
                        agree.LIST_HR_BULAN_DIBAYAR = bulandari;
                        agree.LIST_HR_TAHUN_DIBAYAR = tahundari;
                        agree.MUKTAMAD = item.HR_MUKTAMAD;
                        agreelist.Add(agree);
                    }
                    return PartialView("_ListTransaksiPartial", agreelist.GroupBy(s => new { s.HR_NO_PEKERJA, s.HR_NAMA_PEKERJA }).Select(s => s.FirstOrDefault()));
                }
            }
            catch
            {
                
            }
            return PartialView("_ListTransaksiPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string action, string value)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (action == null || value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (action == "1")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA.Contains(value)).ToList();
            }
            else if (action == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.Contains(value)).ToList();
            }
            else if (action == "3")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU.Contains(value)).ToList();
            }

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> status = new List<SelectListItem>
            {
                new SelectListItem { Text = "AKTIF", Value = "Y" },
                new SelectListItem { Text = "TIDAK AKTIF", Value = "T" },
                new SelectListItem { Text = "PESARA", Value = "P" },
                new SelectListItem { Text = "TAHAN GAJI", Value = "N" },
                new SelectListItem { Text = "GANTUNG", Value = "G" }
            };
            ViewBag.status = status;

            return View(mPeribadi);
        }

        //sejarah transaksi sambilan
        public ActionResult TransaksiSambilan(ManageMessageId? message, AgreementModels agree, int? tahundibayar, int? tahunbekerja)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.Tambah ? "Permohonan Telah Berjaya Disimpan."
              : message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
              : message == ManageMessageId.Kemaskini ? "Profil Anda Telah Berjaya Dikemaskini."
              : "";

            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();

            List<SelectListItem> bulandibayar = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulandibayar = new SelectList(bulandibayar, "Value", "Text", DateTime.Now.Month);
            ViewBag.tunggakanbulandibayar = ViewBag.bulandibayar;

            List<SelectListItem> bulanbekerja = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };

            ViewBag.bulanbekerja = new SelectList(bulanbekerja, "Value", "Text", DateTime.Now.AddMonths(-1).Month);
            ViewBag.tunggakanbulanbekerja = new SelectList(bulanbekerja, "Value", "Text", DateTime.Now.AddMonths(-2).Month);



            if (DateTime.Now.Month == '1')
            {
                if (tahunbekerja != null && tahundibayar != null)
                {
                    ViewBag.tahunbekerja = tahunbekerja - 1;
                    ViewBag.tahundibayar = tahundibayar - 1;
                }
                else
                {
                    ViewBag.tahunbekerja = DateTime.Now.Year - 1;
                }
            }
            if (DateTime.Now.Month != '1')
            {
                if (tahunbekerja != null && tahundibayar != null)
                {
                    ViewBag.tahunbekerja = tahunbekerja;
                    ViewBag.tahundibayar = tahundibayar;
                }
                else
                {
                    ViewBag.tahunbekerja = DateTime.Now.Year;
                }
            }

            var namabulan = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.AddMonths(-1).Month);
            var namabulan1 = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            ViewBag.namabulan = namabulan;
            ViewBag.namabulan1 = namabulan1;

            var GE_PEKERJA = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_AKTIF_IND == ("Y") && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("N") || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("A"))).ToList();
            var PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.TUNGGAKAN = db.HR_MAKLUMAT_ELAUN_POTONGAN.ToList();
            ViewBag.ELAUN = db.HR_ELAUN.ToList();
            ViewBag.POTONGAN = db.HR_POTONGAN.ToList();
            ViewBag.CARUMAN = db.HR_CARUMAN.ToList();
            ViewBag.GAJI = db.HR_GAJI_UPAHAN.ToList();
            ViewBag.PEKERJA = PEKERJA;
            ViewBag.HR_PEKERJA = GE_PEKERJA;

            return View(agree);
        }

        [HttpPost]
        public ActionResult TransaksiSambilan(AgreementModels agree, string jumlahtunggakan, string kelulusanydptunggakan, string Command, string maklumatelaunpotongan, string bulantahuntunggakan, string bulantahuntunggakandibayar, int bulanbekerja, int tahundibayar, int bulandibayar, string tunggakan, decimal? jumlahhari, decimal? jumlahot, int tahunbekerja, string kelulusanydp, string tunggakanydp)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
            HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA).SingleOrDefault();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN.Contains("E") && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN_POTONGAN != "E0164").ToList();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatpotongan1 = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN.Contains("P") && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN_POTONGAN != "P0015").ToList();
            HR_MAKLUMAT_ELAUN_POTONGAN maklumatpotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN.Contains("P") && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN_POTONGAN == "P0015");
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN.Contains("C") && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR && s.HR_AKTIF_IND == "Y" && s.HR_KOD_ELAUN_POTONGAN != "C0020").ToList();
            agree.GAJIPOKOK = mpekerjaan.HR_GAJI_POKOK;
            var gajisehari = (mpekerjaan.HR_GAJI_POKOK / 23) * jumlahhari;
            var gajipokok = gajisehari;
            var gajisehariot = (((agree.GAJIPOKOK / 23) * jumlahhari) * 12 / 2504);
            var gajisehariot1 = gajisehariot * jumlahot;

            var user = User.Identity.GetUserId();
            var tbl = db.Users.Where(p => p.Id == user).SingleOrDefault();
            var emel = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == tbl.UserName).SingleOrDefault();
            var role1 = db.UserRoles.Where(d => d.UserId == tbl.Id).SingleOrDefault();
            var role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();

            if (Command == "Hantar")
            {
                foreach (var kwsp in listkwsp)
                {
                    if (gajipokok >= kwsp.HR_UPAH_DARI && gajipokok <= kwsp.HR_UPAH_HINGGA)
                    {
                        HR_TRANSAKSI_SAMBILAN sambilan = new HR_TRANSAKSI_SAMBILAN
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_BEKERJA = bulanbekerja,
                            HR_TAHUN_BEKERJA = tahunbekerja,
                            HR_TAHUN = tahundibayar,
                            HR_BULAN_DIBAYAR = bulandibayar
                        };
                        db.HR_TRANSAKSI_SAMBILAN.Add(sambilan);
                        db.SaveChanges();
                        HR_TRANSAKSI_SAMBILAN_DETAIL majikankwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = bulandibayar,
                            HR_TAHUN = tahundibayar,
                            HR_KOD = "C0020",
                            HR_BULAN_BEKERJA = bulanbekerja,
                            HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN,
                            HR_KOD_IND = "C",
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(majikankwsp);
                        db.SaveChanges();
                        HR_TRANSAKSI_SAMBILAN_DETAIL pekerjakwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = bulandibayar,
                            HR_TAHUN = tahundibayar,
                            HR_KOD = "P0035",
                            HR_BULAN_BEKERJA = bulanbekerja,
                            HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA,
                            HR_KOD_IND = "P",
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(pekerjakwsp);
                        db.SaveChanges();
                        if (maklumatpotongan != null)
                        {
                            HR_TRANSAKSI_SAMBILAN_DETAIL ksdk = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = maklumatpotongan.HR_KOD_ELAUN_POTONGAN,
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = maklumatpotongan.HR_JUMLAH,
                                HR_KOD_IND = maklumatpotongan.HR_ELAUN_POTONGAN_IND,
                                HR_TUNGGAKAN_IND = "T",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ksdk);
                            db.SaveChanges();
                        }
                        HR_TRANSAKSI_SAMBILAN_DETAIL elaunot = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = bulandibayar,
                            HR_TAHUN = tahundibayar,
                            HR_KOD = "E0164",
                            HR_BULAN_BEKERJA = bulanbekerja,
                            HR_JUMLAH = gajisehariot1,
                            HR_KOD_IND = "E",
                            HR_TUNGGAKAN_IND = "T",
                            HR_JAM_HARI = jumlahot,
                            HR_YDP_LULUS_IND = kelulusanydp,
                            HR_TAHUN_BEKERJA = tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunot);
                        db.SaveChanges();
                        HR_TRANSAKSI_SAMBILAN_DETAIL gajipekerja = new HR_TRANSAKSI_SAMBILAN_DETAIL
                        {
                            HR_NO_PEKERJA = agree.HR_PEKERJA,
                            HR_BULAN_DIBAYAR = bulandibayar,
                            HR_TAHUN = tahundibayar,
                            HR_KOD = "GAJPS",
                            HR_BULAN_BEKERJA = bulanbekerja,
                            HR_JUMLAH = gajipokok,
                            HR_KOD_IND = "G",
                            HR_JAM_HARI = jumlahhari,
                            HR_TUNGGAKAN_IND = "T",
                            HR_TAHUN_BEKERJA = tahunbekerja,
                            HR_MUKTAMAD = 0
                        };
                        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gajipekerja);
                        db.SaveChanges();
                        foreach (var sum in maklumatelaun)
                        {
                            HR_TRANSAKSI_SAMBILAN_DETAIL elaunlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = sum.HR_KOD_ELAUN_POTONGAN,
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = sum.HR_JUMLAH,
                                HR_KOD_IND = sum.HR_ELAUN_POTONGAN_IND,
                                HR_TUNGGAKAN_IND = "T",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunlain);
                            db.SaveChanges();
                        }
                        /*foreach (var sum1 in maklumatpotongan1)
						{
							HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
							{
								HR_NO_PEKERJA = agree.HR_PEKERJA,
								HR_BULAN_DIBAYAR = bulandibayar,
								HR_TAHUN = tahundibayar,
								HR_KOD = sum1.HR_KOD_ELAUN_POTONGAN,
								HR_BULAN_BEKERJA = bulanbekerja,
								HR_JUMLAH = sum1.HR_JUMLAH,
								HR_KOD_IND = sum1.HR_ELAUN_POTONGAN_IND,
								HR_TUNGGAKAN_IND = tunggakan,
								HR_TAHUN_BEKERJA = tahunbekerja
							};
							db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
							db.SaveChanges();
						}*/
                        foreach (var sum2 in maklumatcaruman)
                        {
                            HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = sum2.HR_KOD_ELAUN_POTONGAN,
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = sum2.HR_JUMLAH,
                                HR_KOD_IND = sum2.HR_ELAUN_POTONGAN_IND,
                                HR_TUNGGAKAN_IND = "T",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
                            db.SaveChanges();
                        }
                    }
                }
                if (tunggakan == "Y")
                    foreach (var kwsp in listkwsp)
                    {
                        if (gajipokok >= kwsp.HR_UPAH_DARI && gajipokok <= kwsp.HR_UPAH_HINGGA)
                        {
                            HR_TRANSAKSI_SAMBILAN sambilan = new HR_TRANSAKSI_SAMBILAN
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_TAHUN = tahundibayar,
                                HR_BULAN_DIBAYAR = bulandibayar
                            };
                            db.HR_TRANSAKSI_SAMBILAN.Add(sambilan);
                            db.SaveChanges();
                            HR_TRANSAKSI_SAMBILAN_DETAIL majikankwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = "C0020",
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN,
                                HR_KOD_IND = "C",
                                HR_TUNGGAKAN_IND = "Y",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(majikankwsp);
                            db.SaveChanges();
                            HR_TRANSAKSI_SAMBILAN_DETAIL pekerjakwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = "P0035",
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA,
                                HR_KOD_IND = "P",
                                HR_TUNGGAKAN_IND = "Y",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(pekerjakwsp);
                            db.SaveChanges();
                            if (maklumatpotongan != null)
                            {
                                HR_TRANSAKSI_SAMBILAN_DETAIL ksdk = new HR_TRANSAKSI_SAMBILAN_DETAIL
                                {
                                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                                    HR_BULAN_DIBAYAR = bulandibayar,
                                    HR_TAHUN = tahundibayar,
                                    HR_KOD = maklumatpotongan.HR_KOD_ELAUN_POTONGAN,
                                    HR_BULAN_BEKERJA = bulanbekerja,
                                    HR_JUMLAH = maklumatpotongan.HR_JUMLAH,
                                    HR_KOD_IND = maklumatpotongan.HR_ELAUN_POTONGAN_IND,
                                    HR_TUNGGAKAN_IND = "Y",
                                    HR_TAHUN_BEKERJA = tahunbekerja,
                                    HR_MUKTAMAD = 0
                                };
                                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ksdk);
                                db.SaveChanges();
                            }
                            HR_TRANSAKSI_SAMBILAN_DETAIL elaunot = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = "E0164",
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = gajisehariot1,
                                HR_KOD_IND = "E",
                                HR_TUNGGAKAN_IND = "Y",
                                HR_JAM_HARI = jumlahot,
                                HR_YDP_LULUS_IND = kelulusanydp,
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunot);
                            db.SaveChanges();
                            HR_TRANSAKSI_SAMBILAN_DETAIL gajipekerja = new HR_TRANSAKSI_SAMBILAN_DETAIL
                            {
                                HR_NO_PEKERJA = agree.HR_PEKERJA,
                                HR_BULAN_DIBAYAR = bulandibayar,
                                HR_TAHUN = tahundibayar,
                                HR_KOD = "GAJPS",
                                HR_BULAN_BEKERJA = bulanbekerja,
                                HR_JUMLAH = gajipokok,
                                HR_KOD_IND = "G",
                                HR_JAM_HARI = jumlahhari,
                                HR_TUNGGAKAN_IND = "Y",
                                HR_TAHUN_BEKERJA = tahunbekerja,
                                HR_MUKTAMAD = 0
                            };
                            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gajipekerja);
                            db.SaveChanges();
                            foreach (var sum in maklumatelaun)
                            {
                                HR_TRANSAKSI_SAMBILAN_DETAIL elaunlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                                {
                                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                                    HR_BULAN_DIBAYAR = bulandibayar,
                                    HR_TAHUN = tahundibayar,
                                    HR_KOD = sum.HR_KOD_ELAUN_POTONGAN,
                                    HR_BULAN_BEKERJA = bulanbekerja,
                                    HR_JUMLAH = sum.HR_JUMLAH,
                                    HR_KOD_IND = sum.HR_ELAUN_POTONGAN_IND,
                                    HR_TUNGGAKAN_IND = "Y",
                                    HR_TAHUN_BEKERJA = tahunbekerja,
                                    HR_MUKTAMAD = 0
                                };
                                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunlain);
                                db.SaveChanges();
                            }
                            /*foreach (var sum1 in maklumatpotongan1)
							{
								HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
								{
									HR_NO_PEKERJA = agree.HR_PEKERJA,
									HR_BULAN_DIBAYAR = bulandibayar,
									HR_TAHUN = tahundibayar,
									HR_KOD = sum1.HR_KOD_ELAUN_POTONGAN,
									HR_BULAN_BEKERJA = bulanbekerja,
									HR_JUMLAH = sum1.HR_JUMLAH,
									HR_KOD_IND = sum1.HR_ELAUN_POTONGAN_IND,
									HR_TUNGGAKAN_IND = tunggakan,
									HR_TAHUN_BEKERJA = tahunbekerja
								};
								db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
								db.SaveChanges();
							}*/
                            foreach (var sum2 in maklumatcaruman)
                            {
                                HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                                {
                                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                                    HR_BULAN_DIBAYAR = bulandibayar,
                                    HR_TAHUN = tahundibayar,
                                    HR_KOD = sum2.HR_KOD_ELAUN_POTONGAN,
                                    HR_BULAN_BEKERJA = bulanbekerja,
                                    HR_JUMLAH = sum2.HR_JUMLAH,
                                    HR_KOD_IND = sum2.HR_ELAUN_POTONGAN_IND,
                                    HR_TUNGGAKAN_IND = "Y",
                                    HR_TAHUN_BEKERJA = tahunbekerja,
                                    HR_MUKTAMAD = 0
                                };
                                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
                                db.SaveChanges();
                            }
                        }
                    }

                new AuditTrailModels().Log(emel.HR_EMAIL, emel.HR_NAMA_PEKERJA, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.HR_NAMA_PEKERJA + " Telah menambah data untuk pekerja " + agree.HR_PEKERJA, System.Net.Dns.GetHostName(), emel.HR_TELBIMBIT, Request.RawUrl, "TransaksiSambilan");

                return RedirectToAction("TransaksiSambilan", "PengurusanKakitanganSambilan", new { Message = ManageMessageId.Tambah });
            }
            if (Command == "Kemaskini")
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_TAHUN_BEKERJA == tahunbekerja && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_KOD == "GAJPS").SingleOrDefault();
                HR_TRANSAKSI_SAMBILAN_DETAIL ot = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_TAHUN_BEKERJA == tahunbekerja && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_KOD == "E0164").SingleOrDefault();
                gaji.HR_JUMLAH = agree.GAJIPOKOK;
                gaji.HR_JAM_HARI = jumlahhari;
                ot.HR_JUMLAH = gajisehariot1;
                ot.HR_JAM_HARI = jumlahot;

                db.Entry(ot).State = EntityState.Modified;
                db.Entry(gaji).State = EntityState.Modified;
                db.SaveChanges();

                new AuditTrailModels().Log(emel.HR_EMAIL, emel.HR_NAMA_PEKERJA, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA, System.Net.Dns.GetHostName(), emel.HR_TELBIMBIT, Request.RawUrl, "TransaksiSambilan");

                return RedirectToAction("TransaksiSambilan", "PengurusanKakitanganSambilan", new { Message = ManageMessageId.Kemaskini });
            }
            if (Command == "Muktamad")
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_TAHUN_BEKERJA == tahunbekerja && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_KOD == "GAJPS").SingleOrDefault();
                gaji.HR_MUKTAMAD = 1;

                db.Entry(gaji).State = EntityState.Modified;
                db.SaveChanges();

                new AuditTrailModels().Log(emel.HR_EMAIL, emel.HR_NAMA_PEKERJA, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA, System.Net.Dns.GetHostName(), emel.HR_TELBIMBIT, Request.RawUrl, "TransaksiSambilan");

                return RedirectToAction("TransaksiSambilan", "PengurusanKakitanganSambilan", new { Message = ManageMessageId.Kemaskini });
            }
            return View();
        }

        public ActionResult TunggakanModal(AgreementModels agree, string HR_PEKERJA, string HR_KOD)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();

            HR_MAKLUMAT_ELAUN_POTONGAN maklumat = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == HR_KOD && s.HR_AKTIF_IND == "Y").SingleOrDefault();
            var elaun1 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == maklumat.HR_KOD_ELAUN_POTONGAN);
            if (elaun1 != null)
            {
                agree.KETERANGAN = elaun1.HR_PENERANGAN_ELAUN;
            }
            var potongan1 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == maklumat.HR_KOD_ELAUN_POTONGAN);
            if (potongan1 != null)
            {
                agree.KETERANGAN = potongan1.HR_PENERANGAN_POTONGAN;
            }
            var caruman1 = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == maklumat.HR_KOD_ELAUN_POTONGAN);
            if (caruman1 != null)
            {
                agree.KETERANGAN = caruman1.HR_PENERANGAN_CARUMAN;
            }
            var gaji1 = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == maklumat.HR_KOD_ELAUN_POTONGAN);
            if (gaji1 != null)
            {
                agree.KETERANGAN = gaji1.HR_PENERANGAN_UPAH;
            }
            agree.HR_JUMLAH_MAKLUMAT = maklumat.HR_JUMLAH;
            agree.HR_TARIKH_MULA = maklumat.HR_TARIKH_MULA;
            agree.HR_TARIKH_AKHIR = maklumat.HR_TARIKH_AKHIR;

            return PartialView("_Tunggakan", agree);
        }

        [HttpPost]
        public ActionResult TunggakanModal(AgreementModels agree, string Command)
        {
            if (Command == "Kemaskini")
            {
                ApplicationDbContext db = new ApplicationDbContext();
                MajlisContext mc = new MajlisContext();

                HR_MAKLUMAT_ELAUN_POTONGAN maklumat = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA && s.HR_KOD_ELAUN_POTONGAN == agree.HR_KOD && s.HR_AKTIF_IND == "Y").SingleOrDefault();
                maklumat.HR_JUMLAH = agree.HR_JUMLAH_MAKLUMAT;

                db.Entry(maklumat).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Sejarah", "KakitanganSambilan", new { HR_PEKERJA = agree.HR_PEKERJA, Message = ManageMessageId.BayarTunggakan });
            }
            return PartialView("_Tunggakan", agree);
        }

        public ActionResult ProsesGajiSambilan()
        {
            List<SelectListItem> jenis = new List<SelectListItem>
            {
                new SelectListItem { Text = "GAJI", Value = "1" },
                new SelectListItem { Text = "SOCSO", Value = "2" },
                new SelectListItem { Text = "KWSP", Value = "3" },
            };
            ViewBag.jenis = new SelectList(jenis, "Value", "Text", null);

            return View();
        }

        [HttpPost]
        public ActionResult ProsesGajiSambilan(AgreementModels agree, string bulantahun, string Command, string jenis)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string[] bulantahuntunggakanstring = bulantahun.Split('/');

            int[] intbulantahuntunggakanstring = Array.ConvertAll(bulantahuntunggakanstring, int.Parse);
            for (int i = 0; i < intbulantahuntunggakanstring.Length; i++)
            {
                if (jenis == "1")
                {
                    ObjectParameter text = new ObjectParameter("text", typeof(Guid));

                    var bulan = intbulantahuntunggakanstring[0];
                    var tahun = intbulantahuntunggakanstring[1];
                    var procedure = db2.SP_SAMBILAN_SALARY_TEST(bulan, tahun, text);

                    MemoryStream memoryStream = new MemoryStream();
                    TextWriter tw = new StreamWriter(memoryStream);

                    tw.WriteLine(text.Value);
                    tw.Flush();
                    tw.Close();

                    return File(memoryStream.GetBuffer(), "text/csv", "file.csv");
                }
                if (jenis == "2")
                {
                    ObjectParameter text = new ObjectParameter("text", typeof(Guid));

                    var bulan = intbulantahuntunggakanstring[0];
                    var tahun = intbulantahuntunggakanstring[1];
                    var procedure1 = db3.SP_SOCSO_SAMBILAN(bulan, tahun, text);

                    MemoryStream memoryStream = new MemoryStream();
                    TextWriter tw = new StreamWriter(memoryStream);

                    tw.WriteLine(text.Value);
                    tw.Flush();
                    tw.Close();

                    return File(memoryStream.GetBuffer(), "text/csv", "file.csv");
                }
                if (jenis == "3")
                {
                    ObjectParameter text = new ObjectParameter("text", typeof(Guid));

                    var bulan = intbulantahuntunggakanstring[0];
                    var tahun = intbulantahuntunggakanstring[1];
                    var procedure2 = db3.SP_EFT_KWSP_SAMBILAN(bulan, tahun, text);

                    MemoryStream memoryStream = new MemoryStream();
                    TextWriter tw = new StreamWriter(memoryStream);

                    tw.WriteLine(text.Value);
                    tw.Flush();
                    tw.Close();

                    return File(memoryStream.GetBuffer(), "text/csv", "file.csv");
                }
            }

            return new FileStreamResult(new FileStream("../", FileMode.Open), "application/pdf");
        }

        //laporan
        public ActionResult Laporan()
        {
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> jenis = new List<SelectListItem>
            {
                new SelectListItem { Text = "Borang A Sambilan", Value = "1" },
                //new SelectListItem { Text = "Borang A Tunggakan Sambilan", Value = "2" },
                new SelectListItem { Text = "Borang A Sambilan Sukan", Value = "3" },
                //new SelectListItem { Text = "Borang A Tunggakan Sambilan Sukan", Value = "4" },
                new SelectListItem { Text = "Gaji Sambilan", Value = "5" },
                new SelectListItem { Text = "Gaji Sukan", Value = "6" },
                new SelectListItem { Text = "KSDK", Value = "7" },
                new SelectListItem { Text = "KWSP Sambilan", Value = "8" },
                new SelectListItem { Text = "KWSP Sambilan Sukan", Value = "9" },
                new SelectListItem { Text = "Perkeso Sambilan", Value = "10" },
                new SelectListItem { Text = "Perkeso Sambilan Sukan", Value = "11" }
            };
            ViewBag.jenis = new SelectList(jenis, "Value", "Text");

            return View();
        }

        public ActionResult MasSambilan()
        {
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);
            return View();
        }

        [HttpPost]
        public ActionResult MasSambilan(string bulan, string tahun)
        {
            ObjectParameter text = new ObjectParameter("text", typeof(Guid));

            int intBulan = Convert.ToInt32(bulan);
            int intTahun = Convert.ToInt32(tahun);

            var procedure = db2.SP_SAMBILAN_SALARY(intBulan, intTahun, text);

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine(text.Value);
            tw.Flush();
            tw.Close();

            string fileName = "masSambilan_" + bulan + "_" + tahun;
            string fileNamefull = fileName + ".txt";

            return File(memoryStream.GetBuffer(), "text/txt", fileNamefull);
        }

        public ActionResult BonusSambilan()
        {
            List<AgreementModels> agreelist = new List<AgreementModels>();
            return View(agreelist);
        }
        //public ActionResult BonusSambilan(ManageMessageId? message, string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga)
        //      {
        //	ViewBag.StatusMessage =
        //	  message == ManageMessageId.TambahBonus ? "Permohonan Telah Berjaya Dihantar."
        //	  : message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
        //	  : message == ManageMessageId.Kemaskini ? "Profil Anda Telah Berjaya Dikemaskini."
        //	  : "";

        //	ApplicationDbContext db = new ApplicationDbContext();
        //	MajlisContext mc = new MajlisContext();

        //	var GE_PEKERJA = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y" && s.HR_AKTIF_IND == ("Y") && (s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("N") || s.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN.Contains("A"))).ToList();
        //	var PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
        //	ViewBag.TUNGGAKAN = db.HR_MAKLUMAT_ELAUN_POTONGAN.ToList();
        //	ViewBag.ELAUN = db.HR_ELAUN.ToList();
        //	ViewBag.POTONGAN = db.HR_POTONGAN.ToList();
        //	ViewBag.CARUMAN = db.HR_CARUMAN.ToList();
        //	ViewBag.GAJI = db.HR_GAJI_UPAHAN.ToList();
        //	ViewBag.PEKERJA = PEKERJA;
        //	ViewBag.HR_PEKERJA = GE_PEKERJA;
        //	ViewBag.tahunbulannotis = tahunbulannotis;
        //	ViewBag.tahunbulanbonus = tahunbulanbonus;
        //	ViewBag.bulankiradari = bulankiradari;
        //	ViewBag.bulankirahingga = bulankirahingga;

        //	return View();
        //      }

        //[HttpPost]
        //public ActionResult BonusSambilan(AgreementModels agree, string Command, string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga, string gajisehari, string pilihankwsp, string pilihanpcb, string pilihansocso)
        //{
        //	ApplicationDbContext db = new ApplicationDbContext();
        //	MajlisContext mc = new MajlisContext();

        //	string[] ctahunbulannotis = tahunbulannotis.Split('/');
        //	string[] ctahunbulanbonus = tahunbulanbonus.Split('/');

        //	int[] inserttahunbulannotis = Array.ConvertAll(ctahunbulannotis, int.Parse);
        //	int[] inserttahunbulanbonus = Array.ConvertAll(ctahunbulanbonus, int.Parse);
        //	if (Command == "Hantar")
        //	{
        //		HR_BONUS_SAMBILAN bonus = new HR_BONUS_SAMBILAN();
        //		List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
        //		bonus.HR_NO_PEKERJA = agree.HR_PEKERJA;
        //		for (int i = 0; i < inserttahunbulannotis.Length; i++)
        //		{
        //			bonus.HR_BULAN_NOTIS = inserttahunbulannotis[0];
        //			bonus.HR_TAHUN_NOTIS = inserttahunbulannotis[1];
        //		}
        //		for (int i = 0; i < inserttahunbulanbonus.Length; i++)
        //		{
        //			bonus.HR_BULAN_BONUS = inserttahunbulanbonus[0];
        //			bonus.HR_TAHUN_BONUS = inserttahunbulanbonus[1];
        //		}
        //		bonus.HR_JUMLAH = agree.JUMLAHBONUS;
        //		bonus.HR_KWSP_IND = pilihankwsp;
        //		bonus.HR_PCB_IND = pilihanpcb;
        //		bonus.HR_SOCSO_IND = pilihansocso;
        //		bonus.HR_FINALISED_IND = "Y";
        //		bonus.HR_NP_FINALISED = "Y";
        //		bonus.HR_KOD_ELAUN = "E0166";
        //		foreach (var kwsp in listkwsp)
        //		{
        //			if (agree.JUMLAHBONUS >= kwsp.HR_UPAH_DARI && agree.JUMLAHBONUS <= kwsp.HR_UPAH_HINGGA)
        //			{
        //				bonus.HR_CARUMAN_KWSP = kwsp.HR_CARUMAN_MAJIKAN;
        //				bonus.HR_JUMLAH_KWSP = kwsp.HR_CARUMAN_PEKERJA;
        //				var jumlahbonus = agree.JUMLAHBONUS - kwsp.HR_CARUMAN_PEKERJA;
        //				bonus.HR_BONUS_BERSIH = jumlahbonus.Value.ToString("0.00");
        //			}
        //		}
        //		bonus.HR_TARIKH_FINALISED = DateTime.Now;
        //		db.HR_BONUS_SAMBILAN.Add(bonus);
        //		db.SaveChanges();

        //		return RedirectToAction("BonusSambilan", "PengurusanKakitanganSambilan", new { Message = ManageMessageId.TambahBonus });
        //	}

        //	return View();
        //}

        public ActionResult PembayaranAutomatik()
        {
            return View();
        }

        public JsonResult Bahagian(string HR_PEKERJA)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            mc.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PEKERJAAN item = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();
            GE_BAHAGIAN bahagian = mc.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == item.HR_BAHAGIAN && s.GE_KOD_JABATAN == item.HR_JABATAN).SingleOrDefault();

            return Json(bahagian, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Jabatan(string HR_PEKERJA)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            mc.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PEKERJAAN item = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();
            GE_JABATAN bahagian = mc.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == item.HR_JABATAN).SingleOrDefault();

            return Json(bahagian, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Pekerjaan(string HR_JABATAN, string HR_BAHAGIAN)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;
            List<HR_MAKLUMAT_PEKERJAAN> mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_JABATAN == HR_JABATAN && s.HR_BAHAGIAN == HR_BAHAGIAN && s.HR_TARAF_JAWATAN == "S" || s.HR_TARAF_JAWATAN == "A" && s.HR_KAKITANGAN_IND == "Y" && s.HR_TARIKH_TAMAT <= DateTime.Now).ToList<HR_MAKLUMAT_PEKERJAAN>();
            List<AgreementModels> maklumatperibadi = new List<AgreementModels>();
            if (mpekerjaan.Count() <= 0)
            {
                mpekerjaan = new List<HR_MAKLUMAT_PEKERJAAN>();
            }

            foreach (var item in mpekerjaan)
            {
                List<HR_MAKLUMAT_PERIBADI> peribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_AKTIF_IND == "Y").ToList();
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilandetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).ToList();
                foreach (var item1 in peribadi)
                {
                    AgreementModels agree = new AgreementModels
                    {
                        HR_NAMA_PEKERJA = item1.HR_NAMA_PEKERJA,
                        HR_NO_PEKERJA = item1.HR_NO_PEKERJA
                    };
                    maklumatperibadi.Add(agree);
                }
            }
            return Json(maklumatperibadi, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Maklumat(string HR_PEKERJA, 
            int? tahunbekerja, 
            int? bulanbekerja, 
            int? tahundibayar, 
            int? bulandibayar, 
            string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga, 
            decimal? jumlahot, int? jumlahhari)
        {
            //calculation moved to Model/TraksaksiSambilanModels
            AgreementModels kerjaElaun = TraksaksiSambilanModels.Maklumat(HR_PEKERJA, tahunbekerja, bulanbekerja, tahundibayar, bulandibayar,
                tahunbulannotis, tahunbulanbonus, bulankiradari, bulankirahingga, jumlahot, jumlahhari);
            if(kerjaElaun != null)
            {
                return Json(kerjaElaun, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }

        #region Entah Pakai Entah Tak
        public JsonResult MaklumatBonus(string HR_PEKERJA, string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PEKERJAAN item1 = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();
            if (item1 != null)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == pekerjaan.HR_JAWATAN);
                GE_JABATAN jabatan = mc.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == item1.HR_JABATAN).SingleOrDefault();
                GE_BAHAGIAN bahagian = mc.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == item1.HR_BAHAGIAN && s.GE_KOD_JABATAN == item1.HR_JABATAN).SingleOrDefault();
                var gajibonussehari = pekerjaan.HR_GAJI_POKOK / 23;
                AgreementModels kerjaelaun = new AgreementModels();

                if (tahunbulannotis != "" || tahunbulanbonus != "" || bulankiradari != "" || bulankirahingga != "")
                {
                    string[] ctahunbulannotis = tahunbulannotis.Split('/');
                    string[] ctahunbulanbonus = tahunbulanbonus.Split('/');
                    string[] cbulankiradari = bulankiradari.Split('/');
                    string[] cbulankirahingga = bulankirahingga.Split('/');

                    int[] inserttahunbulannotis = Array.ConvertAll(ctahunbulannotis, int.Parse);
                    int[] inserttahunbulanbonus = Array.ConvertAll(ctahunbulanbonus, int.Parse);
                    int[] insertbulankiradari = Array.ConvertAll(cbulankiradari, int.Parse);
                    int[] insertbulankirahingga = Array.ConvertAll(cbulankirahingga, int.Parse);

                    for (int i = 0; i < inserttahunbulannotis.Length; i++)
                    {
                        var select1 = inserttahunbulannotis.ElementAt(i);
                        var select2 = inserttahunbulanbonus.ElementAt(i);
                        var select3 = insertbulankiradari.ElementAt(i);
                        var select4 = insertbulankirahingga.ElementAt(i);

                        var bulantahunbulannotis = inserttahunbulannotis[0];
                        var tahuntahunbulannotis = inserttahunbulannotis[1];
                        var bulandari = insertbulankiradari.ElementAt(0);
                        var tahundari = insertbulankiradari.ElementAt(1);
                        var bulanhingga = insertbulankirahingga.ElementAt(0);
                        var tahunhingga = insertbulankirahingga.ElementAt(1);

                        var dari = "01/" + bulandari + "/" + tahundari;
                        var hingga = "01/" + bulanhingga + "/" + tahunhingga;
                        var kerjadari = Convert.ToDateTime(dari);
                        var kerjahingga = Convert.ToDateTime(hingga);

                        List<HR_TRANSAKSI_SAMBILAN_DETAIL> transaksisambilandetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga).ToList();
                        List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
                        var sumbonus = transaksisambilandetail.Sum(uf => uf.HR_JUMLAH);
                        var purata = sumbonus / transaksisambilandetail.Count;
                        var diff = ((kerjahingga.Year - kerjadari.Year) * 12) + kerjahingga.Month - kerjadari.Month;
                        var bonus = (purata * transaksisambilandetail.Count) / diff;

                        foreach (var kwsp in listkwsp)
                        {
                            if (bonus >= kwsp.HR_UPAH_DARI && bonus <= kwsp.HR_UPAH_HINGGA)
                            {
                                kerjaelaun.CARUMANKWSP = kwsp.HR_CARUMAN_MAJIKAN;
                                kerjaelaun.POTONGANKWSP = kwsp.HR_CARUMAN_PEKERJA;
                                var jumlah = bonus - kwsp.HR_CARUMAN_PEKERJA;
                                kerjaelaun.JUMLAHBONUS1 = jumlah.Value.ToString("0.00");
                            }
                        }
                        kerjaelaun.BONUS = bonus.Value.ToString("0.00");
                        kerjaelaun.TAKTOLAKKWSP = bonus.Value.ToString("0.00");
                    }
                    kerjaelaun.JABATAN = jabatan.GE_KETERANGAN_JABATAN;
                    kerjaelaun.BAHAGIAN = bahagian.GE_KETERANGAN;
                    kerjaelaun.NOKP = peribadi.HR_NO_KPBARU;
                    kerjaelaun.JAWATAN = jawatan.HR_NAMA_JAWATAN;
                    kerjaelaun.GAJISEHARI = gajibonussehari.Value.ToString("0.00");
                    kerjaelaun.PILIHANKWSP = pekerjaan.HR_STATUS_KWSP;
                    kerjaelaun.PILIHANSOCSO = pekerjaan.HR_STATUS_SOCSO;
                    kerjaelaun.PILIHANPCB = pekerjaan.HR_STATUS_PCB;

                    return Json(kerjaelaun, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult Overtime(string HR_PEKERJA, 
            int? tahunbekerja, 
            int? bulanbekerja, 
            int? tahundibayar, 
            int? bulandibayar, 
            string tahunbulannotis, string tahunbulanbonus, 
            //int? bulanbonus, 
            string bulankiradari, string bulankirahingga,
            decimal? jumlahot, int? jumlahhari)
        {
            AgreementModels kerjaElaun = TraksaksiSambilanModels.Maklumat(HR_PEKERJA, tahunbekerja, bulanbekerja, tahundibayar, bulandibayar,
                tahunbulannotis, tahunbulanbonus, bulankiradari, bulankirahingga, jumlahot, jumlahhari);
            if (kerjaElaun != null)
            {
                return Json(kerjaElaun, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult Tunggakan(string HR_PEKERJA, string bulanbekerja, string tahunbekerja, string bulandibayar, string tahundibayar, string tunggakanjumlahhari, string tunggakanjumlahot)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && (s.HR_TUNGGAKAN_IND == "Y")).ToList();

            return Json(sambilan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BonusBekerja(AgreementModels agree, string HR_PEKERJA, string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            if (tahunbulannotis != "" || tahunbulanbonus != "" || bulankiradari != "" || bulankirahingga != "")
            {
                string[] ctahunbulannotis = tahunbulannotis.Split('/');
                string[] ctahunbulanbonus = tahunbulanbonus.Split('/');
                string[] cbulankiradari = bulankiradari.Split('/');
                string[] cbulankirahingga = bulankirahingga.Split('/');

                int[] inserttahunbulannotis = Array.ConvertAll(ctahunbulannotis, int.Parse);
                int[] inserttahunbulanbonus = Array.ConvertAll(ctahunbulanbonus, int.Parse);
                int[] insertbulankiradari = Array.ConvertAll(cbulankiradari, int.Parse);
                int[] insertbulankirahingga = Array.ConvertAll(cbulankirahingga, int.Parse);

                for (int i = 0; i < inserttahunbulannotis.Length; i++)
                {
                    var select1 = inserttahunbulannotis.ElementAt(i);
                    var select2 = inserttahunbulanbonus.ElementAt(i);
                    var select3 = insertbulankiradari.ElementAt(i);
                    var select4 = insertbulankirahingga.ElementAt(i);

                    var bulantahunbulannotis = inserttahunbulannotis[0];
                    var tahuntahunbulannotis = inserttahunbulannotis[1];
                    var bulandari = insertbulankiradari.ElementAt(0);
                    var tahundari = insertbulankiradari.ElementAt(1);
                    var bulanhingga = insertbulankirahingga.ElementAt(0);
                    var tahunhingga = insertbulankirahingga.ElementAt(1);

                    var dari = "01/" + bulandari + "/" + tahundari;
                    var hingga = "01/" + bulanhingga + "/" + tahunhingga;
                    var kerjadari = Convert.ToDateTime(dari);
                    var kerjahingga = Convert.ToDateTime(hingga);

                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> transaksisambilandetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga).ToList();
                    List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
                    var sumbonus = transaksisambilandetail.Sum(uf => uf.HR_JUMLAH);
                    if (transaksisambilandetail.Count > 0)
                    {
                        var purata = sumbonus / transaksisambilandetail.Count;
                        var diff = ((kerjahingga.Year - kerjadari.Year) * 12) + kerjahingga.Month - kerjadari.Month;
                        var bonus = (purata * transaksisambilandetail.Count) / diff;
                        foreach (var kwsp in listkwsp)
                        {
                            if (bonus >= kwsp.HR_UPAH_DARI && bonus <= kwsp.HR_UPAH_HINGGA)
                            {
                                ViewBag.CARUMANKWSP = kwsp.HR_CARUMAN_MAJIKAN;
                                ViewBag.POTONGANKWSP = kwsp.HR_CARUMAN_PEKERJA;
                            }
                        }
                        return Json(transaksisambilandetail.OrderBy(s => s.HR_TAHUN_BEKERJA).GroupBy(s => new { s.HR_KOD, s.HR_NO_PEKERJA, s.HR_TAHUN, s.HR_BULAN_BEKERJA, s.HR_BULAN_DIBAYAR }).Select(s => s.FirstOrDefault()), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult Bonus(AgreementModels agree, string HR_PEKERJA, int tahunbekerja, int id, int dataid, int bulanbonus)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
            var bulanbonuskira = DateTime.Now.AddMonths(-bulanbonus).Month;
            var tahunkira = DateTime.Now.AddMonths(-bulanbonus).Year;
            var bulan = DateTime.Now.Month;
            var tahun = DateTime.Now.Year;
            var tarikh = "01/" + bulanbonuskira + "/" + tahunkira;
            var now = "01/" + bulan + "/" + tahun;
            var kiratarikh = Convert.ToDateTime(tarikh);
            var tarikhnow = Convert.ToDateTime(now);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= kiratarikh) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= tarikhnow)).ToList();
            var sum = sambilan.Sum(uf => uf.HR_JUMLAH);
            var purata = sum / sambilan.Count;
            var bonus = (purata * sambilan.Count) / bulanbonus;

            foreach (var kwsp in listkwsp)
            {
                if (bonus >= kwsp.HR_UPAH_DARI && bonus <= kwsp.HR_UPAH_HINGGA)
                {
                    agree.CARUMANKWSP = kwsp.HR_CARUMAN_MAJIKAN;
                    agree.POTONGANKWSP = kwsp.HR_CARUMAN_PEKERJA;
                }
            }

            agree.BONUS = bonus.Value.ToString("0.00");

            return Json(agree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Export(int tahun, int bulan, string jenis)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            SPGContext spg = new SPGContext();
            List<AgreementModels> agree = new List<AgreementModels>();
            db.Configuration.ProxyCreationEnabled = false;
            if (jenis == "1")
            {
                var jumlah = spg.PA_TRANSAKSI_CARUMAN.Where(s => s.PA_KOD_CARUMAN == "C0034" && s.PA_BULAN_CARUMAN == bulan && s.PA_TAHUN_CARUMAN == tahun).ToList();
                List<HR_MAKLUMAT_PERIBADI> peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
                foreach (var item in jumlah)
                {
                    var jumlah1 = spg.PA_TRANSAKSI_PEMOTONGAN.Where(s => s.PA_KOD_PEMOTONGAN == "P0160" && s.PA_BULAN_POTONGAN == bulan && s.PA_TAHUN_POTONGAN == tahun && s.PA_NO_PEKERJA == item.PA_NO_PEKERJA).SingleOrDefault();
                    AgreementModels agree1 = new AgreementModels();
                    foreach (var item2 in peribadi)
                    {
                        if (item2.HR_NO_PEKERJA == item.PA_NO_PEKERJA)
                        {
                            agree1.HR_NAMA_PEKERJA = item2.HR_NAMA_PEKERJA;
                        }
                    }
                    var amount = item.PA_JUMLAH_CARUMAN + jumlah1.PA_JUMLAH_PEMOTONGAN;
                    agree1.JUMLAH = amount.Value.ToString("0.00");
                    agree1.HR_NO_PEKERJA = item.PA_NO_PEKERJA;
                    agree.Add(agree1);
                }
            }
            if (jenis == "2")
            {
                var jumlah = spg.PA_TRANSAKSI_CARUMAN.Where(s => s.PA_KOD_CARUMAN == "C0035" && s.PA_BULAN_CARUMAN == bulan && s.PA_TAHUN_CARUMAN == tahun).ToList();
                List<HR_MAKLUMAT_PERIBADI> peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
                foreach (var item in jumlah)
                {
                    var jumlah1 = spg.PA_TRANSAKSI_PEMOTONGAN.Where(s => s.PA_KOD_PEMOTONGAN == "P0163" && s.PA_BULAN_POTONGAN == bulan && s.PA_TAHUN_POTONGAN == tahun && s.PA_NO_PEKERJA == item.PA_NO_PEKERJA).SingleOrDefault();
                    AgreementModels agree1 = new AgreementModels();
                    foreach (var item2 in peribadi)
                    {
                        if (item2.HR_NO_PEKERJA == item.PA_NO_PEKERJA)
                        {
                            agree1.HR_NAMA_PEKERJA = item2.HR_NAMA_PEKERJA;
                        }
                    }
                    var amount = item.PA_JUMLAH_CARUMAN + jumlah1.PA_JUMLAH_PEMOTONGAN;
                    agree1.JUMLAH = amount.Value.ToString("0.00");
                    agree1.HR_NO_PEKERJA = item.PA_NO_PEKERJA;
                    agree.Add(agree1);
                }
            }
            if (jenis == "3")
            {
                var jumlah = spg.PA_TRANSAKSI_CARUMAN.Where(s => s.PA_KOD_CARUMAN == "C0034" && s.PA_BULAN_CARUMAN == bulan && s.PA_TAHUN_CARUMAN == tahun).ToList();
                List<HR_MAKLUMAT_PERIBADI> peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
                foreach (var item in jumlah)
                {
                    var jumlah1 = spg.PA_TRANSAKSI_PEMOTONGAN.Where(s => s.PA_KOD_PEMOTONGAN == "P0161" && s.PA_BULAN_POTONGAN == bulan && s.PA_TAHUN_POTONGAN == tahun && s.PA_NO_PEKERJA == item.PA_NO_PEKERJA).SingleOrDefault();
                    AgreementModels agree1 = new AgreementModels();
                    foreach (var item2 in peribadi)
                    {
                        if (item2.HR_NO_PEKERJA == item.PA_NO_PEKERJA)
                        {
                            agree1.HR_NAMA_PEKERJA = item2.HR_NAMA_PEKERJA;
                        }
                    }
                    if (jumlah1 == null)
                    {
                        var amount = item.PA_JUMLAH_CARUMAN + 0;
                        agree1.JUMLAH = amount.Value.ToString("0.00");
                    }
                    else
                    {
                        var amount = item.PA_JUMLAH_CARUMAN + jumlah1.PA_JUMLAH_PEMOTONGAN;
                        agree1.JUMLAH = amount.Value.ToString("0.00");
                    }
                    agree1.HR_NO_PEKERJA = item.PA_NO_PEKERJA;
                    agree.Add(agree1);
                }
            }
            if (jenis == "4")
            {
                var jumlah = spg.PA_TRANSAKSI_CARUMAN.Where(s => s.PA_KOD_CARUMAN == "C0035" && s.PA_BULAN_CARUMAN == bulan && s.PA_TAHUN_CARUMAN == tahun).ToList();
                List<HR_MAKLUMAT_PERIBADI> peribadi = db.HR_MAKLUMAT_PERIBADI.ToList();
                foreach (var item in jumlah)
                {
                    var jumlah1 = spg.PA_TRANSAKSI_PEMOTONGAN.Where(s => s.PA_KOD_PEMOTONGAN == "P0161" && s.PA_BULAN_POTONGAN == bulan && s.PA_TAHUN_POTONGAN == tahun && s.PA_NO_PEKERJA == item.PA_NO_PEKERJA).SingleOrDefault();
                    AgreementModels agree1 = new AgreementModels();
                    foreach (var item2 in peribadi)
                    {
                        if (item2.HR_NO_PEKERJA == item.PA_NO_PEKERJA)
                        {
                            agree1.HR_NAMA_PEKERJA = item2.HR_NAMA_PEKERJA;
                        }
                    }
                    if (jumlah1 == null)
                    {
                        var amount = item.PA_JUMLAH_CARUMAN + 0;
                        agree1.JUMLAH = amount.Value.ToString("0.00");
                    }
                    else
                    {
                        var amount = item.PA_JUMLAH_CARUMAN + jumlah1.PA_JUMLAH_PEMOTONGAN;
                        agree1.JUMLAH = amount.Value.ToString("0.00");
                    }
                    agree1.HR_NO_PEKERJA = item.PA_NO_PEKERJA;
                    agree.Add(agree1);
                }
            }
            return Json(agree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ElaunPotongan(string HR_PEKERJA)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumat = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR && s.HR_AKTIF_IND == "Y").ToList();
            if (maklumat.Count() <= 0)
            {
                maklumat = new List<HR_MAKLUMAT_ELAUN_POTONGAN>();
            }
            List<AgreementModels> agree = new List<AgreementModels>();
            var maklumatelaunpotongan = "";
            foreach (var item in maklumat)
            {
                AgreementModels agree1 = new AgreementModels();
                var elaun1 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD_ELAUN_POTONGAN);
                if (elaun1 != null)
                {
                    maklumatelaunpotongan = elaun1.HR_PENERANGAN_ELAUN;
                }
                var potongan1 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD_ELAUN_POTONGAN);
                if (potongan1 != null)
                {
                    maklumatelaunpotongan = potongan1.HR_PENERANGAN_POTONGAN;
                }
                var caruman1 = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == item.HR_KOD_ELAUN_POTONGAN);
                if (caruman1 != null)
                {
                    maklumatelaunpotongan = caruman1.HR_PENERANGAN_CARUMAN;
                }
                var gaji1 = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == item.HR_KOD_ELAUN_POTONGAN);
                if (gaji1 != null)
                {
                    maklumatelaunpotongan = gaji1.HR_PENERANGAN_UPAH;
                }
                agree1.KETERANGAN = maklumatelaunpotongan;
                agree1.KOD = item.HR_KOD_ELAUN_POTONGAN;
                agree1.JUMLAH = item.HR_JUMLAH.Value.ToString("0.00");

                agree.Add(agree1);
                ViewBag.elaunpotongan = agree;

            }
            return Json(agree, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SejarahPembayaran(string HR_PEKERJA, int? tahunbekerja, int? bulanbekerja, int? tahundibayar, int? bulandibayar, string bulantahuntunggakan)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_TAHUN_BEKERJA == tahunbekerja).ToList();
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar).ToList();
            List<AgreementModels> agree = new List<AgreementModels>();
            var maklumatelaunpotongan = "";
            foreach (var item in sambilan)
            {
                AgreementModels agree1 = new AgreementModels();
                var elaun1 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD);
                if (elaun1 != null)
                {
                    maklumatelaunpotongan = elaun1.HR_PENERANGAN_ELAUN;
                }
                var potongan1 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD);
                if (potongan1 != null)
                {
                    maklumatelaunpotongan = potongan1.HR_PENERANGAN_POTONGAN;
                }
                var caruman1 = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == item.HR_KOD);
                if (caruman1 != null)
                {
                    maklumatelaunpotongan = caruman1.HR_PENERANGAN_CARUMAN;
                }
                var gaji1 = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == item.HR_KOD);
                if (gaji1 != null)
                {
                    maklumatelaunpotongan = gaji1.HR_PENERANGAN_UPAH;
                }
                agree1.KETERANGAN = maklumatelaunpotongan;
                agree1.KOD = item.HR_KOD;
                agree1.JUMLAH = item.HR_JUMLAH.Value.ToString("0.00");
                agree1.TUNGGAKANIND = item.HR_TUNGGAKAN_IND;

                agree.Add(agree1);
            }
            return Json(agree.OrderBy(s => s.HR_KOD), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult BonusSambilanPartial(string bulantahundari, string bulantahunsehingga)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            DateTime kerjadari = Convert.ToDateTime("01/" + bulantahundari);
            DateTime kerjahingga = Convert.ToDateTime("01/" + bulantahunsehingga);
            DateTime mula = kerjadari;
            var max = 0;
            while (mula <= kerjahingga)
            {
                mula = mula.AddMonths(1);
                max++;
            }

            ViewBag.max = max;
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> column = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> transaksisambilandetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga).GroupBy(s => s.HR_NO_PEKERJA).Select(s => s.FirstOrDefault()).ToList();
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> kiratransaksi = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();
            var diff = ((kerjahingga.Year - kerjadari.Year) * 12) + kerjahingga.Month - kerjadari.Month;
            List<HR_MAKLUMAT_PERIBADI> peribadi = new List<HR_MAKLUMAT_PERIBADI>();
            List<TransaksiModels> transaksi = new List<TransaksiModels>();


            foreach (var item in transaksisambilandetail)
            {
                decimal? sum = 0;
                decimal? sum1 = 0;

                TransaksiModels transaksi1 = new TransaksiModels();
                transaksi1.peribadi = new HR_MAKLUMAT_PERIBADI();
                transaksi1.detail = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();

                kiratransaksi = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga).ToList();
                List<HR_MAKLUMAT_PERIBADI> peribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).ToList();

                foreach (var item1 in peribadi2)
                {
                    transaksi1.peribadi = item1;
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> detail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == item1.HR_NO_PEKERJA && s.HR_KOD == "GAJPS" && s.HR_TAHUN == kerjadari.Year).ToList();
                    if (detail.Count <= 0)
                    {
                        detail = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();
                    }
                    column.AddRange(detail);
                    transaksi1.detail = detail;

                }

                foreach (var item2 in kiratransaksi)
                {
                    sum = sum + item2.HR_JUMLAH;
                    sum1 = sum1 + item2.HR_JUMLAH;
                }

                var purata = sum1 / (diff + 1);
                var bonus = purata * 2;

                transaksi1.PURATA = purata.Value.ToString("0.00");
                transaksi1.TOTAL = sum.Value.ToString("0.00");
                transaksi1.BONUS = bonus.Value.ToString("0.00");
                transaksi.Add(transaksi1);
            }

            ViewBag.column = column;

            return PartialView("_BonusSambilanPartial", transaksi);
        }

        public ActionResult EditBonus(string id)
        {
            ViewBag.id = id;
            return PartialView("_EditBonus");
        }

        public ActionResult Muktamad(string bulantahundari, string bulantahunsehingga)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            DateTime kerjadari = Convert.ToDateTime("01/" + bulantahundari);
            DateTime kerjahingga = Convert.ToDateTime("01/" + bulantahunsehingga);

            List<HR_TRANSAKSI_SAMBILAN_DETAIL> transaksisambilandetail = 
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where
                (s => s.HR_KOD == "GAJPS" 
                && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari 
                && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga)
                .GroupBy(s => s.HR_NO_PEKERJA).
                Select(s => s.FirstOrDefault()).ToList();
            foreach (var item in transaksisambilandetail)
            {
                item.HR_MUKTAMAD = 1;
            }
            db.Entry(transaksisambilandetail).State = EntityState.Modified;
            db.SaveChanges();

            return View();
        }

        public ActionResult MuktamadAll(string bulantahundari, string bulantahunsehingga)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            string[] ctahunbulannotis = bulantahundari.Split('/');
            string[] ctahunbulanbonus = bulantahunsehingga.Split('/');

            int[] inserttahunbulannotis = Array.ConvertAll(ctahunbulannotis, int.Parse);
            int[] inserttahunbulanbonus = Array.ConvertAll(ctahunbulanbonus, int.Parse);

            for (int i = 0; i < inserttahunbulannotis.Length; i++)
            {
                var select1 = inserttahunbulannotis.ElementAt(i);
                var select2 = inserttahunbulanbonus.ElementAt(i);

                var bulankerja = inserttahunbulannotis[0];
                var tahunkerja = inserttahunbulannotis[1];
                var bulanbayar = inserttahunbulanbonus.ElementAt(0);
                var tahunbayar = inserttahunbulanbonus.ElementAt(1);

                List<HR_TRANSAKSI_SAMBILAN_DETAIL> gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_BULAN_BEKERJA == bulankerja && s.HR_TAHUN_BEKERJA == tahunkerja && s.HR_BULAN_DIBAYAR == bulanbayar && s.HR_TAHUN == tahunbayar).ToList();
                foreach (var item in gaji)
                {
                    item.HR_MUKTAMAD = 1;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ListTransaksiPartial", "PengurusanKakitanganSambilan", new { bulantahunbekerja = bulantahundari, bulantahundibayar = bulantahunsehingga, Message = ManageMessageId.Muktamad });
        }

        //public ActionResult ListTransaksi(string HR_PEKERJA)
        //{
        //	ApplicationDbContext db = new ApplicationDbContext();

        //	string[] ctahunbulannotis = bulantahundari.Split('/');
        //	string[] ctahunbulanbonus = bulantahunsehingga.Split('/');

        //	int[] inserttahunbulannotis = Array.ConvertAll(ctahunbulannotis, int.Parse);
        //	int[] inserttahunbulanbonus = Array.ConvertAll(ctahunbulanbonus, int.Parse);

        //	for (int i = 0; i < inserttahunbulannotis.Length; i++)
        //	{
        //		var bulantahunbulannotis = inserttahunbulannotis[0];
        //		var tahuntahunbulannotis = inserttahunbulannotis[1];
        //		var bulanhingga = inserttahunbulanbonus.ElementAt(0);
        //		var tahunhingga = inserttahunbulanbonus.ElementAt(1);

        //		var dari = "01/" + bulantahunbulannotis + "/" + tahuntahunbulannotis;
        //		var hingga = "01/" + bulanhingga + "/" + tahunhingga;
        //		var kerjadari = Convert.ToDateTime(dari);
        //		var kerjahingga = Convert.ToDateTime(hingga);

        //		List<HR_TRANSAKSI_SAMBILAN_DETAIL> transaksisambilandetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) >= kerjadari && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN)) <= kerjahingga).ToList();
        //		transaksisambilandetail.GroupBy(s => s.HR_NO_PEKERJA);

        //		//List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_TAHUN_BEKERJA == tahunbekerja).ToList();
        //		//List<HR_TRANSAKSI_SAMBILAN_DETAIL> sambilan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar).ToList();
        //		List<AgreementModels> agree = new List<AgreementModels>();
        //		var maklumatelaunpotongan = "";
        //		foreach (var item in transaksisambilandetail)
        //		{
        //			AgreementModels agree1 = new AgreementModels();
        //			var elaun1 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD);
        //			if (elaun1 != null)
        //			{
        //				maklumatelaunpotongan = elaun1.HR_PENERANGAN_ELAUN;
        //			}
        //			var potongan1 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD);
        //			if (potongan1 != null)
        //			{
        //				maklumatelaunpotongan = potongan1.HR_PENERANGAN_POTONGAN;
        //			}
        //			var caruman1 = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == item.HR_KOD);
        //			if (caruman1 != null)
        //			{
        //				maklumatelaunpotongan = caruman1.HR_PENERANGAN_CARUMAN;
        //			}
        //			var gaji1 = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == item.HR_KOD);
        //			if (gaji1 != null)
        //			{
        //				maklumatelaunpotongan = gaji1.HR_PENERANGAN_UPAH;
        //			}
        //			agree1.KETERANGAN = maklumatelaunpotongan;
        //			agree1.KOD = item.HR_KOD;
        //			agree1.JUMLAH = item.HR_JUMLAH.Value.ToString("0.00");
        //			agree1.TUNGGAKANIND = item.HR_TUNGGAKAN_IND;

        //			agree.Add(agree1);

        //		}
        //		return Json(agree.OrderBy(s => s.HR_KOD), JsonRequestBehavior.AllowGet);
        //	}
        //	return Json(JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult LaporanPDF(int bulan, string tahun)
        {
            string path_file = Server.MapPath(Url.Content("~/Content/template/"));

            var associativeArray = new Dictionary<int?, string>() { { 1, "JANUARI" }, { 2, "FEBRUARI" }, { 3, "MAC" }, { 4, "APRIL" }, { 5, "MEI" }, { 6, "JUN" }, { 7, "JULAI" }, { 8, "OGOS" }, { 9, "SEPTEMBER" }, { 10, "OKTOBER" }, { 11, "NOVEMBER" }, { 12, "DISEMBER" } };
            var Bulan = "";

            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    Bulan = m.Value;
                }

            }

            var tarikh = DateTime.Now.ToString("dd/MM/yyyy");
            var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
            templateEngine.Process(
                source: path_file + "template_gaji_pekerja_sambilan.docx",
                destination: path_file + "gaji_pekerja_sambilan.docx",
                    data: new
                    {
                        BULAN = Bulan,
                        BULAN1 = bulan,
                        TAHUN = tahun,
                        TARIKH = tarikh

                    });
            //Interop
            Application appWord = new Application();
            wordDocument = appWord.Documents.Open(@"C:\Users\RSA\Desktop\backup\espp\webapp\Content\template\gaji_pekerja_sambilan.docx");
            wordDocument.ExportAsFixedFormat(@"C:\Users\RSA\Desktop\backup\espp\webapp\Content\template\DocTo.pdf", WdExportFormat.wdExportFormatPDF);

            appWord.Quit();

            string FilePath = Server.MapPath("~/Content/template/DocTo.pdf");
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(FilePath);
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
            return File(FilePath, "application/pdf");
        }

        public Microsoft.Office.Interop.Word.Document wordDocument { get; set; }

        //11 laporan
        [HttpPost]
        public ActionResult PDFSenaraiPergerakanGaji(string jenis, int? bulan, int? tahun, string Command, string gajibonus, string sambilan, int? pk)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();

            List<HR_MAKLUMAT_KEWANGAN8> model = db.HR_MAKLUMAT_KEWANGAN8.Where(s => s.HR_BULAN == bulan && s.HR_TAHUN == tahun).ToList<HR_MAKLUMAT_KEWANGAN8>();

            var associativeArray = new Dictionary<int?, string>() { { 1, "JANUARI" }, { 2, "FEBRUARI" }, { 3, "MAC" }, { 4, "APRIL" }, { 5, "MEI" }, { 6, "JUN" }, { 7, "JULAI" }, { 8, "OGOS" }, { 9, "SEPTEMBER" }, { 10, "OKTOBER" }, { 11, "NOVEMBER" }, { 12, "DISEMBER" } };
            var Bulan = "";
            foreach (var m in associativeArray)
            {
                if (bulan == m.Key)
                {
                    Bulan = m.Value;
                }
            }
            string[] listint = { "1", "2", "3", "4" };
            if (listint.Contains(jenis))
            {
                if (Command == "Excel")
                {
                    int _bulan = Convert.ToInt32(bulan);
                    int _tahun = Convert.ToInt32(tahun);
                    IWorkbook workbook = BorangAReport.GetReport(_bulan, _tahun, jenis);

                    // code to create workbook 
                    using (var exportData = new MemoryStream())
                    {
                        workbook.Write(exportData);
                        string saveAsFileName = string.Format("LaporanExport-{0:d}.xlsx", DateTime.Now).Replace("/", "-");

                        byte[] bytes = exportData.ToArray();
                        return File(bytes, "application/vnd.ms-excel", saveAsFileName);
                    }
                }
                else if (Command == "Pdf")
                {
                    //1234
                }
            }
            if (jenis == "5") //gaji sambilan
            {
                if (Command == "Pdf")
                {
                    return GetGajiSambilan(bulan, Bulan, tahun);
                }
            }
            if (jenis == "6") //Gaji sukan
            {
                if (Command == "Pdf")
                {
                    return GetGajiSukan(bulan, Bulan, tahun);
                }
            }
            if (jenis == "7") //KSDK
            {
                if (Command == "Pdf")
                {
                    return GetPDFKsdk(bulan, Bulan, tahun);
                }
            }
            if (jenis == "8") //Kwsp sambilan
            {
                if (Command == "Pdf")
                {
                    return GetPDFKwspSambilan(bulan, Bulan, tahun);
                }
            }
            if (jenis == "9") //kwsp sambilan sukan
            {
                if (Command == "Pdf")
                {
                    return GetPDFKwspSambilanSukan(bulan, Bulan, tahun);
                }
            }
            if (jenis == "10") //perkeso sambilan
            {
                if (Command == "Pdf")
                {
                    return GetPDFPerkesoSambilan(bulan, Bulan, tahun);
                }
                else if (Command == "Text")
                {
                    return GetTextPerkesoSambilan(bulan, Bulan, tahun);
                }
            }
            if (jenis == "11") //perkeso sambilan sukan
            {
                if (Command == "Pdf")
                {
                    return GetPDFPerkesoSambilanSukan(bulan, Bulan, tahun);
                }
                else if (Command == "Text")
                {
                    return GetTextPerkesoSambilanSukan(bulan, Bulan, tahun);
                }
            }
            var output1 = new MemoryStream();
            return new FileStreamResult(output1, "application/pdf");
        }

        //Fixing Gaji Sambilan
        [NonAction]
        private ActionResult GetGajiSambilan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //PageSejarahModel gajiModel = new PageSejarahModel();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;
            int count = 1;

            var html = "<html><head>";
            html += "<title>Senarai Pengiraan Gaji</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            var date = Convert.ToDateTime(datebulan);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                .Where(s => s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            var gaji = listbulan.OrderBy(s => s.HR_NO_PEKERJA);

            //int bulanInt = Convert.ToInt32(bulan);
            int tahunInt = Convert.ToInt32(tahun);

            foreach (var item in gaji)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).FirstOrDefault();
                //HR_TRANSAKSI_SAMBILAN_DETAIL listgaji = 
                //    db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().FirstOrDefault(s => s.HR_KOD == "GAJPS" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));
                //HR_TRANSAKSI_SAMBILAN_DETAIL listovertime = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                //    .FirstOrDefault(s => s.HR_KOD == "E0164" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));
                //HR_TRANSAKSI_SAMBILAN_DETAIL listkwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                //    .FirstOrDefault(s => s.HR_KOD == "P0035" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));

                //List<HR_TRANSAKSI_SAMBILAN_DETAIL> listcola = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                //    .Where(s => s.HR_KOD == "E0234" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
                //List<HR_TRANSAKSI_SAMBILAN_DETAIL> listksdk = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
                //    .Where(s => s.HR_KOD == "P0015" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();

                //if (listovertime == null)
                //{
                //    listovertime = new HR_TRANSAKSI_SAMBILAN_DETAIL();
                //    listovertime.HR_JAM_HARI = 0;
                //}
                //if (listkwsp == null)
                //{
                //    listkwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL();
                //    listkwsp.HR_JAM_HARI = 0;
                //    listkwsp.HR_JUMLAH = 0;
                //}
                //if (pekerjaan.HR_GAJI_POKOK == null)
                //{
                //    pekerjaan.HR_GAJI_POKOK = 0;
                //}

                //var gajisehari = pekerjaan.HR_GAJI_POKOK / 23;
                //var sebulankerja = gajisehari * listgaji.HR_JAM_HARI;
                //var rate = (gajisehari * listgaji.HR_JAM_HARI * 12 / 2504);
                //var totalovertime = rate * listovertime.HR_JAM_HARI;
                //var subtotal = totalovertime + sebulankerja;
                //var gajibersih = subtotal - listkwsp.HR_JUMLAH;

                //add listSocso
                //decimal gajiPokok = listgaji.HR_JUMLAH.Value;
                //decimal hariBekerja = listgaji.HR_JAM_HARI.Value; //hari ambik dari ListGaji
                //decimal jamBekerja = listovertime.HR_JAM_HARI.Value; //jam ambik dari listOvertime
                //decimal elaunOT = listovertime.HR_JUMLAH.Value;
                //decimal listSocso = PageSejarahModel.GetPotonganSocso(db, gajiPokok, elaunOT);

                //kira gaji
                //dapatkan Transaksi Sambilan Detail. ada banyak kod: C0020, E0064, E0164, E0234, GAJPS, P0015, P0035
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> userTransaksiDetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA
                    && s.HR_BULAN_BEKERJA == item.HR_BULAN_BEKERJA
                    && s.HR_BULAN_DIBAYAR == item.HR_BULAN_DIBAYAR
                    && s.HR_TAHUN == item.HR_TAHUN
                    && s.HR_TAHUN_BEKERJA == item.HR_TAHUN_BEKERJA).ToList();

                //kalau ada tranaksi, ambik gaji dan potongan dari tranasksi
                HR_TRANSAKSI_SAMBILAN_DETAIL gajiFromDB = 
                    userTransaksiDetail.SingleOrDefault(s => s.HR_KOD_IND == "G");
                var hariBekerja = gajiFromDB.HR_JAM_HARI.Value;
                var gajiSehari = PageSejarahModel.GetGajiSehari(db, item.HR_NO_PEKERJA);

                var gajiPokok = gajiFromDB.HR_JUMLAH.Value;
                var gajiPer3 = (gajiFromDB.HR_JUMLAH / 3).Value.ToString("0.00");
                var rate = (gajiSehari * hariBekerja * 12 / 2504);

                //elaun
                List<string> listElaunKa = PageSejarahModel.ListElaunKa;
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunka = userTransaksiDetail
                       .Where(s => listElaunKa.Contains(s.HR_KOD)).ToList();
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunot = userTransaksiDetail
                    .Where(s => s.HR_KOD == "E0164").ToList();
                var jamBekerja = elaunot.FirstOrDefault().HR_JAM_HARI.Value;
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain = userTransaksiDetail
                    .Where(s => s.HR_KOD_IND == "E"
                    && (!listElaunKa.Contains(s.HR_KOD) && s.HR_KOD != "E0164")).ToList();

                //potongan
                decimal potonganSocso = PageSejarahModel.GetPotonganSocso(db, gajiPokok, elaunot.Sum(s => s.HR_JUMLAH).Value);
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganksdk = userTransaksiDetail
                      .Where(s => s.HR_KOD == "P0015").ToList();
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganlain = userTransaksiDetail
                    .Where(s => s.HR_KOD_IND == "P" && (s.HR_KOD != "P0015" && s.HR_KOD != "P0035")).ToList();
                HR_MAKLUMAT_ELAUN_POTONGAN potonganKWSP = 
                    PageSejarahModel.GetPotonganKWSP(db, item.HR_NO_PEKERJA, gajiPokok);
                
                var totalElaunka = elaunka.Sum(s => s.HR_JUMLAH);
                var totalElaunLain = elaunlain.Sum(s => s.HR_JUMLAH);
                var totalElaunot = elaunot.Sum(s => s.HR_JUMLAH);
                var totalElaunkaSebulan = totalElaunka * hariBekerja;
                var totalElaunLainSebulan = totalElaunLain * hariBekerja;

                decimal gajikasar = PageSejarahModel.GetGajiKasar
                    (gajiPokok,
                    totalElaunka,
                    totalElaunLain,
                    totalElaunot.Value, hariBekerja);
                var gajiSebelumKWSP = gajikasar
                  - potonganSocso
                  - potonganksdk.Sum(s => s.HR_JUMLAH)
                  - potonganlain.Sum(s => s.HR_JUMLAH);

                //gaji bersih = gaji pokok + elaun - potongan
                var gajiBersih = gajikasar
                    - potonganSocso
                    - potonganKWSP.HR_JUMLAH
                    - potonganksdk.Sum(s => s.HR_JUMLAH)
                    - potonganlain.Sum(s => s.HR_JUMLAH);
                
                html += "<table width='100%'>";
                html += "<tr>";
                html += "<td font-size: '7%' width='2%'>Bil</td><td font-size: '7%'>Nama</td><td font-size: 7%' width='7%'>No. Pekerja</td><td font-size: 7%' width='10%'>No. K/P</td><td font-size: 7%' width='7%'>Hari Bekerja</td>"
                        + "<td font-size: '7%'>Bulan Bekerja</td><td font-size: '7%'>Tahun Bekerja</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td font-size: 7%' width='2%'>" + count + "</td><td font-size: 7%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_NO_PEKERJA + "</td><td font-size: 7%' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_JAM_HARI + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_BULAN_BEKERJA + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_TAHUN_BEKERJA + "</td>";
                html += "</tr>";
                html += "</table>";

                html += "<table width='100%'>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "</table>";

                html += "<table width='100%'>";
                //gaji + elaun
                html += "<tr><td> Gaji Pokok </td><td>=> RM" + gajiSehari.ToString("0.00") 
                    + " x " + hariBekerja + " </td><td> RM" + gajiPokok.ToString("0.00") + " </td></tr>";
                html += "<tr><td> EKA </td><td>=> RM" + totalElaunka.Value.ToString("0.00") + " x " + hariBekerja + " </td><td> RM" + totalElaunkaSebulan.Value.ToString("0.00") +"</td></tr>";
                //html += "<tr><td> COLA </td><td>=> 6.52 x 5.4345 x 0.00 </td><td> RM0.00 </td></tr>";
                html += "<tr><td> COLA </td><td>=> RM" + totalElaunLain.Value.ToString("0.00")  + " x " + hariBekerja + "</td><td> RM" + totalElaunLainSebulan.Value.ToString("0.00") +"</td></tr>";
                //html += "<tr><td> Kerja L/Masa</td><td>=> " + rate.ToString("0.00") + " x " + jamBekerja + " </td><td> </td></tr>";
                html += "<tr><td> Kerja L/Masa Yang Dibenarkan 1/3 (gaji pokok)</td><td>=> RM" + rate.ToString("0.00") + " x " + jamBekerja + " </td><td> RM" + totalElaunot.Value.ToString("0.00") + " </td></tr>";
                html += "<tr><td> &nbsp;</td></tr>";
                html += "<tr><td></td><td> Jumlah </td><td style='border-top: 1px solid;border-bottom: 1px solid'> RM" + gajikasar.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                //potongan
                if (potonganKWSP != null)
                {
                    html += "<tr><td>Tolak KWSP Pekerja</td><td>=> RM" + potonganKWSP.HR_JUMLAH.Value.ToString("0.00") + " </td><td> RM" + potonganKWSP.HR_JUMLAH.Value.ToString("0.00") + "</td></tr>";
                }
                html += "<tr><td>Tolak SOCSO Pekerja</td><td>=> RM" + potonganSocso.ToString("0.00") + " </td><td> RM" + potonganSocso.ToString("0.00") + "</td></tr>";
                //html += "<tr><td>&nbsp;</td></tr>";
                //html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td>Tolak Kelab Sukan</td><td>=> </td><td> RM"+ potonganksdk.Sum(s => s.HR_JUMLAH).Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>Tolak Lain(eg.sewa)</td><td>=> </td><td> RM" + potonganlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00") +"</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td></td><td>Gaji Bersih Diterima</td><td style='border-top: 1px solid;border-bottom: 1px solid'> RM" + gajiBersih.Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td></td><td>Jumlah Besar Gaji Bersih Diterima</td><td style='border-top: 1px solid;border-bottom: double'> RM" + gajiBersih.Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "</table>";

                html += "<table width='100%'>";
                html += "<tr><td style='border-top: dashed;'>&nbsp;</td></tr>";
                html += "</table>";

                i++;
                count++;
                if (i >= 2)
                {
                    html += "<div style='page-break-before:always'>&nbsp;</div>";
                    i = 0;
                }
            }

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI PENGIRAAN GAJI PEKERJA SAMBILAN UNTUK BULAN \n" + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetGajiSukan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;
            int count = 1;

            var html = "<html><head>";
            html += "<title>Senarai Pengiraan Gaji</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            var date = Convert.ToDateTime(datebulan);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "GAJPS" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            var gaji = listbulan.OrderBy(s => s.HR_NO_PEKERJA);

            foreach (var item in gaji)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                HR_TRANSAKSI_SAMBILAN_DETAIL listgaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().SingleOrDefault(s => s.HR_KOD == "GAJPS" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));
                HR_TRANSAKSI_SAMBILAN_DETAIL listovertime = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().SingleOrDefault(s => s.HR_KOD == "E0164" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));
                HR_TRANSAKSI_SAMBILAN_DETAIL listkwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().SingleOrDefault(s => s.HR_KOD == "P0035" && s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> listcola = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "E0234" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
                HR_TRANSAKSI_SAMBILAN_DETAIL listksdk = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().SingleOrDefault(s => s.HR_KOD == "P0015" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date));

                if (listovertime == null)
                {
                    listovertime = new HR_TRANSAKSI_SAMBILAN_DETAIL();
                    listovertime.HR_JAM_HARI = 0;
                }
                if (listkwsp == null)
                {
                    listkwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL();
                    listkwsp.HR_JAM_HARI = 0;
                    listkwsp.HR_JUMLAH = 0;
                }
                if (pekerjaan.HR_GAJI_POKOK == null)
                {
                    pekerjaan.HR_GAJI_POKOK = 0;
                }

                var gajisehari = pekerjaan.HR_GAJI_POKOK / 23;
                var sebulankerja = gajisehari * listgaji.HR_JAM_HARI;
                var rate = (gajisehari * listgaji.HR_JAM_HARI * 12 / 2504);
                var totalovertime = rate * listovertime.HR_JAM_HARI;
                var subtotal = totalovertime + sebulankerja;
                decimal? gajibersih = 0;
                if (listksdk != null)
                {
                    gajibersih = subtotal - listkwsp.HR_JUMLAH - listksdk.HR_JUMLAH;
                }

                html += "<table width='100%'>";
                html += "<tr>";
                html += "<td font-size: '7%' width='2%'>Bil</td><td font-size: '7%'>Nama</td><td font-size: 7%' width='7%'>No. Pekerja</td><td font-size: 7%' width='10%'>No. K/P</td><td font-size: 7%' width='7%'>Hari Bekerja</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td font-size: 7%' width='2%'>" + count + "</td><td font-size: 7%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_NO_PEKERJA + "</td><td font-size: 7%' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td font-size: 7%' width='7%'>" + item.HR_JAM_HARI + "</td>";
                html += "</tr>";
                html += "</table>";

                html += "<table width='100%'>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "</table>";

                html += "<table width='100%'>";
                html += "<tr><td> Gaji Pokok </td><td>=> RM" + gajisehari.Value.ToString("0.00") + " x " + listgaji.HR_JAM_HARI + " </td><td> RM" + sebulankerja.Value.ToString("0.00") + " </td></tr>";
                html += "<tr><td> EKA </td><td>=> </td><td></td></tr>";
                html += "<tr><td> COLA </td><td>=> 6.52 x 5.4345 x 0.00 </td><td> RM0.00 </td></tr>";
                html += "<tr><td> Kerja L/Masa</td><td>=> " + rate.Value.ToString("0.00") + " x " + listovertime.HR_JAM_HARI + " </td><td> RM" + totalovertime.Value.ToString("0.00") + " </td></tr>";
                html += "<tr><td> Kerja L/Masa Yang Dibenarkan 1/3 (gaji pokok)</td><td>=> " + rate.Value.ToString("0.00") + " x " + listovertime.HR_JAM_HARI + " </td><td> RM" + totalovertime.Value.ToString("0.00") + " </td></tr>";
                html += "<tr><td> &nbsp;</td></tr>";
                html += "<tr><td></td><td> Jumlah </td><td style='border-top: 1px solid;border-bottom: 1px solid'> RM" + subtotal.Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                if (listkwsp != null)
                {
                    html += "<tr><td>Tolak KWSP Pekerja</td><td>=> " + listkwsp.HR_JUMLAH.Value.ToString("0.00") + " </td><td> RM" + listkwsp.HR_JUMLAH.Value.ToString("0.00") + "</td></tr>";
                }
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                if (listksdk == null)
                {
                    html += "<tr><td>Tolak Kelab Sukan</td><td>=> RM0 </td><td></td></tr>";
                }
                if (listksdk != null)
                {
                    html += "<tr><td>Tolak Kelab Sukan</td><td>=> RM" + listksdk.HR_JUMLAH.Value.ToString("0.00") + "</td><td></td></tr>";
                }
                html += "<tr><td>Tolak Lain(eg.sewa)</td><td>=> </td><td></td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td></td><td>Gaji Bersih Diterima</td><td style='border-top: 1px solid;border-bottom: 1px solid'> RM" + gajibersih.Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td></td><td>Jumlah Besar Gaji Bersih Diterima</td><td style='border-top: 1px solid;border-bottom: double'>" + gajibersih.Value.ToString("0.00") + "</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "<tr><td>&nbsp;</td></tr>";
                html += "</table>";

                html += "<table width='100%'>";
                html += "<tr><td style='border-top: dashed;'>&nbsp;</td></tr>";
                html += "</table>";

                i++;
                count++;
                if (i >= 2)
                {
                    html += "<div style='page-break-before:always'>&nbsp;</div>";
                    i = 0;
                }
            }

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI PENGIRAAN GAJI PEKERJA SUKAN UNTUK BULAN \n" + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetPDFKsdk(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;

            var html = "<html><head>";
            html += "<title>Senarai Potongan Sukan</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            var date = Convert.ToDateTime(datebulan);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "P0015" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            var maklumat = listbulan.OrderBy(s => s.HR_NO_PEKERJA);

            html += "<table width='100%'>";
            html += "<tr><td style='font-size:11px;'>Bil.</td><td style='font-size:11px;'>Nama</td><td style='font-size:11px;'>No. K/P</td><td style='font-size:11px;'>Jumlah</td></tr>";

            foreach (var item in listbulan)
            {
                i++;
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();

                html += "<tr>";
                html += "<td align='left' width='3%' style='font-size:11px;'>" + i + "</td>";
                html += "<td align='left' width='70%' style='font-size:11px;'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "<td align='left' width='20%' style='font-size:11px;'>" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td align='left' width='7%' style='font-size:11px;'>1.50</td>";
                html += "</tr>";
            }
            html += "</table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table>";
            html += "<tr><td align='left' style='font-size:11px;'>(MOHAMAD ROSNANI BIN HJ. HAMID,PPT)</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Timbalan Pengarah (Sumber Manusia),</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>b.p. Datuk Bandar,</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Majlis Bandaraya Petaling Jaya</td></tr>";
            html += "</table>";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("SENARAI POTONGAN KELAB SUKAN DAN KEBUDAYAAN UNTUK BULAN DIBAYAR " + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetPDFKwspSambilan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;

            var html = "<html><head>";
            html += "<title>Senarai KWSP Sambilan</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            var date = Convert.ToDateTime(datebulan);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "C0020" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            var maklumat = listbulan.OrderBy(s => s.HR_NO_PEKERJA);
            List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
            decimal? sum = 0;
            decimal? sum1 = 0;
            decimal? sum2 = 0;
            decimal? sum3 = 0;

            html += "<table width='100%'>";
            html += "<tr>";
            html += "<td style='font-size:11px;'><strong>BIL</strong></td>";
            html += "<td style='font-size:11px;'><strong>NAMA</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. K/P</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. KWSP</strong></td>";
            html += "<td style='font-size:11px;'><strong>GAJI POKOK (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN PEKERJA (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN MAJIKAN (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>JUMLAH (RM)</strong></td>";
            html += "</tr>";
            foreach (var item in maklumat)
            {
                i++;
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                if (pekerjaan.HR_TARAF_JAWATAN == "N")
                {
                    html += "<tr>";
                    html += "<td style='font-size:11px;' align='left' width='3%'>" + i + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='20%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_NO_KWSP + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_GAJI_POKOK.Value.ToString("0.00") + "</td>";
                    foreach (var kwsp in listkwsp)
                    {
                        if (pekerjaan.HR_GAJI_POKOK >= kwsp.HR_UPAH_DARI && pekerjaan.HR_GAJI_POKOK <= kwsp.HR_UPAH_HINGGA)
                        {
                            html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_PEKERJA.ToString("0.00") + "</td>";
                            html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_MAJIKAN.ToString("0.00") + "</td>";
                            var tambah = kwsp.HR_CARUMAN_MAJIKAN + kwsp.HR_CARUMAN_PEKERJA;
                            html += "<td style='font-size:11px;' align='left'  width='10%'>" + tambah.ToString("0.00") + "</td>";
                            sum1 = sum1 + kwsp.HR_CARUMAN_PEKERJA;
                            sum2 = sum2 + kwsp.HR_CARUMAN_MAJIKAN;
                        }
                    }
                    sum = sum + pekerjaan.HR_GAJI_POKOK;
                    sum3 = sum1 + sum2;
                    html += "</tr>";
                }
            }
            html += "<tr><td></td><td style='font-size:11px;'><strong>JUMLAH KESELURUHAN</strong></td><td></td><td></td><td style='font-size:11px;'><strong>" + sum.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum1.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum2.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum3.Value.ToString("0.00") + "</strong></td></tr>";
            html += "</table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table>";
            html += "<tr><td align='left' style='font-size:11px;'>(MOHAMAD ROSNANI BIN HJ. HAMID,PPT)</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Timbalan Pengarah (Sumber Manusia),</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>b.p. Datuk Bandar,</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Majlis Bandaraya Petaling Jaya</td></tr>";
            html += "</table>";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA PENYATA CARUMAN KWSP BAGI JAWATAN PEKERJA SAMBILAN BAGI BULAN \n" + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetPDFKwspSambilanSukan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 1;

            var html = "<html><head>";
            html += "<title>Senarai KWSP Sambilan Sukan</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            var date = Convert.ToDateTime(datebulan);
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "C0020" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            var maklumat = listbulan.OrderBy(s => s.HR_NO_PEKERJA);
            List<HR_KWSP> listkwsp = db.HR_KWSP.ToList();
            decimal? sum = 0;
            decimal? sum1 = 0;
            decimal? sum2 = 0;
            decimal? sum3 = 0;

            html += "<table width='100%'>";
            html += "<tr>";
            html += "<td style='font-size:11px;'><strong>BIL</strong></td>";
            html += "<td style='font-size:11px;'><strong>NAMA</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. K/P</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. KWSP</strong></td>";
            html += "<td style='font-size:11px;'><strong>GAJI POKOK (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN PEKERJA (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN MAJIKAN (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>JUMLAH (RM)</strong></td>";
            html += "</tr>";
            foreach (var item in maklumat)
            {
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable().Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA).SingleOrDefault();
                if (pekerjaan.HR_TARAF_JAWATAN == "A")
                {
                    html += "<tr>";
                    html += "<td style='font-size:11px;' align='left' width='3%'>" + i + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='20%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_NO_KWSP + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_GAJI_POKOK.Value.ToString("0.00") + "</td>";
                    foreach (var kwsp in listkwsp)
                    {
                        if (pekerjaan.HR_GAJI_POKOK >= kwsp.HR_UPAH_DARI && pekerjaan.HR_GAJI_POKOK <= kwsp.HR_UPAH_HINGGA)
                        {
                            html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_PEKERJA.ToString("0.00") + "</td>";
                            html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_MAJIKAN.ToString("0.00") + "</td>";
                            var tambah = kwsp.HR_CARUMAN_MAJIKAN + kwsp.HR_CARUMAN_PEKERJA;
                            html += "<td style='font-size:11px;' align='left'  width='10%'>" + tambah.ToString("0.00") + "</td>";
                            sum1 = sum1 + kwsp.HR_CARUMAN_PEKERJA;
                            sum2 = sum2 + kwsp.HR_CARUMAN_MAJIKAN;
                        }
                    }
                    sum = sum + pekerjaan.HR_GAJI_POKOK;
                    sum3 = sum1 + sum2;
                    html += "</tr>";
                    i++;
                }
            }
            html += "<tr><td></td><td style='font-size:11px;'><strong>JUMLAH KESELURUHAN</strong></td><td></td><td></td><td style='font-size:11px;'><strong>" + sum.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum1.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum2.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum3.Value.ToString("0.00") + "</strong></td></tr>";
            html += "</table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table>";
            html += "<tr><td align='left' style='font-size:11px;'>(MOHAMAD ROSNANI BIN HJ. HAMID,PPT)</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Timbalan Pengarah (Sumber Manusia),</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>b.p. Datuk Bandar,</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Majlis Bandaraya Petaling Jaya</td></tr>";
            html += "</table>";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA PENYATA CARUMAN KWSP BAGI JAWATAN PEKERJA SUKAN BAGI BULAN \n" + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetPDFPerkesoSambilan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;

            var html = "<html><head>";
            html += "<title>Senarai Perkeso Sambilan</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            //var datebulan = "01/" + bulan + "/" + tahun;
            //var date = Convert.ToDateTime(datebulan);
            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable().Where(s => s.HR_KOD == "P0160" && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date)).ToList();
            //var maklumat = listbulan.OrderBy(s => s.HR_NO_PEKERJA);
            //List<HR_SOCSO> listkwsp = db.HR_SOCSO.ToList();

            SPGContext spgDb = new SPGContext();
            string[] kwspSambilan = new string[]
            {
                "P0160",
                "P0163"
            };

            //N = sambilan, A = sukan
            List<string> list_pekerja = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_TARAF_JAWATAN == "N").Select(x => x.HR_NO_PEKERJA).ToList();

            var potongList = spgDb.PA_TRANSAKSI_PEMOTONGAN
                .Where(s => s.PA_BULAN_POTONGAN == bulan
                && s.PA_TAHUN_POTONGAN == tahun
                && kwspSambilan.Contains(s.PA_KOD_PEMOTONGAN)
                && list_pekerja.Contains(s.PA_NO_PEKERJA))
                .GroupBy(s => new { s.PA_NO_PEKERJA, s.PA_BULAN_POTONGAN, s.PA_TAHUN_POTONGAN })
                .Select(g => new
                {
                    PA_No_Pekerja = g.Key.PA_NO_PEKERJA,
                    PA_Jumlah_Pemontongan = g.Sum(x => x.PA_JUMLAH_PEMOTONGAN),
                    PA_Bulan = g.Key.PA_BULAN_POTONGAN,
                    PA_Tahun = g.Key.PA_TAHUN_POTONGAN
                })
                .ToList();

            decimal? sum = 0;
            decimal? sum1 = 0;
            decimal? sum2 = 0;
            decimal? sum3 = 0;

            html += "<table width='100%'>";
            html += "<tr>";
            html += "<td style='font-size:11px;'><strong>BIL</strong></td>";
            html += "<td style='font-size:11px;'><strong>NAMA</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. K/P</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. PERKESO</strong></td>";
            html += "<td style='font-size:11px;'><strong>GAJI POKOK (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN PEKERJA (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN MAJIKAN (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>JUMLAH (RM)</strong></td>";
            html += "</tr>";
            foreach (var item in potongList)
            {
                i++;
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).SingleOrDefault();
                if (pekerjaan.HR_TARAF_JAWATAN == "N")
                {

                    html += "<tr>";
                    html += "<td style='font-size:11px;' align='left' width='3%'>" + i + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='20%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_NO_SOCSO + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_GAJI_POKOK.Value.ToString("0.00") + "</td>";

                    HR_SOCSO kwsp = db.HR_SOCSO
                   .Where(k => pekerjaan.HR_GAJI_POKOK >= k.HR_GAJI_DARI
                    && pekerjaan.HR_GAJI_POKOK <= k.HR_GAJI_HINGGA).FirstOrDefault();
                    if(kwsp != null)
                    {
                        html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_PEKERJA.ToString("0.00") + "</td>";
                        html += "<td style='font-size:11px;' align='left' width='10%'>" + kwsp.HR_CARUMAN_MAJIKAN.ToString("0.00") + "</td>";
                        var tambah = kwsp.HR_CARUMAN_MAJIKAN + kwsp.HR_CARUMAN_PEKERJA;
                        html += "<td style='font-size:11px;' align='left'  width='10%'>" + tambah.ToString("0.00") + "</td>";
                        sum1 = sum1 + kwsp.HR_CARUMAN_PEKERJA;
                        sum2 = sum2 + kwsp.HR_CARUMAN_MAJIKAN;
                    }

                    sum = sum + pekerjaan.HR_GAJI_POKOK;
                    sum3 = sum1 + sum2;
                    html += "</tr>";
                }
            }
            html += "<tr><td></td><td style='font-size:11px;'><strong>JUMLAH KESELURUHAN</strong></td><td></td><td></td><td style='font-size:11px;'><strong>" + sum.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum1.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum2.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum3.Value.ToString("0.00") + "</strong></td></tr>";
            html += "</table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table>";
            html += "<tr><td align='left' style='font-size:11px;'>(MOHAMAD ROSNANI BIN HJ. HAMID,PPT)</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Timbalan Pengarah (Sumber Manusia),</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>b.p. Datuk Bandar,</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Majlis Bandaraya Petaling Jaya</td></tr>";
            html += "</table>";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");


                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA PENYATA CARUMAN PERKESO BAGI JAWATAN PEKERJA SAMBILAN BAGI BULAN \n" + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetPDFPerkesoSambilanSukan(int? bulan, string Bulan, int? tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 28, 28);
            int i = 0;

            var html = "<html><head>";
            html += "<title>Senarai Perkeso Sambilan Sukan</title><link rel='shortcut icon' href='~/Content/img/lpktn.jpeg' type='image/x-icon'/></head>";
            html += "<body>";

            var datebulan = "01/" + bulan + "/" + tahun;
            //var date = Convert.ToDateTime(datebulan);
            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> listbulan = 
            //    db.HR_TRANSAKSI_SAMBILAN_DETAIL.AsEnumerable()
            //    .Where(s => s.HR_KOD == "P0161" || s.HR_KOD == "C0034" 
            //    && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) >= date) 
            //    && (Convert.ToDateTime("01/" + s.HR_BULAN_DIBAYAR + "/" + s.HR_TAHUN) <= date))
            //    .ToList();
            //var maklumat = listbulan.OrderBy(s => s.HR_NO_PEKERJA);

            decimal? sum = 0;
            decimal? sum1 = 0;
            decimal? sum2 = 0;
            decimal? sum3 = 0;

            SPGContext spgDb = new SPGContext();
            string[] kwspSambilan = new string[]
            {
                "P0160",
                "P0163"
            };

            //N = sambilan, A = sukan
            List<string> list_pekerja = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_TARAF_JAWATAN == "A").Select(x => x.HR_NO_PEKERJA).ToList();

            var potongList = spgDb.PA_TRANSAKSI_PEMOTONGAN
                .Where(s => s.PA_BULAN_POTONGAN == bulan
                && s.PA_TAHUN_POTONGAN == tahun
                && kwspSambilan.Contains(s.PA_KOD_PEMOTONGAN)
                && list_pekerja.Contains(s.PA_NO_PEKERJA))
                .GroupBy(s => new { s.PA_NO_PEKERJA, s.PA_BULAN_POTONGAN, s.PA_TAHUN_POTONGAN })
                .Select(g => new
                {
                    PA_No_Pekerja = g.Key.PA_NO_PEKERJA,
                    PA_Jumlah_Pemontongan = g.Sum(x => x.PA_JUMLAH_PEMOTONGAN),
                    PA_Bulan = g.Key.PA_BULAN_POTONGAN,
                    PA_Tahun = g.Key.PA_TAHUN_POTONGAN
                })
                .ToList();

            html += "<table width='100%'>";
            html += "<tr>";
            html += "<td style='font-size:11px;'><strong>BIL</strong></td>";
            html += "<td style='font-size:11px;'><strong>NAMA</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. K/P</strong></td>";
            html += "<td style='font-size:11px;'><strong>NO. PERKESO</strong></td>";
            html += "<td style='font-size:11px;'><strong>GAJI POKOK (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN PEKERJA (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>CARUMAN MAJIKAN (RM)</strong></td>";
            html += "<td style='font-size:11px;'><strong>JUMLAH (RM)</strong></td>";
            html += "</tr>";
            foreach (var item in potongList)
            {
                i++;
                HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).SingleOrDefault();
                HR_MAKLUMAT_PEKERJAAN pekerjaan = db.HR_MAKLUMAT_PEKERJAAN.AsEnumerable()
                    .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).SingleOrDefault();
                html += "<tr>";
                html += "<td style='font-size:11px;' align='left' width='3%'>" + i + "</td>";
                html += "<td style='font-size:11px;' align='left' width='20%'>" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "<td style='font-size:11px;' align='left' width='10%'>" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_NO_SOCSO + "</td>";
                html += "<td style='font-size:11px;' align='left' width='10%'>" + pekerjaan.HR_GAJI_POKOK.Value.ToString("0.00") + "</td>";

                HR_SOCSO kwsp = db.HR_SOCSO
                    .Where(k => pekerjaan.HR_GAJI_POKOK >= k.HR_GAJI_DARI
                    && pekerjaan.HR_GAJI_POKOK <= k.HR_GAJI_HINGGA).FirstOrDefault();

                if(kwsp != null)
                {
                    html += "<td style='font-size:11px;' align='left' width='10%'>"
                          + kwsp.HR_CARUMAN_PEKERJA.ToString("0.00") + "</td>";
                    html += "<td style='font-size:11px;' align='left' width='10%'>"
                        + kwsp.HR_CARUMAN_MAJIKAN.ToString("0.00") + "</td>";
                    var tambah = kwsp.HR_CARUMAN_MAJIKAN + kwsp.HR_CARUMAN_PEKERJA;
                    html += "<td style='font-size:11px;' align='left'  width='10%'>"
                        + tambah.ToString("0.00") + "</td>";
                    sum1 = sum1 + kwsp.HR_CARUMAN_PEKERJA;
                    sum2 = sum2 + kwsp.HR_CARUMAN_MAJIKAN;
                }

                sum = sum + pekerjaan.HR_GAJI_POKOK;
                sum3 = sum1 + sum2;
                html += "</tr>";

            }
            html += "<tr><td></td><td style='font-size:11px;'><strong>JUMLAH KESELURUHAN</strong></td><td></td><td></td><td style='font-size:11px;'><strong>" + sum.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum1.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum2.Value.ToString("0.00") + "</strong></td><td style='font-size:11px;'><strong>" + sum3.Value.ToString("0.00") + "</strong></td></tr>";
            html += "</table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table><tr><td>&nbsp;</td></tr></table>";
            html += "<table>";
            html += "<tr><td align='left' style='font-size:11px;'>(MOHAMAD ROSNANI BIN HJ. HAMID,PPT)</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Timbalan Pengarah (Sumber Manusia),</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>b.p. Datuk Bandar,</td></tr>";
            html += "<tr><td align='left' style='font-size:11px;'>Majlis Bandaraya Petaling Jaya</td></tr>";
            html += "</table>";

            html += "</body></html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                //string imagepath = Server.MapPath("~/Content/img/logo-o.png");
                
                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/img/logo-mbpj.gif"));
                iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph paragraph = 
                    new iTextSharp.text.Paragraph("MAJLIS BANDARAYA PETALING JAYA PENYATA CARUMAN PERKESO BAGI JAWATAN PEKERJA SUKAN BAGI BULAN \n" 
                    + Bulan + " " + tahun + "")
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pic.ScaleToFit(100f, 80f);
                pic.Alignment = Image.TEXTWRAP | Image.ALIGN_LEFT;
                pic.IndentationRight = 30f;
                paragraph.SpacingBefore = 10f;
                document.Add(pic);
                document.Add(paragraph);
                document.Add(new iTextSharp.text.Paragraph("\n"));
                document.Add(new iTextSharp.text.Paragraph("\n"));

                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        [NonAction]
        private ActionResult GetTextPerkesoSambilan(int? bulan, string Bulan, int? tahun)
        {
            List<TextFileModel> text = TextFileModel
                .GetPerkesoSambilan(Convert.ToInt32(bulan), Convert.ToInt32(tahun));
            string longTextfile = GetTextFile(text);
            return File(Encoding.UTF8.GetBytes(longTextfile), "text/plain",
                string.Format("socso_sambilan_{0}{1}.txt", bulan.ToString().PadLeft(2, '0'), tahun));
        }

        [NonAction]
        private ActionResult GetTextPerkesoSambilanSukan(int? bulan, string Bulan, int? tahun)
        {
            List<TextFileModel> text = 
                TextFileModel.GetPerkesoSambilanSukan(Convert.ToInt32(bulan),Convert.ToInt32(tahun));            
            string longTextfile = GetTextFile(text);
            return File(Encoding.UTF8.GetBytes(longTextfile), "text/plain",
                string.Format("socso_sambilan_sukan_{0}{1}.txt", bulan.ToString().PadLeft(2, '0'), tahun));
        }

        [NonAction]
        private string GetTextFile(List<TextFileModel> text)
        {
            string longTextfile = string.Empty;
            foreach (TextFileModel item in text)
            {
                //string line = string.Format("{0}\t \t{1}{2}", item.String1, item.String2, item.String3);
                //longTextfile += line + Environment.NewLine;
                //string line2 = string.Format("{0}\t \t \t{1}\t{2}", string.Empty, item.String4, item.String5);
                //longTextfile += line2 + Environment.NewLine;
                string line = string.Format("{0}\t \t{1}{2} \t \t \t{3} \t{4}", 
                    item.String1, item.String2, item.String3, item.String4, item.String5);
                longTextfile += line + Environment.NewLine;
            }
            return longTextfile;
        }

        //Khairil: New Action
        public ActionResult UrusBonus(string tahunbekerja)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            int tahun = 0;
            try
            {
                tahun = Convert.ToInt32(tahunbekerja);
            }
            catch
            {

            }

            if (tahun > 0)
            {
                ViewBag.Tahun = tahun;
                List<BonusSambilanMonthModel> list = BonusSambilanMonthModel.GetBonusByYear(tahun);
                return View(list);
            }

            return View();
        }

        public enum ManageMessageId
        {
            Tambah,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ResetPassword,
            Kemaskini,
            KemaskiniTunggakan,
            BayarTunggakan,
            TambahBonus,
            Error,
            Muktamad
        }

    }
}