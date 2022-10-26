using System;
using System.Linq;
using System.Text;

namespace parking_practice
{
    //Q1:假設計算停車時間的邏輯是不理會秒數，例如13:10:59視為13:10:00。所以停車時間如果是13:10:59-13:11:10。仍視為1分鐘。
    internal class Program : Solution
    {
        static void Main(string[] args)
        {
            Solution sol = new Solution();

            #region 測資測試


            //"2022/5/1 00:00:00", "2022/5/3 00:11:59", 107, 3

            DateTime firstdate = new DateTime(2022, 10, 1, 23, 48, 0);
            DateTime seconddate = new DateTime(2022, 10, 3, 0, 11, 59);

            var result = sol.CalcFeeForMultiDays(firstdate, seconddate).ToList();

            //DateTime start = new DateTime();
            //DateTime end = new DateTime();
            //int fee = 0;

            //foreach (var item in result)
            //{
            //    start = item.StartTime;
            //    end = item.EndTime;
            //    fee = item.Fee;
            //    Console.WriteLine(start + " " + end +  " " + fee);
            //}

            StringBuilder sb = new StringBuilder();
            sb.Append("總計費用：");
            sb.Append(result.Sum(s => s.Fee));
            sb.Append("元");
            sb.AppendLine("");

            Console.WriteLine(sb.ToString());

            #endregion









        }
    }
}
