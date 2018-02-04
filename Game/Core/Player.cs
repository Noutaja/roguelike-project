using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Actions;
using RLGame.Bodyparts;
using Action = RLGame.Actions.BaseActions.Action;
using RLNET;

namespace RLGame.Core
{
	public class Player : Actor
	{
		public Player() {
			int headHealth = 15;
			int torsoHealth = 50;
			int armHealth = 15;
			int legHealth = 20;
			Bodyparts = new List<Bodypart>{
				new Torso(torsoHealth, true, 15),
				new Head(headHealth, true, 5),

				new Arm(armHealth, false, 6),
				new Arm(armHealth, false, 6),

				new Leg(legHealth, false, 7),
				new Leg(legHealth, false, 7)
			};
			Actions = new List<Action> {
				new Walk( this ),
				new Wait( this )
			};

			Attack = 10;
			AttackChance = 50;
			Awareness = 15;
			Color = Colors.Player;
			Defense = 2;
			DefenseChance = 40;
			Name = "Rogue";
			Initiative = 20;
			Speed = 1;
			Regen = 1;
			Symbol = '@';
			LastAction = new Wait(this);
		}

		override public void OnUpdateEvent( object sender, EventArgs e ) {
			if ( IsHurt() )
			{
				for ( int i = 0; i < 1000; i++ )
				{
					bool healed = false;
					Bodypart bodypart = GetBodypart( false );
					if ( bodypart.Health + Regen <= bodypart.MaxHealth )
					{
						bodypart.Health += (int) Regen;
						healed = true;
					}
					if ( healed )
						break;
				}
			}
		}

		public void DrawStats( RLConsole statConsole ) {
			statConsole.Print( 1, 1, $"Name:    {Name}", Colors.Text );
			statConsole.Print( 1, 3, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text );
			statConsole.Print( 1, 5, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text );

			int i = 0;
			foreach ( Bodypart bodypart in Bodyparts )
			{
				int yPosition = 7 + ( i * 2 );

				// Figure out the width of the health bar by dividing current health by max health
				int width = Convert.ToInt32( ( (double) bodypart.Health / (double) bodypart.MaxHealth ) * 16.0 );
				if ( width < 0 )
					width = 0;

				int remainingWidth = 16 - width;

				// Set the background colors of the health bar to show how damaged the monster is
				if (bodypart.IsVital)
				{
					statConsole.SetBackColor( 2, yPosition, width, 1, Swatch.PrimaryLighter );
				} 
				else
				{
					statConsole.SetBackColor( 2, yPosition, width, 1, Swatch.Primary );
				}
				statConsole.SetBackColor( 2 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );

				// Print the monsters name over top of the health bar
				if (bodypart.IsVital)
				{
					statConsole.Print( 3, yPosition, $"{bodypart.Name} {bodypart.Health}", Colors.TextHeading );
				}
				else
				{
					statConsole.Print( 3, yPosition, $"{bodypart.Name} {bodypart.Health}", Colors.Text );
				}

				i++;
			}
		}
	}
}
