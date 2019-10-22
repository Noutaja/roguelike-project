using RLGame.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.UI
{
	public class InventoryUI
	{
		public Menu Menu;
		private InventoryScreen _inventoryScreen;

		public InventoryUI(InventoryScreen inventoryScreen) {
			_inventoryScreen = inventoryScreen;
			Menu = new Menu();
			Menu.AddEntry(new BasicItem( Menu, "Return to game", () => { _inventoryScreen.Close(); } ) { Location = new RogueSharp.Point(0, 15)} );
		}

	}
}
