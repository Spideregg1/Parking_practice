using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MainParking
{
    #region interface
    public interface IFeeRule
    {
        int CalcFee(DateTime start, DateTime end);
    }

    public interface IMinuteRule
    {
        int totalMins(DateTime start, DateTime end);
    }

    #endregion

    public class Solution
    {
        //停車時間總長度在[0,10],免費
        //停車時間總長度在[11,30],7元
        //停車時間總長度在[31,59],10元
        //停車時間總長度在[60,1439]:
        //先算有幾小時,每小時收10元
        //若有剩餘分鐘數,且小於等於30分,加收7元
        //若有剩餘分鐘數,且大於30分,加收10元
        //一天最多只收50元

        // TODO const VS readonly
        private readonly IFeeRule _feeRule;

        public Solution(IFeeRule feerule)
        {
            _feeRule = feerule;
        }



        public class SingleDayFee
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public int Fee { get; set; }
        }

        public class ParkingFee
        {
            public IEnumerable<SingleDayFee> Items { get; private set; }
            public int TotalFee { get; }

            public ParkingFee(IEnumerable<SingleDayFee> items)
            {
                Items = items;
                TotalFee = items.Sum(s => s.Fee);
            }
        }

        public ParkingFee CalcParkingFee(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new Exception("輸入錯誤");
            }
            var 費用 = CalcFeeForMultiDays(start, end);
            return new ParkingFee(費用);
        }


        public IEnumerable<SingleDayFee> CalcFeeForMultiDays(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new Exception("輸入錯誤");
            }

            var timelist = Timelist(start, end);
            return SingleDayFeeToList(timelist);
        }


        private IEnumerable<SingleDayFee> SingleDayFeeToList(List<(DateTime start, DateTime end)> timelist)
        {
            return timelist.Select(datetimerange => new SingleDayFee()
            {
                StartTime = datetimerange.start,
                EndTime = datetimerange.end,
                Fee = (int)_feeRule.CalcFee(datetimerange.start, datetimerange.end)
            });
        }



        private List<(DateTime start, DateTime end)> Timelist(DateTime start, DateTime end)
        {
            DateTime Only_start_Date = OnlyDate(start);
            DateTime Only_end_Date = OnlyDate(end);

            var result = new List<(DateTime start, DateTime end)>();



            if (is_Same_day())
            {
                insertSingleDay();
                return result;
            }

            insertStart();
            insertRange();
            insertEnd();
            return result;

            bool is_Same_day() //判斷是否同一日
            {
                return Only_start_Date == Only_end_Date;
            }



            void insertSingleDay() //插入單一天
            {
                result.Add((start, end));
            }

            void insertStart()
            {
                result.Add((start, 一天結束的時間(start)));
            }

            void insertEnd()
            {
                result.Add((Only_end_Date, end));
            }

            void insertRange()
            {

                var resultRange = Enumerable.Range(1, Only_end_Date.Subtract(Only_start_Date).Days - 1)
                     .Select(x => Only_start_Date.AddDays(x))
                     .Select(dt => (dt, 一天結束的時間(dt)));

                result.AddRange(resultRange);
            }
        }

        private DateTime 一天結束的時間(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
        }

        private DateTime OnlyDate(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }








        private DateTime ParkingTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

        public bool 是星期六或星期日(DateTime dt)
        {
            return (dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday);
        }


    }

    #region 收費規則
    public class ParkingAFeeRule : IFeeRule
    {
        private readonly IMinuteRule _minuteRule;
        public ParkingAFeeRule()
        {
            _minuteRule = new MinuteARule();
        }
        public int CalcFee(DateTime start, DateTime end)
        {
            int total_min = _minuteRule.totalMins(start, end);

            if (IsFree(total_min))
                return MainParking.ZeroFee;

            if (Is_One_Day_Max_Fee(total_min))
                return MainParking.MaxFee;




            int hours_fee = hour_of_parking_fee(total_min);
            int mins_fee = minute_of_parking_fee(total_min);
            return MainParking.every_of_parking_fee(total_min, hours_fee, mins_fee);

        }

        public bool IsFree(int total_min)//計算是否免費[0,10]
        {
            return total_min >= 0 && total_min <= 10;
        }

        public bool Is_One_Day_Max_Fee(int total_min)
        {
            int Max_Fee_in_One_Day = 300;
            return total_min >= Max_Fee_in_One_Day;
        }

        public int hour_of_parking_fee(int total_min) //每小時計費
        {
            int onehourfee = 10;
            return MainParking.hour_of_parking_fee(total_min, onehourfee);
        }
        public bool cal_Min(int total_min)
        {
            if (total_min % MainParking.Minute_in_a_Hour == MainParking.Zero_Minute)
                return true;
            return false;
        }

        public int minute_of_parking_fee(int total_min)//剩餘分鐘計費
        {
            int 減掉小時後剩下的分鐘數 = total_min % MainParking.Minute_in_a_Hour;

            int seven_fee = 7;
            int ten_fee = 10;

            if (減掉小時後剩下的分鐘數 <= MainParking.Half_Minute_in_a_hour)
                return seven_fee;
            return ten_fee;
        }
        public int every_of_parking_fee(int total_min, int hours_fee, int mins_fee)//總數計費
        {
            hours_fee = hour_of_parking_fee(total_min);
            mins_fee = minute_of_parking_fee(total_min);
            if (cal_Min(total_min))
                return hours_fee;
            return hours_fee + mins_fee;
        }

    }

    public class ParkingBFeeRule : IFeeRule
    {
        private readonly int DailyMaxFee = 250;
        private readonly IMinuteRule _minuteRule;
        public ParkingBFeeRule()
        {
            _minuteRule = new MinuteBRule();
        }
        public int CalcFee(DateTime start, DateTime end)
        {
            int total_min = _minuteRule.totalMins(start, end);

            if (isSatorSun(start) == false)
                return ParkingACalcFee(start, end);

            int hour_fee = hourfee(total_min);

            if (isMaxDailyFee(hour_fee))
                return DailyMaxFee;
            return hour_fee;

        }

        private int hourfee(int total_mins)
        {
            int B_fee = 15;
            return MainParking.hour_of_parking_fee(total_mins, B_fee);
        }

        public bool isMaxDailyFee(int fee)
        {
            return fee >= DailyMaxFee;
        }





        public bool isSatorSun(DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday;
        }

        private int ParkingACalcFee(DateTime start, DateTime end)
        {
            var feeRule = new ParkingAFeeRule();
            return feeRule.CalcFee(start, end);
        }


    }

    public class ParkingCFeeRule : IFeeRule
    {
        private readonly int DailyMaxFee = 300;
        private readonly IMinuteRule _minuteRule;
        int Hour_min = 60;
        public ParkingCFeeRule()
        {
            _minuteRule = new MinuteCRule();
        }
        public int CalcFee(DateTime start, DateTime end)
        {
            int total_min = _minuteRule.totalMins(start, end);

            if (isSatorSun(start))
                return MainParking.ZeroFee;

            int basic_fee = 20;
            if (total_min <=Hour_min)
                return basic_fee;
            int hour_fee = hourfee(total_min,basic_fee);

            if (isMaxDailyFee(hour_fee))
                return DailyMaxFee;

            return hour_fee+basic_fee;

        }

        private int hourfee(int total_mins,int first_fee)
        {
            int  accum_fee = 30;
            int Remain_time = total_mins - Hour_min;
            if (Remain_time <= MainParking.Zero_Minute)
                return MainParking.ZeroFee;
            return MainParking.hour_of_parking_fee(Remain_time, accum_fee);
        }

        public bool isMaxDailyFee(int fee)
        {
            return fee >= DailyMaxFee;
        }

        public bool isSatorSun(DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday;
        }

       


    }
    #endregion

    #region 主收費
    public class MainParking
    {
        public const int Minute_in_a_Hour = 60;
        public const int Zero_Minute = 0;
        public const int Half_Minute_in_a_hour = 30;
        public const int ZeroFee = 0;
        public const int MaxFee = 50;
        public const int One_hour_fee = 10;


        public static int CalMin(DateTime start, DateTime end)
        {
            DateTime startTime = ParkingTime(start);
            DateTime endTime = ParkingTime(end);
            return (int)endTime.Subtract(startTime).TotalMinutes;
        }
        public static bool cal_Min(int total_min)
        {
            if (total_min % Minute_in_a_Hour == Zero_Minute)
                return true;
            return false;
        }

        public static int hour_of_parking_fee(int total_min, int one_hour_fee) //每小時計費
        {
            return (total_min / Minute_in_a_Hour) * one_hour_fee;
        }
        public static int MinsPlus(int total_mins)
        {
            if (total_mins % Minute_in_a_Hour == Zero_Minute)
                return total_mins;

            return ((total_mins / Minute_in_a_Hour) * Minute_in_a_Hour) + Minute_in_a_Hour;
        }
        public static int minute_of_parking_fee(int total_min)//剩餘分鐘計費
        {
            int 減掉小時後剩下的分鐘數 = total_min % Minute_in_a_Hour;

            int seven_fee = 7;
            int ten_fee = 10;

            if (減掉小時後剩下的分鐘數 <= Half_Minute_in_a_hour)
                return seven_fee;
            return ten_fee;
        }
        public static int every_of_parking_fee(int total_min, int hours_fee, int mins_fee)//總數計費
        {
            hours_fee = hour_of_parking_fee(total_min, One_hour_fee);
            mins_fee = minute_of_parking_fee(total_min);
            if (cal_Min(total_min))
                return hours_fee;
            return hours_fee + mins_fee;
        }




        private static DateTime ParkingTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

    }
    #endregion

    #region 分鐘規則
    public class MinuteARule : IMinuteRule
    {
        public int totalMins(DateTime start, DateTime end)
        {
            int totalmins = MainParking.CalMin(start, end);
            return totalmins;
        }
    }

    public class MinuteBRule : IMinuteRule
    {
        public int totalMins(DateTime start, DateTime end)
        {
            int totalmins = MainParking.CalMin(start, end);
            return MainParking.MinsPlus(totalmins);
        }
    }

    public class MinuteCRule : IMinuteRule
    {
        public int totalMins(DateTime start, DateTime end)
        {
            int totalmins = MainParking.CalMin(start, end);
            return MainParking.MinsPlus(totalmins);
        }
    }

    #endregion
}
