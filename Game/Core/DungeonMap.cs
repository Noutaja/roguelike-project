using RogueSharp;
using RLNET;
using System.Collections.Generic;
using System.Linq;

namespace RLGame.Core
{
	public class DungeonMap : Map
	{
		public int MapLevel { get; set; }

		public List<Rectangle> Rooms;
		public List<Monster> _monsters;
		public List<List<Rectangle>> islands;

		public Stairs StairsUp { get; set; }
		public Stairs StairsDown { get; set; }

		public DungeonMap() {
			Game.SchedulingSystem.Clear();
			Game.SchedulingSystem.Add( Game.SchedulingSystem.update );
			Rooms = new List<Rectangle>();
			_monsters = new List<Monster>();
		}

		public void PreLevelChange() {
			Player player = Game.Player;
			SetIsWalkable( player.X, player.Y, true );

			foreach ( Monster monster in _monsters )
			{
				SetIsWalkable( monster.X, monster.Y, true );
			}
			Game.SchedulingSystem.Remove( player );
		}

		public void PostLevelChange( bool down ) {
			Player player = Game.Player;
			Game.SchedulingSystem.Clear();
			Game.SchedulingSystem.Add( Game.SchedulingSystem.update );
			if ( down )
			{
				player.X = StairsUp.X;
				player.Y = StairsUp.Y;
				AddPlayer( player );
			}
			else
			{
				player.X = StairsDown.X;
				player.Y = StairsDown.Y;
				AddPlayer( player );
			}
			foreach ( Monster monster in _monsters )
			{
				AddMonster( monster );
			}
		}

		public void AddPlayer( Player player ) {
			Game.Player = player;
			SetIsWalkable( player.X, player.Y, false );
			UpdatePlayerFieldOfView();
			if ( !Game.SchedulingSystem.Contains( player ) )
				Game.SchedulingSystem.Add( player );
		}

		public void AddMonster( Monster monster ) {
			if ( !_monsters.Contains( monster ) )
				_monsters.Add( monster );
			SetIsWalkable( monster.X, monster.Y, false );
			Game.SchedulingSystem.Add( monster );

		}

		public void RemoveMonster( Monster monster ) {
			_monsters.Remove( monster );
			SetIsWalkable( monster.X, monster.Y, true );
			Game.SchedulingSystem.Remove( monster, true );

		}

		public Monster GetMonsterAt( int x, int y ) {
			foreach ( Monster monster in _monsters )
			{
				if ( monster.X == x && monster.Y == y )
				{
					return monster;
				}
			}
			return null;
		}

		public Point GetRandomWalkableLocationInRoom( Rectangle room ) {
			if ( DoesRoomHaveWalkableSpace( room ) )
			{
				for ( int i = 0; i < 100; i++ )
				{
					int x = Game.Random.Next( 1, room.Width ) + room.X;
					int y = Game.Random.Next( 1, room.Height ) + room.Y;
					if ( IsWalkable( x, y ) )
					{
						return new Point( x, y );
					}
				}
			}

			// If we didn't find a walkable location in the room return 0,0
			return new Point();
		}

		public bool CanMoveDownToNextLevel() {
			Player player = Game.Player;
			return StairsDown.X == player.X && StairsDown.Y == player.Y;
		}

		public bool CanMoveUpToPreviousLevel() {
			Player player = Game.Player;
			return StairsUp.X == player.X && StairsUp.Y == player.Y && MapLevel > 1;
		}

		public bool DoesRoomHaveWalkableSpace( Rectangle room ) {
			for ( int x = 1; x <= room.Width; x++ )
			{
				for ( int y = 1; y <= room.Height; y++ )
				{
					if ( IsWalkable( x + room.X, y + room.Y ) )
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool SetActorPosition( Actor actor, int x, int y ) {
			if ( GetCell( x, y ).IsWalkable )
			{
				//Set previous cell walkable
				SetIsWalkable( actor.X, actor.Y, true );
				actor.X = x;
				actor.Y = y;
				//Set new cell unwalkable
				SetIsWalkable( actor.X, actor.Y, false );

				//Update fov if player is moving
				if ( actor is Player )
				{
					UpdatePlayerFieldOfView();
				}
				return true;
			}
			return false;
		}

		public void SetIsWalkable( int x, int y, bool isWalkable ) {
			ICell cell = GetCell( x, y ); //!!!!!!!!!!!!!!ICell!!!!!!!!!!!!!!!!!
			SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
		}

		public void UpdatePlayerFieldOfView() {
			Player player = Game.Player;
			ComputeFov( player.X, player.Y, player.Awareness, true );
			foreach ( Cell cell in GetAllCells() )
			{
				if ( IsInFov( cell.X, cell.Y ) )
				{
					SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
				}
			}
		}

		public void Draw( RLConsole mapConsole, RLConsole statConsole, RLConsole timeLineConsole ) {
			//Draw base cells
			foreach ( Cell cell in GetAllCells() )
			{
				SetConsoleSymbolForCell( mapConsole, cell );
			}

			//Draw stairs
			StairsUp.Draw( mapConsole, this );
			StairsDown.Draw( mapConsole, this );

			//Draw monsters
			foreach ( Monster monster in _monsters )
			{
				monster.Draw( mapConsole, this );
				monster.UpdateTimeline( Game.Timeline, this );
			}

			int i = 0;
			foreach ( Monster monster in _monsters )
			{
				// When the monster is in the field-of-view also draw their stats
				if ( IsInFov( monster.X, monster.Y ) )
				{
					monster.Draw( mapConsole, this );

					// Pass in the index to DrawStats and increment it afterwards
					monster.DrawStats( statConsole, i );
					i++;
				}
			}
		}

		private void SetConsoleSymbolForCell( RLConsole console, Cell cell ) {
			//Check for cell exploration
			if ( !cell.IsExplored )
			{
				return;
			}
			//Check for cell visibility
			if ( IsInFov( cell.X, cell.Y ) )
			{
				if ( cell.IsWalkable )
				{
					console.Set( cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.' );
				}
				else
				{
					console.Set( cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#' );
				}
			}
			else
			{
				if ( cell.IsWalkable )
				{
					console.Set( cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.' );
				}
				else
				{
					console.Set( cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#' );
				}
			}
		}

		public void RevealMap() {
			for ( int y = 1; y < Height; y++ )
			{
				for ( int x = 1; x < Width; x++ )
				{
					ICell cell = GetCell( x, y );
					SetCellProperties( x, y, cell.IsTransparent, cell.IsWalkable, true );
				}
			}
		}
	}
}
