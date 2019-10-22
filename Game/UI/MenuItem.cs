using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.UI
{
	public abstract class MenuItem
	{
		protected Menu _parent;
		public String Title;
		public Action Action;
		public int Width;
		public Point Location;

		public virtual void Activate() {
			throw new NotImplementedException();
		}

		public virtual void Draw( RLConsole console, int i) {
			throw new NotImplementedException();
		}
	}
}
