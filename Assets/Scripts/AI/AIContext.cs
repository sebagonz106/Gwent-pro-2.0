using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent_AI
{
    interface IContext
    {
        List<Card> Hand { get; set; }
        List<Card> Board { get; set; }
        List<Card> Graveyard { get; set; }
    }
    class AIContext : IContext
    {
        Player player;

        public AIContext(Player player)
        {
            this.player = player;
        }

        public List<Card> Hand
        {
            get
            {
                List<Card> list = new List<Card>();
                foreach (var item in player.Hand)
                {
                    if (!item.Equals(Utils.BaseCard)) list.Add(item);
                }
                return list;
            }
            set => throw new NotImplementedException();
        }
        public List<Card> Board
        {
            get
            {
                List<Card> list = Utils.GetEnemyOf(player).Battlefield.CardsInBattlefield;
                list.AddRange(player.Battlefield.CardsInBattlefield);
                return list;
            }
            set => throw new NotImplementedException();
        }
        public List<Card> Graveyard { get => player.Battlefield.Graveyard; set => throw new NotImplementedException(); }
    }
}
