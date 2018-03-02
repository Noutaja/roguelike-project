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
		private Direction lastDirection = Direction.None;
		public GameController GameController = Game.GameController;
		private DungeonMap map;
		private Player player = Game.GameController.Player;
		private PlayerControlState controlState;
		private Dictionary<PlayerControlState, Func<RLKey, bool>> controls;
		private bool debug = true;

		public PlayerControls( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			pressedKeys = new List<RLKey>();
			controlState = PlayerControlState.Normal;
			controls = new Dictionary<PlayerControlState, Func<RLKey, bool>>()
							{
								{ PlayerControlState.Debug, Debug },
								{ PlayerControlState.Normal, Normal },
								{ PlayerControlState.LightAttack, LightAttack },
								{ PlayerControlState.MediumAttack, MediumAttack }
			};
		}
		
		public bool CheckInput( RLKeyPress keyPress ) {
			RLKey key = keyPress.Key;
			map = GameController.CurrentMap;
			bool didPlayerAct = false;
			//check if pressedKeys is still up to date
			for ( int i = 0; i < pressedKeys.Count; i++ )
			{
				RLKey k = pressedKeys[i];
				if ( Keyboard.GetState().IsKeyUp( (Key) k ) )
				{
					pressedKeys.Remove( k );
					i--;
				}
			}
			if ( !pressedKeys.Contains( keyPress.Key ) ) { pressedKeys.Add( keyPress.Key ); }
			lastDirection = CheckDirection(key);
			controlState = CheckForState();

			foreach ( var control in controls )
			{
				if ( control.Key.Equals( controlState ) )
				{
					didPlayerAct = control.Value( key );
				}
			}

			if ( didPlayerAct )
			{
				//Add the player back into scheduling system, after taking an action(and getting a new speed)
				Game.SchedulingSystem.Add( player );
				GameController.EndPlayerTurn();
			}
			controlState = ResetState();
			return didPlayerAct;
		}
		
		private PlayerControlState ResetState() {
			return PlayerControlState.Normal;
		}

		private bool LightAttack( RLKey key ) {
			if ( IsPressed( RLKey.Keypad5 ) )
			{
				ISelfAction action = (ISelfAction) player.Actions.Find( x => x.Name == "Wait" );
				return action.Execute();
			}
			else if ( MovementPressed() )
			{
				ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Punch" );
				return action.Execute( map.GetCell( player.X, player.Y ), lastDirection );
			}
			return false;
		}

		private bool MediumAttack( RLKey key ) {
			if ( IsPressed( RLKey.Keypad5 ) )
			{
				ISelfAction action = (ISelfAction) player.Actions.Find( x => x.Name == "Wait" );
				return action.Execute();
			}
			else if ( MovementPressed() )
			{
				ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Slash" );
				return action.Execute( map.GetCell( player.X, player.Y), lastDirection );
			}
			return false;
		}

		private bool Normal( RLKey key ) {
			if ( IsPressed( RLKey.Keypad5 ) )
			{
				ISelfAction action = (ISelfAction) player.Actions.Find( x => x.Name == "Wait" );
				return action.Execute();
			}
			else if ( MovementPressed() )
			{
				return CheckMovement();
			}
			return false;
		}

		private bool Debug( RLKey key ) {
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
				Console.WriteLine( $"Map entities: {GameController.CurrentMap.Monsters.Count}" );
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

		private PlayerControlState CheckForState() {
			if ( IsPressed( RLKey.Z ) )
				return PlayerControlState.LightAttack;

			else if ( IsPressed( RLKey.X ) )
				return PlayerControlState.MediumAttack;
			

			else if ( IsPressed( RLKey.F12 ) && debug )
				return PlayerControlState.Debug;

			else
				return PlayerControlState.Normal;
		}

		private Direction CheckDirection( RLKey key ) {
			if ( MovementPressed() )
			{
				lastMovementKey = key;
			}
			else if ( Keyboard.GetState().IsKeyUp( (Key) key ) )
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

		private bool CheckMovement() {
			ICellAction action = (ICellAction) player.Actions.Find( x => x.Name == "Walk" );
			ICell cell = map.GetAdjacentCell( player.X, player.Y, lastDirection );
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

		private bool IsPressed( RLKey key ) {
			return pressedKeys.Contains( key );
		}

		private bool MovementPressed() {
			List<RLKey> movementKeys = new List<RLKey>() {  RLKey.Keypad1, RLKey.Keypad2, RLKey.Keypad3,
															RLKey.Keypad4, RLKey.Keypad6, RLKey.Keypad7,
															RLKey.Keypad8, RLKey.Keypad9
		};
			return pressedKeys.Any( x => movementKeys.Contains( x ) );
		}

		
	}
}
