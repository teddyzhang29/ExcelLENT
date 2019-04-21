using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class StringField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer, ParseParam param)
        {
            serializer.StringField(this, lexer.Lexical);
            lexer.NextLexical();
        }
    }
}
