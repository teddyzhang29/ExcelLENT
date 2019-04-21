using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class ListField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer, ParseParam param)
        {
            lexer.Match("{");
            serializer.BeginList(this);
            if (Children.Count > 0)
            {
                Children[0].OnSerialize(serializer, lexer, param);
                while (lexer.Lexical == ";")
                {
                    lexer.Match(";");
                    Children[0].OnSerialize(serializer, lexer, param);
                }
            }
            lexer.Match("}");
            serializer.EndList(this);
        }
    }
}
