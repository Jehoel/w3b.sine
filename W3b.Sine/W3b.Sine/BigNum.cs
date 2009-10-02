using System;
using System.Collections.Generic;
using System.Text;

using Cult = System.Globalization.CultureInfo;

namespace W3b.Sine {
	
	/// <summary>An immutable arbitary-precision number class.</summary>
	public abstract class BigNum : IComparable, IComparable<BigNum>, IEquatable<BigNum>, ICloneable {
		
#region Equality and Comparisons
		
		public abstract BigNum Clone();
		
		Object ICloneable.Clone() {
			return Clone();
		}
		
		public abstract          Int32   CompareTo(Object obj);
		public abstract          Int32   CompareTo(BigNum other);
		
		public abstract override Boolean Equals(Object obj);
		public abstract          Boolean Equals(BigNum other);
		
		public abstract override Int32   GetHashCode();
		public abstract override String  ToString();
		
#endregion
		
#region Must-Implement Math Operations
		
		public abstract Boolean IsZero { get; }
		public abstract Boolean Sign   { get; set; }
		
		protected abstract BigNum Add(BigNum other);
		
		protected abstract BigNum Multiply(BigNum multiplicand);
		
		/// <summary>Divides this instance by the divisor, returning the quotient.</summary>
		protected abstract BigNum Divide(BigNum divisor);
		
		/// <summary>Divides this instance by the divisor, returning the remainder.</summary>
		protected abstract BigNum Modulo(BigNum divisor);
		
		/// <summary>Inverts this instance's sign</summary>
		protected abstract BigNum Negate();
		
		/// <summary>Returns the norm of the number</summary>
		protected internal abstract BigNum Absolute();
		
		protected internal abstract BigNum Floor();
		protected internal abstract BigNum Ceiling();
		
		/// <summary>Removes digit places beyond the specified figure of significance</summary>
		protected internal abstract void Truncate(Int32 significance);
		
#endregion
		
#region Consts
		
		// consts computed by Mathematica, my ComputePi function is inaccurate after 4 digits for some reason
		public static readonly BigNum HalfPi = new BigNumDec( "1.5707963267948966192313216916397514420985846996876" );
		public static readonly BigNum Pi     = new BigNumDec( "3.1415926535897932384626433832795028841971693993751" );
		public static readonly BigNum TwoPi  = new BigNumDec( "6.2831853071795864769252867665590057683943387987502" );
		
		// these consts from Wikipedia, used in the Gamma function
		public static readonly BigNum EulerMascheroni = new BigNumDec("0.57721566490153286060651209008240243104215933593992");
		public static readonly BigNum Euler           = new BigNumDec("2.71828182845904523536");
		public static readonly BigNum SqrtTwo         = new BigNumDec("1.41421356237309504880168872420969807856967187537694807317667973799");
		public static readonly BigNum GoldenRatio     = new BigNumDec("1.6180339887");
		
#endregion
		
#region Creation
		
		public static implicit operator String(BigNum n) {
			return n.ToString();
		}
		public static implicit operator BigNum(Single value) {
			return Create( value );
		}
		public static implicit operator BigNum(Double value) {
			return Create( value );
		}
		public static implicit operator BigNum(Decimal value) {
			return Create( value );
		}
		public static implicit operator BigNum(Int16 value) {
			return Create( value );
		}
		public static implicit operator BigNum(Int32 value) {
			return Create( value );
		}
		public static implicit operator BigNum(Int64 value) {
			return Create( value );
		}
		public static implicit operator BigNum(String value) {
			return Create( value );
		}
		
		/// <summary>Creates an instance of BigNum set to zero.</summary>
		public static BigNum Create() {
			return new BigNumDec(0);
		}
		
		public static BigNum Create(Int16 value) {
			return new BigNumDec(value);
		}
		public static BigNum Create(Int32 value) {
			return new BigNumDec(value);
		}
		public static BigNum Create(Int64 value) {
			return new BigNumDec(value);
		}
		public static BigNum Create(Single value) {
			return new BigNumDec(value);
		}
		public static BigNum Create(Decimal value) {
			return Create( value.ToString(Cult.InvariantCulture) );
		}
		public static BigNum Create(Double value) {
			return new BigNumDec(value);
		}
		public static BigNum Create(String value) {
			return new BigNumDec(value);
		}
		
		public static Boolean TryParse(String s, out BigNum value) {
			
			try {
				
				value = Create( s );
				
			} catch(Exception) {
				
				value = null;
				return false;
			}
			
			return true;
		}
		
#endregion
		
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
		
		public static BigNum operator ^(BigNum a, Int32 b) {
			if(a == null) return null;
			return a.Power( b );
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
		
		public static Boolean operator ==(BigNum a, Int64 b) {
			return Equals(a, b);
		}
		public static Boolean operator !=(BigNum a, Int64 b) {
			return !Equals(a, b);
		}
		
#endregion
		
#region Reference Function Implementation
		
		// operations not needing to be implemented by derived classes, but virtual just in case derived classes have a more efficient implementation
		
		public virtual BigNum Subtract(BigNum other) {
			return Add( other.Negate() );
		}
		
		protected internal virtual BigNum Power(Int32 exponent) {
			
			Int32 pow = exponent < 0 ? -exponent : exponent; // abs(exponent);
			
			BigNum retVal = 1;
			for(int i=0;i<pow;i++) {
				retVal = retVal.Multiply( this );
			}
			
			if(exponent < 0) { // reciprocal
				retVal = BigNum.Create(1) / retVal;
			}
			
			return retVal;
			
		}
		
#endregion
		
	}
}
