using OpenTK.Input;
using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces;
using RLGame.Interfaces.ActionTypes;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RLGame.Systems
{
	public class PlayerControls
	{
		private readonly RLRootConsole _rootConsole;
		private List<RLKey> pressedKeys;
		private RLKey lastMovementKey = RLKey.Unknown;
		public GameController GameController = Game.GameController;
		private Player player = Game.GameController.Player;
		private DungeonMap map;
		private bool debug = true;

		public PlayerControls( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			pressedKeys = new List<RLKey>();
		}
		
		public bool CheckInput( RLKeyPress keyPress ) {
			map = GameController.CurrentMap;
			bool didPlayerAct = false;
			//check if pressedKeys is still up to date
			for ( int i = 0; i < pressedKeys.Count; i++ )
			{
				RLKey key = pressedKeys[i];
				if ( Keyboard.GetState().IsKeyUp( (Key) key ) )
				{
					pressedKeys.Remove( key );
					i--;
				}
			}
			if ( !pressedKeys.Contains( keyPress.Key ) ) { pressedKeys.Add( keyPress.Key ); }

			if ( debug ) { didPlayerAct = DebugKeys(); }

			//What is pressed
			if ( IsPressed( RLKey.Keypad5 ) )
			{
				ISelfAction action = (ISelfAction) player.Actions.Find( x => x.Name == "Wait" );
				didPlayerAct = action.Execute();
			}
			else if ( IsPressed( RLKey.Escape ) )
			{
				_rootConsole.Close();
			}
			else if ( IsPressed( RLKey.Period ) )
			{

				if ( GameController.CurrentMap.CanMoveDownToNextLevel() )
				{
					GameController.ChangeLevel( true );
					didPlayerAct = false;
				}
				else if ( GameController.CurrentMap.CanMoveUpToPreviousLevel() )
				{
					GameController.ChangeLevel( false );
					didPlayerAct = false;
				}
			}
			else if ( MovementPressed() && IsPressed( RLKey.Z ) )
			{
				ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Punch" );
				didPlayerAct = action.Execute( map.GetAdjacentCell( player.X, player.Y, CheckDirection( keyPress ) ) );
			}
			else if ( MovementPressed() )
			{
				didPlayerAct = CheckMovement( keyPress );
			}


			if ( didPlayerAct )
			{
				//Add the player back into scheduling system, after taking an action(and getting a new speed)
				Game.SchedulingSystem.Add( player );
				GameController.EndPlayerTurn();
			}
			return didPlayerAct;
		}

		private Direction CheckDirection( RLKeyPress keyPress ) {
			if ( MovementPressed() )
			{
				lastMovementKey = keyPress.Key;
			}
			else if ( Keyboard.GetState().IsKeyUp( (Key) keyPress.Key ) )
			{
				lastMovementKey = RLKey.Unknown;
			}

			switch ( lastMovementKey )
			{
				case RLKey.Keypad7:
					return Direction.UpLeft;
				case RLKey.Keypad8:
					return Direction.Up;
				case RLKey.Keypad9:
					return Direction.UpRight;
				case RLKey.Keypad4:
					return Direction.Left;
				case RLKey.Keypad6:
					return Direction.Right;
				case RLKey.Keypad1:
					return Direction.DownLeft;
				case RLKey.Keypad2:
					return Direction.Down;
				case RLKey.Keypad3:
					return Direction.DownRight;
				default:
					return Direction.None;
			}
		}

		private bool CheckMovement( RLKeyPress keyPress ) {
			ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Walk" );
			Direction direction = CheckDirection( keyPress );
			ICell cell = map.GetAdjacentCell( player.X, player.Y, direction );
			bool succ = action.Execute( cell );
			if ( succ )
			{
				if ( GameController.CurrentMap.CanMoveDownToNextLevel() )
				{
					GameController.ChangeLevel( true );
					succ = false;
				}
				else if ( GameController.CurrentMap.CanMoveUpToPreviousLevel() )
				{
					GameController.ChangeLevel( false );
					succ = false;
				}
			}
			return succ;
		}

		private bool IsPressed(RLKey key ) {
			return pressedKeys.Contains( key );
		}

		private bool MovementPressed() {
			List<RLKey> movementKeys = new List<RLKey>() {  RLKey.Keypad1, RLKey.Keypad2, RLKey.Keypad3,
															RLKey.Keypad4, RLKey.Keypad6, RLKey.Keypad7,
															RLKey.Keypad8, RLKey.Keypad9
		};
			return pressedKeys.Any( x => movementKeys.Contains( x ) );
		}

		//!DEBUG!
		private bool DebugKeys() {
			if ( IsPressed( RLKey.S ) )
			{
				Console.WriteLine( "--------------------------" );
				int i = 0;
				foreach ( var a in Game.SchedulingSystem.SCHEDULEABLES )
				{
					foreach ( IScheduleable b in a.Value )
					{
						i++;
						Console.WriteLine( $"{a.Key}: {b.GetType().ToString()}" );
					}
				}
				Console.WriteLine( "--------------------------" );
				Console.WriteLine( $"Scheduling entities: {i - 1}" );
				Console.WriteLine( $"Map entities: {GameController.CurrentMap._monsters.Count}" );
				Console.WriteLine();
			}
			else if ( IsPressed( RLKey.C ) )
			{
				GameController.ChangeLevel( true );
				return false;
			}
			else if ( IsPressed( RLKey.E ) )
			{
				GameController.CurrentMap.RevealMap();
			}
			return false;
		}
	}
}
