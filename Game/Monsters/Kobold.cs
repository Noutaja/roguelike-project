using RLGame.Bodyparts;
using RLGame.Core;
using RLGame.Interfaces;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Monsters
{
	public class Kobold : Monster
	{
		public static Kobold Create( int level ) {
			Kobold kobold = new Kobold();
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int armHealth = Dice.Roll( "2D3-1" );
			int legHealth = Dice.Roll( "3D3-2" );
			List<Bodypart> bodyparts = new List<Bodypart> {
				new Torso(torsoHealth, true, 5),
				new Head(headHealth, true, 1),

				new Arm(armHealth, false, 2),
				new Arm(armHealth, false, 2),

				new Leg(legHealth, false, 2),
				new Leg(legHealth, false, 2)


			};

			kobold.Bodyparts = bodyparts;
			int health = kobold.CalculateSimpleHealth();

			kobold.Attack = Dice.Roll( "1D3" ) + level / 3;
			kobold.AttackChance = Dice.Roll( "25D3" );
			kobold.Awareness = 20;
			kobold.Color = Colors.KoboldColor;
			kobold.Defense = Dice.Roll( "1D3" ) + level / 3;
			kobold.DefenseChance = Dice.Roll( "10D4" );
			kobold.SimpleHealth = health;
			kobold.SimpleMaxHealth = health;
			kobold.Name = "Kobold";
			kobold.Speed = 30;
			kobold.Symbol = 'k';
			kobold.Regen = 0;
			return kobold;
		}
	}
}
