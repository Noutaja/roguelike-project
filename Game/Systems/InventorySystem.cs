using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class InventorySystem
	{
		private List<Item> _inventory;

		public InventorySystem() {
			_inventory = new List<Item>();
		}

		public void Init() {

		}

		public void AddItem(Item i ) {
			if(i != null)
			_inventory.Add( i );
		}

		public void RemoveItem(Item i ) {
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
	}
}
