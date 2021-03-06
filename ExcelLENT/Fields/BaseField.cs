﻿using BBGo.ExcelLENT.Serializer;
using System.Collections.Generic;

namespace BBGo.ExcelLENT.Fields
{
    public abstract class BaseField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BaseField> Children { get; private set; } = new List<BaseField>();

        internal abstract void OnSerialize(ISerializer serializer, Reader reader);
    }
}
