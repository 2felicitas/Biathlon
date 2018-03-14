using System;
using System.Linq;
using System.Text;

namespace biathlon
{
  /// <summary>
  /// Хранит информацию о биатлонисте
  /// </summary>
  class Athlete
  {
    private int        id;                                                          // 
    private string     name;                                                        // 
    private string     surname;                                                     // 
    private DateTime   birthDate;                                                   // 
    private string     country;                                                     //
    private Attributes attributes;                                                  //
    private Equipment  skis;                                                        //
    private int        money;                                                       //

    public Athlete(int _id, string a, string b, string c, Attributes d, string e)
    {
      id = _id;
      name = a;
      surname = b;
      birthDate = new DateTime(1977, 8, 2);
      country = c;
      attributes = d;
      skis = new Equipment(e);
      money = 0;
    }

    #region{Свойства}
    public int ID
    {
      get { return id; }
    }
    public string Name
    {
      get { return name; }
    }
    public string Surname
    {
      get { return surname; }
    }
    public string FullName
    {
      get { return Name.First() + ". " + Surname; }
    }
    public DateTime BirthDate
    {
      get { return birthDate; }
    }
    public string Country
    {
      get { return country; }
    }
    public Attributes Attributes
    {
      get { return attributes; }
    }
    public Equipment Skis
    {
      get { return skis; }
    }
    public int Money
    {
      get { return money; }
    }
    public double ProneAccuracy
    {
      get { return attributes.ProneAccuracy; }
    }
    public double StandingAccuracy
    {
      get { return attributes.StandingAccuracy; }
    }
    public double AscentSpeed
    {
      get { return attributes.AscentSpeed; }
    }
    public double DescentSpeed
    {
      get { return attributes.DescentSpeed; }
    }
    public double PlainSpeed
    {
      get { return attributes.PlainSpeed; }
    }
    public int Stamina
    {
      get { return attributes.Stamina; }
    }
    public double WindResistance
    {
      get { return attributes.WindResistance; }
    }
    public double ProneSpeed
    {
      get { return attributes.ProneSpeed; }
    }
    public double StandingSpeed
    {
      get { return attributes.StandingSpeed; }
    }
    public double PronePreparation
    {
      get { return attributes.PronePreparation; }
    }
    public double StandingPreparation
    {
      get { return attributes.StandingPreparation; }
    }
    public double OverallAccuracy
    {
      get { return (ProneAccuracy + StandingAccuracy) / 2.0; }
    }
    public int CurStamina
    {
      get { return attributes.CurStamina; }
      set { attributes.CurStamina = value; }
    }
    #endregion

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(id);
      sb.Append(": ");
      sb.Append(name);
      sb.Append(' ');
      sb.Append(surname);
      return sb.ToString();
    }

    /// <summary>
    /// Ремонтирует лыжи
    /// </summary>
    public void RepairSkis()
    {
      skis.Repair();
      money -= skis.RepairCost;
    }
    /// <summary>
    /// Покупка новых лыж
    /// </summary>
    /// <param name="newSkis"></param>
    public void BuySkis(Equipment newSkis)
    {
      skis = newSkis;
      money -= newSkis.Price;
    } 
  }
}
