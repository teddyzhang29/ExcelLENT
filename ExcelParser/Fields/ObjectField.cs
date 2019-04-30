using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public class ObjectField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Lexer lexer)
        {
            lexer.Match("{");
            serializer.BeginObject(this);
            for (int i = 0; i < Children.Count - 1; i++)
            {
                Children[i].OnSerialize(serializer, lexer);
                lexer.Match(";");
            }
            if (Children.Count > 0)
            {
                Children[Children.Count - 1].OnSerialize(serializer, lexer);
            }
            lexer.Match("}");
            serializer.EndObject(this);
        }
    }
}