using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class CariPekerjaModel
    {
        public string NoPekerja { get; set; }
        public string Nama { get; set; }
        public string NoKPBaru { get; set; }
        public string Gambar { get; set; }
        public string Jantina { get; set; }

        public static List<CariPekerjaModel> GetPekerja(string jenis, string value)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            if (jenis == "1")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NO_PEKERJA == value
                    && s.HR_AKTIF_IND == "Y").ToList();
            }
            else if (jenis == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NAMA_PEKERJA.Contains(value)
                    && s.HR_AKTIF_IND == "Y").ToList();
            }
            else
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NO_KPBARU.Contains(value)
                    && s.HR_AKTIF_IND == "Y").ToList();
            }

            List<CariPekerjaModel> results = new List<CariPekerjaModel>();
            foreach (HR_MAKLUMAT_PERIBADI var2 in mPeribadi)
            {
                CariPekerjaModel pekerja = new CariPekerjaModel();
                pekerja.NoPekerja = var2.HR_NO_PEKERJA;
                pekerja.Nama = var2.HR_NAMA_PEKERJA;
                pekerja.NoKPBaru = var2.HR_NO_KPBARU;
                pekerja.Jantina = var2.HR_JANTINA;

                HR_GAMBAR_PENGGUNA gambarDb = db.HR_GAMBAR_PENGGUNA
                    .Where(s => s.HR_NO_PEKERJA == pekerja.NoPekerja).FirstOrDefault();
                if (gambarDb != null)
                {
                    string photoName = gambarDb.HR_PHOTO + gambarDb.HR_FORMAT_TYPE;
                    string fullPath = HttpContext.Current.Server.MapPath("~/Content/uploads/" + photoName);
                    pekerja.Gambar = fullPath;
                }
                results.Add(pekerja);
            }

            return results;
        }
    }

    public class TanggunganModel
    {
        public int MyProperty { get; set; }
    }
}