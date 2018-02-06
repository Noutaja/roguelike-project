using RLGame.Core;
using RLGame.Interfaces;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Systems
{
	public class CommandSystem
	{
		public bool IsPlayerTurn { get; set; }

		public void EndPlayerTurn() {
			IsPlayerTurn = false;
		}

		public void AdvanceTime() {
			IScheduleable scheduleable = Game.SchedulingSystem.Get();

			if ( scheduleable is Player )
			{
				IsPlayerTurn = true;
			}
			else if ( scheduleable is Attack )
			{
				Attack attack = scheduleable as Attack;
				attack.Activate();
				AdvanceTime();
			}
			else if ( scheduleable is Monster )
			{
				Monster monster = scheduleable as Monster;
				monster.Activate();
				Game.SchedulingSystem.Add( monster );
				AdvanceTime();
			}
			else if ( scheduleable is Update )
			{
				Update update = scheduleable as Update;
				update.Activate();
				Game.SchedulingSystem.Add( update );
				AdvanceTime();
			}
		}
	}
}
