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

        public (Card, Zone, Card) Play(IContext context, out bool leaderEffect, int count = 2)
        {
            board.NewTurn();
            (Card, Zone, Card) value = MyPlay(context, count, out leaderEffect);
            board.Undo();
            return value;
        }

        (Card, Zone, Card) MyPlay(IContext context, int count, out bool leaderEffect)
        {
            leaderEffect = false;
            Card toPlay = null;
            Zone zone = Zone.Melee;
            Card target = null;
            double difference = board.GetDamage() - board.GetEnemyDamage();

            if((difference>0 && gameBoard.GetCurrentEnemy().EndRound) || !(Utils.GetEnemyOf(player).Score - player.Score >= 2) && 
                                                                          (difference <-25||(difference<-12 && new System.Random().Next(0, 10) == 6)))
                return (toPlay, zone, target);

            else for (int i = 0; i <= context.Hand.Count; i++)
            {
                Card card = null;
                Zone thisZone = Zone.Melee;
                Card tempTarget = null;
                bool tempLeaderEffect = false;

                board.NewTurn();
                if (i == context.Hand.Count && !player.LeaderEffectUsedThisRound) tempLeaderEffect = LeaderEffect(context, count, out card);
                else if (context.Hand[i].Equals(Utils.BaseCard))
                {
                    board.Undo();
                    continue;
                }
                else
                {
                    card = context.Hand[i];
                    if (card is BaitCard bait) tempTarget =  PlayBait(context, count - 1, bait);
                    else
                    {
                        if(board.AddNormalCard(card, out Zone bestZone)) thisZone = bestZone;
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
                    target = tempTarget;
                    leaderEffect = tempLeaderEffect;
                }
                board.Undo();
            }

            if (leaderEffect) player.Leader.Effect(player.context.UpdatePlayerInstance(player.ListByZone[zone], toPlay));
            else if (toPlay is BaitCard bait) board.AddBait(bait, target);
            else if (toPlay != null) board.AddNormalCard(toPlay, out Zone temp);

            return (toPlay, zone, target);
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
                        if(card is null || card.InitialDamage < item.InitialDamage || (card.InitialDamage == item.InitialDamage && card is UnitCard unit && unit.Level is Level.Silver && 
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
                MyPlay(context, count - 1, out bool temp);
            }

            return !error;
        }

        Card PlayBait(IContext context, int count, BaitCard bait)
        {
            Card bestCard = null;
            double difference = board.GetDamage() - board.GetEnemyDamage();

            for (int i = 0; i < player.Battlefield.CardsInBattlefield.Count; i++)
            {
                Card card = player.Battlefield.CardsInBattlefield[i];
                if (card.Type is Type.Bait) continue;
                board.NewTurn();
                if(!board.AddBait(bait, card))
                {
                    board.Undo();
                    continue;
                }
                MyPlay(context, count-1, out bool temp);

                double thisDifference = board.GetDamage() - board.GetEnemyDamage();
                if (thisDifference>difference || thisDifference == difference &&
                                                 (context.Hand.Count > 4 ||
                                                 (Utils.GetEnemyOf(player).Score - player.Score >= 2)))
                {
                    difference = thisDifference;
                    bestCard = card;
                }
                board.Undo();
            }

            if (!(bestCard is null)) board.AddBait(bait, bestCard);
            return bestCard;
        }
    }
}
