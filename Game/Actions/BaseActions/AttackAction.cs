using RLGame.Core;
using RLGame.Helpers;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions.BaseActions
{
	abstract public class AttackAction : Action
	{
		public int Damage;
		protected int _attackSpeed;
		protected TimelineEvent _hitmarker;
		protected bool[,] _attackPattern;

		public AttackAction() {
			_hitmarker = new TimelineEvent( Colors.Blood, '*' );
		}

		protected void OverlayAttackPattern( ICell cell, Direction direction, Attack attack ) {
			bool[,] rotatedAttackPattern = Rotate( direction );
			Point center = new Point( ( rotatedAttackPattern.GetLength( 0 ) - 1 ) / 2, ( rotatedAttackPattern.GetLength( 1 ) - 1 ) / 2 );
			for ( int y = 0; y < rotatedAttackPattern.GetLength( 1 ); y++ )
			{
				for ( int x = 0; x < rotatedAttackPattern.GetLength( 0 ); x++ )
				{
					if ( rotatedAttackPattern[y, x] )
					{
						int yOffset = ( center.Y - y );
						int xOffset = ( center.X - x );
						attack.AddArea( GetCell( cell.X - xOffset, cell.Y - yOffset ) );
					}
				}
			}
		}

		protected bool[,] Rotate(Direction direction) {
			bool[,] rotatedPattern = _attackPattern;
			for ( int i = 0; i < TimesToRotate(direction); i++ )
			{
				rotatedPattern = ArrayExtensions.Rotate45( rotatedPattern );
			}
			return rotatedPattern;
		}

		private int TimesToRotate(Direction direction ) {
			switch ( direction )
			{
				case Direction.Up:
					return 0;
				case Direction.UpLeft:
					return 1;
				case Direction.Left:
					return 2;
				case Direction.DownLeft:
					return 3;
				case Direction.Down:
					return 4;
				case Direction.DownRight:
					return 5;
				case Direction.Right:
					return 6;
				case Direction.UpRight:
					return 7;
				default:
					return 0;

			}
		}
	}
}
