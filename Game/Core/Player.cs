using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Actions;
using RLGame.Bodyparts;
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
			Bodyparts.Add( new Torso( torsoHealth, true, 15 ) );
			Bodyparts.Add( new Head( headHealth, true, 4 ) );

			Bodyparts.Add( new Arm( armHealth, false, 6, 10 ) { Name = "Left Arm" } );
			Bodyparts.Add( new Arm( armHealth, false, 6, 10 ) { Name = "Right Arm" } );

			Bodyparts.Add( new Leg( legHealth, false, 7 ) { Name = "Left Leg" } );
			Bodyparts.Add( new Leg( legHealth, false, 7 ) { Name = "Right Leg" } );


			Awareness = 15;
			Color = Colors.Player;
			Name = "Adventurer";
			Initiative = 50;
			Speed = 1;
			Regen = 1;
			Symbol = '@';
			Actions.Add( new Walk( this ) );
			Actions.Add( new Wait( this ) );
			Actions.Add( Prototypes.Attacks.Punch(this) );
			Actions.Add( Prototypes.Attacks.Slash(this) );
			LastAction = new Wait( this );
		}

		override public void OnUpdateEvent( object sender, EventArgs e ) {
			if ( IsHurt() )
			{
				for ( int i = 0; i < 1000; i++ )
				{
					bool healed = false;
					Bodypart bodypart = GetBodypart( false );
					int newHealth = bodypart.Health + (int) Regen;
					if ( newHealth <= bodypart.MaxHealth && bodypart.Health > 0 )
					{
						bodypart.Health = newHealth;
						healed = true;
					}
					if ( healed )
						break;
				}
			}
		}

		public void DrawStats( RLConsole statConsole ) {
			statConsole.Print( 1, 1, $"Name: {Name}", Colors.Text );
			statConsole.Print( 1, 3, $"Strength: {Strength}", Colors.Text );

			int i = 0;
			foreach ( Bodypart bodypart in Bodyparts )
			{
				int yPosition = 5 + ( i * 2 );

				// Figure out the width of the health bar by dividing current health by max health
				int width = Convert.ToInt32( ( (double) bodypart.Health / (double) bodypart.MaxHealth ) * 16.0 );
				if ( width < 0 )
					width = 0;

				int remainingWidth = 16 - width;

				// Set the background colors of the health bar to show how damaged the monster is
				if ( bodypart.IsVital )
				{
					statConsole.SetBackColor( 2, yPosition, width, 1, Swatch.PrimaryLighter );
				}
				else
				{
					statConsole.SetBackColor( 2, yPosition, width, 1, Swatch.Primary );
				}
				statConsole.SetBackColor( 2 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );

				// Print the monsters name over top of the health bar
				if ( bodypart.IsVital )
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
