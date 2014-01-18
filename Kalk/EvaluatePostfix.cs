using System;
using System.Collections.Generic;

namespace Kalk
{
	public class EvaluatePostfix
	{
		private static Dictionary<string, string> _operatorShortcuts = new Dictionary<string, string> ();

		static EvaluatePostfix ()
		{
			_operatorShortcuts.Add ("+", "add");
			_operatorShortcuts.Add ("-", "subtract");
			_operatorShortcuts.Add ("*", "multiply");
			_operatorShortcuts.Add ("/", "divide");
			_operatorShortcuts.Add ("×", "multiply");
			_operatorShortcuts.Add ("÷", "divide");
			_operatorShortcuts.Add ("^", "exponent");
		}

		public static double Transform (string expression)
		{
			// empty strings should return 0
			if (expression == null || expression.Trim () == "")
				return 0;

			// tokenize the expression
			// noting that things will be separated by spaces to make our lives
			// easier a priori
			string[] tokens = expression.Split (' ');

			// our stack of values
			Stack<double> valueStack = new Stack<double> ();

			// go through all of the tokens
			foreach (string token in tokens) {
				// determine if it is a value or an "operator"
				if (_operatorShortcuts.ContainsKey (token) || Functions.functionList.ContainsKey (token)) {
					// it's an "operator"
					// get it!
					Functions.Function function = null;
					if (_operatorShortcuts.ContainsKey (token))
						function = Functions.functionList [_operatorShortcuts [token]];
					else
						function = Functions.functionList [token];

					// make sure we have enough values on the stack
					if (valueStack.Count < function.nArgs)
						// TODO: better exceptions
						throw new EquationParseException ("Not enough values to process!");

					// now pop those values off
					double[] x = new double[function.nArgs];
					for (int i = 0; i < function.nArgs; i++)
						x [i] = valueStack.Pop ();

					// evaluate it!
					double result = function.function (x);
					valueStack.Push (result);

				} else {
					// it's a value!
					valueStack.Push (Variables.EvaluateLiteral (token));
				}
			}

			// now, we should only have a single value on the stack - the result
			// if so, return it
			if (valueStack.Count == 1)
				return valueStack.Pop ();

			// there are too many values!
			throw new EquationParseException ("Not enough operators for values!");
		}
	}
}

