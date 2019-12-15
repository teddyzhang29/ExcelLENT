using System;
using System.Text;

namespace BBGo.ExcelLENT
{
    public class Reader
    {
        public char Current { get { return m_content[m_position]; } }
        private string m_content;
        private int m_position;

        public void Init(string content)
        {
            m_content = content;
            m_position = 0;
        }

        public string NextContent()
        {
            if (m_content == null)
                return null;

            StringBuilder builder = new StringBuilder();
            while (m_position < m_content.Length)
            {
                char peek = m_content[m_position++];
                if (peek == ';' || peek == '}')
                {
                    m_position--;
                    break;
                }

                builder.Append(peek);
            }

            string content = builder.ToString();
            return content;
        }

        public void Match(char expect)
        {
            if (m_content == null || m_content[m_position] != expect)
            {
                throw new Exception($"Syntas Error:Character not match。Expected:`{expect}`, but:`{m_content[m_position]}`, full text:`{m_content}`");
            }
            m_position++;
        }
    }
}
