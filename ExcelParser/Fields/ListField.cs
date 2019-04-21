namespace ExcelParser.Fields
{
    public class ListField : BaseField
    {
        //{a;b;c;d;e}
        protected override void OnParseContent()
        {
            ListField field = new ListField();
            m_lexer.Match("{");
            if (Children.Count > 0)
            {
                BaseField child = Children[0];
                child.ParseContent(m_lexer);
                while (m_lexer.Forward == ";")
                {
                    m_lexer.Match(";");
                    child.ParseContent(m_lexer);
                }
            }
            m_lexer.Match("}");
        }
    }
}
