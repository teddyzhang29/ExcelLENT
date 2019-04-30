using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class FloatField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            serializer.FloatField(this, float.Parse(lexer.Lexical));
            lexer.NextLexical();
        }
    }
}