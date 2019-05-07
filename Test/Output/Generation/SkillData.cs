namespace Config
{
    public class SkillData
    {
        private static System.Collections.Generic.List<Row> s_rows;
        public static void Deserialize(string serialization)
        {
            s_rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<Row>>(serialization);
        }

        public class Row
        {
            /// <summary>
            /// id
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 技能列表
            /// </summary>
            public System.Collections.Generic.List<int> skills { get; set; }
            /// <summary>
            /// 位置
            /// </summary>
            public (int x, int z) pos { get; set; }
            /// <summary>
            /// 技能
            /// </summary>
            public System.Collections.Generic.List<(int id, float level)> skill2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public System.Collections.Generic.List<bool> success { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public (string name, string address) desc { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public ((string name, int level) player, (int x, int z) pos) nestedObj { get; set; }
        }

    }
}
