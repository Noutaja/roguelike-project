using RLGame.Core;
using RLGame.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLGame.Interfaces
{
	public interface IBehavior
	{
		bool Act( Monster monster );
	}
}
