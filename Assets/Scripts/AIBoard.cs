using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentAI
{
    class AIBoard
    {
        #region Fields
        string faction = "";

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

        #region Receiving info
        public void Receive(Board board, string faction)
        {

        }

        public void Receive(List<Card> board, string faction)
        {

        }

        public AIBoard(string faction)
        {
            this.faction = faction;
        }
        #endregion

        #region Damage values
        public double GetEnemyDamage()
        {
            return 0;
        }

        public double GetDamage()
        {
            return 0;
        }
        #endregion

        #region Adding cards
        public Zone Add(Card card)
        {
            switch (card.Type)
            {
                case Type.Unit: return Add((UnitCard)card);

                case Type.Bonus: return Add((BonusCard)card);

                case Type.Weather: return Add((WeatherCard)card);

                case Type.Clear: return Add((ClearCard)card);

                default: throw new System.NotImplementedException();
            }
        }

        Zone Add(UnitCard unit)
        {
            throw new System.NotImplementedException();
        }
        Zone Add(BonusCard bonus)
        {
            throw new System.NotImplementedException();
        }
        Zone Add(WeatherCard weather)
        {
            throw new System.NotImplementedException();
        }
        Zone Add(ClearCard clear)
        {
            throw new System.NotImplementedException();
        }
        #endregion

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
    }
}
