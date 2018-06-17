using RLGame.Core;
using RLGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Bodyparts
{
	public class Torso : Bodypart {
		public Torso( int maxHealth, bool isVital, int size ) : base( maxHealth, isVital, size ) {
			Name = "Torso";
			PartType = BodypartType.Torso;
		}
	}
}
