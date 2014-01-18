using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kalk
{
	[TestFixture]
	public class TokenizeExpressionTests
	{
		string _result;

		[Test]
		public void HandlesSingleItem ()
		{
			Given ("5");
			Expect ("5");
		}

		[Test]
		public void HandlesSimpleExpressionWithSpaces ()
		{
			Given ("1 + 1");
			Expect ("1|+|1");
		}

		[Test]
		public void HandlesSimpleExpressionWithoutSpaces ()
		{
			Given ("a*b");
			Expect ("a|*|b");
		}

		[Test]
		public void HandlesParentheses ()
		{
			Given ("(a * 2)-3");
			Expect ("(|a|*|2|)|-|3");
		}

		[Test]
		public void HandlesSingleNegativeNumbers ()
		{
			Given ("-2");
			Expect ("-2");
		}

		[Test]
		public void HandlesNegativeNumbersAfterOperator ()
		{
			Given ("a + -2");
			Expect ("a|+|-2");
		}

		[Test]
		public void HandlesNegativeNumbersInParentheses ()
		{
			Given ("(-2)");
			Expect ("(|-2|)");
		}

		[Test]
		public void HandlesSingleNegativeVariables ()
		{
			Given ("-pi");
			Expect ("(|-1|*|pi|)");
		}

		[Test]
		public void HandlesSingleNegativeFunctions ()
		{
			Given ("-sqrt(4)");
			Expect ("-1|*|sqrt|(|4|)");
		}

		[Test]
		public void HandlesNegativeVariablesInExpression ()
		{
			Given ("5 * -pi");
			Expect ("5|*|(|-1|*|pi|)");
		}

		[Test]
		public void HandlesComplexCase ()
		{
			Given ("(-2)*(-5^-1)");
			Expect ("(|-2|)|*|(|-5|^|-1|)");
		}

		private void Given (string expression)
		{
			List<string> tokens = InfixToPostix.tokenizeExpression (expression);
			_result = string.Join ("|", tokens.ToArray ());
		}

		private void Expect (string expected)
		{
			//Assert.That (_result, Is.EqualTo (expected));
			Assert.AreEqual (expected, _result);
		}
	}
}

