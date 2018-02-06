using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces.ActionTypes
{
	public interface ITargetAction
	{
		bool Execute( Actor actor );
	}
}
