using RLNET;
using RLGame.Core;
using RLGame.Systems;
using RogueSharp.Random;
using System;
using Weighted_Randomizer;
using System.Collections.Generic;
using OpenTK.Input;

namespace RLGame
{
	public static class Game
	{
		private static readonly int _screenWidth = 100;
		private static readonly int _screenHeight = 70;
		private static RLRootConsole _rootConsole;

		private static readonly int _mapWidth = 80;
		private static readonly int _mapHeight = 48;
		private static RLConsole _mapConsole;

		private static readonly int _messageWidth = 80;
		private static readonly int _messageHeight = 11;
		private static RLConsole _messageConsole;

		private static readonly int _statWidth = 20;
		private static readonly int _statHeight = 70;
		private static RLConsole _statConsole;

		private static readonly int _timeLineWidth = 80;
		private static readonly int _timeLineHeight = 11;
		private static RLConsole _timeLineConsole;

		private static bool _renderRequired = true;
		private static bool _newMap = true;

		private static CommandSystem _commandSystem;
		public static CommandSystem CommandSystem {
			get { return _commandSystem; }
			private set {
				_commandSystem = value;
				if ( PlayerControls != null )
					PlayerControls.CommandSystem = value;
			}
		}
		public static SchedulingSystem SchedulingSystem { get; private set; }
		public static Timeline Timeline { get; private set; }
		public static MessageLog MessageLog { get; private set; }
		public static PlayerControls PlayerControls { get; private set; }
		public static IRandom Random { get; private set; }
		public static IWeightedRandomizer<Bodypart> WeightedRandom;
		public static Player Player { get; set; }
		public static DungeonMap CurrentMap { get; private set; }
		public static List<DungeonMap> Maps { get; private set; }

		static void Main( string[] args ) {
			string fontFileName = "terminal8x8.png";

			//Initialize RNG
			int seed = (int) DateTime.UtcNow.Ticks;
			WeightedRandom = new StaticWeightedRandomizer<Bodypart>( seed );
			Random = new DotNetRandom( seed );
			string consoleTitle = $"RLGame Level 1 - Seed {seed}"; //Show the seed

			_rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
			_mapConsole = new RLConsole( _mapWidth, _mapHeight );
			_messageConsole = new RLConsole( _messageWidth, _messageHeight );
			_statConsole = new RLConsole( _statWidth, _statHeight );
			_timeLineConsole = new RLConsole( _timeLineWidth, _timeLineHeight );

			Maps = new List<DungeonMap>();
			SchedulingSystem = new SchedulingSystem();
			CommandSystem = new CommandSystem();
			Player = new Player();
			Timeline = new Timeline();
			MessageLog = new MessageLog();
			PlayerControls = new PlayerControls( _rootConsole );
			MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 50, 8, 4, 1, true );
			CurrentMap = mapGenerator.CreateMap();
			CurrentMap.UpdatePlayerFieldOfView();
			Maps.Add( CurrentMap );

			MessageLog.Add( $"Level created with seed '{seed}'" );

			_rootConsole.Update += OnRootConsoleUpdate;
			_rootConsole.Render += OnRootConsoleRender;
			_rootConsole.Run();
		}

		public static void ChangeLevel( bool down ) {
			int newLevel = CurrentMap.MapLevel;
			if ( down == true )
				newLevel++;
			else
				newLevel--;
			CurrentMap.PreLevelChange();

			if ( Maps.Count < newLevel )
			{
				MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 50, 8, 4, newLevel, down );
				CurrentMap = mapGenerator.CreateMap();
				MessageLog = new MessageLog();
				MessageLog.Add( $"Entering level {CurrentMap.MapLevel}" );
				CommandSystem = new CommandSystem();
				Timeline.Clear();
				_rootConsole.Title = $"Level {CurrentMap.MapLevel}";
				_newMap = true;
				Maps.Add( CurrentMap );
			}
			else
			{
				CurrentMap = Maps[newLevel - 1];
				CurrentMap.PostLevelChange( down );
				MessageLog = new MessageLog();
				MessageLog.Add( $"Entering level {CurrentMap.MapLevel}" );
				CommandSystem = new CommandSystem();
				Timeline.Clear();
				_rootConsole.Title = $"Level {CurrentMap.MapLevel}";
				_newMap = true;
			}
		}

		private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e ) {
			RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
			if ( CommandSystem.IsPlayerTurn )
			{
				_renderRequired = false;
				if ( keyPress != null )
				{
					_renderRequired = PlayerControls.CheckInput( keyPress );
				}
			}
			else
			{
				CommandSystem.AdvanceTime();
				_renderRequired = true;
			}
		}

		private static void OnRootConsoleRender( object sender, UpdateEventArgs e ) {
			if ( _newMap )
			{
				_newMap = false;
				_renderRequired = true;
			}
			//if ( _renderRequired )
			{
				_mapConsole.Clear();
				_statConsole.Clear();
				_messageConsole.Clear();
				_timeLineConsole.Clear();

				CurrentMap.Draw( _mapConsole, _statConsole, _timeLineConsole );
				Player.Draw( _mapConsole, CurrentMap );
				Player.DrawStats( _statConsole );
				MessageLog.Draw( _messageConsole );
				Timeline.Draw( _timeLineConsole );

				RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight,
					_rootConsole, 0, _timeLineHeight );
				RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight,
					_rootConsole, _mapWidth, 0 );
				RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight,
					_rootConsole, 0, _screenHeight - _messageHeight );
				RLConsole.Blit( _timeLineConsole, 0, 0, _timeLineWidth, _timeLineHeight,
					_rootConsole, 0, 0 );

				_rootConsole.Draw();
			}
		}
	}
}
