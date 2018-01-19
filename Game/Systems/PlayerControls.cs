using RLGame.Core;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class PlayerControls
	{
		private readonly RLRootConsole _rootConsole;
		public CommandSystem CommandSystem = Game.CommandSystem;

		public PlayerControls( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
		}

		public bool CheckInput( RLKeyPress keyPress ) {
			bool didPlayerAct = false;
			
			if ( keyPress.Key == RLKey.Keypad7 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.UpLeft );
			}
			else if ( keyPress.Key == RLKey.Keypad8 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.Up );
			}
			else if ( keyPress.Key == RLKey.Keypad9 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.UpRight );
			}
			else if ( keyPress.Key == RLKey.Keypad4 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.Left );
			}
			else if ( keyPress.Key == RLKey.Keypad6 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.Right );
			}
			else if ( keyPress.Key == RLKey.Keypad1 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.DownLeft );
			}
			else if ( keyPress.Key == RLKey.Keypad2 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.Down );
			}
			else if ( keyPress.Key == RLKey.Keypad3 )
			{
				didPlayerAct = CommandSystem.MovePlayer( Direction.DownRight );
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
					didPlayerAct = true;
				}
				else if ( Game.CurrentMap.CanMoveUpToPreviousLevel() )
				{
					Game.ChangeLevel( false );
					didPlayerAct = true;
				}
			}
			else if ( keyPress.Key == RLKey.C )
			{
				Game.ChangeLevel( true );
				didPlayerAct = true;
			}
			else if ( keyPress.Key == RLKey.E )
			{
				Game.CurrentMap.RevealMap();
				didPlayerAct = true;
			}

			if ( didPlayerAct )
			{
				CommandSystem.EndPlayerTurn();
			}
			return didPlayerAct;
		}
	}
}
