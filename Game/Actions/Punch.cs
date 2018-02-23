﻿using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces.ActionTypes;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Prototypes;

namespace RLGame.Actions
{
	public class Punch : AttackAction, ICellAction
	{
		public Punch( Actor actor ) {
			TimeMultiplier = 0.2;
			_attackSpeed = (int) ( actor.Initiative * ( TimeMultiplier / 2 ) );
			Actor = actor;
			Name = "Punch";
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
			throw new NotImplementedException();
		}
	}
}
