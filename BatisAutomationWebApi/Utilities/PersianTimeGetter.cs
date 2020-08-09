using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatissWebOA.Presentation.Utility
{
    public class PersianTimeGetter
    {
        private PersianCalendar _persianCalendar;

        public PersianTimeGetter()
        {
            _persianCalendar = new PersianCalendar();
        }
        public string GetDateNow()
        {
            var now = DateTime.Now;
            return string.Format("{0:D4}/{1:D2}/{2:D2}", _persianCalendar.GetYear(now),
                _persianCalendar.GetMonth(now), _persianCalendar.GetDayOfMonth(now));
        }
        public string GetTimeNow()
        {
            var now = DateTime.Now;
            return string.Format("{0:D2}:{1:D2}", now.Hour, now.Minute);
        }
        public string GetYearNow()
        {
            var now = DateTime.Now;
            return _persianCalendar.GetYear(now).ToString();
        }
        public string GetMonthNow()
        {
            var now = DateTime.Now;
            var monthNow = _persianCalendar.GetMonth(now);
            switch (monthNow)
            {
                case 1: return "فروردین";
                case 2: return "اردیبهشت";
                case 3: return "خرداد";
                case 4: return "تیر";
                case 5: return "مرداد";
                case 6: return "شهریور";
                case 7: return "مهر";
                case 8: return "آبان";
                case 9: return "آذر";
                case 10: return "دی";
                case 11: return "بهمن";
                case 12: return "اسفند";
                default: return "";
            }
        }


        public string GetDayNow()
        {
            var now = DateTime.Now;
            return _persianCalendar.GetDayOfMonth(now).ToString();
        }

    }
}
