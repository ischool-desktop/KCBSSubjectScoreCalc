using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBSSubjectScoreCalc
{
    [FISCA.UDT.TableName("KCBS.SubjectScoreLog")]
    class KCBSSubjectScoreLog : ActiveRecord
    {
        [FISCA.UDT.Field(Field = "ref_student_id")]
        public string RefStudentID { get; set; }

        [FISCA.UDT.Field(Field = "school_year")]
        public int SchoolYear { get; set; }

        [FISCA.UDT.Field(Field = "semester")]
        public int Semester { get; set; }

        [FISCA.UDT.Field(Field = "subject_name")]
        public string SubjectName { get; set; }

        [FISCA.UDT.Field(Field = "subject_level")]
        public int SubjectLevel { get; set; }

        [FISCA.UDT.Field(Field = "score")]
        public decimal Score { get; set; }

        [FISCA.UDT.Field(Field = "percentage")]
        public decimal Percentage { get; set; }

        [FISCA.UDT.Field(Field = "user")]
        public string User { get; set; }
    }
}
