using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramPraca.Data
{
    public class Logger
    {
        public static string LogsPath { get; set; } = "";
        static string _action = "";
       
        public static string DeletedUser { get; set; }
        public static string AddedUser { get; set; }
        public static string AddedColumn { get; set; }
        public static string DeletedColumn { get; set; }
        public static string ColumnOldName { get; set; }
        public static string ColumnNewName { get; set; }

        public static void CreateAction(int type)
        {
            switch (type)
            {
                case 1:
                    _action = $"dodał nową kolumne {AddedColumn}";
                    break;
                case 2:
                    _action = $"usunął kolumne {DeletedColumn}";

                    break;
                case 3:
                    _action = $"dodał nowego użytkownika {AddedUser}";

                    break;
                case 4:
                    _action = $"usunął użytkownika {DeletedUser}";

                    break;
                case 5:
                    _action = $"zmienił nazwę kolumny {ColumnOldName} na {ColumnNewName}";

                    break;
                
            }
            MakeLog();

        }

        public static void MakeLog()
        {
            
            var file = File.AppendText(LogsPath);
            
            file.Write($"{DateTime.Now}: Użytkownik {Main.User["Login"]} {_action} \n" +
                $"----------------------------------------------------------------------------\n");
            _action = "";
            file.Flush();
            file.Close();
        }
    }
}
