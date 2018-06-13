using RLGame.Interfaces;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Core
{
	public class Item : IDrawable
	{
		public int Weight { get; set; }

		public Item() {
			Weight = 1;
			Color = Colors.Gold;
			Symbol = ',';
		}
		//IDrawable
		public RLColor Color { get; set; }
		public char Symbol { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public void Draw( RLConsole console, IMap map ) {
			if ( !map.GetCell( X, Y ).IsExplored )
			{
				return;
			}
			if ( map.IsInFov( X, Y ) )
			{
				console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol );
			}
			else
			{
				console.Set( X, Y, Colors.Floor, Colors.FloorBackground, '.' );
			}
		}
	}
}
