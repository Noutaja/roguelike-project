using RLGame.Core;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class InventorySystem
	{
		private readonly List<Item> _inventory;

		public InventorySystem() {
			_inventory = new List<Item>();
		}

		public void Init() {

		}

		public List<Item> Inventory() {
			List<Item> i = new List<Item>();
			foreach ( Item item in _inventory )
			{
				i.Add( item );
			}
			return i;
		}

		public void AddItem( Item i ) {
			if ( i != null )
				_inventory.Add( i );

			GameController.MessageLog.Add( $"Picked up an item." );
		}

		public void RemoveItem( Item i ) {
			_inventory.Remove( i );
		}

		public int ItemCount() {
			return _inventory.Count;
		}

		public int WeightTotal() {
			int weight = 0;
			foreach ( Item i in _inventory )
			{
				weight += i.Weight;
			}
			return weight;
		}

		public void Draw( RLConsole console ) {
			int topPadding = 1;
			int sidePadding = 1;
			int i = 0;

			foreach ( Item item in _inventory )
			{
				console.Print( sidePadding, topPadding + i, item.ToString(), Colors.Text );
				i++;
			}
		}
	}
}
