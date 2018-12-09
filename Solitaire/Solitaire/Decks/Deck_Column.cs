using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire.Decks
{
	class Deck_Column : Deck
	{
		public Deck_Column(int x, int y, List<Deck> deckList, Imainview refInterface)
		{
			this.refInterface = refInterface;
			this.cards = new List<Card>();

			this.x = x;
			this.y = y;

			this.deckList = deckList;

			this.deckDefaultCard = new Card(x, y, eRank.ACE, eSUIT.SPADES, cardsdll.mdFaceDown, eBACK.THE_O, this, deckList, refInterface);

			updateNext();
		}

		public override bool canDealTop() { return false; }

		public override void updateNext()
		{
			nextX = x;
			nextY = y + (cards.Count() * 20);
		}

		/*
		 * Can add to the column if it is empty,
		 * or if the top card is a different color and one more in rank
		 */
		public override bool addTo(Card toAdd)
		{
			if(cards.Count() == 0 && toAdd.getRank() == eRank.KING)
			{
				toAdd.x = nextX;
				toAdd.y = nextY;
				this.cards.Add(toAdd);
				toAdd.setOwner(this);
				toAdd.BringToFront();
				toAdd.Refresh();
				updateNext();
				return true;
			}
			else
				if(((int)toAdd.getRank() + 1 == (int)topCard().getRank()) && differentColors(toAdd, topCard()))
				{
					toAdd.x = nextX;
					toAdd.y = nextY;
					this.cards.Add(toAdd);
					toAdd.setOwner(this);
					toAdd.BringToFront();
					toAdd.Refresh();
					updateNext();
					return true;
				}
				else
				{
					return false;
				}
		}

		public bool differentColors(Card card1, Card card2)
		{
			if(card1.getSuit() == eSUIT.DIAMOND || card1.getSuit() == eSUIT.HEARTS)
				if(card2.getSuit() == eSUIT.CLUBS || card2.getSuit() == eSUIT.SPADES)
					return true;
				else
					return false;
			else
				if(card2.getSuit() == eSUIT.CLUBS || card2.getSuit() == eSUIT.SPADES)
					return false;
				else
					return true;
		}
		
		/*
		 * You can always take a column's top card if it has one
		 */
		public override bool canTakeTop()
		{
			if(cards.Count() > 0)
				return true;
			return false;
		}

		public override void removeTop()
		{
			if(cards.Count() > 0)
			{
				cards.RemoveAt(cards.Count() - 1);
				updateNext();
			}
		}

		public override Card topCard()
		{
			if(cards.Count() > 0)
				return cards[cards.Count() - 1];
			else
				return deckDefaultCard;
		}

		/*
		 * Forced add
		 */
		public override void manualAdd(Card toAdd)
		{
			toAdd.x = nextX;
			toAdd.y = nextY;
			this.cards.Add(toAdd);
			toAdd.setOwner(this);
			toAdd.BringToFront();
			toAdd.Refresh();
			updateNext();
		}

		public override void shuffle()
		{
			////ListHelper.shuffle(this.cards);
			//Random rng = new Random();
			//int n = cards.Count;
			//while(n > 1)
			//{
			//    n--;
			//    int k = rng.Next(n + 1);
			//    Card value = cards[k];
			//    cards[k] = cards[n];
			//    cards[n] = value;
			//}

		}
	}
}
