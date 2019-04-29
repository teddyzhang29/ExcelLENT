using System.Collections.Generic;
using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public abstract class BaseField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseField> Children { get; private set; } = new List<BaseField>();

        internal abstract void OnSerialize(ISerializer serializer, Lexer lexer, ParseParam param);
    }
}
