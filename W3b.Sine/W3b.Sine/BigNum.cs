/*
*  BSD 3-clause license:
* 
* Copyright (c) 2008, David Rees / David "W3bbo" Rees / http://www.softicide.com / http://www.w3bdevil.com
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the David Rees nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY David Rees ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL David Rees BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;

using Cult = System.Globalization.CultureInfo;

namespace W3b.Sine {
	
	/// <summary>An immutable arbitary-precision number class.</summary>
	public abstract class BigNum : IComparable<BigNum>, ICloneable {
		
		public abstract BigNum Add(BigNum other);
		
		public abstract BigNum Multiply(BigNum multiplicand);
		
		/// <summary>Divides this instance by the divisor, returning the quotient.</summary>
		public abstract BigNum Divide(BigNum divisor);
		
		/// <summary>Divides this instance by the divisor, returning the remainder.</summary>
		public abstract BigNum Modulo(BigNum divisor);
		
		public abstract Int32 CompareTo(BigNum other);
		
		/// <summary>Inverts this instance's sign</summary>
		public abstract BigNum Negate();
		
		/// <summary>Returns the norm of the number</summary>
		public abstract BigNum Absolute();
		
		public abstract BigNum Floor();
		public abstract BigNum Ceiling();
		
		public abstract Boolean IsZero { get; }
		public abstract Boolean Sign   { get; set; }
		
		public abstract BigNum Clone();
		
		public abstract void Load(String textRepresentation);
		public abstract void Load(Int64 value);
		public abstract void Load(Double value);
		
		Object ICloneable.Clone() {
			return Clone();
		}
		
		/// <summary>Gets the name of the implementation, for rejecting incompatible BigNum types.</summary>
		public abstract String ImplName { get; }
		
		public abstract override Boolean Equals(object obj);
		public abstract override Int32 GetHashCode();
		public abstract override String ToString();
		
		/// <summary>Creates an instance of BigNum set to zero.</summary>
		public static BigNum CreateInstance() {
			return new BigNumDec(0);
		}
		public static BigNum CreateInstance(Double value) {
			return new BigNumDec(value);
		}
		public static BigNum CreateInstance(String value) {
			return new BigNumDec(value);
		}
		
#region Operators
		
		public static BigNum operator +(BigNum a, BigNum b) {
			if(a == null) return null;
			if(b == null) return null;
			return a.Add(b);
		}
		
		public static BigNum operator -(BigNum a) {
			if(a == null) return null;
			return a.Negate();
		}
		
		public static BigNum operator -(BigNum a, BigNum b) {
			if(a == null) return null;
			if(b == null) return null;
			return a.Add( b.Negate() );
		}
		
		public static BigNum operator *(BigNum a, BigNum b) {
			if(a == null) return null;
			if(b == null) return null;
			return a.Multiply(b);
		}
		
		public static BigNum operator /(BigNum a, BigNum b) {
			if(a == null) return null;
			if(b == null) return null;
			return a.Divide(b);
		}
		
		public static BigNum operator %(BigNum a, BigNum b) {
			if(a == null) return null;
			if(b == null) return null;
			return a.Modulo(b);
		}
		
		public static Boolean operator ==(BigNum a, BigNum b) {
			return Equals(a, b);
		}
		
		public static Boolean operator !=(BigNum a, BigNum b) {
			return !Equals(a, b);
		}
		
		public static Boolean operator <(BigNum a, BigNum b) {
			if(a == null && b == null) return false;
			if(a == null) return true;
			if(b == null) return false;
			return a.CompareTo(b) == -1;
		}
		
		public static Boolean operator <=(BigNum a, BigNum b) {
			if(a == null && b == null) return true;
			if(a == null) return true;
			if(b == null) return false;
			Int32 result = a.CompareTo(b);
			return result == -1 || result == 0;
		}
		
		public static Boolean operator >(BigNum a, BigNum b) {
			if(a == null && b == null) return false;
			if(a == null) return true;
			if(b == null) return false;
			return b.CompareTo(a) == -1;
		}
		
		public static Boolean operator >=(BigNum a, BigNum b) {
			if(a == null && b == null) return true;
			if(a == null) return true;
			if(b == null) return false;
			Int32 result = b.CompareTo(a);
			return result == -1 || result == 0;
		}
		
		public static implicit operator String(BigNum n) {
			return n.ToString();
		}
		public static implicit operator BigNum(Int64 value) {
			return CreateInstance( value );
		}
		public static implicit operator BigNum(String value) {
			return CreateInstance( value );
		}
		
		public static Boolean operator ==(BigNum a, Int64 b) {
			return Equals(a, b);
		}
		public static Boolean operator !=(BigNum a, Int64 b) {
			return !Equals(a, b);
		}
		
#endregion
		
#region Other Operations
		
		// operations not needing to be implemented by derived classes, but virtual just in case
		
		public virtual BigNum Subtract(BigNum other) {
			return Add( other.Negate() );
		}
		
		public virtual BigNum Power(Int32 exponent) {
			
			Int32 pow = exponent < 0 ? -exponent : exponent; // abs(exponent);
			
			BigNum retVal = BigNum.CreateInstance(1);
			for(int i=0;i<pow;i++) {
				retVal = retVal.Multiply( this );
			}
			
			if(exponent  < 0) { // reciprocal
				retVal = BigNum.CreateInstance(1).Divide(retVal);
			}
			
			return retVal;
			
		}
		
		public virtual BigNum Factorial() {
			if(this.IsZero) return BigNum.CreateInstance(1);
			return this.Multiply( this.Add( -1 ) );
		}
		
		private static Double FactorialDbl(Double number) {
			if(number == 0) return 1;
			return number * FactorialDbl(number - 1);
		}
		
		private static Double PowerDbl(Double number, Int32 exponent) {
			Int32 pow = exponent < 0 ? -exponent : exponent; // abs(exponent)
			
			Double retVal = 1;
			for(int i=0;i<pow;i++) {
				retVal = retVal * number;
			}
			
			if(exponent < 0) { // reciprocal minus powers
				retVal = 1 / retVal;
			}
			
			return retVal;
		}
		
		private static BigNum Factorial(Int32 number) {
			Double result = FactorialDbl(number);
			return BigNum.CreateInstance( result );
		}
		
	#region Trigonometric
		
		// consts computed by Mathematica, my ComputePi function is inaccurate after 4 digits for some reason
		private const String HalfPi = "1.5707963267948966192313216916397514420985846996876";
		private const String     Pi = "3.1415926535897932384626433832795028841971693993751";
		private const String  TwoPi = "6.2831853071795864769252867665590057683943387987502";
		
		private const Int32 _iterations = 100;
		
		public virtual BigNum Sine() {
			
			// calculate sine using the taylor series, the infinite sum of x^r/r! but to n iterations
			BigNum retVal = BigNum.CreateInstance( 0 );
			
			// first, reduce this to between 0 and 2Pi
			BigNum twoPi = TwoPi;
			BigNum theta = Clone();
			if( theta > twoPi || theta < 0 )
				theta = this.Modulo( twoPi );
			
			Boolean subtract = false;
			/*for(Int32 r=0;r<_iterations;r++) {
				
				BigNum xPowerR = theta.Power(2*r +1);
				BigNum factori = Factorial( 2*r + 1);
				
				BigNum element = xPowerR / factori;
				
				retVal += subtract ? -element : element;
				
				subtract = !subtract;
			}*/
			// using bignums for sine computation is too heavy. It's faster (and just as accurate) to use Doubles
			
			Double thetaDbl = Double.Parse( theta.ToString(), Cult.InvariantCulture );
			for(Int32 r=0;r<_iterations;r++) {
				
				Double xPowerR = PowerDbl(thetaDbl, 2*r + 1);
				Double factori = FactorialDbl( 2*r + 1);
				
				Double element = xPowerR / factori;
				
				Double addThis = subtract ? -element : element;
				BigNum addThisBn = BigNum.CreateInstance( addThis );
				
				retVal = retVal.Add( addThisBn );
				
				subtract = !subtract;
			}
			
			return retVal;
			
		}
		
		public virtual BigNum Cosine() {
			BigNum theta = Add( HalfPi );
			BigNum sine = theta.Sine();
			
			return sine;
		}
		
		public virtual BigNum Tangent() {
			
			BigNum theta = Clone();
			BigNum sine = theta.Sine();
			BigNum cosi = theta.Cosine();
			
			return sine / cosi;
		}
		
		public virtual BigNum Cosec() {
			return Sine().Power(-1);
		}
		
		public virtual BigNum Secant() {
			return Cosine().Power(-1);
		}
		
		public virtual BigNum Cotangent() {
			return Tangent().Power(-1);
		}
		
	#endregion
		
#endregion
		
	}
}
