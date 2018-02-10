using RLGame.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Behaviors.BaseBehaviors
{
	abstract public class Behavior
	{
		protected DungeonMap dungeonMap;
		protected Player player;
		protected FieldOfView monsterFov;
		protected int? TurnsSincePF;
		protected int resetTime;
		protected PathFinder cleanMapPF;
		protected Path path;

		public Behavior( int resetAt = 15) {
			dungeonMap = Game.GameController.CurrentMap;
			player = Game.GameController.Player;
			monsterFov = new FieldOfView( dungeonMap );
			resetTime = resetAt;
			path = null;
		}

		protected void Initialize() {
			if ( dungeonMap != Game.GameController.CurrentMap )
			{
				dungeonMap = Game.GameController.CurrentMap;
				foreach ( Monster m in GetMonsters() )
				{
					dungeonMap.SetIsWalkable( m.X, m.Y, true );
				}
				dungeonMap.SetIsWalkable( player.X, player.Y, true );
				cleanMapPF = new PathFinder( dungeonMap, 1.41 );
				foreach ( Monster m in GetMonsters() )
				{
					dungeonMap.SetIsWalkable( m.X, m.Y, false );
				}
				dungeonMap.SetIsWalkable( player.X, player.Y, false );
			}
			player = Game.GameController.Player;
			monsterFov = new FieldOfView( dungeonMap );
		}

		protected List<Monster> GetMonsters() {
			return dungeonMap.Monsters;
		}

		protected Path GetPath( int startX, int startY, int endX, int endY ) {
			// Before we find a path, make sure to make the monster and player Cells walkable
			dungeonMap.SetIsWalkable( startX, startY, true );
			dungeonMap.SetIsWalkable( endX, endY, true );

			PathFinder pathFinder = new PathFinder( dungeonMap, 1.41 );
			Path path = null;

			try
			{
				path = pathFinder.ShortestPath(
					dungeonMap.GetCell( startX, startY ),
					dungeonMap.GetCell( endX, endY ) );
			}
			catch ( PathNotFoundException )
			{
				//Remove actors from map
				foreach ( Monster m in GetMonsters() )
				{
					dungeonMap.SetIsWalkable( m.X, m.Y, true );
				}
				dungeonMap.SetIsWalkable( player.X, player.Y, true );
				

				path = cleanMapPF.ShortestPath(
					dungeonMap.GetCell( startX, startY ),
					dungeonMap.GetCell( endX, endY ) );

			}

			//Put actors back on the map
			foreach ( Monster m in GetMonsters() )
			{
				dungeonMap.SetIsWalkable( m.X, m.Y, false );
			}
			dungeonMap.SetIsWalkable( player.X, player.Y, false );

			return path;
		}
	}
}
