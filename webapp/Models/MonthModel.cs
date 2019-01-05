using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MonthModel
    {
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mac { get; set; }
        public string Apr { get; set; }
        public string Mei { get; set; }
        public string Jun { get; set; }
        public string Julai { get; set; }
        public string Ogos { get; set; }
        public string Sept { get; set; }
        public string Okt { get; set; }
        public string Nov { get; set; }
        public string Dis { get; set; }
    }

    public class BonusSambilanMonthModel
    {
        public BonusSambilanMonthModel()
        {
            IsMuktamad = false;
        }
        public int Nombor { get; set; }
        public int MonthNumber { get; set; }
        public bool IsMuktamad { get; set; }

        public string MonthName
        {
            get
            {
                string _monthName;
                switch (MonthNumber)
                {
                    case (1):
                        _monthName = "Jan";
                        break;
                    case (2):
                        _monthName = "Feb";
                        break;
                    case (3):
                        _monthName = "Mac";
                        break;
                    case (4):
                        _monthName = "Apr";
                        break;
                    case (5):
                        _monthName = "Mei";
                        break;
                    case (6):
                        _monthName = "Jun";
                        break;
                    case (7):
                        _monthName = "Jul";
                        break;
                    case (8):
                        _monthName = "Ogos";
                        break;
                    case (9):
                        _monthName = "Sept";
                        break;
                    case (10):
                        _monthName = "Okt";
                        break;
                    case (11):
                        _monthName = "Nov";
                        break;
                    case (12):
                        _monthName = "Dis";
                        break;
                    default:
                        _monthName = string.Empty;
                        break;
                }
                return _monthName;
            }
        }
        public int MonthValue { get; set; }

        public static List<BonusSambilanMonthModel> GetBonusByYear(int tahun)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<BonusSambilanMonthModel> list = new List<BonusSambilanMonthModel>();
            //MonthModel m = new MonthModel();

            int counter = 1;
            for (int i = 1; i <= 12; i++)
            {
                //List<HR_TRANSAKSI_SAMBILAN_DETAIL> detail =
                //    db.HR_TRANSAKSI_SAMBILAN_DETAIL.Where
                //    (s => s.HR_KOD == "GAJPS" && s.HR_TAHUN == tahun && s.HR_BULAN_DIBAYAR == i).ToList();
                //detail.Sort((x, y) => x.HR_BULAN_BEKERJA.CompareTo(y.HR_BULAN_BEKERJA));

                List<HR_BONUS_SAMBILAN_DETAIL> detail =
                    db.HR_BONUS_SAMBILAN_DETAIL.Where
                    (s => s.HR_TAHUN_BONUS == tahun
                    && s.HR_BULAN_BONUS == i
                    && s.HR_STATUS == 1).ToList();


                BonusSambilanMonthModel m = new BonusSambilanMonthModel();
                m.MonthNumber = i;
                m.MonthValue = detail.Count();
                int muktamadInt = detail.Select(s => s.HR_MUKTAMAD).FirstOrDefault();
                m.IsMuktamad = muktamadInt == 0 ? false : true;

                if (m.MonthValue > 0)
                {
                    m.Nombor = counter;
                    counter++;
                    list.Add(m);
                }
            }

            return list;
        }
    }

}