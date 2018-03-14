using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Threading;


namespace biathlon
{

  class TimeSpanCompare : IComparer<TimeSpan?>
  {
    public int Compare(TimeSpan? x, TimeSpan? y)
    {
      if (x.HasValue && y.HasValue)
        if (x.Value > y.Value)
          return 1;
        else
          if (x.Value < y.Value)
            return -1;
          else
            return 0;
      else
        if (x.HasValue)
          return 1;
        else
          if (y.HasValue)
            return -1;
          else
            return 0;
    }
  }


  public static class StaticRandom
  {
    static int seed = Environment.TickCount;
    static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
    static double prevGauss = 0;
    static bool   isStored = false;
    public static int Next()
    {
      return random.Value.Next();
    }
    public static int Next(int maxValue)
    {
      return random.Value.Next(maxValue);
    }
    public static int Next(int minValue, int maxValue)
    {
      return random.Value.Next(minValue, maxValue);
    }
    public static double NextDouble()
    {
      return random.Value.NextDouble();
    }
    public static double NextDouble(double minValue, double maxValue)
    {
      return random.Value.NextDouble() * (maxValue - minValue) + minValue;
    }
    public static double NextGaussian(double mu = 0, double sigma = 1)
    {
      double x, u, v, s;
      if (isStored)
      {
        isStored = !isStored;
        return prevGauss * sigma + mu;
      }
      do
      {
        u = 2 * StaticRandom.NextDouble() - 1;
        v = 2 * StaticRandom.NextDouble() - 1;
        s = u * u + v * v;
      } while (s >= 1 || s == 0);
      x = Math.Sqrt(-2 * Math.Log(s) / s);
      prevGauss = x * u;
      isStored = !isStored;
      return x * v * sigma + mu;
    }
    public static double NextGaussian(double mu, double sigma, double minValue, double maxValue)
    {
      double x;
      do
      {
        x = NextGaussian(mu, sigma);
      } while (x > maxValue || x < minValue);
      return x;
    }
  }
}
