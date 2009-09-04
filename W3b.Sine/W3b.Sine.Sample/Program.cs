using System;
using Cult = System.Globalization.CultureInfo;
using System.Collections.Generic;

namespace W3b.Sine {
	
// TODO Ideas:
// a) Change the symbols dictionary from <String,BigNum> to <String,Expression>, which means it'll still work for constant values
// b) Have two entry modes: evaluation or command
//     i) command mode would do stuff like alter or view the symbol/function dictionary, and give it a blue background?
// c) Create a FractionBigNum class which stores numbers as a numerator/denominator pair
// d) Add in support for native functions
	
	public static class Program {
		
		public static Int32 Main(String[] args) {
			
			if(args.Length > 0) {
				
				String one = args[0];
				switch(one.ToUpperInvariant()) {
					case "?":
					case "/?":
					case "-?":
					case "HELP":
					case "/HELP":
					case "-HELP":
						
						PrintHelp();
						break;
						
					case "TEST":
					case "/TEST":
					case "-TEST":
						
						BigNumTests.Test();
						break;
						
					default:
						
						// HACK: Remove the path from the command line, using quote-finding isn't doing it right
						String expr = Environment.CommandLine;
						expr = expr.Substring( expr.LastIndexOf('"') + 2 ); // +1 to clear the " and another to clear the space
						
						EvaluateExpression( expr );
						break;
				}
			
			} else {
				
				while(PromptUser()) {
				}
				
			}
			
			return 0;
		}
		
		private static void PrintHelp() {
			
			Console.WriteLine("Commands:");
			Console.WriteLine("\tq - Quit");
			Console.WriteLine("\tadd symbolName = <expr>");
			Console.WriteLine("\t\tAdd an expression's value to the dictionary (expressions are not reevaluated, use functions instead)");
			Console.WriteLine("\trem symbolName");
			Console.WriteLine("\t\tRemove a symbol to the dictionary");
			Console.WriteLine();
			Console.WriteLine("\tAny other input is interpreted as an expression. For this reason don't use 'add' or 'rem' as symbol names");
			Console.WriteLine();
			Console.WriteLine("Command-line options:");
			Console.WriteLine("\tSample.exe /help");
			Console.WriteLine("\t\tPrints this message");
			Console.WriteLine("\tSample.exe /test");
			Console.WriteLine("\t\tRuns BigNum tests");
			Console.WriteLine("\tSample.exe <expr>");
			Console.WriteLine("\t\tEvaluates <expr> and quits");
			
		}
		
		private static Int32 _count;
		private static Dictionary<String,BigNum> _symbols = new Dictionary<String,BigNum>();
		
		private static Boolean PromptUser() {
			
			Console.WriteLine("Enter expression " + (_count++).ToString() + ", or 'q' to quit" );
			
			String s = Console.ReadLine();
			if( s == "q" ) return false;
			
			if( s.StartsWith("add ", StringComparison.OrdinalIgnoreCase) ) {
				
				String name = s.Substring(4, s.IndexOf('=') - 5 ).Trim();
				String expr = s.Substring( s.IndexOf('=') + 1 ).Trim();
				
				try {
					
					Expression xp = new Expression( expr );
					BigNum ret = xp.Evaluate( _symbols );
					_symbols.Add( name, ret );
					Console.WriteLine("Added: " + name + " = " + ret.ToString() );
					
				} catch(Exception ex) {
					
					PrintException(ex);
				}
				
			} else if( s.StartsWith("rem ", StringComparison.OrdinalIgnoreCase) ) {
				
				String name = s.Substring(4);
				
				_symbols.Remove( name );
				
				Console.WriteLine("Removed: " + name);
				
			} else if( String.Equals(s, "Help", StringComparison.OrdinalIgnoreCase) ) {
				
				PrintHelp();
				
			} else {
				
				EvaluateExpression( s );
			}
			
			return true;
			
		}
		
		private static void EvaluateExpression(String expression) {
			
			try {
				
				Expression expr = new Expression( expression );
				BigNum ret = expr.Evaluate( _symbols );
				
				Console.WriteLine("Result: " + ret.ToString() );
				
			} catch(Exception ex) {
				
				PrintException(ex);
			}
			
		}
		
		private static void PrintException(Exception ex) {
			
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\tException during evaluation");
			
			while(ex != null) {
				
				Console.Write('\t');
				Console.Write(ex.GetType().FullName);
				Console.Write(" - ");
				Console.WriteLine(ex.Message);
				
				String[] lines = ex.StackTrace.Split('\n');
				foreach(String line in lines) {
					
					Console.Write('\t');
					Console.WriteLine(line);
				}
				
				Console.WriteLine();
				
				ex = ex.InnerException;
			}
			
			Console.ResetColor();
			
		}
		
	}
	
}
