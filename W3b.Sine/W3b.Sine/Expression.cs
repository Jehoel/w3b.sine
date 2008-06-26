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
	
	/// <summary>Represents a simple expression stack (LIFO).</summary>
	public class ExpressionStack {
		
		private Stack<Expression> _expressions;
		
		private static Boolean _printOperations;
		
		private BigNum _numberSoFar;
		
		public ExpressionStack() {
			_expressions = new Stack<Expression>();
		}
		
		/// <summary>Evaluates the stacked expressions in a FIFO (aka LILO) order.</summary>
		public BigNum Evaluate() {
			
			_numberSoFar = BigNum.CreateInstance();
			
			Expression[] expressions = _expressions.ToArray();
			
			for(int i=expressions.Length-1;i>=0;i--) {
				
				Expression expr = expressions[i];
				
				BigNum val = null;
				if( expr.Value != Double.NegativeInfinity ) 
					val = BigNum.CreateInstance( expr.Value );
				
				switch(expr.Function) {
					case MathFunction.Add:
						_numberSoFar = _numberSoFar.Add( val );          break;
					case MathFunction.Sub:
						_numberSoFar = _numberSoFar.Add( val.Negate() ); break;
					case MathFunction.Mul:
						_numberSoFar = _numberSoFar.Multiply( val );     break;
					case MathFunction.Div:
						_numberSoFar = _numberSoFar.Divide( val );       break;
					case MathFunction.Mod:
						_numberSoFar = _numberSoFar.Modulo( val );       break;
					case MathFunction.Pow:
						_numberSoFar = _numberSoFar.Power( Convert.ToInt32( expr.Value ) ); break;
					case MathFunction.Fac:
						_numberSoFar = _numberSoFar.Factorial();         break;
					
					case MathFunction.Sin:
						_numberSoFar = _numberSoFar.Sine();              break;
					case MathFunction.Cos:
						_numberSoFar = _numberSoFar.Cosine();            break;
					case MathFunction.Tan:
						_numberSoFar = _numberSoFar.Tangent();           break;
					case MathFunction.Csc:
						_numberSoFar = _numberSoFar.Cosec();             break;
					case MathFunction.Sec:
						_numberSoFar = _numberSoFar.Secant();            break;
					case MathFunction.Cot:
						_numberSoFar = _numberSoFar.Cotangent();         break;
				}
				
				if(_printOperations)
					Console.WriteLine( "<-- " + expr.ToString() + " = " + _numberSoFar.ToString() );
				
			}
			
			//_expressions.Clear();
			
			return _numberSoFar;
			
		}
		
		/// <summary>Pushes an expression onto the stack.</summary>
		public void Push(Expression expression) {
			_expressions.Push(expression);
			if(_printOperations) Console.WriteLine("--> " + expression.ToString() );
		}
		
		/// <summary>Removes the last entered expression from the stack.</summary>
		public Expression Pop() {
			Expression expr = _expressions.Pop();
			if(_printOperations) Console.WriteLine("<-- " + expr.ToString() );
			return expr;
		}
		
		public void Clear() {
			_expressions.Clear();
			if(_printOperations) Console.WriteLine("<-- All");
		}
		
		/// <summary>Set to True to print operations to Console as they happen.</summary>
		public static Boolean PrintOperations {
			get { return _printOperations; }
			set { _printOperations = value; }
		}
	}
	
	public class Expression {
		
		private MathFunction _function  = MathFunction.None;
		private Double   _value     = Double.NegativeInfinity;
		
		public Expression(String text) {
			
			if( text == null) throw new ArgumentNullException("text");
			if( text.Length == 0) throw new ArgumentException("text must be at least 1 character long.");
			
			text = text.Replace(" ", "").ToLower(Cult.InvariantCulture);
			
			/////////////////////////////////
			// Look for unary functions (their only argument is whatever's on the stack already)
			/////////////////////////////////
			if(text.Length == 3) {
				switch(text) {
					case "sin": _function = MathFunction.Sin; return;
					case "cos": _function = MathFunction.Cos; return;
					case "tan": _function = MathFunction.Tan; return;
					case "csc": _function = MathFunction.Csc; return;
					case "sec": _function = MathFunction.Sec; return;
					case "cot": _function = MathFunction.Cot; return;
				}
			} else if(text.Length == 1) {
				if(text == "!") {
					_function = MathFunction.Fac; return;
				} else {
					throw new FormatException("text could not be parsed as an expression.");
				}
			}
			
			/////////////////////////////////
			// Look for binary functions (second argument provided in the text)
			/////////////////////////////////
			Char c = text[0];
			switch(c) {
				case '+': _function = MathFunction.Add; break;
				case '-': _function = MathFunction.Sub; break;
				case '*': _function = MathFunction.Mul; break;
				case '/': _function = MathFunction.Div; break;
				case '%': _function = MathFunction.Mod; break;
				case '^': _function = MathFunction.Pow; break;
				default:
					throw new FormatException("text could not be parsed as an expression.");
			}
			
			// then the number part
			String number = text.Substring(1);
			if(!Double.TryParse(number, out _value)) {
				throw new FormatException("text could not be parsed as an expression.");
			}
			
		}
		
		public MathFunction Function {
			get { return _function; }
		}
		
		public Double Value {
			get { return _value; }
		}
		
		public override string ToString() {
			
			switch(_function) {
				case MathFunction.Add: return "+" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Sub: return "-" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Mul: return "*" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Div: return "/" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Mod: return "%" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Pow: return "^" + _value.ToString(Cult.InvariantCulture);
				case MathFunction.Fac: return "!";
				default:
					if(_value != Double.NegativeInfinity) {
						return _function.ToString() + "(" + _value.ToString(Cult.InvariantCulture) + ")";
					} else {
						return _function.ToString();
					}
			}
			
		}
		
	}
	
	public enum MathFunction {
		None,
		Add,
		Sub,
		Mul,
		Div,
		Mod,
		/// <summary>Factorial</summary>
		Fac,
		/// <summary>Power</summary>
		Pow,
		/// <summary>Sine</summary>
		Sin,
		/// <summary>Cosine</summary>
		Cos,
		/// <summary>Tan</summary>
		Tan,
		/// <summary>Cosec</summary>
		Csc,
		/// <summary>Secant</summary>
		Sec,
		/// <summary>Cotangent</summary>
		Cot
	}
	
}
