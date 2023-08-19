using System;
using UnityEngine;

namespace Utage
{
	public class ExpressionToken
	{
		public enum TokenType
		{
			Lpa,
			Rpa,
			Comma,
			Unary,
			Binary,
			Substitution,
			Number,
			Value,
			Function
		}

		private const string Lpa = "(";

		private const string Rpa = ")";

		private const string Comma = ",";

		private const string Not = "!";

		private const string Prod = "*";

		private const string Div = "/";

		private const string Mod = "%";

		public const string Plus = "+";

		public const string Minus = "-";

		private const string GreaterEq = ">=";

		private const string LessEq = "<=";

		private const string Greater = ">";

		private const string Less = "<";

		private const string EqEq = "==";

		private const string NotEq = "!=";

		private const string And = "&&";

		private const string Or = "||";

		private const string Eq = "=";

		private const string PlusEq = "+=";

		private const string MinusEq = "-=";

		private const string ProdEq = "*=";

		private const string DivEq = "/=";

		private const string ModEq = "%=";

		public static readonly ExpressionToken LpaToken = new ExpressionToken("(", false, TokenType.Lpa, 0);

		public static readonly ExpressionToken RpaToken = new ExpressionToken(")", false, TokenType.Rpa, 0);

		public static readonly ExpressionToken CommaToken = new ExpressionToken(",", false, TokenType.Comma, 0);

		public static readonly ExpressionToken UniPlus = new ExpressionToken("+", false, TokenType.Unary, 1);

		public static readonly ExpressionToken UniMinus = new ExpressionToken("-", false, TokenType.Unary, 1);

		private static readonly ExpressionToken[] OperatorArray = new ExpressionToken[23]
		{
			LpaToken,
			RpaToken,
			CommaToken,
			new ExpressionToken(">=", false, TokenType.Binary, 4),
			new ExpressionToken("<=", false, TokenType.Binary, 4),
			new ExpressionToken(">", false, TokenType.Binary, 4),
			new ExpressionToken("<", false, TokenType.Binary, 4),
			new ExpressionToken("==", false, TokenType.Binary, 5),
			new ExpressionToken("!=", false, TokenType.Binary, 5),
			new ExpressionToken("&&", false, TokenType.Binary, 6),
			new ExpressionToken("||", false, TokenType.Binary, 7),
			new ExpressionToken("=", false, TokenType.Substitution, 8),
			new ExpressionToken("+=", false, TokenType.Substitution, 8),
			new ExpressionToken("-=", false, TokenType.Substitution, 8),
			new ExpressionToken("*=", false, TokenType.Substitution, 8),
			new ExpressionToken("/=", false, TokenType.Substitution, 8),
			new ExpressionToken("%=", false, TokenType.Substitution, 8),
			new ExpressionToken("!", false, TokenType.Unary, 1),
			new ExpressionToken("*", false, TokenType.Binary, 2),
			new ExpressionToken("/", false, TokenType.Binary, 2),
			new ExpressionToken("%", false, TokenType.Binary, 2),
			new ExpressionToken("+", false, TokenType.Binary, 3),
			new ExpressionToken("-", false, TokenType.Binary, 3)
		};

		private string name;

		private bool isAlphabet;

		private TokenType type;

		private int priority;

		private object variable;

		private int numFunctionArg;

		private const string FuncRandom = "Random";

		private const string FuncRandomF = "RandomF";

		private const string FuncCeil = "Ceil";

		private const string FuncCeilToInt = "CeilToInt";

		private const string FuncFloor = "Floor";

		private const string FuncFloorToInt = "FloorToInt";

		public string Name
		{
			get
			{
				return name;
			}
		}

