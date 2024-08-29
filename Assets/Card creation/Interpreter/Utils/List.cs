using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gwent_Interpreter.Utils
{
    public class GwentList : IList<Card>
    {
        List<Card> list;
        Board board;
        Player player;

        public int Count => list.Count;

        public bool IsReadOnly => false;

        Card IList<Card>.this[int index] { get => list[index]; set => list[index] = value; }

        public GwentList(List<Card> list, Player player = null) //fix so that EmptyCards are omited
        {
            this.list = list;
            if (player is null) board = Board.Instance;
            else this.player = player;
        }
        public GwentList()
        {
            list = new List<Card>();
            board = Board.Instance;
        }

        public Card this[Num index]
        {
            get => list[Convert.ToInt32(index.Value)];
            set => list[Convert.ToInt32(index.Value)] = value;
        }
        public Card this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public void Remove(Card card)
        {
            if (player is null) player = card.FactionEnum == Faction.Fidel ? Player.Fidel : Player.Batista;

            Debug.Log(card.Name);
            player.Battlefield.ToGraveyard(card);
        }
        public GwentList Find(Predicate<Card> predicate) => new GwentList(list.FindAll(predicate), player);
        public void Push(Card card) => Insert(list.Count-1, card);
        public Card Pop()
        {
            Card card = null;
            for (int i = list.Count-1; i >= 0; i--)
            {
                card = list[i];
                if (card.Equals(Getter.BaseCard)) continue;
                else
                {
                    list[i] = Getter.BaseCard;
                    break;
                }
            }
            Debug.Log(card.Name);
            return card;
        }
        public void Shuffe()
        {
            int randomNumber;
            Card swapCard;

            Debug.Log("sb");

            for (int i = list.Count - 1; i >= 0; i--)
            {
                randomNumber = (new System.Random()).Next(list.Count - 1);
                swapCard = list[randomNumber];
                list[randomNumber] = list[i];
                list[i] = swapCard;
                Debug.Log(swapCard.Name);
            }
            Debug.Log("se");
        }
        public void SendBottom(Card card) => Insert(0, card);

        public int IndexOf(Card item) => list.IndexOf(item);

        public void Insert(int index, Card item)
        {
            if (item.Equals(Getter.BaseCard)) return;
            Debug.Log(item.Name);
            Debug.Log(index);
            if (list[index].Name == "Empty") list[index] = item;
            else
            {
                Card temp = list[index];
                list[index] = item;
                MyAdd(temp, index+1);
            }

            if (player is null) player = list[index].FactionEnum == Faction.Fidel ? Player.Fidel : Player.Batista;
            if (list.Equals(player.Hand)) player.UpdateEmptySlots();
        }

        public void RemoveAt(int index) => Remove(list[index]);

        public void Insert(Num num, Card card) => Insert(Convert.ToInt32(num.Value), card);

        public void RemoveAt(Num num) => RemoveAt(Convert.ToInt32(num.Value));

        public void Add(Card item)
        {
            if (item.Equals(Getter.BaseCard)) return;
            else if (!(player is null) && player.Hand.Equals(list)) player.AddToHand(item);
            else MyAdd(item);
        }

        void MyAdd(Card item, int startIndex=0)
        {
            Debug.Log(item.Name);
            for (int i = startIndex; i - startIndex < list.Count; i++)
            {
                Debug.Log(i % list.Count);
                if (list[i % list.Count].Name == "Empty") { list[i % list.Count] = item; break; }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Remove(list[i]);
            }
        }

        public bool Contains(Card item) => list.Contains(item);

        public void CopyTo(Card[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        bool ICollection<Card>.Remove(Card item)
        {
            if (this.Contains(item)) { this.Remove(item); return true; }
            else return false;
        }

        public IEnumerator<Card> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
