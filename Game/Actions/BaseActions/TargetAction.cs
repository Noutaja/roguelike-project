using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions.BaseActions
{
	abstract public class TargetAction : Action
	{
		abstract public bool Execute( Actor actor );
	}
}
