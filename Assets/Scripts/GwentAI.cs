using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentAI
{
    #region Interfaces
    interface IContext
    {
        List<Card> Hand { get; set; }
        List<Card> Board { get; set; }
        List<Card> Graveyard { get; set; }
    }
    interface IPlayer
    {
        Card Play(IContext context);
    }
    #endregion

    class GwentAI : IPlayer
    {
        string faction = "";
        AIBoard board;

        Player player => faction == "Fidel" ? Player.Fidel : Player.Batista;
        Board gameBoard => Board.Instance;

        public GwentAI(string faction)
        {
            this.faction = faction;
            board = new AIBoard(faction);
        }

        public Card Play(IContext context) => Play(context, 0).Item1;

        public (Card, Zone) Play(IContext context, int count = 0)
        {
            if (count >= 2) return (null, Zone.Melee);

            Card toPlay = null;
            Zone zone = Zone.Melee;
            board.Receive(gameBoard, faction);
            double difference = board.GetDamage() - board.GetEnemyDamage();

            foreach (var card in context.Hand)
            {
                double thisDifference = BestPlay(card, count, out Zone thisZone, out bool passed);
                if (passed) continue;

                context.Hand.Remove(card);
                context.Board.Add(card);

                if (thisDifference > difference || (!passed && thisDifference == difference && 
                                                   (context.Hand.Count>4 || 
                                                   (Utils.GetEnemyOf(player).Score - player.Score>=2))))
                {
                    difference = thisDifference;
                    toPlay = card;
                    zone = thisZone;
                }

                context.Board.Remove(card);
                context.Hand.Add(card);
            }

            return (toPlay, zone);
        }

        double BestPlay(Card card, int count, out Zone zone, out bool passed)
        {
            zone = Zone.Melee;
            passed = true;
            return int.MinValue;
        }
    }
}
