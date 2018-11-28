using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Cinema
{
    public enum User { EndOfWork = 0, Admin = 1, Customer = 2 }

    [Serializable]
    public class CustomerDictionaryObject
    {
        public string Key { get; set; }

        public Customer Value { get; set; }

        public CustomerDictionaryObject() { }

        public CustomerDictionaryObject(string key, Customer value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class ScheduleDictionaryObject
    {
        public DateTime Key { get; set; }

        public List<SessionDictionaryObject> Value { get; set; }

        public ScheduleDictionaryObject() { }

        public ScheduleDictionaryObject(DateTime key, List<SessionDictionaryObject> value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class SessionDictionaryObject
    {
        public string Key { get; set; }

        public List<HallSessionDictionaryObject> Value { get; set; }

        public SessionDictionaryObject() { }

        public SessionDictionaryObject(string key, List<HallSessionDictionaryObject> value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class HallSessionDictionaryObject
    {
        public string Key { get; set; }

        public Session Value { get; set; }

        public HallSessionDictionaryObject() { }

        public HallSessionDictionaryObject(TimeSpan key, Session value)
        {
            Key = key.ToString();
            Value = value;
        }
    }
    
    class APICinema
    {
        private List<Film> filmList = new List<Film>();

        private List<CinemaHallScheme> hallSchemeList = new List<CinemaHallScheme>();

        private SortedDictionary<DateTime, Schedule> daylySessionList = new SortedDictionary<DateTime, Schedule>();

        private SortedDictionary<string, Customer> customers = new SortedDictionary<string, Customer>();

        private struct Administrator
        {
            public string LogIn { get; set; } 
            public string Password { get; set; }
        }

        private Administrator admin;

        public APICinema()
        {
            Load();
        }

        private void Load()
        {
            admin.LogIn = "admin";
            admin.Password = "12345";

            XmlSerializer serializer = new XmlSerializer(typeof(List<Film>));
            using (Stream stream = new FileStream("Films.xml", FileMode.Open))
            {
                filmList = serializer.Deserialize(stream) as List<Film>;
            }

            serializer = new XmlSerializer(typeof(List<CinemaHallScheme>));
            using (Stream stream = new FileStream("Halls.xml", FileMode.Open))
            {
                hallSchemeList = serializer.Deserialize(stream) as List<CinemaHallScheme>;
            }

            List<CustomerDictionaryObject> transfer = new List<CustomerDictionaryObject>();
            serializer = new XmlSerializer(typeof(List<CustomerDictionaryObject>));
            using (Stream stream = new FileStream("Customers.xml", FileMode.Open))
            {
                transfer = serializer.Deserialize(stream) as List<CustomerDictionaryObject>;
            }
            foreach (var item in transfer)
            {
                customers.Add(item.Key, item.Value);
            }

            List<ScheduleDictionaryObject> transferForSchedule = new List<ScheduleDictionaryObject>();
            serializer = new XmlSerializer(typeof(List<ScheduleDictionaryObject>));
            using (Stream stream = new FileStream("Schedule.xml", FileMode.Open))
            {
                transferForSchedule = serializer.Deserialize(stream) as List<ScheduleDictionaryObject>;
            }
            foreach (var item in transferForSchedule)
            {
                Schedule schedule = new Schedule();
                SortedDictionary<string, ScheduleForHall> dayschedule = new SortedDictionary<string, ScheduleForHall>();

                foreach (var it in item.Value)
                {
                    ScheduleForHall hallschedule = new ScheduleForHall();
                    foreach (var i in it.Value)
                    {
                        hallschedule.HallSchedule.Add(TimeSpan.Parse(i.Key), i.Value);
                    }
                    dayschedule.Add(it.Key, hallschedule);
                }
                schedule.currentSchedule = dayschedule;
                daylySessionList.Add(item.Key, schedule);
            }
        }

        public void Export()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Film>));
            using (Stream stream = new FileStream("Films.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, filmList);
            }
            serializer = new XmlSerializer(typeof(List<CinemaHallScheme>));
            using (Stream stream = new FileStream("Halls.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, hallSchemeList);
            }

            serializer = new XmlSerializer(typeof(List<CustomerDictionaryObject>));
            List<CustomerDictionaryObject> transfer = new List<CustomerDictionaryObject>();
            foreach (var item in customers)
            {
                transfer.Add(new CustomerDictionaryObject(item.Key, item.Value));
            }

            using (Stream stream = new FileStream("Customers.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, transfer);
            }

            serializer = new XmlSerializer(typeof(List<ScheduleDictionaryObject>));
            List<ScheduleDictionaryObject> schedule = new List<ScheduleDictionaryObject>();
            foreach (var item in daylySessionList)
            {
                List<SessionDictionaryObject> daySchedule = new List<SessionDictionaryObject>();
                foreach (var it in item.Value.currentSchedule)
                {
                    List<HallSessionDictionaryObject> hallschedule = new List<HallSessionDictionaryObject>();
                    foreach (var i in it.Value.HallSchedule)
                    {
                        HallSessionDictionaryObject obj = new HallSessionDictionaryObject();
                        hallschedule.Add(new HallSessionDictionaryObject(i.Key, i.Value));
                    }
                    daySchedule.Add(new SessionDictionaryObject(it.Key, hallschedule));
                }
                schedule.Add(new ScheduleDictionaryObject(item.Key, daySchedule)); 
            }

            using (Stream stream = new FileStream("Schedule.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, schedule);
            }

        }

        //ADMIN OPTIONS
        public bool AdminAuthentication(string logIn, string password)
        {
            if (logIn == admin.LogIn && password == admin.Password)
            {
                AdminOptions();
                return true;
            }
            return false;
        }

        private void AdminMenu()
        {
            Console.WriteLine($"{"To add new film -", -50} |{" enter \"1\"",10}");
            Console.WriteLine($"{"To change information about film - ",-50} |{" enter \"2\"",-10}");
            Console.WriteLine($"{"To delete film from list -",-50} |{" enter \"3\"",-10}");
            Console.WriteLine($"{"To create schedule for date -",-50} |{" enter \"4\"",-10}");
            Console.WriteLine($"{"To change schedule -",-50} |{" enter \"5\"",-10}");
            Console.WriteLine($"{"To add new scheme of hall -",-50} |{ " enter \"6\"",-10}");
            Console.WriteLine($"{"To change information about scheme of hall -",-50} |{" enter \"7\"",-10}");
            Console.WriteLine($"{"To print statistic about permanent customers -",-50} |{" enter \"8\"",-10}");
            Console.WriteLine($"{"To come back to main menu - ",-50} |{" enter \"0\"",-10}");
        }

        private void AdminOptions()
        { 
            byte action = 1;
            while (action != 0)
            {
                AdminMenu();
                Console.WriteLine();
                Console.Write("Choose action...");
                try
                {
                    action = Byte.Parse(Console.ReadLine());
                    Console.Clear();
                    switch (action)
                    {
                        case 1: AddFilm(); break;
                        case 2: ChangeInfoAboutFilm(); break;
                        case 3: RemoveFilm(); break;
                        case 4: CreateSchedule(); break;
                        case 5: CorrectSchedule(); break;
                        case 6:
                            {
                                Console.WriteLine("Enter the name for new hall...");
                                string hallName = Console.ReadLine();
                                Console.WriteLine("Enter the count of row in the hall...");
                                byte rows = byte.Parse(Console.ReadLine());
                                Console.WriteLine("Enter the count of places in the row...");
                                byte places = byte.Parse(Console.ReadLine());
                                hallSchemeList.Add(new CinemaHallScheme(hallName, rows, places));
                            }
                            ; break;
                        case 7:
                            {
                                Console.WriteLine($"THE LIST OF HALLS:");
                                Console.WriteLine($"|{"Name",30} |{"Count of places",7} |");
                                foreach (CinemaHallScheme item in hallSchemeList)
                                {
                                    Console.WriteLine("|{0,30} |{1,7} |", item.Name, item.CountOfRows * item.CountOfPlacesInRow);
                                }
                                Console.WriteLine("Enter the name of hall, the session in which you want to add...");
                                string hallName = Console.ReadLine();
                                Console.WriteLine("To change name of  hall - enter \"1\"");
                                Console.WriteLine("To change count of rows - enter \"2\"");
                                Console.WriteLine("To change count of places in a row - enter \"3\"");
                                Console.WriteLine("To delete scheme of hall in a list - enter \"4\"");
                                Console.WriteLine("To come back to menu - enter \"0\"");
                                try
                                {
                                    switch (int.Parse(Console.ReadLine()))
                                    {
                                        case 1:
                                            {
                                                Console.WriteLine("Enter new name...");
                                                string name = Console.ReadLine();
                                                hallSchemeList[hallSchemeList.FindIndex(i => i.Name == hallName)].Name = name;
                                            }
                                            ; break;
                                        case 2:
                                            {
                                                Console.WriteLine("Enter new count of rows...");
                                                byte rows = byte.Parse(Console.ReadLine());
                                                hallSchemeList[hallSchemeList.FindIndex(i => i.Name == hallName)].CountOfRows = rows;
                                            }; break;
                                        case 3:
                                            {
                                                Console.WriteLine("Enter new count of rows...");
                                                byte places = byte.Parse(Console.ReadLine());
                                                hallSchemeList[hallSchemeList.FindIndex(i => i.Name == hallName)].CountOfPlacesInRow = places;
                                            }
                                            break;
                                        case 4: hallSchemeList.Remove(hallSchemeList.Find(i => i.Name == hallName)); break;
                                        case 0:; break;
                                        default: break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                            ; break;
                        case 8:
                            {
                                SortedDictionary<int, string> statistic = new SortedDictionary<int, string>();
                                foreach (var item in customers)
                                {
                                    statistic.Add(item.Value.CountWatchedFilms, item.Key);
                                }
                                Console.WriteLine($"{"LOGIN",20} | {"COUNT OF WATCHED FILMS",20}");
                                foreach (var item in statistic)
                                {
                                    Console.WriteLine("{0,20} | {1,10}", item.Value, item.Key);
                                }
                            }; break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (action != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                }
                Console.Clear();
            }
        }

        //admin options connected with film
        private void PrintFilmList()
        {
            Console.WriteLine("THE LIST OF FILMS");
            Console.WriteLine();
            Console.WriteLine($"{"FILM",-30}| {"START",-11}| {"FINISH",-11}");
            foreach (Film item in filmList)
            {
                Console.WriteLine($"{item.Name,-30}| {item.StartInCinema:d} | {item.FinishInCinema:d}");
            }
            Console.WriteLine();
        }

        private void PrintFilmList(Predicate<Film> Match, string name)
        {
            Console.WriteLine(name);
            foreach (Film item in filmList)
            {
                if(Match(item))
                    Console.WriteLine(item.Name);
            }
        }
   
        private void AddFilm()
        {
            Console.Write("Enter the name of film...");
            string name = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Enter the description of film...");
            string description = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Enter the duration of film...");
            short moveLength = short.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Enter the age limit of film...");
            byte ageLimit = byte.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Enter the base ticket price for film...");
            int baseTicketPrice = int.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Enter the date of start tracking film in cinema...");
            DateTime startInCinema = DateTime.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Enter the date of finish tracking film in cinema...");
            DateTime finishInCinema = DateTime.Parse(Console.ReadLine());
            filmList.Add(new Film(name, description, moveLength, ageLimit, baseTicketPrice, startInCinema, finishInCinema));
        }

        private void ChangeInfoAboutFilmMenu()
        {
            Console.WriteLine("To change the name of the film - enter \"1\"");
            Console.WriteLine("To change the description of the film - enter \"2\"");
            Console.WriteLine("To change the duration of the film - enter \"3\"");
            Console.WriteLine("To change the age limit of the film - enter \"4\"");
            Console.WriteLine("To change the base ticket price for the film - enter \"5\"");
            Console.WriteLine("To change the date of start tracking film in cinema - enter \"6\"");
            Console.WriteLine("To change the date of finish tracking film in cinema - enter \"7\"");
            Console.WriteLine("To come back to main menu - enter \"0\"");
        }

        private void ChangeInfoAboutFilm()
        {
            PrintFilmList();
            Console.WriteLine("Enter the name of film you want to change...");
            string filmName = Console.ReadLine();
            try
            {
                Console.Clear();
                int filmIndex = filmList.FindIndex(i => i.Name == filmName);
                ChangeInfoAboutFilmMenu();
                byte action = 1;
                Console.WriteLine();
                Console.Write("Choose action...");
                try
                {
                    action = Byte.Parse(Console.ReadLine());
                    Console.Clear();
                    switch (action)
                    {
                        case 1: {
                                Console.Write("Enter the new name of the film...");
                                string name = Console.ReadLine();
                                filmList[filmIndex].Name = name;
                            } break;
                        case 2: {
                                Console.WriteLine("Enter the new description of the film...");
                                string description = Console.ReadLine();
                                filmList[filmIndex].Description = description;
                            } break;
                        case 3: {
                                Console.Write("Enter the duration of film...");
                                short moveLength = short.Parse(Console.ReadLine());
                                filmList[filmIndex].MoveLength = moveLength;
                            } break;
                        case 4: {
                                Console.Write("Enter the age limit of film...");
                                byte ageLimit = byte.Parse(Console.ReadLine());
                                filmList[filmIndex].AgeLimit = ageLimit;
                            } break;
                        case 5: {
                                Console.Write("Enter the base ticket price for film...");
                                int baseTicketPrice = int.Parse(Console.ReadLine());
                                filmList[filmIndex].BaseTicketPrice = baseTicketPrice;
                            } break;
                        case 6: {
                                Console.Write("Enter the date of start tracking film in cinema...");
                                DateTime startInCinema = DateTime.Parse(Console.ReadLine());
                                filmList[filmIndex].StartInCinema = startInCinema;
                            } break;
                        case 7: {
                                Console.Write("Enter the date of finish tracking film in cinema...");
                                DateTime finishInCinema = DateTime.Parse(Console.ReadLine());
                                filmList[filmIndex].FinishInCinema = finishInCinema;
                            } break;
                        case 0: break;
                        default: Console.WriteLine("You enter wrong number!"); break;
                    }
                    if(action <= 1 && action >=7)
                        Console.WriteLine("The action was sucsessfull!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            catch(ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("You enter the wrong name of film! Try again!");
                Console.WriteLine("To come back to menu press \"Enter\"");
                Console.ReadLine();
                Console.Clear();
            }
        }

        private void RemoveFilm()
        {
            PrintFilmList();
            Console.WriteLine("Enter the name of film you want to change...");
            string filmName = Console.ReadLine();
            if ((filmList.FindIndex(i => i.Name == filmName)) != -1)
                filmList.RemoveAt(filmList.FindIndex(i => i.Name == filmName));
            else
                Console.WriteLine("You enter wrong name of film!");
        }

        //admin options connected with schedule
        private void CreateSchedule()
        {
            DateTime date = DateTime.Now;
            Console.WriteLine("Enter the date for which you want to create schedule...");
            date = DateTime.Parse(Console.ReadLine());
            Console.Clear();
            if (!daylySessionList.ContainsKey(date))
            {
                daylySessionList.Add(date, new Schedule(hallSchemeList));
                AddNewSession(date);
            }
            else
                Console.WriteLine("The schedule for entered date already exist!");
        }

        private void CorrectScheduleMenu()
        {
            Console.WriteLine("To add new session - enter \"1\"");
            Console.WriteLine("To delete session - enter \"2\"");
            Console.WriteLine("To delete schedule - enter \"3\"");
            Console.WriteLine("To come back to menu - enter \"0\"");
        }

        private void CorrectSchedule()
        {
            Console.WriteLine("Enter the date for which you want to correct schedule...");
            DateTime date = DateTime.Parse(Console.ReadLine());
            Console.Clear();
            if (daylySessionList.ContainsKey(date))
            {
                CorrectScheduleMenu();
                Console.WriteLine("Choose action...");
                byte action = byte.Parse(Console.ReadLine());
                switch (action)
                {
                    case 1: AddNewSession(date); break;
                    case 2: DeleteSession(date); break;
                    case 3: daylySessionList.Remove(date); break;
                }
                Console.Clear();
            }
            else
            {
                Console.WriteLine("The schedule for entered date doesn't exist!");
            }
        }

        private void DeleteSession(DateTime date)
        {
            Console.WriteLine($"SESSION LIST FOR {date}");
            daylySessionList[date].PrintInfo();
            Console.WriteLine("Enter the name of hall, session in which you want to delete...");
            string hallName = Console.ReadLine();
            Console.WriteLine("Enter the time of start session you want to delete...");
            TimeSpan sessionTime = TimeSpan.Parse(Console.ReadLine());
            try
            {
                daylySessionList[date].currentSchedule[hallName].HallSchedule.Remove(sessionTime);
                Console.WriteLine("The action was sucssesfull!");
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private void AddNewSessionErrorMenu()
        {
            Console.WriteLine("To try again - enter \"1\"");
            Console.WriteLine("To come back to menu - enter \"0\"");
        }

        private void AddNewSessionMenu()
        {
            Console.WriteLine("To come back to choosing film - enter \"1\"");
            Console.WriteLine("To come back to choosing hall - enter \"2\"");
            Console.WriteLine("To add new session with current setup - enter \"3\"");
            Console.WriteLine("To come back to menu - enter \"0\"");
        }

        private void AddNewSession(DateTime date)
        {
            Film film = null;
            CinemaHallScheme hall = null;
            TimeSpan sessionTime;
            byte action = 1;
            bool error = false;
            while (action != 0)
            {
                switch (action)
                {
                    case 1:
                        {
                            Console.WriteLine($"THE LIST OF FILMS, WHICH IS IN TRACKING ON DATE {date}:");
                            Console.WriteLine();
                            foreach (Film item in filmList)
                            {
                                if (date > item.StartInCinema && date < item.FinishInCinema)
                                    Console.WriteLine(item.Name);
                            }
                            Console.WriteLine();
                            Console.WriteLine("Enter the name of film, the session of which you want to add...");
                            string filmName = Console.ReadLine();
                            foreach (Film item in filmList)
                            {
                                if (item.Name == filmName)
                                    film = item;
                            }
                            if (film == null)
                            {
                                Console.WriteLine("You enter the wrong name of film! Try again!");
                                error = true;
                            }
                            else
                                action = 2;
                        }
                        break;
                    case 2:
                        {
                            Console.WriteLine("THE LIST OF HALLS:");
                            Console.WriteLine();
                            Console.WriteLine($"|{"Name",-20} |{"Count of places",-15} |");
                            foreach (CinemaHallScheme item in hallSchemeList)
                            {
                                Console.WriteLine("|{0,-20} |{1,-15} |", item.Name, item.CountOfRows * item.CountOfPlacesInRow);
                            }
                            Console.WriteLine();
                            Console.WriteLine("Enter the name of hall, the session in which you want to add...");
                            string hallName = Console.ReadLine();
                            foreach (CinemaHallScheme item in hallSchemeList)
                            {
                                if (item.Name == hallName)
                                    hall = item;
                            }
                            if (hall == null)
                            {
                                Console.WriteLine("You enter wrong name of hall! Try again!");
                                error = true;
                            }
                            else
                            {
                                if (daylySessionList[date].MaxFreeTime(hall.Name) < new TimeSpan(film.MoveLength / 60, film.MoveLength % 60, 0))
                                {
                                    Console.WriteLine("The hall has no enought free time between sessions for this film!");
                                    Console.WriteLine("Choose another hall from list!");
                                    error = true;
                                }
                                else
                                    action = 3;
                            }
                        }
                        break;
                    case 3:
                        {
                            Console.WriteLine($"SESSION LIST FOR {hall.Name} ON {date}");
                            Console.WriteLine();
                            Console.WriteLine($"{"Start of session",-17} | {"Film name",-30} | {"Finish of session",-15}");
                            foreach (var item in daylySessionList[date].currentSchedule[hall.Name].HallSchedule)
                            {
                                Console.WriteLine("{0,-17} | {1,-30} | {2,-15}", item.Key, item.Value.FilmName, item.Value.Finish);
                            }
                            Console.WriteLine();
                            Console.WriteLine($"The duration of film is {film.MoveLength}");
                            Console.WriteLine();
                            Console.WriteLine("Enter the time for start session...");
                            sessionTime = TimeSpan.Parse(Console.ReadLine());
                            try
                            {
                                daylySessionList[date].AddNewSession(sessionTime, film, hall);
                                Console.Clear();
                                AddNewSessionMenu();
                                switch (byte.Parse(Console.ReadLine()))
                                {
                                    case 1: action = 1; break;
                                    case 2: action = 2; break;
                                    case 0: action = 0; break;
                                    default: break;
                                }
                            }
                            catch (Exception e)
                            {
                                error = true;
                                Console.WriteLine(e.Message);
                            }
                            Console.Clear();

                        }
                        break;
                }
                if (error == true)
                {
                    AddNewSessionErrorMenu();
                    Console.WriteLine("Choose action...");
                    if (Byte.Parse(Console.ReadLine()) == 0)
                        action = 0;
                    error = false; 
                }
                Console.Clear();
            }
        }


        //CUSTOMER OPTIONS
        private void CustomerEnterMenu()
        {
            Console.WriteLine($"{"To log in - ", -45} {" enter \"1\"",-15}");
            Console.WriteLine($"{"To sign up - ",-45} {" enter \"2\"",-15}");
            Console.WriteLine($"{"To continue without registration - ",-45} {" enter \"3\"",-15}");
            Console.WriteLine($"{"To come back to main menu - ",-45} {" enter \"0\"",-15}");
            Console.WriteLine();
        }

        private void CustomerMenu()
        {
            Console.WriteLine($"{"To watch the film list - ",-50} {" enter \"1\"", -15}");
            Console.WriteLine($"{"To watch the schedule for current film - ",-50} {" enter \"2\"",-15}");
            Console.WriteLine($"{"To watch the schedule on the current day - ",-50} {" enter \"3\"",-15}");
            Console.WriteLine($"{"To buy ticket - ",-50} {" enter \"4\"",-15}");
            Console.WriteLine($"{"To come back to main menu - ",-50} {" enter \"0\"",-15}");
            Console.WriteLine();
        }

        public void CustomerOptions()
        {
            byte action;
            string currentAccount = null;
            do
            {
                CustomerEnterMenu();
                Console.Write("Choose action...");
                action = Byte.Parse(Console.ReadLine());
                Console.Clear();
                switch (action)
                {
                    case 1: currentAccount = Login(); break;
                    case 2: currentAccount = SignUp(); break;
                    case 3: break;
                    default:
                        {
                            Console.WriteLine("You enter uncorrect number!");
                            Console.WriteLine();
                            Console.WriteLine("To come back to menu press \"Enter\"");
                            Console.ReadLine();
                        }
                        break;
                }
                Console.Clear();
            }
            while (action != 0 && action != 3 && currentAccount == null);
            if (action != 0)
            {
                while (action != 0)
                {
                    CustomerMenu();
                    Console.Write("Choose action...");
                    action = Byte.Parse(Console.ReadLine());
                    Console.Clear();
                    switch (action)
                    {
                        case 1:
                            {
                                PrintFilmList();
                                Console.WriteLine("To watch detail information about film - enter name of film");
                                Console.WriteLine("To come back to menu press \"Enter\"");
                                string name = Console.ReadLine();
                                Console.WriteLine();
                                if (name != "")
                                {
                                    foreach (Film item in filmList)
                                    {
                                        if (item.Name == name)
                                            item.PrintInfo();
                                    }
                                }
                            }
                            break;
                        case 2: {
                                PrintFilmList();
                                Console.WriteLine("Enter name of film...");
                                string name = Console.ReadLine();
                                Console.WriteLine();
                                Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,15}", "FILM NAME", "START", "FINISH", "HALL");
                                foreach (var item in daylySessionList)
                                {
                                    Console.WriteLine($"Date: {item.Key}");
                                    item.Value.PrintInfo(i => i.FilmName == name);
                                }
                            } break;
                        case 3: { Console.WriteLine("Enter date...");
                                DateTime date = DateTime.Parse(Console.ReadLine());
                                Console.WriteLine();
                                Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,-15}", "FILM NAME", "START", "FINISH", "HALL");
                                daylySessionList[date]?.PrintInfo();} break;
                        case 4:
                            {
                                if (ToBuyTicket())
                                {
                                    if (currentAccount != null)
                                        customers[currentAccount].ByTicket();
                                }
                            } break;
                        case 0: break;
                        default: Console.WriteLine("You enter uncorrect number!"); break;
                    }
                    if (action != 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("To come back to menu press \"Enter\"");
                        Console.ReadLine();
                    }
                    Console.Clear();
                }
            }
        }

        private string Login()
        {
            Console.WriteLine("Enter login...");
            string login = Console.ReadLine();
            if (customers.ContainsKey(login))
            {
                Console.WriteLine("Enter password...");
                string password = Console.ReadLine();
                if (customers[login].Autorize(password))
                    return login;
                else
                    Console.WriteLine("You entered wrong password!");
            }
            else
                Console.WriteLine("Account with entered login doesn't exist!");
            return null;

        }

        private string SignUp()
        {
            Console.WriteLine("Enter your name...");
            string name = Console.ReadLine();
            Console.WriteLine("Enter your surname...");
            string surname = Console.ReadLine();
            Console.WriteLine("Enter your date of birth...");
            DateTime birth = DateTime.Parse(Console.ReadLine());
            string login = null;
            while (login == null)
            {
                Console.WriteLine("Enter your login...");
                login = Console.ReadLine();
                if (customers.ContainsKey(login))
                {
                    Console.WriteLine("Account with entered login already exist!Try again!");
                    login = null;
                }

            }
            Console.WriteLine("Enter your password...");
            string password = Console.ReadLine();
            customers.Add(login, new Customer(name, surname, birth, password));
            return login;
        }

        private bool ToBuyTicket()
        {
            PrintFilmList();
            Console.WriteLine("Enter name of film...");
            string name = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,15}", "FILM NAME", "START", "FINISH", "HALL");
            foreach (var item in daylySessionList)
            {
                Console.WriteLine($"Date: {item.Key}");
                item.Value.PrintInfo(i => i.FilmName == name);
            }
            Console.WriteLine("Enter the date...");
            DateTime date = DateTime.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Enter the name of hall...");
            string hallName = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Enter the time of session...");
            TimeSpan time = TimeSpan.Parse(Console.ReadLine());
            Console.WriteLine();
            byte row, place, action = 1;

            while (true)
            {
                Console.Clear();
                try
                {
                    daylySessionList[date].currentSchedule[hallName].HallSchedule[time].PrintInfoAboutTickets();
                    Console.WriteLine("Enter the number of row...");
                    row = byte.Parse(Console.ReadLine());
                    Console.WriteLine();
                    Console.WriteLine("Enter the number of place...");
                    place = byte.Parse(Console.ReadLine());
                    int countOfPlaceInRow = daylySessionList[date].currentSchedule[hallName].HallSchedule[time].HallScheme.CountOfPlacesInRow;
                    if (daylySessionList[date].currentSchedule[hallName].HallSchedule[time].tickets[((row - 1) * countOfPlaceInRow) + (place -1)].Status == PlaceStatus.Free)
                    {
                        daylySessionList[date].currentSchedule[hallName].HallSchedule[time].tickets[((row - 1) * countOfPlaceInRow) + (place -1)].Status = PlaceStatus.Sold;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Sorry, this place is already sold or reserved!");
                        Console.WriteLine("To choose another place - enter \"1\"");
                        Console.WriteLine("To come back to menu - enter \"0\"");
                        action = byte.Parse(Console.ReadLine());
                        if (action == 0)
                            return false;
                        Console.Clear();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("The session with entered parametrs doesn't exist!");
                }
            }
        }
    }   
}
