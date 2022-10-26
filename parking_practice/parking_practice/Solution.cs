using System;
using System.Collections.Generic;
using System.Linq;

namespace parking_practice
{

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

        readonly int Minute_in_a_Hour = 60;
        readonly int Zero_Minute = 0;
        readonly int Half_Minute_in_a_hour = 30;
        readonly int ZeroFee = 0;
        readonly int MaxFee = 50;
        readonly int FeeInEveryHour = 10;



        //readonly IFeeStat

        public class SingleDayFee
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public int Fee { get; set; }
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
                Fee = (int)CalcFee(datetimerange.start, datetimerange.end)
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

        #region Q2 停一天的費用
        //Q2


        public int CalcFee(DateTime start, DateTime end)
        {
            int total_min = CalMin(start, end);

            if (IsFree(total_min))
                return ZeroFee;

            if (Is_Full_Day(total_min))
            {
                return MaxFee;
            }



            int hours_fee = hour_of_parking_fee(total_min);
            int mins_fee = minute_of_parking_fee(total_min);
            return every_of_parking_fee(total_min, hours_fee, mins_fee);

        }

        public bool IsFree(int total_min)//計算是否免費[0,10]
        {
            return total_min >= 0 && total_min <= 10;
        }
        public bool Is_Full_Day(int total_min)
        {
            int 一整天的分鐘數 = 1439;
            return total_min == 一整天的分鐘數;
        }

        public int hour_of_parking_fee(int total_min) //每小時計費
        {
            return (total_min / Minute_in_a_Hour) * FeeInEveryHour;
        }
        public bool cal_Min(int total_min)
        {
            if (total_min % Minute_in_a_Hour == Zero_Minute)
                return true;
            return false;
        }

        public int minute_of_parking_fee(int total_min)//剩餘分鐘計費
        {
            int 減掉小時後剩下的分鐘數 = total_min % Minute_in_a_Hour;

            int seven_fee = 7;
            int ten_fee = 10;

            if (減掉小時後剩下的分鐘數 <= Half_Minute_in_a_hour)
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
        public int CalMin(DateTime start, DateTime end)
        {
            DateTime startTime = ParkingTime(start);
            DateTime endTime = ParkingTime(end);
            return (int)endTime.Subtract(startTime).TotalMinutes;
        }


        private DateTime ParkingTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

        #endregion
    }


}