		public TokenType Type
		{
			get
			{
				return type;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
		}

		public object Variable
		{
			get
			{
				return variable;
			}
			set
			{
				variable = value;
			}
		}

		public int NumFunctionArg
		{
			get
			{
				return numFunctionArg;
			}
		}

		public bool IsValueType
		{
			get
			{
				TokenType tokenType = Type;
				if ((uint)(tokenType - 6) <= 1u)
				{
					return true;
				}
				return false;
			}
		}

		public ExpressionToken(string name, bool isAlphabet, TokenType type, int priority, object variable)
		{
			Create(name, isAlphabet, type, priority, variable);
		}

		public ExpressionToken(string name, bool isAlphabet, TokenType type, int priority)
		{
			Create(name, isAlphabet, type, priority, null);
		}

		private void Create(string name, bool isAlphabet, TokenType type, int priority, object variable)
		{
			this.name = name;
			this.isAlphabet = isAlphabet;
			this.type = type;
			this.priority = priority;
			this.variable = variable;
		}

		public static bool CheckSeparator(char c)
		{
			if (!char.IsWhiteSpace(c))
			{
				switch (c)
				{
				case ',':
					break;
				case '!':
				case '%':
				case '&':
				case '(':
				case ')':
				case '*':
				case '+':
				case '-':
				case '/':
				case '<':
				case '=':
				case '>':
				case '|':
					return true;
				default:
					return false;
				}
			}
			return true;
		}

		public static ExpressionToken FindOperator(string exp, int index)
		{
			ExpressionToken[] operatorArray = OperatorArray;
			foreach (ExpressionToken expressionToken in operatorArray)
			{
				if (!expressionToken.isAlphabet && expressionToken.name.Length <= exp.Length - index && exp.IndexOf(expressionToken.name, index, expressionToken.name.Length) == index)
				{
					return expressionToken;
				}
			}
			return null;
		}

		public static ExpressionToken CreateToken(string name)
		{
			if (name.Length == 0)
			{
				Debug.LogError(" Token is enmpty");
			}
			int result;
			if (int.TryParse(name, out result))
			{
				return new ExpressionToken(name, false, TokenType.Number, 0, result);
			}
			float val;
			if (WrapperUnityVersion.TryParseFloatGlobal(name, out val))
			{
				return new ExpressionToken(name, false, TokenType.Number, 0, val);
			}
			bool result2;
			if (bool.TryParse(name, out result2))
			{
				return new ExpressionToken(name, false, TokenType.Number, 0, result2);
			}
			string outStr;
			if (TryParseString(name, out outStr))
			{
				return new ExpressionToken(name, false, TokenType.Number, 0, outStr);
			}
			ExpressionToken token;
			if (TryParseFunction(name, out token))
			{
				return token;
			}
			return new ExpressionToken(name, false, TokenType.Value, 0);
		}

		private static bool TryParseString(string str, out string outStr)
		{
			outStr = "";
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			if (str.Length < 2)
			{
				return false;
			}
			if (str[0] == '"' && str[str.Length - 1] == '"')
			{
				outStr = str.Substring(1, str.Length - 2);
				return true;
			}
			return false;
		}

		public static ExpressionToken OperateSubstition(ExpressionToken value1, ExpressionToken token, ExpressionToken value2, Func<string, object, bool> callbackSetValue)
		{
			value1.variable = CalcSubstition(value1.variable, token, value2.variable);
			if (value1.type == TokenType.Value && !callbackSetValue(value1.name, value1.variable))
			{
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperateSubstition, token.name, value1.variable));
			}
			return value1;
		}

