using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Solitaire.Decks
{
	class Deck_Starter : Deck
	{
		public Deck_Starter(int x, int y, List<Deck> deckList, Imainview refInterface)
		{
			this.refInterface = refInterface;
			this.cards = new List<Card>();

			this.x = x;
			this.y = y;

			this.deckList = deckList;

			//this.deckDefaultCard = new Card(x, y, eRank.ACE, eSUIT.SPADES, cardsdll.mdFaceDown, eBACK.SPACE, this);
			this.deckDefaultCard = new Card(x, y, eRank.ACE, eSUIT.SPADES, cardsdll.mdFaceDown, eBACK.UNUSED, this, deckList, refInterface);

			updateNext();
		}

		public override bool canDealTop() { return false; }

		public override void updateNext()
		{
			nextX = x;
			nextY = y;
		}

		/*
		 * You cannot add to the starter deck
		 */
		public override bool addTo(Card toAdd)
		{
			return false;
		}

		/*
		 * You cannot take the starter deck's top
		 */
		public override bool canTakeTop()
		{
			return true;
		}

		public override void removeTop()
		{
			if(this.cards.Count() > 0)
			{
				this.cards.RemoveAt(this.cards.Count() - 1);
				updateNext();
			}
		}

		public override Card topCard()
		{
			if(this.cards.Count() > 0)
				return this.cards[this.cards.Count() - 1];

			return null;
		}

		public override void shuffle() 
		{
			//ListHelper.shuffle(this.cards);
			Random rng = new Random();
			int n = cards.Count;
			while(n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				Card value = cards[k];
				cards[k] = cards[n];
				cards[n] = value;
			}
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
	}
}
