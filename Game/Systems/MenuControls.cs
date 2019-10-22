using OpenTK.Input;
using RLGame.UI;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class MenuControls
	{
		private RLRootConsole _rootConsole;
		private List<RLKey> pressedKeys;
		private Menu _currentMenu;

		public MenuControls( RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			pressedKeys = new List<RLKey>();
		}

		public bool CheckInput( RLKeyPress keyPress ) {
			RLKey key = keyPress.Key;
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
			//Add currently pressed key to the list
			if ( !pressedKeys.Contains( key ) ) { pressedKeys.Add( key ); }

			MenuKeys( key );
			SystemKeys( key );
			return true;
		}

		private bool MenuKeys( RLKey key ) {
			if ( IsPressed( RLKey.Up ) )
			{
				_currentMenu.Previous();
			}
			else if ( IsPressed( RLKey.Down ) )
			{
				_currentMenu.Next();
			}
			return false;
		}

		private bool SystemKeys( RLKey key ) {
			if ( IsPressed( RLKey.Escape ) )
			{
				_rootConsole.Close();
			}
			return false;
		}

		private bool IsPressed( RLKey key ) {
			return pressedKeys.Contains( key );
		}

		public void SetCurrentMenu(Menu menu ) {
			_currentMenu = menu;
		}
	}
}
