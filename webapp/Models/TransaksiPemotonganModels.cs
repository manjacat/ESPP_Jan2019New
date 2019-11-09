using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiPemotonganModels
    {
        public virtual DbSet<PA_TRANSAKSI_PEMOTONGAN> PA_TRANSAKSI_PEMOTONGAN { get; set; }
    }

    public partial class PA_TRANSAKSI_PEMOTONGAN
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PA_KOD_PEMOTONGAN { get; set; }
        public System.DateTime PA_TARIKH_PROSES { get; set; }
        public Nullable<decimal> PA_JUMLAH_PEMOTONGAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public byte PA_BULAN_POTONGAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public short PA_TAHUN_POTONGAN { get; set; }
        public string PA_PROSES_IND { get; set; }
        public string PA_VOT_PEMOTONGAN { get; set; }
        public Nullable<System.DateTime> PA_TARIKH_KEYIN { get; set; }

        //TODO InsertData
        public static void InsertSpgPotongan(ApplicationDbContext sppDb, SPGContext spgDb, int tahun, int bulan)
        {
            //int tahun = 2019;
            //int bulan = 4;

            List<HR_TRANSAKSI_SAMBILAN_DETAIL> gajiAll =
               sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
               .Where(s => s.HR_KOD == "GAJPS"
               && s.HR_TAHUN == tahun
               && s.HR_BULAN_DIBAYAR == bulan).ToList();

            foreach (var item in gajiAll)
            {
                string noPekerja = item.HR_NO_PEKERJA;
                int bulanBekerja = item.HR_BULAN_BEKERJA;
                int bulanDibayar = item.HR_BULAN_DIBAYAR;
                int tahunBekerja = item.HR_TAHUN_BEKERJA;
                int tahunDibayar = item.HR_TAHUN;

                //InsertP005(sppDb, spgDb, noPekerja, tahun, bulan);
                //InsertP0163(sppDb, spgDb, noPekerja, tahun, bulan);
                DeleteSpgPotongan(sppDb, spgDb, noPekerja, 
                    tahunBekerja, bulanBekerja);
                ReinsertSpgPotongan(sppDb, spgDb, noPekerja, 
                    tahunBekerja, bulanBekerja);
            }

        }

        private static void DeleteSpgPotongan(ApplicationDbContext sppDb, SPGContext spgDb,
            string noPekerja, int tahun, int bulan)
        {
            List<PA_TRANSAKSI_PEMOTONGAN> spgPotongan = spgDb.PA_TRANSAKSI_PEMOTONGAN
                  .Where(s => s.PA_NO_PEKERJA == noPekerja
                  && s.PA_TAHUN_POTONGAN == tahun
                  && s.PA_BULAN_POTONGAN == bulan).ToList();
            if(spgPotongan.Count > 0)
            {
                try
                {
                    spgDb.PA_TRANSAKSI_PEMOTONGAN.RemoveRange(spgPotongan);
                    spgDb.SaveChanges();
                }
                catch(Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        private static void ReinsertSpgPotongan(ApplicationDbContext sppDb, SPGContext spgDb,
            string noPekerja, int tahun, int bulan)
        {
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppPotongan =
                sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_NO_PEKERJA == noPekerja
                && s.HR_TAHUN_BEKERJA == tahun
                && s.HR_BULAN_BEKERJA == bulan
                && s.HR_KOD_IND == "P").ToList();

            //get potongan SOCSO sebab potongan SOCSO takda dalam DB
            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans =
            //HR_TRANSAKSI_SAMBILAN_DETAIL.GetTransaksiDibayar(sppDb, tahun, bulan);

            //decimal totalElaunOT = sppTrans
            //   .Where(s => s.HR_KOD_IND == "E"
            //   && s.HR_KOD == "E0164"
            //   && s.HR_NO_PEKERJA == noPekerja)
            //   .Select(s => s.HR_JUMLAH).Sum().Value;

            //decimal gajiPokok = sppTrans
            //    .Where(s => s.HR_KOD == "GAJPS"
            //    && s.HR_NO_PEKERJA == noPekerja)
            //    .Select(s => s.HR_JUMLAH).Sum().Value;

            //decimal potonganSocso = PageSejarahModel
            //   .GetPotonganSocso(sppDb, gajiPokok, totalElaunOT);
            //decimal jumlahSocso = 0;

            ////add potonganSocso to sppPotongan
            //HR_TRANSAKSI_SAMBILAN_DETAIL sppSocso = new HR_TRANSAKSI_SAMBILAN_DETAIL
            //{
            //    HR_NO_PEKERJA = noPekerja,
            //    HR_BULAN_DIBAYAR = bulan,
            //    HR_TAHUN = tahun,
            //    HR_JAM_HARI = 0,
            //    HR_JUMLAH = potonganSocso,
            //    HR_KOD = "P0163",
            //    HR_KOD_IND = "P",
            //    HR_MUKTAMAD = null
            //};

            //sppPotongan.Add(sppSocso);
            //jumlahSocso = sppSocso.HR_JUMLAH.Value;

            var sppPotonganGroup = sppPotongan
                .GroupBy(s => s.HR_KOD).ToList();
            
            //get total potongan
            decimal jumlahPotongan = sppPotongan
                .Select(s => s.HR_JUMLAH).Sum().Value;

            List<PA_TRANSAKSI_PEMOTONGAN> spgPotonganList = 
                new List<PA_TRANSAKSI_PEMOTONGAN>();

            foreach(var p in sppPotonganGroup)
            {
                //insert
                string test = p.Select(s => s.HR_KOD)
                    .FirstOrDefault();

                PA_TRANSAKSI_PEMOTONGAN spgPotongan = new PA_TRANSAKSI_PEMOTONGAN
                {
                    PA_NO_PEKERJA = noPekerja,
                    PA_KOD_PEMOTONGAN = p.Select(s => s.HR_KOD).FirstOrDefault(),
                    PA_TARIKH_PROSES = DateTime.Now,
                    PA_JUMLAH_PEMOTONGAN = p.Select(s => s.HR_JUMLAH).Sum().Value,
                    PA_BULAN_POTONGAN = (byte)bulan,
                    PA_TAHUN_POTONGAN = (short)tahun,
                    PA_PROSES_IND = "P",
                    //ambik dari HR_POTONGAN.HR_VOT_POTONGAN_P
                    //PA_VOT_PEMOTONGAN = "41-02-01-00-03501",
                    PA_VOT_PEMOTONGAN = "41-02-01-00-03502",
                    PA_TARIKH_KEYIN = DateTime.Now,
                };
                spgDb.PA_TRANSAKSI_PEMOTONGAN.Add(spgPotongan);
                spgDb.SaveChanges();
                spgPotonganList.Add(spgPotongan);
            }
            //if(spgPotonganList.Count > 0)
            //{
            //    //spgDb.PA_TRANSAKSI_PEMOTONGAN.AddRange(spgPotonganList);
            //    //spgDb.SaveChanges();
            //    //try
            //    //{
            //    //    spgDb.PA_TRANSAKSI_PEMOTONGAN.AddRange(spgPotonganList);
            //    //    spgDb.SaveChanges();
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    Console.Write(ex.ToString());
            //    //}
            //}

        }


        private static void InsertP0163(ApplicationDbContext sppDb, SPGContext spgDb,
            string noPekerja, int tahun, int bulan)
        {
            //PA_TRANSAKSI_PEMOTONGAN spgPotongan = spgDb.PA_TRANSAKSI_PEMOTONGAN
            //      .Where(s => s.PA_NO_PEKERJA == noPekerja
            //      && s.PA_TAHUN_POTONGAN == tahun
            //      && s.PA_BULAN_POTONGAN == bulan
            //      && s.PA_KOD_PEMOTONGAN == "P0163").FirstOrDefault();

            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppPotongan = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
            //    .Where(s => s.HR_NO_PEKERJA == noPekerja
            //    && s.HR_TAHUN == tahun
            //    && s.HR_BULAN_DIBAYAR == bulan
            //    && s.HR_KOD_IND == "P").ToList();

            ////get potongan SOCSO sebab potongan SOCSO takda dalam DB
            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans =
            //HR_TRANSAKSI_SAMBILAN_DETAIL.GetTransaksi(sppDb, tahun, bulan);

            //decimal totalElaunOT = sppTrans
            //   .Where(s => s.HR_KOD_IND == "E"
            //   && s.HR_KOD == "E0164"
            //   && s.HR_NO_PEKERJA == noPekerja)
            //   .Select(s => s.HR_JUMLAH).Sum().Value;

            //decimal gajiPokok = sppTrans
            //    .Where(s => s.HR_KOD == "GAJPS"
            //    && s.HR_NO_PEKERJA == noPekerja)
            //    .Select(s => s.HR_JUMLAH).Sum().Value;

            ////decimal potonganSocso = PageSejarahModel
            ////   .GetPotonganSocso(sppDb, gajiPokok, totalElaunOT);

            ////add potonganSocso to sppPotongan
            //HR_TRANSAKSI_SAMBILAN_DETAIL sppSocso = new HR_TRANSAKSI_SAMBILAN_DETAIL
            //{
            //    HR_NO_PEKERJA = noPekerja,
            //    HR_BULAN_DIBAYAR = bulan,
            //    HR_TAHUN = tahun,
            //    HR_JAM_HARI = 0,
            //    HR_JUMLAH = potonganSocso,
            //    HR_KOD = "P0163",
            //    HR_KOD_IND = "P",
            //    HR_MUKTAMAD = null
            //};
            
            //sppPotongan.Add(sppSocso);

            //decimal jumlahSocso = sppSocso.HR_JUMLAH.Value;
            ////get total potongan
            //decimal jumlahPotongan = sppPotongan.Select(s => s.HR_JUMLAH).Sum().Value;

            //if (spgPotongan == null)
            //{
            //    //insert
            //    spgPotongan = new PA_TRANSAKSI_PEMOTONGAN
            //    {
            //        PA_NO_PEKERJA = noPekerja,
            //        PA_KOD_PEMOTONGAN = "P0163",
            //        PA_TARIKH_PROSES = DateTime.Now,
            //        PA_JUMLAH_PEMOTONGAN = jumlahSocso,
            //        PA_BULAN_POTONGAN = (byte)bulan,
            //        PA_TAHUN_POTONGAN = (short)tahun,
            //        PA_PROSES_IND = "P",
            //        //ambik dari HR_POTONGAN.HR_VOT_POTONGAN_P
            //        PA_VOT_PEMOTONGAN = "41-02-01-00-03501",
            //        PA_TARIKH_KEYIN = DateTime.Now,
            //    };
            //    try
            //    {
            //        spgDb.PA_TRANSAKSI_PEMOTONGAN.Add(spgPotongan);
            //        spgDb.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.Write(ex.ToString());
            //    }
            //}
            //else
            //{
            //    //update
            //    spgPotongan.PA_NO_PEKERJA = noPekerja;
            //    spgPotongan.PA_KOD_PEMOTONGAN = "P0163";
            //    spgPotongan.PA_TARIKH_PROSES = DateTime.Now;
            //    spgPotongan.PA_JUMLAH_PEMOTONGAN = jumlahSocso;
            //    spgPotongan.PA_BULAN_POTONGAN = (byte)bulan;
            //    spgPotongan.PA_TAHUN_POTONGAN = (short)tahun;
            //    spgPotongan.PA_PROSES_IND = "P";
            //    spgPotongan.PA_VOT_PEMOTONGAN = "41-02-01-00-03501";
            //    try
            //    {
            //        spgDb.Entry(spgPotongan).State = EntityState.Modified;
            //        spgDb.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }
            //}
        }

        private static void InsertP005(ApplicationDbContext sppDb, SPGContext spgDb,
            string noPekerja, int tahun, int bulan)
        {
            //PA_TRANSAKSI_PEMOTONGAN spgPotongan = spgDb.PA_TRANSAKSI_PEMOTONGAN
            //       .Where(s => s.PA_NO_PEKERJA == noPekerja
            //       && s.PA_TAHUN_POTONGAN == tahun
            //       && s.PA_BULAN_POTONGAN == bulan
            //       && s.PA_KOD_PEMOTONGAN == "P0005").FirstOrDefault();

            //List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppPotongan = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
            //    .Where(s => s.HR_NO_PEKERJA == noPekerja
            //    && s.HR_TAHUN == tahun
            //    && s.HR_BULAN_DIBAYAR == bulan
            //    && s.HR_KOD_IND == "P").ToList();

            ////get potongan SOCSO sebab potongan SOCSO takda dalam DB
            ////List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans =
            ////HR_TRANSAKSI_SAMBILAN_DETAIL.GetTransaksi(sppDb, tahun, bulan);

            ////decimal totalElaunOT = sppTrans
            ////   .Where(s => s.HR_KOD_IND == "E"
            ////   && s.HR_KOD == "E0164"
            ////   && s.HR_NO_PEKERJA == noPekerja)
            ////   .Select(s => s.HR_JUMLAH).Sum().Value;

            ////decimal gajiPokok = sppTrans
            ////    .Where(s => s.HR_KOD == "GAJPS"
            ////    && s.HR_NO_PEKERJA == noPekerja)
            ////    .Select(s => s.HR_JUMLAH).Sum().Value;

            ////decimal potonganSocso = PageSejarahModel
            ////   .GetPotonganSocso(sppDb, gajiPokok, totalElaunOT);

            ////add potonganSocso to sppPotongan
            ////HR_TRANSAKSI_SAMBILAN_DETAIL sppSocso = new HR_TRANSAKSI_SAMBILAN_DETAIL
            ////{
            ////    HR_NO_PEKERJA = noPekerja,
            ////    HR_BULAN_DIBAYAR = bulan,
            ////    HR_TAHUN = tahun,
            ////    HR_JAM_HARI = 0,
            ////    HR_JUMLAH = potonganSocso,
            ////    HR_KOD = "P0163",
            ////    HR_KOD_IND = "P",
            ////    HR_MUKTAMAD = null
            ////};

            ////sppPotongan.Add(sppSocso);

            ////get total potongan
            //decimal jumlahPotongan = sppPotongan.Select(s => s.HR_JUMLAH).Sum().Value;

            //if (spgPotongan == null)
            //{
            //    //insert
            //    spgPotongan = new PA_TRANSAKSI_PEMOTONGAN
            //    {
            //        PA_NO_PEKERJA = noPekerja,
            //        PA_KOD_PEMOTONGAN = "P0005",
            //        PA_TARIKH_PROSES = DateTime.Now,
            //        PA_JUMLAH_PEMOTONGAN = jumlahPotongan,
            //        PA_BULAN_POTONGAN = (byte)bulan,
            //        PA_TAHUN_POTONGAN = (short)tahun,
            //        PA_PROSES_IND = "P",
            //        //ambik dari HR_POTONGAN.HR_VOT_POTONGAN_P
            //        PA_VOT_PEMOTONGAN = "41-02-01-00-03501",
            //        PA_TARIKH_KEYIN = DateTime.Now,
            //    };
            //    try
            //    {
            //        spgDb.PA_TRANSAKSI_PEMOTONGAN.Add(spgPotongan);
            //        spgDb.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.Write(ex.ToString());
            //    }
            //}
            //else
            //{
            //    //update
            //    spgPotongan.PA_NO_PEKERJA = noPekerja;
            //    spgPotongan.PA_KOD_PEMOTONGAN = "P0005";
            //    spgPotongan.PA_TARIKH_PROSES = DateTime.Now;
            //    spgPotongan.PA_JUMLAH_PEMOTONGAN = jumlahPotongan;
            //    spgPotongan.PA_BULAN_POTONGAN = (byte)bulan;
            //    spgPotongan.PA_TAHUN_POTONGAN = (short)tahun;
            //    spgPotongan.PA_PROSES_IND = "P";
            //    spgPotongan.PA_VOT_PEMOTONGAN = "41-02-01-00-03501";
            //    try
            //    {
            //        spgDb.Entry(spgPotongan).State = EntityState.Modified;
            //        spgDb.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }
            //}
        }
    }
}