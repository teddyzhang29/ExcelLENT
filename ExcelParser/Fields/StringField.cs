namespace ExcelParser.Fields
{
    public class StringField : BaseField
    {
        public string value;
        protected override void OnParseContent()
        {
            value = m_lexer.Forward;
        }
    }
}
