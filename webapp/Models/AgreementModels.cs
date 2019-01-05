using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class AgreementModels
    {
        [Column("USER_ID")]
        public string USER_ID { get; set; }
        [Column("USER_LEVEL")]
        public string USER_LEVEL { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Column("USER_PASSWORD")]
        public string USER_PASSWORD { get; set; }
        [Column("USER_GROUP")]
        public string USER_GROUP { get; set; }
        [Column("PEGAWAI_TAMBAH")]
        public string PEGAWAI_TAMBAH { get; set; }
        [Column("TKH_TAMBAH")]
        public string TKH_TAMBAH { get; set; }
        [Column("PEGAWAI_UBAH")]
        public string PEGAWAI_UBAH { get; set; }
        [Column("TKH_UBAH")]
        public string TKH_UBAH { get; set; }
        [Column("NAMA_PENUH_PENGGUNA")]
        public string NAMA_PENUH_PENGGUNA { get; set; }
        [Column("PENGGUNAAN")]
        public string PENGGUNAAN { get; set; }
        [Column("DEFAULT_WINDOW")]
        public string DEFAULT_WINDOW { get; set; }
        [Column("BANTUAN_BUTANG")]
        public string BANTUAN_BUTANG { get; set; }
        [Column("SARING_SENARAI_TUGAS")]
        public string SARING_SENARAI_TUGAS { get; set; }
        [Column("NOTA")]
        public string NOTA { get; set; }
        [Column("LOGIN_AKHIR")]
        public string LOGIN_AKHIR { get; set; }
        [Column("ID_KAKITANGAN")]
        public string ID_KAKITANGAN { get; set; }
        [Column("ROWGUID")]
        public string ROWGUID { get; set; }
        public string HR_UNIT { get; set; }
        //////////////////////////////////////////////////////
        [Column("HR_NO_PEKERJA")]
        public string HR_NO_PEKERJA { get; set; }
        [Column("HR_NO_KPBARU")]
        public string HR_NO_KPBARU { get; set; }
        [Column("HR_NAMA_PEKERJA")]
        public string HR_NAMA_PEKERJA { get; set; }
        [Column("HR_NO_KPLAMA")]
        public string HR_NO_KPLAMA { get; set; }
        [Column("HR_TARIKH_LAHIR")]
        public string HR_TARIKH_LAHIR { get; set; }
        [Column("HR_TEMPAT_LAHIR")]
        public string HR_TEMPAT_LAHIR { get; set; }
        [Column("HR_WARGANEGARA")]
        public string HR_WARGANEGARA { get; set; }
        [Column("HR_KETURUNAN")]
        public string HR_KETURUNAN { get; set; }
        [Column("HR_AGAMA")]
        public string HR_AGAMA { get; set; }
        [Column("HR_JANTINA")]
        public string HR_JANTINA { get; set; }
        [Column("HR_TARAF_KAHWIN")]
        public string HR_TARAF_KAHWIN { get; set; }
        [Column("HR_LESEN")]
        public string HR_LESEN { get; set; }
        [Column("HR_KELAS_LESEN")]
        public string HR_KELAS_LESEN { get; set; }
        [Column("HR_TALAMAT1")]
        public string HR_TALAMAT1 { get; set; }
        [Column("HR_TALAMAT2")]
        public string HR_TALAMAT2 { get; set; }
        [Column("HR_TALAMAT3")]
        public string HR_TALAMAT3 { get; set; }
        [Column("HR_TPOSKOD")]
        public string HR_TPOSKOD { get; set; }
        [Column("HR_TNEGERI")]
        public string HR_TNEGERI { get; set; }
        [Column("HR_SALAMAT1")]
        public string HR_SALAMAT1 { get; set; }
        [Column("HR_SALAMAT2")]
        public string HR_SALAMAT2 { get; set; }
        [Column("HR_SALAMAT3")]
        public string HR_SALAMAT3 { get; set; }
        [Column("HR_SBANDAR")]
        public string HR_SBANDAR { get; set; }
        [Column("HR_SPOSKOD")]
        public string HR_SPOSKOD { get; set; }
        [Column("HR_SNEGERI")]
        public string HR_SNEGERI { get; set; }
        [Column("HR_TAHUN_SPM")]
        public string HR_TAHUN_SPM { get; set; }
        [Column("HR_GRED_BM")]
        public string HR_GRED_BM { get; set; }
        [Column("HR_TELRUMAH")]
        public string HR_TELRUMAH { get; set; }
        [Column("HR_TELPEJABAT")]
        public string HR_TELPEJABAT { get; set; }
        [Column("HR_TELBIMBIT")]
        public string HR_TELBIMBIT { get; set; }
        [Required]
        [EmailAddress]
        [Column("HR_EMAIL")]
        public string HR_EMAIL { get; set; }
        [Column("HR_AKTIF_IND")]
        public string HR_AKTIF_IND { get; set; }
        [Column("HR_CC_KENDERAAN")]
        public string HR_CC_KENDERAAN { get; set; }
        [Column("HR_NO_KENDERAAN")]
        public string HR_NO_KENDERAAN { get; set; }
        [Column("HR_JENIS_KENDERAAN")]
        public string HR_JENIS_KENDERAAN { get; set; }
        [Column("HR_ALASAN")]
        public string HR_ALASAN { get; set; }
        [Column("HR_IDPEKERJA")]
        public string HR_IDPEKERJA { get; set; }
        [Column("HR_TARIKH_KEYIN")]
        public string HR_TARIKH_KEYIN { get; set; }
        [Column("HR_NP_KEYIN")]
        public string HR_NP_KEYIN { get; set; }
        [Column("HR_TARIKH_UBAH")]
        public string HR_TARIKH_UBAH { get; set; }

        //GE_JABATAN
        public string HR_JABATAN { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string Name { get; set; }
        public string GE_KOD_JABATAN { get; set; }
        public string GE_KETERANGAN_JABATAN { get; set; }
        public string GE_ALAMAT1 { get; set; }
        public string GE_ALAMAT2 { get; set; }
        public string GE_ALAMAT3 { get; set; }
        public string GE_BANDAR { get; set; }
        public string GE_POSKOD { get; set; }
        public string GE_NEGERI { get; set; }
        public string GE_TELPEJABAT1 { get; set; }
        public string GE_TELPEJABAT2 { get; set; }
        public string GE_FAKS1 { get; set; }
        public string GE_FAKS2 { get; set; }
        public string GE_EMAIL { get; set; }
        public string GE_NO_KETUA { get; set; }
        public string GE_SINGKATAN { get; set; }
        public string GE_AKTIF_IND { get; set; }
        public string HR_PEKERJA { get; set; }
        public string HR_BULAN_DIBAYAR { get; set; }
		public string HR_TAHUN { get; set; }
		public string HR_BULAN_BEKERJA { get; set; }
		public string HR_TAHUN_DIBAYAR { get; set; }
		public string HR_TAHUN_BEKERJA { get; set; }
		public int LIST_HR_TAHUN { get; set; }
		public int LIST_HR_BULAN_BEKERJA { get; set; }
		public int LIST_HR_TAHUN_DIBAYAR { get; set; }
		public int LIST_HR_BULAN_DIBAYAR { get; set; }

		//GE_BAHAGIAN
		public string GE_KOD_BAHAGIAN { get; set; }
        public string HR_BAHAGIAN { get; set; }
        public string GE_KOD_JABATAN1 { get; set; }
        public string GE_KETERANGAN { get; set; }
        public string GE_ALAMAT11 { get; set; }
        public string GE_ALAMAT21 { get; set; }
        public string GE_ALAMAT31 { get; set; }
        public string GE_BANDAR1 { get; set; }
        public string GE_POSKOD1 { get; set; }
        public string GE_NEGERI1 { get; set; }
        public string GE_TELPEJABAT11 { get; set; }
        public string GE_TELPEJABAT21 { get; set; }
        public string GE_FAKS11 { get; set; }
        public string GE_FAKS21 { get; set; }
        public string GE_EMAIL1 { get; set; }
        public string GE_NO_KETUA1 { get; set; }
        public string GE_SINGKATAN1 { get; set; }
        public string GE_AKTIF_IND1 { get; set; }
        public decimal? GAJIPOKOK { get; set; }
        public string ELAUNKA { get; set; }
		public string POTONGANKSDK { get; set; }
		public string GAJIBASIC { get; set; }
		public string ELAUNLAIN { get; set; }
        public string POTONGLAIN { get; set; }
        public string GAJIPER3 { get; set; }
        public string GAJIKASAR { get; set; }
        public string GAJISEBELUMKWSP { get; set; }
		public decimal? KIRAKWSP { get; set; }
		public string JABATAN { get; set; }
		public string BAHAGIAN { get; set; }
		public string JUMLAHBAYARANOT { get; set; }
		public string KIRATUNGGAKAN { get; set; }
		public decimal? JAMBEKERJA { get; set; }
		public decimal? HARIBEKERJA { get; set; }
		public string HR_KOD { get; set; }
		public int HR_BULAN_DIBAYAR_TUNGGAKAN { get; set; }
		public int HR_TAHUN_TUNGGAKAN { get; set; }
		public int HR_BULAN_BEKERJA_TUNGGAKAN { get; set; }
		public decimal? HR_JUMLAH_TUNGGAKAN { get; set; }
		public int HR_TAHUN_BEKERJA_TUNGGAKAN { get; set; }
		public string HR_NAMA_PEKERJA_TUNGGAKAN { get; set; }
		public string KETERANGAN { get; set; }
		public string KOD { get; set; }
		public string NOKP { get; set; }
		public string JAWATAN { get; set; }
		public string GAJISEHARI { get; set; }
		public byte BULANNOTIS { get; set; }
		public string BONUS { get; set; }
		public string BULANBONUS { get; set; }
		public decimal? JUMLAHBONUS { get; set; }
		public string JUMLAHBONUS1 { get; set; }
		public decimal? CARUMANKWSP { get; set; }
		public decimal? POTONGANKWSP { get; set; }
		public decimal? JUMLAHKWSP { get; set; }
		public int BULANDIBAYAR { get; set; }
        //khairil
        public int BULANDIBAYARPREV
        {
            get
            {
                try
                {
                    int retInt = BULANDIBAYAR == 1 ? 12 : BULANDIBAYAR - 1;
                    return retInt;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int TAHUNDIBAYAR { get; set; }
		public int BULANBEKERJA { get; set; }
        //khairil
        public int BULANBEKERJAPREV { get
            {
                try
                {
                    int retInt = BULANBEKERJA == 1 ? 12 : BULANBEKERJA - 1;
                    return retInt;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int TAHUNBEKERJA { get; set; }
		public string PILIHANKWSP { get; set; }
		public string PILIHANSOCSO { get; set; }
		public string PILIHANPCB { get; set; }
		public decimal? JUMLAHHARI { get; set; }
		public decimal? TUNGGAKANJAMOT { get; set; }
		public string TUNGGAKANOT { get; set; }
		public string TUNGGAKANKASAR { get; set; }
		public string TUNGGAKANELAUN { get; set; }
		public string TUNGGAKANIND { get; set; }
		public string TAKTOLAKKWSP { get; set; }
		public decimal? GAJIBERSIH { get; set; }
		public string JUMLAH { get; set; }
		public decimal? HR_JUMLAH_MAKLUMAT { get; set; }
		public DateTime? HR_TARIKH_MULA { get; set; }
		public DateTime? HR_TARIKH_AKHIR { get; set; }
		public decimal? CARUMANSOCSO { get; set; }
		public decimal? POTONGANSOCSO { get; set; }
		public int? MUKTAMAD { get; set; }

		public IEnumerable<Agreement1Models> agree1 { get; set; }
    }

    public class Agreement1Models
    {
        //GE_JABATAN
        public string HR_PEKERJA { get; set; }
        public string HR_JABATAN { get; set; }
        public string HR_UNIT { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string Name { get; set; }
        public string GE_KOD_JABATAN { get; set; }
        public string GE_KETERANGAN_JABATAN { get; set; }
        public string GE_ALAMAT1 { get; set; }
        public string GE_ALAMAT2 { get; set; }
        public string GE_ALAMAT3 { get; set; }
        public string GE_BANDAR { get; set; }
        public string GE_POSKOD { get; set; }
        public string GE_NEGERI { get; set; }
        public string GE_TELPEJABAT1 { get; set; }
        public string GE_TELPEJABAT2 { get; set; }
        public string GE_FAKS1 { get; set; }
        public string GE_FAKS2 { get; set; }
        public string GE_EMAIL { get; set; }
        public string GE_NO_KETUA { get; set; }
        public string GE_SINGKATAN { get; set; }
        public string GE_AKTIF_IND { get; set; }

        //GE_BAHAGIAN
        public string GE_KOD_BAHAGIAN { get; set; }
        public string HR_BAHAGIAN { get; set; }
        public string GE_KOD_JABATAN1 { get; set; }
        public string GE_KETERANGAN { get; set; }
        public string GE_ALAMAT11 { get; set; }
        public string GE_ALAMAT21 { get; set; }
        public string GE_ALAMAT31 { get; set; }
        public string GE_BANDAR1 { get; set; }
        public string GE_POSKOD1 { get; set; }
        public string GE_NEGERI1 { get; set; }
        public string GE_TELPEJABAT11 { get; set; }
        public string GE_TELPEJABAT21 { get; set; }
        public string GE_FAKS11 { get; set; }
        public string GE_FAKS21 { get; set; }
        public string GE_EMAIL1 { get; set; }
        public string GE_NO_KETUA1 { get; set; }
        public string GE_SINGKATAN1 { get; set; }
        public string GE_AKTIF_IND1 { get; set; }

        public bool Selected { get; set; }
        public string FeatureName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        [Column("GROUPFEATUREID")]
        public Guid GroupFeatureID { get; set; }
        [Column("USERGROUPID")]
        public Guid UserGroupID { get; set; }
        [Column("SYSTEMFEATUREID")]
        public Guid SystemFeatureID { get; set; }
        [Column("CREATEDATETIME")]
        public DateTime? CreateDateTime { get; set; }
        [Column("ROLE_ID")]
        public string Role_ID { get; set; }
        [Column("PAGE_ID")]
        public Guid Page_ID { get; set; }
        [Column("VIEW_ID")]
        public int? View_ID { get; set; }
        [Column("INSERT_ID")]
        public int? Insert_ID { get; set; }
        [Column("UPDATE_ID")]
        public int? Update_ID { get; set; }
        [Column("DELETE_ID")]
        public int? Delete_ID { get; set; }
        [Column("ROWGUID")]
        public Guid rowguid { get; set; }
        [Column("FIELD_NAME")]
        public string Field_Name { get; set; }
        public Guid GroupUserID { get; set; }
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Column("MODULEID")]
        public Guid MODULEID { get; set; }
        [Column("PAGENAME")]
        public string PAGENAME { get; set; }
        public DateTime? CreateDateTime1 { get; set; }
        public string Id { get; set; }
        public Guid SystemFeatureID1 { get; set; }
        public Guid ModuleId { get; set; }

    }
}