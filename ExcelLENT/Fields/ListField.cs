using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class ListField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            reader.Match('{');
            serializer.BeginList(this);
            if (Children.Count > 0 && reader.Current != '}')
            {
                Children[0].OnSerialize(serializer, reader);
                while (reader.Current == ';')
                {
                    reader.Match(';');
                    Children[0].OnSerialize(serializer, reader);
                }
            }
            reader.Match('}');
            serializer.EndList(this);
        }
    }
}