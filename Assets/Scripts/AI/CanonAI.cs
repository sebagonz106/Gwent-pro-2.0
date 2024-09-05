using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent_AI
{
    class CanonAI : IPlayer
    {
        string player;
        ICard leader;

        public CanonAI (string playerName, ICard leader)
        {
            player = playerName;
            this.leader = leader;
        }
        public ICard Play(IContext context) => MyPlay(context, out double dif);

        public ICard MyPlay(IContext context, out double difference, int count = 3)
        {
            difference = Difference(context.Board);
            ICard card = null;

            if (count <= 0 || context.Hand.Count == 0 || context.Hand.Count <= 5 && Difference(context.Board)<-10) return null;

            List<ICard> boardSave = new List<ICard>();
            foreach (var item in context.Board) boardSave.Add(item);

            for (int i = 0; i <= context.Hand.Count; i++)
            {
                ICard tempCard = null;
                double maxDifference = int.MinValue;

                if (i == context.Hand.Count)
                {
                    tempCard = leader;
                    leader.Effect(context);
                    MyPlay(context, out maxDifference, count - 1);
                }
                else if (context.Hand[i].CardType == "Señuelo") continue;
                else
                {
                    tempCard = context.Hand[i];
                    context.Hand.RemoveAt(i);
                    context.Board.Add(tempCard);
                    tempCard.Effect(context);
                    MyPlay(context, out maxDifference, count - 1);
                    context.Hand.Insert(i, tempCard);
                }

                if(maxDifference > difference)
                {
                    card = tempCard;
                    difference = maxDifference;
                }

                context.Board = boardSave;
            }

            return card;
        }

        #region Utils
        double Difference(List<ICard> list)
        {
            Dictionary<string, double> playerBonus = new Dictionary<string, double>() { { "Melee" , 1}, { "Range", 1 }, { "Siege", 1 } };
            Dictionary<string, double> enemyBonus = new Dictionary<string, double>() { { "Melee", 1 }, { "Range", 1 }, { "Siege", 1 } };
            Dictionary<string, double> weathers = new Dictionary<string, double>() { { "Melee", 0 }, { "Range", 0 }, { "Siege", 0 } };
            List<ICard> player = new List<ICard>();
            List<ICard> enemy = new List<ICard>();

            foreach (var item in list)
            {
                if (item.CardType == "Clima") ApplyEffect(weathers, item, (double a, double b) => a+b);
                else if (item.CardType == "Despeje") ApplyEffect(weathers, item, (double a, double b) => a - b);
                else if (item.OwnerName == this.player) AddToList(playerBonus, player, item);
                else AddToList(enemyBonus, enemy, item);
            }

            return GetPower(player, playerBonus, weathers) - GetPower(enemy, enemyBonus, weathers);
        }

        private static void ApplyEffect(Dictionary<string, double> dictionary, ICard item, Func<double, double, double> func)
        {
            foreach (var zone in item.Zones)
            {
                dictionary[zone] = func(item.Power, dictionary[zone]);
                if (dictionary[zone] < 0) dictionary[zone] = 0;
            }
        }

        private static void AddToList(Dictionary<string, double> dictionary, List<ICard> list, ICard item)
        {
            if (item.CardType == "Aumento") ApplyEffect(dictionary, item, (double a, double b) => a * b);
            else list.Add(item);
        }

        double GetPower(List<ICard> cards, Dictionary<string, double> bonus, Dictionary<string, double> weathers)
        {
            double power = 0;

            foreach (var card in cards)
            {
                if (card.CardType == "Unidad") power += (card.Power - weathers[card.Zones[0]]) * bonus[card.Zones[0]];
                if (power < 0) power = 0;
            }

            return power;
        }
        #endregion
    }
}
