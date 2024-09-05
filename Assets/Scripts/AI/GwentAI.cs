using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gwent_AI
{
    #region Interface
    interface IPlayer
    {
        ICard Play(IContext context);
    }
    #endregion

    class GwentAI
    {
        string faction = "";
        AIBoard board;

        Player player => faction == "Fidel" ? Player.Fidel : Player.Batista;
        Board gameBoard => Board.Instance;
        bool IsPlaying => faction == "Fidel" ? !gameBoard.IsBatistaPlayingOrAboutToPlay : gameBoard.IsBatistaPlayingOrAboutToPlay;

        public GwentAI(string faction)
        {
            this.faction = faction;
            board = new AIBoard(faction);
        }

        public bool DirectPlay(AIContext context, out string info, int level = 0)
        {
            info = faction + " se pasó y no jugará más.";

            (Card, Zone, Card) value = level>0? HardPlay(context, 2, out bool leaderEffect) :
                                       level<0? EasyPlay(context, out leaderEffect) :
                                                NormalPlay(context, 2, out leaderEffect);

            if (leaderEffect) info = faction + " usó el efecto de su líder" + (value.Item3 is null? "." : ("sobre la carta '" + value.Item3.Name + "'."));
            else if (value.Item1 is BaitCard) info = faction + " usó el señuelo '" + value.Item1.Name + "' para recuperar la carta '" + value.Item3.Name + "'.";
            else if (value.Item1 is Card) info = faction + " jugó la carta '" + value.Item1.Name + "' en la zona de " + (value.Item2 is Zone.Melee ? "cuerpo a cuerpo." : (value.Item2 is Zone.Range ? "rango." : "asedio."));
            else return false;

            return true;
        }

        public (Card, Zone, Card) Play(AIContext context, out bool leaderEffect, int count = 2)
        {
            board.NewTurn();
            (Card, Zone, Card) value = NormalPlay(context, count, out leaderEffect);
            board.Undo();
            return value;
        }

        (Card, Zone, Card) EasyPlay(AIContext context, out bool leaderEffect) => NormalPlay(context, 0, out leaderEffect);

        #region Normal play
        (Card, Zone, Card) NormalPlay(AIContext context, int count, out bool leaderEffect)
        {
            leaderEffect = false;
            Card toPlay = null;
            Zone zone = Zone.Melee;
            Card target = null;
            double difference = board.GetDifference();

            if (!(UseLeader(count, difference, context, ref target, ref leaderEffect) || Pass(count, difference, context)))
            {
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    Card card = null;
                    Zone thisZone = Zone.Melee;
                    Card tempTarget = null;
                    bool tempLeaderEffect = false;

                    board.NewTurn();
                    if (i == context.Hand.Count)
                    {
                        if (!player.LeaderEffectUsedThisRound && context.Hand.Count < 10)
                            tempLeaderEffect = LeaderEffect(context, count, out tempTarget);
                    }
                    else if (context.Hand[i].Equals(Utils.BaseCard))
                    {
                        board.Undo();
                        continue;
                    }
                    else
                    {
                        card = context.Hand[i];
                        if (card is BaitCard bait) tempTarget = PlayBait(context, count - 1, bait);
                        else
                        {
                            if (board.AddNormalCard(card, out Zone bestZone)) thisZone = bestZone;
                            NormalPlay(context, count - 1, out bool temp);
                        }
                    }

                    if(Utils.GetEnemyOf(player).Battlefield.CardsInBattlefield.Count == 0 && card is UnitCard unit && unit.Level is Level.Golden)
                    {
                        board.Undo();
                        continue;
                    }
                    double thisDifference = board.GetDifference();
                    if (((Losing  || difference>-6) && thisDifference > difference) || ((tempLeaderEffect || card != null) &&
                                                                                        thisDifference == difference &&
                                                                                        (context.Hand.Count > 4 ||
                                                                                        Losing)))
                    {
                        difference = thisDifference;
                        toPlay = card;
                        zone = thisZone;
                        target = tempTarget;
                        leaderEffect = tempLeaderEffect;
                    }
                    board.Undo();
                }
            }

            if (toPlay is null && !player.LeaderEffectUsedThisRound)
            {
                board.NewTurn();
                double thisDifference = board.GetDifference();
                leaderEffect = LeaderEffect(context, 0, out target) && (thisDifference >= difference || thisDifference > 0);
                target = leaderEffect ? target : null;
                board.Undo();
            }

            if (leaderEffect) player.Leader.Effect(player.Leader.NeedsCardSelection ? player.context.UpdatePlayerInstance(target.CurrentPosition, target) : player.context);
            else if (toPlay is BaitCard bait) board.AddBait(bait, target);
            else if (toPlay != null) board.AddNormalCard(toPlay, zone);

            return (toPlay, zone, target);
        }

        bool LeaderEffect(AIContext context, int count, out Card card)
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
                player.Leader.Effect(player.Leader.NeedsCardSelection? player.context.UpdatePlayerInstance(card.CurrentPosition, card) : player.context);
                NormalPlay(context, count - 1, out bool temp);
            }

            return !error;
        }

        Card PlayBait(AIContext context, int count, BaitCard bait)
        {
            Card bestCard = null;
            double difference = board.GetDifference();

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
                NormalPlay(context, count-1, out bool temp);

                double thisDifference = board.GetDifference();
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
        #endregion

        #region Hard play
        (Card, Zone, Card) HardPlay(AIContext context, int count, out bool leaderEffect)
        {
            leaderEffect = false;
            Card toPlay = null;
            Zone zone = Zone.Melee;
            Card target = null;
            double difference = board.GetDifference();

            Player enemy = Utils.GetEnemyOf(player);
            List<Card> enemyHandSave = new List<Card>();
            foreach (var item in enemy.Hand) enemyHandSave.Add(item);
            enemy.Hand = GetRemainigCards(enemy);
            GwentAI enemyAI = new GwentAI(enemy.Name);
            AIContext enemyContext = new AIContext(enemy);

            if(!(UseLeader(count, difference, context, ref target, ref leaderEffect) || Pass(count, difference, context)))
            {
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    Card card = null;
                    Zone thisZone = Zone.Melee;
                    Card tempTarget = null;
                    bool tempLeaderEffect = false;

                    board.NewTurn();
                    if (i == context.Hand.Count)
                    {
                        if (!player.LeaderEffectUsedThisRound && context.Hand.Count < 10)
                            tempLeaderEffect = LeaderEffect(context, 0, out tempTarget);
                    }
                    else if (context.Hand[i].Equals(Utils.BaseCard))
                    {
                        board.Undo();
                        continue;
                    }
                    else
                    {
                        card = context.Hand[i];
                        if (card is BaitCard bait) tempTarget = PlayBait(context, 0, bait);
                        else if (board.AddNormalCard(card, out Zone bestZone)) thisZone = bestZone;
                    }

                    SimulatePlay(enemyAI, enemyContext, this, context, 2);
                    
                    if(Utils.GetEnemyOf(player).Battlefield.CardsInBattlefield.Count <=3 && card is UnitCard unit && unit.Level is Level.Golden)
                    {
                        board.Undo();
                        continue;
                    }
                    double thisDifference = board.GetDifference();
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
            }

            if (leaderEffect) player.Leader.Effect(player.Leader.NeedsCardSelection ? player.context.UpdatePlayerInstance(target.CurrentPosition, target) : player.context);
            else if (toPlay is BaitCard bait) board.AddBait(bait, target);
            else if (toPlay != null) board.AddNormalCard(toPlay, zone);
            enemy.Hand = enemyHandSave;
            return (toPlay, zone, target);
        }

        List<Card> GetRemainigCards(Player player)
        {
            List<Card> list = CardsWarehouse.GetDeck(player.Name);
            foreach (var item in player.Battlefield.CardsInBattlefield)
            {
                list.Remove(item);
            }
            foreach (var item in player.Battlefield.Graveyard)
            {
                list.Remove(item);
            }
            return list;
        }

        void SimulatePlay(GwentAI first, AIContext firstContext, GwentAI last, AIContext lastContext, int count)
        {
            for (int i = 0; i <= count; i++)
            {
                first.NormalPlay(firstContext, 1, out bool leaderEffect);
                last.NormalPlay(lastContext, 1, out leaderEffect);
            }
        }
        #endregion

        #region Utils
        bool UseLeader(int count, double difference, AIContext context, ref Card target, ref bool leaderEffect)
        {
            if (count >= 0 && difference > 0 && gameBoard.GetCurrentEnemy().EndRound && !player.LeaderEffectUsedThisRound)
            {
                board.NewTurn();
                leaderEffect = LeaderEffect(context, 0, out target) && difference > 0;
                target = leaderEffect ? target : null;
                board.Undo();
                return true;
            }
            else return false;
        }
        bool Pass(int count, double difference, AIContext context) => (count < 0
                                                                   || (Board.Instance.RoundCount == 1 && context.Hand.Count<5)
                                                                   || (difference > 0 && gameBoard.GetCurrentEnemy().EndRound)
                                                                   || (!Losing && (difference < -25 ||
                                                                                  (difference < -12 && new System.Random().Next(0, 10) == 6) ||
                                                                                  (difference < -5 && context.Hand.Count <= 4)))
                                                                   || (difference > 12 && CountListWithEmptyCards(Utils.GetEnemyOf(player).Hand)<5));
        bool Losing => (Utils.GetEnemyOf(player).Score - player.Score >= 2);

        int CountListWithEmptyCards(List<Card> list)
        {
            int count = 0;
            foreach (var item in list) if (!item.Equals(Utils.BaseCard)) count++;
            return count;
        }
        #endregion
    }
}
