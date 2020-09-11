﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Data
{
    public enum CharacterValidation
    {
        None,
        Integer,
        Decimal,
        Alphanumeric,
        Name,
        numberAndName,
        EmailAddress,
        Custom
    }
    public class CharOperation
    {
        static readonly char[] Separators = { ' ', '.', ',', '\t', '\r', '\n' };
        const string EmailCharacters = "!#$%&'*+-/=?^_`{|}~";
        const string Num = "0123456789";
        public static char Validate(CharacterValidation validat, string text, int pos, char ch)
        {
            if (validat == CharacterValidation.None)
                return ch;
            if (pos > text.Length)
                pos = text.Length;
            if (validat == CharacterValidation.Integer)
            {
                if (ch == '-')
                {
                    if (text == "")
                        return ch;
                    if (text.Length > 0)
                        return (char)0;
                }
                if (ch < '0' | ch > '9')
                    return (char)0;
                return ch;
            }
            else if (validat == CharacterValidation.Decimal)
            {
                if (ch >= '0' && ch <= '9')
                {
                    if (ch == '.')
                        if (text.IndexOf('.') < 0)
                            return ch;
                    return (char)0;
                }
                return ch;
            }
            else if (validat == CharacterValidation.Alphanumeric)
            {
                // All alphanumeric characters
                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.numberAndName)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.Name)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
            }
            else if (validat == CharacterValidation.EmailAddress)
            {

                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
                if (ch == '@' && text.IndexOf('@') == -1) return ch;
                if (EmailCharacters.IndexOf(ch) != -1) return ch;
                if (ch == '.')
                {
                    char lastChar = (text.Length > 0) ? text[UnityEngine.Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                    char nextChar = (text.Length > 0) ? text[UnityEngine.Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
                    if (lastChar != '.' && nextChar != '.')
                        return ch;
                }
            }
            return (char)0;
        }
        public static string Validate(CharacterValidation validat, string text, int count)
        {
            if (count > 0)
                if (count > text.Length)
                    text = text.Substring(0, count);
            switch (validat)
            {
                case CharacterValidation.Integer:
                    return ValidateInteger(text);
                case CharacterValidation.Decimal:
                    return ValidateDecimal(text);
                case CharacterValidation.Alphanumeric:
                    return ValidateAlphanumeric(text);
                case CharacterValidation.EmailAddress:
                    return ValidateEmailAddress(text);
            }
            return text;
        }
        public static string ValidateInteger(string str)
        {
            int len = str.Length;
            for(int i=0;i<str.Length;i++)
            {
                if (str[i] < '0' | str[i] > '9')
                {
                    if (i == 0)
                        if (str[0] == '-')
                            continue;
                    len = i;
                    break;
                }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0,len);
        }
        public static string ValidateDecimal(string str)
        {
            bool point = false;
            int len = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                {
                    if (!point)  
                        if (str[i] == '.')
                        {
                            point = true;
                            continue;
                        }
                    len = i;
                    break;
                }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
        public static string ValidateAlphanumeric(string str)
        {
            int len = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                    if (str[i] < 'a' | str[i] > 'z')
                        if (str[i] < 'A' | str[i] > 'Z')
                        {
                            len = i;
                            break;
                        }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
        public static string ValidateEmailAddress(string str)
        {
            int len = str.Length;
            bool point = false;
            bool at = false;
            char chr= str[0];
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                    if (str[i] < 'a' | str[i] > 'z')
                        if (str[i] < 'A' | str[i] > 'Z')
                        {
                            if (str[i] == '@')
                            {
                                if (i == 0)
                                    break;
                                if (!at)
                                {
                                   if(!point)
                                    {
                                        at = true;
                                        continue;
                                    }
                                }

                            }
                            else if (str[i] == '.')
                            {
                                if (chr == '@')
                                    break;
                                if (!point)
                                {
                                    if(at)
                                    {
                                        point = true;
                                        continue;
                                    }
                                }
                            }
                            len = i;
                            break;
                        }
                chr = str[i];
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
        public static string GetInt(string str, int len)
        {
            if (str == "" | str == null)
                return "0";
            char[] buf = new char[len + 1];
            int num = 0;
            bool frist = false;
            bool real = false;
            int rc = 0;
            for (int i = 0;i < str.Length;i++)
            {
                var c = str[i];
                if (c == '-')
                {
                    if (!frist)
                    {
                        frist = true;
                        buf[num] = '-';
                        num++;
                    }
                    else break;
                }else if(c==' ')
                {

                }else if(c<'0'| c>'9')
                {
                    break;
                }
                else if (c == '0')
                {
                    if (real)
                    {
                        buf[num] = '0';
                        num++;
                        rc++;
                        if (rc >= len)
                            break;
                    }
                }
                else
                {
                    real = true;
                    buf[num] = c;
                    num++;
                    rc++;
                    if (rc >= len)
                        break;
                }
            }
            return new string(buf, 0, num);
        }
        public static string GetInt(string str)
        {
            return GetInt(str,11);
        }
        public static string GetUInt(string str)
        {
            str = GetInt(str,11);
            return str.Replace("-","");
        }
        public static string Getlong(string str)
        {
            return GetInt(str,22);
        }
        public static string GetUlong(string str)
        {
            str = GetInt(str, 22);
            return str.Replace("-", "");
        }
        public static string GetFloat(string str, int len)
        {
            if (str == "" | str == null)
                return "0";
            char[] buf = new char[len + 2];
            int num = 0;
            bool frist = false;
            bool real = false;
            bool dot = false;
            int rc = 0;
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '-')
                {
                    if (!frist)
                    {
                        frist = true;
                        buf[num] = '-';
                        num++;
                    }
                    else break;
                }
                else if (c == ' ')
                {
                }
                else if (c < '0' | c > '9')
                {
                    if (c == '.')
                    {
                        if (dot)
                            break;
                        dot = true;
                        if (real)
                        {
                            buf[num] = '.';
                            num++;
                        }
                        else
                        {
                            buf[num] = '0';
                            num++;
                            buf[num] = '.';
                            num++;
                            rc++;
                            if (rc >= len)
                                break;
                        }
                    }
                    else break;
                }
                else if (c == '0')
                {
                    if (real | dot)
                    {
                        buf[num] = '0';
                        num++;
                        rc++;
                        if (rc >= len)
                            break;
                    }
                }
                else
                {
                    real = true;
                    buf[num] = c;
                    num++;
                    rc++;
                    if (rc >= len)
                        break;
                }
            }
            if (!real)
                return "0";
            if (num > 0)
                if (buf[num - 1] == '.')
                    num--;
            return new string(buf, 0, num);
        }
        public static string GetFloat(string str)
        {
            return GetFloat(str,8);
        }
        public static string GetDouble(string str)
        {
            return GetFloat(str,17);
        }
        public static string GetIntArray(string str,int len)
        {
            StringBuilder sb = new StringBuilder();
             var ss =str.Split(',');
            bool next = false;
            for(int i=0;i<ss.Length;i++)
            {
                if (next)
                    sb.Append(',');
                sb.Append(GetInt(ss[i], len));
                next = true;
            }
            return sb.ToString();
        }
        public static string GetIntArray(string str)
        {
            return GetIntArray(str,11);
        }
        public static string GetLongArray(string str)
        {
            return GetIntArray(str,22);
        }
        public static string GetFloatArray(string str,int len)
        {
            StringBuilder sb = new StringBuilder();
            var ss = str.Split(',');
            bool next = false;
            for (int i = 0; i < ss.Length; i++)
            {
                if (next)
                    sb.Append(',');
                sb.Append(GetFloat(ss[i], len));
                next = true;
            }
            return sb.ToString();
        }
        public static string GetFloatArray(string str)
        {
            return GetFloatArray(str,8);
        }
        public static string GetDoubleArray(string str)
        {
            return GetFloatArray(str, 17);
        }
    }
}