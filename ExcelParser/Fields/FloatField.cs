namespace ExcelParser.Fields
{
    public class FloatField : BaseField
    {
        public float value;
        protected override void OnParseContent()
        {
            float.TryParse(m_lexer.Forward, out value);
        }
    }
}
