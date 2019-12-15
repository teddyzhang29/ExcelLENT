using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class IntField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            serializer.IntField(this, int.Parse(reader.NextContent()));
        }
    }
}