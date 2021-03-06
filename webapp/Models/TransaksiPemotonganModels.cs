﻿using System;
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
        public static void InsertSpgPotongan(ApplicationDbContext sppDb, SPGContext spgDb, int tahunDibayar, int bulanDibayar)
        {
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans =
                HR_TRANSAKSI_SAMBILAN_DETAIL
                .GetTransaksiDibayar(sppDb, tahunDibayar, bulanDibayar);

            if (sppTrans != null)
            {
                InsertToSPG(sppDb, spgDb, sppTrans);
            }
        }

        private static void InsertToSPG(ApplicationDbContext sppDb, SPGContext spgDb,
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans)
        {
            List<string> noPekerja_all =
                sppTrans.Select(s => s.HR_NO_PEKERJA).Distinct().ToList();
            int bulanDibayar = sppTrans.Where(s => s.HR_KOD == "GAJPS").Select(s => s.HR_BULAN_DIBAYAR).FirstOrDefault();
            int tahunDibayar = sppTrans.Where(s => s.HR_KOD == "GAJPS").Select(s => s.HR_TAHUN).FirstOrDefault();

            foreach (string noPekerja in noPekerja_all)
            {
                //get List of transaksi by No Pekerja
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTransData =
                    sppTrans.Where(s => s.HR_NO_PEKERJA == noPekerja).ToList();
                var listElaun = sppTransData.Where(s => s.HR_KOD_IND == "P").ToList();

                foreach (string kodpemotongan in listElaun.Select(s => s.HR_KOD).ToList())
                {
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> sKod =
                        sppTransData.Where(s => s.HR_KOD == kodpemotongan).ToList();
                    PA_TRANSAKSI_PEMOTONGAN spgTrans = spgDb.PA_TRANSAKSI_PEMOTONGAN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_POTONGAN == tahunDibayar
                    && s.PA_BULAN_POTONGAN == bulanDibayar
                    && s.PA_KOD_PEMOTONGAN == kodpemotongan).FirstOrDefault();
                    var jumlahPemotongan = sKod.Select(s => s.HR_JUMLAH).Sum();
                    var votPemotongan = GetKodVOT(noPekerja, kodpemotongan);

                    if (spgTrans != null)
                    {
                        //update data
                        //spgTrans.PA_NO_PEKERJA = noPekerja;
                        //spgTrans.PA_KOD_PEMOTONGAN = kodElaun;
                        spgTrans.PA_JUMLAH_PEMOTONGAN = jumlahPemotongan;
                        //spgTrans.PA_BULAN_POTONGAN = (byte)bulanDibayar;
                        //spgTrans.PA_TAHUN_POTONGAN = (short)tahunDibayar;
                        spgTrans.PA_PROSES_IND = "P";
                        spgTrans.PA_TARIKH_PROSES = DateTime.Now;
                        //spgTrans.PA_VOT_PEMOTONGAN = "VOT";
                        spgTrans.PA_TARIKH_KEYIN = DateTime.Now;                        
                        try
                        {
                            spgDb.Entry(spgTrans).State = EntityState.Modified;
                            spgDb.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else
                    {
                        //insert data
                        spgTrans = new PA_TRANSAKSI_PEMOTONGAN
                        {
                            PA_NO_PEKERJA = noPekerja,
                            PA_KOD_PEMOTONGAN = kodpemotongan,
                            PA_JUMLAH_PEMOTONGAN = jumlahPemotongan,
                            PA_BULAN_POTONGAN = (byte)bulanDibayar,
                            PA_TAHUN_POTONGAN = (short)tahunDibayar,
                            PA_PROSES_IND = "P",
                            PA_TARIKH_PROSES = DateTime.Now,
                            PA_VOT_PEMOTONGAN = votPemotongan,
                            PA_TARIKH_KEYIN = DateTime.Now
                        };
                        spgDb.PA_TRANSAKSI_PEMOTONGAN.Add(spgTrans);
                        spgDb.SaveChanges();
                    }
                }
            }
        }

        private static string GetKodVOT(string noPekerja, string kodPemotongan)
        {
            string retString = "11-00-00-00-00000";
            ApplicationDbContext db = new ApplicationDbContext();
            HR_MAKLUMAT_PEKERJAAN mWork = db.HR_MAKLUMAT_PEKERJAAN.Where
                (s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            HR_POTONGAN mPotongan = db.HR_POTONGAN.Where
                (s => s.HR_KOD_POTONGAN == kodPemotongan).FirstOrDefault();
            if (mWork != null && mPotongan != null)
            {
                string cropString = string.Empty;
                if(mPotongan.HR_VOT_POTONGAN.Length > 5)
                {
                    var indexChar = mPotongan.HR_VOT_POTONGAN.Length - 5;
                    cropString = mPotongan.HR_VOT_POTONGAN.Substring(indexChar, 5);
                }
                else
                {
                    cropString = mPotongan.HR_VOT_POTONGAN;
                }

                if(cropString.Length == 0)
                {
                    cropString = "00000";
                }

                retString = string.Format("{0}-{1}-{2}-{3}-{4}",
                    PageSejarahModel.NoVOTKepala,
                    mWork.HR_JABATAN,
                    mWork.HR_BAHAGIAN,
                    mWork.HR_UNIT,
                    cropString);
            }
            return retString;
        }
    }
}