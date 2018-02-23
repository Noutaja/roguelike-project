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
		public Kobold( int level ) {
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int armHealth = Dice.Roll( "2D3-1" );
			int legHealth = Dice.Roll( "3D3-2" );
			List<Bodypart> bodyparts = new List<Bodypart> {
										new Torso(torsoHealth, true, 5),
										new Head(headHealth, true, 1),

										new Arm(armHealth, false, 2, 5),
										new Arm(armHealth, false, 2, 5),

										new Leg(legHealth, false, 2),
										new Leg(legHealth, false, 2)
			};
			Bodyparts = bodyparts;
			int health = CalculateSimpleHealth();
			
			Awareness = 20;
			Color = Colors.KoboldColor;
			SimpleHealth = health;
			SimpleMaxHealth = health;
			Name = "Kobold";
			Initiative = Dice.Roll( "10D6+20" ); ;
			Speed = Game.Random.Next(1,Initiative);
			Symbol = 'k';
			Regen = 0;
			LastAction = new Wait( this );

			Actions = new List<Action> {
				new Walk( this ),
				new Wait( this ),
				new Punch( this ){Damage = 5}
			};
		}
	}
}
