using RLGame.Interfaces;
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

		private RLRootConsole _rootConsole;

		public InventoryScreen(bool transparent, bool pauses, RLRootConsole rootConsole ) {
			_rootConsole = rootConsole;
			Transparent = transparent;
			Pauses = pauses;
		}

		public void Close() {
			
		}

		public void Init() {
		}

		public void OnRender() {
		}

		public bool OnUpdate( RLKeyPress keyPress ) {
			return true;
		}
	}
}
