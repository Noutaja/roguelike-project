using RLGame.Actions;
using RLGame.Bodyparts;
using RLGame.Core;
using Action = RLGame.Actions.BaseActions.Action;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Monsters
{
	class Shade : Monster
	{
		public Shade( int level ) {
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int legHealth = Dice.Roll( "3D3-2" );
			List<Bodypart> bodyparts = new List<Bodypart> {
										new Torso(torsoHealth, true, 5),
										new Head(headHealth, true, 1){ Strength = 5 },

										new Leg(legHealth, false, 2)
			};
			Bodyparts = bodyparts;
			int health = CalculateSimpleHealth();

			Awareness = 10;
			Color = Colors.ShadeColor;
			SimpleHealth = health;
			SimpleMaxHealth = health;
			Name = "Shade";
			Initiative = Dice.Roll( "10D7+20" ); ;
			Speed = Game.Random.Next( 1, Initiative );
			Symbol = 's';
			Regen = 0;
			LastAction = new Wait( this );

			Actions = new List<Action> {
				new Walk( this ),
				new Wait( this ),
				new Bite( this ){damage = Strength}
			};
		}
	}
}
