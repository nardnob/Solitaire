using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Solitaire
{
	/*
	 * Abstract base Deck class
	 * Useful for holding all decks in a list and using polymorphism to access them
	 * 
	 */
	abstract class Deck : Panel
	{
		protected List<Card> cards;
		public Card deckDefaultCard;
		protected List<Deck> deckList;
		protected int nextX, nextY;
		protected int x, y;
		public Imainview refInterface;

		public abstract bool addTo(Card toAdd);
		public abstract void manualAdd(Card toAdd);
		public abstract bool canTakeTop();
		public abstract void removeTop();
		public abstract Card topCard();
		public abstract void shuffle();
		public abstract void updateNext();
		public abstract bool canDealTop();

		public int getX() { return x; }
		public int getY() { return y; }
		public int getNextX() { return nextX; }
		public int getNextY() { return nextY; }
		public int size() { return cards.Count(); }

		public void resend(Card card, Deck owner)
		{
			card.BringToFront();
			card.Update();
			card.x = owner.nextX;
			card.y = owner.nextY;
			owner.cards.Add(card);
			owner.updateNext();
			card.cardTravelToPoint(card, card.x, card.y);
		}

		public void printList()
		{
			for(int i = 0; i < this.cards.Count(); i++)
			{
				Debug.Print(this.cards[i].getRank().ToString() + " of " + this.cards[i].getSuit().ToString());
			}
		}

		public List<Card> getFrom(Card fromCard)
		{	
			int index = cards.IndexOf(fromCard);
			return cards.GetRange(index, cards.Count() - index);
		}
	}
}
