using System;
using BitArray = System.Collections.BitArray;
using StringBuilder = System.Text.StringBuilder;

namespace W3b.Sine {
	
	/// <summary>An immutable big real number class.</summary>
	/// <remarks>It's a class rather than struct because of reference type members</remarks>
	public class BigNumBinary : /* IComparable<BigNum>, */ ICloneable {
		
		// the two bitarrays are normalised after construction, for the sake of two's compliment operation. The fractional part shares the same two's complemtent "state" with integerpart
		
		/// <summary>The integer part of the number.</summary>
		/// <remarks>The integer bitarray works from right-to-left, that is _integerPart[0] is the least significant, _integerPart[n] being the most significant</remarks>
		private BitArray _integerPart;
		
		/// <summary>The fractional part of the number.</summary>
		/// <remarks>The fraction bitarray works from left-to-right, that is _fractionPart[0] is the most significant, _fractionPart[n] being the least significant</remarks>
		private BitArray _fractionPart;
		
//                    <----          |       ---->
//                      integer part | fraction part
//                        9876543210 | 0123456789
		
#region Constructors
		
		public BigNumBinary() {
			// init to zero
			_integerPart  = new BitArray(0, false);
			_fractionPart = new BitArray(0, false);
		}
		
		public BigNumBinary(Int64 value) : this() {
			
			_integerPart = new BitArray(64 * 8, false);
			
			Int64 mask = 0;
			for(int i=0;i<64*8;i++) {
				
				mask = (Int64)Math.Power(2, i);
				
				if( (value & mask) == mask )
					_integerPart[i] = true;
			}
			
			Normalise( value < 0 );
		}
		
		private BigNumBinary(BitArray integerPart, BitArray fractionalPart) {
			
			if(integerPart    == null) integerPart    = new BitArray(0, false);
			if(fractionalPart == null) fractionalPart = new BitArray(0, false);
			
			_integerPart  = integerPart;
			_fractionPart = fractionalPart;
		}
		
#endregion
		
		/// <summary>Removes unnecessary leading (or following) zeros in the bitarrays</summary>
		/// <param name="isNegative">When false, normalising will leave a 0 bit at the start of the integer bitarray for Two's Complement.</param>
		private void Normalise(Boolean isNegative) {
			
			/////////////////////////
			// Integer part
			/////////////////////////
			int i; // Last index, counting from end to beginning (left-to-right).
			for(i = _integerPart.Length - 1; i>=0;i--) {
				
				if(isNegative) {
					if(!_integerPart[i]) break;
				} else {
					if(_integerPart[i]) break;
				}
			}
			if(i == -1)
				_integerPart.Length = 1;
			else if(isNegative)
				_integerPart.Length = i + 2; // truncate right up to the location of the last 1 (since that's the sign bit)
			else
				_integerPart.Length = i + 2; // leave an extra 0
			
			/////////////////////////
			// Fractional part
			/////////////////////////
			for(i = _fractionPart.Length -1; i>=0;i--) {
				
				if(_fractionPart[i]) break;
			}
			_fractionPart.Length = i + 1;
			
		}
		
		/// <summary>Concatenates the two bitarrays together for arithmetic operations. Returns the index of the radix point as an output parameter.</summary>
		private static BitArray Join(BitArray integer, BitArray fractional, out Int32 radixIndex) {
			
			radixIndex = integer.Length;
			
			BitArray joined = new BitArray( integer );
			
			if(integer.Length > 0) {
				
				joined.Length += fractional.Length;
				
				for(int i=0;i<fractional.Length;i++) {
					
					joined[radixIndex + i] = fractional[radixIndex + i];
					
				}
			
			}
			
			return joined;
			
		}
		
#region Operation Implementations
		
		public static BigNumBinary Add(BigNumBinary a, Int64 b) {
			return Add( a, new BigNumBinary(b) );
		}
		
