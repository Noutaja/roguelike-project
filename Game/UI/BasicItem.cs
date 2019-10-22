using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Core;
using RLNET;

namespace RLGame.UI
{
	public class BasicItem : MenuItem
	{
		public BasicItem( Menu parent, String title, Action action ) {
			_parent = parent;
			Action = action;
			Title = title;
			Width = title.Length;
		}

		public override void Activate() {
			Action();
		}

		public override void Draw( RLConsole console, int i ) {
			if ( Location != null )
			{
				console.Print(0, i, Title, Colors.Text);
			}
			else
			{
				console.Print( Location.X, Location.Y, Title, Colors.Text );
			}
		}
	}
}
