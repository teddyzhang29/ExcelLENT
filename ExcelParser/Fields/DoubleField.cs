namespace ExcelParser.Fields
{
    public class DoubleField : BaseField
    {
        public double value;
        protected override void OnParseContent()
        {
            double.TryParse(m_lexer.Forward, out value);
        }
    }
}
