using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  partial class Race
  {
    /// <summary>
    /// Вычисляет время прохождения отсечки k биатлонистом под номером curAthlete
    /// </summary>
    /// <param name="curSection">Текущая отсечка</param>
    /// <param name="curAthlete">Номер биатлониста</param>
    /// <returns>Возвращает время в формате TimeSpan</returns>
    private TimeSpan sectionPassTime(int curAthlete, int lap, int k)
    {
      double   curSection = course.Sections[k];
      double   prevSection = course.Sections[k - 1];
      int      j = 1;
      int      stamina;
      int      gStamina = athletes[curAthlete].Stamina;
      double   result = 0.0;
      double[] points = new double[course.Profile.Count];

      course.Profile.Keys.CopyTo(points, 0);
      while (points[j] <= prevSection)
        j++;
      do
      {
        stamina = athletes[curAthlete].CurStamina;
        double speed = 0.0;
        double height = course.Profile[points[j]] - course.Profile[points[j - 1]];  // Вычисление коэффициента
        double tg = height / (points[j] - points[j - 1]);                           // наклона участка
        double length = Math.Min(points[j], curSection) -
                 Math.Max(points[j - 1], prevSection);

        double rand = StaticRandom.RandGaussian(stamina / (double)gStamina * 0.12 +
                                           0.94 + (lap - Laps / 2.0) / 50.0, 0.02);
        speed = rand * calcSpeed(athletes[curAthlete], tg) / 60;
        var time = results[curAthlete].TimeStamps[lap, k-1].Value.TotalMinutes;

        if (stamina < 5)
          speed -= speed / 10.0;
        if (weather.Type.ToString().Contains("Frost"))
          if (Type == RaceTypes.Individual || Type == RaceTypes.Sprint)
            speed += (speed / 20.0) * (time + curAthlete * 0.5) / 100;
          else
            speed += (speed / 20.0) * time / 100;
        else if (weather.Type.ToString().Contains("Sun"))
          if (Type == RaceTypes.Individual ||  Type == RaceTypes.Sprint)
            speed -= (speed / 25.0) * (time + curAthlete * 0.5) / 100;
          else
            speed -= (speed / 25.0) * time / 100;
        else if (weather.Type.ToString().Contains("Rain"))
          speed -= speed / 15.0;
        speed *= StaticRandom.RandDouble(1, athletes[curAthlete].Skis.Quality / 100.0 + 1);

        athletes[curAthlete].Attributes.calculateStamina(tg, rand, length);

        /*                    
        if (tg < -0.02)                                                             //
        {                                                                           // 
          speed = StaticRandom.NextDouble(0.99, 1.01) *                             //
                  athletes[curAthlete].Attributes.DescentSpeed / 60.0;              // Вычисление скорости в км/мин
          if ((2 * rand < stamina) && (stamina > 30))                               //
          {
            //speedup = '+';//
            rand = StaticRandom.NextDouble();
            speed += rand * speed / 20.0;                                           // Ускорение с потерей
            athletes[curAthlete].Attributes.Tired(rand * (-1 / tg) / 200, 0.1);     // выносливости
          }                                                                         //
          else                                                                      //
            athletes[curAthlete].Attributes.Rest(length);                           // Восстановление выносливости
          if (stamina < 5)                                                          //
            speed -= speed / 10.0;                                                  // При низкой выносливости -
        }                                                                           // снижение скорости
        else                                                                        //
          if (tg > 0.02)                                                            //
          {                                                                         //
            speed = StaticRandom.NextDouble(0.99, 1.01) *                           //
                    athletes[curAthlete].Attributes.AscentSpeed / 60.0;             // Вычисление скорости в км/мин
            if (2 * rand < stamina)                                                 //
            {
              //speedup = '+';//
              rand = StaticRandom.NextDouble();
              speed += rand*speed / 20.0;                                           // Ускорение с потерей
              athletes[curAthlete].Attributes.Tired(rand * 2 * tg, length);         // выносливости
            }                                                                       //
            else                                                                    // Стандартная потеря
              athletes[curAthlete].Attributes.Tired(tg, length);                    //    выносливости при подъеме
            if (stamina < 5)                                                        // При низкой выносливости -
              speed -= speed / 10.0;                                                //    снижение скорости
          }                                                                         //
          else                                                                      //
          {                                                                         //
            speed = StaticRandom.NextDouble(0.99, 1.01) *                           //
                    athletes[curAthlete].Attributes.PlainSpeed / 60.0;              // Вычисление скорости в км/мин
            if (2 * rand < stamina)                                                 //
            {
              //speedup = '+';//
              rand = StaticRandom.NextDouble(); 
              speed += rand * speed / 20.0;                                         // Ускорение с потерей
              athletes[curAthlete].Attributes.Tired(rand * tg, length);             // выносливости
            }                                                                       //
            if (stamina < 5)                                                        // При низкой выносливости -
              speed -= speed / 10.0;                                                // снижение скорости
          }                                                                         //*/

        result += length / speed;                                                   // Вычисление времени
                                                                                    // прохождения участка
      } while (points[j++] < curSection);
      delay(result * 1000);
      staminas.stamina[curAthlete, lap * (course.Sections.Length - 1) + k - 1] = athletes[curAthlete].CurStamina;
      return TimeSpan.FromMinutes(result);
    }

    private double calcSpeed(Athlete a, double tg)
    {
      if (tg <= -0.1)
        return a.DescentSpeed * (1 + (-0.1 - tg));
      else if (tg > -0.1 && tg <= 0)
        return a.DescentSpeed * (-10) * tg + a.PlainSpeed * (10 * tg + 1);
      else if (tg > 0 && tg < 0.1)
        return a.PlainSpeed * (1 - 10 * tg) + a.AscentSpeed * 10 * tg;
      else
        return a.AscentSpeed * (1 + (0.1 - tg));
    }
  }
}
