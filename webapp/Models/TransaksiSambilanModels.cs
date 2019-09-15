using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TraksaksiSambilanModels
    {
		public TraksaksiSambilanModels() { }
		public virtual DbSet<HR_TRANSAKSI_SAMBILAN> HR_TRANSAKSI_SAMBILAN { get; set; }

        //will be used in to Store claculation Related to Transaksi Sambilan

        public static AgreementModels Maklumat(string HR_PEKERJA,
            int? tahunbekerja,
            int? bulanbekerja,
            int? tahundibayar,
            int? bulandibayar,
            string tahunbulannotis, string tahunbulanbonus, string bulankiradari, string bulankirahingga,
            decimal? jumlahot, int? jumlahhari)
        {
            if(jumlahhari == null)
            {
                jumlahhari = 0;
            }

            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            int consHariBekerja = PageSejarahModel.ConstHariBekerja; //value is 21

            //Cari Maklumat Pekerjaan
            HR_MAKLUMAT_PEKERJAAN userMaklumat = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();
            List<string> listElaunKa = PageSejarahModel.ListElaunKa;

            if (userMaklumat != null)
            {
                GE_JABATAN userJabatan = mc.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == userMaklumat.HR_JABATAN).SingleOrDefault();
                GE_BAHAGIAN userBahagian = mc.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == userMaklumat.HR_BAHAGIAN && s.GE_KOD_JABATAN == userMaklumat.HR_JABATAN).SingleOrDefault();

                //dapatkan Transaksi Sambilan Detail. ada banyak kod: C0020, E0064, E0164, E0234, GAJPS, P0015, P0035
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> userTransaksiDetail = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == HR_PEKERJA 
                    && s.HR_BULAN_BEKERJA == bulanbekerja 
                    && s.HR_BULAN_DIBAYAR == bulandibayar 
                    && s.HR_TAHUN == tahundibayar 
                    && s.HR_TAHUN_BEKERJA == tahunbekerja).ToList();

                HR_MAKLUMAT_PERIBADI userPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_MAKLUMAT_PEKERJAAN userPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_PEKERJA);
                HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == userPekerjaan.HR_JAWATAN);              

                HR_MAKLUMAT_PEKERJAAN mpekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_PEKERJA).SingleOrDefault();

                AgreementModels kerjaelaun = new AgreementModels();

                //if (transaksisambilandetail.Count != 0 && transaksisambilan.Count != 0)
                if (userTransaksiDetail.Count != 0)
                {

                    //kalau ada tranaksi, ambik gaji dan potongan dari tranasksi
                    decimal gajiSehari = PageSejarahModel.GetGajiSehari(db, HR_PEKERJA);
                    HR_TRANSAKSI_SAMBILAN_DETAIL gaji = userTransaksiDetail.SingleOrDefault(s => s.HR_KOD_IND == "G");
                    kerjaelaun.GAJISEHARI = gajiSehari.ToString("0.00");
                    kerjaelaun.GAJIPOKOK = gaji.HR_JUMLAH;                   
                    kerjaelaun.GAJIPER3 = (gaji.HR_JUMLAH / 3).Value.ToString("0.00");

                    //elaun
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunka = userTransaksiDetail
                        .Where(s => listElaunKa.Contains(s.HR_KOD)).ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunot = userTransaksiDetail
                        .Where(s => s.HR_KOD == "E0164").ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain = userTransaksiDetail
                        .Where(s => s.HR_KOD_IND == "E" 
                        && (!listElaunKa.Contains(s.HR_KOD) && s.HR_KOD != "E0164")).ToList();

                    //potongan
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganksdk = userTransaksiDetail
                        .Where(s => s.HR_KOD == "P0015").ToList();
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> potonganlain = userTransaksiDetail
                        .Where(s => s.HR_KOD_IND == "P" && (s.HR_KOD != "P0015" && s.HR_KOD != "P0035")).ToList();
                    HR_MAKLUMAT_ELAUN_POTONGAN potonganKWSP = 
                        PageSejarahModel.GetPotonganKWSP(db, HR_PEKERJA, kerjaelaun.GAJIPOKOK.Value)
                        .Where(s => s.HR_ELAUN_POTONGAN_IND == "P").FirstOrDefault();
                                                            
                    kerjaelaun.JABATAN = userJabatan.GE_KETERANGAN_JABATAN;
                    kerjaelaun.BAHAGIAN = userBahagian.GE_KETERANGAN;
                    kerjaelaun.NOKP = userPeribadi.HR_NO_KPBARU;
                    kerjaelaun.JAWATAN = jawatan.HR_NAMA_JAWATAN;

                    //check for null
                    if(elaunot.FirstOrDefault() != null)
                    {
                        kerjaelaun.JAMBEKERJA = elaunot.FirstOrDefault().HR_JAM_HARI;
                    }
                    else
                    {
                        kerjaelaun.JAMBEKERJA = 0;
                    }
                    
                    kerjaelaun.HARIBEKERJA = gaji.HR_JAM_HARI;                    

                    //elaun
                    kerjaelaun.ELAUNKA = elaunka.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.JUMLAHBAYARANOT = elaunot.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.ELAUNLAIN = elaunlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");

                    //potongan
                    decimal potonganSocso = PageSejarahModel.GetPotonganSocso(db, gaji.HR_JUMLAH.Value, elaunot.Sum(s => s.HR_JUMLAH).Value);
                    kerjaelaun.POTONGANSOCSO = potonganSocso;
                    kerjaelaun.POTONGANKWSP = potonganKWSP.HR_JUMLAH;
                    kerjaelaun.POTONGANKSDK = potonganksdk.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.POTONGLAIN = potonganlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    
                    kerjaelaun.TUNGGAKANIND = userTransaksiDetail
                        .Where(s => s.HR_KOD == "GAJPS").SingleOrDefault().HR_TUNGGAKAN_IND;

                    //calculation
                    var totalElaunka = elaunka.Sum(s => s.HR_JUMLAH);
                    var totalElaunLain = elaunlain.Sum(s => s.HR_JUMLAH);
                    var totalElaunot = elaunot.Sum(s => s.HR_JUMLAH);
                    decimal gajikasar = PageSejarahModel.GetGajiKasar
                        (gaji.HR_JUMLAH.Value, 
                        totalElaunka, 
                        totalElaunLain, 
                        totalElaunot.Value, kerjaelaun.HARIBEKERJA.Value);
                    kerjaelaun.GAJIKASAR = gajikasar.ToString("0.00");
                    var gajiSebelumKWSP = gajikasar
                      - potonganSocso
                      - potonganksdk.Sum(s => s.HR_JUMLAH)
                      - potonganlain.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJISEBELUMKWSP = gajiSebelumKWSP.Value.ToString("0.00");
                    //gaji bersih = gaji pokok + elaun - potongan
                    var bersih = gajikasar
                        - potonganSocso
                        - potonganKWSP.HR_JUMLAH
                        - potonganksdk.Sum(s => s.HR_JUMLAH)
                        - potonganlain.Sum(s => s.HR_JUMLAH);

                    kerjaelaun.GAJIBERSIH = decimal.Parse(bersih.Value.ToString("0.00"));
                    kerjaelaun.MUKTAMAD = gaji.HR_MUKTAMAD;

                    return kerjaelaun;
                }
                else if (userTransaksiDetail.Count == 0)
                {
                    //gaji related
                    var gajiSehari = PageSejarahModel.GetGajiSehari(db, HR_PEKERJA);
                    kerjaelaun.GAJISEHARI = gajiSehari.ToString("0.00");
                    decimal gajiPokok = PageSejarahModel.GetGajiPokok(db, HR_PEKERJA, jumlahhari.Value);
                    kerjaelaun.GAJIPOKOK = gajiPokok;
                    decimal? gajiper3 = kerjaelaun.GAJIPOKOK / 3;
                    kerjaelaun.GAJIPER3 = gajiper3.Value.ToString("0.00");

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganksdk = PageSejarahModel.GetPotonganKSDK(db, HR_PEKERJA);
                    List<HR_MAKLUMAT_ELAUN_POTONGAN> potonganlain = PageSejarahModel.GetPotonganLain(db, HR_PEKERJA);
                    HR_MAKLUMAT_ELAUN_POTONGAN potonganKWSP = PageSejarahModel.GetPotonganKWSP(db, HR_PEKERJA, kerjaelaun.GAJIPOKOK.Value)
                        .Where(s => s.HR_ELAUN_POTONGAN_IND == "P").FirstOrDefault();
                    List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunka = PageSejarahModel.GetElaunKa(db, HR_PEKERJA);
                    List<HR_MAKLUMAT_ELAUN_POTONGAN> elaunLain = PageSejarahModel.GetElaunLain(db, HR_PEKERJA);
                    
                    //semua Elaun
                    int jumlahHariInt = Convert.ToInt32(jumlahhari != null ? jumlahhari : 0);
                    decimal jumlahJamOT = Convert.ToDecimal(jumlahot != null ? jumlahot : 0);
                    var elaunot = PageSejarahModel.GetElaunOT(db, HR_PEKERJA, jumlahHariInt, jumlahJamOT);
                    kerjaelaun.JUMLAHBAYARANOT = elaunot.ToString("0.00");
                    kerjaelaun.ELAUNKA = elaunka.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.ELAUNLAIN = elaunLain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");

                    //semua potongan
                    decimal potonganSocso = PageSejarahModel.GetPotonganSocso(db, gajiPokok, elaunot);
                    kerjaelaun.POTONGANSOCSO = potonganSocso;
                    kerjaelaun.POTONGANKWSP = potonganKWSP.HR_JUMLAH;
                    kerjaelaun.POTONGANKSDK = potonganksdk.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");
                    kerjaelaun.POTONGLAIN = potonganlain.Sum(s => s.HR_JUMLAH).Value.ToString("0.00");

                    //other info
                    kerjaelaun.JABATAN = userJabatan.GE_KETERANGAN_JABATAN; 
                    kerjaelaun.BAHAGIAN = userBahagian.GE_KETERANGAN;
                    kerjaelaun.NOKP = userPeribadi.HR_NO_KPBARU;
                    kerjaelaun.JAWATAN = jawatan.HR_NAMA_JAWATAN;

                    var totalElaunka = elaunka.Sum(s => s.HR_JUMLAH);
                    var totalElaunLain = elaunLain.Sum(s => s.HR_JUMLAH);
                    decimal gajikasar = PageSejarahModel
                        .GetGajiKasar(gajiPokok, totalElaunka, totalElaunLain, elaunot, jumlahHariInt);
                    kerjaelaun.GAJIKASAR = gajikasar.ToString("0.00");
                    //double? gajikasar1 = (double)gajikasar * 0.11;
                    var gajiSebelumKWSP = gajikasar
                      - potonganSocso
                      - potonganksdk.Sum(s => s.HR_JUMLAH)
                      - potonganlain.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJISEBELUMKWSP = gajiSebelumKWSP.Value.ToString("0.00");
                    //gaji bersih = gaji pokok + elaun - potongan
                    var bersih = gajikasar
                        - potonganSocso
                        - potonganKWSP.HR_JUMLAH
                        - potonganksdk.Sum(s => s.HR_JUMLAH)
                        - potonganlain.Sum(s => s.HR_JUMLAH);
                    kerjaelaun.GAJIBERSIH = decimal.Parse(bersih.Value.ToString("0.00"));
                        return kerjaelaun;
                }
            }
            return null;
        }
    }
	public partial class HR_TRANSAKSI_SAMBILAN
	{
		[Key]
		public string HR_NO_PEKERJA { get; set; }
		public int HR_BULAN_DIBAYAR { get; set; }
		public int HR_TAHUN { get; set; }
		public int HR_BULAN_BEKERJA { get; set; }
		public int HR_TAHUN_BEKERJA { get; set; }
	}
}