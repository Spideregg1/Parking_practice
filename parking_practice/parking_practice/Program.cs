using System;
using System.Linq;
using System.Text;

namespace MainParking
{
    //Q1:假設計算停車時間的邏輯是不理會秒數，例如13:10:59視為13:10:00。所以停車時間如果是13:10:59-13:11:10。仍視為1分鐘。
    internal class Program
    {
        static void Main(string[] args)
        {


            var fee = new ParkingAFeeRule();
            var parkinga = new Solution(fee);

            var feeB = new ParkingBFeeRule();
            var parkingb = new Solution(feeB);

            var feeC = new ParkingCFeeRule();
            var parkingc = new Solution(feeC);

            #region 測資測試


            //"2022/5/1 00:00:00", "2022/5/3 00:11:59", 107, 3

            DateTime firstdate = new DateTime(2022, 5, 2, 23, 49, 0);

            DateTime seconddate = new DateTime(2022, 5, 4, 0,11, 59);

            

            var result = parkinga.CalcFeeForMultiDays(firstdate, seconddate).ToList();

            var resulta = parkinga.CalcParkingFee(firstdate, seconddate).TotalFee;
            var resultb = parkingb.CalcParkingFee(firstdate, seconddate).TotalFee;
            var resultc = parkingc.CalcParkingFee(firstdate, seconddate).TotalFee;
            //DateTime start = new DateTime();
            //DateTime end = new DateTime();
            //int fee = 0;

            //foreach (var item in result)
            //{
            //    start = item.StartTime;
            //    end = item.EndTime;

            //    Console.WriteLine(start + " " + end );
            //}

            StringBuilder sb = new StringBuilder();
            sb.Append("方案A：");
            sb.AppendLine(resulta.ToString()+"元");
            sb.Append("方案B：");
            sb.AppendLine(resultb.ToString()+"元");
            sb.Append("方案C：");
            sb.AppendLine(resultc.ToString() + "元");



            Console.WriteLine(sb.ToString());

            #endregion









        }
    }
}
