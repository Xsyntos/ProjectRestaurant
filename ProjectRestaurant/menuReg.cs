﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectRestaurant
{
    class menuReg
    {
        //Reservation menu

        public static Action reservationMenu1(int y)
        {

            void resv()
            {
                var list = new List<option>();
                for (int i = 1; i <= 7; i++)
                {
                    list.Add(new option
                    {
                        printToConsole = $"{DateTime.Now.AddDays((double)i + (y * 7)).Day}-{DateTime.Now.AddDays((double)i + (y * 7)).Month}-{DateTime.Now.AddDays((double)i + (y * 7)).Year}, 12 PM",
                        func = reservationMenuDay(new DateTime(DateTime.Now.AddDays((double)i + (y * 7)).Year, DateTime.Now.AddDays((double)i + (y * 7)).Month, DateTime.Now.AddDays((double)i + (y * 7)).Day, 12, 0, 0))
                    }
                    );
                    list.Add(new option
                    {
                        printToConsole = $"{DateTime.Now.AddDays((double)i + (y * 7)).Day}-{DateTime.Now.AddDays((double)i + (y * 7)).Month}-{DateTime.Now.AddDays((double)i + (y * 7)).Year}, 6 PM",
                        func = reservationMenuDay(new DateTime(DateTime.Now.AddDays((double)i + (y * 7)).Year, DateTime.Now.AddDays((double)i + (y * 7)).Month, DateTime.Now.AddDays((double)i + (y * 7)).Day, 18, 0, 0))
                    }


    // TO DO: ga terug knop
    );
                }
                list.Add(new option
                {
                    printToConsole = "Next Week",
                    func = reservationMenu1(y + 1)
                });

                if (y > 0)
                {
                    list.Add(new option
                    {
                        printToConsole = "Previous Week",
                        func = reservationMenu1(y - 1)
                    });
                }
                list.Add(new option
                {
                    printToConsole = "Return",
                    func = mainCustomermenu
                });

                Menu menu = new Menu
                {
                    options = list.ToArray(),
                    prefix = "Select a Date"
                };
                menu.RunMenu();
            }
            return resv;
        }
        public static Action reservationMenuDay(DateTime i)
        {
            var date = i;
            void reservationMenu2()
            {
                var availabletables = json_table.getFreeTable(date);
                var option = new List<option>();
                foreach (var i in availabletables)
                {
                    if (i.vip)
                    {
                        option.Add(new option
                        {
                            printToConsole = $"{i.Id}. {i.capacity} persons VIP",
                            func = makeReservationMenu(date, i)
                        }
                        );
                    }
                    else
                    {
                        option.Add(new option
                        {
                            printToConsole = $"{i.Id}. {i.capacity} persons",
                            func = makeReservationMenu(date, i)
                        }
                        );
                    }


                }
                option.Add(new option
                {
                    printToConsole = "Return",
                    func = reservationMenu1(0)
                });
                Menu menu = new Menu
                {
                    options = option.ToArray(),
                    prefix = "Select a Table"

                };

                menu.RunMenu();
            }
            return reservationMenu2;
        }
        public static Action makeReservationMenu(DateTime date, table table)
        {
            void MakeReservationMenu()
            {
                var owncredit = client_variable.user.creditcard;
                if (client_variable.user.role == "guest")
                {
                    Console.WriteLine("Enter your Creditcard Number!");
                    var credit = Console.ReadLine();
                    if (Checker.Check(credit))
                    {
                        client_variable.user.creditcard = credit;
                        Console.WriteLine("Please enter your Name!");
                        client_variable.user.username = Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid Creditcard number!\nPress a Key to continue...");
                        Console.ReadKey();
                        reservationMenu1(0)();
                    }
                }
                else
                {
                    if (client_variable.user.creditcard == "123")
                    {
                        Console.WriteLine("Enter your Creditcard Number!");
                        var credit = Console.ReadLine();
                        if (Checker.Check(credit))
                        {
                            client_variable.user.creditcard = credit;
                            json_customer.updateUser();
                            owncredit = client_variable.user.creditcard;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Creditcard number!\nPress a Key to continue...");
                            Console.ReadKey();
                            reservationMenu1(0)();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Do you want to use your account creditcard? YES/NO");
                        string x = Console.ReadLine();

                        if (!(x.ToUpper() == "YES" || x.ToUpper() == "Y"))
                        {

                            Console.WriteLine("Enter your Creditcard Number!");
                            var credit = Console.ReadLine();
                            if (Checker.Check(credit))
                            {
                                client_variable.user.creditcard = credit;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Creditcard number!\nPress a Key to continue...");
                                Console.ReadKey();
                                reservationMenu1(0)();
                            }
                        }

                    }
                }

                json_reservation.makeReservation(date, client_variable.user, table);
                var arr = json_reservation.reservationsofdate(date);
                reservation res = null;

                foreach (var x in arr)
                {
                    if (x.table.Id == table.Id)
                    {
                        res = x;
                    }
                }
                client_variable.user.creditcard = owncredit;
                Console.WriteLine("Reservation Complete\n" + "Reservation Code is: " + res.Id);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                if (client_variable.user.role == "guest")
                {
                    GuestMenu();
                }
                else
                {
                    mainCustomermenu();
                }
            }
            return MakeReservationMenu;
        }
        // Next menu:
        public static void mainMenu()
        {
            var optie = new option[] {
                new option
                {
                    printToConsole = "Log-in",
                    func = login.Login
                },
                new option
                {
                    printToConsole =  "Registrate" ,
                    func = Registration.RegistrationFirstVersion
                },
                new option
                {
                    printToConsole = "Log-in as Guest",
                    func = GuestMenu
                },
                new option
                {
                    printToConsole = "Quit"
                }
            };
            var menu = new Menu
            {
                options = optie,
                prefix = "Welcome to our Restaurant"
            };
            menu.RunMenu();
        }
        public static void mainCustomermenu()
        {
            var optie = new option[]
            {
                new option
                {
                    printToConsole = "Make a Reservation",
                    func = reservationMenu1(0)
                },
                new option
                {
                    printToConsole = "Check reservations",
                    func = userReservationMenu
                },
                new option
                {
                    printToConsole = "Change account settings",
                    func = accountSettings
                },
                new option
                {
                    printToConsole = "Take Away",
                    func = Takeaway.Takeawayinput

                },
                new option
                {
                    printToConsole = "Log-out",
                    func = mainMenu
                }

            };
            var menu = new Menu
            {
                prefix = "Welcome to our Restaurant",
                options = optie
            };
            menu.RunMenu();
        }
        public static void mainAdminmenu()
        {
            var x = new option[]
            {
               new option
               {
                   printToConsole = "Check all reservations"
               },
               new option
               {
                   printToConsole = "Tables"
               },
               new option
               {
                   printToConsole = "Change Account settings",
                   func = accountSettings
               },
               new option
               {
                   printToConsole = "Log-out",
                   func = mainMenu
               }


            };
            var menu = new Menu
            {
                prefix = "Welcome Boss",
                options = x
            };
            menu.RunMenu();

        }


        public static void userReservationMenu()
        {
            var options = new List<option>();
            foreach (var i in json_reservation.getUserReservations())
            {
                options.Add(
                    new option
                    {
                        printToConsole = $"{i.Id} - {i.date}",
                        func = reservationMenu(i)
                    }
                    );

            }
            options.Add(new option
            {
                printToConsole = "Return",
                func = mainCustomermenu
            });

            var menu = new Menu
            {
                prefix = "Reservations",
                options = options.ToArray()
            };
            menu.RunMenu();
        }

        public static Action reservationMenu(reservation res)
        {
            void rest()
            {
                void cancel()
                {
                    Console.Clear();
                    Console.WriteLine(@$"

                                           
 _____         _                       _   
| __  |___ ___| |_ ___ _ _ ___ ___ ___| |_ 
|    -| -_|_ -|  _| .'| | |  _| .'|   |  _|
|__|__|___|___|_| |__,|___|_| |__,|_|_|_|  
                                           
Cancel Reservation  ");
                    Console.WriteLine("WARNING:  If you want to cancel the reservation, you must do so 24 hours in advance, otherwise we will charge you for 10 euros!");
                    Console.WriteLine("To Cancel your Reservation type YES");
                    var x = Console.ReadLine();
                    if (x.ToUpper() == "YES")
                    {
                        json_reservation.removeReservation(res.Id);
                        Console.WriteLine("Your Reservation is succesfully canceled!\nPress a key to Continue");
                        if ((res.date - DateTime.Now).Days < 0)
                        {
                            Console.WriteLine("We Charged you 10 euros!");
                        }
                        Console.ReadKey();
                        if (client_variable.user.role == "guest") { GuestMenu(); }
                        else { mainCustomermenu(); }
                    }
                    else
                    {
                        Console.WriteLine("Your Reservation is not canceled\nPress a key to Continue");
                        Console.ReadKey();
                        if (client_variable.user.role == "guest") { GuestMenu(); }
                        else { menuReg.mainCustomermenu(); }

                    }
                }
                var options = new option[]
                {
                    new option
                    {
                        printToConsole = "Cancel Reservation",
                        func = cancel
                    },

                    new option
                    {
                        printToConsole = "Return",
                        func = userReservationMenu
                    }
                };
                if (client_variable.user.role == "guest")
                {
                    options[1].func = GuestMenu;
                }

                var menu = new Menu
                {
                    options = options,
                    prefix = $"Id: {res.Id}\nTable: {res.table.stringy()}\nDate: {res.date}"
                };

                menu.RunMenu();
            }
            return rest;
        }

        public static void guestReservation()
        {
            Console.Clear();
            Console.WriteLine(@$"

                                           
 _____         _                       _   
| __  |___ ___| |_ ___ _ _ ___ ___ ___| |_ 
|    -| -_|_ -|  _| .'| | |  _| .'|   |  _|
|__|__|___|___|_| |__,|___|_| |__,|_|_|_|  
                                           
Search Reservation  ");

            Console.WriteLine("Typ your username of your reservation");
            string username = Console.ReadLine();
            Console.WriteLine("Typ your code of your reservation");
            string code = Console.ReadLine();
            reservation res = json_reservation.GetReservation(code);

            if (json_reservation.doesReservationexist(code))
            {
                if (res.user.username == username && res.user.role == "guest")
                {
                    reservationMenu(res)();
                }
                else
                {
                    Console.WriteLine("Invalid username or reservation doesn't belong to a guest!\nPress a Key to continue...");
                    Console.ReadKey();
                    GuestMenu();
                }
            }
            else
            {
                Console.WriteLine("Invalid Reservation Code!\nPress a Key to continue...");
                Console.ReadKey();
                GuestMenu();
            }




        }
        //Guest-Account
        public static void GuestMenu()
        {
            client_variable.user = new user()
            {
                role = "guest",
            };
            var options = new List<option>();

            options.Add(new option
            {
                printToConsole = "Make Reservation",
                func = reservationMenu1(0)
            });
            options.Add(new option
            {
                printToConsole = "Check Reservation",
                func = guestReservation
            });
            options.Add(new option
            {
                printToConsole = "Take-Away"
            });
            options.Add(new option
            {
                printToConsole = "Log-out",
                func = mainMenu
            });

            Menu men = new Menu
            {
                options = options.ToArray(),
                prefix = "Welcome Guest!"
            };
            men.RunMenu();
        }

        // Account Settings!
        public static void accountSettings()
        {
            void changeUsername()
            {
                Console.Clear();
                Console.WriteLine(@$"

                                           
 _____         _                       _   
| __  |___ ___| |_ ___ _ _ ___ ___ ___| |_ 
|    -| -_|_ -|  _| .'| | |  _| .'|   |  _|
|__|__|___|___|_| |__,|___|_| |__,|_|_|_|  
                                           
Change Username  ");
                Console.WriteLine("Enter your old username");
                string oldusername = Console.ReadLine();
                if (oldusername == client_variable.user.username)
                {
                    Console.WriteLine("Enter your new username");
                    string newusername = Console.ReadLine();
                    Console.WriteLine("Confirm your new username");
                    string newusername2 = Console.ReadLine();
                    if (newusername == newusername2)
                    {
                        client_variable.user.username = newusername;
                        json_customer.updateUser();
                        accountSettings();
                    }
                    else
                    {
                        Console.WriteLine("Usernames are not the same!\nPress a Key to continue...");
                        Console.ReadKey();
                        accountSettings();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid username!\nPress a Key to continue...");
                    Console.ReadKey();
                    accountSettings();
                }
            }
            void changePassword()
            {
                Console.Clear();
                Console.WriteLine(@$"

                                           
 _____         _                       _   
| __  |___ ___| |_ ___ _ _ ___ ___ ___| |_ 
|    -| -_|_ -|  _| .'| | |  _| .'|   |  _|
|__|__|___|___|_| |__,|___|_| |__,|_|_|_|  
                                           
Change password  ");
                Console.WriteLine("Enter your old password");
                string oldusername = Console.ReadLine();
                if (Hash.Encrypt(oldusername) == client_variable.user.password)
                {
                    Console.WriteLine("Enter your new password");
                    string newusername = Console.ReadLine();
                    Console.WriteLine("Confirm your new password");
                    string newusername2 = Console.ReadLine();
                    if (newusername == newusername2)
                    {
                        client_variable.user.password = Hash.Encrypt(newusername);
                        json_customer.updateUser();
                        accountSettings();
                    }
                    else
                    {
                        Console.WriteLine("Passwords are not the same!\nPress a Key to continue...");
                        Console.ReadKey();
                        accountSettings();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Password!\nPress a Key to continue...");
                    Console.ReadKey();
                    accountSettings();
                }


            }

            void changeCreditcard()
            {
                Console.Clear();
                Console.WriteLine(@$"

                                           
 _____         _                       _   
| __  |___ ___| |_ ___ _ _ ___ ___ ___| |_ 
|    -| -_|_ -|  _| .'| | |  _| .'|   |  _|
|__|__|___|___|_| |__,|___|_| |__,|_|_|_|  
                                           
Change Creditcard  ");

                string oldusername = "123";
                if (!(client_variable.user.creditcard == "123"))
                {
                    Console.WriteLine("Enter your old creditcard");
                    oldusername = Console.ReadLine();
                }
                if (oldusername == client_variable.user.creditcard)
                {
                    Console.WriteLine("Enter your new creditcard");
                    string newusername = Console.ReadLine();
                    if (!Checker.Check(newusername))
                    {
                        Console.WriteLine("Invalid Creditcard!\nPress a Key to continue...");
                        Console.ReadKey();
                        accountSettings();
                    }
                    Console.WriteLine("Confirm your new creditcard");
                    string newusername2 = Console.ReadLine();
                    if (newusername == newusername2)
                    {
                        client_variable.user.creditcard = newusername;
                        json_customer.updateUser();
                        accountSettings();
                    }
                    else
                    {
                        Console.WriteLine("Creditcards are not the same!\nPress a Key to continue...");
                        Console.ReadKey();
                        accountSettings();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid creditcard!\nPress a Key to continue...");
                    Console.ReadKey();
                    accountSettings();
                }
            }

            option[] options = new option[]
            {
                new option()
                {
                    printToConsole = "Change Username",
                    func = changeUsername
                },
                new option()
                {
                    printToConsole = "Change Password",
                    func = changePassword
                },
                new option()
                {
                    printToConsole = "Change Creditcard",
                    func = changeCreditcard
                },
                new option()
                {
                    printToConsole = "Return",
                    func = mainCustomermenu
                }
            };

            if (client_variable.user.role == "admin")
            {
                options[3].func = mainAdminmenu;
            }

            Menu men = new Menu()
            {
                options = options,
                prefix = "Change Account Settings"
            };
            men.RunMenu();



        }



    }
}
