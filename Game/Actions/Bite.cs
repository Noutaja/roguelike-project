using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces.ActionTypes;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions
{
	public class Bite : AttackAction, ICellAction
	{
		public Bite( Actor actor ) {
			TimeMultiplier = 0.2;
			_attackSpeed = (int) ( actor.Initiative * ( TimeMultiplier / 2 ) );
			_attackPattern = Prototypes.AttackPatterns.Basic();
			Actor = actor;
			Name = "Bite";
			Tags.Add( ActionTag.Melee );
		}
		public bool Execute( ICell cell ) {
			Attack attack = new Attack( Actor, Name, Damage, _attackSpeed );
			attack.AddArea( cell );
			attack.AddHitmarker( _hitmarker );
			ModifySpeed();
			SetLastAction();
			return true;
		}

		public bool Execute( ICell cell, Direction direction ) {
			Attack attack = new Attack( Actor, Name, Damage, _attackSpeed );
			OverlayAttackPattern( cell, direction, attack );
			attack.AddHitmarker( _hitmarker );
			ModifySpeed();
			SetLastAction();
			return true;
		}
	}
}
