using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace W3b.Sine {
	
	public class ExpressionContext {
		
		private static Function[] _builtIn = {
			
		};
		
		public ReadOnlyCollection<Symbol> Symbols { get; private set; }
		
		
		
	}
	
	public abstract class Symbol {
		
		public String Name { get; protected set; }
		
		public abstract void ToString();
		
	}
	
	public class Variable : Symbol {
		
		
		
	}
	
	public abstract class Function : Symbol {
		
		public FunctionType               FunctionType   { get; protected set; }
		public ReadOnlyCollection<String> ParameterNames { get; protected set; }
		
		public abstract BigNum Evaluate(params BigNum[] arguments);
	}
	
	public class BuiltInFunction : Function {
		
		private Delegate _impl;
		
		public BuiltInFunction(String name, Delegate implementation, params String[] parameterNames) {
			
			Name           = name;
			FunctionType   = FunctionType.BuiltIn;
			ParameterNames = new ReadOnlyCollection<String>(parameterNames);
			
			_impl = implementation;
		}
		
		public override BigNum Evaluate(params BigNum[] arguments) {
			
			Object ret = _impl.DynamicInvoke( arguments );
			
			return ret as BigNum;
		}
		
	}
	
	public class UserFunction : Function {
		
		private Expression _expr;
		
		public UserFunction(String name, String expression, params String[] parameterNames) {
			
			Name           = name;
			FunctionType   = FunctionType.User;
			ParameterNames = new ReadOnlyCollection<String>(parameterNames);
			
			_expr = new Expression( expression, );
		}
		
		public override BigNum Evaluate(params BigNum[] arguments) {
			
			_expr.Evaluate( null );
			
		}
		
	}
	
	public delegate BigNum BigNumFunction1(BigNum x);
	public delegate BigNum BigNumFunction2(BigNum x, BigNum y);
	public delegate BigNum BigNumFunction3(BigNum x, BigNum y, BigNum z);
	
	public enum FunctionType {
		BuiltIn,
		Predefined,
		User
	}
	
}
