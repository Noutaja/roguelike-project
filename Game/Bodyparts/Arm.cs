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
		public int Strength = 0;
		public Arm( int maxHealth, bool isVital, int size, int strength ) : base( maxHealth, isVital, size ) {
			Name = "Arm";
			PartType = BodypartType.Arm;
			Strength = strength;
		}
	}
}
