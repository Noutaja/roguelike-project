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
		public CommandSystem CommandSystem = Game.CommandSystem;
		private Player player = Game.Player;
		private DungeonMap map;

		private List<RLKey> movementKeys = new List<RLKey>() {  RLKey.Keypad1,
																RLKey.Keypad2,
																RLKey.Keypad3,
																RLKey.Keypad4,
																RLKey.Keypad6,
																RLKey.Keypad7,
																RLKey.Keypad8,
																RLKey.Keypad9
		};

		public PlayerControls( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			pressedKeys = new List<RLKey>();
		}

		public bool CheckInput( RLKeyPress keyPress ) {
			map = Game.CurrentMap;
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
			
			if ( pressedKeys.Any( x => movementKeys.Contains( x ) ) && pressedKeys.Contains( RLKey.Z ) )
			{
				ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Punch" );
				didPlayerAct = action.Execute( map.GetAdjacentCell( player.X, player.Y, CheckDirection( keyPress ) ) );
			}
			else if ( movementKeys.Contains( keyPress.Key ) )
			{
				didPlayerAct = CheckMovement( keyPress );
			}
			else if ( pressedKeys.Contains( RLKey.Keypad5 ) )
			{
				ISelfAction action = (ISelfAction) player.Actions.Find( x => x.Name == "Wait" );
				didPlayerAct = action.Execute();
			}
			else if ( pressedKeys.Contains( RLKey.Escape ) )
			{
				_rootConsole.Close();
			}
			else if ( pressedKeys.Contains( RLKey.Period ) )
			{
				if ( Game.CurrentMap.CanMoveDownToNextLevel() )
				{
					Game.ChangeLevel( true );
				}
				else if ( Game.CurrentMap.CanMoveUpToPreviousLevel() )
				{
					Game.ChangeLevel( false );
				}
			}
			else if ( pressedKeys.Contains( RLKey.C ) )
			{
				Game.ChangeLevel( true );
			}
			else if ( pressedKeys.Contains( RLKey.E ) )
			{
				Game.CurrentMap.RevealMap();
			}

			if ( didPlayerAct )
			{
				//Add the player back into scheduling system, after taking an action(and getting a new speed)
				Game.SchedulingSystem.Add( player );
				CommandSystem.EndPlayerTurn();
			}
			return didPlayerAct;
		}

		private Direction CheckDirection( RLKeyPress keyPress ) {
			if ( movementKeys.Contains( keyPress.Key ) )
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
			return action.Execute( map.GetAdjacentCell( player.X, player.Y, direction ) );
		}
	}
}
