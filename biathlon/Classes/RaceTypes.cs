using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  enum RaceTypes { Sprint, Pursuit, Individual, Mass_Start, Relay }

  class RaceType
  {
    private RaceTypes type;
    private int       laps;
    private string    rangeDistr;

    public RaceType(RaceTypes _type)
    {
      type = _type;
      switch (_type)
      {
        case RaceTypes.Sprint:
          laps = 3;
          rangeDistr = "ps";
          break;
        case RaceTypes.Pursuit:
        case RaceTypes.Mass_Start:
          laps = 5;
          rangeDistr = "ppss";
          break;
        case RaceTypes.Individual:
          laps = 5;
          rangeDistr = "psps";
          break;
        default:
          break;
      }
    }

    #region{Свойства}
    public RaceTypes Type
    {
      get { return type; }
      set { type = value; }
    }
    public int Laps
    {
      get { return laps; }
      set { laps = value; }
    }
    public string RangeDistr
    {
      get { return rangeDistr; }
      set { rangeDistr = value; }
    }
    #endregion

  }
}
