using System.Collections.Generic;
using ExcelParser.Serializer;

namespace ExcelParser.Fields
{
    public abstract class BaseField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseField> Children { get; private set; } = new List<BaseField>();
        public BaseField Parent { get; set; }

        public void AddChild(BaseField child)
        {
            if (child == null)
                return;

            child.Parent = this;
            Children.Add(child);
        }

        internal abstract void OnSerialize(ISerializer serializer, Lexer lexer, ParseParam param);
    }
}
