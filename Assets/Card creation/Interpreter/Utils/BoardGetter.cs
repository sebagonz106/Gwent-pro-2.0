using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent_Interpreter.Utils
{
    static class BoardGetter
    {
        public static Board BoardInstance => Board.Instance; //null reference otherwise
    }
}
