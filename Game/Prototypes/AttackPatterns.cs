using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Prototypes
{
	public static class AttackPatterns
	{
		public static bool[,] Basic() {
			return new bool[,]
				{
					{ false, true, false },
					{ false, false, false },
					{ false, false, false }
				};
		}

		public static bool[,] Horizontal3() {
			return new bool[,] 
				{
					{ true, true, true },
					{ false, false, false },
					{ false, false, false }
				};
		}
	}
}
