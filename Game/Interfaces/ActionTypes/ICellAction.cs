using RLGame.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces.ActionTypes
{
	public interface ICellAction
	{
		bool Execute( ICell cell );
	}
}
