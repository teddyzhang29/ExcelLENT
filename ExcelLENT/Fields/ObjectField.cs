using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class ObjectField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            reader.Match('{');
            serializer.BeginObject(this);
            for (int i = 0; i < Children.Count - 1; i++)
            {
                Children[i].OnSerialize(serializer, reader);
                reader.Match(';');
            }
            if (Children.Count > 0)
            {
                Children[Children.Count - 1].OnSerialize(serializer, reader);
            }
            reader.Match('}');
            serializer.EndObject(this);
        }
    }
}