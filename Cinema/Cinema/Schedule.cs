using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    class ScheduleForHall
    {
        public SortedDictionary<TimeSpan, Session> HallSchedule = new SortedDictionary<TimeSpan, Session>();

        public ScheduleForHall() { }
    }
    
    class Schedule
    {
        public SortedDictionary<string, ScheduleForHall> currentSchedule = new SortedDictionary<string, ScheduleForHall>();

        public Schedule() { }

        public Schedule(List<CinemaHallScheme> hallList) 
        {
            foreach (CinemaHallScheme item in hallList)
            {
                currentSchedule.Add(item.Name, new ScheduleForHall());
            }
        }

        public void AddNewSession(TimeSpan start, Film film, CinemaHallScheme hall)
        {
            if (!currentSchedule[hall.Name].HallSchedule.ContainsKey(start))
            {
                Session prevElement = null;
                TimeSpan nextElement = new TimeSpan();
                TimeSpan finish = start + new TimeSpan(film.MoveLength / 60, film.MoveLength % 60, 0);
                foreach (var item in currentSchedule[hall.Name].HallSchedule)
                {
                    if (start < item.Key)
                    {
                        nextElement = item.Key;
                        break;
                    }
                    prevElement = item.Value;
                }
                if (prevElement != null)
                {
                    if (TimeSpan.Parse(prevElement.Finish) > start)
                        throw new Exception("This time in schedule is already occupied!");
                }
                if (nextElement != new TimeSpan(0, 0, 0))
                {
                    if (finish > nextElement)
                        throw new Exception("This time in schedule is already occupied!");
                }
                currentSchedule[hall.Name].HallSchedule.Add(start, new Session(film.Name, hall, finish.ToString()));
            }
            else
                throw new Exception("This time in schedule is already occupied!");
        }

        public TimeSpan MaxFreeTime(string hallName)
        {
            TimeSpan startWorkCinema = new TimeSpan(8, 0, 0);
            TimeSpan finishWorkCinema = new TimeSpan(23, 30, 0);
            TimeSpan maxFreeTime = new TimeSpan(0, 0, 0);
            TimeSpan freeTime;
            TimeSpan finishPrevSession = startWorkCinema;
            foreach(var item in currentSchedule[hallName].HallSchedule)
            {
                freeTime = item.Key - finishPrevSession;
                if(freeTime > maxFreeTime)
                    maxFreeTime = freeTime;
                finishPrevSession = TimeSpan.Parse(item.Value.Finish);
            }
            freeTime = finishWorkCinema - finishPrevSession;
            if (freeTime > maxFreeTime)
                maxFreeTime = freeTime;
            return maxFreeTime;
        }

        public void PrintInfo()
        {
            foreach (var item in currentSchedule)
            {
                foreach (var i in item.Value.HallSchedule)
                {
                     Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,-15}", i.Value.FilmName, i.Key, i.Value.Finish, i.Value.HallScheme.Name);
                }
            }
        }

        public void PrintInfo(Predicate<Session> Match)
        {
            foreach (var item in currentSchedule)
            {
                foreach(var i in item.Value.HallSchedule)
                {
                    if(Match(i.Value))
                        Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,-15}", i.Value.FilmName, i.Key, i.Value.Finish, i.Value.HallScheme.Name);
                }
            }
        }

        public void PrintInfo(string hallName)
        {
            foreach (var item in currentSchedule)
            {
                if (item.Key == hallName)
                {
                    foreach (var i in item.Value.HallSchedule)
                    {
                        Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,-15}", i.Value.FilmName, i.Key, i.Value.Finish, i.Value.HallScheme.Name);
                    }
                }
            }
        }

        public void PrintInfo(Predicate<Session> Match, string hallName)
        {
            foreach (var item in currentSchedule)
            {
                if (item.Key == hallName)
                {
                    foreach (var i in item.Value.HallSchedule)
                    {
                        if (Match(i.Value))
                            Console.WriteLine("{0,-30}| {1,-10}| {2,-10}| {3,-15}", i.Value.FilmName, i.Key, i.Value.Finish, i.Value.HallScheme.Name);
                    }
                }
            }
        }
    }
}
