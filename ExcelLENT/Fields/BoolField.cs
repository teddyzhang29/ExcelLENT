using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class BoolField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            serializer.BoolField(this, bool.Parse(lexer.Lexical));
            lexer.NextLexical();
        }
    }
}
