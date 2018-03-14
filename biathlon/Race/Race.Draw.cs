using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  partial class Race
  {
    /// <summary>
    /// Жеребьевка биатлонистов на гонку
    /// </summary>
    /// <param name="atList">Список биатлонистов, участвующих в гонке</param>
    public void Draw(List<Athlete> b,
                     List<RaceStats> prSprint = null)
    {
      List<Athlete> atList = new List<Athlete>(b);

      if ((athletes = ListDraw(atList, prSprint)) == null)
        return;

      int n = athletes.Count;
      results = new List<RaceStats>();                                              // Инициализация списка
      for (int i = 1; i <= n; i++)                                                  // результатов                                                                          // 
        results.Add(new RaceStats(i, Laps, course.Sections.Length));           // 

      Initialize(prSprint, results.Count);

      leaders = new TimeStampList(Laps, course.Sections.Length);               // Инициализация списка
                                                                                    // лидеров
    }

    private List<Athlete> ListDraw(List<Athlete> atList, List<RaceStats> prSprint)
    {
      int n = atList.Count;
      if (atList.Count == 0)
        return null;
      List<Athlete> result = new List<Athlete>();
      if (Type == RaceTypes.Sprint || Type == RaceTypes.Individual)
        for (int i = 0; i < n; i++)
        {
          int j = StaticRandom.Rand(atList.Count);
          result.Add(atList[j]);
          atList.RemoveAt(j);
        }
      else if (Type == RaceTypes.Pursuit)
        if (prSprint != null)
        {
          prSprint.OrderByFinish().Take(Math.Min(60, atList.Count)).ToList().
            ForEach(x => result.Add(atList[x.Bib]));
          //prSprint.OrderBy(x => x.Finish(), new TimeSpanCompare()).
          //        Take(Math.Min(60, atList.Count)).ToList().
          //        ForEach(x => athletes.Add(atList[x.Bib]));

          //for (int i = 0; i < Math.Min(60, atList.Count); i++)
          //  athletes.Add(atList[h[i].Bib]);
        }
        else
          return null;
      else
        result = atList.ToList();
      return result;
    }

    private List<RaceStats> Initialize(List<RaceStats> prSprint, int n)
    {
      if (prSprint != null)
      {
        var h = prSprint.OrderBy(x => x.Finish, new TimeSpanCompare()).ToList();    // Заполнение начальных
        for (int i = 0; i < n; i++)                                                 // значений времени
          results[i].TimeStamps[0, 0] = h[i].Finish - h[0].Finish;                  // результатами спринта,
                                                                                    // если гонка преследования
      }                                                                             
      else
        results.ForEach(x => x.TimeStamps[0, 0] = TimeSpan.FromSeconds(0));         // Нулями - иначе
      return results;
    }
  }
}
