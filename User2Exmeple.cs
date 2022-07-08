using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
namespace ConsoleApp23
{
    internal class Program
    {
        [Flags]
        public enum CsharpEnum
        {
            Unknown = 0,
            Intern = 1,
            Employee = 2,
            Administration = 3,
            Root = 4
        }

        class AccountInfo
        {
            public string Password { get; set; }
            public CsharpEnum AccountType { get; set; }
        }
        static Dictionary<string, AccountInfo> Account = new Dictionary<string, AccountInfo>() {
            { "Username1", new AccountInfo() { Password = "Password1",  AccountType = CsharpEnum.Root} }
        };
        static void Main(string[] args)
        {
            string UserName = "Username1";
            string Password = "Password1";

            if (!Account.ContainsKey(UserName))
            {
                Console.WriteLine("Nop");
            }
            else
            {
                if (Account[UserName].Password == Password)
                {
                    switch (Account[UserName].AccountType)
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
                } else {
                    Console.WriteLine("BadPassword");
                }
            }
            Console.ReadLine();
        }
    }
}
