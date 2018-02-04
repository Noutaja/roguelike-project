using RLGame.Bodyparts;
using RLGame.Core;
using Action = RLGame.Actions.BaseActions.Action;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Actions;

namespace RLGame.Monsters
{
	public class Kobold : Monster
	{
		public Kobold(int level ) {
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int armHealth = Dice.Roll( "2D3-1" );
			int legHealth = Dice.Roll( "3D3-2" );
			List<Bodypart> bodyparts =	new List<Bodypart> {
										new Torso(torsoHealth, true, 5),
										new Head(headHealth, true, 1),

										new Arm(armHealth, false, 2),
										new Arm(armHealth, false, 2),

										new Leg(legHealth, false, 2),
										new Leg(legHealth, false, 2)
			};
			Bodyparts = bodyparts;
			int health = CalculateSimpleHealth();

			Attack = Dice.Roll( "1D3" ) + level / 3;
			AttackChance = Dice.Roll( "25D3" );
			Awareness = 20;
			Color = Colors.KoboldColor;
			Defense = Dice.Roll( "1D3" ) + level / 3;
			DefenseChance = Dice.Roll( "10D4" );
			SimpleHealth = health;
			SimpleMaxHealth = health;
			Name = "Kobold";
			Initiative = 30;
			Speed = Initiative;
			Symbol = 'k';
			Regen = 0;
			LastAction = new Wait( this );

			Actions = new List<Action> {
				new Walk( this ),
				new Wait( this )
			};
		}
	}
}