		private static object CalcSubstition(object value1, ExpressionToken token, object value2)
		{
			if (token.name == "=")
			{
				return value2;
			}
			if (value1 is int)
			{
				if (value2 is int)
				{
					return CalcSubstitionSub((int)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcSubstitionSub((int)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcSubstitionSub((int)value1, token, (string)value2);
				}
			}
			else if (value1 is float)
			{
				if (value2 is int)
				{
					return CalcSubstitionSub((float)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcSubstitionSub((float)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcSubstitionSub((float)value1, token, (string)value2);
				}
			}
			else if (value1 is string)
			{
				if (value2 is int)
				{
					return CalcSubstitionSub((string)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcSubstitionSub((string)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcSubstitionSub((string)value1, token, (string)value2);
				}
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcSubstitionSub(int value1, ExpressionToken token, int value2)
		{
			switch (token.name)
			{
			case "+=":
				return value1 + value2;
			case "-=":
				return value1 - value2;
			case "*=":
				return value1 * value2;
			case "/=":
				return value1 / value2;
			case "%=":
				return value1 % value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcSubstitionSub(int value1, ExpressionToken token, float value2)
		{
			switch (token.name)
			{
			case "+=":
				return (float)value1 + value2;
			case "-=":
				return (float)value1 - value2;
			case "*=":
				return (float)value1 * value2;
			case "/=":
				return (float)value1 / value2;
			case "%=":
				return (float)value1 % value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcSubstitionSub(float value1, ExpressionToken token, int value2)
		{
			switch (token.name)
			{
			case "+=":
				return value1 + (float)value2;
			case "-=":
				return value1 - (float)value2;
			case "*=":
				return value1 * (float)value2;
			case "/=":
				return value1 / (float)value2;
			case "%=":
				return value1 % (float)value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcSubstitionSub(float value1, ExpressionToken token, float value2)
		{
			switch (token.name)
			{
			case "+=":
				return value1 + value2;
			case "-=":
				return value1 - value2;
			case "*=":
				return value1 * value2;
			case "/=":
				return value1 / value2;
			case "%=":
				return value1 % value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcSubstitionSub(string value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+=")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcSubstitionSub(string value1, ExpressionToken token, int value2)
		{
			string text = token.name;
			if (text == "+=")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcSubstitionSub(string value1, ExpressionToken token, float value2)
		{
			string text = token.name;
			if (text == "+=")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcSubstitionSub(int value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+=")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcSubstitionSub(float value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+=")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		public static ExpressionToken OperateUnary(ExpressionToken value, ExpressionToken token)
		{
			return new ExpressionToken("", false, TokenType.Number, 0, CalcUnary(value.variable, token));
		}

		private static object CalcUnary(object value, ExpressionToken token)
		{
			switch (token.name)
			{
			case "!":
				if (value is bool)
				{
					return !(bool)value;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			case "+":
				if (value is float)
				{
					return value;
				}
				if (value is int)
				{
					return value;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			case "-":
				if (value is float)
				{
					return 0f - (float)value;
				}
				if (value is int)
				{
					return -(int)value;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		public static ExpressionToken OperateBinary(ExpressionToken value1, ExpressionToken token, ExpressionToken value2)
		{
			return new ExpressionToken("", false, TokenType.Number, 0, CalcBinary(value1.variable, token, value2.variable));
		}

		private static object CalcBinary(object value1, ExpressionToken token, object value2)
		{
			switch (token.name)
			{
			case "*":
			case "/":
			case "%":
			case "+":
			case "-":
			case ">":
			case "<":
			case ">=":
			case "<=":
				return CalcBinaryNumber(value1, token, value2);
			case "==":
			case "!=":
				return CalcBinaryEq(value1, token, value2);
			case "&&":
			case "||":
				return CalcBinaryAndOr(value1, token, value2);
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcBinaryNumber(object value1, ExpressionToken token, object value2)
		{
			if (value1 is int)
			{
				if (value2 is int)
				{
					return CalcBinaryNumberSub((int)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcBinaryNumberSub((int)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcBinaryNumberSub((int)value1, token, (string)value2);
				}
			}
			else if (value1 is float)
			{
				if (value2 is int)
				{
					return CalcBinaryNumberSub((float)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcBinaryNumberSub((float)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcBinaryNumberSub((float)value1, token, (string)value2);
				}
			}
			else if (value1 is string)
			{
				if (value2 is int)
				{
					return CalcBinaryNumberSub((string)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcBinaryNumberSub((string)value1, token, (float)value2);
				}
				if (value2 is string)
				{
					return CalcBinaryNumberSub((string)value1, token, (string)value2);
				}
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(int value1, ExpressionToken token, int value2)
		{
			switch (token.name)
			{
			case "*":
				return value1 * value2;
			case "/":
				return value1 / value2;
			case "%":
				return value1 % value2;
			case "+":
				return value1 + value2;
			case "-":
				return value1 - value2;
			case ">":
				return value1 > value2;
			case "<":
				return value1 < value2;
			case ">=":
				return value1 >= value2;
			case "<=":
				return value1 <= value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcBinaryNumberSub(int value1, ExpressionToken token, float value2)
		{
			switch (token.name)
			{
			case "*":
				return (float)value1 * value2;
			case "/":
				return (float)value1 / value2;
			case "%":
				return (float)value1 % value2;
			case "+":
				return (float)value1 + value2;
			case "-":
				return (float)value1 - value2;
			case ">":
				return (float)value1 > value2;
			case "<":
				return (float)value1 < value2;
			case ">=":
				return (float)value1 >= value2;
			case "<=":
				return (float)value1 <= value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcBinaryNumberSub(float value1, ExpressionToken token, int value2)
		{
			switch (token.name)
			{
			case "*":
				return value1 * (float)value2;
			case "/":
				return value1 / (float)value2;
			case "%":
				return value1 % (float)value2;
			case "+":
				return value1 + (float)value2;
			case "-":
				return value1 - (float)value2;
			case ">":
				return value1 > (float)value2;
			case "<":
				return value1 < (float)value2;
			case ">=":
				return value1 >= (float)value2;
			case "<=":
				return value1 <= (float)value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcBinaryNumberSub(float value1, ExpressionToken token, float value2)
		{
			switch (token.name)
			{
			case "*":
				return value1 * value2;
			case "/":
				return value1 / value2;
			case "%":
				return value1 % value2;
			case "+":
				return value1 + value2;
			case "-":
				return value1 - value2;
			case ">":
				return value1 > value2;
			case "<":
				return value1 < value2;
			case ">=":
				return value1 >= value2;
			case "<=":
				return value1 <= value2;
			default:
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
		}

		private static object CalcBinaryNumberSub(string value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(string value1, ExpressionToken token, float value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(string value1, ExpressionToken token, int value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(string value1, ExpressionToken token, bool value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(float value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(int value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryNumberSub(bool value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (text == "+")
			{
				return value1 + value2;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryEq(object value1, ExpressionToken token, object value2)
		{
			if (value1 is int)
			{
				if (value2 is int)
				{
					return CalcBinaryEqSub((int)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcBinaryEqSub((int)value1, token, (float)value2);
				}
			}
			else if (value1 is float)
			{
				if (value2 is int)
				{
					return CalcBinaryEqSub((float)value1, token, (int)value2);
				}
				if (value2 is float)
				{
					return CalcBinaryEqSub((float)value1, token, (float)value2);
				}
			}
			else if (value1 is bool)
			{
				if (value2 is bool)
				{
					return CalcBinaryEqSub((bool)value1, token, (bool)value2);
				}
			}
			else if (value1 is string && value2 is string)
			{
				return CalcBinaryEqSub((string)value1, token, (string)value2);
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryEqSub(int value1, ExpressionToken token, int value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return value1 != value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 == value2;
		}

		private static object CalcBinaryEqSub(int value1, ExpressionToken token, float value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return (float)value1 != value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return (float)value1 == value2;
		}

		private static object CalcBinaryEqSub(float value1, ExpressionToken token, int value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return value1 != (float)value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 == (float)value2;
		}

		private static object CalcBinaryEqSub(float value1, ExpressionToken token, float value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return value1 != value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 == value2;
		}

		private static object CalcBinaryEqSub(bool value1, ExpressionToken token, bool value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return value1 != value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 == value2;
		}

		private static object CalcBinaryEqSub(string value1, ExpressionToken token, string value2)
		{
			string text = token.name;
			if (!(text == "=="))
			{
				if (text == "!=")
				{
					return value1 != value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 == value2;
		}

		private static object CalcBinaryAndOr(object value1, ExpressionToken token, object value2)
		{
			if (value1 is bool && value2 is bool)
			{
				return CalcBinaryAndOrSub((bool)value1, token, (bool)value2);
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
		}

		private static object CalcBinaryAndOrSub(bool value1, ExpressionToken token, bool value2)
		{
			string text = token.name;
			if (!(text == "&&"))
			{
				if (text == "||")
				{
					return value1 || value2;
				}
				throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ExpressionOperator, token.name));
			}
			return value1 && value2;
		}

		private static bool TryParseFunction(string name, out ExpressionToken token)
		{
			switch (name)
			{
			case "Random":
			case "RandomF":
				token = new ExpressionToken(name, false, TokenType.Function, 0);
				token.numFunctionArg = 2;
				return true;
			case "Ceil":
			case "CeilToInt":
			case "Floor":
			case "FloorToInt":
				token = new ExpressionToken(name, false, TokenType.Function, 0);
				token.numFunctionArg = 1;
				return true;
			default:
				token = null;
				return false;
			}
		}

		public static ExpressionToken OperateFunction(ExpressionToken token, ExpressionToken[] args)
		{
			switch (token.name)
			{
			case "Random":
			{
				int num2 = UnityEngine.Random.Range(ExpressionCast.ToInt(args[0].variable), ExpressionCast.ToInt(args[1].variable) + 1);
				return new ExpressionToken("", false, TokenType.Number, 0, num2);
			}
			case "RandomF":
			{
				float num = UnityEngine.Random.Range(ExpressionCast.ToFloat(args[0].variable), ExpressionCast.ToFloat(args[1].variable));
				return new ExpressionToken("", false, TokenType.Number, 0, num);
			}
			case "Ceil":
				return new ExpressionToken("", false, TokenType.Number, 0, Mathf.Ceil(ExpressionCast.ToFloat(args[0].variable)));
			case "CeilToInt":
				return new ExpressionToken("", false, TokenType.Number, 0, Mathf.CeilToInt(ExpressionCast.ToFloat(args[0].variable)));
			case "Floor":
				return new ExpressionToken("", false, TokenType.Number, 0, Mathf.Floor(ExpressionCast.ToFloat(args[0].variable)));
			case "FloorToInt":
				return new ExpressionToken("", false, TokenType.Number, 0, Mathf.FloorToInt(ExpressionCast.ToFloat(args[0].variable)));
			default:
				throw new Exception("Unkonw Function :" + token.name);
			}
		}
	}
}
