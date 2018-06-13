using RLGame.Core;
using RLGame.Interfaces;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RLGame.GameStates;

namespace RLGame.Systems
{
	public class GameController
	{
		private Thread mapLoader;
		private object mapsLock = new object();
		public List<DungeonMap> Maps { get; private set; }
		public DungeonMap CurrentMap { get; private set; }
		public static SchedulingSystem SchedulingSystem { get; private set; }
		public static Timeline Timeline { get; private set; }
		public static InventorySystem InventorySystem { get; private set; }
		private readonly int MAXMAPWIDTH;
		private readonly int MAXMAPHEIGHT;
		public Player Player;
		private bool _isPlayerTurn;
		public bool IsPlayerTurn {
			get { return _isPlayerTurn; }
			set { _isPlayerTurn = value; }
		}

		public GameController( int maxMapWidth, int maxMapHeight ) {
			Maps = new List<DungeonMap>();
			MAXMAPWIDTH = maxMapWidth;
			MAXMAPHEIGHT = maxMapHeight;
		}

		public void Init() {
			SchedulingSystem = new SchedulingSystem();
			Timeline = new Timeline();
			InventorySystem = new InventorySystem();
			InventorySystem.Init();

			Player = new Player() { X = -1, Y = -1 };
			MapGenerator mapGenerator = new MapGenerator( MAXMAPWIDTH, MAXMAPHEIGHT, 50, 8, 4, 1, true );
			CurrentMap = mapGenerator.CreateMap();
			//CurrentMap.UpdatePlayerFieldOfView();
			Maps.Add( CurrentMap );
			CurrentMap.PostLevelChange( true );

			mapLoader = new Thread( PreloadNextLevel );
			mapLoader.Start( CurrentMap.MapLevel + 1 );
		}
		public void EndPlayerTurn() {
			IsPlayerTurn = false;
		}

		public void AdvanceTime() {
			IScheduleable scheduleable = SchedulingSystem.Get();

			if ( scheduleable is Player )
			{
				IsPlayerTurn = true;
			}
			else if ( scheduleable is Attack )
			{
				Attack attack = scheduleable as Attack;
				attack.Activate();
				AdvanceTime();
			}
			else if ( scheduleable is Monster )
			{
				Monster monster = scheduleable as Monster;
				monster.Activate();
				SchedulingSystem.Add( monster );
				AdvanceTime();
			}
			else if ( scheduleable is Update )
			{
				Update update = scheduleable as Update;
				update.Activate();
				SchedulingSystem.Add( update );
				AdvanceTime();
			}
		}

		public void ChangeLevel( bool down ) {
			int newLevel = CurrentMap.MapLevel;
			if ( down == true )
				newLevel++;
			else
				newLevel--;
			CurrentMap.PreLevelChange();
			mapLoader.Join();
			lock ( mapsLock )
				CurrentMap = Maps[newLevel - 1];

			CurrentMap.PostLevelChange( down );
			MainScreen.MessageLog.Clear();
			MainScreen.MessageLog.Add( $"Entering level {CurrentMap.MapLevel}" );
			Timeline.Clear();
			IsPlayerTurn = false;

			bool lastGenerated;
			lock ( mapsLock )
				lastGenerated = CurrentMap.MapLevel == Maps.Count;
			if ( lastGenerated )
			{
				mapLoader = new Thread( PreloadNextLevel );
				mapLoader.Start( CurrentMap.MapLevel + 1 );
			}
		}

		private void PreloadNextLevel( Object newLevel ) {
			int tmp = (int) newLevel;
			MapGenerator mapGenerator = new MapGenerator( MAXMAPWIDTH, MAXMAPHEIGHT, 50, 8, 4, tmp, true );
			DungeonMap newMap = mapGenerator.CreateMap();
			lock ( mapsLock )
				Maps.Add( newMap );
		}
	}
}
