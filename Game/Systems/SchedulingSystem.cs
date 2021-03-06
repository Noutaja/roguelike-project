﻿using RLGame.Core;
using RLGame.GameStates;
using RLGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class SchedulingSystem
	{
		public int Time { get; private set; }
		public Update update;
		public readonly SortedDictionary<int, List<IScheduleable>> SCHEDULEABLES;
		public SchedulingSystem() {
			Time = 0;
			SCHEDULEABLES = new SortedDictionary<int, List<IScheduleable>>();
			update = new Update();
			Add( update );
		}

		// Add a new object to the schedule 
		// Place it at the current time plus the object's Time property.
		public void Add( IScheduleable scheduleable ) {

			int key = Time + scheduleable.Time;
			if ( !SCHEDULEABLES.ContainsKey( key ) )
			{
				SCHEDULEABLES.Add( key, new List<IScheduleable>() );
			}
			SCHEDULEABLES[key].Add( scheduleable );
		}

		public Boolean Contains( IScheduleable scheduleable ) {
			foreach ( var scheduleablesList in SCHEDULEABLES )
			{
				if ( scheduleablesList.Value.Contains( scheduleable ) )
				{
					return true;
				}
			}
			return false;
		}

		// Remove a specific object from the schedule.
		public void Remove( IScheduleable scheduleable, bool removeFromTimeline ) {
			//Create a dummy key/value pair
			KeyValuePair<int, List<IScheduleable>> scheduleableListFound
			  = new KeyValuePair<int, List<IScheduleable>>( -1, null );

			foreach ( var scheduleablesList in SCHEDULEABLES )
			{
				//Search for a match
				if ( scheduleablesList.Value.Contains( scheduleable ) )
				{
					//Copy the matching pair over the dummy
					scheduleableListFound = scheduleablesList;
					break;
				}
			}
			if ( scheduleableListFound.Value != null )
			{
				//Remove the scheduleable from the timeline
				if ( removeFromTimeline )
				{
					GameController.Timeline.Remove( scheduleable );
				}

				//Remove the scheduleable from the list
				scheduleableListFound.Value.Remove( scheduleable );
				//If list is now empty, remove the list
				if ( scheduleableListFound.Value.Count <= 0 )
				{
					SCHEDULEABLES.Remove( scheduleableListFound.Key );
				}
			}
		}

		// Get the next object whose turn it is from the schedule. Advance time if necessary
		public IScheduleable Get() {
			var firstScheduleableGroup = SCHEDULEABLES.First();
			var firstScheduleable = firstScheduleableGroup.Value.First();
			GameController.Timeline.Add( firstScheduleableGroup.Key, firstScheduleable );
			Remove( firstScheduleable, false );
			Time = firstScheduleableGroup.Key;
			return firstScheduleable;
		}

		// Reset the time and clear out the schedule
		public void Clear() {
			Time = 0;
			SCHEDULEABLES.Clear();
		}
	}
}
