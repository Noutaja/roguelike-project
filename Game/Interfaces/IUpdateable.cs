using RLGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces
{
	public interface IUpdateable
	{
		void OnUpdateEvent( object sender, EventArgs e );
	}
}
