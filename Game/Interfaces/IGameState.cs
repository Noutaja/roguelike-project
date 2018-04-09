using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces
{
	public interface IGameState
	{
		bool Transparent { get; }
		bool Pauses { get; }
		void Init(RLRootConsole rootConsole);
		void Close();
		bool OnUpdate( RLKeyPress keyPress );
		void OnRender();

	}
}
