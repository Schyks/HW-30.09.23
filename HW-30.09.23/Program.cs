using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HW_30._09._23
{
    internal class Program
    {
        public static void ProcessQueryResult(SqlDataReader reader)
        {
            do
            {
                int line = 0;
                while (reader.Read())
                {
                    line++;
                    string res = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        res += $"{reader[i]}  ";
                    }
                    Console.WriteLine(res);
                }
                Console.WriteLine("\n\n");
            }
            while (reader.NextResult());
            reader?.Close();
        }
        static void Main(string[] args)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT first_name, last_name FROM Student JOIN S_Cards ON Student.id=S_Cards.id_student WHERE S_Cards.date_in IS NULL\r\nUNION\r\nSELECT first_name, last_name FROM Teacher JOIN T_Cards ON Teacher.id=T_Cards.id_teacher WHERE T_Cards.date_in IS NULL", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Завдання 1.\tСписок боржникiв:\n");
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 2.\tАвтор книжки №3:\n");
            cmd.CommandText = "SELECT first_name, last_name FROM Author JOIN Book ON Author.id = Book.id_author WHERE Book.id = 3";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 3.\tСписок доступних книжок :\n");
            cmd.CommandText = "SELECT Book.name FROM Book JOIN (SELECT C.id_book, COUNT(C.id_book) AS CNT from (SELECT * from S_Cards WHERE date_in IS NULL UNION SELECT * from T_Cards WHERE date_in IS NULL)as C GROUP BY id_book) AS Cards\r\nON Book.id=Cards.id_book WHERE Book.quantity>Cards.CNT";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 4.\tКнижки у користувача  №6:\n");
            cmd.CommandText = "SELECT Book.name FROM Student JOIN S_Cards ON Student.id = S_Cards.id_student JOIN Book ON S_Cards.id_book = Book.id WHERE S_Cards.date_in IS NULL AND Student.id = 6\r\nUNION\r\nSELECT Book.name FROM Teacher JOIN T_Cards ON Teacher.id = T_Cards.id_teacher JOIN Book ON T_Cards.id_book = Book.id WHERE T_Cards.date_in IS NULL AND Teacher.id = 6";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 5.\tКнижки, взятi протягом 2 недiль назад:\n");
            cmd.CommandText = "SELECT Book.name FROM Book JOIN (SELECT * from S_Cards WHERE date_out<= DATEADD(WEEK, -2, GETDATE()) UNION SELECT * from T_Cards WHERE date_out<= DATEADD(WEEK, -2, GETDATE())) AS C ON Book.id=C.id_book\r\n";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 6.\tОбнулити всi заборгованостi:\n");
            cmd.CommandText = "UPDATE T_Cards SET date_in = FORMAT(GETDATE(), 'yyyy-MM-dd') WHERE date_in IS NULL\r\n;UPDATE S_Cards SET date_in = FORMAT(GETDATE(), 'yyyy-MM-dd') WHERE date_in IS NULL";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);
            Console.WriteLine("Завдання 7.\tКiлькiсть книг, взята конкретним користувачем за останнiй рiк:\n");
            cmd.CommandText = "SELECT Cards.first_name, Cards.last_name, COUNT(Cards.id_book) AS Cnt\r\nFROM (SELECT Student.first_name, Student.last_name, S_Cards.id_book, S_Cards.date_out FROM S_Cards JOIN Student ON S_Cards.id_student=Student.id WHERE date_out>= DATEADD(YEAR, -1, GETDATE())) AS Cards \r\nGROUP BY  Cards.first_name, Cards.last_name\r\nUNION \r\nSELECT Cards.first_name, Cards.last_name, COUNT(Cards.id_book) AS Cnt\r\nFROM (SELECT Teacher.first_name, Teacher.last_name, T_Cards.id_book, T_Cards.date_out FROM T_Cards JOIN Teacher ON T_Cards.id_teacher=Teacher.id WHERE date_out>= DATEADD(YEAR, -1, GETDATE())) AS Cards \r\nGROUP BY  Cards.first_name, Cards.last_name";
            reader = cmd.ExecuteReader();
            ProcessQueryResult(reader);

            conn?.Close();
        }
    }
}
