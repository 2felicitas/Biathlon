using System;

namespace biathlon
{
/// <summary>
  /// Хранит информацию об атрибутах биатлониста
  /// </summary>
  class Attributes
  {
    private double proneAccuracy;                                                   // %%
    private double standingAccuracy;                                                // %%
    private double ascentSpeed;                                                     // км/ч
    private double descentSpeed;                                                    // км/ч
    private double plainSpeed;                                                      // км/ч
    private int    stamina;                                                         // 
    private int    curStamina;                                                      //
    private double windResistance;                                                  // %%*100
    private double proneSpeed;                                                      // сек
    private double standingSpeed;                                                   // сек
    private double pronePreparation;                                                // сек
    private double standingPreparation;                                             // сек

    public Attributes(double pa, double sa,
                      double ass, double des, double pls,
                      int    st,
                      double wr,
                      double ps, double ss,
                      double pp, double sp)
    {
      proneAccuracy       = pa;
      standingAccuracy    = sa;                                                   
      ascentSpeed         = ass; //+ (14.6-ass)*StaticRandom.NextDouble();
      descentSpeed        = des; //+ (35-des)*StaticRandom.NextDouble();
      plainSpeed          = pls; //+ (26-pls)*StaticRandom.NextDouble();
      stamina             = st;                                                   
      curStamina          = stamina;
      windResistance      = wr;                                                   
      proneSpeed          = ps;                                                   
      standingSpeed       = ss;                                                   
      pronePreparation    = pp;                                                   
      standingPreparation = sp;                                                   
    }
    public Attributes()
    {
      proneAccuracy       = 0.90;                                                   
      standingAccuracy    = 0.90;                                                   
      ascentSpeed         = 13.0;                                                   
      descentSpeed        = 33.0;                                                   
      plainSpeed          = 23.5;                                                   
      stamina             = 90;                                                     
      curStamina          = stamina;                                                
      windResistance      = 90;                                                     
      proneSpeed          = 29;                                                     
      standingSpeed       = 25;                                                     
      pronePreparation    = 20;                                                     
      standingPreparation = 18;                                                     
    }
  
    #region{Свойства}
    public double ProneAccuracy
    {
      get { return proneAccuracy; }
    }    
    public double StandingAccuracy
    {
      get { return standingAccuracy; }
    }    
    public double AscentSpeed
    {
      get { return ascentSpeed; }
    }
    public double DescentSpeed
    {
      get { return descentSpeed; }
    }
    public double PlainSpeed
    {
      get { return plainSpeed; }
    }
    public int Stamina
    {
      get { return stamina; }
    }
    public double WindResistance
    {
      get { return windResistance; }
    }
    public double ProneSpeed
    {
      get { return proneSpeed; }
    }
    public double StandingSpeed
    {
      get { return standingSpeed; }
    }
    public double PronePreparation
    {
      get { return pronePreparation; }
    }
    public double StandingPreparation
    {
      get { return standingPreparation; }
    }
    public double OverallAccuracy
    {
      get { return (proneAccuracy + standingAccuracy) / 2.0; }
    }
    public int CurStamina
    {
      get { return curStamina; }
      set { curStamina = value; }
    }
    #endregion

    public void RestoreStamina(int days)
    {
      for (int i = 0; i < days; i++)
			{
			  curStamina += (int)Math.Round((stamina - curStamina) / 3.0 * 2.0);
        if ((stamina - curStamina) < 3)
        {
          curStamina = stamina;
          break;
        }
			}
    }

    public void calculateStamina(double tg, double kspeed, double length)
    {
      double y = 625/6.0 * Math.Pow(tg, 3) + 515/24.0 * tg + 1.15;
      y = y - (1 - kspeed) * 6.6 - (1 - kspeed) * Math.Abs(y);
      y = y * length / 0.15;
      curStamina -= (int)Math.Round(y);
      if (curStamina < 0)
        curStamina = 0;
    }

    public void Rest(double length)
    {
      curStamina += (int)Math.Round(50 * length);
      if (curStamina > stamina)
        curStamina = stamina;
    }
  }
}
