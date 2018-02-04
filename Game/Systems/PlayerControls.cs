using OpenTK.Input;
using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces;
using RLNET;
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
			if ( !pressedKeys.Contains( keyPress.Key ) ) { pressedKeys.Add( keyPress.Key ); }

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

			if ( movementKeys.Contains( keyPress.Key ) )
			{
				didPlayerAct = CheckMovement( keyPress );
			}
			else if ( keyPress.Key == RLKey.Keypad5 )
			{
				SelfAction wait = (SelfAction) player.Actions.Find( x => x.Name == "Wait" );
				didPlayerAct = wait.Execute();
			}
			else if ( keyPress.Key == RLKey.Escape )
			{
				_rootConsole.Close();
			}
			else if ( keyPress.Key == RLKey.Period )
			{
				if ( Game.CurrentMap.CanMoveDownToNextLevel() )
				{
					Game.ChangeLevel( true );
					//didPlayerAct = true;
				}
				else if ( Game.CurrentMap.CanMoveUpToPreviousLevel() )
				{
					Game.ChangeLevel( false );
					//didPlayerAct = true;
				}
			}
			else if ( keyPress.Key == RLKey.C )
			{
				Game.ChangeLevel( true );
				//didPlayerAct = true;
			}
			else if ( keyPress.Key == RLKey.E )
			{
				Game.CurrentMap.RevealMap();
				//didPlayerAct = true;
			}

			if ( didPlayerAct )
			{
				Game.SchedulingSystem.Add( player );
				CommandSystem.EndPlayerTurn();
			}
			return didPlayerAct;
		}

		private bool CheckMovement( RLKeyPress keyPress ) {
			if ( keyPress.Key == RLKey.Keypad7 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X - 1, player.Y - 1 ) );
			}
			else if ( keyPress.Key == RLKey.Keypad8 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X, player.Y - 1 ) );
			}
			else if ( pressedKeys.Contains( RLKey.Keypad9 ) )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X + 1, player.Y - 1 ) );
			}
			else if ( keyPress.Key == RLKey.Keypad4 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X - 1, player.Y ) );
			}
			else if ( keyPress.Key == RLKey.Keypad6 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X + 1, player.Y ) );
			}
			else if ( keyPress.Key == RLKey.Keypad1 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X - 1, player.Y + 1 ) );
			}
			else if ( keyPress.Key == RLKey.Keypad2 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X, player.Y + 1 ) );
			}
			else if ( keyPress.Key == RLKey.Keypad3 )
			{
				CellAction walk = (CellAction) player.Actions.Find( x => x.Name == "Walk" );
				return walk.Execute( map.GetCell( player.X + 1, player.Y + 1 ) );
			}

			return false;
		}
	}
}
