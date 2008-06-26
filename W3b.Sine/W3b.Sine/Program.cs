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
using Cult = System.Globalization.CultureInfo;

namespace W3b.Sine {
	
	public static class Program {
		
		private static ExpressionStack _stack;
		
		public static Int32 Main(string[] args) {
			
			/*Double theta;
			
			while( (theta = PromptTheta()) != Double.NegativeInfinity) {
				DoWork(theta);
			}
			
			return 0;*/
			
			Console.WriteLine("David Rees' \"GetDavidIntoBirminghamUni\" Mathematical Expression Evaluator");
			Console.WriteLine("...yes, it supports arbitrary precision!");
			
			PrintHelp();
			
			Console.WriteLine();
			
			_stack = new ExpressionStack();
			ExpressionStack.PrintOperations = true;
			
			RenderPrompt();
			
			while(EvaluateCommand()) {
			}
			
			return 0;
			
			
		}
		
		private static void PrintHelp() {
			
			Console.WriteLine();
			Console.WriteLine("Syntax: To represent large numbers like 10 to the power 40, type \"1E40\"");
			Console.WriteLine("        Operations like + and * are entered by putting 'add *' then the number. Whitespace is ignored");
			Console.WriteLine("        Trigonometric and Factorial functions have no arguments, just type \"sin\"");
			Console.WriteLine("");
			Console.WriteLine("Recognised Tokens:");
			Console.WriteLine("        Commands :  add, rem, clear, eval, test, help, quit");
			Console.WriteLine("        Operators:  +, -, *, /, %, ^, !");
			Console.WriteLine("        Functions:  fac, sin, cos, tan, csc, sec, cot");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			Console.WriteLine("        add  :  Adds the following expression to the expression stack");
			Console.WriteLine("               Example: \"add +1\", \"add Sin\"");
			Console.WriteLine("        rem  :  Removes the last expression from the stack");
			Console.WriteLine("        clear:  Removes all expression from the stack");
			Console.WriteLine("        eval :  Evalues the stack and returns the result, but maintains stack state.");
			Console.WriteLine("        test :  Runs through test cases.");
			Console.WriteLine("        quit :  Terminates the program.");
			Console.WriteLine("        help :  Shows this message.");
			Console.WriteLine();
			Console.WriteLine("Function Documentation:");
			Console.WriteLine("        Trig Functions:  The trigonometric functions have no arguments. They take whatever number was just generated.");
			Console.WriteLine("        Power:  Pow requires a signed integer.");
			Console.WriteLine("                Example: \"^12\", \"^-2\"");
			Console.WriteLine("        Factorial:  Takes no arguments.");
			Console.WriteLine("                Example: \"!\", \"fac\"");
			Console.WriteLine();
			
		}
		
		private static Boolean EvaluateCommand() {
			
			String command = Console.ReadLine().Trim().ToLower(Cult.InvariantCulture);
			
			switch(command) {
				case "rem":
					_stack.Pop();
					return true;
				case "clear":
					_stack.Clear();
					return true;
				case "test":
					BigNumTests.Test();
					return true;
				case "quit":
					return false;
				case "help":
					PrintHelp();
					return true;
				case "eval":
					BigNum result = _stack.Evaluate();
					if(!ExpressionStack.PrintOperations)
						Console.WriteLine(" = " + result.ToString() );
					return true;
			}
				
			if(command.StartsWith("add ", StringComparison.OrdinalIgnoreCase)) {
				
				command = command.Substring(4);
				
				try {
					
					Expression expr = new Expression( command );
					_stack.Push( expr );
					
				} catch(FormatException fex) {
					Console.WriteLine("Unrecognised command. Are you missing the operator? (" + fex.Message + ")");
				}
				
			} else {
				
				Console.WriteLine("Unrecognised command.");
			}
			
			return true;
			
		}
		
		private static Double PromptTheta() {
			
			Console.WriteLine("Argument theta (radians)? Q to quit"); // TODO: Maybe accept simple expressions involving Pi?
			Boolean isOk = false;
			Double theta = 0;
			do {
				String input = Console.ReadLine();
				if(input == "Q" || input == "q" ) return Double.NegativeInfinity;;
				if(! (isOk = Double.TryParse( input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out theta))) {
					Console.WriteLine("Try again.");
				}
			} while(!isOk);
			
			return theta;
			
		}
		
