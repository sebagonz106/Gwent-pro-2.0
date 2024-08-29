using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class Getter
{
    public static Board BoardInstance => Board.Instance; //null reference otherwise
    public static Card BaseCard => Utils.BaseCard;
}
