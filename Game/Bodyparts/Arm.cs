using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Core;

namespace RLGame.Bodyparts
{
	public class Arm : Bodypart
	{
		public Arm( int maxHealth, bool isVital, int size ) : base( maxHealth, isVital, size ) {
			Name = "Arm";
			partType = BodypartType.Arm;
		}
	}
}
