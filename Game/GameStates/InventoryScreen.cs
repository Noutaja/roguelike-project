using RLGame.Interfaces;
using RLGame.Systems;
using RLGame.UI;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.GameStates
{
	public class InventoryScreen : IGameState
	{
		public bool Transparent { get; }
		public bool Pauses { get; }

		private readonly int INVENTORYWIDTH = 80;
		private readonly int INVENTORYHEIGHT = 48;
		private RLConsole _inventoryConsole;

		private readonly int MESSAGEWIDTH = 80;
		private readonly int MESSAGEHEIGHT = 11;
		private RLConsole _messageConsole;

		private readonly RLRootConsole _rootConsole;

		private InventorySystem inventorySystem;
		private MenuControls menuControls;
		private InventoryUI inventoryUI;

		public InventoryScreen(bool transparent, bool pauses, RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			_inventoryConsole = new RLConsole( INVENTORYWIDTH, INVENTORYHEIGHT );
			_messageConsole = new RLConsole( MESSAGEWIDTH, MESSAGEHEIGHT );
			inventorySystem = GameController.InventorySystem;
			inventoryUI = new InventoryUI(this);
			menuControls = new MenuControls( rootConsole );
			Transparent = transparent;
			Pauses = pauses;
		}

		public void Close() {
			Game.GameStack.Pop();
		}

		public void Init() {

		}

		public void OnRender() {
			_inventoryConsole.Clear();
			_messageConsole.Clear();

			DrawInventory( _inventoryConsole );
			GameController.InventorySystem.Draw( _inventoryConsole );
			GameController.MessageLog.Draw( _messageConsole );

			RLConsole.Blit( _inventoryConsole, 0, 0, INVENTORYWIDTH, INVENTORYHEIGHT,
				_rootConsole, 0, 11 );
			RLConsole.Blit( _messageConsole, 0, 0, MESSAGEWIDTH, MESSAGEHEIGHT,
				_rootConsole, 0, Game.SCREENHEIGHT - MESSAGEHEIGHT );
		}

		public bool OnUpdate( RLKeyPress keyPress ) {
				if ( keyPress != null )
				{
					menuControls.CheckInput( keyPress );
					return true;
				}
			
			
			return false;
		}

		private void DrawInventory( RLConsole console) {

		}
	}
}
