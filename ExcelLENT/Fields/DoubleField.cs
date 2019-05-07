using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class DoubleField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            serializer.DoubleField(this, double.Parse(lexer.Lexical));
            lexer.NextLexical();
        }
    }
}