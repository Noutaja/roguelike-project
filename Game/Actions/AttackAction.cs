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
	public class AttackAction : BaseAttack, ICellAction
	{
		public AttackAction( Actor actor ) {
			Actor = actor;
		}
		public bool Execute( ICell cell ) {
			throw new NotImplementedException();
		}

		public bool Execute( ICell cell, Direction direction ) {
			Attack attack = new Attack( Actor, Name, Damage, AttackSpeed );
			OverlayAttackPattern( cell, direction, attack );
			attack.AddHitmarker( Hitmarker );
			ModifySpeed();
			SetLastAction();
			return true;
		}
	}
}
