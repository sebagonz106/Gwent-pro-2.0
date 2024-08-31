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

        public Card Play(IContext context) => Play(context, out bool leaderEffect, 0).Item1;

        public (Card, Zone, int) Play(IContext context, out bool leaderEffect, int count = 2)
        {
            board.Receive(gameBoard, faction);
            (Card, Zone, int) value = MyPlay(context, count, out leaderEffect);
            board.Undo();
            return value;
        }

        (Card, Zone, int) MyPlay(IContext context, int count, out bool leaderEffect)
        {
            leaderEffect = false;
            Card toPlay = null;
            Zone zone = Zone.Melee;
            int position = -1;
            double difference = board.GetDamage() - board.GetEnemyDamage();

            for (int i = 0; i <= context.Hand.Count; i++)
            {
                Card card = null;
                Zone thisZone = Zone.Melee;
                int thisPosition = -1;
                bool tempLeaderEffect = false;

                if (i == context.Hand.Count && !player.LeaderEffectUsedThisRound) tempLeaderEffect = LeaderEffect(context, count, out card);
                else
                {
                    card = context.Hand[i];
                    if (card is BaitCard bait) thisPosition = PlayBait(context, count - 1, bait, out zone);
                    else
                    {
                        thisZone = board.Add(card);
                        MyPlay(context, count - 1, out bool temp);
                    }
                }

                double thisDifference = board.GetDamage() - board.GetEnemyDamage();
                if (thisDifference > difference || ((tempLeaderEffect || card != null) &&
                                                   thisDifference == difference &&
                                                   (context.Hand.Count > 4 ||
                                                   (Utils.GetEnemyOf(player).Score - player.Score >= 2))))
                {
                    difference = thisDifference;
                    toPlay = card;
                    zone = thisZone;
                    position = thisPosition;
                    leaderEffect = tempLeaderEffect;
                }
                board.Undo();
            }

            if (leaderEffect)
            {
                player.Leader.Effect(player.context.UpdatePlayerInstance(player.ListByZone[zone], toPlay));
                player.LeaderEffectUsedThisRound = true;
            }
            else if (toPlay is BaitCard bait) board.AddBait(bait, zone, position);
            else if (toPlay != null) board.Add(toPlay);

            return (toPlay, zone, position);
        }

        bool LeaderEffect(IContext context, int count, out Card card)
        {
            card = null;
            bool error = false;
            if (player.Leader.NeedsCardSelection)
            {
                foreach (var item in context.Board)
                {
                    if(item.Faction == faction)
                    {
                        if(card is null || card.Power < item.Power || (card.Power == item.Power && card is UnitCard unit && unit.Level is Level.Silver && 
                                                                                                   item is UnitCard other && other.Level is Level.Golden))
                        {
                            card = item;
                        }
                    }
                }
                error = card is null;
            }
            if (!error)
            {
                player.Leader.Effect(player.context.UpdatePlayerInstance(card.CurrentPosition, card));
                player.LeaderEffectUsedThisRound = true;
                MyPlay(context, count - 1, out bool temp);
                player.LeaderEffectUsedThisRound = false;
            }

            return !error;
        }

        int PlayBait(IContext context, int count, BaitCard bait, out Zone zone)
        {
            int position = -1;
            zone = Zone.Melee;
            double difference = board.GetDamage() - board.GetEnemyDamage();

            for (int i = 0; i < player.Battlefield.CardsInBattlefield.Count; i++)
            {
                Card card = player.Battlefield.CardsInBattlefield[i];
                if (card.Type is Type.Bait) continue;
                Zone thisZone = board.AddBait(bait, card.CurrentPosition, card.CurrentPosition.IndexOf(card));
                MyPlay(context, count-1, out bool temp);

                double thisDifference = board.GetDamage() - board.GetEnemyDamage();
                if (thisDifference>difference || thisDifference == difference &&
                                                 (context.Hand.Count > 4 ||
                                                 (Utils.GetEnemyOf(player).Score - player.Score >= 2)))
                {
                    difference = thisDifference;
                    position = card.CurrentPosition.IndexOf(card);
                    zone = thisZone;
                }
                board.Undo();
            }

            if (position != -1) board.AddBait(bait, zone, position);
            return position;
        }
    }
}
