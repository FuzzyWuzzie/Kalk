using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Kalk
{
	public class Functions
	{
		public static bool useDegrees = true;

		public delegate double FunctionDelegate (double[] x);

		public class Function
		{
			public FunctionDelegate function;
			public int nArgs = 0;

			public Function (FunctionDelegate function, int nArgs)
			{
				this.function = function;
				this.nArgs = nArgs;
			}
		}

		public static Dictionary<string, Function> functionList = new Dictionary<string, Function> ();

		static Functions ()
		{
			MethodInfo[] functions = typeof(Functions).GetMethods ().Where (m => m.GetCustomAttributes (typeof(MathFunction), false).Length > 0).ToArray ();
			foreach (MethodInfo function in functions) {
				string name = function.Name;
				int nArgs = 0;
				foreach (object attrib in function.GetCustomAttributes(false)) {
					if (attrib.GetType ().Equals (typeof(MathFunction))) {
						nArgs = ((MathFunction)attrib).nArgs;
						break;
					}
				}
				FunctionDelegate d = (FunctionDelegate)Delegate.CreateDelegate (typeof(FunctionDelegate),
				                                                                function);
				functionList.Add (name.ToLower (), new Function (d, nArgs));
			}
		}

		[MathFunction(2)]
		public static double Add (double[] x)
		{
			return x [0] + x [1];
		}

		[MathFunction(2)]
		public static double Subtract (double[] x)
		{
			return x [1] - x [0];
		}

		[MathFunction(2)]
		public static double Multiply (double[] x)
		{
			return x [0] * x [1];
		}

		[MathFunction(2)]
		public static double Divide (double[] x)
		{
			return x [1] / x [0];
		}

		[MathFunction(2)]
		public static double Exponent (double[] x)
		{
			return Math.Pow (x [1], x [0]);
		}

		[MathFunction(1)]
		public static double Sqrt (double[] x)
		{
			return Math.Sqrt (x [0]);
		}

		[MathFunction(1)]
		public static double Sin (double[] x)
		{
			return Math.Sin (x [0] * (useDegrees ? (Math.PI / 180.0) : (1)));
		}

		[MathFunction(1)]
		public static double Cos (double[] x)
		{
			return Math.Cos (x [0] * (useDegrees ? (Math.PI / 180.0) : (1)));
		}

		[MathFunction(1)]
		public static double Tan (double[] x)
		{
			return Math.Tan (x [0] * (useDegrees ? (Math.PI / 180.0) : (1)));
		}

		[MathFunction(1)]
		public static double Asin (double[] x)
		{
			return Math.Asin (x [0]) * (useDegrees ? 180.0 / Math.PI : 1);
		}

		[MathFunction(1)]
		public static double Acos (double[] x)
		{
			return Math.Acos (x [0]) * (useDegrees ? 180.0 / Math.PI : 1);
		}

		[MathFunction(1)]
		public static double Atan (double[] x)
		{
			return Math.Atan (x [0]) * (useDegrees ? 180.0 / Math.PI : 1);
		}

		[MathFunction(2)]
		public static double Atan2 (double[] x)
		{
			return Math.Atan2 (x [1], x [0]) * (useDegrees ? 180.0 / Math.PI : 1);
		}

		[MathFunction(1)]
		public static double Ln (double[] x)
		{
			return Math.Log (x [0]);
		}

		[MathFunction(1)]
		public static double Log (double[] x)
		{
			return Math.Log10 (x [0]);
		}
	}
}

