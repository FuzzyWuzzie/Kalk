using System;

namespace Kalk
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MathFunction : System.Attribute
	{
		public int nArgs = 0;

		public MathFunction (int nArgs)
		{
			this.nArgs = nArgs;
		}
	}
}

