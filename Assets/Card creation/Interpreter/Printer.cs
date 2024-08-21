using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Gwent_Interpreter
{
    class Printer
    {
        TMP_Text terminal;
        public Printer (TMP_Text terminal)
        {
            this.terminal = terminal;
            Reset();
        }
        public void Print(string message)
        {
            terminal.text += message+='\n';
        }
        public void Reset() => terminal.text = "";
    }
}
