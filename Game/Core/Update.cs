﻿using RLGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Core
{
	public class Update : IScheduleable
	{
		public int Time { get; private set; }
		public bool History { get { return false; } }
		public event EventHandler UpdateEvent;

		public Update() {
			Time = 10;
		}

		public void Activate() {
			UpdateEvent( this, new EventArgs() );
		}
	}
}
