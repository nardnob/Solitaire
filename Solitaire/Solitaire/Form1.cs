using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Solitaire.Decks;
using System.Diagnostics;

namespace Solitaire
{
	public enum eDeck
	{
		Deck_Starter,
		Deck_Main,
		Deck_Drawn,
		Deck_Ace1,
		Deck_Ace2,
		Deck_Ace3,
		Deck_Ace4,
		Deck_Column1,
		Deck_Column2,
		Deck_Column3,
		Deck_Column4,
		Deck_Column5,
		Deck_Column6,
		Deck_Column7
	}

	public partial class Form1 : Form, Imainview
	{
		public const int cardSpacingX = 91;
		public const int cardSpacingY = 116;
		private eBACK mainBackType = eBACK.SPACE;
		private List<Deck> deckList = new List<Deck>();

		public void refreshForm()
		{
			this.Update();
		}

		public Form1()
		{
			Imainview refInterface = this;

			InitializeComponent();

			#region " Initializing the Decks "

			deckList.Add(new Deck_Starter(this.Size.Width / 2 - Card.width / 2, this.Size.Height / 2 - Card.height / 2, deckList, refInterface));
			deckList.Add(new Deck_Main(30, 30, deckList, refInterface));
			deckList.Add(new Deck_Drawn(deckList[1].getX() + cardSpacingX, deckList[1].getY(), deckList, refInterface));
			deckList.Add(new Deck_Ace((int)(deckList[1].getX() + cardSpacingX * 3.6), deckList[1].getY(), deckList, refInterface));
			deckList.Add(new Deck_Ace((int)(deckList[1].getX() + cardSpacingX * 4.6), deckList[1].getY(), deckList, refInterface));
			deckList.Add(new Deck_Ace((int)(deckList[1].getX() + cardSpacingX * 5.6), deckList[1].getY(), deckList, refInterface));
			deckList.Add(new Deck_Ace((int)(deckList[1].getX() + cardSpacingX * 6.6), deckList[1].getY(), deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 0.0), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 1.1), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 2.2), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 3.3), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 4.4), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 5.5), deckList[1].getY() + cardSpacingY, deckList, refInterface));
			deckList.Add(new Deck_Column((int)(deckList[1].getX() + cardSpacingX * 6.6), deckList[1].getY() + cardSpacingY, deckList, refInterface));

			//Add each deck's default cards to the form
			for(int i = 0; i < deckList.Count(); i++)
			{
				this.Controls.Add(deckList[i].deckDefaultCard);
			}

			//special case to send the starter deck's default card to the back (transparent)
			deckList[(int)eDeck.Deck_Starter].deckDefaultCard.SendToBack();

			//Add cards to the starter deck and shuffle them
			for(int i = (int)eRank.ACE; i <= (int)eRank.KING; i++)
			{
				for(int j = (int)eSUIT.CLUBS; j <= (int)eSUIT.SPADES; j++)
				{
					Card newCard = new Card(0, 0, (eRank)i, (eSUIT)j, cardsdll.mdFaceUp, mainBackType, deckList[(int)eDeck.Deck_Starter], deckList, refInterface);
					deckList[(int)eDeck.Deck_Starter].manualAdd(newCard);
					this.Controls.Add(newCard);
				}
			}

			deckList[(int)eDeck.Deck_Starter].shuffle();

			deckList[(int)eDeck.Deck_Starter].printList();

			#endregion


			#region " Deal the Cards "

			//28 cards among the 7 columns
			for(int i = 0; i < 7; i++)
			{
				for(int j = 0; j <= i; j++)
				{
					Card toDeal = deckList[(int)eDeck.Deck_Starter].topCard();
					toDeal.getOwner().removeTop();
					deckList[(int)eDeck.Deck_Column1 + i].manualAdd(toDeal);
					toDeal.BringToFront();
					if(i != j)
						toDeal.mode = cardsdll.mdFaceDown;
				}
			}

			//remaining 24 cards to the main deck
			for(int i = deckList[(int)eDeck.Deck_Starter].size(); i > 0; i--)
			{
				Card toDeal = deckList[(int)eDeck.Deck_Starter].topCard();
				toDeal.getOwner().removeTop();
				toDeal.BringToFront();
				toDeal.mode = cardsdll.mdFaceDown;
				deckList[(int)eDeck.Deck_Main].manualAdd(toDeal);
			}

			#endregion

		}

		private void Form1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)Keys.Escape)
			{
				this.Close();
			}
		}
	
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if(e.CloseReason == CloseReason.WindowsShutDown)
				return;

			FrmConfirmExit frmConfirmExit = new FrmConfirmExit();
			if(frmConfirmExit.ShowDialog() != DialogResult.OK)
			{
				this.Update();
				e.Cancel = true;
			}
			frmConfirmExit.Dispose();
		}
	}
}
