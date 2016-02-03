using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParentServer
{
    class DataBaseHandler
    {
        public static string sqlQuery;


        public static OleDbConnection ConnectToDb()
        {
            string path = Application.StartupPath + "/ParentDatabase.mdb";//מיקום מסד בפורוייקט
            string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;data source=" + path;
            OleDbConnection conn = new OleDbConnection(connString);
            return conn;
        }

        public static void DoQuery()
        //הפעולה מקבלת שם מסד נתונים ומחרוזת מחיקה/ הוספה/ עדכון
        //ומבצעת את הפקודה על המסד הפיזי
        {
            OleDbConnection conn = ConnectToDb();
            conn.Open();//פתיחת חיבור למסד הנתונים
            OleDbCommand com = new OleDbCommand(sqlQuery, conn);//הגדרת אובייקט commend לטיפול בפעולת sql לעדכון המסד
            com.ExecuteNonQuery();
            com.Dispose();
            conn.Close();//סגירת חיבור
        }

        /// <summary>
        /// To Execute update / insert / delete queries
        ///  הפעולה מקבלת שם קובץ ומשפט לביצוע ומחזירה את מספר השורות שהושפעו מביצוע הפעולה
        /// </summary>
        public static int RowsAffected()//הפעולה מקבלת מסלול מסד נתונים ופקודת עדכון
        //ומבצעת את הפקודה על המסד הפיזי
        {

            OleDbConnection conn = ConnectToDb();
            conn.Open();
            OleDbCommand com = new OleDbCommand(sqlQuery, conn);
            int rowsA = com.ExecuteNonQuery();
            conn.Close();
            return rowsA;
        }

        /// <summary>
        /// הפעולה מקבלת שם קובץ ומשפט לחיפוש ערך - מחזירה אמת אם הערך נמצא ושקר אחרת
        /// </summary>
        public static bool IsExist()//הפעולה מקבלת שם קובץ ומשפט בחירת נתון ומחזירה אמת אם הנתונים קיימים ושקר אחרת
        {

            OleDbConnection conn = ConnectToDb();
            conn.Open();
            OleDbCommand com = new OleDbCommand(sqlQuery, conn);
            OleDbDataReader data = com.ExecuteReader();
            bool found;
            found = (bool)data.Read();// אם יש נתונים לקריאה יושם אמת אחרת שקר - הערך קיים במסד הנתונים
            conn.Close();
            return found;

        }


        public static DataTable ExecuteDataTable()
        {
            OleDbConnection conn = ConnectToDb();
            conn.Open();
            OleDbDataAdapter tableAdapter = new OleDbDataAdapter(sqlQuery, conn);
            DataTable dt = new DataTable();
            tableAdapter.Fill(dt);
            return dt;
        }

        public static void ExecuteNonQuery()
        {
            OleDbConnection conn = ConnectToDb();
            conn.Open();
            OleDbCommand command = new OleDbCommand(sqlQuery, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static int ExecuteScalar()//פונקציה למציאת הID הקטן ביותר במסד והחזרתו
        {
            OleDbConnection conn = ConnectToDb();
            conn.Open();
            OleDbCommand command = new OleDbCommand(sqlQuery, conn);
            int IdMin = int.Parse(command.ExecuteScalar().ToString());
            conn.Close();
            return IdMin;
        }
      
    }
}
