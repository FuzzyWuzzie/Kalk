using System;
using NUnit.Framework;

namespace Kalk
{
	[TestFixture]
	public class EvaluatePostfixTests
	{
		double _result;

		[Test]
		public void EmptyExpressionResultsInZero ()
		{
			Given ("");
			Expect (0);
		}

		[Test]
		public void SingleValueResultsInSame ()
		{
			Given ("42");
			Expect (42);
		}

		[Test]
		public void SimpleAdditionIsCorrect ()
		{
			Given ("4 2 +");
			Expect (6);
		}

		[Test]
		public void SimpleSubtractionIsCorrect ()
		{
			Given ("4 2 -");
			Expect (2);
		}

		[Test]
		public void SimpleMultiplicationIsCorrect ()
		{
			Given ("4 2 *");
			Expect (8);
		}

		[Test]
		public void SimpleDivisionIsCorrect ()
		{
			Given ("4 2 /");
			Expect (2);
		}

		[Test]
		public void SimpleExponentIsCorrect ()
		{
			Given ("4 2 ^");
			Expect (16);
		}

		[Test]
		public void SimpleFunctionIsCorrect ()
		{
			Given ("4 sqrt");
			Expect (2);
		}

		[Test]
		public void ATan2IsCorrect ()
		{
			Given ("1 0 atan2");
			Expect (90);
		}

		[Test]
		public void LongerExpressionIsCorrect ()
		{
			Given ("5 1 2 + 4 * + 3 -");
			Expect (14);
		}

		[Test]
		public void DecimalsAreCorrect ()
		{
			Given ("42.42 3.14159 +");
			Expect (45.56159);
		}

		[Test]
		public void HandlesNegativeExponent ()
		{
			Given ("10 -3 ^");
			Expect (0.001);
		}

		[Test]
		public void HandlesPositiveExponent ()
		{
			Given ("10 3 ^");
			Expect (1000);
		}

		private void Given (string expression)
		{
			_result = EvaluatePostfix.Transform (expression);
		}

		private void Expect (double expected)
		{
			Assert.That (_result, Is.EqualTo (expected));
		}
	}
}

