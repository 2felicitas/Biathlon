using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biathlon
{
  partial class Race
  {
    public void Start()
    {
      /*Task[] a = new Task[athletes.Count];
      for (int threadID = 0; threadID < a.Length; threadID++)
      {
        Action<object> action = new Action<object>(go);
        a[threadID] = new Task(action, threadID);
        a[threadID].Start();
      }*/
      //TimerCallback Go = new TimerCallback(go);
      //System.Threading.Timer t = new System.Threading.Timer(Go, 0, 0 * 30, System.Threading.Timeout.Infinite);
      for (int i = 0; i < athletes.Count; i++)
      {
        go(i);
      }
    }
  }
}
