using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  class TimeSpanCompare : IComparer<TimeSpan?>
  {
    public int Compare(TimeSpan? x, TimeSpan? y)
    {
      if (x.HasValue && y.HasValue)
        if (x.Value > y.Value)
          return 1;
        else if (x.Value < y.Value)
          return -1;
        else
          return 0;
      else if (x.HasValue)
        return 1;
      else if (y.HasValue)
        return -1;
      else
        return 0;
    }
  }
}
