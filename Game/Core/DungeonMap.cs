using RogueSharp;
using RLNET;
using System.Collections.Generic;
using System.Linq;
using RLGame.GameStates;

namespace RLGame.Core
{
	public class DungeonMap : Map
	{
		public int MapLevel { get; set; }

		public List<Rectangle> Rooms;
		public List<Monster> Monsters { get; private set; }
		public List<List<Rectangle>> islands;

		public Stairs StairsUp { get; set; }
		public Stairs StairsDown { get; set; }

		public DungeonMap() {
			Rooms = new List<Rectangle>();
			Monsters = new List<Monster>();
		}
		public void Initialize() {
			Main.SchedulingSystem.Clear();
			Main.SchedulingSystem.Add( Main.SchedulingSystem.update );
		}

		public void PreLevelChange() {
			Player player = Main.GameController.Player;
			SetIsWalkable( player.X, player.Y, true );

			foreach ( Monster monster in Monsters )
			{
				SetIsWalkable( monster.X, monster.Y, true );
			}
			Main.SchedulingSystem.Remove( player, false );
		}

		public void PostLevelChange( bool down ) {
			Player player = Main.GameController.Player;
			Initialize();
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
			foreach ( Monster monster in Monsters )
			{
				AddMonster( monster, true );
			}
			UpdatePlayerFieldOfView();
		}

		public void AddPlayer( Player player ) {
			Main.GameController.Player = player;
			SetIsWalkable( player.X, player.Y, false );
			UpdatePlayerFieldOfView();
			if ( !Main.SchedulingSystem.Contains( player ) )
				Main.SchedulingSystem.Add( player );
		}

		public void AddMonster( Monster monster, bool setActive ) {
			if ( !Monsters.Contains( monster ) )
				Monsters.Add( monster );
			if ( setActive )
			{
				SetIsWalkable( monster.X, monster.Y, false );
				Main.SchedulingSystem.Add( monster );
			}
		}

		public void RemoveMonster( Monster monster ) {
			Monsters.Remove( monster );
			SetIsWalkable( monster.X, monster.Y, true );
			Main.SchedulingSystem.Remove( monster, true );
			Main.SchedulingSystem.update.UpdateEvent -= monster.OnUpdateEvent;

		}

		public Actor GetActorAt( int x, int y ) {
			foreach ( Monster monster in Monsters )
			{
				if ( monster.X == x && monster.Y == y )
				{
					return monster;
				}
			}
			Player player = Main.GameController.Player;
			if ( player.X == x && player.Y == y )
			{
				return player;
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
			Player player = Main.GameController.Player;
			return StairsDown.X == player.X && StairsDown.Y == player.Y;
		}

		public bool CanMoveUpToPreviousLevel() {
			Player player = Main.GameController.Player;
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
			ICell cell = GetCell( x, y );
			SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
		}

		public void UpdatePlayerFieldOfView() {
			Player player = Main.GameController.Player;
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
			foreach ( Monster monster in Monsters )
			{
				monster.Draw( mapConsole, this );
				monster.UpdateTimeline( Main.Timeline, this );
			}

			int i = 0;
			foreach ( Monster monster in Monsters )
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

		public Direction GetDirection( ICell origin, ICell target ) {
			int xDifference = target.X - origin.X;
			int yDifference = target.Y - origin.Y;

			if ( xDifference < 0 && yDifference < 0 )
				return Direction.UpLeft;

			else if ( xDifference == 0 && yDifference < 0 )
				return Direction.Up;

			else if ( xDifference > 0 && yDifference < 0 )
				return Direction.UpRight;

			else if ( xDifference < 0 && yDifference == 0 )
				return Direction.Left;

			else if ( xDifference > 0 && yDifference == 0 )
				return Direction.Right;

			else if ( xDifference < 0 && yDifference > 0 )
				return Direction.DownLeft;

			else if ( xDifference == 0 && yDifference > 0 )
				return Direction.Down;

			else if ( xDifference > 0 && yDifference > 0 )
				return Direction.DownRight;

			else
				return Direction.None;
		}

		public ICell GetAdjacentCell( int x, int y, Direction direction ) {
			switch ( direction )
			{
				case Direction.UpLeft:
					return GetCell( x - 1, y - 1 );
				case Direction.Up:
					return GetCell( x, y - 1 );
				case Direction.UpRight:
					return GetCell( x + 1, y - 1 );
				case Direction.Left:
					return GetCell( x - 1, y );
				case Direction.Right:
					return GetCell( x + 1, y );
				case Direction.DownLeft:
					return GetCell( x - 1, y + 1 );
				case Direction.Down:
					return GetCell( x, y + 1 );
				case Direction.DownRight:
					return GetCell( x + 1, y + 1 );
				default:
					return null;
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
