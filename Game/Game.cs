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
		private static readonly int SCREENWIDTH = 100;
		private static readonly int SCREENHEIGHT = 70;
		private static RLRootConsole _rootConsole;

		private static readonly int MAPWIDTH = 80;
		private static readonly int MAPHEIGHT = 48;
		private static RLConsole _mapConsole;

		private static readonly int MESSAGEWIDTH = 80;
		private static readonly int MESSAGEHEIGHT = 11;
		private static RLConsole _messageConsole;

		private static readonly int STATWIDTH = 20;
		private static readonly int STATHEIGHT = 70;
		private static RLConsole _statConsole;

		private static readonly int TIMELINEWIDTH = 80;
		private static readonly int TIMELINEHEIGHT = 11;
		private static RLConsole _timeLineConsole;

		private static GameController _gameController;
		public static GameController GameController {
			get { return _gameController; }
			private set {
				_gameController = value;
				if ( PlayerControls != null )
					PlayerControls.GameController = value;
			}
		}
		public static SchedulingSystem SchedulingSystem { get; private set; }
		public static Timeline Timeline { get; private set; }
		public static MessageLog MessageLog { get; private set; }
		public static PlayerControls PlayerControls { get; private set; }
		public static IRandom Random { get; private set; }
		public static IWeightedRandomizer<Bodypart> WeightedRandom;
		public static DungeonMap CurrentMap { get; private set; }
		public static List<DungeonMap> Maps { get; private set; }

		static void Main( string[] args ) {
			Keyboard.GetState();//IT JUST WERKS NOW?? PLS FIX :D (OnRootConsoleUpdate)
			string fontFileName = "terminal8x8.png";

			//Initialize RNG
			int seed = (int) DateTime.UtcNow.Ticks;
			WeightedRandom = new StaticWeightedRandomizer<Bodypart>( seed );
			Random = new DotNetRandom( seed );
			string consoleTitle = $"RLGame Seed {seed}"; //Show the seed

			_rootConsole = new RLRootConsole( fontFileName, SCREENWIDTH, SCREENHEIGHT, 8, 8, 1f, consoleTitle );
			_mapConsole = new RLConsole( MAPWIDTH, MAPHEIGHT );
			_messageConsole = new RLConsole( MESSAGEWIDTH, MESSAGEHEIGHT );
			_statConsole = new RLConsole( STATWIDTH, STATHEIGHT );
			_timeLineConsole = new RLConsole( TIMELINEWIDTH, TIMELINEHEIGHT );

			Maps = new List<DungeonMap>();
			SchedulingSystem = new SchedulingSystem();
			GameController = new GameController(MAPWIDTH, MAPHEIGHT);
			GameController.Initialize();
			Timeline = new Timeline();
			MessageLog = new MessageLog();
			PlayerControls = new PlayerControls( _rootConsole );
			

			MessageLog.Add( $"Level created with seed '{seed}'" );

			_rootConsole.Update += OnRootConsoleUpdate;
			_rootConsole.Render += OnRootConsoleRender;
			_rootConsole.Run();
		}

		private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e ) {
			RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
			if ( GameController.IsPlayerTurn )
			{
				if ( keyPress != null )
				{
					PlayerControls.CheckInput( keyPress );
				}
			}
			else
			{
				GameController.AdvanceTime();
			}
		}

		private static void OnRootConsoleRender( object sender, UpdateEventArgs e ) {
			
				_mapConsole.Clear();
				_statConsole.Clear();
				_messageConsole.Clear();
				_timeLineConsole.Clear();

				GameController.CurrentMap.Draw( _mapConsole, _statConsole, _timeLineConsole );
				GameController.Player.Draw( _mapConsole, GameController.CurrentMap );
				GameController.Player.DrawStats( _statConsole );
				MessageLog.Draw( _messageConsole );
				Timeline.Draw( _timeLineConsole );

				RLConsole.Blit( _mapConsole, 0, 0, MAPWIDTH, MAPHEIGHT,
					_rootConsole, 0, TIMELINEHEIGHT );
				RLConsole.Blit( _statConsole, 0, 0, STATWIDTH, STATHEIGHT,
					_rootConsole, MAPWIDTH, 0 );
				RLConsole.Blit( _messageConsole, 0, 0, MESSAGEWIDTH, MESSAGEHEIGHT,
					_rootConsole, 0, SCREENHEIGHT - MESSAGEHEIGHT );
				RLConsole.Blit( _timeLineConsole, 0, 0, TIMELINEWIDTH, TIMELINEHEIGHT,
					_rootConsole, 0, 0 );

				_rootConsole.Draw();
			
		}
	}
}
