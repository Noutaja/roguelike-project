using RLGame.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions.BaseActions
{
	abstract public class CellAction : Action
	{
		abstract public bool Execute( ICell cell );
	}
}
