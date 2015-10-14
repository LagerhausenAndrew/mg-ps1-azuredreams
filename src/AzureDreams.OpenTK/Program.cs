using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams.OpenGL
{
  class Program
  {
    [STAThread]
    static void Main()
    {
      using (var game = new AzureDreamsGameWindow())
      {
        game.Run();
      }
    }
  }
}
