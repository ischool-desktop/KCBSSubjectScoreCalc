using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBSSubjectScoreCalc
{
    [FISCA.UDT.TableName("KCBS.SubjectScoreConfig")]
    class KCBSSubjectScoreConfig : ActiveRecord
    {
        [FISCA.UDT.Field(Field = "subject")]
        public string Subject { get; set; }

        [FISCA.UDT.Field(Field = "level")]
        public int Level { get; set; }

        [FISCA.UDT.Field(Field = "ercentage")]
        public int Percentage { get; set; }
    }
}
