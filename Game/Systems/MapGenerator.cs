using System;
using System.Collections.Generic;
using System.Linq;
using RLGame.Core;
using RLGame.GameStates;
using RLGame.Monsters;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace RLGame.Systems
{
	public class MapGenerator
	{
		private readonly int WIDTH;
		private readonly int HEIGHT;
		private readonly int MAXROOMS;
		private readonly int ROOMMAXSIZE;
		private readonly int ROOMMINSIZE;
		private readonly int MAPLEVEL;
		private readonly bool MOVINGDOWN;
		private List<List<Rectangle>> _islands;

		private readonly DungeonMap _map;

		public MapGenerator( int width, int height,
		int maxRooms, int roomMaxSize, int roomMinSize,
		int mapLevel, bool movingDown ) {
			WIDTH = width;
			HEIGHT = height;
			MAXROOMS = maxRooms;
			ROOMMAXSIZE = roomMaxSize;
			ROOMMINSIZE = roomMinSize;
			MAPLEVEL = mapLevel;
			MOVINGDOWN = movingDown;
			_map = new DungeonMap();
		}

		public DungeonMap CreateMap() {
			// Set the properties of all cells to false
			_map.Initialize( WIDTH, HEIGHT );
			_map.MapLevel = MAPLEVEL;

			// Try to place as many rooms as the specified maxRooms
			for ( int i = 0; i < MAXROOMS; i++ )
			{
				// Determine the size and position of the room randomly
				int roomWidth = Game.Random.Next( ROOMMINSIZE, ROOMMAXSIZE );
				int roomHeight = Game.Random.Next( ROOMMINSIZE, ROOMMAXSIZE );
				int roomXPosition = Game.Random.Next( 0, WIDTH - roomWidth - 1 );
				int roomYPosition = Game.Random.Next( 0, HEIGHT - roomHeight - 1 );

				var newRoom = new Rectangle( roomXPosition, roomYPosition,
				  roomWidth, roomHeight );

				//ENABLE IF STATEMENT TO HAVE MORE CONFINED ROOMS!
				if ( Game.Random.Next( 0, 9 ) < 1 )
				{
					// Check to see if the room rectangle intersects with any other rooms
					bool newRoomIntersects = _map.Rooms.Any( room => newRoom.Intersects( room ) );

					// As long as it doesn't intersect add it to the list of rooms
					if ( !newRoomIntersects )
					{
						_map.Rooms.Add( newRoom );
					}
				}
				else
				{
					_map.Rooms.Add( newRoom );
				}
			}
			// Iterate through each room that we wanted placed 
			// call CreateRoom to make it
			foreach ( Rectangle room in _map.Rooms )
			{
				CreateRoom( room );
			}

			CreateHallways();
			_map.islands = _islands;
			CreateStairs();
			
			GenerateMonsters();

			return _map;
		}

		private void CreateHallways() {
			//Initially connect all rooms to closest rooms
			_islands = new List<List<Rectangle>>();
			foreach ( Rectangle room1 in _map.Rooms )
			{
				Rectangle closestRoom = new Rectangle();
				double distance2 = Double.PositiveInfinity;

				foreach ( Rectangle room2 in _map.Rooms )
				{

					if ( room2 != room1 )
					{
						int roomCenterX = room1.Center.X;
						int roomCenterY = room1.Center.Y;
						int otherRoomCenterX = room2.Center.X;
						int otherRoomCenterY = room2.Center.Y;

						double tmp = Math.Sqrt( ( Math.Pow( roomCenterX - otherRoomCenterX, 2 ) +
							Math.Pow( roomCenterY - otherRoomCenterY, 2 ) ) );

						if ( tmp < Math.Sqrt( distance2 * distance2 ) )
						{
							distance2 = tmp;
							closestRoom = room2;
						}
					}
				}
				int closestRoomCenterX = closestRoom.Center.X;
				int closestRoomCenterY = closestRoom.Center.Y;
				int currentRoomCenterX = room1.Center.X;
				int currentRoomCenterY = room1.Center.Y;
				if ( Game.Random.Next( 1, 2 ) == 1 )
				{
					CreateHorizontalTunnel( closestRoomCenterX, currentRoomCenterX, closestRoomCenterY );
					CreateVerticalTunnel( closestRoomCenterY, currentRoomCenterY, currentRoomCenterX );
				}
				else
				{
					CreateVerticalTunnel( closestRoomCenterY, currentRoomCenterY, closestRoomCenterX );
					CreateHorizontalTunnel( closestRoomCenterX, currentRoomCenterX, currentRoomCenterY );
				}
			}

			_islands = FormIslands();

			//Start connecting closest islands together
			bool allConnected;
			do
			{
				allConnected = true;
				//Temporary islands that will get merged together with each pass
				List<List<Rectangle>> tmpIslands = FormIslands();
				//Connections of rooms closest to one another between islands
				List<List<Rectangle>> connections = new List<List<Rectangle>>();
				foreach ( List<Rectangle> island1 in tmpIslands )
				{
					Rectangle roomA = new Rectangle();
					Rectangle roomB = new Rectangle();
					double distance = double.PositiveInfinity;
					foreach ( Rectangle room1 in island1 )
					{
						foreach ( List<Rectangle> island2 in tmpIslands )
						{

							if ( island1 != island2 )
							{
								foreach ( Rectangle room2 in island2 )
								{
									if ( room1 != room2 )
									{
										int room1X = room1.Center.X;
										int room1Y = room1.Center.Y;
										int room2X = room2.Center.X;
										int room2Y = room2.Center.Y;

										double tmp = Math.Sqrt( ( Math.Pow( room1X - room2X, 2 ) +
											Math.Pow( room1Y - room2Y, 2 ) ) );

										if ( tmp < Math.Sqrt( distance * distance ) )
										{
											distance = tmp;
											roomA = room1;
											roomB = room2;

										}
									}
								}
							}

						}
					}
					bool alreadyConnected = false;
					foreach ( List<Rectangle> connection in connections )
					{
						if ( connection.Contains( roomA ) )
						{
							alreadyConnected = true;
						}
					}
					if ( !alreadyConnected )
					{
						connections.Add( new List<Rectangle>() { roomA, roomB } );
					}

				}

				while ( connections.Any() )
				{
					List<Rectangle> connection = connections[0];
					int room1X = connection[0].Center.X;
					int room1Y = connection[0].Center.Y;
					int room2X = connection[1].Center.X;
					int room2Y = connection[1].Center.Y;
					if ( Game.Random.Next( 1, 2 ) == 1 )
					{
						CreateHorizontalTunnel( room1X, room2X, room1Y );
						CreateVerticalTunnel( room1Y, room2Y, room2X );
					}
					else
					{
						CreateVerticalTunnel( room1Y, room2Y, room1X );
						CreateHorizontalTunnel( room1X, room2X, room2Y );
					}
					connections.Remove( connection );
				}

				//Check if map is interconnected
				foreach ( Rectangle room in _map.Rooms )
				{
					PathFinder pf = new PathFinder( _map, 1.41 );
					Path path = null;

					path = pf.TryFindShortestPath( ( _map.GetCell( _map.Rooms[0].Center.X, _map.Rooms[0].Center.Y ) ),
														_map.GetCell( room.Center.X, room.Center.Y ) );
					if ( path == null )
					{
						allConnected = false;
					}
				}
			} while ( !allConnected );
		}

		private List<List<Rectangle>> FormIslands() {
			//Temporary variables to add up rooms and islands
			List<List<Rectangle>> newIslands = new List<List<Rectangle>>();
			List<Rectangle> tmpRooms = new List<Rectangle>();
			foreach ( Rectangle room in _map.Rooms )
			{
				tmpRooms.Add( room );
			}
			//Form islands
			while ( tmpRooms.Count > 0 )
			{
				Rectangle currentRoom = tmpRooms[0];
				List<Rectangle> island = new List<Rectangle>();
				//Add all connected rooms to currentRoom to an island
				for ( int j = 0; j < tmpRooms.Count; j++ )
				{
					Rectangle room = tmpRooms[j];
					PathFinder pf = new PathFinder( _map );
					Path path = null;

					path = pf.TryFindShortestPath( ( _map.GetCell( currentRoom.Center.X, currentRoom.Center.Y ) ),
														_map.GetCell( room.Center.X, room.Center.Y ) );

					if ( path != null )
					{
						j--;
						island.Add( room );
						tmpRooms.Remove( room );
					}
				}
				newIslands.Add( island );
			}
			return newIslands;
		}

		private void CreateStairs() {
			if ( MOVINGDOWN )
			{
				_map.StairsUp = new Stairs {
					X = _map.Rooms.First().Center.X + 1,
					Y = _map.Rooms.First().Center.Y,
					IsUp = true
				};
				_map.StairsDown = new Stairs {
					X = _map.Rooms.Last().Center.X,
					Y = _map.Rooms.Last().Center.Y,
					IsUp = false
				};
			}
			else
			{
				_map.StairsUp = new Stairs {
					X = _map.Rooms.Last().Center.X,
					Y = _map.Rooms.Last().Center.Y,
					IsUp = true
				};
				_map.StairsDown = new Stairs {
					X = _map.Rooms.First().Center.X + 1,
					Y = _map.Rooms.First().Center.Y,
					IsUp = false
				};
			}
		}

		//Carve out the room
		private void CreateRoom( Rectangle room ) {
			for ( int x = room.Left + 1; x < room.Right; x++ )
			{
				for ( int y = room.Top + 1; y < room.Bottom; y++ )
				{
					_map.SetCellProperties( x, y, true, true, false );
				}
			}
		}

		private void CreateHorizontalTunnel( int xStart, int xEnd, int yPosition ) {
			for ( int x = Math.Min( xStart, xEnd ); x <= Math.Max( xStart, xEnd ); x++ )
			{
				_map.SetCellProperties( x, yPosition, true, true );
			}
		}

		private void CreateVerticalTunnel( int yStart, int yEnd, int xPosition ) {
			for ( int y = Math.Min( yStart, yEnd ); y <= Math.Max( yStart, yEnd ); y++ )
			{
				_map.SetCellProperties( xPosition, y, true, true );
			}
		}

		private void PlacePlayer() {
			Player player = MainScreen.GameController.Player;
			if ( player == null )
			{
				player = new Player();
			}

			player.X = _map.Rooms[0].Center.X;
			player.Y = _map.Rooms[0].Center.Y;

			_map.AddPlayer( player );
		}

		private void GenerateMonsters() {
			foreach ( var room in _map.Rooms )
			{
				// Each room has a 60% chance of having monsters
				if ( Dice.Roll( "1D10" ) < 3 )
				{
					// Generate between 1 and 4 monsters
					var numberOfMonsters = Dice.Roll( "1D4" );
					for ( int i = 0; i < numberOfMonsters; i++ )
					{
						// Find a random walkable location in the room to place the monster
						Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom( room );
						// It's possible that the room doesn't have space to place a monster
						// In that case skip creating the monster
						if ( randomRoomLocation != Point.Zero )
						{
							// Temporarily hard code this monster to be created at level 1
							var monster = Prototypes.Monsters.Shade();
							monster.X = randomRoomLocation.X;
							monster.Y = randomRoomLocation.Y;
							_map.AddMonster( monster, false );
						}
					}
				}
			}
			var room1 = _map.Rooms.First();
			var numberOfMonsters1 = Dice.Roll( "1D1" );
			for ( int i = 0; i < 1; i++ )
			{
				// Find a random walkable location in the room to place the monster
				Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom( room1 );
				// It's possible that the room doesn't have space to place a monster
				// In that case skip creating the monster
				if ( randomRoomLocation != Point.Zero )
				{
					// Temporarily hard code this monster to be created at level 1
					var monster = Prototypes.Monsters.Shade();
					monster.X = randomRoomLocation.X;
					monster.Y = randomRoomLocation.Y;
					_map.AddMonster( monster, false );
				}
			}
		}
	}
}
