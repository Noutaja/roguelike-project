using RLGame.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions.BaseActions
{
	abstract public class Action
	{
		public double TimeMultiplier { get; protected set; }
		public Actor Actor { get; protected set; }
		public String Name { get; protected set; }

		protected ICell GetCell( int x, int y ) {
			DungeonMap map = Game.CurrentMap;
			return map.GetCell( x, y );
		}

		protected void ModifySpeed() {
			Actor.Speed = (int) ( Actor.Initiative * TimeMultiplier );
		}

		protected void SetLastAction() {
			Actor.LastAction = this;
		}
	}
}