		private static void DoWork(Double theta) {
			
			Console.WriteLine( "Sine:\t".PadLeft(12)     +  Math.Sine   ( theta ) );
			Console.WriteLine( "Cosine:\t".PadLeft(12)   +  Math.Cosine ( theta ) );
			Console.WriteLine( "Tangent:\t".PadLeft(12)  +  Math.Tangent( theta ) );
			
			Console.WriteLine( "Cosec:\t".PadLeft(12)     + Math.Cosec( theta ) );
			Console.WriteLine( "Secant:\t".PadLeft(12)    + Math.Secant( theta ) );
			Console.WriteLine( "Cotangent:\t".PadLeft(12) + Math.Cotangent( theta ) );
			
		}
		
		private static String[] _prompts = new String[] {
			"At your bidding, master",
			"What is my bidding?",
			"Sir?",
			"Happy to Help",
			"My leige?",
			"Your eminance",
			"I am programmed to be polite",
			"The indefinite integral of (1/Cabin)",
			"If you go to a bookstore and ask them where the 'Self-Help' section is, would that defeat the purpose?",
			"I plan on living forever. So far, so good.",
			"I always wanted to be a procrastinator, never got around to it.",
			"If space & time are the same as Einstein said, can you be five miles late?",
		};
		
		private static Random _rnd = new Random();
		
		private static void RenderPrompt() {
			
			Int32 idx = _rnd.Next(0, 13); // 0-12
			
			if(idx == 12) {
				RenderClippy(4, "It looks like you're trying to enter an arithmetic expression.", "Would you like help with that?");
			} else {
				Console.WriteLine( _prompts[idx] );
			}
			
		}
		private static void RenderClippy(Int32 leftMargin, String message, String question) {
			
			String messageLine1 = message, messageLine2 = String.Empty, messageLine3 = String.Empty, question1 = question, question2 = String.Empty;
			
			if(messageLine1.Length > 28) {
				messageLine2 = messageLine1.Substring(28);
				messageLine1 = messageLine1.Substring(0, 28);
			}
			if(messageLine2.Length > 28) {
				messageLine3 = messageLine2.Substring(28); if(messageLine2.Length > 28) messageLine2 = messageLine2.Substring(0, 28);
				messageLine2 = messageLine2.Substring(0, 28);
			}
			
			if(question1.Length > 28) {
				question2 = question1.Substring(28); if(question2.Length > 28) question2 = question2.Substring(0, 28);
				question1 = question1.Substring(0, 28);
			}
			
			messageLine1 = messageLine1.PadRight(28);
			messageLine2 = messageLine2.PadRight(28);
			messageLine3 = messageLine3.PadRight(28);
			question1    = question1.PadRight(28);
			question2    = question2.PadRight(28);
			
			System.IO.TextWriter wrt = Console.Out;
			
			wrt.WriteLine( Margin(leftMargin) + @" __" );
			wrt.WriteLine( Margin(leftMargin) + @"/  \       _____________________________" );
			wrt.WriteLine( Margin(leftMargin) + @"|  |      /                             \" );
			wrt.WriteLine( Margin(leftMargin) + @"@  @      | " + messageLine1 +         "|" );
			wrt.WriteLine( Margin(leftMargin) + @"|| ||  <--| " + messageLine2 +         "|" );
			wrt.WriteLine( Margin(leftMargin) + @"|| ||     | " + messageLine3 +         "|");
			wrt.WriteLine( Margin(leftMargin) + @"|\_/|     |                             |" );
			wrt.WriteLine( Margin(leftMargin) + @"\___/     | " + question1    +         "|" );
			wrt.WriteLine( Margin(leftMargin) + @"          | " + question2    +         "|" );
			wrt.WriteLine( Margin(leftMargin) + @"          \____________________________/" );
			
		}
		private static String Margin(Int32 leftMargin) {
			return "".PadLeft(leftMargin);
		}
		
	}
}
