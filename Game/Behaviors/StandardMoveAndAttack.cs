﻿using RLGame.Actions.BaseActions;
using RLGame.Behaviors.BaseBehaviors;
using RLGame.Core;
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
	public class StandardMoveAndAttack : Behavior, IBehavior
	{
		public bool Act( Monster monster ) {

			// If the monster has not been alerted, compute a field-of-view 
			// Use the monster's Awareness value for the distance in the FoV check
			// If the player is in the monster's FoV then alert it
			// Add a message to the MessageLog regarding this alerted status
			if ( !monster.TurnsAlerted.HasValue )
			{
				monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
				if ( monsterFov.IsInFov( player.X, player.Y ) )
				{
					Game.MessageLog.Add( $"{monster.Name} is eager to fight {player.Name}" );
					monster.TurnsAlerted = 1;
				}
			}

			if ( monster.TurnsAlerted.HasValue )
			{
				// Before we find a path, make sure to make the monster and player Cells walkable
				dungeonMap.SetIsWalkable( monster.X, monster.Y, true );
				dungeonMap.SetIsWalkable( player.X, player.Y, true );

				PathFinder pathFinder = new PathFinder( dungeonMap, 1.41 );
				Path path = null;

				try
				{
					path = pathFinder.ShortestPath(
						dungeonMap.GetCell( monster.X, monster.Y ),
						dungeonMap.GetCell( player.X, player.Y ) );
				}
				catch ( PathNotFoundException )
				{
					// The monster can see the player, but cannot find a path to him
					// This could be due to other monsters blocking the way
					// Add a message to the message log that the monster is waiting
					Game.MessageLog.Add( $"{monster.Name} waits for a turn" );
				}

				// Don't forget to set the walkable status back to false
				dungeonMap.SetIsWalkable( monster.X, monster.Y, false );
				dungeonMap.SetIsWalkable( player.X, player.Y, false );

				// In the case that there was a path, tell the monster to move
				if ( path != null )
				{
					try
					{
						if ( path.Length == 2 )
						{
							ICellAction action = (ICellAction) monster.Actions.Find( x => x.Tags.Any( y => y == ActionTag.Melee ) );
							action.Execute( path.StepForward() );
						}
						else
						{
							ICellAction action = (ICellAction) monster.Actions.Find( x => x.Name == "Walk" );
							action.Execute( path.StepForward() );
						}
					}
					catch ( NoMoreStepsException )
					{
						Game.MessageLog.Add( $"{monster.Name} growls in frustration" );
					}
				}

				monster.TurnsAlerted++;

				// Lose alerted status every 15 turns. 
				// As long as the player is still in FoV the monster will stay alert
				// Otherwise the monster will quit chasing the player.
				if ( monster.TurnsAlerted > 15 )
				{
					monster.TurnsAlerted = null;
				}
			}
			return true;
		}
	}
}