		public static BigNumBinary Add(BigNumBinary a, BigNumBinary b) {
			
			// RULES:
			// 1) Only works on integers
			// 2) Two's Complement addition
			
			a = a.Clone();
			b = b.Clone();
			
			Boolean aIsNegative = a.IsNegative;
			Boolean bIsNegative = b.IsNegative;
			
			BitArray ai = a._integerPart , bi = b._integerPart;
			BitArray af = a._fractionPart, bf = b._fractionPart;
			
			// need to normalise the bitarrays so they're the same length with appropriate sign bit
			NormaliseArrays(ai, bi);
			NormaliseArrays(af, bf);
			
			Int32 aRidx = -1;
			BitArray aj = Join(a._integerPart, a._fractionPart, out aRidx);
			Int32 bRidx = -1;
			BitArray bj = Join(b._integerPart, b._fractionPart, out bRidx);
			
			if(aj.Length != bj.Length) throw new Exception("Joined bitarrays different length"); // not meant to happen, the normalization thing should ensure both are the same length
			
			BitArray working = new BitArray( aj.Length );
			
			Boolean carry = false;
			for(int i=0;i<aj.Length;i++) {
				
				Boolean ad = GetBinaryDigit(i, aj, aIsNegative);
				Boolean bd = GetBinaryDigit(i, bj, bIsNegative);
				
				if( ad && bd ) {
					working[i] = true;
				}
				
				Boolean d = false; // current digit
				
				if(ad && bd) { // carry
					
					if(carry) {
						d = true; // still carry
					} else {
						d = false;
						carry = true;
					}
					
				} else if(ad ^ bd) { // 1, 0 || 0, 1
					
					if(carry) {
						d = false;
					} else {
						d = true;
					}
					
				} else if(carry) { // 0, 0
					d = true;
					carry = false;
				}
				
				working[i] = d;
				
			}
			
			
			
			if(aIsNegative ^ bIsNegative) {
				
			} else {
				if(carry) {
					// increase a by 1 and set it to 1
					working.Length++;
					working[working.Length - 1] = true;
				}
			}
			
			// then split up
			//BitArray newInteger   = new BitArray(;
			//BitArray newFraction;
			
			return new BigNumBinary( working, null );
			
		}
		
		/// <summary>Makes both bitarrays the same length. If the shortest is negative it pads it with 1s, otherwise 0s</summary>
		private static void NormaliseArrays(BitArray a, BitArray b) {
			
			// pad shortest to the left...
				// ...with zeros if positive, ones if negative
			
			BitArray shortest = a;
			BitArray longest  = b;
			if(b.Length < a.Length) { shortest = b; longest = a; }
			
			Int32 oldLength = shortest.Length;
			if(longest.Length == 0) return; // no point doing anything if the longest (therefore also the shortest) is empty
			Boolean shortestIsNegative = shortest[shortest.Length-1];
			
			shortest.Length = longest.Length; // will be init'd to zero
			
			if(shortestIsNegative)
				// if the shortest is negative, need to make those bits 1
				for(int i=oldLength-1;i<shortest.Length;i++) {
					shortest[i] = true;
				}
			
			// done
			
		}
		
		private static Boolean GetBinaryDigit(Int32 idx, BitArray array, Boolean isNegative) {
			if(idx > array.Length - 1) return isNegative;
			else return array[idx];
		}
		
		public static BigNumBinary Multiply(BigNumBinary a, BigNumBinary b) {
			
			// http://en.wikipedia.org/wiki/Booth%27s_multiplication_algorithm
			
			throw new NotImplementedException();
		}
		
		public static BigNumBinary Divide(BigNumBinary dividend, BigNumBinary divisor) {
			
			if(divisor == new BigNumBinary(0)) throw new DivideByZeroException();
			
			// http://courses.cs.vt.edu/~cs1104/Division/ShiftSubtract/Shift.Subtract.html
			
			throw new NotImplementedException();
		}
		
		public static BigNumBinary Mod(BigNumBinary a, BigNumBinary b) {
			
			// a % b == a  - ( b * Floor[a / b] )
			
			BigNumBinary retVal = Divide(a, b);
			retVal = BigNumBinary.Floor( retVal );
			retVal = BigNumBinary.Multiply( b, retVal );
			retVal = BigNumBinary.Add( a, retVal.Negate() ); // equivalent to "a - retVal"
			
			return retVal;
			
		}
		
		public static Boolean Equals(BigNumBinary a, BigNumBinary b) {
			
			return a._integerPart == b._integerPart && a._fractionPart == b._fractionPart;
			
		}
		
		public override Boolean Equals(object obj) {
			BigNumBinary b = obj as BigNumBinary;
			if(b != null) return Equals(this, b);
			return false;
		}
		
		public static BigNumBinary Absolute(BigNumBinary a) {
			BigNumBinary retVal = a.Clone();
			
			return retVal;
		}
		
		public static BigNumBinary Floor(BigNumBinary a) {
			
			BigNumBinary retVal = a.Clone();
			
			Boolean fractionalPartWasNonZero = retVal._fractionPart.Length > 0; // ugly hack, but assuming the _fractionalPart is always normalised this isn't a problem
			
			retVal._fractionPart = new BitArray(0, false); // reset the fractional part to zero
			
			if( a.IsNegative && fractionalPartWasNonZero ) {
				// if _fractionalPart is nonzero minus 1 from the integer part
				retVal = retVal.Add( new BigNumBinary(-1) );
			}
			
			return retVal;
		}
		
		/// <summary>Inverts the sign of a.</summary>
		public static BigNumBinary Neg(BigNumBinary a) {
			
			throw new NotImplementedException();
		}
		
