using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  partial class Race
  {
    /// <summary>
    /// Вычисляет результат стрельбы и затраченное на неё время 
    /// </summary>
    /// <param name="curLap">Текущий круг</param>
    /// <param name="curAthlete">Номер биатлониста</param>
    /// <returns>Возвращает время в формате TimeSpan</returns>
    private TimeSpan range(int curAthlete, int curLap)
    {
      int    penaltyLaps   = 0;
      double curAcc        = 0.0;
      double curPrep       = 0.0;
      double shootingSpeed = 0.0;
      double result        = 0.0;
      double windVelocity  = 0.0;
      double time = results[curAthlete].TimeStamps.Last(curLap).Value.TotalMinutes;
      if (type.Type == RaceTypes.Individual)
        time -= results[curAthlete].Range.Misses(curLap);
      if (RangeDistr[curLap] == 'p')
      {
        curAcc = athletes[curAthlete].ProneAccuracy;                                // Точность, скорость стрельбы
        curPrep = athletes[curAthlete].PronePreparation / 60.0;                     // (в секундах)
        shootingSpeed = athletes[curAthlete].ProneSpeed;                            // и время подготовки
      }                                                                             // (в минутах)
      else                                                                          // в зависимости от типа стрельбы
      {                                                                             // 
        curAcc = athletes[curAthlete].StandingAccuracy;                             //
        curPrep = athletes[curAthlete].StandingPreparation / 60.0;                  //
        shootingSpeed = athletes[curAthlete].StandingSpeed;                         //
      }
      result += curPrep;
      delay(curPrep * 1000 / 2);
      if (Type == RaceTypes.Individual || Type == RaceTypes.Sprint)                 // Вычисление силы ветра 
        time += curAthlete * 0.5;                                                   // в данный момент времени

      for (int i = 0; i < 5; i++)
      {
        windVelocity =                                                              // Сила ветра во время
          weather.WindSpeed[(int)Math.Floor(time / 100 * weather.WindSpeed.Length)];// выстрела

        double shotTime = (shootingSpeed / 5 + windVelocity * 0.5 *                 // Вычисление времени, 
                                (StaticRandom.RandDouble() - 0.25)) / 60.0;         // затраченного на выстрел

        delay(shotTime * 1000);
        time += shotTime;

        double shotAcc = curAcc - Math.Pow(windVelocity / 10.0, 2) *                // Вычисление точности стрельбы
                  (1 - athletes[curAthlete].WindResistance / 100.0);                // при данных ветровых условиях

        double a = StaticRandom.RandDouble();                                       // Вычисление попадания
        if (a < shotAcc)                                                            // и количества штрафных
          results[curAthlete].Hit(curLap, i);                                       // кругов
        else                                                                        // 
          penaltyLaps++;                                                            // 

        result += shotTime;
      }
      delay(curPrep * 1000 / 2);
      if (Type == RaceTypes.Individual)
        time = penaltyLaps;
      else
      {
        athletes[curAthlete].Attributes.calculateStamina(0, 1, 
                             penaltyLaps * penaltyLap / 1000);
        time = penaltyLaps * (penaltyLap / 1000 /
                             (athletes[curAthlete].PlainSpeed / 60.0));
        delay(time * 1000);
      }
      athletes[curAthlete].Attributes.Rest(0.2);
      result += time;
      return TimeSpan.FromMinutes(result);
    }
  }
}
