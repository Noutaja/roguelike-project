using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class MessageLog
	{
		private static readonly int MAXLINES = 9;
		private readonly Queue<string> LINES;

		public MessageLog() {
			LINES = new Queue<string>();
		}

		public void Clear() {
			LINES.Clear();
		}

		public void Add(string message ) {
			LINES.Enqueue( message );
			
			if(LINES.Count > MAXLINES )
			{
				LINES.Dequeue();
			}
		}

		public void Draw(RLConsole console ) {
			string[] lines = LINES.ToArray();
			for ( int i = 0; i < lines.Length; i++ )
			{
				console.Print( 1, i + 1, lines[i], RLColor.White );
			}
		}
	}
}
