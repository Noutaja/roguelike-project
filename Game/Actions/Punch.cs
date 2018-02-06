using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces.ActionTypes;
using Action = RLGame.Actions.BaseActions.Action;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions
{
	public class Punch : AttackAction, ICellAction
	{
		public Punch( Actor actor ) {
			TimeMultiplier = 0.2;
			attackSpeed = (int) ( actor.Initiative * ( TimeMultiplier / 2 ) );
			Actor = actor;
			Name = "Punch";
			Tags.Add( ActionTag.Melee );
		}
		public bool Execute( ICell cell ) {
			Attack attack = new Attack( Actor, Name, damage, attackSpeed );
			attack.AddArea( cell );
			attack.AddHitmarker( _hitmarker );
			ModifySpeed();
			SetLastAction();
			return true;
		}
	}
}
