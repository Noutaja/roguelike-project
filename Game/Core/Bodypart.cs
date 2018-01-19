using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Interfaces;

namespace RLGame.Core
{
	public class Bodypart : IBodypart
	{
		private int _health { get; set; }
		public BodypartType partType { get; set; }
		public string Name { get; set; }
		public int Health {
			get { return _health; }
			set {
				_health = value;
				if ( _health > MaxHealth )
					_health = MaxHealth;
			}
		}
		public int MaxHealth { get; set; }
		public bool IsVital { get; set; }
		public int Size { get; set; }

		public Bodypart(int maxHealth, bool isVital, int size) {
			MaxHealth = maxHealth;
			Health = maxHealth;
			IsVital = isVital;
			Size = size;
		}
	}
}
