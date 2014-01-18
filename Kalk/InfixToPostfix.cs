using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kalk
{
	public class InfixToPostix
	{
		public class OperatorAttributes
		{
			public readonly int precedence;
			public readonly bool leftAssociative;

			public OperatorAttributes (int precedence, bool leftAssociative)
			{
				this.precedence = precedence;
				this.leftAssociative = leftAssociative;
			}
		}

		private static Dictionary<string, OperatorAttributes> _operatorList = new Dictionary<string, OperatorAttributes> ();

		static InfixToPostix ()
		{
			/*_operatorList.Add ("(", new OperatorAttributes (1, false));
			_operatorList.Add (")", new OperatorAttributes (1, false));
			_operatorList.Add ("^", new OperatorAttributes (2, false));
			_operatorList.Add ("+", new OperatorAttributes (3, true));
			_operatorList.Add ("-", new OperatorAttributes (3, true));
			_operatorList.Add ("*", new OperatorAttributes (4, true));
			_operatorList.Add ("/", new OperatorAttributes (4, true));
			_operatorList.Add ("%", new OperatorAttributes (4, true));*/
			
			/*_operatorList.Add ("(", new OperatorAttributes (5, false));
			_operatorList.Add (")", new OperatorAttributes (5, false));*/
			_operatorList.Add ("^", new OperatorAttributes (4, false));
			_operatorList.Add ("*", new OperatorAttributes (3, true));
			_operatorList.Add ("×", new OperatorAttributes (3, true));
			_operatorList.Add ("/", new OperatorAttributes (3, true));
			_operatorList.Add ("÷", new OperatorAttributes (3, true));
			_operatorList.Add ("+", new OperatorAttributes (2, true));
			_operatorList.Add ("-", new OperatorAttributes (2, true));
		}

		private static bool isOperator (string expression)
		{
			return _operatorList.ContainsKey (expression);
		}

		private static bool isSyntacticDevice (string expression)
		{
			return
				expression.Equals ("(") ||
				expression.Equals (")") ||
				expression.Equals (",");
		}

		private static bool isLiteral (string expression)
		{
			return Regex.IsMatch (expression, @"(\w+|(\d+\.?\d+))");
		}

		private static bool isVariable (string expression)
		{
			return Variables.variables.ContainsKey (expression);
		}

		private static bool isConstant (string expression)
		{
			return Variables.constants.ContainsKey (expression);
		}

		private static bool isWhiteSpace (string expression)
		{
			return Regex.IsMatch (expression, @"\w+");
		}

		private static bool isFunction (string expression)
		{
			return Functions.functionList.ContainsKey (expression.ToLower ());
			//return _definedFunctions.Contains (expression.ToLower ());
		}

		public static List<string> tokenizeExpression (string expression)
		{
			List<string> tokens = new List<string> ();

			// start going through the string, one character at a time, building up a buffer
			string buffer = "";
			foreach (char c in expression) {
				// if we have an operator or parentheses, add it as a token
				if (isOperator (c.ToString ()) || isSyntacticDevice (c.ToString ())) {
					// handle negative numbers
					if (c == '-') {
						// figure out what to do with it
						// if this is the first token, add it to the buffer
						if (tokens.Count == 0 && buffer.Equals ("")) {
							buffer += c;
						}
						// if there was an operator before this, it should be a negative number
						else if (tokens.Count > 0 && 
							(isOperator (tokens [tokens.Count - 1]) || tokens [tokens.Count - 1] == "(") && 
							buffer.Equals ("")) {
							buffer += c;
						}
						// otherwise, do the normal thing
						else {
							// flush the buffer first
							if (buffer.Length > 0) {
								tokens.Add (buffer.Trim ());
								buffer = "";
							}
							tokens.Add (c.ToString ());
						}
					} else {
						// flush the buffer first
						if (buffer.Length > 0) {
							tokens.Add (buffer.Trim ());
							buffer = "";
						}
						tokens.Add (c.ToString ());
					}
				} else if (c == ' ') {
					// if it's whitespace, flush the buffer
					if (buffer.Length > 0) {
						tokens.Add (buffer.Trim ());
						buffer = "";
					}
				} else if (isLiteral (c.ToString ()) || c == '.') {
					// add to the buffer if it's any other reasonable input
					buffer += c;
				} else {
					// we have no clue what this is, alert an error!
					// TODO: more specific exceptions!
					throw new EquationParseException ("Undefined symbol: " + c);
				}
			}

			// if our buffer is not empty, add it as a final token
			if (buffer.Length > 0)
				tokens.Add (buffer);

			// now go through the list again and find anything negative that isn't a number
			List<string> newTokens = new List<string> ();
			foreach (string token in tokens) {
				if (token.StartsWith ("-")) {
					// it starts with a negative, specially handle it
					string literalPart = token.Substring (1);
					if (isVariable (literalPart) || isConstant (literalPart)) {
						// if the remaining part isn't a number, add (-1 * x)
						newTokens.Add ("(");
						newTokens.Add ("-1");
						newTokens.Add ("*");
						newTokens.Add (literalPart);
						newTokens.Add (")");
					} else if (isFunction (literalPart)) {
						// need to specially deal with functions
						// just be lazy for now
						// TODO: a better way to do this!
						newTokens.Add ("-1");
						newTokens.Add ("*");
						newTokens.Add (literalPart);
					} else {
						// it must be a number, just pass it on as is
						newTokens.Add (token);
					}
				} else {
					// pass it on
					newTokens.Add (token);
				}
			}

			return newTokens;
		}

		public static string Transform (string expression)
		{
			// check for null input
			if (expression == null || expression.Trim () == "")
				return "";

			string output = "";
			Stack<String> operatorStack = new Stack<string> ();

			// tokenize our items
			List<string> tokens = tokenizeExpression (expression);

			// TODO: parse errors

			// loop through each of the tokens
			foreach (string token in tokens) {
				// check if it's a function token
				if (isFunction (token))
					operatorStack.Push (token);
				// if it's a number (or variable), add it to the output
				else if (isLiteral (token))
					output += token + " ";
				// check if it's a function argument separator
				else if (token.Equals (",")) {
					if (operatorStack.Count == 0)
						// we have a parse error if we have an empty argument
						throw new EquationParseException ("Empty argument");
					while (!operatorStack.Peek().Equals("(")) {
						output += operatorStack.Pop () + " ";
						if (operatorStack.Count == 0)
							// if we ran out of operators on the stack and we
							// didn't hit a '(', then we have an error as we
							// didn't open the parentheses before the function!
							throw new EquationParseException ("No function opening brace");
					}
				}
				// push left parentheses onto the stack
				else if (token.Equals ("("))
					operatorStack.Push (token);
				else if (token.Equals (")")) {
					while (operatorStack.Peek() != "(") {
						output += operatorStack.Pop () + " ";

						// make sure our operator stack contains a "("
						// (if we run out of operators and we still haven't
						// hit a '(', we have a parse exception!)
						if (operatorStack.Count == 0)
							// TODO: more information about the equation parse exception
							throw new EquationParseException ("No matching opening brace");
					}

					// pop the "(" off of the stack
					operatorStack.Pop ();

					// if the top operator is a function, add it to the output
					if (operatorStack.Count > 0 && isFunction (operatorStack.Peek ()))
						output += operatorStack.Pop () + " ";
				}
				// it must be an operator
				else if (_operatorList.ContainsKey (token)) {
					// figure out what to do with the operator
					while (operatorStack.Count > 0 &&
					isOperator(operatorStack.Peek()) 
					 &&
					((_operatorList[token].leftAssociative && _operatorList[token].precedence == _operatorList[operatorStack.Peek()].precedence)
					||
					(_operatorList[token].precedence < _operatorList[operatorStack.Peek()].precedence)))
						output += operatorStack.Pop () + " ";

					// add the operator token to the stack
					operatorStack.Push (token);
				} else {
					// it's not recognized!
					// TODO: write custom exception for unrecognized input
					throw new EquationParseException ("Unrecognized token: " + token);
				}
			}

			// once we're done with all the tokens, finish off the operators
			// TODO: check for missing parenthesis
			while (operatorStack.Count > 0)
				output += operatorStack.Pop () + " ";

			// return the trimmed string
			return output.Trim ();
		}
	}
}

