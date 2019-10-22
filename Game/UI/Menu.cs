using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace RLGame.UI
{
	public class Menu
	{
		private List<MenuItem> _entries;
		public MenuItem SelectedItem;
		public int Width;

		public Menu() {
			_entries = new List<MenuItem>();
		}

		public void AddEntry(MenuItem entry ) {
			_entries.Add( entry );
			if(entry.Width > Width ) { Width = entry.Width; }
		}

		public void RemoveEntry(MenuItem entry ) {
			_entries.Remove( entry );
			if ( entry.Width == Width )
			{
				Width = 0;
				foreach ( MenuItem item in _entries )
				{
					if ( item.Width > Width ) { Width = item.Width; }
				} 
			}
		}

		public void Next() {
			int i = ( _entries.IndexOf( SelectedItem ) + 1 ) % _entries.Count();
			SelectedItem = _entries[i];
		}

		public void Previous() {
			int i = ( _entries.IndexOf( SelectedItem ) - 1 ) % _entries.Count();
			SelectedItem = _entries[i];
		}


		public void Draw( RLConsole console ) {
			for (int i = 0; i < _entries.Count; i++ )
			{
				MenuItem item = _entries[i];
				item.Draw( console, i );
			}
		}
	}
}
