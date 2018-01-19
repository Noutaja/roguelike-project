using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Core;

namespace RLGame.Interfaces
{
	public interface IBodypart
	{
		BodypartType partType { get; set; }
		string Name { get; set; }
		int Health { get; set; }
		int MaxHealth { get; set; }
		bool IsVital { get; set; }
		int Size { get; set; }
	}
}
