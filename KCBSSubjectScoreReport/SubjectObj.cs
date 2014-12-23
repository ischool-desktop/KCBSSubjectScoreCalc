using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KCBSSubjectScoreReport
{
    class SubjectObj
    {
        public SubjectObj(ScoreRow sRow)
        {
            kcbs_sore = sRow;

        }

        /// <summary>
        /// 原 - 原始成績
        /// </summary>
        public ScoreRow kcbs_sore { get; set; }

        /// <summary>
        /// 原始成績
        /// </summary>
        public SHSchool.Data.SHSubjectScore SubjectScore { get; set; }

        /// <summary>
        /// 學生基本資料
        /// </summary>
        public SHSchool.Data.SHStudentRecord student { get; set; }


    }
}
