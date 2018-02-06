using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions.BaseActions
{
	abstract public class AttackAction : Action
	{
		public int damage;
		protected int attackSpeed;
		protected TimelineEvent _hitmarker;

		public AttackAction() {
			_hitmarker = new TimelineEvent( Colors.Blood, '*' );
		}
	}
}
