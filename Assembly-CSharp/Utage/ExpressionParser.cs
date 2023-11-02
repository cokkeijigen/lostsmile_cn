using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class ExpressionParser
	{
		private string exp;

		private string errorMsg;

		private List<ExpressionToken> tokens;

		public string Exp => exp;

		public string ErrorMsg => errorMsg;

		private void AddErrorMsg(string msg)
		{
			if (string.IsNullOrEmpty(errorMsg))
			{
				errorMsg = "";
			}
			else
			{
				errorMsg += "\n";
			}
			errorMsg += msg;
		}

		public ExpressionParser(string exp, Func<string, object> callbackGetValue, Func<string, object, bool> callbackCheckSetValue, bool isBoolean)
		{
			Create(exp, callbackGetValue, callbackCheckSetValue, isBoolean);
		}

		public ExpressionParser(string exp, Func<string, object> callbackGetValue, Func<string, object, bool> callbackCheckSetValue)
		{
			Create(exp, callbackGetValue, callbackCheckSetValue, false);
		}

		private void Create(string exp, Func<string, object> callbackGetValue, Func<string, object, bool> callbackCheckSetValue, bool isBoolean)
		{
			this.exp = exp;
			tokens = ToReversePolishNotation(exp);
			if (string.IsNullOrEmpty(ErrorMsg))
			{
				if (isBoolean)
				{
					CalcExpBoolean(callbackGetValue, callbackCheckSetValue);
				}
				else
				{
					CalcExp(callbackGetValue, callbackCheckSetValue);
				}
			}
		}

		public object CalcExp(Func<string, object> callbackGetValue, Func<string, object, bool> callbackSetValue)
		{
			bool flag = false;
			foreach (ExpressionToken token in tokens)
			{
				if (token.Type == ExpressionToken.TokenType.Value)
				{
					object obj = callbackGetValue(token.Name);
					if (obj == null)
					{
						AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.ExpUnknownParameter, token.Name));
						flag = true;
					}
					else
					{
						token.Variable = obj;
					}
				}
			}
			if (!flag)
			{
				return Calc(callbackSetValue);
			}
			return null;
		}

		public bool CalcExpBoolean(Func<string, object> callbackGetValue, Func<string, object, bool> callbackSetValue)
		{
			object obj = CalcExp(callbackGetValue, callbackSetValue);
			if (obj != null && obj.GetType() == typeof(bool))
			{
				return (bool)obj;
			}
			AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.ExpResultNotBool));
			return false;
		}

		private object Calc(Func<string, object, bool> callbackSetValue)
		{
			try
			{
				Stack<ExpressionToken> stack = new Stack<ExpressionToken>();
				foreach (ExpressionToken token in tokens)
				{
					switch (token.Type)
					{
					case ExpressionToken.TokenType.Substitution:
					{
						ExpressionToken value = stack.Pop();
						ExpressionToken value2 = stack.Pop();
						stack.Push(ExpressionToken.OperateSubstition(value2, token, value, callbackSetValue));
						break;
					}
					case ExpressionToken.TokenType.Unary:
						stack.Push(ExpressionToken.OperateUnary(stack.Pop(), token));
						break;
					case ExpressionToken.TokenType.Binary:
					{
						ExpressionToken value = stack.Pop();
						ExpressionToken value2 = stack.Pop();
						stack.Push(ExpressionToken.OperateBinary(value2, token, value));
						break;
					}
					case ExpressionToken.TokenType.Number:
					case ExpressionToken.TokenType.Value:
						stack.Push(token);
						break;
					case ExpressionToken.TokenType.Function:
					{
						int numFunctionArg = token.NumFunctionArg;
						ExpressionToken[] array = new ExpressionToken[numFunctionArg];
						for (int i = 0; i < numFunctionArg; i++)
						{
							array[numFunctionArg - i - 1] = stack.Pop();
						}
						stack.Push(ExpressionToken.OperateFunction(token, array));
						break;
					}
					}
				}
				if (stack.Count != 1)
				{
					throw new Exception(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.ExpIllegal));
				}
				return stack.Peek().Variable;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
				AddErrorMsg(ex.Message);
				return null;
			}
		}

		private List<ExpressionToken> ToReversePolishNotation(string exp)
		{
			List<ExpressionToken> tokenArray = SplitToken(exp);
			if (!CheckTokenCount(tokenArray))
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.ExpIllegal));
			}
			return ToReversePolishNotationSub(tokenArray);
		}

		private static List<ExpressionToken> SplitToken(string exp)
		{
			List<ExpressionToken> list = new List<ExpressionToken>();
			list.Add(ExpressionToken.LpaToken);
			int index = 0;
			string strToken = "";
			while (index < exp.Length)
			{
				char c = exp[index];
				bool flag = false;
				switch (c)
				{
				case '"':
					SkipGroup('"', '"', ref strToken, exp, ref index);
					flag = true;
					list.Add(ExpressionToken.CreateToken(strToken));
					strToken = "";
					break;
				case '[':
					SkipGroup('[', ']', ref strToken, exp, ref index);
					flag = true;
					break;
				}
				if (flag)
				{
					continue;
				}
				if (char.IsWhiteSpace(c))
				{
					if (!string.IsNullOrEmpty(strToken))
					{
						list.Add(ExpressionToken.CreateToken(strToken));
					}
					strToken = "";
					index++;
					continue;
				}
				ExpressionToken expressionToken = ExpressionToken.FindOperator(exp, index);
				if (expressionToken == null)
				{
					strToken += c;
					index++;
					continue;
				}
				if (!string.IsNullOrEmpty(strToken))
				{
					ExpressionToken item = ExpressionToken.CreateToken(strToken);
					list.Add(item);
				}
				bool flag2 = list.Count > 0 && list[list.Count - 1].IsValueType;
				if (!flag2 && expressionToken.Name == "-")
				{
					list.Add(ExpressionToken.UniMinus);
				}
				else if (!flag2 && expressionToken.Name == "+")
				{
					list.Add(ExpressionToken.UniPlus);
				}
				else
				{
					list.Add(expressionToken);
				}
				strToken = "";
				index += expressionToken.Name.Length;
			}
			if (!string.IsNullOrEmpty(strToken))
			{
				list.Add(ExpressionToken.CreateToken(strToken));
			}
			list.Add(ExpressionToken.RpaToken);
			return list;
		}

		private static bool SkipGroup(char begin, char end, ref string strToken, string exp, ref int index)
		{
			strToken += begin;
			index++;
			while (index < exp.Length)
			{
				char c = exp[index];
				if (c != end)
				{
					strToken += c;
				}
				else
				{
					if (strToken[strToken.Length - 1] != '\\')
					{
						strToken += c;
						index++;
						return true;
					}
					strToken = strToken.Remove(strToken.Length - 1) + c;
				}
				index++;
			}
			return false;
		}

		private static bool CheckStringSeparate(char c, string strToken)
		{
			if (strToken.Length > 0 && strToken[0] == '"')
			{
				return false;
			}
			return true;
		}

		private bool CheckTokenCount(List<ExpressionToken> tokenArray)
		{
			int num = 0;
			foreach (ExpressionToken item in tokenArray)
			{
				switch (item.Type)
				{
				case ExpressionToken.TokenType.Binary:
				case ExpressionToken.TokenType.Substitution:
					num--;
					break;
				case ExpressionToken.TokenType.Number:
				case ExpressionToken.TokenType.Value:
					num++;
					break;
				case ExpressionToken.TokenType.Function:
					num += 1 - item.NumFunctionArg;
					break;
				}
			}
			if (num != 1)
			{
				Debug.LogError(num);
			}
			return num == 1;
		}

		private List<ExpressionToken> ToReversePolishNotationSub(List<ExpressionToken> tokens)
		{
			List<ExpressionToken> list = new List<ExpressionToken>();
			Stack<ExpressionToken> stack = new Stack<ExpressionToken>();
			foreach (ExpressionToken token in tokens)
			{
				try
				{
					switch (token.Type)
					{
					case ExpressionToken.TokenType.Lpa:
						stack.Push(token);
						break;
					case ExpressionToken.TokenType.Rpa:
						while (stack.Count != 0)
						{
							ExpressionToken expressionToken3 = stack.Peek();
							if (expressionToken3.Type == ExpressionToken.TokenType.Lpa)
							{
								stack.Pop();
								break;
							}
							list.Add(stack.Pop());
						}
						break;
					case ExpressionToken.TokenType.Unary:
					case ExpressionToken.TokenType.Binary:
					case ExpressionToken.TokenType.Substitution:
					case ExpressionToken.TokenType.Function:
					{
						ExpressionToken expressionToken2 = stack.Peek();
						while (stack.Count != 0 && token.Priority > expressionToken2.Priority && expressionToken2.Type != 0)
						{
							list.Add(expressionToken2);
							stack.Pop();
							expressionToken2 = stack.Peek();
						}
						stack.Push(token);
						break;
					}
					case ExpressionToken.TokenType.Number:
					case ExpressionToken.TokenType.Value:
						list.Add(token);
						break;
					case ExpressionToken.TokenType.Comma:
						while (true)
						{
							ExpressionToken expressionToken = stack.Peek();
							if (expressionToken.Type != 0)
							{
								list.Add(stack.Pop());
								continue;
							}
							break;
						}
						break;
					default:
						AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.UnknownType, token.Type.ToString()));
						break;
					}
				}
				catch (Exception ex)
				{
					AddErrorMsg(ex.ToString());
				}
			}
			return list;
		}
	}
}
