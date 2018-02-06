using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Core;
using RLGame.Interfaces;

namespace RLGame.Bodyparts
{
	class Head : Bodypart
	{
		public int Strength = 1;
		public Head( int maxHealth, bool isVital, int size) : base( maxHealth, isVital, size ) {
			Name = "Head";
			partType = BodypartType.Head;
		}
	}
}
