using RLGame.Interfaces;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Core
{
	public class TimelineEvent : IScheduleable
	{
		public int Time { get; set; }
		public bool History { get { return true; } }

		public RLColor Color { get; set; }
		public char Symbol { get; set; }

		public string Name;

		public TimelineEvent( RLColor color, char symbol) {
			Time = 0;
			Color = color;
			Symbol = symbol;
		}
	}
}
