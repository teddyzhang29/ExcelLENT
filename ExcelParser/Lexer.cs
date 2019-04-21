using System;
using System.Text;

namespace ExcelParser
{
    public class Lexer
    {
        public string Forward { get; private set; }

        private string m_fieldType;
        private int m_position;
        private char m_peek;

        public void Init(string fieldType)
        {
            m_position = 0;
            m_fieldType = fieldType;
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

            // 标识符
            if (char.IsLetter(m_peek) || m_peek == '_')
            {
                StringBuilder builder = new StringBuilder();
                do
                {
                    builder.Append(m_peek);
                    m_peek = NextChar();
                } while (char.IsLetter(m_peek) || char.IsDigit(m_peek) || m_peek == '_');
                Forward = builder.ToString();
                return;
            }

            //数字
            if (char.IsDigit(m_peek))
            {
                double num = 0;
                double f = 1;
                bool hasDot = false;
                do
                {
                    if (m_peek == '.')
                    {
                        if (hasDot)
                        {
                            throw new Exception("数字解析错误:多个小数点");
                        }
                        hasDot = true;
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
                Forward = num.ToString();
                return;
            }

            if (m_peek == '\0')
            {
                Forward = null;
                return;
            }

            string str = m_peek.ToString();
            m_peek = ' ';
            Forward = str;
        }

        private char NextChar()
        {
            if (m_position >= m_fieldType.Length)
                return '\0';

            return m_fieldType[m_position++];
        }

        public void Match(string value)
        {
            if (Forward != value)
            {
                throw new Exception($"语法错误:字符不匹配。期望值:`{value}`,当前值:`{Forward}`");
            }
            NextLexical();
        }
    }
}
