namespace ExcelParser.Fields
{
    public class BoolField : BaseField
    {
        public bool value;
        protected override void OnParseContent()
        {
            bool.TryParse(m_lexer.Forward, out value);
        }
    }
}
