using RLGame.Core;
using BaseAction = RLGame.Actions.BaseActions.BaseAction;
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
		int Strength { get; }
		int Awareness { get; set; }
		string Name { get; set; }
		int Initiative { get; set; }
		int Speed { get; set; }
		int Size { get; }
		double Regen { get; set; }
		BaseAction LastAction { get; set; }
		List<Bodypart> Bodyparts { get; set; }
		List<BaseAction> Actions { get; set; }
		
		bool IsHurt();
		bool IsDying();
		Bodypart TakeDamage(int damage, Bodypart bodypart);
		Bodypart GetBodypart( bool sizeWeighted );
		Bodypart GetBodypart( string bodypartName );
		Bodypart GetVitalBodypart( bool sizeWeighted );
	}
}
