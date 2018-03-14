using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace biathlon
{

  class TimeStampList
  {
    ObservableCollection<ObservableCollection<TimeSpan?>> list;

    public TimeStampList(int laps, int sections)
    {
      list = new ObservableCollection<ObservableCollection<TimeSpan?>>();
      for (int i = 0; i < laps; i++)
      {
        list.Add(new ObservableCollection<TimeSpan?>());
        for (int j = 0; j < sections; j++)
          list[i].Add(null);
      }
    }
    public TimeSpan? this[int lap, int sec]
    {
      get { return list[lap][sec]; }
      set { list[lap][sec] = value;}
    }
    public ObservableCollection<TimeSpan?> this[int lap]
    {
      get { return list[lap]; }
    }


    public TimeSpan? Last(int lap)
    {
      return list[lap].Last();
    }

    public int Laps
    { get { return list.Count; } }

    public int Sections
    { get { return list[0].Count; } }

    public TimeSpan? Finish
    { get { return list.Last().Last(); } }

    public TimeSpan? Start
    { get { return list[0][0]; } }
  }

  class RangeList
  {
    ObservableCollection<ObservableCollection<bool>> list;

    public RangeList(int laps, int sections)
    {
      list = new ObservableCollection<ObservableCollection<bool>>();
      for (int i = 0; i < laps - 1; i++)
      {
        list.Add(new ObservableCollection<bool>());
        for (int j = 0; j < 5; j++)
          list[i].Add(false);
      }
    }

    public bool this[int lap, int sec]
    {
      get { return list[lap][sec]; }
      set { list[lap][sec] = value; }
    }
    public ObservableCollection<bool> this[int lap]
    {
      get { return list[lap]; }
    }

    public int Misses(int laps)
    {
      int res = 0;
      for (int i = 0; i < laps - 1; i++ )
        res += list[i].Count(x => !x);
      return res;
    }
    public int Ranges
    { get { return list.Count; } }

    public int Shots
    { get { return list[0].Count; } }
  }

  class RaceStats : INotifyPropertyChanged
  {
    private TimeStampList       timeStamps;
    private RangeList           range;
    //private ObservableCollection<ObservableCollection<SolidColorBrush>> rcolor;
    private int bib;


    #region {Свойства}
    public int Bib
    {
      get { return bib; }
      set { bib = value - 1; }
    }
    public TimeStampList TimeStamps
    {
      get { return timeStamps; }
      set { SetField(ref timeStamps, value, "TimeStamps"); }
    }
    public RangeList Range
    {
      get { return range; }
      set { SetField(ref range, value, "Range"); }
    }
    /*public ObservableCollection<ObservableCollection<SolidColorBrush>> Rcolor
    {
      get { return rcolor; }
      set { SetField(ref rcolor, value, "Rcolor"); }
    }*/
    #endregion

    public RaceStats(int n, int laps, int sections)
    {
      Bib = n;
      timeStamps = new TimeStampList(laps, sections);
      range = new RangeList(laps, sections);
      //rcolor = new ObservableCollection<ObservableCollection<SolidColorBrush>>();
    }

    public string ToString(RaceTypes type)
    {

      double[] b = null;
      switch (type)
      {
        case RaceTypes.Sprint:
        case RaceTypes.Mass_Start:
          b = new double[] { 0.0, 1.1, 1.8, 2.5 };
          break;
        case RaceTypes.Individual:
          b = new double[] { 0.0, 1.2, 2.0, 3.0 };
          break;
        case RaceTypes.Pursuit:
          b = new double[] { 0.0, 1.2, 1.6, 2.0 };
          break;
      }
      StringBuilder a = new StringBuilder();
      for (int i = 0; i < TimeStamps.Laps; i++)
      {
        for (int j = 0; j < TimeStamps.Sections; j++)
        {
          //a.AppendFormat("{0:h\\:mm\\:ss\\.ff};", TimeStamps[i][j]);
          a.Append(TimeStamps[i, j].Value.TotalDays.ToString() + ';');
          if (j != 0)
          {
            a.Append(((b[j] - b[j - 1]) / (timeStamps[i, j].Value - timeStamps[i, j - 1].Value).TotalHours).ToString("f2"));
            a.Append(";");
            a.Append(staminas.stamina[bib, i * (b.Length - 1) + j - 1].ToString());
            a.Append(";");
          }
        }
        if (i != TimeStamps.Laps - 1)
        {
          for (int j = 0; j < Range.Shots; j++)
            if (Range[i, j])
              a.Append('\u25CF');
            else
              a.Append('\u25CB');
          a.Append(";");
        }
      }
      return a.ToString();
    }

    public void Hit(int lap, int n)
    {
      Range[lap, n] = true;
      //Rcolor[lap][n] = Brushes.Black;
    }

    public TimeSpan? Finish
    {
      get { return timeStamps.Finish; }
    }

    public TimeSpan? Start
    {
      get { return timeStamps.Start; }
    }

    public TimeSpan? CourseTime
    {
      get
      {
        TimeSpan? m = Finish - Start;
        for (int i = 1; i < TimeStamps.Laps; i++)
          m += TimeStamps[i - 1].Last() - TimeStamps[i].First();
        return m;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetField<T>(ref T field, T value, string propertyName)
    {
      if (EqualityComparer<T>.Default.Equals(field, value))
        return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /* protected virtual void OnPropertyChanged(string propertyName)
     {
       if (PropertyChanged != null)
       {
         PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
       }
     }
   }*/
  }
}
