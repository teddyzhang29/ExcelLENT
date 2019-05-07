using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class ListField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            lexer.Match("{");
            serializer.BeginList(this);
            if (Children.Count > 0 && lexer.Lexical != "}")
            {
                Children[0].OnSerialize(serializer, lexer);
                while (lexer.Lexical == ";")
                {
                    lexer.Match(";");
                    Children[0].OnSerialize(serializer, lexer);
                }
            }
            lexer.Match("}");
            serializer.EndList(this);
        }
    }
}