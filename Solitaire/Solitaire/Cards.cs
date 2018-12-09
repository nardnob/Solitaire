using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Solitaire
{
	public enum eSUIT : int
	{
		CLUBS = 0,
		DIAMOND = 1,
		HEARTS = 2,
		SPADES = 3
	}

	public enum eRank : int
	{
		ACE = 0,
		TWO = 1,
		THREE = 2,
		FOUR = 3,
		FIVE = 4,
		SIX = 5,
		SEVEN = 6,
		EIGHT = 7,
		NINE = 8,
		TEN = 9,
		JACK = 10,
		QUEEN = 11,
		KING = 12
	}

	public enum eBACK : int
	{
		CROSSHATCH = 53, /* XP = CROSSHATCH */
		SKY = 54, /* XP = SKY */
		MINERAL = 55, /* XP = MINERAL */
		FISH = 56, /* XP = FISH */
		FROG = 57, /* XP = FROG */
		MOONFLOWER = 58, /* XP = MOONFLOWER */
		ISLAND = 59, /* XP = ISLAND */
		SQUARES = 60, /* XP = SQUARES */
		MAGENTA = 61, /* XP = MAGENTA */
		SANDDUNES = 62, /* XP = SANDDUNES */
		SPACE = 63, /* XP = SPACE */
		LINES = 64, /* XP = LINES */
		TOYCARS = 65, /* XP = TOYCARS */
		UNUSED = 66, /* XP = UNUSED */
		THE_X = 67, /* XP = THE_X */
		THE_O = 68  /* XP = THE_0 */
	}

	public class cardsdll
	{
		/*****************************************************************************************
		* Interface to cards.dll                                                                 *
		*****************************************************************************************/

		[DllImport("cards.dll")]
		public static extern bool cdtInit(ref int width, ref int height);

		[DllImport("cards.dll")]
		public static extern void cdtTerm();

		[DllImport("cards.dll")]
		public static extern bool cdtDraw(IntPtr hdc, int x, int y, int card, int mode, int color);

		[DllImport("cards.dll")]
		public static extern bool cdtDrawExt(IntPtr hdc, int x, int y, int dx, int dy, int card, int mode, int color);

		[DllImport("cards.dll")]
		public static extern bool cdtAnimate(IntPtr hdc, int cardback, int x, int y, int frame);

		/*****************************************************************************************
		* Constant declarations                                                                  *
		*****************************************************************************************/

		/* mode parameters */
		public const int mdFaceUp = 0; /* Draw card face up */
		public const int mdFaceDown = 1; /* Draw card face down */
		public const int mdHilite = 2; /* Same as FaceUp except drawn inverted */
		public const int mdGhost = 3; /* Draw a ghost card -- for ace piles */
		public const int mdRemove = 4; /* draw background specified by color */
		public const int mdInvisibleGhost = 5; /* ? */
		public const int mdDeckX = 6; /* Draw X */
		public const int mdDeckO = 7; /* Draw O */

		/*****************************************************************************************
		* Public Interface                                                                       *
		*****************************************************************************************/

		public cardsdll(int width, int height)
		{
			if(!cdtInit(ref width, ref height))
				throw new Exception("cards.dll did not load");
		}

		public void Dispose()
		{
			cdtTerm();
		}

		public bool drawCard(IntPtr hdc, int x, int y, int card, int mode, int color)
		{
			return cdtDraw(hdc, x, y, card, mode, color);
		}

		public bool drawCardBack(IntPtr hdc, int x, int y, eBACK back)
		{
			return cdtDraw(hdc, x, y, (int)back, mdFaceDown, 0);
		}

		public bool drawAnimatedBack(IntPtr hdc, int x, int y, int card, int frame)
		{
			return cdtAnimate(hdc, card, x, y, frame);
		}

		public bool drawInvertedCard(IntPtr hdc, int x, int y, int card)
		{
			return cdtDraw(hdc, x, y, card, mdHilite, 0);
		}

		public bool drawEmptyCard(IntPtr hdc, int x, int y, int color)
		{
			return cdtDraw(hdc, x, y, 1, mdGhost, color);
		}

		public bool drawExtrudedCard(IntPtr hdc, int x, int y, int dx, int dy, int card, int mode, int color)
		{
			return cdtDrawExt(hdc, x, y, dx, dy, card, mode, color);
		}

		public static eSUIT getSuit(int card)
		{
			int suit = card % 4;
			return (eSUIT)suit;
		}

		public static eRank getRank(int card)
		{
			int rank = card / 4;
			return (eRank)rank;
		}
	}
}