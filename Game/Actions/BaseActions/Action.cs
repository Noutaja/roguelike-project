using RLGame.Core;
using RLGame.Interfaces;
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
		public List<ActionTag> Tags { get; protected set; }

		protected Action() {
			Tags = new List<ActionTag>();
		}

		protected ICell GetCell( int x, int y ) {
			DungeonMap map = Game.GameController.CurrentMap;
			return map.GetCell( x, y );
		}

		protected Actor GetActorAt(int x, int y) {
			DungeonMap map = Game.GameController.CurrentMap;
			return map.GetActorAt( x, y );
		}

		protected Actor GetActorAt(ICell cell ) {
			DungeonMap map = Game.GameController.CurrentMap;
			return map.GetActorAt( cell.X, cell.Y );
		}

		protected void AddToScheduling(IScheduleable scheduleable) {
			Game.SchedulingSystem.Add( scheduleable );
		}

		protected void ModifySpeed() {
			Actor.Speed = (int) ( Actor.Initiative * TimeMultiplier );
		}

		protected void SetLastAction() {
			Actor.LastAction = this;
		}
	}
}
