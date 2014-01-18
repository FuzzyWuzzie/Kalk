using System;
using NUnit.Framework;

namespace Kalk
{
	[TestFixture]
	public class InfixToPostfixTests
	{
		string _result;

		[Test]
		public void EmptyExpressionResultsInSame ()
		{
			Given ("");
			Expect ("");
		}

		[Test]
		public void NullExpressionProducesEmptyResults ()
		{
			Given (null);
			Expect ("");
		}

		[Test]
		public void JustANumberResultsInSame ()
		{
			Given ("42");
			Expect ("42");
		}

		[Test]
		public void HandlesASingleBinaryOperator ()
		{
			Given ("4 + 2");
			Expect ("4 2 +");
		}

		[Test]
		public void HandlesMissingWhitespace ()
		{
			Given ("4+2");
			Expect ("4 2 +");
		}

		[Test]
		public void HandlesMultipleOperatorsOfSamePrecedence ()
		{
			Given ("a - 5 + 3");
			Expect ("a 5 - 3 +");
		}

		[Test]
		public void HandlesASingleDecimalNumber ()
		{
			Given ("42.42");
			Expect ("42.42");
		}

		[Test]
		public void HandlesOperatorPrecedence ()
		{
			Given ("2 + 9 * 6");
			Expect ("2 9 6 * +");
		}

		[Test]
		public void HandlesRightAssociativeOperators ()
		{
			Given ("2 * 10 ^ 6");
			Expect ("2 10 6 ^ *");
		}

		[Test]
		public void HandlesMultipleAssociativeOperators ()
		{
			Given ("2 ^ 3 ^ 4");
			Expect ("2 3 4 ^ ^");
		}

		[Test]
		public void HandlesBrackets ()
		{
			Given ("( 3 + 4 )");
			Expect ("3 4 +");
		}

		[Test]
		public void HandlesBracketsAffectingOrderOfOperations ()
		{
			Given ("( 3 + 4 ) * 5");
			Expect ("3 4 + 5 *");
		}

		[Test]
		public void HandlesNestedBrackets ()
		{
			Given ("( 3 + ( 4 - 5 ) ) * 6");
			Expect ("3 4 5 - + 6 *");
		}

		[Test]
		public void HandlesTokensWithoutSpaces ()
		{
			Given ("5+3*2");
			Expect ("5 3 2 * +");
		}

		[Test]
		public void HandlesFunctionCalls ()
		{
			Given ("sin(3)");
			Expect ("3 sin");
		}

		[Test]
		public void HandlesNestedFunctions ()
		{
			Given ("sin(cos(4))");
			Expect ("4 cos sin");
		}

		[Test]
		public void HandlesMultipleFunctionParameters ()
		{
			Given ("atan2(3, 4)");
			Expect ("3 4 atan2");
		}

		[Test]
		public void HandlesComplexCase1 ()
		{
			Given ("3 + 4 * 2 / (1 - 5) ^ 2 ^ 3");
			Expect ("3 4 2 * 1 5 - 2 3 ^ ^ / +");
		}

		[Test]
		public void HandlesComplexCase2 ()
		{
			Given ("tan(4+5, 1 + a^2, (8+b)*10)");
			Expect ("4 5 + 1 a 2 ^ + 8 b + 10 * tan");
		}

		private void Given (string expression)
		{
			_result = InfixToPostix.Transform (expression);
		}

		private void Expect (string expected)
		{
			Assert.That (_result, Is.EqualTo (expected));
		}
	}
}

