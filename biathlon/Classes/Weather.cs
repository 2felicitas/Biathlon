using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  enum WeatherTypes { Sun, Frost, SunWind, RainWind, FrostWind, Neuter }

  /// <summary>
  /// Предоставляет сведения о погоде
  /// </summary>
  class Weather
  {
    private double[] windSpeed;
    private WeatherTypes type;

    public Weather()
    {
      double d1, d2 = 0.1;
      type = (WeatherTypes)StaticRandom.Rand(6);
      windSpeed = new double[500];
      double rand = StaticRandom.RandDouble() * 3.0;
      switch (type)
      {
        case WeatherTypes.Sun:
        case WeatherTypes.Frost:
          d1 = 10.0;
          break;
        case WeatherTypes.SunWind:
        case WeatherTypes.FrostWind:
          d1 = 4.0;
          rand = StaticRandom.RandDouble() * 2.0 + 3.0;
          break;
        case WeatherTypes.RainWind:
          d1 = 4.0;
          rand = StaticRandom.RandDouble() * 2.0 + 3.0;
          d2 = 0.2;
          break;
        case WeatherTypes.Neuter:
        default:
          d1 = 7.0;
          break;
      }
      for (int i = 0; i < windSpeed.Length; i++)
      {
        rand = rand + (StaticRandom.RandDouble() - 0.5) / d1;
        if (rand < 0)
          rand = 0;
        int j = StaticRandom.Rand(26);
        if (j == 1)
          windSpeed[i] = rand + d2 * rand;
        else
          if (j == 2)
            windSpeed[i] = rand - d2 * rand;
          else
            windSpeed[i] = rand;
      }
    }

    #region {Свойства}
    public double[] WindSpeed
    {
      get { return windSpeed; }
      set { windSpeed = value; }
    }
    public WeatherTypes Type
    {
      get { return type; }
      set { type = value; }
    }
    #endregion
  }
}
