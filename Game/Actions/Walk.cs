﻿using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces;
using RogueSharp;
using Action = RLGame.Actions.BaseActions.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Interfaces.ActionTypes;

namespace RLGame.Actions
{
	public class Walk : Action, ICellAction
	{

		public Walk( Actor actor ) {
			TimeMultiplier = 0.3;
			Actor = actor;
			Name = "Walk";
			Tags.Add( ActionTag.Movement );
		}

		public bool Execute( ICell cell ) {
			if ( cell != null )
			{
				int x = cell.X;
				int y = cell.Y;

				if ( Game.GameController.CurrentMap.SetActorPosition( Actor, x, y ) )
				{
					ModifySpeed();
					SetLastAction();
					return true;
				}
			}
				return false; 
		}
	}
}
