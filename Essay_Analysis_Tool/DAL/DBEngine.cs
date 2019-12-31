using System.Data.SQLite;
using System.IO;
using System.Security.Principal;

namespace Essay_Analysis_Tool.DAL
{
    public class DBEngine
    {
        private string cs;
        private string loc;
        private string un;

        /// <summary>
        /// Constructs the <see cref="DBEngine"/>
        /// </summary>
        public DBEngine()
        {
            un = WindowsIdentity.GetCurrent().Name;
            loc = @"C:\Users\" + un + @"Documents\Notepad#\storage.db";
            cs = @"URI=file:" + loc;
        }

        private void Initialize()
        {
            if (!File.Exists(loc))
            {
                var con = new SQLiteConnection(cs);
                con.Open();

                var cmd = new SQLiteCommand(con);
                cmd.CommandText = "INSERT INTO tabs(location) VALUES(@location)";
            }
        }
    }
}
