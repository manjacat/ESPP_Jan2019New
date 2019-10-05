using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;


namespace eSPP.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Nullable<DateTime> PasswordUpdate { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("OracleDbContext", throwIfV1Schema: false)

        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SPP");
            //modelBuilder.HasDefaultSchema("SYSTEM");

            base.OnModelCreating(modelBuilder);

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<eSPP.Models.HR_BONUS_SAMBILAN_DETAIL> HR_BONUS_SAMBILAN_DETAIL { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.ASPNETROLESHTML> ASPNETROLESHTML { get; set; }


        public System.Data.Entity.DbSet<eSPP.Models.UserInfo> UserInfoes { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.SystemFeature> SystemFeatures { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GroupFeature> GroupFeatures { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.UserGroup> UserGroup { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.AuditTrail> AuditTrails { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_PENGGUNA> GE_PENGGUNA { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_PERIBADI> HR_MAKLUMAT_PERIBADI { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_PEKERJAAN> HR_MAKLUMAT_PEKERJAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_ELAUN_POTONGAN> HR_MAKLUMAT_ELAUN_POTONGAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERSARAAN> HR_PERSARAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_ALASAN> HR_ALASAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_JAWATAN> HR_JAWATAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_GELARAN_JAWATAN> HR_GELARAN_JAWATAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_PENGALAMAN_KERJA> HR_MAKLUMAT_PENGALAMAN_KERJA { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PCB> HR_PCB { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_PEWARIS> HR_MAKLUMAT_PEWARIS { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KEMAHIRAN_BAHASA> HR_MAKLUMAT_KEMAHIRAN_BAHASA { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL> HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MATAPELAJARAN> HR_MATAPELAJARAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_AKTIVITI> HR_MAKLUMAT_AKTIVITI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KELAYAKAN> HR_MAKLUMAT_KELAYAKAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KURSUS_LATIHAN> HR_MAKLUMAT_KURSUS_LATIHAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_SIJIL> HR_MAKLUMAT_SIJIL { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KURSUS> HR_KURSUS { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KUARTERS> HR_MAKLUMAT_KUARTERS { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KUARTERS> HR_KUARTERS { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_AGENSI> HR_AGENSI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_TANGGUNGAN> HR_MAKLUMAT_TANGGUNGAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_CARUMAN> HR_CARUMAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_ELAUN> HR_ELAUN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_GRED_ELAUN> HR_GRED_ELAUN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KATEGORI_ELAUN> HR_KATEGORI_ELAUN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_POTONGAN> HR_POTONGAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_JADUAL_GAJI> HR_JADUAL_GAJI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_GAJI_UPAHAN> HR_GAJI_UPAHAN { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_MATRIKS_GAJI> HR_MATRIKS_GAJI { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KURNIAAN> HR_MAKLUMAT_KURNIAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PENCALONAN_KURNIAAN> HR_PENCALONAN_KURNIAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KURNIAAN> HR_KURNIAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_ANUGERAH_HAJI> HR_ANUGERAH_HAJI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_TINDAKAN> HR_TINDAKAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_TINDAKAN_DISIPLIN> HR_TINDAKAN_DISIPLIN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_TINDAKAN_DISIPLIN_DETAIL> HR_TINDAKAN_DISIPLIN_DETAIL { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KEMATIAN> HR_MAKLUMAT_KEMATIAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PENILAIAN_PRESTASI> HR_PENILAIAN_PRESTASI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_CUTI> HR_CUTI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_CUTI_UMUM> HR_CUTI_UMUM { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_CUTI> HR_MAKLUMAT_CUTI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_CUTI_DETAIL> HR_MAKLUMAT_CUTI_DETAIL { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_SENARAI_TARIKH_CUTI> HR_SENARAI_TARIKH_CUTI { get; set; }

        

        public System.Data.Entity.DbSet<eSPP.Models.HR_JUSTIFIKASI_JAWATAN_BARU> HR_JUSTIFIKASI_JAWATAN_BARU { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KEWANGAN8> HR_MAKLUMAT_KEWANGAN8 { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_KEWANGAN8_DETAIL> HR_MAKLUMAT_KEWANGAN8_DETAIL { get; set; }

		public System.Data.Entity.DbSet<eSPP.Models.HR_KLAS_PERKHIDMATAN> HR_KLAS_PERKHIDMATAN { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_JENIS_PEPERIKSAAN> HR_JENIS_PEPERIKSAAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.AP_CREDITORMASTER> AP_CREDITORMASTER { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_SEWAAN_ALATAN> HR_SEWAAN_ALATAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_SOCSO> HR_SOCSO { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KADAR_ELAUN_LEBIHMASA> HR_KADAR_ELAUN_LEBIHMASA { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KEWANGAN8> HR_KEWANGAN8 { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_SOALAN_MB> HR_SOALAN_MB { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KADAR_PERBATUAN> HR_KADAR_PERBATUAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KATEGORI_KURSUS> HR_KATEGORI_KURSUS { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KUMPULAN_PENSYARAH> HR_KUMPULAN_PENSYARAH { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_SUBJEK> HR_SUBJEK { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PENSYARAH> HR_PENSYARAH { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_KWSP> HR_KWSP { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_ANUGERAH_CEMERLANG> HR_ANUGERAH_CEMERLANG { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_GAMBAR_PENGGUNA> HR_GAMBAR_PENGGUNA { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_MAKLUMAT_PEKERJAAN_HISTORY> HR_MAKLUMAT_PEKERJAAN_HISTORY { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_TRANSAKSI_SAMBILAN> HR_TRANSAKSI_SAMBILAN { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_TRANSAKSI_SAMBILAN_DETAIL> HR_TRANSAKSI_SAMBILAN_DETAIL { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_BONUS_SAMBILAN> HR_BONUS_SAMBILAN { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_TUNTUTAN_INSURAN> HR_TUNTUTAN_INSURAN { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_SEMINAR_LUAR> HR_SEMINAR_LUAR { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.HR_SEMINAR_LUAR_DETAIL> HR_SEMINAR_LUAR_DETAIL { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.Clobb> CLOBB { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERATUS_KWSP> HR_PERATUS_KWSP { get; set; }
    
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERBATUAN_PINK> HR_PERBATUAN_PINK { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERBATUAN_TUNTUTAN> HR_PERBATUAN_TUNTUTAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERBATUAN_TUJUAN> HR_PERBATUAN_TUJUAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_GRED_ELAUN_PEKELILING> HR_GRED_ELAUN_PEKELILING { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERATUS_MAKAN> HR_PERATUS_MAKAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PERBATUAN_PUTIH> HR_PERBATUAN_PUTIH { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_PANEL_HOSPITAL> HR_PANEL_HOSPITAL { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_PENDAHULUAN_DIRI> HR_PENDAHULUAN_DIRI { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_ACARA> HR_ACARA { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_TARIKH_ACARA> HR_TARIKH_ACARA { get; set; }

        public System.Data.Entity.DbSet<eSPP.Models.HR_SEJARAH_ELAUN_POTONGAN> HR_SEJARAH_ELAUN_POTONGAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.HR_SEJARAH_PEKERJAAN> HR_SEJARAH_PEKERJAAN { get; set; }
    }
    public class MajlisContext : DbContext
    {
        public MajlisContext()
            : base("MajlisEntities")

        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MAJLIS");
            //modelBuilder.HasDefaultSchema("SYSTEM");

            base.OnModelCreating(modelBuilder);

        }

		public System.Data.Entity.DbSet<eSPP.Models.PRUSER> PRUSER { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.GE_PARAMTABLE_GROUP> GE_PARAMTABLE_GROUP { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_PARAMTABLE> GE_PARAMTABLE { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_BAHAGIAN> GE_BAHAGIAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_JABATAN> GE_JABATAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_UNIT> GE_UNIT { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.GE_SYSTEM> GE_SYSTEM { get; set; }

    }

	public class SPGContext : DbContext
	{
		public SPGContext()
			: base("SPGEntities")

		{
		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("SPG");
			//modelBuilder.HasDefaultSchema("SYSTEM");

			base.OnModelCreating(modelBuilder);

		}
        public System.Data.Entity.DbSet<eSPP.Models.PA_PELARASAN> PA_PELARASAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.PA_TRANSAKSI_CARUMAN> PA_TRANSAKSI_CARUMAN { get; set; }
		public System.Data.Entity.DbSet<eSPP.Models.PA_TRANSAKSI_PEMOTONGAN> PA_TRANSAKSI_PEMOTONGAN { get; set; }
        public System.Data.Entity.DbSet<eSPP.Models.PA_TRANSAKSI_GAJI> PA_TRANSAKSI_GAJI { get; set; }

        //added utk pump data ke SPG
        public System.Data.Entity.DbSet<eSPP.Models.PA_REPORT> PA_REPORT { get; set; }
    }
}