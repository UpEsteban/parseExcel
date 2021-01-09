using System.Collections.Generic;

namespace Sedical.DAO
{
    class SedicalDAO
    {
        public class Column
        {
            public string name { get; set; }
            public string type { get; set; }
            public int isPrimaryKey { get; set; }
            public List<object> values { get; set; }
        }

        public string name { get; set; }
        public List<Column> columns { get; set; }
    }
}
