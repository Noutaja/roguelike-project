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
	class Slash : AttackAction, ICellAction
	{
		public Slash( Actor actor ) {
			TimeMultiplier = 0.3;
			_attackSpeed = (int) ( actor.Initiative * ( TimeMultiplier / 2 ) );
			_attackPattern = Prototypes.AttackPatterns.FrontHorizontal3();
			Actor = actor;
			Name = "Slash";
			Tags.Add( ActionTag.Melee );
		}

		public bool Execute( ICell cell ) {
			throw new Exception( "This attack cannot be called with one parameter!" );
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
