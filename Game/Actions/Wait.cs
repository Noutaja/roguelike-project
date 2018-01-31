using RLGame.Actions.BaseActions;
using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions
{
	public class Wait : SelfAction
	{
		public Wait(Actor actor ) {
			TimeMultiplier = 0.1;
			Actor = actor;
			Name = "Wait";
		}

		public override bool Execute() {
			ModifySpeed();
			SetLastAction();
			return true;
		}
	}
}
