﻿using RLGame.Core;
using RLGame.Interfaces;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class Timeline
	{
		private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;
		private int timeLineLength = 60;
		private List<Actor> _visibleActors;

		public List<Actor> VisibleActors {
			get {
				if ( !_visibleActors.Contains( Game.Player ) )
					_visibleActors.Add( Game.Player );

				return _visibleActors;
			}
			set { _visibleActors = value; }
		}


		public Timeline() {
			_scheduleables = new SortedDictionary<int, List<IScheduleable>>();
			_visibleActors = new List<Actor>();
		}

		public void Clear() {
			_scheduleables.Clear();
			VisibleActors.Clear();
		}

		//Used to remove entries too old to fit on the timeline
		public void Cull() {
			int currentTime = Game.SchedulingSystem.Time;


			while ( _scheduleables.Any() )
			{
				var oldest = _scheduleables.First();
				if ( ( currentTime - oldest.Key ) > timeLineLength )
				{
					_scheduleables.Remove( oldest.Key );
				}
				else
					break;
			}

		}

		public void Add( int time, IScheduleable scheduleable ) {
			int currentTime = Game.SchedulingSystem.Time;
			if ( scheduleable.History )
			{
				if ( scheduleable is Actor )
				{
					if ( VisibleActors.Contains( scheduleable as Actor ) )
					{
						if ( !_scheduleables.ContainsKey( time ) )
						{
							_scheduleables.Add( time, new List<IScheduleable>() );
						}
						_scheduleables[time].Add( scheduleable );
						//Remove too old lists
						Cull();
					} 
				}
				else
				{
					if ( !_scheduleables.ContainsKey( time ) )
					{
						_scheduleables.Add( time, new List<IScheduleable>() );
					}
					_scheduleables[time].Add( scheduleable );
				}
			}
		}

		public void Remove( IScheduleable scheduleable ) {
			//Create a dummy key/value pair
			KeyValuePair<int, List<IScheduleable>> scheduleableListFound
			  = new KeyValuePair<int, List<IScheduleable>>( -1, null );
			List<int> emptyKeys = new List<int>();

			foreach ( var scheduleablesList in _scheduleables )
			{
				//Search for a match
				if ( scheduleablesList.Value.Contains( scheduleable ) )
				{
					//Copy the matching pair over the dummy
					scheduleableListFound = scheduleablesList;
				}
				if ( scheduleableListFound.Value != null )
				{
					//Remove the scheduleable from the list
					scheduleableListFound.Value.Remove( scheduleable );
					//If list is now empty, add the key to be removed later
					if ( scheduleableListFound.Value.Count <= 0 )
					{
						//_scheduleables.Remove( scheduleableListFound.Key );
						emptyKeys.Add( scheduleableListFound.Key );
					}
				}
			}
			VisibleActors.Remove( scheduleable as Actor );
			//Remove empty lists
			foreach ( int key in emptyKeys )
			{
				_scheduleables.Remove( key );

			}

		}

		public void Draw( RLConsole console ) {
			int topPadding = 3;
			int leftPadding = 10;
			int currentTime = Game.SchedulingSystem.Time;
			DrawGraph( console, topPadding, leftPadding );

			Cull();
			DrawEvents( console, topPadding, leftPadding, currentTime );
			DrawActors( console, topPadding, leftPadding, currentTime );
		}

		private void DrawEvents( RLConsole console, int topPadding, int leftPadding, int currentTime ) {
			//Begin with the newest entry
			foreach ( var scheduleables in _scheduleables.Reverse() )
			{
				//Skip the first entry(ongoing turn)
				//if ( scheduleables.Key != currentTime )
				{
					List<IScheduleable> scheduleableList = scheduleables.Value;
					int scheduleableTime = scheduleables.Key;
					
					int timeDifference = Game.SchedulingSystem.Time - scheduleableTime;
					int xPosition = ( timeLineLength + leftPadding ) - ( timeDifference );

					foreach ( IScheduleable scheduleable in scheduleableList )
					{
						if ( scheduleable is TimelineEvent )
						{
							TimelineEvent e = scheduleable as TimelineEvent;
							console.Print( xPosition, topPadding + 1, e.Symbol.ToString(), e.Color );
						}
					}
				}
			}
		}

		private void DrawActors( RLConsole console, int topPadding, int leftPadding, int currentTime ) {
			bool mostRecent = true;
			//Begin with the newest entry
			foreach ( var scheduleables in _scheduleables.Reverse() )
			{
				//Skip the first entry(ongoing turn)
				if ( scheduleables.Key != currentTime )
				{
					List<IScheduleable> scheduleableList = scheduleables.Value;
					int scheduleableTime = scheduleables.Key;

					int i = 0;
					int timeDifference = Game.SchedulingSystem.Time - scheduleableTime;
					int xPosition = ( timeLineLength + leftPadding ) - ( timeDifference );

					

					foreach ( IScheduleable scheduleable in scheduleableList )
					{
						if ( scheduleable is Actor )
						{
							Actor actor = scheduleable as Actor;
							//Draw on the timeline only if the actor is visible
							if ( VisibleActors.Contains( actor ) )
							{
								console.Print( xPosition, topPadding + 2 + i, actor.Symbol.ToString(), actor.Color );

								if ( mostRecent )
								{
									console.Print( xPosition, topPadding - 1 + i, timeDifference.ToString(), RLColor.White );
								}
								//Add the key of only the most recent list above the timeline
								if ( mostRecent )
								{
									console.Print( xPosition + 1, topPadding + 2 + i, ": " + actor.LastAction.Name, RLColor.White );
								}
								i++;
							}
						}
					}
					if ( scheduleableList.OfType<Actor>().Any() )
					{
						mostRecent = false;
					}
				}
			}
		}

		private void DrawGraph( RLConsole console, int topPadding, int leftPadding ) {
			int tmp = 0;
			for ( int i = leftPadding; i < timeLineLength + leftPadding; i++ )
			{
				if ( i % 10 == 0 )
					console.Print( i, topPadding - 2, ( timeLineLength - tmp ).ToString(), RLColor.White );

				tmp++;
			}
			console.Print( 1, topPadding, "Timeline:", RLColor.White );
			for ( int i = leftPadding; i < timeLineLength + leftPadding; i++ )
			{
				if ( i % 10 == 0 )
					console.Print( i, topPadding, "|", RLColor.White );
				else
					console.Print( i, topPadding, "-", RLColor.White );
			}
			console.Print( timeLineLength + leftPadding, topPadding, "0", RLColor.White );
		}
	}
}
