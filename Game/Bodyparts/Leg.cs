using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Core;

namespace RLGame.Bodyparts
{
	public class Leg : Bodypart
	{
		public Leg( int maxHealth, bool isVital, int size ) : base( maxHealth, isVital, size ) {
			Name = "Leg";
			partType = BodypartType.Leg;
		}
	}
}