		public static BigNumBinary Not(BigNumBinary a) {
			
			BitArray newFrac = a._fractionPart;
			BitArray newInt  = a._integerPart;
			
			for(int i=0;i<newFrac.Length;i++) {
				newFrac[i] = !newFrac[i];
			}
			for(int i=0;i<newInt.Length;i++) {
				newInt[i] = !newInt[i];
			}
			
			BigNumBinary retVal = new BigNumBinary( newInt, newFrac );
			
			return retVal;
			
		}
		
#endregion
		
#region Instance Operations
		
		public BigNumBinary Add(BigNumBinary b) {
			return BigNumBinary.Add(this, b);
		}
		
		public BigNumBinary Subtract(BigNumBinary b) {
			return BigNumBinary.Add(this, b.Negate());
		}
		
		public BigNumBinary Negate() {
			return BigNumBinary.Neg(this);
		}
		
		public BigNumBinary Multiply(BigNumBinary b) {
			return BigNumBinary.Multiply(this, b);
		}
		
		public BigNumBinary Divide(BigNumBinary b) {
			return BigNumBinary.Divide(this, b);
		}
		
		public BigNumBinary Mod(BigNumBinary b) {
			return BigNumBinary.Mod(this, b);
		}
		
#endregion
		
#region Operator Overloads
		
		
		
#endregion
		
#region Utility Methods
		
		/// <summary>Gets the index of the most significant digit of the integer part.</summary>
		private Int32 IntMsd {
			get { return _integerPart.Length - 1; }
		}
		
		private Int32 FraLsd {
			get { return _fractionPart.Length - 1; }
		}
		
		public Boolean IsNegative {
			get {
				// integer part uses two's complement, so most-significant-digit will be 1
				return _integerPart[ IntMsd ] == true;
			}
		}
		
		public BigNumBinary Clone() {
			
			BigNumBinary clone = new BigNumBinary();
			clone._integerPart  = new BitArray( this._integerPart );
			clone._fractionPart = new BitArray( this._fractionPart );
			
			return clone;
		}
		object ICloneable.Clone() {
			return Clone();
		}
		
		public override Int32 GetHashCode() {
			return _integerPart.GetHashCode() ^ _fractionPart.GetHashCode();
		}
		
		/// <summary>Returns the Base 10 representation of the integer part of this number.</summary>
		public override string ToString() {
			
			// according to http://sandbox.mc.edu/~bennet/cs110/tc/tctod.html
			// if it's non-negative, simply convert to decimal
			// if it's got 1 at the sign bit, it's complicated
			
			BigNumBinary a = this.Clone();
			
			Boolean isNegative = a.IsNegative;
			
			if( isNegative ) {
				
				// make it positive by inverting the bits, then adding 1
				// then convert to decimal
				
				a = Not( a );
				a = Add(a, 1);
				// a is now the magnitude, add signage later
			}
			
			// for unsigned:
			// for each binary digit
			Int64 value = 0; // only works up to 64-bit numbers
			BitArray bits = a._integerPart;
			if( bits.Length > 64 ) throw new NotImplementedException();
			
			for(int i=0;i<bits.Length;i++) {
				if( bits[i] ) {
					value += (Int64)Math.Power(2, i);
				}
			}
			
			// add sign
			String retVal = value.ToString();
			retVal = (isNegative ? "-" : /* '+' */ String.Empty) + retVal;
			
			return retVal;
			
			
		
		}
		
		private Boolean GetBinaryDigit(Int32 idx) {
			if(idx > IntMsd ) return false;
			return _integerPart[idx];
		}
		
#endregion
		
#region ToString
		
		/// <summary>Returns the *raw in-memory* representation of a bitarray.</summary>
		public static String GetMemory(BitArray a) {
			
			// warning: this method is a mess and might not return the right answer
			
			StringBuilder sb = new StringBuilder();
			//sb.Append( IsNegative ? '-' : '+' );
			
			// Integer part
			
			for(int i=0;i<a.Length;i++) {
				sb.Append( a[i] ? '1' : '0' );
			}
			
		/*	// Fraction part
			// get index of last non-zero
			Int32 last1Index = -1;
			for(int i=0;i<_fractionPart.Length;i++) {
				if(_fractionPart[i]) last1Index = i;
			}
			
			if(last1Index > 0) {
				
				sb.Append('.');
				
				for(int i=0;i<_fractionPart.Length;i++) {
					if(i == last1Index) break;
					if(_fractionPart[i]) sb.Append('1');
					else sb.Append('0');
				}
				
			} */
			
			return sb.ToString();
			
		}
		
		public String GetIntegerPartBase2String() {
			
			StringBuilder sb = new StringBuilder();
			
			for(int i=IntMsd;i>=0;i--) {
				sb.Append( _integerPart[i] ? '1' : '0' );
			}
			
			return sb.ToString();
			
		}
		
#endregion
	
	}
	
}
