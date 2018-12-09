using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire.Decks
{
	class Deck_Ace : Deck
	{
		public Deck_Ace(int x, int y, List<Deck> deckList, Imainview refInterface)
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
			nextY = y;
		}

		/*
		 * For ace decks, you can add if the ace deck is empty and you're adding an ace,
		 * or if the top card of the ace deck is one rank below and the same suit (and you can't add a card with a follower0
		 */
		public override bool addTo(Card toAdd)
		{
			if( !toAdd._hasFollowers && ( (this.cards.Count() == 0 && toAdd.getRank() == eRank.ACE) 
				|| (this.cards.Count() != 0 && this.topCard().getRank() == toAdd.getRank() - 1 && this.topCard().getSuit() == toAdd.getSuit())) )
			{
				toAdd.cardTravelToPoint(toAdd, nextX, nextY);
				toAdd.x = nextX;
				toAdd.y = nextY;
				this.cards.Add(toAdd);
				toAdd.setOwner(this);
				toAdd.BringToFront();
				toAdd.Refresh();
				updateNext();
				return true;
			}

			return false;
		}

		/*
		 * For ace decks, you can always remove the top if there is a top to remove
		 */
		public override bool canTakeTop()
		{
			if(this.cards.Count() > 0)
				return true;
			return false;
		}

		/*
		 * To remove the top card, just pop off the back from the cards list
		 */
		public override void removeTop()
		{
			if(this.cards.Count() > 0)
			{
				this.cards.RemoveAt(this.cards.Count() - 1);
				updateNext();
			}
		}

		/*
		 * Just return the top card if there is one, else return null
		 */
		public override Card topCard()
		{
			if(this.cards.Count > 0)
				return this.cards[this.cards.Count() - 1];
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
