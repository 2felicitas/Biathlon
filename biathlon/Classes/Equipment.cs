using System;
using System.Linq;

namespace biathlon
{
  class Equipment
  {
    private string name;
    private int    quality;
    private int    capacity;
    private int    price;
    private int    repairCost;

    public Equipment(string a)
    {
      name = a;
      quality = int.Parse(a.Last().ToString());
      capacity = 100;
      price = 0;
      repairCost = 0;
    }

    #region {Свойства}
    public string Name
    {
      get { return name; }
    }
    public int Quality
    {
      get { return quality; }
    }

    public int Capacity
    {
      get { return capacity; }
    }
    public int Price
    {
      get { return price; }
    }
    public int RepairCost
    {
      get { return repairCost; }
    }
    #endregion


    public void Repair()
    {
      if (capacity > 80)
        capacity = 100;
      else
        capacity += 20;
    }
  }
}
