using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Actions
{
	public class Walk : CellAction
	{

		public Walk( Actor actor ) {
			TimeMultiplier = 0.3;
			Actor = actor;
			Name = "Walk";
		}

		public override bool Execute( ICell cell ) {
			int x = cell.X;
			int y = cell.Y;
			
			if ( Game.CurrentMap.SetActorPosition( Actor, x, y ) )
			{
				ModifySpeed();
				SetLastAction();
				return true;
			}

			return false;
		}
	}
}
