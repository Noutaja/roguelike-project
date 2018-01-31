using RLGame.Core;
using Action = RLGame.Actions.BaseActions.Action;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces
{
	public interface IActor
	{
		int Attack { get; set; }
		int AttackChance { get; set; }
		int Awareness { get; set; }
		int Defense { get; set; }
		int DefenseChance { get; set; }
		string Name { get; set; }
		int Initiative { get; set; }
		int Speed { get; set; }
		double Regen { get; set; }
		Action LastAction { get; set; }
		List<Bodypart> Bodyparts { get; set; }

		int Size();
		bool IsHurt();
		bool IsDying();
		Bodypart TakeDamage(int damage, Bodypart bodypart);
		Bodypart GetBodypart( bool sizeWeighted );
		Bodypart GetBodypart( string bodypartName );
		Bodypart GetVitalBodypart( bool sizeWeighted );
	}
}
