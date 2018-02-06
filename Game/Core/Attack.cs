using RLGame.Interfaces;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Core
{
	public class Attack : IScheduleable
	{
		private string _name;
		private Actor _attacker;
		private List<Actor> _targets;
		private List<ICell> _area;
		private TimelineEvent _hitmarker;
		private int _damage;

		public Attack( Actor attacker, string name, int damage, int speed ) {
			_attacker = attacker;
			_name = name;
			_targets = new List<Actor>();
			_area = new List<ICell>();
			_damage = damage;
			_speed = speed;
			Game.SchedulingSystem.Add( this );
		}

		public void Activate() {
			//Check if anything is hit
			foreach ( ICell cell in _area )
			{
				Actor actor = GetActorAt( cell.X, cell.Y );
				if ( actor != null )
				{
					_targets.Add( actor );
				}
			}
			//Deal damage
			if ( !_targets.Any() )
			{
				Game.MessageLog.Add( $"  {_attacker.Name}'s {_name} missed" );
				return;
			}

				foreach(Actor defender in _targets )
				{
					Bodypart bodypart = defender.TakeDamage( _damage, defender.GetBodypart( true ) );
					Game.MessageLog.Add( $"  {defender.Name}'s {bodypart.Name} was hit for {_damage} damage" );

					if(defender is Player )
					{
						Game.Timeline.Add(Game.SchedulingSystem.Time, _hitmarker );
					}

					if ( defender.IsDying() )
					{
						if ( defender is Player )
						{
							Game.MessageLog.Add( $"  {defender.Name} was killed, GAME OVER MAN!" );
						}
						else if ( defender is Monster )
						{
							Game.CurrentMap.RemoveMonster( (Monster) defender );

							Game.MessageLog.Add( $"  {defender.Name} died." );
						}
					}
				}
			
		}

		public void AddArea( ICell cell ) {
			_area.Add( cell );
		}

		public void AddHitmarker(TimelineEvent hitmarker) {
			_hitmarker = hitmarker;
			_hitmarker.Name = _name;
		}

		private Actor GetActorAt( int x, int y ) {
			DungeonMap map = Game.CurrentMap;
			return map.GetActorAt( x, y );
		}

		//IScheduleable
		private int _speed;
		public int Time {
			get { return _speed; }
		}
		public bool History {
			get { return false; }
		}
	}
}
