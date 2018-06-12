using RLNET;
using RLGame.Core;
using RLGame.Systems;
using RogueSharp.Random;
using System;
using Weighted_Randomizer;
using System.Collections.Generic;
using OpenTK.Input;
using RLGame.Interfaces;
using RLGame.GameStates;

namespace RLGame
{
	public static class Game
	{
		public static readonly int SCREENWIDTH = 100;
		public static readonly int SCREENHEIGHT = 70;
		private static RLRootConsole _rootConsole;

		private static Stack<IGameState> _gameStack;
		
		public static IRandom Random { get; private set; }
		public static IWeightedRandomizer<Bodypart> WeightedRandom;

		static void Main( string[] args ) {
			Keyboard.GetState();//IT JUST WERKS NOW?? PLS FIX :D (OnRootConsoleUpdate)
			string fontFileName = "terminal8x8.png";

			//Initialize RNG
			int seed = (int) DateTime.UtcNow.Ticks;
			WeightedRandom = new StaticWeightedRandomizer<Bodypart>( seed );
			Random = new DotNetRandom( seed );
			string consoleTitle = $"RLGame Seed {seed}"; //Show the seed

			_rootConsole = new RLRootConsole( fontFileName, SCREENWIDTH, SCREENHEIGHT, 8, 8, 1f, consoleTitle );

			_gameStack = new Stack<IGameState>();
			MainScreen main = new MainScreen( false, true, _rootConsole );
			main.Init();
			_gameStack.Push( main );
			
			_rootConsole.Update += OnRootConsoleUpdate;
			_rootConsole.Render += OnRootConsoleRender;
			_rootConsole.Run();
		}

		private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e ) {
			RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
			Stack<IGameState> s = new Stack<IGameState>();
			IGameState state = null;
			do
			{
				state = _gameStack.Pop();
				s.Push( state );
				if ( state.OnUpdate( keyPress ) )
				{
					while ( s.Count > 0 )
					{
						_gameStack.Push( s.Pop() );
					}
					return;
				}
			}
			while ( !state.Pauses && _gameStack.Count > 0 );
			
			while(s.Count > 0 )
			{
				_gameStack.Push( s.Pop() );
			}
		}

		private static void OnRootConsoleRender( object sender, UpdateEventArgs e ) {

			Stack<IGameState> s = new Stack<IGameState>();
			IGameState state = null;
			do
			{
				state = _gameStack.Pop();
				s.Push( state );
				state.OnRender();
			}
			while ( !state.Transparent && _gameStack.Count > 0 );

			while ( s.Count > 0 )
			{
				_gameStack.Push( s.Pop() );
			}

			_rootConsole.Draw();
			
		}
	}
}
