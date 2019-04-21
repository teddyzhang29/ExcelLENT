using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class BoolField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer, ParseParam param)
        {
            serializer.BoolField(this, bool.Parse(lexer.Lexical));
            lexer.NextLexical();
        }
    }
}
