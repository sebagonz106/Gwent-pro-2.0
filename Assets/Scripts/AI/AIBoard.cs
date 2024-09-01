using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent_AI
{
    class AIBoard
    {
        #region Fields
        string faction = "";
        Player player;

        #region Not need for this implementation
        Dictionary<Zone, List<Card>> zones = new Dictionary<Zone, List<Card>>(3)
        {
            {Zone.Melee, new List<Card>() },
            {Zone.Range, new List<Card>() },
            {Zone.Siege, new List<Card>() }
        };
        Dictionary<Zone, List<Card>> enemyZones = new Dictionary<Zone, List<Card>>(3)
        {
            {Zone.Melee, new List<Card>() },
            {Zone.Range, new List<Card>() },
            {Zone.Siege, new List<Card>() }
        };
        List<Card> weather;
        Dictionary<Zone, Card> bonus = new Dictionary<Zone, Card>(3)
        {
            {Zone.Melee, null },
            {Zone.Range, null },
            {Zone.Siege, null }
        };
        #endregion
        #endregion

        #region Receiving info
        public AIBoard(string faction)
        {
            player = Utils.GetPlayerByName(faction);
            this.faction = faction;
        }

        public void Receive(Board board, string faction)
        {
            throw new NotImplementedException();
        }

        public void Receive(List<Card> board, string faction)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Damage values
        public double GetEnemyDamage()
        {
            return player.TotalDamage;
        }

        public double GetDamage()
        {
            return Utils.GetEnemyOf(player).TotalDamage;
        }

        public double GetDifference() => GetDamage() - GetEnemyDamage();
        #endregion

        #region Adding cards
        public bool AddNormalCard(Card card, out Zone bestZone)
        {
            bestZone = Zone.Melee;
            if (card.Type is Type.Bait || card.Type is Type.Bait) throw new System.ArgumentException();

            double biggestDifference = int.MinValue;
            foreach (Zone zone in card.AvailableRange)
            {
                NewTurn();
                if(player.PlayCard(player.Hand.IndexOf(card), GetEmptyPosition(zone), zone, out bool effectFailed) && biggestDifference <= GetDifference())
                {
                    biggestDifference = GetDifference();
                    bestZone = zone;
                }
                Undo();
            }

            return player.PlayCard(player.Hand.IndexOf(card), GetEmptyPosition(bestZone), bestZone, out bool temp);
        }

        int GetEmptyPosition(Zone zone)
        {
            List<Card> list = player.ListByZone[zone];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(Utils.BaseCard)) return i;
            }
            return -1;
        }

        public bool AddBait(BaitCard bait, Card target) => bait.Effect(player.context.UpdatePlayerInstance(target.CurrentPosition, target));

        public bool AddNormalCard(Card card, Zone bestZone) => player.PlayCard(player.Hand.IndexOf(card), GetEmptyPosition(bestZone), bestZone, out bool temp);
        #endregion

        #region Removing
        public void Remove(Card card, Zone range)
        {
            if (card is WeatherCard) weather.Remove(card);
            else if (card.Faction == faction)
            {
                if (card is UnitCard) zones[range].Remove(card);
                else if (card is BonusCard) bonus[range] = null;
            }
            else
            {
                if (card is UnitCard) zones[range].Remove(card);
                else if (card is BonusCard) bonus[range] = null;
            }
        }
        public void Undo()
        {
            Board.Instance.Undo();
            Board.Instance.RemoveLastTurn();
        }
        #endregion

        public void NewTurn() => Board.Instance.NewTurn();
    }
}
