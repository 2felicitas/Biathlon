using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using System.IO;

namespace biathlon
{
  partial class Race
  {
    /// <summary>
    /// Расчитывает время гонки для биатлониста с номером threadID
    /// </summary>
    /// <param name="threadID">Номер биатлониста в гонке</param>
    private void go(object threadID)
    {
      int bib = (int)threadID;
      int n = course.Sections.Length;
      for (int j = 0; j < Laps; j++)
      {
        for (int k = 1; k < n; k++)
        {
          results[bib].TimeStamps[j, k] = results[bib].TimeStamps[j, k - 1] +       // Расчёт времени на k-ой
                                                  sectionPassTime(bib, j, k);       //    отсечке j-ого круга
          LeaderChange(bib, j, k);
        }
        if (j != Laps - 1)
        {
          results[bib].TimeStamps[j + 1, 0] = results[bib].TimeStamps[j, n - 1] +   // Расчёт времени на стрельбу и
                                                                   range(bib, j);   //    выполнение стрельбы
          LeaderChange(bib, j, 0);
        }
      }
    }

    private void LeaderChange(int bib, int lap, int section)
    {
      if (leaders[lap, section] == null ||                                          // Смена лидера, если она
          results[bib].TimeStamps[lap, section] < leaders[lap, section])            //    имела место
      {
        leaders[lap, section] = results[bib].TimeStamps[lap, section];
      }
    }

    private static void delay(double t)
    {
      /*int i = 0;
      var timer = new System.Timers.Timer();
      timer.Interval = t;
      timer.Elapsed += (s, args) => i = 1;
      timer.Start();
      while (i == 0) { }*/
    }
  } 
}
