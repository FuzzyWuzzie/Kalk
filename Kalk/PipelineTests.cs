using System;
using NUnit.Framework;

namespace Kalk
{
	[TestFixture]
	public class PipelineTests
	{
		private double _result;

		[Test]
		public void HandlesNegativeScientificNotation ()
		{
			Given ("5*10^-3");
			Expect (0.005);
		}

		private void Given (string expression)
		{
			string postFix = InfixToPostix.Transform (expression);
			_result = EvaluatePostfix.Transform (postFix);
		}

		private void Expect (double expected)
		{
			Assert.AreEqual (expected, _result);
		}
	}
}

