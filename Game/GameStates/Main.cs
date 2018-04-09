using OpenTK.Input;
using RLGame.Core;
using RLGame.Interfaces;
using RLGame.Systems;
using RLNET;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weighted_Randomizer;

namespace RLGame.GameStates
{
	public class Main : IGameState {
		public bool Transparent { get; }
		public bool Pauses { get; }
		
		private RLRootConsole _rootConsole;

		private readonly int MAPWIDTH = 80;
		private readonly int MAPHEIGHT = 48;
		private RLConsole _mapConsole;

		private readonly int MESSAGEWIDTH = 80;
		private readonly int MESSAGEHEIGHT = 11;
		private RLConsole _messageConsole;

		private readonly int STATWIDTH = 20;
		private readonly int STATHEIGHT = 70;
		private RLConsole _statConsole;

		private readonly int TIMELINEWIDTH = 80;
		private readonly int TIMELINEHEIGHT = 11;
		private RLConsole _timeLineConsole;

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
		public static DungeonMap CurrentMap { get; private set; }
		public static List<DungeonMap> Maps { get; private set; }

		public Main(bool transparent, bool pause) {
			Transparent = transparent;
			Pauses = pause;
			_mapConsole = new RLConsole( MAPWIDTH, MAPHEIGHT );
			_messageConsole = new RLConsole( MESSAGEWIDTH, MESSAGEHEIGHT );
			_statConsole = new RLConsole( STATWIDTH, STATHEIGHT );
			_timeLineConsole = new RLConsole( TIMELINEWIDTH, TIMELINEHEIGHT );

			Maps = new List<DungeonMap>();
			SchedulingSystem = new SchedulingSystem();
			GameController = new GameController( MAPWIDTH, MAPHEIGHT );
			Timeline = new Timeline();
			MessageLog = new MessageLog();
			PlayerControls = new PlayerControls( _rootConsole );
		}

		public void Init( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			GameController.Init();
		}

		public void Close() {
		}

		public bool OnUpdate( RLKeyPress keyPress ) {
			if ( GameController.IsPlayerTurn )
			{
				if ( keyPress != null )
				{
					PlayerControls.CheckInput( keyPress );
					return true;
				}
			}
			else
			{
				GameController.AdvanceTime();
			}
			return false;
		}

		public void OnRender() {

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
				_rootConsole, 0, Game.SCREENHEIGHT - MESSAGEHEIGHT );
			RLConsole.Blit( _timeLineConsole, 0, 0, TIMELINEWIDTH, TIMELINEHEIGHT,
				_rootConsole, 0, 0 );

		}
	}
}
