using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class StringField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            serializer.StringField(this, reader.NextContent());
        }
    }
}