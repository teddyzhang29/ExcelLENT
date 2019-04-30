using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class IntField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            serializer.IntField(this, int.Parse(lexer.Lexical));
            lexer.NextLexical();
        }
    }
}