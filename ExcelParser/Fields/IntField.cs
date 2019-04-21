namespace ExcelParser.Fields
{
    public class IntField : BaseField
    {
        public int value;
        protected override void OnParseContent()
        {
            int.TryParse(m_lexer.Forward, out value);
        }
    }
}