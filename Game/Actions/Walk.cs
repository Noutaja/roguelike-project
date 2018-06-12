using RLGame.Actions.BaseActions;
using RLGame.Core;
using RLGame.Interfaces;
using RogueSharp;
using BaseAction = RLGame.Actions.BaseActions.BaseAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Interfaces.ActionTypes;
using RLGame.GameStates;

namespace RLGame.Actions
{
	public class Walk : BaseAction, ICellAction
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

				if ( MainScreen.GameController.CurrentMap.SetActorPosition( Actor, x, y ) )
				{
					ModifySpeed();
					SetLastAction();
					return true;
				}
			}
				return false; 
		}

		public bool Execute( ICell cell, Direction direction ) {
			return Execute( cell );
		}
	}
}
