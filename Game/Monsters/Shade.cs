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
using RLGame.Behaviors;

namespace RLGame.Monsters
{
	class Shade : Monster
	{
		public Shade( int level ) {
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int legHealth = Dice.Roll( "3D3-2" );
			Bodyparts.Add( new Torso( torsoHealth, true, 5 ) );
			Bodyparts.Add( new Head( headHealth, true, 2 ) { Strength = 5 } );
			Bodyparts.Add( new Leg( legHealth, false, 1 ) );
			Awareness = 10;
			int health = CalculateSimpleHealth();
			SimpleHealth = health;
			SimpleMaxHealth = health;
			Name = "Shade";
			Initiative = Dice.Roll( "10D7+20" );
			Speed = Game.Random.Next( 1, Initiative );
			Symbol = 's';
			Color = Colors.ShadeColor;
			Regen = 0;
			Actions.Add( new Walk( this ) );
			Actions.Add( new Wait( this ) );
			Actions.Add( new Bite( this ) { Damage = Strength } );
			LastAction = new Wait( this );
			Behaviors.Add(new HuntPlayer());
		}
	}
}
