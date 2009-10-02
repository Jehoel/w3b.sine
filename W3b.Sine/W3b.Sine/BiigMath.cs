#define DoubleTrig
//#define EnableNotYetImplemented

using System;
using System.Collections.Generic;
using System.Text;

using Cult = System.Globalization.CultureInfo;

namespace W3b.Sine {
	
	public static class BigMath {
		
#region Numbers
		
		public static BigNum Abs(BigNum num) {
			
			return num.Absolute();
		}
		
		public static BigNum Floor(BigNum num) {
			
			return num.Floor();
		}
		
		public static BigNum Ceiling(BigNum num) {
			
			return num.Ceiling();
		}
		
		public static BigNum Max(BigNum x, BigNum y) {
			
			return x < y ? y : x;
		}
		
		public static BigNum Min(BigNum x, BigNum y) {
			
			return x > y ? y : x;
		}
		
		
		
#endregion
#region Exponentiation and Factorial
		
		public static BigNum Pow(BigNum num, Int32 exponent) {
			
			return num.Power( exponent );
		}
		
	#if EnableNotYetImplemented
		
		public static BigNum Pow(BigNum num, BigNum exponent) {
			
			// http://en.wikipedia.org/wiki/Exponentiation
			
			// a^x == E^( x * ln(a) )
			
			throw new NotImplementedException();
		}
		
		/// <summary>Computes Euler's number raised to the specified exponent</summary>
		public static BigNum Exp(BigNum exponent) {
			
			// E^x ~= sum(int i = 0 to inf, x^i/i!)
			//     ~= 1 + x + x^2/2! + x^3/3! + etc
			
			throw new NotImplementedException();
		}
		
		/// <summary>Computes the Natural Logarithm (Log to base E, or Ln(x)) of the specified argument</summary>
		public static BigNum Log(BigNum argument) {
			
			// Ln(x) ~= 
			
			throw new NotImplementedException();
		}
		
		private static BigNum LogLT1(BigNum x) {
			// Ln(x) ~= (x-1) - (x-1)^2/2 + (x-1)^3/3 - (x-1)^4/4 + ....
			throw new NotImplementedException();
		}
		
		private static BigNum LogGT1(BigNum x) {
			// Ln(y/(y-1)) ~= 1/y + 1/(2y^2) + 1/(3y^3) + ...
			// x == y/(y-1)
			// TODO: Reduce to get y
			throw new NotImplementedException();
		}
		
		/////////////////////////////////////////
		
		public static BigNum Gamma(BigNum num) {
			
			// http://www.rskey.org/gamma.htm
			
			// n! = nn√2πn exp(1/[12n + 2/(5n + 53/42n)] – n)(1 + O(n–8))
			
			// which requires the exponential function...
			
			throw new NotImplementedException();
			
		}
		
	#endif
		
		public static BigNum Factorial(BigNum num) {
			// HACK: Is there a more efficient implementation?
			// I know there is a way to cache and use earlier results, but not much more
			
			// also, note this function fails if num is non-integer. This should be the Gamma function instead
			
			if(num.IsZero) return 1;
			if(num < 0) throw new ArgumentException("Argument must be greater than or equal to zero", "num");
			return num * Factorial( num - 1 );
		}
		
#endregion
#region Trig
		
		public static BigNum Sin(BigNum theta) {
			
			// calculate sine using the taylor series, the infinite sum of x^r/r! but to n iterations
			BigNum retVal = 0;
			
			// first, reduce this to between 0 and 2Pi
			if( theta > BigNum.TwoPi || theta < 0 )
				theta = theta % BigNum.TwoPi;
			
			Boolean subtract = false;
			
			// using bignums for sine computation is too heavy. It's faster (and just as accurate) to use Doubles
	#if DoubleTrig
			
			Double thetaDbl = Double.Parse( theta.ToString(), Cult.InvariantCulture );
			for(Int32 r=0;r<20;r++) { // 20 iterations is enough, any more just yields inaccurate less-significant digits
				
				Double xPowerR = Math.Pow( thetaDbl, 2*r + 1 );
				Double factori = BigMath.Factorial( (double)( 2*r + 1 ) );
				
				Double element = xPowerR / factori;
				
				Double addThis = subtract ? -element : element;
				
				retVal += addThis;
				
				subtract = !subtract;
			}
			
			
	#else
			
			for(Int32 r=0;r<_iterations;r++) {
				
				BigNum xPowerR = theta.Power(2*r +1);
				BigNum factori = Factorial( 2*r + 1);
				
				BigNum element = xPowerR / factori;
				
				retVal += subtract ? -element : element;
				
				subtract = !subtract;
			}
			
	#endif
			
			// TODO: This calculation generates useless and inaccurate trailing digits that must be truncated
			// so truncate them, when I figure out how many digits can be removed
			
			retVal.Truncate( 10 );
			
			return retVal;
			
		}
		
		public static BigNum Cos(BigNum theta) {
			
			// using Cos(x) == Sin(x + 90deg)
			
			theta += BigNum.HalfPi;
			
			return Sin( theta );
		}
		
		public static BigNum Tan(BigNum theta) {
			
			// using Tan(x) == Sin(x) / Cos(x)
			
			BigNum sine = Sin(theta);
			BigNum cosi = Cos(theta);
			
			return sine / cosi;
		}
		
		public static BigNum Csc(BigNum theta) {
			return Sin(theta).Power(-1);
		}
		
		public static BigNum Sec(BigNum theta) {
			return Cos(theta).Power(-1);
		}
		
		public static BigNum Cot(BigNum theta) {
			return Tan(theta).Power(-1);
		}
		
#endregion
#region Internal Utility
		
		internal static Double Factorial(Double number) {
			if(number == 0) return 1;
			return number * Factorial( (double)( number - 1 ) );
		}
		
#endregion
		
		
	}
}
