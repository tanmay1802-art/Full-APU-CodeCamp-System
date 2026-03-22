using System;
using System.Windows.Forms;

namespace APUCodeCamp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Test DB connection before opening Login
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show(
                    "Cannot connect to the database.\n\n" +
                    "Please make sure:\n" +
                    "1. SQL Server is running\n" +
                    "2. Database 'APUCodeCampDB' exists\n" +
                    "3. You ran DatabaseSetup.sql in SSMS first\n" +
                    "4. The connection string in DatabaseHelper.cs matches your SQL Server name\n\n" +
                    "Common server names:\n" +
                    "  .\\SQLEXPRESS\n" +
                    "  localhost\\SQLEXPRESS\n" +
                    "  YOURPC\\SQLEXPRESS",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new frmLogin());
        }
    }
}
