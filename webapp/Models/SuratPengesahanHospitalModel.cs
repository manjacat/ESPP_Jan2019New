using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Models
{
    public class SuratPengesahanHospitalModel
    {
        public SuratPengesahanHospitalModel()
        {
            //SearchResult = new List<CariPekerjaModel>();
            Jawatan = string.Empty;
            MaklumatTanggungan = new List<MaklumatTanggunganModel>();
        }

        //public List<CariPekerjaModel> SearchResult { get; set; }

        public string NamaPekerja { get; set; }
        public string NoPekerja { get; set; }
        public string NoKPBaru { get; set; }
        public string Jawatan { get; set; }
        public string GredGaji { get; set; }
        public decimal GajiBulanan { get; set; }
        public bool IsRawatanSendiri { get; set; }
        public string TarikhString { get; set; }
        public bool IsHospital { get; set; }
        public string HospitalName { get; set; }
        public bool IsPengesahanMajikan { get; set; }

        public List<MaklumatTanggunganModel> MaklumatTanggungan { get; set; }

        public static SuratPengesahanHospitalModel GetByNoPekerja(string noPekerja)
        {
            SuratPengesahanHospitalModel model = new SuratPengesahanHospitalModel();
            ApplicationDbContext db = new ApplicationDbContext();
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI
                .Where(s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            List<HR_MAKLUMAT_TANGGUNGAN> mTanggungan = db.HR_MAKLUMAT_TANGGUNGAN
                .Where(s => s.HR_NO_PEKERJA == noPekerja).ToList();

            model.NamaPekerja = mPeribadi.HR_NAMA_PEKERJA;
            model.NoPekerja = noPekerja;
            model.NoKPBaru = mPeribadi.HR_NO_KPBARU;


            if(mPekerjaan.HR_JAWATAN != null)
            {
                string jawatanString = db.HR_JAWATAN
                    .Where(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN)
                    .Select(s => s.HR_NAMA_JAWATAN).FirstOrDefault();
                model.Jawatan = jawatanString;
            }
            model.GredGaji = mPekerjaan.HR_GRED;
            model.GajiBulanan = mPekerjaan.HR_GAJI_POKOK == null? 0: mPekerjaan.HR_GAJI_POKOK.Value;
            model.IsRawatanSendiri = true;
            model.TarikhString = DateTime.Now.ToString("dd/MM/yyyy");
            model.IsHospital = true;
            model.HospitalName = string.Empty;
            model.IsPengesahanMajikan = false;

            model.MaklumatTanggungan = MaklumatTanggunganModel.GetListTanggungan(mTanggungan);

            return model;
        }
    }

    public class MaklumatTanggunganModel
    {
        public string Nama { get; set; }
        public string Hubungan { get; set; }
        public string NoKP { get; set; }

        public static List<MaklumatTanggunganModel> GetListTanggungan(List<HR_MAKLUMAT_TANGGUNGAN> dbList)
        {
            MajlisContext db2 = new MajlisContext();
            List<MaklumatTanggunganModel> outputList = new List<MaklumatTanggunganModel>();
            foreach(HR_MAKLUMAT_TANGGUNGAN single in dbList)
            {
                MaklumatTanggunganModel o = new MaklumatTanggunganModel
                {
                    Nama = single.HR_NAMA_TANGGUNGAN,
                    NoKP = single.HR_NO_KP
                };

                int hubunganInt = Convert.ToInt32(single.HR_HUBUNGAN);

                string hubunganString = db2.GE_PARAMTABLE
                    .Where(s => s.GROUPID == 125 
                    && s.ORDINAL == hubunganInt)
                    .Select(s => s.SHORT_DESCRIPTION)
                    .FirstOrDefault();
                o.Hubungan = hubunganString;

                outputList.Add(o);
            }
            return outputList;
        }
    }
}