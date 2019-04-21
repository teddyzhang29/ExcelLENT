using System.Collections.Generic;

namespace ExcelParser.Fields
{
    public abstract class BaseField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseField> Children { get; private set; } = new List<BaseField>();

        protected Lexer m_lexer;

        internal void ParseContent(Lexer lexer)
        {
            m_lexer = lexer;
            OnParseContent();
        }

        protected abstract void OnParseContent();
    }
}
