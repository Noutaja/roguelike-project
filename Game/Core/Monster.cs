using RLGame.Behaviors;
using RLGame.Interfaces;
using RLGame.Systems;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Core
{
	public class Monster : Actor
	{
		public int? TurnsAlerted { get; set; }
		public bool IsVisible { get; set; }

		public int SimpleHealth { get; set; }
		public int SimpleMaxHealth { get; set; }

		public List<IBehavior> Behaviors { get; set; }

		public Monster() {
			Behaviors = new List<IBehavior>();
		}

		public virtual void Activate() {
			var behavior = Behaviors.FirstOrDefault();
			behavior.Act( this );
		}

		public void DrawStats( RLConsole statConsole, int position ) {
			SimpleHealth = CalculateSimpleHealth();
			// Start at Y=13 which is below the player stats.
			// Multiply the position by 2 to leave a space between each stat
			int yPosition = 25 + ( position * 2 );

			// Begin the line by printing the symbol of the monster in the appropriate color
			statConsole.Print( 1, yPosition, Symbol.ToString(), Color );

			// Figure out the width of the health bar by dividing current health by max health
			int width = Convert.ToInt32( ( (double)SimpleHealth / (double)SimpleMaxHealth ) * 16.0 );
			int remainingWidth = 16 - width;

			// Set the background colors of the health bar to show how damaged the monster is
			statConsole.SetBackColor( 3, yPosition, width, 1, Swatch.Primary );
			statConsole.SetBackColor( 3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );

			// Print the monsters name over top of the health bar
			statConsole.Print( 2, yPosition, $": {Name} {SimpleHealth}", Swatch.DbLight );
		}

		public virtual int CalculateSimpleHealth() {
			int hp = 0;
			foreach ( Bodypart bodypart in Bodyparts )
			{
				if ( bodypart.IsVital )
				{
					hp += bodypart.Health;
				}
			}
			return hp;
		}
	}
}
