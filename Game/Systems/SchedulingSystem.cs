using RLGame.Core;
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
		public int Time;
		public Update update;
		private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

		public SchedulingSystem() {
			Time = 0;
			_scheduleables = new SortedDictionary<int, List<IScheduleable>>();
			update = new Update();
			Add( update );
		}

		// Add a new object to the schedule 
		// Place it at the current time plus the object's Time property.
		public void Add( IScheduleable scheduleable ) {
			
			int key = Time + scheduleable.Time;
			if ( !_scheduleables.ContainsKey( key ) )
			{
				_scheduleables.Add( key, new List<IScheduleable>() );
			}
			_scheduleables[key].Add( scheduleable );
		}

		public Boolean Contains(IScheduleable scheduleable ) {
			foreach (var scheduleablesList in _scheduleables)
			{
				if ( scheduleablesList.Value.Contains( scheduleable ) )
				{
					return true;
				}
			}
			return false;
		}

		// Remove a specific object from the schedule.
		public void Remove( IScheduleable scheduleable ) {
			//Create a dummy key/value pair
			KeyValuePair<int, List<IScheduleable>> scheduleableListFound
			  = new KeyValuePair<int, List<IScheduleable>>( -1, null );

			foreach ( var scheduleablesList in _scheduleables )
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
				//Remove the scheduleable from the list
				scheduleableListFound.Value.Remove( scheduleable );
				//If list is now empty, remove the list
				if ( scheduleableListFound.Value.Count <= 0 )
				{
					_scheduleables.Remove( scheduleableListFound.Key );
				}
			}
		}

		// Remove a specific object from the schedule AND timeline.
		// Used for when an monster is killed to remove it before it's action comes up again.
		public void Remove( IScheduleable scheduleable, bool onKill ) {
			//Create a dummy key/value pair
			KeyValuePair<int, List<IScheduleable>> scheduleableListFound
			  = new KeyValuePair<int, List<IScheduleable>>( -1, null );

			foreach ( var scheduleablesList in _scheduleables )
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
				//Remove the scheduleable from the list
				Game.Timeline.Remove( scheduleable );
				scheduleableListFound.Value.Remove( scheduleable );
				//If list is now empty, remove the list
				if ( scheduleableListFound.Value.Count <= 0 )
				{
					_scheduleables.Remove( scheduleableListFound.Key );
				}
			}
		}

		// Get the next object whose turn it is from the schedule. Advance time if necessary
		public IScheduleable Get() {
			var firstScheduleableGroup = _scheduleables.First();
			var firstScheduleable = firstScheduleableGroup.Value.First();
			Game.Timeline.Add( firstScheduleableGroup.Key,firstScheduleable );
			Remove( firstScheduleable );
			Time = firstScheduleableGroup.Key;
			return firstScheduleable;
		}

		// Get the current time (turn) for the schedule
		public int GetTime() {
			return Time;
		}

		// Reset the time and clear out the schedule
		public void Clear() {
			Time = 0;
			_scheduleables.Clear();
		}
	}
}
