using System.Text;

namespace BBGo.ExcelLENT.Generator
{
    public class CodeBuilder
    {
        private int indent = 0;
        private StringBuilder m_structBuilder;

        public CodeBuilder()
        {
            m_structBuilder = new StringBuilder();
        }

        public CodeBuilder AddIndent()
        {
            indent++;
            return this;
        }
        public CodeBuilder SubtractIndent()
        {
            indent--;
            return this;
        }

        public CodeBuilder Append(string text, bool ignoreIndent = false)
        {
            for (int i = 0; !ignoreIndent && i < indent; i++)
            {
                m_structBuilder.Append("    ");
            }
            m_structBuilder.Append(text);
            return this;
        }

        public CodeBuilder AppendLine(string text, bool ignoreIndent = false)
        {
            for (int i = 0; !ignoreIndent && i < indent; i++)
            {
                m_structBuilder.Append("    ");
            }
            m_structBuilder.AppendLine(text);
            return this;
        }
        public CodeBuilder AppendLine()
        {
            m_structBuilder.AppendLine();
            return this;
        }

        public override string ToString()
        {
            return m_structBuilder.ToString();
        }
    }
}