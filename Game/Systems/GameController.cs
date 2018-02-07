﻿using RLGame.Core;
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
	public class GameController
	{
		public List<DungeonMap> Maps { get; private set; }
		public DungeonMap CurrentMap { get; private set; }
		private readonly int MAXMAPWIDTH;
		private readonly int MAXMAPHEIGHT;
		public Player Player;
		private bool _isPlayerTurn;
		public bool IsPlayerTurn {
			get { return _isPlayerTurn; }
			set { _isPlayerTurn = value; }
		}

		public GameController( int maxMapWidth, int maxMapHeight ) {
			Maps = new List<DungeonMap>();
			MAXMAPWIDTH = maxMapWidth;
			MAXMAPHEIGHT = maxMapHeight;

			Player = new Player();
		}

		public void Initialize() {
			MapGenerator mapGenerator = new MapGenerator( MAXMAPWIDTH, MAXMAPHEIGHT, 50, 8, 4, 1, true );
			CurrentMap = mapGenerator.CreateMap();
			CurrentMap.UpdatePlayerFieldOfView();
			Maps.Add( CurrentMap );
		}
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

		public void ChangeLevel( bool down ) {
			int newLevel = CurrentMap.MapLevel;
			if ( down == true )
				newLevel++;
			else
				newLevel--;
			CurrentMap.PreLevelChange();

			if ( Maps.Count < newLevel )
			{
				MapGenerator mapGenerator = new MapGenerator( MAXMAPWIDTH, MAXMAPHEIGHT, 50, 8, 4, newLevel, down );
				CurrentMap = mapGenerator.CreateMap();
				Game.MessageLog.Clear();
				Game.MessageLog.Add( $"Entering level {CurrentMap.MapLevel}" );
				Game.Timeline.Clear();
				Maps.Add( CurrentMap );
				IsPlayerTurn = false;
			}
			else
			{
				CurrentMap = Maps[newLevel - 1];
				CurrentMap.PostLevelChange( down );
				Game.MessageLog.Clear();
				Game.MessageLog.Add( $"Entering level {CurrentMap.MapLevel}" );
				Game.Timeline.Clear();
				IsPlayerTurn = false;
			}
		}
	}
}
