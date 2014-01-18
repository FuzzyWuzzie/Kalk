using System;
using System.Collections.Generic;

namespace Kalk
{
	public class Variables
	{
		public static Dictionary<string, double> constants = new Dictionary<string, double> ();
		public static Dictionary<string, double> variables = new Dictionary<string, double> ();

		static Variables ()
		{
			constants.Add ("pi", Math.PI);

			foreach (char c in "abcdefg") {
				variables.Add (c.ToString (), 0);
			}
		}

		public static double EvaluateLiteral (string literal)
		{
			// if it's a constant or a variable, just spout back the value
			if (constants.ContainsKey (literal))
				return constants [literal];
			else if (variables.ContainsKey (literal))
				return variables [literal];

			// otherwise, it must be a numeric value
			double val = 0;
			if (!double.TryParse (literal, out val))
				// TODO: not a value
				throw new EquationParseException ("Invalid literal: " + literal);
			return val;
		}
	}
}

