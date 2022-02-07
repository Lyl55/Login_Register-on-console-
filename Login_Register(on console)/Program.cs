using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Login_Register_on_console_.Model;

namespace Login_Register_on_console_
{
    class Program
    {
        static class Helper
        {
            private static int _index = 0;
            public static int indexer => ++_index;
            public static readonly string secretcode = "ITClub";
            public static List<User> users { get; set; } = new List<User>();
        }

        static void Main(string[] args)
        {
            inc:
            Console.WriteLine("Start to...\n1.Register\n2.Login");
            int number = Convert.ToInt32(Console.ReadLine());
            switch (number)
            {
                case 1:
                    Register();
                    goto inc;
                case 2:
                    Login();
                    goto inc;
                default:
                    Console.WriteLine("Enter correct value");
                    goto inc;
            }
        }


        static void Login()
        {
            try
            {
                int tryCounter = 0;
                int tryCount = 3;

                DateTime timeOutTime = new DateTime();
                int timeOutLimitAsMinute = 1;
                bool timeOuted = false;
                var model = new LoginModel();
                Console.Write("Username or email: ");
                model.UsernameOrEmail = Console.ReadLine();
                Console.Write("Password: ");
                model.Password = Console.ReadLine();
                var user = validationforLogin(model);

                while (true)
                {

                    if (tryCounter < tryCount)
                    {

                        if (model.Password == user.Password) break;

                        else
                        {
                            ++tryCounter;
                            Console.WriteLine("Password is not correct, please check it !");
                            Console.Write("Password: ");
                            model.Password = Console.ReadLine();
                        }

                        timeOuted = true;
                    }
                    else if (timeOuted == true)
                    {
                        Console.WriteLine("TimeOut: {0} minute", timeOutLimitAsMinute);
                        timeOutTime = DateTime.Now;
                        timeOuted = false;
                    }
                    else
                    {
                        if (DateTime.Now.AddMinutes(-timeOutLimitAsMinute).Minute == timeOutTime.Minute)
                        {
                            Console.WriteLine("Your timeout ended!");
                            tryCounter = 0;
                            break;
                        }
                    }
                }

                switch (user.UserType)
                {
                    case "Admin":
                        ShowAdminPanel(user);
                        x:
                        Console.WriteLine("1.Make active\n2.Make de-active\n3.Add\n4.Delete\n5.Go to beginning");
                        int x = Convert.ToInt32(Console.ReadLine());
                        switch (x)
                        {
                            case 1:
                                activebyAdmin();
                                goto x;
                            case 2:
                                deactivebyAdmin();
                                goto x;
                            case 3:
                                addbyAdmin();
                                goto x;
                            case 4:
                                deletebyAdmin();
                                goto x;
                            case 5:
                                break;
                            default:
                                Console.WriteLine("Number is entered not correct!!!");
                                goto x;
                        }

                        break;
                    case "Standard":
                        Console.WriteLine("Login succsessfully!!!");
                        break;
                    default:
                        Console.WriteLine("Incorrect username or password");
                        break;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        static void deletebyAdmin()
        {
            Console.WriteLine("Enter user id for remove:");
            int id = Convert.ToInt32(Console.ReadLine());
            var itemToRemove = Helper.users.SingleOrDefault(r => r.Id == id);
            if (itemToRemove != null)
            {
                Helper.users.Remove(itemToRemove);
                Console.WriteLine("Successfully deleted!!!");
            }
            else
            {
                Console.WriteLine("No item for delete!!!");
            }

        }

        static void addbyAdmin()
        {
            var model = new RegisterModel();
            model.Id = Helper.indexer;
            Console.WriteLine("Your information:");
            Console.Write("Name:");
            model.Name = Console.ReadLine();
            Console.Write("Surname: ");
            model.Surname = Console.ReadLine();
            Console.Write("Username: ");
            model.Username = Console.ReadLine();
            Console.Write("Password: ");
            model.Password = Console.ReadLine();
            Console.Write("Email: ");
            model.Email = Console.ReadLine();
            var user = validationforRegister(model);
            Helper.users.Add(user);
            Console.WriteLine("Successfully added!!!");
        }

        static void activebyAdmin()
        {
            var user = methodForUserLogin();
            if (!user.IsActive)
            {
                user.IsActive = true;
                Console.WriteLine("You are made active by admin");
            }
            else Console.WriteLine("You are already active");
        }

        static void deactivebyAdmin()
        {
            var user = methodForUserLogin();
            if (user.IsActive)
            {
                user.IsActive = false;
                Console.WriteLine("You are made de-active by admin ");
            }
            else Console.WriteLine("You are already de-active");
        }

        static void ShowAdminPanel(User currentuser)
        {
            Console.WriteLine("\tId\tFullName\tUsername\tEmail\t\t\tIsActive");
            foreach (var item in Helper.users)
            {
                if (item.UserType == "Admin" && item.Id != currentuser.Id) continue;
                Console.WriteLine(
                    $"\t{item.Id}\t{item.Name} {item.Surname}\t{item.Username}\t{item.Email}\t\t\t{item.IsActive}");
            }
        }

        static User methodForUserLogin()
        {
            var model = new LoginModel();
            Console.Write("Username or Email: ");
            model.UsernameOrEmail = Console.ReadLine();
            Console.Write("Password: ");
            model.Password = Console.ReadLine();
            var user = validationforLogin(model);
            return user;
        }

        static void Register()
        {
            var model = new RegisterModel();
            model.Id = Helper.indexer;
            Console.WriteLine("Your information:");
            Console.Write("Name:");
            model.Name = Console.ReadLine();
            Console.Write("Surname: ");
            model.Surname = Console.ReadLine();
            Console.Write("Username: ");
            model.Username = Console.ReadLine();
            Console.Write("Password: ");
            model.Password = Console.ReadLine();
            Console.Write("Email: ");
            model.Email = Console.ReadLine();
            Console.Write("Code: ");
            model.UserType = Console.ReadLine() == Helper.secretcode
                ? "Admin"
                : "Standard";
            model.IsActive = true;
            var user = validationforRegister(model);
            Helper.users.Add(user);
        }

        static User validationforLogin(LoginModel model)
        {
            User user = new User();
            if (model.UsernameOrEmail.Contains('@'))
            {
                user = Helper.users.Find(a => a.Email == model.UsernameOrEmail);
            }
            else
            {
                user = Helper.users.Find(a => a.Username == model.UsernameOrEmail);
            }

            if (user.Id <= 0)
            {
                throw new Exception("Doesn't exist");
            }

            if (!user.IsActive)
            {
                throw new Exception("Doesn't available");
            }

            return user;
        }

        static User validationforRegister(RegisterModel model)
        {

            if ((model.Name).Length < 3 || (model.Surname).Length < 3 || (model.Username).Length < 3)
            {
                throw new Exception("Name,surname or username format is wrong");
            }

            if ((model.Password).Length < 5 || !model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsLower) ||
                !model.Password.Any(char.IsDigit))
            {
                throw new Exception("Wrong password format");
            }

            string specialSymbol = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "";
            char[] arr = specialSymbol.ToCharArray();
            bool isValid = false;
            foreach (var ch in model.Password)
            {
                foreach (var symb in arr)
                {
                    if (ch == symb)
                    {
                        isValid = true;
                        break;
                    }
                }
            }

            if (!isValid)
            {
                throw new Exception("Wrong password format");
            }

            string pattern =
                @"\A(?:[a-z0-9!#$%&'+/=?^`{|}-]+(?:\.[a-z0-9!#$%&'+/=?^`{|}-]+)@(?:[a-z0-9](?:[a-z0-9-][a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (Regex.IsMatch(model.Email, pattern))
            {
                throw new Exception("Mail is invalid");
            }

            User user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Surname = model.Surname,
                Username = model.Username,
                Password = model.Password,
                Id = model.Id,
                IsActive = model.IsActive,
                UserType = model.UserType,
            };
            return user;
        }
    }
}

