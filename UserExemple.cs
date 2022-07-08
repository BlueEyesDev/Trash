using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
namespace ConsoleApp23
{
    internal class Program
    {
        [Flags]
        public enum CsharpEnum {
            Unknown = 0,
            Intern = 1,
            Employee = 2,
            Administration = 3,
            Root = 4
        }
        static void Main(string[] args)
        {
            string UserName = "Username1";
            string Password = "Password1";
            
            CsharpEnum GetAccount = (CsharpEnum)Enum.Parse(typeof(CsharpEnum), UTF8Encoding.UTF8.GetString(new WebClient().UploadValues("http://127.0.0.1/exemple/api.php", new NameValueCollection() {
                { "user", UserName},
                { "password", Password}
            })));

            switch (GetAccount)
            {   
                case CsharpEnum.Unknown:
                    Console.WriteLine("Compte non trouvé");
                    break;
                case CsharpEnum.Intern:
                    Console.WriteLine("Bonjour le stagiere");
                    break;
                case CsharpEnum.Employee:
                    Console.WriteLine("Bonjour l'employer");
                    break;
                case CsharpEnum.Administration:
                    Console.WriteLine("Bonjour l'administration");
                    break;
                case CsharpEnum.Root:
                    Console.WriteLine("<3 Bonjour le dev <3");
                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }
    }
}
