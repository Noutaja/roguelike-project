using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces.ActionTypes;
using BaseAction = RLGame.Actions.BaseActions.BaseAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions
{
	public class Wait : BaseAction, ISelfAction
	{
		public Wait(Actor actor ) {
			TimeMultiplier = 0.1;
			Actor = actor;
			Name = "Wait";
			Tags.Add( ActionTag.Pass );
		}

		public bool Execute() {
			ModifySpeed();
			SetLastAction();
			return true;
		}
	}
}
