using System;
using System.Collections.Generic;
using System.Text;

namespace W3b.Sine {
	
	public class BigInt : BigNum {
		
		// TODO: Make use of the IntX library
		
		public BigInt(long number) {
		}
		
		public BigInt(String number) {
		}
		
		public override BigNumFactory Factory {
			get { return BigIntFactory.Instance; }
		}
		
		public override BigNum Clone() {
			throw new NotImplementedException();
		}
		
		public override int CompareTo(object obj) {
			throw new NotImplementedException();
		}
		
		public override int CompareTo(BigNum other) {
			throw new NotImplementedException();
		}
		
		public override bool Equals(object obj) {
			throw new NotImplementedException();
		}
		
		public override bool Equals(BigNum other) {
			throw new NotImplementedException();
		}
		
		public override int GetHashCode() {
			throw new NotImplementedException();
		}
		
		public override string ToString() {
			throw new NotImplementedException();
		}
		
		public override bool IsZero {
			get { throw new NotImplementedException(); }
		}
		
		public override bool Sign {
			get {
				throw new NotImplementedException();
			}
		}
		
		protected override BigNum Add(BigNum other) {
			throw new NotImplementedException();
		}
		
		protected override BigNum Multiply(BigNum multiplicand) {
			throw new NotImplementedException();
		}
		
		protected override BigNum Divide(BigNum divisor) {
			throw new NotImplementedException();
		}
		
		protected override BigNum Modulo(BigNum divisor) {
			throw new NotImplementedException();
		}
		
		protected override BigNum Negate() {
			throw new NotImplementedException();
		}
		
		protected internal override BigNum Absolute() {
			throw new NotImplementedException();
		}
		
		protected internal override BigNum Floor() {
			throw new NotImplementedException();
		}
		
		protected internal override BigNum Ceiling() {
			throw new NotImplementedException();
		}
		
		protected internal override void Truncate(int significance) {
			throw new NotImplementedException();
		}
	}
}
