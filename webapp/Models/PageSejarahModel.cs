using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    //class only for listing ppl with gaji berkecuali.
    public class GajiPekerja
    {
        public string NoPekerja { get; set; }
        public decimal GajiSehari { get; set; }
    }

    public class PageSejarahModel
    {
        public static int ConstHariBekerja
        {
            get
            {
                return 21;
            }
        }

        public static List<string> ListElaunKa
        {
            get
            {
                ApplicationDbContext db = new ApplicationDbContext();
                List<string> kodElaunKA = db.HR_ELAUN
                    .Where(s => s.HR_PENERANGAN_ELAUN.Contains("KHIDMAT AWAM"))
                    .Select(s => s.HR_KOD_ELAUN).ToList();
                return kodElaunKA;
                //return new List<string>
                //{
                //    "E0064",
                //    "E0096",
                //    "E0151",
                //    "E0105"
                //};
            }
        }

        //temporary not used. treated elaunlain = elauncola.
        public static List<string> ListElaunCola
        {
            get
            {
                ApplicationDbContext db = new ApplicationDbContext();
                List<string> kodElaunCola = db.HR_ELAUN
                    .Where(s => s.HR_PENERANGAN_ELAUN.Contains("COLA"))
                    .Select(s => s.HR_KOD_ELAUN).ToList();
                return kodElaunCola;
            }
        }


        /// <summary>
        /// No Pekerja with Pengecualian by Individu
        /// </summary>
        public static List<GajiPekerja> ListPekerjaBerkecuali
        {
            //LAI KOK SEONG 01595
            //WOON SZE MEI 01535
            //VIJAYA KUMARAN PILLAY A/L SEKARAN 01648
            //DAPHNE NG CHIEW YEN 02142
            //MUHAMMAD SYAWAL BIN MOHD ISMAIL 02812
            //YANG LI LIAN 02938
            //YAP CHENG WEN 03387
            //CHEW CHOON ENG 01550

            //KHOO CHIN BEE 01866
            //CHEONG JAY MEE 02452
            //KANG BO LONG 03552
            //RIDZUWAN BIN RAHMAT 02463
            //NURSHAMILIA BINTI MD. SAMSUDIN 02797
            //THEIVIYA A/P SELVA RAJOO 02816
            //VASUTHEVAN A/L LORTCHUAN 01790

            get
            {
                return new List<GajiPekerja>
                {
                    new GajiPekerja{ NoPekerja = "01595", GajiSehari = 29.990M},
                    new GajiPekerja{ NoPekerja = "01535", GajiSehari = 29.990M},
                    new GajiPekerja{ NoPekerja = "01648", GajiSehari = 29.990M},
                    new GajiPekerja{ NoPekerja = "02142", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "02812", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "02938", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "03387", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "01550", GajiSehari = 29.990M},
                    //baru tambah v2
                    new GajiPekerja{ NoPekerja = "01866", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "02452", GajiSehari = 29.990M},
                    new GajiPekerja{ NoPekerja = "03552", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "02463", GajiSehari = 29.990M},
                    new GajiPekerja{ NoPekerja = "02797", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "02816", GajiSehari = 35.670M},
                    new GajiPekerja{ NoPekerja = "01790", GajiSehari = 29.990M}
                };
            }

        }

        public PageSejarahModel()
        {
            bulandibayar = DateTime.Now.Month;
            bulanbekerja = DateTime.Now.AddMonths(-1).Month;
            tahundibayar = DateTime.Now.Year;
            tahunbekerja = DateTime.Now.AddMonths(-1).Year;
            kelulusanydp = "P";
            kelulusanydptunggakan = "P";
            IsMuktamad = false;
        }

        public PageSejarahModel(string HR_PEKERJAstr,
            string tahunbekerjastr,
            string bulanbekerjastr,
            string tahundibayarstr,
            string bulandibayarstr)
        {
            HR_PEKERJA = HR_PEKERJAstr;
            tahunbekerja = string.IsNullOrEmpty(tahunbekerjastr) ? DateTime.Now.Year : Convert.ToInt32(tahunbekerjastr);
            bulanbekerja = string.IsNullOrEmpty(bulanbekerjastr) ? DateTime.Now.AddMonths(-1).Month : Convert.ToInt32(bulanbekerjastr);
            tahundibayar = string.IsNullOrEmpty(tahundibayarstr) ? DateTime.Now.Year : Convert.ToInt32(tahundibayarstr);
            bulandibayar = string.IsNullOrEmpty(bulandibayarstr) ? DateTime.Now.Month : Convert.ToInt32(bulandibayarstr);
            try
            {
                //HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_BULAN_BEKERJA == bulanbekerja && s.HR_BULAN_DIBAYAR == bulandibayar && s.HR_TAHUN == tahundibayar && s.HR_TAHUN_BEKERJA == tahunbekerja && s.HR_KOD_IND == "G");
                ApplicationDbContext db = new ApplicationDbContext();
                int? muktamad = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(x => x.HR_NO_PEKERJA == HR_PEKERJA
                    && x.HR_TAHUN_BEKERJA == tahunbekerja
                    && x.HR_TAHUN == tahundibayar
                    && x.HR_BULAN_BEKERJA == bulanbekerja
                    && x.HR_BULAN_DIBAYAR == bulandibayar
                    && x.HR_KOD_IND == "G").Select(x => x.HR_MUKTAMAD).FirstOrDefault();
                if (muktamad == 1) //if muktamad = 1
                {
                    IsMuktamad = true;
                }
            }
            catch
            {
                IsMuktamad = false;
            }

        }

        // bulan bekerja (current)
        [Display(Name = "No. Pekerja")]
        public string HR_PEKERJA { get; set; }
        [Display(Name = "Tahun Bekerja")]
        public int tahunbekerja { get; set; }
        [Display(Name = "Bahagian")]
        public string bahagian { get; set; }
        [Display(Name = "Bulan Bekerja")]
        public int bulanbekerja { get; set; }
        [Display(Name = "Jabatan")]
        public string jabatan { get; set; }
        [Display(Name = "Tahun Dibayar")]
        public int tahundibayar { get; set; }
        [Display(Name = "Tunggakan")]
        public string tunggakan { get; set; } //values: Y/T
        [Display(Name = "Bulan Dibayar")]
        public int bulandibayar { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Gaji Pokok")]
        public decimal gajipokok { get; set; }
        [Display(Name = "Jumlah Hari")]
        public int jumlahhari { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Elaun Khidmat Awam")]
        public decimal elaunkhidmatawam { get; set; }
        [Display(Name = "Jumlah OT")]
        public int jumlahot { get; set; }
        [Display(Name = "Elaun Lain")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal elaunlain { get; set; }
        [Display(Name = "Jumlah Bayar OT")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal jumlahbayaranot { get; set; }
        [Display(Name = "Potongan KDSK")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal potonganksdk { get; set; }
        [Display(Name = "1/3 Gaji Pokok")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal gajiper3 { get; set; }
        [Display(Name = "Potongan KSWP")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal potongankwsp { get; set; }
        [Display(Name = "SOCSO")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal socso { get; set; }
        [Display(Name = "Lain-lain Potongan")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal potonganlain { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Gaji Sebelum Potongan KWSP")]
        public decimal gajisebelumkwsp { get; set; }
        [Display(Name = "Gaji Kasar Bulan Semasa")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal gajikasar { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Gaji Bersih")]
        public decimal gajibersih { get; set; }
        [Display(Name = "Kelulusan Datuk Bandar")]
        public string kelulusanydp { get; set; } //values Y, T, P

        //bulan sebelum
        [Display(Name = "Tahun Bekerja")]
        public int tunggakantahunbekerja
        {
            get
            {
                try
                {
                    if (bulanbekerja == 1)
                    {
                        return tahunbekerja - 1;
                    }
                    return tahunbekerja;
                }
                catch
                {
                    return 2000;
                }
            }
        }

        [Display(Name = "Bulan Bekerja")]
        public int tunggakanbulanbekerja
        {
            get
            {
                try
                {
                    if (bulanbekerja == 1)
                    {
                        return 12;
                    }
                    return bulanbekerja - 1;
                }
                catch
                {
                    return 1;
                }
            }
        }
        [Display(Name = "Tahun Dibayar")]
        public int tunggakantahundibayar
        {
            get
            {
                return tahundibayar;
            }
        }
        [Display(Name = "Bulan Dibayar")]
        public int tunggakanbulandibayar
        {
            get
            {
                return bulandibayar;
            }
        }

        [Display(Name = "Gaji Pokok")]
        public decimal tunggakangajipokok { get; set; }
        [Display(Name = "Jumlah Hari")]
        public int tunggakanjumlahhari { get; set; }
        [Display(Name = "Elaun Khidmat Awam")]
        public decimal tunggakanelaunkhidmatawam { get; set; }
        [Display(Name = "Jumlah OT")]
        public int tunggakanjumlahot { get; set; }
        [Display(Name = "Elaun Lain")]
        public decimal tunggakanelaunlain { get; set; }
        [Display(Name = "Jumlah Bayar OT")]
        public decimal tunggakanjumlahbayaranot { get; set; }
        [Display(Name = "Potongan KDSK")]
        public decimal tunggakanpotonganksdk { get; set; }
        [Display(Name = "1/3 Gaji Pokok")]
        public decimal tunggakangajiper3 { get; set; }
        [Display(Name = "Potongan KSWP")]
        public decimal tunggakanpotongankwsp { get; set; }
        [Display(Name = "SOCSO")]
        public decimal tunggakansocso { get; set; }
        [Display(Name = "Lain-lain Potongan")]
        public decimal tunggakanpotonganlain { get; set; }
        [Display(Name = "Gaji Sebelum Potongan KWSP")]
        public decimal tunggakangajisebelumkwsp { get; set; }
        [Display(Name = "Gaji Kasar Bulan Semasa")]
        public decimal tunggakangajikasar { get; set; }
        [Display(Name = "Gaji Bersih")]
        public decimal tunggakangajibersih { get; set; }
        [Display(Name = "Kelulusan Datuk Bandar")]
        public string kelulusanydptunggakan { get; set; } //values Y, T, P

        public bool IsMuktamad { get; set; }

        //List Sjarah Pembayaran?

        //Methods
        public int GetJumlahHari()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var jumlahHari = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BEKERJA == bulanbekerja
                && x.HR_TAHUN_BEKERJA == tahunbekerja
                && x.HR_BULAN_DIBAYAR == bulandibayar
                && x.HR_TAHUN == tahundibayar
                && x.HR_KOD == "GAJPS").Select(x => x.HR_JAM_HARI).FirstOrDefault();
            jumlahHari = jumlahHari == null ? 0 : jumlahHari;
            return Convert.ToInt32(jumlahHari);
        }

        public int GetJumlahOT()
        {
            //E0164 = ELAUN LEBIH MASA SAMBILAN
            ApplicationDbContext db = new ApplicationDbContext();
            var jumlahHari = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BEKERJA == bulanbekerja
                && x.HR_TAHUN_BEKERJA == tahunbekerja
                && x.HR_BULAN_DIBAYAR == bulandibayar
                && x.HR_TAHUN == tahundibayar
                && x.HR_KOD == "E0164").Select(x => x.HR_JAM_HARI).FirstOrDefault();
            jumlahHari = jumlahHari == null ? 0 : jumlahHari;
            return Convert.ToInt32(jumlahHari);
        }

        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetElaunKa(ApplicationDbContext db, string HR_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunka = userMaklumatPotongan
                .Where(s => ListElaunKa.Contains(s.HR_KOD_ELAUN_POTONGAN)).ToList();
            return elaunka;
        }

        //guna utk Insert. masukkan semua elaun termasuk elaun KA
        //all elaun except elaun E0164 (elaun OT)
        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetElaunSemua(ApplicationDbContext db, string HR_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunLain = userMaklumatPotongan
                .Where(s => s.HR_ELAUN_POTONGAN_IND == "E"
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && (s.HR_KOD_ELAUN_POTONGAN != "E0164"))
                .ToList();

            return elaunLain;
        }

        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetElaunLain(ApplicationDbContext db, string HR_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunLain = userMaklumatPotongan
                .Where(s => s.HR_ELAUN_POTONGAN_IND == "E"
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && (!ListElaunKa.Contains(s.HR_KOD_ELAUN_POTONGAN) && s.HR_KOD_ELAUN_POTONGAN != "E0164"))
                .ToList();

            return elaunLain;
        }

        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetPotonganKSDK(ApplicationDbContext db, string HR_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganKSDK = userMaklumatPotongan
                .Where(s => s.HR_KOD_ELAUN_POTONGAN == "P0015"
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y").ToList();

            return potonganKSDK;
        }

        /// <summary>
        /// Get Potongan Lain Where KOD Potongan is not P0015 (Potongan KSDK) and not P0035 (KWSP)
        /// </summary>
        /// <param name="HR_PEKERJA"></param>
        /// <returns></returns>
        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetPotonganLain(ApplicationDbContext db, string HR_PEKERJA)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganlain = userMaklumatPotongan
                .Where(s => s.HR_ELAUN_POTONGAN_IND == "P"
                && DateTime.Now >= s.HR_TARIKH_MULA && DateTime.Now <= s.HR_TARIKH_AKHIR
                && s.HR_AKTIF_IND == "Y"
                && (s.HR_KOD_ELAUN_POTONGAN != "P0015" && s.HR_KOD_ELAUN_POTONGAN != "P0035")).ToList();

            HR_MAKLUMAT_ELAUN_POTONGAN potonganSukan = GetPotonganElaunSukan(HR_PEKERJA);
            if (potonganSukan != null)
            {
                potonganlain.Add(potonganSukan);
            }

            return potonganlain;
        }

        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetPotonganSemua(ApplicationDbContext db, string HR_PEKERJA, decimal gajiPokok)
        {
            List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganSemua = new List<HR_MAKLUMAT_ELAUN_POTONGAN>();
            potonganSemua.AddRange(GetPotonganKSDK(db, HR_PEKERJA));
            HR_MAKLUMAT_ELAUN_POTONGAN potonganKWSP = GetPotonganKWSP(db, HR_PEKERJA, gajiPokok)
                .Where(s => s.HR_ELAUN_POTONGAN_IND == "P").FirstOrDefault();
            if(potonganKWSP != null)
            {
                potonganSemua.Add(potonganKWSP);
            }
            potonganSemua.AddRange(GetPotonganLain(db, HR_PEKERJA));
            HR_MAKLUMAT_ELAUN_POTONGAN potonganSukan = GetPotonganElaunSukan(HR_PEKERJA);
            if ( potonganSukan != null)
            {
                potonganSemua.Add(potonganSukan);
            }

            return potonganSemua;
        }

        public static HR_MAKLUMAT_ELAUN_POTONGAN GetPotonganElaunSukan(string HR_PEKERJA)
        {
            HR_MAKLUMAT_ELAUN_POTONGAN elaunSukan = null;
            //only certain number of ppl need to pay elaun sukan
            var check = ListPekerjaBerkecuali.Where(s => s.NoPekerja == HR_PEKERJA).ToList();
            if (check.Count > 0)
            {
                elaunSukan = new HR_MAKLUMAT_ELAUN_POTONGAN
                {
                    HR_NO_PEKERJA = HR_PEKERJA,
                    HR_ELAUN_POTONGAN_IND = "P",
                    HR_KOD_ELAUN_POTONGAN = "P8888", //temp Code for Yuran sukan
                    HR_AKTIF_IND = "Y",
                    HR_JUMLAH = 1.50M //Yuran sukan is RM1.50
                };
            }
            //if no pekerja not in the list, it will return null
            return elaunSukan;
        }

        public static HR_KWSP GetKWSPData(ApplicationDbContext db, decimal gajiPokok)
        {
            decimal gajipokok = decimal.Round(gajiPokok, 2);
            HR_KWSP kwsp = db.HR_KWSP
               .Where(s => gajipokok >= s.HR_UPAH_DARI
               && gajipokok <= s.HR_UPAH_HINGGA).FirstOrDefault();
            return kwsp;
        }

        public static List<HR_MAKLUMAT_ELAUN_POTONGAN> GetPotonganKWSP
            (ApplicationDbContext db, string HR_PEKERJA, decimal gajiPokok)
        {
            //HR_MAKLUMAT_ELAUN_POTONGAN userCaruman = db.HR_MAKLUMAT_ELAUN_POTONGAN
            //            .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "C").First();

            List<HR_MAKLUMAT_ELAUN_POTONGAN> userMaklumatPotongan = db.HR_MAKLUMAT_ELAUN_POTONGAN
                        .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).ToList();

            //Caruman Pekerja = Kira dari Gaji POKOK? or Kira dari Gaji POKOK + elaun
            HR_KWSP kwsp = GetKWSPData(db, gajiPokok);

            decimal carumanPekerja = 0;
            decimal carumanMajikan = 0;
            if (kwsp != null)
            {
                carumanPekerja = decimal.Round(kwsp.HR_CARUMAN_PEKERJA, 2);
                carumanMajikan = decimal.Round(kwsp.HR_CARUMAN_MAJIKAN, 2);
            }

            HR_MAKLUMAT_ELAUN_POTONGAN userCaruman = new HR_MAKLUMAT_ELAUN_POTONGAN
            {
                HR_NO_PEKERJA = HR_PEKERJA,
                HR_ELAUN_POTONGAN_IND = "P",
                HR_KOD_ELAUN_POTONGAN = "P0035",
                HR_AKTIF_IND = "Y",
                HR_JUMLAH = carumanPekerja
            };

            HR_MAKLUMAT_ELAUN_POTONGAN userCaruman2 = new HR_MAKLUMAT_ELAUN_POTONGAN
            {
                HR_NO_PEKERJA = HR_PEKERJA,
                HR_ELAUN_POTONGAN_IND = "C",
                HR_KOD_ELAUN_POTONGAN = "C0020",
                HR_AKTIF_IND = "Y",
                HR_JUMLAH = carumanMajikan
            };

            List<HR_MAKLUMAT_ELAUN_POTONGAN> userCarumanTotal = new List<HR_MAKLUMAT_ELAUN_POTONGAN>();
            userCarumanTotal.Add(userCaruman);
            userCarumanTotal.Add(userCaruman2);

            return userCarumanTotal;
        }

        public static decimal GetGajiPokok(ApplicationDbContext db, string HR_PEKERJA, int hariBekerja)
        {
            //add new condition: no need to go to DB if hari bekerja is 0
            if(hariBekerja > 0)
            {
                decimal gajiSehari = GetGajiSehari(db, HR_PEKERJA);
                decimal gajiPokok = gajiSehari * hariBekerja;
                gajiPokok = decimal.Round(gajiPokok, 2);

                return gajiPokok;
            }
            return 0.00M;            
        }

        public static decimal GetGajiSehari(ApplicationDbContext db, string HR_PEKERJA)
        {
            GajiPekerja gajiPekerja = ListPekerjaBerkecuali
                .Where(s => s.NoPekerja == HR_PEKERJA).FirstOrDefault();
            //kalau gaji pekerja ada dalam list berkecuali, ambik value yang ni
            if (gajiPekerja != null)
            {
                return gajiPekerja.GajiSehari;
            }

            HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).FirstOrDefault();
            decimal gajiSehari = 54.00M;
            if (mpekerjaan != null)
            {
                // kalau gred > 19, gaji RM54
                // kalau gaji < 19, gaji RM72
                //List<string> gred54 = new List<string>
                //{
                //    "C",
                //    "D",
                //    "E"
                //};
                //new rule:
                //Kumpulan A = RM100 gaji sehari
                //Kumpulan A = RM72 gaji sehari
                //Kumpulan C (+D+E?) = RM54 gaji sehari
                string gred = mpekerjaan.HR_KUMPULAN;
                switch (gred)
                {
                    case "A" :
                        gajiSehari = 100.00M;
                        break;
                    case "B":
                        gajiSehari = 72.00M;
                        break;
                    default :
                        gajiSehari = 54.00M;
                        break;
                }
            }
            return gajiSehari;
        }

        public static decimal GetElaunOT(ApplicationDbContext db, string HR_PEKERJA, int jumlahHari, decimal jumlahJamOT)
        {
            //add new condition: no need to calculate if hari or jam OT is 0
            if(jumlahJamOT > 0 && jumlahHari > 0)
            {
                var gajiSehari = GetGajiSehari(db, HR_PEKERJA);
                var gajisehariot = (gajiSehari * jumlahHari) * 12 / 2504;
                var elaunOT = gajisehariot * jumlahJamOT;
                return elaunOT;
            }
            return 0;            
        }

        //9/5/2019 - added Extra calculaction to calculate GajiKasar
        public static decimal GetGajiKasar(decimal gaji,
            decimal? elaunka,
            decimal? elaunlain,
            decimal elaunot,
            decimal hariBekerja)
        {
            try
            {
                //EKA = elaunKA x jumlahhari bekerja
                //COLA = cola x jumlahhari bekerja
                var totalElaunka = elaunka * hariBekerja;
                var totalElaunLain = elaunlain * hariBekerja;
                var totalElaunOT = elaunot;

                var gajikasar = gaji + totalElaunka + totalElaunLain + totalElaunOT;
                if (gajikasar == null)
                {
                    return 0;
                }
                return gajikasar.Value;
            }
            catch
            {
                return 0;
            }            
        }

        public static decimal GetPotonganSocso(ApplicationDbContext db, decimal gajiPokok, decimal elaunOT)
        {
            //gaji kasar here means gaji kasar - elaun
            decimal gajiKasar = gajiPokok + elaunOT;
            if(gajiKasar > 0)
            {
                try
                {
                    //fix issue userSocso keluar error kalau user taip hari = 1
                    HR_SOCSO userSocso = db.HR_SOCSO.Where(s => s.HR_GAJI_DARI <= gajiKasar
                       && gajiKasar <= s.HR_GAJI_HINGGA).SingleOrDefault();

                    return userSocso.HR_JUMLAH;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return 0;
                }
            }
            return 0;
        }

        //new functions 15/09/2019
        public static decimal GetElaunKA_fromDB()
        {
            //todo
            return 0;
        }

        public static PageSejarahModel Insert(PageSejarahModel agree, string user, string command)
        {
            var testVal = agree.tunggakangajipokok;

            ApplicationDbContext db = new ApplicationDbContext();
            //MajlisContext mc = new MajlisContext();
            HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA).SingleOrDefault();

            //Only add elaun lain (including elaun KA), do not add ElaunOT
            List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunsemua = GetElaunSemua(db, agree.HR_PEKERJA);

            //TODO: make sure bila insert, semua maklumat potongan masuk
            List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganSemua = GetPotonganSemua(db, agree.HR_PEKERJA, agree.gajipokok);

            //Add Caruman
            //List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman = GetCaruman(db, agree.HR_PEKERJA);

            //For Logging
            try
            {
                var tbl = db.Users.Where(p => p.Id == user).FirstOrDefault();
                var emel = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == tbl.UserName).FirstOrDefault();
                var role1 = db.UserRoles.Where(d => d.UserId == tbl.Id).FirstOrDefault();
                var role = db.Roles.Where(e => e.Id == role1.RoleId).FirstOrDefault();

                if (string.IsNullOrEmpty(command))
                {
                    command = "muktamad";
                }

                switch (command.ToLower())
                {
                    case ("hantar"):
                        InsertHantar(db, agree, elaunsemua, potonganSemua, agree.gajipokok);
                        TrailLog(emel, role,
                            emel.HR_NAMA_PEKERJA + " Telah menambah data untuk pekerja " + agree.HR_PEKERJA);
                        break;
                    case ("kemaskini"):
                        InsertHantar(db, agree, elaunsemua, potonganSemua, agree.gajipokok);
                        //InsertKemaskini(db, agree, listkwsp, elaunLain, potonganSemua,
                        //    maklumatcaruman, agree.gajipokok);
                        TrailLog(emel, role,
                            emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA);
                        break;
                    case ("muktamad"):
                        InsertMuktamad(db, agree);
                        TrailLog(emel, role,
                            emel.HR_NAMA_PEKERJA + " Telah mengubah data untuk pekerja " + agree.HR_PEKERJA);
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
           
            return agree;
        }

        private static void InsertMuktamad(ApplicationDbContext db, PageSejarahModel agree)
        {
            HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                && s.HR_BULAN_BEKERJA == agree.bulanbekerja
                && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
                && s.HR_BULAN_DIBAYAR == agree.bulandibayar
                && s.HR_TAHUN == agree.tahundibayar
                && s.HR_KOD == "GAJPS").FirstOrDefault();
            gaji.HR_MUKTAMAD = 1;

            db.Entry(gaji).State = EntityState.Modified;
            db.SaveChanges();
        }

        //private static void InsertKemaskini(ApplicationDbContext db, PageSejarahModel agree,
        //    List<HR_KWSP> listkwsp,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatpotongan,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
        //    decimal gajipokok)
        //{
        //    InsertBulan(db, agree);
        //    if (agree.tunggakan == "Y")
        //    {
        //        InsertTunggakan(db, agree, listkwsp, maklumatelaun, maklumatpotongan, maklumatcaruman, gajipokok);
        //    }
        //}

        //private static void InsertBulan(ApplicationDbContext db, PageSejarahModel agree)
        //{
        //    HR_TRANSAKSI_SAMBILAN_DETAIL gaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
        //        .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
        //        && s.HR_BULAN_BEKERJA == agree.bulanbekerja
        //        && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
        //        && s.HR_BULAN_DIBAYAR == agree.bulandibayar
        //        && s.HR_TAHUN == agree.tahundibayar
        //        && s.HR_KOD == "GAJPS").SingleOrDefault();
        //    HR_TRANSAKSI_SAMBILAN_DETAIL ot = db.HR_TRANSAKSI_SAMBILAN_DETAIL
        //        .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
        //        && s.HR_BULAN_BEKERJA == agree.bulanbekerja
        //        && s.HR_TAHUN_BEKERJA == agree.tahunbekerja
        //        && s.HR_BULAN_DIBAYAR == agree.bulandibayar
        //        && s.HR_TAHUN == agree.tahundibayar
        //        && s.HR_KOD == "E0164").SingleOrDefault();
        //    if (gaji != null)
        //    {
        //        gaji.HR_JUMLAH = agree.gajipokok;
        //        gaji.HR_JAM_HARI = agree.jumlahhari;
        //        gaji.HR_YDP_LULUS_IND = agree.kelulusanydptunggakan;
        //        db.Entry(gaji).State = EntityState.Modified;
        //    }
        //    else
        //    {
        //        gaji = new HR_TRANSAKSI_SAMBILAN_DETAIL
        //        {
        //            HR_NO_PEKERJA = agree.HR_PEKERJA,
        //            HR_BULAN_BEKERJA = agree.bulanbekerja,
        //            HR_TAHUN_BEKERJA = agree.tahunbekerja,
        //            HR_BULAN_DIBAYAR = agree.bulandibayar,
        //            HR_TAHUN = agree.tahundibayar,
        //            HR_KOD = "GAJPS",
        //            HR_KOD_IND = "G",
        //            HR_JUMLAH = agree.gajipokok,
        //            HR_JAM_HARI = agree.jumlahhari,
        //            HR_YDP_LULUS_IND = agree.kelulusanydp
        //        };
        //        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gaji);
        //    }
        //    if (ot != null)
        //    {
        //        ot.HR_JUMLAH = GetElaunOT(db, agree.HR_PEKERJA, agree.jumlahhari, agree.jumlahot);
        //        ot.HR_JAM_HARI = agree.jumlahot;
        //        ot.HR_YDP_LULUS_IND = agree.kelulusanydp;
        //        db.Entry(ot).State = EntityState.Modified;
        //    }
        //    else
        //    {
        //        ot = new HR_TRANSAKSI_SAMBILAN_DETAIL
        //        {
        //            HR_NO_PEKERJA = agree.HR_PEKERJA,
        //            HR_BULAN_BEKERJA = agree.bulanbekerja,
        //            HR_TAHUN_BEKERJA = agree.tahunbekerja,
        //            HR_BULAN_DIBAYAR = agree.bulandibayar,
        //            HR_TAHUN = agree.tahundibayar,
        //            HR_KOD = "E0164",
        //            HR_KOD_IND = "E",
        //            HR_JUMLAH = GetElaunOT(db, agree.HR_PEKERJA, agree.jumlahhari, agree.jumlahot),
        //            HR_JAM_HARI = agree.jumlahot,
        //            HR_YDP_LULUS_IND = agree.kelulusanydptunggakan
        //        };
        //        db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(ot);
        //    }

        //    db.SaveChanges();
        //}

        //private static void InsertTunggakan(ApplicationDbContext db, PageSejarahModel agree,
        //    List<HR_KWSP> listkwsp,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatpotongan,
        //    List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
        //    decimal gajipokok)
        //{
        //    foreach (var kwsp in listkwsp)
        //    {
        //        if (gajipokok >= kwsp.HR_UPAH_DARI && gajipokok <= kwsp.HR_UPAH_HINGGA)
        //        {
        //            InsertHRSAMBILAN(db, agree, true);
        //            InsertMAJIKANKWSP(db, agree, kwsp, true);
        //            InsertPekerjaKSWP(db, agree, kwsp, true);

        //            if (maklumatpotongan != null)
        //            {
        //                InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan, true);
        //            }
        //            InsertELAUNOT(db, agree, true);
        //            InsertGAJIPEKERJA(db, agree, true);
        //            InsertELAUNLAIN(db, agree, maklumatelaun, true);
        //            InsertMAKLUMATCARUMAN(db, agree, maklumatcaruman, true);
        //        }
        //    }
        //}

        private static void TrailLog(HR_MAKLUMAT_PERIBADI emel, IdentityRole role,
            string message)
        {
            try
            {
                System.Web.HttpContext httpContext = System.Web.HttpContext.Current;
                new AuditTrailModels().Log(emel.HR_EMAIL, emel.HR_NAMA_PEKERJA,
                   httpContext.Request.UserHostAddress,
                   role.Name,
                   message,
                   System.Net.Dns.GetHostName(),
                   emel.HR_TELBIMBIT, httpContext.Request.RawUrl, "Sejarah");
            }
            catch
            {

            }

        }

        private static void InsertHantar(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatpotongan,
            //List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
            decimal gajipokok)
        {
            //instead of looping 401 records, a LINQ is used here to get the correct KWSP based on gaji pokok
            //KWSP based on gaji pokok? or KWSP based on gaji pokok + elaun
            HR_KWSP kwsp = db.HR_KWSP
                .Where(s => gajipokok >= s.HR_UPAH_DARI
                && gajipokok <= s.HR_UPAH_HINGGA).SingleOrDefault();
            try
            {
                InsertHRSAMBILAN(db, agree);
                InsertMAJIKANKWSP(db, agree, kwsp);
                InsertPEKERJAKSWP(db, agree, kwsp);
                if (maklumatpotongan != null)
                {
                    InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan);
                }
                InsertELAUNOT(db, agree);
                InsertGAJIPEKERJA(db, agree);
                InsertELAUNLAIN(db, agree, maklumatelaun);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (agree.tunggakan == "Y")
            {
                try
                {
                    InsertHRSAMBILAN(db, agree, true);
                    InsertMAJIKANKWSP(db, agree, kwsp, true);
                    InsertPEKERJAKSWP(db, agree, kwsp, true);

                    if (maklumatpotongan != null)
                    {
                        InsertMAKLUMATPOTONGAN(db, agree, maklumatpotongan, true);
                    }
                    InsertELAUNOT(db, agree, true);
                    InsertGAJIPEKERJA(db, agree, true);
                    InsertELAUNLAIN(db, agree, maklumatelaun, true);
                    //InsertMAKLUMATCARUMAN(db, agree, maklumatcaruman, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /*
         * /////////////////////////////////////////////////////////////////////////////
         * ///////// INSERT DATA TO HR_TRANSAKSI_SAMBILAN/SAMBILAN_DETAIL //////////////
         * /////////////////////////////////////////////////////////////////////////////
         */

        #region Insert Into HR SAMBILAN DETAIL

        private static void InsertHRSAMBILAN(ApplicationDbContext db, PageSejarahModel agree,
            bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;

            #region HR_transaksiSambilan Table

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            HR_TRANSAKSI_SAMBILAN sambilan = db.HR_TRANSAKSI_SAMBILAN
                       .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                       && s.HR_BULAN_DIBAYAR == bulanDibayar
                       && s.HR_BULAN_BEKERJA == bulanBekerja
                       && s.HR_TAHUN == tahunDibayar
                       && s.HR_TAHUN_BEKERJA == tahunBekerja).FirstOrDefault();
            if (sambilan == null)
            {
                try
                {
                    sambilan = new HR_TRANSAKSI_SAMBILAN
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_BEKERJA = bulanBekerja,
                        HR_TAHUN_BEKERJA = tahunBekerja,
                        HR_TAHUN = tahunDibayar,
                        HR_BULAN_DIBAYAR = bulanDibayar
                    };
                    db.HR_TRANSAKSI_SAMBILAN.Add(sambilan);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    //this try/catch block is to prevent system crashing 
                    //when we try to "Tambah" a new tunggakan for a pekerja.
                    //this will ensure all other data is inserted to HR_TRANSAKSI_SAMBILAN_DETAIL later.
                    Console.Write(ex.ToString());
                }
            }
            #endregion
        }

        private static void InsertMAJIKANKWSP(ApplicationDbContext db, PageSejarahModel agree, HR_KWSP kwsp, bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            #region MajikanKWSP C0020
            HR_TRANSAKSI_SAMBILAN_DETAIL majikankwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == bulanDibayar
                      && s.HR_BULAN_BEKERJA == bulanBekerja
                      && s.HR_TAHUN == tahunDibayar
                      && s.HR_TAHUN_BEKERJA == tahunBekerja
                      && s.HR_KOD == "C0020").FirstOrDefault();

            if (majikankwsp != null)
            {
                majikankwsp.HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN;
                majikankwsp.HR_YDP_LULUS_IND = kelulusanYDP;
                majikankwsp.HR_KOD_IND = "C";
                majikankwsp.HR_TUNGGAKAN_IND = "T";
                majikankwsp.HR_MUKTAMAD = 0;
                db.Entry(majikankwsp).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                majikankwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_DIBAYAR = bulanDibayar,
                    HR_TAHUN = tahunDibayar,
                    HR_KOD = "C0020",
                    HR_BULAN_BEKERJA = bulanBekerja,
                    HR_JUMLAH = kwsp.HR_CARUMAN_MAJIKAN,
                    HR_KOD_IND = "C",
                    HR_TUNGGAKAN_IND = "T",
                    HR_TAHUN_BEKERJA = tahunBekerja,
                    HR_YDP_LULUS_IND = agree.kelulusanydp,
                    HR_MUKTAMAD = 0
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(majikankwsp);
                db.SaveChanges();
            }
            #endregion
        }

        private static void InsertPEKERJAKSWP(ApplicationDbContext db, PageSejarahModel agree, HR_KWSP kwsp, bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            #region PekerjaKWSP P0035

            HR_TRANSAKSI_SAMBILAN_DETAIL pekerjakwsp = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                    && s.HR_BULAN_DIBAYAR == bulanDibayar
                    && s.HR_BULAN_BEKERJA == bulanBekerja
                    && s.HR_TAHUN == tahunDibayar
                    && s.HR_TAHUN_BEKERJA == tahunBekerja
                    && s.HR_KOD == "P0035").FirstOrDefault();

            if (pekerjakwsp != null)
            {
                pekerjakwsp.HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA;
                pekerjakwsp.HR_YDP_LULUS_IND = kelulusanYDP;
                pekerjakwsp.HR_KOD_IND = "P";
                pekerjakwsp.HR_TUNGGAKAN_IND = "T";
                pekerjakwsp.HR_MUKTAMAD = 0;
                db.Entry(pekerjakwsp).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                pekerjakwsp = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_DIBAYAR = bulanDibayar,
                    HR_TAHUN = tahunDibayar,
                    HR_KOD = "P0035",
                    HR_BULAN_BEKERJA = bulanBekerja,
                    HR_JUMLAH = kwsp.HR_CARUMAN_PEKERJA,
                    HR_KOD_IND = "P",
                    HR_TUNGGAKAN_IND = "T",
                    HR_TAHUN_BEKERJA = tahunBekerja,
                    HR_MUKTAMAD = 0
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(pekerjakwsp);
                db.SaveChanges();
            }

            #endregion
        }

        private static void InsertMAKLUMATPOTONGAN(ApplicationDbContext db, PageSejarahModel agree, List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganSemua,
            bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            #region potongan semua
            foreach (HR_MAKLUMAT_ELAUN_POTONGAN potongan in potonganSemua)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL transaksi = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                        && s.HR_BULAN_DIBAYAR == bulanDibayar
                        && s.HR_BULAN_BEKERJA == bulanBekerja
                        && s.HR_TAHUN == tahunDibayar
                        && s.HR_TAHUN_BEKERJA == tahunBekerja
                        && s.HR_KOD == potongan.HR_KOD_ELAUN_POTONGAN
                        && s.HR_KOD_IND == potongan.HR_ELAUN_POTONGAN_IND
                        && s.HR_TUNGGAKAN_IND == "T").FirstOrDefault();
                if (transaksi != null)
                {
                    transaksi.HR_JUMLAH = potongan.HR_JUMLAH;
                    transaksi.HR_YDP_LULUS_IND = kelulusanYDP;
                    db.Entry(transaksi).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    transaksi = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = bulanDibayar,
                        HR_TAHUN = tahunDibayar,
                        HR_KOD = potongan.HR_KOD_ELAUN_POTONGAN,
                        HR_BULAN_BEKERJA = bulanBekerja,
                        HR_JUMLAH = potongan.HR_JUMLAH,
                        HR_KOD_IND = potongan.HR_ELAUN_POTONGAN_IND,
                        HR_TUNGGAKAN_IND = "T",
                        HR_YDP_LULUS_IND = kelulusanYDP,
                        HR_TAHUN_BEKERJA = tahunBekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(transaksi);
                    db.SaveChanges();
                }
            }
            #endregion
        }

        private static void InsertELAUNOT(ApplicationDbContext db, PageSejarahModel agree,
            bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;
            agree.gajipokok = GetGajiPokok(db, agree.HR_PEKERJA, agree.jumlahhari);
            var jumlahJamOT = agree.jumlahot;
            var jumlahElaunOT = GetElaunOT(db, agree.HR_PEKERJA, agree.jumlahhari, agree.jumlahot);

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
                jumlahJamOT = agree.tunggakanjumlahot;
                agree.gajipokok = GetGajiPokok(db, agree.HR_PEKERJA, agree.tunggakanjumlahhari);
                jumlahElaunOT = GetElaunOT(db, agree.HR_PEKERJA, agree.tunggakanjumlahhari, agree.tunggakanjumlahot);
            }

            #region Elaun OT E0164
            HR_TRANSAKSI_SAMBILAN_DETAIL elaunotData = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                     .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                     && s.HR_BULAN_DIBAYAR == bulanDibayar
                     && s.HR_BULAN_BEKERJA == bulanBekerja
                     && s.HR_TAHUN == tahunDibayar
                     && s.HR_TAHUN_BEKERJA == tahunBekerja
                     && s.HR_KOD == "E0164").FirstOrDefault();
            if (elaunotData != null)
            {
                elaunotData.HR_JAM_HARI = jumlahJamOT;
                elaunotData.HR_JUMLAH = jumlahElaunOT;
                elaunotData.HR_YDP_LULUS_IND = kelulusanYDP;
                elaunotData.HR_KOD_IND = "E";
                elaunotData.HR_TUNGGAKAN_IND = "T";
                elaunotData.HR_MUKTAMAD = 0;
                db.Entry(elaunotData).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                elaunotData = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_DIBAYAR = bulanDibayar,
                    HR_TAHUN = tahunDibayar,
                    HR_KOD = "E0164",
                    HR_BULAN_BEKERJA = bulanBekerja,
                    HR_TAHUN_BEKERJA = tahunBekerja,
                    HR_JUMLAH = jumlahElaunOT,
                    HR_KOD_IND = "E",
                    HR_TUNGGAKAN_IND = "T",
                    HR_JAM_HARI = jumlahJamOT,
                    HR_YDP_LULUS_IND = kelulusanYDP,
                    HR_MUKTAMAD = 0
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunotData);
                db.SaveChanges();
            }
            #endregion
        }

        private static void InsertGAJIPEKERJA(ApplicationDbContext db, PageSejarahModel agree, bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            int jumlahHari = agree.jumlahhari;
            string kelulusanYDP = agree.kelulusanydp;
            decimal gajiPokok = agree.gajipokok;

            if (isTunggakan)
            {
                gajiPokok = agree.tunggakangajipokok;
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                jumlahHari = agree.tunggakanjumlahhari;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            #region gaji Pekerja GAJPS
            HR_TRANSAKSI_SAMBILAN_DETAIL gajipekerja = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                      .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                      && s.HR_BULAN_DIBAYAR == bulanDibayar
                      && s.HR_BULAN_BEKERJA == bulanBekerja
                      && s.HR_TAHUN == tahunDibayar
                      && s.HR_TAHUN_BEKERJA == tahunBekerja
                      && s.HR_KOD == "GAJPS").FirstOrDefault();
            if (gajipekerja != null)
            {
                gajipekerja.HR_JUMLAH = gajiPokok;
                gajipekerja.HR_KOD_IND = "G";
                gajipekerja.HR_JAM_HARI = jumlahHari;
                gajipekerja.HR_TUNGGAKAN_IND = "T";
                gajipekerja.HR_MUKTAMAD = 0;
                db.Entry(gajipekerja).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                gajipekerja = new HR_TRANSAKSI_SAMBILAN_DETAIL
                {
                    HR_NO_PEKERJA = agree.HR_PEKERJA,
                    HR_BULAN_DIBAYAR = bulanDibayar,
                    HR_TAHUN = tahunDibayar,
                    HR_KOD = "GAJPS",
                    HR_BULAN_BEKERJA = bulanBekerja,
                    HR_JUMLAH = agree.gajipokok,
                    HR_KOD_IND = "G",
                    HR_JAM_HARI = jumlahHari,
                    HR_TUNGGAKAN_IND = "T",
                    HR_TAHUN_BEKERJA = tahunBekerja,
                    HR_MUKTAMAD = 0
                };
                db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(gajipekerja);
                db.SaveChanges();
            }
            #endregion
        }

        private static void InsertELAUNLAIN(ApplicationDbContext db, PageSejarahModel agree,
            List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatelaun, bool isTunggakan = false)
        {
            int bulanDibayar = agree.bulandibayar;
            int tahunDibayar = agree.tahundibayar;
            int bulanBekerja = agree.bulanbekerja;
            int tahunBekerja = agree.tahunbekerja;
            string kelulusanYDP = agree.kelulusanydp;

            if (isTunggakan)
            {
                bulanDibayar = agree.tunggakanbulandibayar;
                tahunDibayar = agree.tunggakantahundibayar;
                bulanBekerja = agree.tunggakanbulanbekerja;
                tahunBekerja = agree.tunggakantahunbekerja;
                kelulusanYDP = agree.kelulusanydptunggakan;
            }

            #region Semua Elaun except E0164 (elaunOT)
            foreach (var elaun in maklumatelaun)
            {
                HR_TRANSAKSI_SAMBILAN_DETAIL elaunlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                     .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
                    && s.HR_BULAN_DIBAYAR == bulanDibayar
                    && s.HR_BULAN_BEKERJA == bulanBekerja
                    && s.HR_TAHUN == tahunDibayar
                    && s.HR_TAHUN_BEKERJA == tahunBekerja
                    && s.HR_KOD == elaun.HR_KOD_ELAUN_POTONGAN
                    && s.HR_KOD_IND == elaun.HR_ELAUN_POTONGAN_IND).FirstOrDefault();
                if (elaunlain != null)
                {
                    elaunlain.HR_JUMLAH = elaun.HR_JUMLAH;
                    elaunlain.HR_TUNGGAKAN_IND = "T";
                    elaunlain.HR_MUKTAMAD = 0;
                    db.Entry(elaunlain).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    elaunlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = agree.HR_PEKERJA,
                        HR_BULAN_DIBAYAR = bulanDibayar,
                        HR_TAHUN = tahunDibayar,
                        HR_KOD = elaun.HR_KOD_ELAUN_POTONGAN,
                        HR_BULAN_BEKERJA = bulanBekerja,
                        HR_JUMLAH = elaun.HR_JUMLAH,
                        HR_KOD_IND = elaun.HR_ELAUN_POTONGAN_IND,
                        HR_TUNGGAKAN_IND = "T",
                        HR_TAHUN_BEKERJA = tahunBekerja,
                        HR_MUKTAMAD = 0
                    };
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(elaunlain);
                    db.SaveChanges();
                }
            }
            #endregion

        }

        //private static void InsertMAKLUMATCARUMAN(ApplicationDbContext db, PageSejarahModel agree, List<HR_MAKLUMAT_ELAUN_POTONGAN> maklumatcaruman,
        //    bool isTunggakan = false)
        //{
        //    int bulanDibayar = agree.bulandibayar;
        //    int tahunDibayar = agree.tahundibayar;
        //    int bulanBekerja = agree.bulanbekerja;
        //    int tahunBekerja = agree.tahunbekerja;

        //    if (isTunggakan)
        //    {
        //        bulanDibayar = agree.tunggakanbulandibayar;
        //        tahunDibayar = agree.tunggakantahundibayar;
        //        bulanBekerja = agree.tunggakanbulanbekerja;
        //        tahunBekerja = agree.tunggakantahunbekerja;
        //    }

        //    #region makluman caruman (C0020)?
        //    foreach (var caruman in maklumatcaruman)
        //    {
        //        HR_TRANSAKSI_SAMBILAN_DETAIL potonganlain = db.HR_TRANSAKSI_SAMBILAN_DETAIL
        //            .Where(s => s.HR_NO_PEKERJA == agree.HR_PEKERJA
        //            && s.HR_BULAN_DIBAYAR == bulanDibayar
        //            && s.HR_BULAN_BEKERJA == bulanBekerja
        //            && s.HR_TAHUN == tahunDibayar
        //            && s.HR_TAHUN_BEKERJA == tahunBekerja
        //            && s.HR_KOD == caruman.HR_KOD_ELAUN_POTONGAN
        //            && s.HR_KOD_IND == caruman.HR_ELAUN_POTONGAN_IND).FirstOrDefault();

        //        if (potonganlain != null)
        //        {
        //            potonganlain.HR_JUMLAH = caruman.HR_JUMLAH;
        //            potonganlain.HR_TUNGGAKAN_IND = "T";
        //            potonganlain.HR_MUKTAMAD = 0;
        //            db.Entry(potonganlain).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            potonganlain = new HR_TRANSAKSI_SAMBILAN_DETAIL
        //            {
        //                HR_NO_PEKERJA = agree.HR_PEKERJA,
        //                HR_BULAN_DIBAYAR = agree.bulandibayar,
        //                HR_TAHUN = agree.tahundibayar,
        //                HR_KOD = caruman.HR_KOD_ELAUN_POTONGAN,
        //                HR_BULAN_BEKERJA = agree.bulanbekerja,
        //                HR_JUMLAH = caruman.HR_JUMLAH,
        //                HR_KOD_IND = caruman.HR_ELAUN_POTONGAN_IND,
        //                HR_TUNGGAKAN_IND = "T",
        //                HR_TAHUN_BEKERJA = agree.tahunbekerja,
        //                HR_MUKTAMAD = 0
        //            };
        //            db.HR_TRANSAKSI_SAMBILAN_DETAIL.Add(potonganlain);
        //            db.SaveChanges();
        //        }
        //    }
        //    #endregion
        //}

        #endregion


        //private static void UpdateSambilanDetail(ApplicationDbContext db, PageSejarahModel agree)
        //{
        //    bool isNew = false;
        //    HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
        //        .Where(x => x.HR_NO_PEKERJA == agree.HR_PEKERJA
        //        && x.HR_TAHUN_BONUS == agree.tahundibayar
        //        && x.HR_BULAN_BONUS == agree.bulandibayar).FirstOrDefault();
        //    if (det == null)
        //    {
        //        isNew = true;
        //        //insert det
        //        det = new HR_BONUS_SAMBILAN_DETAIL();
        //        det.HR_NO_PEKERJA = agree.HR_PEKERJA;
        //        det.HR_TAHUN_BONUS = agree.tahundibayar;
        //        det.HR_BULAN_BONUS = agree.bulandibayar;
        //        det.HR_NO_KPBARU = db.HR_MAKLUMAT_PERIBADI
        //            .Where(p => p.HR_NO_PEKERJA == agree.HR_PEKERJA)
        //            .Select(p => p.HR_NO_KPBARU).FirstOrDefault();
        //    }

        //    det = UpdateBonusSambilanInfo(db, det, agree);

        //    if (isNew)
        //    {
        //        db.HR_BONUS_SAMBILAN_DETAIL.Add(det);
        //        db.SaveChanges();
        //    }
        //    else
        //    {
        //        //update det
        //        db.Entry(det).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}

        //private static HR_BONUS_SAMBILAN_DETAIL UpdateBonusSambilanInfo
        //        (ApplicationDbContext db, HR_BONUS_SAMBILAN_DETAIL det, PageSejarahModel agree)
        //{
        //    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain =
        //       db.HR_TRANSAKSI_SAMBILAN_DETAIL.
        //       Where(x => x.HR_NO_PEKERJA == agree.HR_PEKERJA
        //       && x.HR_TAHUN == agree.tahundibayar
        //       && x.HR_BULAN_DIBAYAR == agree.bulandibayar).ToList();

        //    det.HR_JANUARI = elaunlain
        //            .Where(c => c.HR_BULAN_BEKERJA == 1).Sum(c => c.HR_JUMLAH);
        //    det.HR_FEBRUARI = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 2).Sum(c => c.HR_JUMLAH);
        //    det.HR_MAC = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 3).Sum(c => c.HR_JUMLAH);
        //    det.HR_APRIL = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 4).Sum(c => c.HR_JUMLAH);
        //    det.HR_MEI = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 5).Sum(c => c.HR_JUMLAH);
        //    det.HR_JUN = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 6).Sum(c => c.HR_JUMLAH);
        //    det.HR_JULAI = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 7).Sum(c => c.HR_JUMLAH);
        //    det.HR_OGOS = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 8).Sum(c => c.HR_JUMLAH);
        //    det.HR_SEPTEMBER = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 9).Sum(c => c.HR_JUMLAH);
        //    det.HR_OKTOBER = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 10).Sum(c => c.HR_JUMLAH);
        //    det.HR_NOVEMBER = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 11).Sum(c => c.HR_JUMLAH);
        //    det.HR_DISEMBER = elaunlain
        //        .Where(c => c.HR_BULAN_BEKERJA == 12).Sum(c => c.HR_JUMLAH);
        //    det.HR_JUMLAH_GAJI = elaunlain.Sum(c => c.HR_JUMLAH);
        //    int totalBulan = elaunlain.Select(c => c.HR_BULAN_BEKERJA).Distinct().Count();
        //    if (totalBulan > 0)
        //    {
        //        det.HR_GAJI_PURATA = det.HR_JUMLAH_GAJI == null ?
        //            0 : decimal.Round(Convert.ToDecimal(det.HR_JUMLAH_GAJI) / totalBulan, 3);
        //    }
        //    det.HR_MUKTAMAD = 0;

        //    return det;
        //}
    }
}