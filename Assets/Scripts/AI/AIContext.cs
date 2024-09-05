using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent_AI
{
    public interface IContext
    {
        List<ICard> Hand { get; set; }
        List<ICard> Board { get; set; }
        List<ICard> Graveyard { get; set; }
    }
    class AIContext
    {
        Player player;
        List<Card> hand;

        public AIContext(Player player)
        {
            this.player = player;
        }

        public AIContext(Player player, List<Card> hand)
        {
            this.player = player;
            this.hand = hand;
        }

        public List<Card> Hand
        {
            get
            {
                if (!(hand is null)) return hand;
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
