using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKakitanganModels
    {
        public string ACTIVE_ITEM { get; set; }
        public List<MaklumatPeribadi> HR_SENARAI_PERIBADI { get; set; }
        public MaklumatPeribadi HR_MAKLUMAT_PERIBADI { get; set; }
        public HR_GAMBAR_PENGGUNA HR_GAMBAR_PENGGUNA { get; set; }
        public MaklumatPekerjaan HR_MAKLUMAT_PEKERJAAN { get; set; }
        //public IEnumerable<MaklumatPengalamanKerja> HR_MAKLUMAT_PENGALAMAN_KERJA_MPPJ { get; set; }
        public IEnumerable<HR_MAKLUMAT_PEKERJAAN_HISTORY> HR_MAKLUMAT_PEKERJAAN_HISTORY { get; set; }
        public IEnumerable<MaklumatPengalamanKerja> HR_MAKLUMAT_PENGALAMAN_KERJA { get; set; }
        public IEnumerable<MaklumatPewaris> HR_MAKLUMAT_PEWARIS { get; set; }

        public IEnumerable<MaklumatKemahiranBahasa> HR_MAKLUMAT_KEMAHIRAN_BAHASA { get; set; }
        public IEnumerable<MaklumatKemahiranTeknikal> HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL { get; set; }
        public IEnumerable<MaklumatKelayakan> HR_MAKLUMAT_KELAYAKAN { get; set; }
        public IEnumerable<MaklumatSijil> HR_MAKLUMAT_SIJIL { get; set; }
        public IEnumerable<MaklumatKursusLatihan> HR_MAKLUMAT_KURSUS_LATIHAN { get; set; }
        public IEnumerable<MaklumatAktiviti> HR_MAKLUMAT_AKTIVITI { get; set; }
        public MaklumatKuarters HR_MAKLUMAT_KUARTERS { get; set; }
        public IEnumerable<MaklumatTanggungan> HR_MAKLUMAT_TANGGUNGAN { get; set; }
        public IEnumerable<MaklumatElaunPotongan> HR_MAKLUMAT_ELAUN_POTONGAN_G { get; set; }
        public IEnumerable<MaklumatElaunPotongan> HR_MAKLUMAT_ELAUN_POTONGAN_E { get; set; }
        public IEnumerable<MaklumatElaunPotongan> HR_MAKLUMAT_ELAUN_POTONGAN_P { get; set; }
        public IEnumerable<MaklumatElaunPotongan> HR_MAKLUMAT_ELAUN_POTONGAN_C { get; set; }
        public IEnumerable<MaklumatKurniaan> HR_MAKLUMAT_KURNIAAN { get; set; }
        public MaklumatAnugerahHaji HR_ANUGERAH_HAJI { get; set; }
        public MaklumatPersaraan HR_PERSARAAN { get; set; }
        public IEnumerable<MaklumatTindakanDisiplin> HR_TINDAKAN_DISIPLIN { get; set; }
        public MaklumatKematian HR_MAKLUMAT_KEMATIAN { get; set; }
        public MaklumatPenilaianPrestasi HR_PENILAIAN_PRESTASI { get; set; }
        public MaklumatCuti HR_MAKLUMAT_CUTI { get; set; }
        public IEnumerable<MaklumatAnugerahCemerlang> HR_ANUGERAH_CEMERLANG { get; set; }


        // Kewangan8
        public HR_MAKLUMAT_KEWANGAN8 HR_MAKLUMAT_KEWANGAN8 { get; set; }
        public HR_MAKLUMAT_KEWANGAN8_DETAIL HR_MAKLUMAT_KEWANGAN8_DETAIL { get; set; }
    }
    public class MaklumatPeribadi
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NO_KPBARU { get; set; }
        public string HR_NAMA_PEKERJA { get; set; }
        public string HR_NO_KPLAMA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_WARGANEGARA { get; set; }
        public string HR_KETURUNAN { get; set; }
        public string HR_AGAMA { get; set; }
        public string HR_JANTINA { get; set; }
        public string HR_TARAF_KAHWIN { get; set; }
        public string HR_LESEN { get; set; }
        public string HR_KELAS_LESEN { get; set; }
        public string HR_TALAMAT1 { get; set; }
        public string HR_TALAMAT2 { get; set; }
        public string HR_TALAMAT3 { get; set; }
        public string HR_TBANDAR { get; set; }
        public string HR_TPOSKOD { get; set; }
        public string HR_TNEGERI { get; set; }
        public string HR_SALAMAT1 { get; set; }
        public string HR_SALAMAT2 { get; set; }
        public string HR_SALAMAT3 { get; set; }
        public string HR_SBANDAR { get; set; }
        public string HR_SPOSKOD { get; set; }
        public string HR_SNEGERI { get; set; }
        public string HR_TAHUN_SPM { get; set; }
        public string HR_GRED_BM { get; set; }
        public string HR_TELRUMAH { get; set; }
        public string HR_TELPEJABAT { get; set; }
        public string HR_TELBIMBIT { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Email tidak sah")]
        public string HR_EMAIL { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<decimal> HR_CC_KENDERAAN { get; set; }
        public string HR_NO_KENDERAAN { get; set; }
        public string HR_JENIS_KENDERAAN { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_IDPEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
    }
    
    public class MaklumatPekerjaan
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_GELARAN { get; set; }
        public string HR_JABATAN { get; set; }
        public string HR_BAHAGIAN { get; set; }
        public string HR_JAWATAN { get; set; }
        public string HR_GRED { get; set; }
        public string HR_KATEGORI { get; set; }
        public string HR_KUMP_PERKHIDMATAN { get; set; }
        public string HR_TARAF_JAWATAN { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_NO_AKAUN_BANK { get; set; }
        public Nullable<System.DateTime> HR_BULAN_KENAIKAN_GAJI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SAH_JAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT_KONTRAK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_SISTEM { get; set; }
        public string HR_NO_PENYELIA { get; set; }
        public string HR_STATUS_KWSP { get; set; }
        public string HR_STATUS_SOCSO { get; set; }
        public string HR_STATUS_PCB { get; set; }
        public string HR_STATUS_PENCEN { get; set; }
        public Nullable<decimal> HR_NILAI_KWSP { get; set; }
        public Nullable<decimal> HR_NILAI_SOCSO { get; set; }
        public string HR_KOD_PCB { get; set; }
        public string HR_KATEGORI_PCB { get; set; }
        public string HR_GAJI_PRORATA { get; set; }
        public string HR_MATRIKS_GAJI { get; set; }
        public string HR_UNIT { get; set; }
        public string HR_KUMPULAN { get; set; }
        public string HR_KOD_BANK { get; set; }
        public string HR_TINGKATAN { get; set; }
        public string HR_KAKITANGAN_IND { get; set; }
        public string HR_FAIL_PERKHIDMATAN { get; set; }
        public string HR_NO_SIRI { get; set; }
        public string HR_BAYARAN_CEK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KE_JABATAN { get; set; }
        public string HR_KOD_GAJI { get; set; }
        public Nullable<decimal> HR_GAJI_MIN { get; set; }
        public Nullable<decimal> HR_GAJI_MAX { get; set; }
        public string HR_KELAS_PERJALANAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LANTIKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TIDAK_AKTIF { get; set; }
        public string HR_GAJI_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_GAJI { get; set; }
        public Nullable<System.DateTime> HR_PCB_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_PCB_TARIKH_AKHIR { get; set; }
        public Nullable<decimal> HR_NILAI_PCB { get; set; }
        public string HR_KOD_GELARAN_J { get; set; }
        public string HR_TANGGUH_GERAKGAJI_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN2 { get; set; }
        public string HR_NP_KEYIN2 { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH2 { get; set; }
        public string HR_NP_UBAH2 { get; set; }
        public string HR_SKIM { get; set; }
        public string HR_PERGERAKAN_GAJI { get; set; }
        public string HR_NO_KWSP { get; set; }
        public string HR_NO_PENCEN { get; set; }
        public string HR_NO_SOCSO { get; set; }
        public string HR_NO_PCB { get; set; }
        public string HR_INITIAL { get; set; }
        public string HR_AM_YDP { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK_KERAJAAN { get; set; }
        public string HR_UNIFORM { get; set; }
        public string HR_TEKNIKAL { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KELUAR_MBPJ { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SAH_J_TUKAR_LANTIK { get; set; }
    }
    public class MaklumatPengalamanKerja
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_SYARIKAT { get; set; }
        public string HR_JAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_ALASAN_BERHENTI { get; set; }
    }
    public class MaklumatPewaris
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_PEWARIS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_JANTINA { get; set; }
        public string HR_PALAMAT1 { get; set; }
        public string HR_PALAMAT2 { get; set; }
        public string HR_PALAMAT3 { get; set; }
        public string HR_PBANDAR { get; set; }
        public string HR_PPOSKOD { get; set; }
        public string HR_PNEGERI { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_TELRUMAH { get; set; }
        public string HR_TELPEJABAT { get; set; }
        public string HR_TELBIMBIT { get; set; }
        public string HR_NO_KP { get; set; }
        public string HR_NO_KP_LAMA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string HR_PEWARIS_IND { get; set; }
    }

    public class MaklumatKemahiranBahasa
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_BAHASA { get; set; }
        public string HR_PEMBACAAN { get; set; }
        public string HR_PENULISAN { get; set; }
        public string HR_PERTUTURAN { get; set; }
    }

    public class MaklumatKemahiranTeknikal
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<short> HR_SEQ_NO { get; set; }
        public string HR_KEMAHIRAN { get; set; }
        public string HR_TAHAP { get; set; }
    }

    public class MaklumatKelayakan
    {
        public string HR_NO_PEKERJA { get; set; }
        public short HR_SEQ_NO { get; set; }
        public string HR_KEPUTUSAN { get; set; }
        public string HR_PANGKAT { get; set; }
        public string HR_TAHUN_MULA { get; set; }
        public string HR_TAHUN_TAMAT { get; set; }
        public string HR_SEKOLAH_INSTITUSI { get; set; }
    }

    public class MaklumatSijil
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_DIPEROLEHI { get; set; }
        public string HR_NAMA_SIJIL_PEPERIKSAAN { get; set; }
        public string HR_ANJURAN { get; set; }
        public string HR_KEPUTUSAN { get; set; }
    }

    public class MaklumatKursusLatihan
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KOD_KURSUS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_ANJURAN { get; set; }
        public string HR_KEPUTUSAN { get; set; }
    }
    public class MaklumatAktiviti
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKTIVITI { get; set; }
        public string HR_PERINGKAT { get; set; }
        public string HR_NAMA_AKTIVITI { get; set; }
        public string HR_ANJURAN { get; set; }
    }
    public class MaklumatKuarters
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KOD_KUARTERS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KELUAR { get; set; }
        public string HR_NO_UNIT { get; set; }
        public string HR_GANDAAN2X { get; set; }
        public string HR_GERAI { get; set; }
        public string HR_CATATAN { get; set; }
        public string HR_IDP { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_GANDAAN5X { get; set; }
        public Nullable<decimal> HR_JUMLAH_POTONGAN { get; set; }
        public virtual HR_KUARTERS HR_KUARTERS { get; set; }
    }
    public class MaklumatTanggungan
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_TANGGUNGAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        public string HR_NO_KP { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_SEK_IPT { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_JANTINA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }

        public static List<MaklumatTanggungan> GetListTanggungan(string noPekerja)
        {
            List<MaklumatTanggungan> output = new List<MaklumatTanggungan>();
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_MAKLUMAT_TANGGUNGAN> tanggunganDb = db.HR_MAKLUMAT_TANGGUNGAN
                .Where(s => s.HR_NO_PEKERJA == noPekerja).ToList();
            foreach (var item in tanggunganDb)
            {
                MaklumatTanggungan mTanggungan = new MaklumatTanggungan();
                mTanggungan.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                mTanggungan.HR_NAMA_TANGGUNGAN = item.HR_NAMA_TANGGUNGAN;
                mTanggungan.HR_TARIKH_LAHIR = item.HR_TARIKH_LAHIR;
                mTanggungan.HR_NO_KP = item.HR_NO_KP;
                mTanggungan.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR;
                mTanggungan.HR_SEK_IPT = item.HR_SEK_IPT;
                mTanggungan.HR_HUBUNGAN = item.HR_HUBUNGAN;
                if (item.HR_HUBUNGAN != null)
                {
                    mTanggungan.HR_HUBUNGAN = new string(item.HR_HUBUNGAN.TakeWhile(x => char.IsDigit(x)).ToArray());
                }
                mTanggungan.HR_JANTINA = item.HR_JANTINA;
                mTanggungan.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                mTanggungan.HR_NP_KEYIN = item.HR_NP_KEYIN;
                mTanggungan.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                mTanggungan.HR_NP_UBAH = item.HR_NP_UBAH;
                output.Add(mTanggungan);
            }
            return output;
        }
    }
    public class MaklumatElaunPotongan
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KOD_ELAUN_POTONGAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_NO_FAIL { get; set; }
        public Nullable<decimal> HR_JUMLAH { get; set; }
        public string HR_ELAUN_POTONGAN_IND { get; set; }
        public string HR_MOD_BAYARAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
        public Nullable<decimal> HR_TUNTUTAN_MAKSIMA { get; set; }
        public Nullable<decimal> HR_BAKI { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<int> HR_HARI_BEKERJA { get; set; }
        public string HR_NO_PEKERJA_PT { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_UBAH_IND { get; set; }
        public string HR_GRED_PT { get; set; }
        public string HR_MATRIKS_GAJI_PT { get; set; }
        public string HR_GAJI_MIN { get; set; }
        public string HR_GAJI_MAX { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string HR_AUTO_IND { get; set; }
    }

    public class MaklumatKurniaan
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KURNIAAN { get; set; }
        public string HR_KOD_KURNIAAN { get; set; }
        public string HR_PERINGKAT { get; set; }
        public string HR_KURNIAAN_IND { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_STATUS { get; set; }

        //public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PENCALONAN { get; set; }
        //public string HR_KOD_KURNIAAN { get; set; }
        //public string HR_KURNIAAN_IND { get; set; }
        //public string HR_NEGERI { get; set; }
        public string HR_NP_PENCALON { get; set; }
        //public string HR_STATUS { get; set; }
    }

    public class MaklumatAnugerahHaji
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TAHUN_PERGI { get; set; }
        public string HR_STATUS_HAJI { get; set; }
        public string HR_NP_YDP { get; set; }
        public string HR_LULUS_IND { get; set; }
        public string HR_NP_UP { get; set; }
        public string HR_NP_PEG { get; set; }
    }

    public class MaklumatAnugerahCemerlang
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_ANUGERAH { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PENERIMAAN { get; set; }
    }

    public class MaklumatPersaraan
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_BERSARA { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_BERSARA_IND { get; set; }
        public string HR_BAYARAN_IND { get; set; }
        public Nullable<decimal> HR_JUMLAH_BAYARAN { get; set; }
        public Nullable<int> HR_JUMLAH_CUTI { get; set; }
        public string HR_PALAMAT1 { get; set; }
        public string HR_PALAMAT2 { get; set; }
        public string HR_PALAMAT3 { get; set; }
        public string HR_PBANDAR { get; set; }
        public string HR_PPOSKOD { get; set; }
        public string HR_PNEGERI { get; set; }
        public Nullable<decimal> HR_EKA { get; set; }
        public Nullable<decimal> HR_ITP { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_TERIMA_BAYARAN_IND { get; set; }
        public string HR_NP_PEGAWAI { get; set; }
        public string HR_JAWATAN_PEGAWAI { get; set; }

    }

    public class MaklumatTindakanDisiplin
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KESALAHAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KESALAHAN { get; set; }
        public string HR_KOD_TINDAKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
    }

    public class MaklumatKematian
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEMATIAN { get; set; }
        public string HR_NO_KP_PEWARIS { get; set; }
        public string HR_ALAMAT1 { get; set; }
        public string HR_ALAMAT2 { get; set; }
        public string HR_ALAMAT3 { get; set; }
        public string HR_BANDAR { get; set; }
        public string HR_NO_TELRUMAH { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_POSKOD { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_NAMA_PEWARIS { get; set; }
        public string HR_NO_TELPEJABAT { get; set; }
        public string HR_NO_TELBIMBIT { get; set; }
        public string HR_NO_VOUCHER { get; set; }
        public string HR_NAMA_PEGAWAI { get; set; }
        public string HR_JAWATAN_PEGAWAI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_BAYAR { get; set; }
        public string HR_MAKLUMAT_KHIDMAT { get; set; }
        public Nullable<decimal> HR_JUMLAH_WANG { get; set; }
        public string HR_VOT { get; set; }
    }

    public class MaklumatPenilaianPrestasi
    {
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<short> HR_TAHUN_PRESTASI { get; set; }
        public Nullable<decimal> HR_PENGHASILAN_PPP { get; set; }
        public Nullable<decimal> HR_PENGHASILAN_PPK { get; set; }
        public Nullable<decimal> HR_PENGETAHUAN_PPP { get; set; }
        public Nullable<decimal> HR_PENGETAHUAN_PPK { get; set; }
        public Nullable<decimal> HR_KUALITI_PPP { get; set; }
        public Nullable<decimal> HR_KUALITI_PPK { get; set; }
        public Nullable<decimal> HR_SUMBANGAN_PPP { get; set; }
        public Nullable<decimal> HR_SUMBANGAN_PPK { get; set; }
        public Nullable<decimal> HR_PURATA_PENGHASILAN { get; set; }
        public Nullable<decimal> HR_PURATA_PENGETAHUAN { get; set; }
        public Nullable<decimal> HR_PURATA_KUALITI { get; set; }
        public Nullable<decimal> HR_PURATA_SUMBANGAN { get; set; }
        public Nullable<decimal> HR_PERATUS_PENGHASILAN { get; set; }
        public Nullable<decimal> HR_PERATUS_PENGETAHUAN { get; set; }
        public Nullable<decimal> HR_PERATUS_KUALITI { get; set; }
        public Nullable<decimal> HR_PERATUS_SUMBANGAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_BESAR { get; set; }
        public string HR_CEMERLANG_IND { get; set; }
        public string HR_JENIS_IND { get; set; }
        public Nullable<short> HR_CUTI_KERAJAAN { get; set; }
        public Nullable<short> HR_CUTI_SWASTA { get; set; }
    }
    public class MaklumatCuti
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KOD_CUTI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PERMOHONAN { get; set; }
        public Nullable<short> HR_BAKI_CUTI_REHAT { get; set; }
        public Nullable<short> HR_JUMLAH_MAKSIMUM { get; set; }
        public Nullable<short> HR_BAKI_TAHUN_LEPAS { get; set; }

        public Nullable<int> HR_BAKI_PENCEN { get; set; }
        public Nullable<short> HR_TAHUN { get; set; }
        public Nullable<short> HR_BIL_CUTI_TEMP { get; set; }
        public Nullable<int> HR_BAKI_PENCEN_TERKUMPUL { get; set; }
        public Nullable<int> HR_KELAYAKAN_BULANAN { get; set; }
        public Nullable<int> HR_BIL_CUTI_DIAMBIL { get; set; }


        //Detail Cuti

        public Nullable<System.DateTime> HR_TARIKH_MULA_CUTI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT_CUTI { get; set; }
        public string HR_CALAMAT1 { get; set; }
        public string HR_CALAMAT2 { get; set; }
        public string HR_CALAMAT3 { get; set; }
        public string HR_CBANDAR { get; set; }
        public string HR_CPOSKOD { get; set; }
        public string HR_CNEGERI { get; set; }
        public string HR_TEL { get; set; }
        public string HR_NP_PENGGANTI { get; set; }
        public string HR_NAMA_PROGRAM { get; set; }
        public string HR_TEMPAT_PROGRAM { get; set; }
        public string HR_ANJURAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_ISTERI_BERSALIN { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_NO_SIRI { get; set; }
        public Nullable<short> HR_BIL_HARI_CUTI { get; set; }
        public string HR_SOKONG_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SOKONG { get; set; }
        public string HR_LULUS_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LULUS { get; set; }
        public string HR_NP_KJ { get; set; }
        public string HR_HR_LULUS_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LULUS_HR { get; set; }
        public string HR_NO_PEKERJA_LULUS { get; set; }
        public string HR_ULASAN { get; set; }
        public string HR_LULUS_YDP_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_YDP { get; set; }
        public string HR_NO_PEKERJA_YDP { get; set; }
        public string HR_HARI_CUTI { get; set; }
        public string HR_NAMA_KLINIK { get; set; }

        public IEnumerable<System.DateTime> HR_SENARAI_TARIKH { get; set; }
        public IEnumerable<System.DateTime> HR_TARIKH_BATAL { get; set; }
        public string HR_KATEGORI_CUTI { get; set; }
        public Nullable<int> HR_BAKI_TERKINI { get; set; }
        public Nullable<int> HR_JUM_FAEDAH { get; set; }
        public Nullable<int> DISABLED { get; set; }
    }
}