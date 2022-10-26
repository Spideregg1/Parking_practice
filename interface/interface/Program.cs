using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_practice
{

    public interface IUser
    {
        bool 性別(string gender);
        string 星座();

    }
    public class real_user : IUser
    {
        public bool 性別(string gender)
        {
            return gender=="男"?true:false;
        }
        public string 星座()
        {
            return "巨蟹";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            IUser user = new real_user();

            Console.WriteLine(user.性別("女"));
        }
    }
}
