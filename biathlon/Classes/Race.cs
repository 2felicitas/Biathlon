using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  class Season
  {
    private List<Race>       races;
    private HashSet<Athlete> curAthletes;
  }

  /// <summary>
  /// Предоставляет объект для проведения гонки и хранения её параметров
  /// </summary>
  partial class Race
  {
    private const double    penaltyLap = 150.0;                                     // Длина штрафного круга, м
    private bool            isCompleted;                                            // Прошла ли гонка
    private RaceType        type;                                                   // Вид гонки
    private Course          course;                                                 // Трасса
    private DateTime        date;                                                   // Время гонки
    private List<Athlete>   athletes;                                               // Список биатлонистов
                                                                                    //    после жеребьевки
    private Weather         weather;                                                // Погода
    private List<RaceStats> results;                                                // Результаты гонки
    private TimeStampList   leaders;                                                // Список времён лидеров
                                                                                    //    на каждой отсечке
    /// <summary>
    /// Инициализирует новый экземпляр класса Race с заданными параметрами трассы,
    /// вида и даты, а также генерирует вектор силы ветра
    /// </summary>
    /// <param name="_course">Трасса</param>
    /// <param name="_type">Тип</param>
    /// <param name="_date">Дата</param>
    public Race(Course _course, RaceType _type, DateTime _date)
    {
      course = _course;
      type = _type;
      date = _date;
      weather = new Weather();
      athletes = new List<Athlete>();
      isCompleted = false;
    }

    #region {Свойства}
    public bool IsCompleted
    {
      get { return isCompleted; }
    }
    public Course Course
    {
      get { return course; }
    }
    public DateTime Date
    {
      get { return date; }
    }
    public List<Athlete> Athletes
    {
      get { return athletes; }
    }
    public Weather Weather
    {
      get { return weather; }
    }
    public List<RaceStats> Results
    {
      get { return results; }
    }
    public TimeStampList Leaders
    {
      get { return leaders; }
    }
    public RaceTypes Type
    {
      get { return type.Type;}
    }
    public int Laps
    {
      get { return type.Laps;}
    }
    public string RangeDistr
    {
      get { return type.RangeDistr; }
    }
    #endregion
  }
}
