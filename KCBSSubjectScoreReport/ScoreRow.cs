using K12.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KCBSSubjectScoreReport
{
    class ScoreRow
    {
        public ScoreRow(DataRow row)
        {
            ref_student_id = "" + row["ref_student_id"];
            subject = "" + row["subject_name"];
            subject_level = "" + row["subject_level"];
            user = "" + row["user"];

            school_year = "" + row["school_year"];
            semester = "" + row["semester"];

            originalscore = decimal.Parse("" + row["score"]); //調整成績
            percentage = decimal.Parse("" + row["percentage"]); //調整比例
        }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string ref_student_id { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        public string school_year { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public string semester { get; set; }

        /// <summary>
        /// 科目級別
        /// </summary>
        public string subject_level { get; set; }

        /// <summary>
        /// 科目名稱
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// 原 - 原始成績
        /// </summary>
        public decimal originalscore { get; set; }

        /// <summary>
        /// 調整比例
        /// </summary>
        public decimal percentage { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string user { get; set; }

    }
}
