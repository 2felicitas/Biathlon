using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  class Track
  {
    private Course Sprint;
    private Course Pursuit;
    private Course Individual;
    private Course Mass_Start;

    public Track(string name)
    {
      Name = name;
    }

    public string Name { get; set; }

    public void AddSp(Course sp)
    {
      Sprint = sp;
    }
    public void AddP(Course p)
    {
      Pursuit = p;
    }
    public void AddInd(Course ind)
    {
      Individual = ind;
    }
    public void AddM(Course m)
    {
      Mass_Start = m;
    }
  }

  class Course
  {
    private SortedDictionary<double,double> profile;
    public readonly double[] Sections;

    public Course(string _name, string length)
    {
      Name = _name;
      switch (length)
      {
        case "2.5":
          Sections = new double[] { 0.0, 1.1, 1.8, 2.5 };
          break;
        case "3.0":
          Sections = new double[] { 0.0, 1.2, 2.0, 3.0 };
          break;
        case "2.0":
          Sections = new double[] { 0.0, 1.2, 1.6, 2.0 };
          break;
        default:
          break;
      }
      profile = new SortedDictionary<double, double>();
    }

    #region{Свойства}
    public string Name { get; set; }
    public SortedDictionary<double, double> Profile
    {
      get { return profile; }
    }
    #endregion

    public void Add(double x, double height)
    {
      if (x > Sections.Last())
        return;
      profile.Add(x, height);
    }
  }
}
