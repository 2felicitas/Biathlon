using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Globalization;

namespace biathlon
{
  public static class staminas
  {
    public static int[,] stamina = new int[50,20];  
  }
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      Start();
      Close();
    }

    //private void prepare_Click(object sender, RoutedEventArgs e)
    private void Start()
    {
      Dictionary<int, Athlete> athletes = new Dictionary<int, Athlete>();
      using (StreamReader sr = new StreamReader(@"athletes.txt"))
      {
        string line;
        line = sr.ReadLine();
        int id = 0;
        while (!sr.EndOfStream)
        {
          line = sr.ReadLine();
          if (line != "")
          {
            string[] attr = line.Split('|');
            Athlete newAt = new Athlete(id, attr[0].Trim(), attr[1].Trim(), attr[2],
                                new Attributes(double.Parse(attr[3], CultureInfo.InvariantCulture),
                                               double.Parse(attr[4], CultureInfo.InvariantCulture),
                                               double.Parse(attr[5], CultureInfo.InvariantCulture),
                                               double.Parse(attr[6], CultureInfo.InvariantCulture),
                                               double.Parse(attr[7], CultureInfo.InvariantCulture),
                                               int.Parse(attr[8], CultureInfo.InvariantCulture),
                                               double.Parse(attr[9], CultureInfo.InvariantCulture),
                                               double.Parse(attr[10], CultureInfo.InvariantCulture),
                                               double.Parse(attr[11], CultureInfo.InvariantCulture),
                                               double.Parse(attr[12], CultureInfo.InvariantCulture),
                                               double.Parse(attr[13], CultureInfo.InvariantCulture)),
                                        attr[14]);
            if (!athletes.ContainsValue(newAt))
            {
              athletes.Add(id, newAt);
              id++;
            }
          }
        }
      }
      Dictionary<string, Course[]> map = ReadMaps();

      List<Race> race = new List<Race>();
      RaceType type = new RaceType(RaceTypes.Individual);
      DateTime t = new DateTime(2017, 10, 1);
      double[,] range = new double[athletes.Count, 4];
      double[] behind = new double[athletes.Count];
      int[] points = {40,35,32,30,28,26,24,23,22,21,20,19,18,17,16,15,
                      14,13,12,11,10,9,8,7,6,5,4,3,2,1};
      Dictionary<int,int[]> season = new Dictionary<int, int[]>();
      for (int i = 0; i < athletes.Count; i++)
        season.Add(athletes.Keys.ElementAt(i), new int[map.Count * 4]);

      using (StreamWriter sw = File.CreateText(@"test.csv"))
      {
        for (int k = 0; k < map.Count * 4; k++)
        {
          if (k % 4 == 0)
          {
            type = new RaceType(RaceTypes.Sprint);
            t += new TimeSpan(14, 0, 0, 0);
          }
          else if (k % 4 == 1)
          {
            type = new RaceType(RaceTypes.Pursuit);
            t += new TimeSpan(1, 0, 0, 0);
          }
          else if (k % 4 == 2)
          {
            type = new RaceType(RaceTypes.Individual);
            t += new TimeSpan(3, 0, 0, 0);
          }
          else
          {
            type = new RaceType(RaceTypes.Mass_Start);
            t += new TimeSpan(2, 0, 0, 0);
          }
          race.Add(new Race(map.ElementAt(k / 4).Value[k % 4 % 3], type, t));

          sw.WriteLine(RaceInfo(race[k]));

          if (race[k].Type == RaceTypes.Pursuit)
            race[k].Draw(race[k - 1].Athletes, race[k - 1].Results);
          else if (race[k].Type == RaceTypes.Mass_Start)
          {
            List<int> idlist = new List<int>();
            List<Athlete> masslist = new List<Athlete>();
            var currentTable = season.OrderByDescending(x => x.Value.Sum()).ToList();
            for (int i = 0; i < 20; i++)
              idlist.Add(currentTable[i].Key);
            var topFiveThisEvent = season.OrderByDescending(x => x.Value.Skip(k - 3).Sum()).ToList();
            for (int i = 0; i < 5; i++)
              idlist.Add(topFiveThisEvent.First(x => !idlist.Contains(x.Key)).Key);
            foreach (var id in idlist)
              masslist.Add(athletes[id]);
            race[k].Draw(masslist);
          }
          else
            race[k].Draw(athletes.Values.ToList());

          race[k].Start();

          var h = race[k].Results.OrderBy(x => x.Finish, new TimeSpanCompare()).ToList();
          List<double> CourseTime = h.Select(x => x.CourseTime.Value.TotalSeconds).ToList();
          for (int curA = 0; curA < h.Count; curA++)
          {
            for (int lap = 0; lap < race[k].Laps - 1; lap++)
            {
              if (race[k].RangeDistr[lap] == 's')
                range[race[k].Athletes[h[curA].Bib].ID, 3] += 5;
              else
                range[race[k].Athletes[h[curA].Bib].ID, 1] += 5;
              foreach (var l in h[curA].Range[lap])
              {
                if (l)
                  if (race[k].RangeDistr[lap] == 's')
                    range[race[k].Athletes[h[curA].Bib].ID, 2]++;
                  else
                    range[race[k].Athletes[h[curA].Bib].ID, 0]++;
              }
            }
            if (k % 4 == 0)
              behind[race[k].Athletes[h[curA].Bib].ID] += (CourseTime[curA] / CourseTime.OrderBy(x => x).Take(5).Average() - 1) * 100;

            TimeSpan? m = TimeSpan.FromSeconds(CourseTime[curA]) - TimeSpan.FromSeconds(CourseTime.Min());
            if (CourseTime[curA] == CourseTime.Min())
              m = TimeSpan.FromSeconds(CourseTime.Min());

            //TimeSpan? m = h[j].Finish - h[j].Start;
            //for (int i = 1; i < race[k].Type.Laps; i++)
            //{
            //  m += h[j].TimeStamps[i - 1].Last() - h[j].TimeStamps[i].First();
            //}
            sw.WriteLine(AthleteRaceResults(race[k], h[curA], m, curA));
          }
          for (int i = 0; i < Math.Min(points.Length, h.Count); i++)
            season[race[k].Athletes[h[i].Bib].ID][k] = points[i];

          Extensions.updateStamina(ref athletes, race[k].Athletes);

          foreach (var id in athletes.Keys)
            if (race[k].Type == RaceTypes.Sprint)
              athletes[id].Attributes.RestoreStamina(1);
            else
              athletes[id].Attributes.RestoreStamina(4);

          sw.WriteLine("\r\n");
        }
        sw.WriteLine();
        var cup = season.OrderByDescending(x => x.Value.Sum() - x.Value.Min())
                        .CreateOrderedEnumerable(x => x.Value.Max(), Comparer<int>.Default, true)
                        .CreateOrderedEnumerable(x => x.Value.Count(val => val == x.Value.Max()), Comparer<int>.Default, true)
                        .CreateOrderedEnumerable(x => x.Value.Where(val => val < Math.Max(x.Value.Max(),1)).Max(), Comparer<int>.Default, true)
                        .CreateOrderedEnumerable(x => x.Value.Count(val => val < x.Value.Max() && val == x.Value.Where(inval => inval < Math.Max(x.Value.Max(), 1)).Max()), Comparer<int>.Default, true)
                        .ToList();
        sw.WriteLine(";№;Name;Points;;Total;Prone;Standing;% Behind");
        for (int i = 0; i < athletes.Count; i++)
        {
          sw.WriteLine(";{0};{1}. {2};{3};;{4:f2}%;{5:f2}%;{6:f2}%;{7:f2}%",
            i + 1, athletes[cup[i].Key].Name.First(),
            athletes[cup[i].Key].Surname,
            cup[i].Value.Sum() - cup[i].Value.Min(),
            (range[cup[i].Key, 0] + range[cup[i].Key, 2]) /
            (range[cup[i].Key, 1] + range[cup[i].Key, 3]) * 100,
            range[cup[i].Key, 0] / range[cup[i].Key, 1] * 100,
            range[cup[i].Key, 2] / range[cup[i].Key, 3] * 100,
            behind[cup[i].Key] / (map.Count));
        }
        using (StreamWriter sw2 = File.CreateText(@"test2.csv"))
        {
          sw2.WriteLine(); 
          for (int i = 0; i < athletes.Count; i++)
          {
            sw2.Write("{0};{1};",
              i + 1,
              athletes[cup[i].Key].FullName);
            for (int j = 0; j < map.Count * 4; j++)
              sw2.Write("{0};", cup[i].Value[j]);
            sw2.Write(cup[i].Value.Sum() - cup[i].Value.Min());
            sw2.Write("\r\n");
          }
          for (int k = 0; k < 2; k++)
          {
            if (k == 0)
              sw2.Write("\r\n\r\n;Sprint Cup;;;;;;;;;;;;;;;;Individual Cup;\r\n");
            else 
              sw2.Write("\r\n\r\n;Pursuit Cup;;;;;;;;;;;;;;;;Mass Start Cup;\r\n");
            Dictionary<int, int[]> season1 = new Dictionary<int, int[]>();
            Dictionary<int, int[]> season2 = new Dictionary<int, int[]>();
            foreach (int id in season.Keys)
              season1.Add(id, season[id].GetNth(k, 4).ToArray());
            var cup1 = season1.OrderByDescending(x => x.Value.Sum())
                              .ThenByDescending(x => x.Value.Max())
                              .ThenByDescending(x => x.Value.Count(val => val == x.Value.Max()))
                              .ThenByDescending(x => x.Value.Where(val => val < Math.Max(x.Value.Max(), 1)).Max())
                              .ThenByDescending(x => x.Value.Count(val => val < x.Value.Max() && val == x.Value.Where(inval => inval < Math.Max(x.Value.Max(), 1)).Max()))
                              .ToList();
            foreach (int id in season.Keys)
              season2.Add(id, season[id].GetNth(k + 2, 4).ToArray());
            var cup2 = season2.OrderByDescending(x => x.Value.Sum())
                              .ThenByDescending(x => x.Value.Max())
                              .ThenByDescending(x => x.Value.Count(val => val == x.Value.Max()))
                              .ThenByDescending(x => x.Value.Where(val => val < Math.Max(x.Value.Max(), 1)).Max())
                              .ThenByDescending(x => x.Value.Count(val => val < x.Value.Max() && val == x.Value.Where(inval => inval < Math.Max(x.Value.Max(), 1)).Max()))
                              .ToList();
            for (int i = 0; i < athletes.Count; i++)
            {
              sw2.Write("{0};{1};",
                i + 1,
                athletes[cup1[i].Key].FullName);
              for (int j = 0; j < map.Count; j++)
                sw2.Write("{0};", cup1[i].Value[j]);
              sw2.Write("{0};;;;;;;{1};{2}. {3};;;;;;",
                cup1[i].Value.Sum(), i + 1,
                athletes[cup2[i].Key].Name.First(),
                athletes[cup2[i].Key].Surname);
              for (int j = 0; j < map.Count; j++)
                sw2.Write("{0};", cup2[i].Value[j]);
              sw2.Write(cup2[i].Value.Sum());
              sw2.Write("\r\n");
            } 
          }

          /*sw.WriteLine();
          for (int i = 0; i < 11; i++)
            sw.Write(i.ToString().PadLeft(4));
          sw.WriteLine();
          double sum = 0.0;
          for (int i = 0; i < 11; i++)
          {
            sum += i * an[i];
            sw.Write(an[i].ToString().PadLeft(4));
          }
          sw.Write("  " + (int)(100 * (1 - sum / (20 * 500))) + "%" + "    " + (int)(100 * (1 - sumwind/(20*r))));*/
        }
      }
      //Refresh(ref race);
    }

    private Dictionary<string, Course[]> ReadMaps()
    {
      Dictionary<string, Course[]> map = new Dictionary<string, Course[]>();
      using (StreamReader sr = new StreamReader(@"maps.txt"))
      {
        string line;
        while (!sr.EndOfStream)
        {
          line = sr.ReadLine();
          if (line != "")
          {
            int i = 0;
            string[] lines = line.Split(';');
            if (lines[1] == "3.0")
              i = 2;
            else if (lines[1] == "2.0")
              i = 1;
            if (!map.ContainsKey(lines[0]))
              map.Add(lines[0], new Course[3]);
            map[lines[0]][i] = new Course(lines[0], lines[1]);
            while ((line = sr.ReadLine()) != "" && line != null)
            {
              string[] hw = line.Split(' ');
              map[lines[0]][i].Add(double.Parse(hw[0], CultureInfo.InvariantCulture),
                         double.Parse(hw[1], CultureInfo.InvariantCulture));
            }
          }
        }
      }
      return map;
    }

    private string RaceInfo(Race r)
    {
      StringBuilder line = new StringBuilder();
      line.Append(";;");
      line.Append(r.Course.Name);
      line.Append(";");
      line.Append(r.Type.ToString());
      line.Append(";");
      line.Append(r.Weather.Type.ToString());
      line.Append(";");
      line.Append(r.Weather.WindSpeed.Average().ToString("f4"));
      line.Append("\n");
      line.Append(RaceHeader(r));
      return line.ToString();
    }

    private string RaceHeader(Race r)
    {
      StringBuilder line = new StringBuilder();
      if (r.Type == RaceTypes.Pursuit)
        line.Append(";№;Name;Time;Range;Trail;Course Time;Clear Pursuit;");
      else
        line.Append(";№;Name;Time;Range;Trail;Course Time;;");
      for (int i = 0; i < r.Laps; i++)
      {
        if (i == 0)
          line.Append("Start;");
        else
          line.AppendFormat(";Shooting {0};", i);
        for (int j = 1; j < r.Course.Sections.Length; j++)
        {
          line.Append(r.Course.Sections[j] + i * r.Course.Sections.Last());
          line.Append(" km;;;");
        }
      }
      return line.ToString();
    }

    private string AthleteRaceResults(Race r, RaceStats h, TimeSpan? CourseTime, int curAthlete)
    {
      StringBuilder line = new StringBuilder();
      line.Append(FinishInfo(r, h, curAthlete));
      line.Append(CourseTime.ToString(true));
      line.Append(PursuitTime(h, r.Type));
      line.Append(h.ToString(r.Type));
      return line.ToString();
    }

    private string FinishInfo(Race r, RaceStats h, int curAthlete)
    {
      StringBuilder line = new StringBuilder();
      line.Append(curAthlete + 1);
      line.Append(";");
      line.Append(h.Bib + 1);
      line.Append(";");
      line.AppendFormat(r.Athletes[h.Bib].FullName);
      line.Append(";");
      line.Append(h.Finish.ToString(false));
      line.Append(";");
      for (int i = 0; i < r.Laps - 1; i++)
      {
        line.Append(h.Range[i].Count(x => !x));
        line.Append(" ");
      }
      line.Append(";");
      line.Append((h.Finish - r.Leaders.Finish).ToString(true));
      line.Append(";");
      return line.ToString();
    }

    private string PursuitTime(RaceStats h, RaceTypes type)
    {
      StringBuilder line = new StringBuilder();
      line.Append(";");
      if (type == RaceTypes.Pursuit)
        line.Append((h.Finish - h.Start).ToString(false));
      line.Append(";");
      return line.ToString();
    }
  }

    /*private void Refresh(ref Race a)
    {
      resGrid.Columns.Clear();
      rangeGrid.Children.Clear();
      for (int sec = 0; sec < a.Course.Sections.Length; sec++)
      {
        var col = new DataGridTextColumn();
        col.Width = 60;
        col.Header = a.Course.Sections[sec].ToString() + " km";
        col.Binding = new Binding(string.Format("[{0}]", sec));
        col.Binding.StringFormat = "h\\:mm\\:ss\\.ff";
        resGrid.Columns.Add(col);
      }
      resGrid.ItemsSource = a.Results[0].TimeStamps;
      //for (int lap = 0; lap < a.Type.Laps; lap++)
      //  for (int sec = 0; sec < Track.Sections.Length; sec++)
      //  {
      //    var col = new DataGridTextColumn();
      //    col.Width = 60;
      //    col.Header = (Track.Sections[sec] + lap * Track.Sections.Last()).ToString() + " km";
      //    col.Binding = new Binding(string.Format(".Timestamps[{0}][{1}]", lap, sec));
      //    col.Binding.StringFormat = "h\\:mm\\:ss\\.ff";
      //    resGrid.Columns.Add(col);
      //  }
      //resGrid.ItemsSource = a.Results;
      for (int lap = 0; lap < a.Type.Laps - 1; lap++)
        for (int sec = 0; sec < a.Course.Sections.Length; sec++)
        {
          var shot = new Ellipse();
          shot.Stroke = Brushes.Black;
          shot.Height = shot.Width = 13;
          shot.VerticalAlignment = VerticalAlignment.Center;
          shot.HorizontalAlignment = HorizontalAlignment.Center;
          Binding binding = new Binding();
          binding.Path = new PropertyPath(string.Format("[{0}]", sec));
          binding.Source = a.Results[0].Rcolor[lap];
          BindingOperations.SetBinding(shot, Ellipse.FillProperty, binding);
          Grid.SetColumn(shot, sec);
          Grid.SetRow(shot, lap);
          rangeGrid.Children.Add(shot);
        }
    }*/

  static class Extensions
  {
    public static string ToString(this TimeSpan? b, bool plus)
    {
      StringBuilder a = new StringBuilder();
      if (plus && b.Value != TimeSpan.Zero && b.Value.TotalMinutes < 15)
        a.Append("+");
      a.Append("{0:");
      if (b.Value.TotalHours >= 1)
        a.Append("h\\:");
      if (b.Value.TotalMinutes >= 1)
        if (b.Value.TotalMinutes >= 10)
          a.Append("mm\\:");
        else
          a.Append("m\\:");
      if (b.Value.TotalSeconds >= 10)
        a.Append("ss\\,ff}");
      else
        a.Append("s\\,ff}");
      return string.Format(a.ToString(), b.Value);
    }

    public static List<RaceStats> OrderByFinish (this List<RaceStats> b)
    {
      return b.OrderBy(x => x.Finish, new TimeSpanCompare()).ToList();
    }

    public static IEnumerable<T> GetNth<T>(this IEnumerable<T> list, int s, int n)
    {
      for (int i = s; i < list.Count(); i += n)
        yield return list.ElementAt(i);
    }

    public static void updateStamina(ref Dictionary<int, Athlete> a, List<Athlete> source)
    {
      source = source.OrderBy(x => x.ID).ToList();
      var idcollection = source.Select(x => x.ID).ToList();
      int i = 0;
      foreach (var id in idcollection)
      {
        a[id].Attributes.CurStamina = source[i].Attributes.CurStamina;
        i++;
      }
    }
  }
}
