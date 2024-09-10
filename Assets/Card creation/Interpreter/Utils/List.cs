using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Gwent_Interpreter.Utils
{
    public class GwentList : IList<Card>
    {
        List<Card> list;
        Board board;
        Player player;

        public int Count
        {
            get
            {
                int count = list.Count;
                foreach (var item in list)
                    if (item.Equals(Getter.BaseCard)) count--;

                return count;
            }
        }

        public bool IsReadOnly => false;

        Card IList<Card>.this[int index] { get => this[index]; set => this[index] = value; }

        public GwentList(List<Card> list, Player player = null)
        {
            this.list = list;
            board = Board.Instance;
            this.player = player;
        }
        public GwentList()
        {
            list = new List<Card>();
            board = Board.Instance;
        }

        public Card this[Num index]
        {
            get => this[Convert.ToInt32(index.Value)];
            set => this[Convert.ToInt32(index.Value)] = value;
        }
        public Card this[int index]
        {
            get => list[index];
            set
            {
                bool deckOrGraveyard = (player is null ? false : list.Equals(player.Deck) || list.Equals(player.Battlefield.Graveyard));
                board.Receive(new AddOperation(value, list, index, deckOrGraveyard));
                board.Receive(new RemoveOperation(list[index], list, index, deckOrGraveyard));

                list[index] = value;
            }
        }

        public void Remove(Card card) => card.Owner.Battlefield.ToGraveyard(card);
        public GwentList Find(Predicate<Card> predicate) => new GwentList(list.FindAll(predicate), player);
        public void Push(Card card)
        {
            if (!(player is null) && (list.Equals(player.Deck) || list.Equals(player.Battlefield.Graveyard)))
            {
                list.Add(card);
                board.Receive(new AddOperation(card, list, list.Count-1, true));
            }
            else Insert(list.Count - 1, card);
        }
        public Card Pop()
        {
            Card card = null;

            if ( !(player is null) && (list.Equals(player.Deck) || list.Equals(player.Battlefield.Graveyard)) )
            {
                card = list[list.Count - 1];
                board.Receive(new RemoveOperation(card, list, list.Count - 1, true));
                list.RemoveAt(list.Count - 1);
            }
            else for (int i = list.Count-1; i >= 0; i--)
            {
                card = list[i];
                if (card.Equals(Getter.BaseCard)) continue;
                else
                {
                    list[i] = Getter.BaseCard;
                    board.Receive(new RemoveOperation(card, list, i, false));
                    break;
                }
            }

            return card;
        }
        public void Shuffle() => Getter.Shuffle(this.list);
        public void SendBottom(Card card) => Insert(0, card);

        public int IndexOf(Card item) => list.IndexOf(item);

        public void Insert(int index, Card item)
        {
            if (item.Equals(Getter.BaseCard)) return;

            if (!(player is null) && (list.Equals(player.Deck) || list.Equals(player.Battlefield.Graveyard)))
            {
                list.Insert(index, item);
                board.Receive(new AddOperation(item, list, index, true));
            }
            else
            {
                if (list[index].Equals(Getter.BaseCard)) list[index] = item;
                else
                {
                    Card temp = list[index];
                    list[index] = item;
                    board.Receive(new RemoveOperation(temp, list, index));
                    MyAdd(temp, index + 1);
                }

                board.Receive(new AddOperation(item, list, index));
                if (list.Equals(item.Owner.Hand)) item.Owner.UpdateEmptySlots();
            }
        }

        public void RemoveAt(int index) => Remove(list[index]);

        public void Insert(Num num, Card card) => Insert(Convert.ToInt32(num.Value), card);

        public void RemoveAt(Num num) => RemoveAt(Convert.ToInt32(num.Value));

        public void Add(Card item)
        {
            if (item.Equals(Getter.BaseCard)) return;
            else if (!(player is null))
            {
                if (player.Hand.Equals(list)) player.AddToHand(item);
                else if (player.Deck.Equals(list) || player.Battlefield.Graveyard.Equals(list))
                {
                    board.Receive(new AddOperation(item, list, list.Count, true));
                    list.Add(item);
                }
            }
            else MyAdd(item);
        }

        void MyAdd(Card item, int startIndex=0)
        {
            for (int i = startIndex; i - startIndex < list.Count; i++)
            {
                if (list[i % list.Count].Equals(Getter.BaseCard))
                {
                    list[i % list.Count] = item;
                    board.Receive(new AddOperation(item, list, i % list.Count));
                    break;
                }
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

        public IEnumerator<Card> GetEnumerator() => list.Where((Card card) => !card.Equals(Getter.BaseCard)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
