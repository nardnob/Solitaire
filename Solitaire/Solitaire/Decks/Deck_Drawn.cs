using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire.Decks
{
	class Deck_Drawn : Deck
	{
		public Deck_Drawn(int x, int y, List<Deck> deckList, Imainview refInterface)
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
		 * For the drawn deck, you can't manually add to it. The deck must be filled up from the main deck.
		 */
		public override bool addTo(Card toAdd)
		{
			if(toAdd.getOwner() == deckList[(int)eDeck.Deck_Main])
			{
				toAdd.x = nextX;
				toAdd.y = nextY;
				this.cards.Add(toAdd);
				toAdd.setOwner(this);
				toAdd.mode = cardsdll.mdFaceUp;
				toAdd.BringToFront();
				toAdd.Refresh();
				updateNext();
				return true;
			}

			return false;
		}

		/*
		 * For the drawn deck, you can always remove the top if there is a top to remove
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
			return null;
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
