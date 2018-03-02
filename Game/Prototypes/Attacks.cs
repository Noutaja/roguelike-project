using RLGame.Actions;
using RLGame.Actions.BaseActions;
using RLGame.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Prototypes
{
	public static class Attacks
	{
		public static AttackAction Punch(Actor actor) {
			AttackAction action = new AttackAction( actor );
			action.TimeMultiplier = 0.2;
			action.AttackSpeed = (int) ( actor.Initiative * ( action.TimeMultiplier / 2 ) );
			action.AttackPattern = AttackPatterns.Basic();
			action.Actor = actor;
			action.Name = "Punch";
			action.Tags.Add( ActionTag.Melee );
			action.Damage = actor.Strength;
			return action;
		}

		public static AttackAction Slash( Actor actor ) {
			AttackAction action = new AttackAction( actor );
			action.TimeMultiplier = 0.3;
			action.AttackSpeed = (int) ( actor.Initiative * ( action.TimeMultiplier / 2 ) );
			action.AttackPattern = AttackPatterns.Horizontal3();
			action.Actor = actor;
			action.Name = "Slash";
			action.Tags.Add( ActionTag.Melee );
			action.Damage = actor.Strength*2;
			return action;
		}

		public static BaseAction Bite(Actor actor) {
			AttackAction action = new AttackAction( actor );
			action.TimeMultiplier = 0.2;
			action.AttackSpeed = (int) ( actor.Initiative * ( action.TimeMultiplier / 2 ) );
			action.AttackPattern = AttackPatterns.Basic();
			action.Actor = actor;
			action.Name = "Bite";
			action.Tags.Add( ActionTag.Melee );
			action.Damage = actor.Strength;
			return action;
		}
	}
}
