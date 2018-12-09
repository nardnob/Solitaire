using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Solitaire
{
	class Card : Panel
	{
		private cardsdll cardHandle;
		private IntPtr hdc = new IntPtr();
		private int rank;
		private int suit;
		public int mode;
		private eBACK backType;
		public int x;
		public int y;
		public const int width = 71;
		public const int height = 96;
		private Deck owner;
		protected bool active = false;
		protected List<Deck> deckList;
		protected bool hasFollowers = false;
		protected List<Card> followerList;
		Imainview refInterface;

		public bool _hasFollowers { get { return hasFollowers; } }

		public Card()
		{
		}

		public Card(int x, int y, eRank rank, eSUIT suit, int mode, eBACK backType, Deck owner, List<Deck> deckList, Imainview refInterface)
			: base()
		{
			this.x = x;
			this.y = y;
			this.mode = mode;
			this.rank = (int)rank;
			this.suit = (int)suit;
			this.backType = backType;
			this.cardHandle = new cardsdll(width, height);
			this.Size = new Size(width, height);
			this.owner = owner;
			this.deckList = deckList;
			this.refInterface = refInterface;

			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.handleMouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.handleMouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.handleMouseMove);
			this.DoubleClick += new EventHandler(Card_DoubleClick);
		}

		void Card_DoubleClick(object sender, EventArgs e)
		{
			for(int i = (int)eDeck.Deck_Ace1; i <= (int)eDeck.Deck_Ace4; i++)
			{
				this.getOwner().removeTop();
				if(!deckList[i].addTo(this))
				{
					this.getOwner().resend(this, this.getOwner());
				}
				else
				{
					break;
				}
			}
		}

		public bool clickIntersects(int x, int y, Card card)
		{
			if(card != null)
				if((x >= card.x && x <= card.x + Card.width) && (y >= card.y && y <= card.y + Card.height))
					return true;
			return false;
		}

		private void handleMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//if clicking on the main deck and the main deck is empty
			if(this.getOwner() == this.deckList[(int)eDeck.Deck_Main] && this.getOwner().size() == 0)
			{
				//remaining 24 cards to the main deck
				for(int i = deckList[(int)eDeck.Deck_Drawn].size(); i > 0; i--)
				{
					Card toDeal = deckList[(int)eDeck.Deck_Drawn].topCard();
					toDeal.getOwner().removeTop();
					toDeal.BringToFront();
					toDeal.mode = cardsdll.mdFaceDown;
					deckList[(int)eDeck.Deck_Main].manualAdd(toDeal);
				}
			}

			//If clicked on the main deck, and the clicked card is the top card, draw
			if(this.getOwner() == this.deckList[(int)eDeck.Deck_Main] && this == this.getOwner().topCard())
			{
				Deck lastOwner = this.getOwner();
				this.getOwner().removeTop();
				if(!this.deckList[(int)eDeck.Deck_Drawn].addTo(this))
				{
					this.getOwner().resend(this, lastOwner);
				}
			}
			else
			{
				//If clicking a deck where we can take the top card, and this card isn't upside down
				if(this.getOwner().canTakeTop() && this.mode == cardsdll.mdFaceUp)
				{
					//if the clicked card is the top card
					if(this == this.getOwner().topCard())
					{
						this.active = true;
						this.BringToFront();
						this.x = e.X + this.x - width / 2;
						this.y = e.Y + this.y - height / 2;
						this.Location = new Point(x, y);
					}
					else //This card is face up but is not the top card. Pick up a "stack"
					{
						this.active = true;
						Debug.Print("Make a stack!");
						this.followerList = this.getOwner().getFrom(this);
						this.hasFollowers = true;
						//this.BringToFront();
						for(int i = 0; i < this.followerList.Count(); i++)
						{
							followerList[i].BringToFront();
						}
						/*
						for(int i = 1; i < stack.Count(); i++)
						{
							//stack[i].following = i;
							Debug.Print(stack[i].getRank().ToString() + " of " + stack[i].getSuit().ToString());
						}
						 */
					}
				}
				else
				{
					//if you can take the top card, and you are clicking on the top card, but the top card is face down, turn it face up
					if(this.getOwner().canTakeTop() && this == this.getOwner().topCard() && this.mode == cardsdll.mdFaceDown)
					{
						this.mode = cardsdll.mdFaceUp;
						this.Refresh();
					}
				}
			}
		}

		private void handleMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.active)
			{
				int x = e.X + this.x;
				int y = e.Y + this.y;
				this.active = false;

				bool setDown = false;

				//Loop through the decks and check for a mouse-up collission with the deck
				for(int i = 0; i < deckList.Count(); i++)
				{
					if(deckList[i] != this.getOwner() && clickIntersects(x, y, deckList[i].topCard()))
					{
						//Deck lastOwner = this.getOwner();
						this.getOwner().removeTop();
						if(!deckList[i].addTo(this))
						{
							this.getOwner().resend(this, this.getOwner());;
						}
						else
						{
							setDown = true;

							if(this.hasFollowers)
							{
								for(int j = 0; j < this.followerList.Count(); j++)
								{
									deckList[i].updateNext();
									this.followerList[j].getOwner().removeTop();
									deckList[i].addTo(this.followerList[j]);
								}
							}
						}
					}
				}

				if(!setDown)
				{
					if(this.hasFollowers)
					{
						for(int j = 0; j < this.followerList.Count(); j++)
						{
							this.followerList[j].getOwner().removeTop();
							this.followerList[j].getOwner().updateNext();
						}

						for(int j = 0; j < this.followerList.Count(); j++)
						{
							this.followerList[j].getOwner().resend(this.followerList[j], this.followerList[j].getOwner());
						}

						this.hasFollowers = false;
						this.followerList = null;
					}
					else
					{
						this.getOwner().removeTop();
						this.getOwner().resend(this, this.getOwner());
					}
				}
			}
		}

		private void handleMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{//739, 584 && (this.y + e.Y > 0 && this.y + e.Y + height < 584)
			if(this.active)
			{
				this.x = e.X + this.x - width / 2;
				this.y = e.Y + this.y - height / 2;
				if(this.y < 0)
					this.y = 0;

				if(this.hasFollowers)
				{
					if(this.y + height + (this.followerList.Count() - 1) * 20 > 556)
						this.y = 556 - height - (this.followerList.Count() - 1) * 20;
				}
				else
				{
					if(this.y + height > 556)
						this.y = 556 - height;
				}


				if(this.x < 0)
					this.x = 0;
				if(this.x + width > 733)
					this.x = 733 - width;
				this.Location = new Point(this.x, this.y);
				//this.BringToFront();

				if(this.hasFollowers)
				{
					for(int i = 1; i < this.followerList.Count(); i++)
					{
						this.followerList[i].x = this.x;
						this.followerList[i].y = this.y + i * 20;
						this.followerList[i].Location = new Point(this.followerList[i].x, this.followerList[i].y);
					}
				}
				else
				{

				}
			}
			/*
			else
				if(this.following != 0)
				{
					this.x = e.X + this.x - width / 2;
					this.y = e.Y + this.y - height / 2 + following * 20;
					if(this.y < 0)
						this.y = 0;
					if(this.y + height > 556)
						this.y = 556 - height;
					if(this.x < 0)
						this.x = 0;
					if(this.x + width > 733)
						this.x = 733 - width;
					this.Location = new Point(this.x, this.y);
				}
			this.Update();
			 */
		}

		// Override Panel's OnPaint method
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics x = e.Graphics;

			hdc = x.GetHdc();
			x.ReleaseHdc(hdc);
			Draw();
		}

		// Draw card
		private void Draw()
		{
			Location = new Point(x, y);
			switch(mode)
			{
			case cardsdll.mdFaceDown:
				cardHandle.drawCardBack(hdc, 0, 0, backType);
				break;
			case cardsdll.mdFaceUp:
				cardHandle.drawExtrudedCard(hdc, 0, 0, width, height, rank * 4 + suit, mode, 0);
				break;
			}
		}

		public eRank getRank()
		{
			return (eRank)rank;
		}

		public eSUIT getSuit()
		{
			return (eSUIT)suit;
		}

		public Deck getOwner() { return this.owner; }

		public void setOwner(Deck owner) { this.owner = owner; }

		public void cardTravelToPoint(Card card, int toX, int toY)
		{
			double nowX = card.Location.X;
			double nowY = card.Location.Y;
			double mag = Math.Sqrt(Math.Pow(toX - nowX, 2) + Math.Pow(toY - nowY, 2));
			double unitVecX = (toX - nowX) / mag;
			double unitVecY = (toY - nowY) / mag;

			while(mag > 1)
			{
				nowX += unitVecX * 0.007;
				nowY += unitVecY * 0.007;
				card.Location = new Point((int)nowX, (int)nowY);
				mag = Math.Sqrt(Math.Pow(toX - nowX, 2) + Math.Pow(toY - nowY, 2));
				refInterface.refreshForm();
			}

			card.Location = new Point(toX, toY);
		}
	}
}
