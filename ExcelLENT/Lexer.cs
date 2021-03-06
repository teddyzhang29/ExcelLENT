﻿using System;
using System.Text;

namespace BBGo.ExcelLENT
{
    public class Lexer
    {
        public string Lexical { get; private set; }
        public int Position { get; private set; }
        public int ForwardPosition { get; private set; }

        private string m_text;
        private char m_peek;

        public void Init(string fieldType)
        {
            ForwardPosition = -1;
            m_text = fieldType;
            m_peek = ' ';
            NextLexical();
        }

        public void NextLexical()
        {
            // 跳过空白
            while (char.IsWhiteSpace(m_peek))
            {
                m_peek = NextChar();
            }

            Position = ForwardPosition;

            // 标识符
            if (char.IsLetter(m_peek) || m_peek == '_')
            {
                StringBuilder builder = new StringBuilder();
                do
                {
                    builder.Append(m_peek);
                    m_peek = NextChar();
                } while (char.IsLetter(m_peek) || char.IsDigit(m_peek) || m_peek == '_');
                Lexical = builder.ToString();
                return;
            }

            //数字
            if (m_peek == '-' || char.IsDigit(m_peek))
            {
                double num = 0;
                double f = 1;
                bool hasDot = false;
                bool nagtive = false;
                do
                {
                    if (m_peek == '.')
                    {
                        if (hasDot)
                        {
                            throw new Exception("Parse number failed: Multiple dot.");
                        }
                        hasDot = true;
                    }
                    else if (m_peek == '-')
                    {
                        if (nagtive)
                        {
                            throw new Exception("Parse number failed: Multiple nagtive.");
                        }
                        nagtive = true;
                    }
                    else
                    {
                        if (hasDot)
                        {
                            f *= 0.1;
                            num = num + f * (m_peek - 48);
                        }
                        else
                        {
                            num = num * 10 + m_peek - 48;
                        }
                    }
                    m_peek = NextChar();
                } while (char.IsDigit(m_peek) || m_peek == '.');

                if (nagtive)
                    num = -num;

                Lexical = num.ToString();
                return;
            }

            if (m_peek == '\0')
            {
                Lexical = null;
                return;
            }

            string str = m_peek.ToString();
            m_peek = ' ';
            Lexical = str;
        }

        private char NextChar()
        {
            if (ForwardPosition + 1 >= m_text.Length)
                return '\0';

            return m_text[++ForwardPosition];
        }

        public void Match(string value)
        {
            if (Lexical != value)
            {
                throw new Exception($"Syntas Error:Character not match。Expected:`{value}`, but:`{Lexical}`, full text:`{m_text}`");
            }
            NextLexical();
        }
    }
}
