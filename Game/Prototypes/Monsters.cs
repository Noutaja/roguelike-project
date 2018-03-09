using RLGame.Actions;
using RLGame.Behaviors;
using RLGame.Bodyparts;
using RLGame.Core;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Prototypes
{
	public static class Monsters
	{
		public static Monster Shade() {
			Monster monster = new Monster();
			int headHealth = Dice.Roll( "1D5" );
			int torsoHealth = Dice.Roll( "2D5" );
			int legHealth = Dice.Roll( "3D3-2" );
			monster.Bodyparts.Add( new Torso( torsoHealth, true, 5 ) );
			monster.Bodyparts.Add( new Head( headHealth, true, 2 ) { Strength = 5 } );
			monster.Bodyparts.Add( new Leg( legHealth, false, 1 ) );
			monster.Awareness = 10;
			int health = monster.CalculateSimpleHealth();
			monster.SimpleHealth = health;
			monster.SimpleMaxHealth = health;
			monster.Name = "Shade";
			monster.Initiative = Dice.Roll( "10D7+20" );
			monster.Speed = Game.Random.Next( 1, monster.Initiative );
			monster.Symbol = 's';
			monster.Color = Colors.ShadeColor;
			monster.Regen = 0;
			monster.Actions.Add( new Walk( monster ) );
			monster.Actions.Add( new Wait( monster ) );
			monster.Actions.Add( Prototypes.Attacks.Bite( monster ) );
			monster.LastAction = new Wait( monster );
			monster.Behaviors.Add( new HuntPlayer() );
			return monster;
		}
	}
}
