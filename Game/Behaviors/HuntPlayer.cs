﻿using RLGame.Behaviors.BaseBehaviors;
using RLGame.Core;
using RLGame.GameStates;
using RLGame.Interfaces;
using RLGame.Interfaces.ActionTypes;
using RLGame.Systems;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Behaviors
{
	public class HuntPlayer : Behavior, IBehavior
	{
		public bool Act( Monster monster ) {
			Initialize();

			monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
			if ( monsterFov.IsInFov( player.X, player.Y ) )
			{
				if ( path == null )
				{
					GameController.MessageLog.Add( $"{monster.Name} is eager to fight {player.Name}" );
				}
				path = CreatePath( monster );
			}
			else if ( !TurnsSincePF.HasValue )
			{
				path = CreatePath( monster );
			}

			if ( path != null )
			{
				try
				{
					ICell currentCell = dungeonMap.GetCell( monster.X, monster.Y);
					if ( path.Length == 2 )
					{
						ICellAction action = (ICellAction) monster.Actions.Find( x => x.Tags.Any( y => y == ActionTag.Melee ) );
						action.Execute( currentCell, dungeonMap.GetDirection( currentCell, path.StepForward() ) );
					}
					else
					{
						ICellAction action = (ICellAction) monster.Actions.Find( x => x.Name == "Walk" );
						action.Execute( path.StepForward() );
					}
				}
				catch ( NoMoreStepsException )
				{
					ISelfAction action = (ISelfAction) monster.Actions.Find( x => x.Tags.Any( y => y == ActionTag.Pass ) );
					action.Execute();
				}
			}
			TurnsSincePF++;
			if ( TurnsSincePF > resetTime )
			{
				TurnsSincePF = null;
			}

			return true;
		}

		private Path CreatePath( Monster monster ) {
			Path path = null;
			try
			{
				path = GetPath( monster.X, monster.Y, player.X, player.Y );
			}
			catch ( PathNotFoundException )
			{
				ISelfAction action = (ISelfAction) monster.Actions.Find( x => x.Tags.Any( y => y == ActionTag.Pass ) );
				action.Execute();
			}
			TurnsSincePF = 0;
			return path;
		}
	}
}
