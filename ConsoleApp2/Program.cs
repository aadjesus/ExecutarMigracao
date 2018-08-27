using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using System;
using System.Reflection;


namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dbSetup = new DatabaseSetup(logText => Console.WriteLine(logText));
                dbSetup.UpAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class DatabaseSetup
    {
        private readonly TextWriterAnnouncer announcer;
        private readonly Assembly assembly;
        private readonly RunnerContext migrationContext;
        private readonly MigrationOptions options;
        private readonly MigrationProcessorFactory factory = null;
        public DatabaseSetup(Action<string> LogAction)
        {
            //var arq = @"D:\BgmRodotec\Globus7\BgmRodotec.Escala\src\BgmRodotec.Escala.Infra\Database\BgmRodotec.Escala.Infra.Database\bin\Debug\BgmRodotec.Escala.Infra.Database.dll";
            //assembly = Assembly.Load(arq);
            assembly = Assembly.GetAssembly(typeof(BgmRodotec.Escala.Infra.Database.Migrations.ExtensionMigration));

            announcer = new TextWriterAnnouncer(s => LogAction(s));

            migrationContext = new RunnerContext(announcer)
            {
                PreviewOnly = false,
                Timeout = 300
            };

            options = new MigrationOptions { PreviewOnly = false, Timeout = 300 };
            var x1 = "oracle";
            switch (x1)
            {
                case "oracle":
                    factory = new FluentMigrator.Runner.Processors.Oracle.OracleProcessorFactory();
                    break;
                case "mssql":
                    factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();
                    break;
                case "mysql":
                    factory = new FluentMigrator.Runner.Processors.MySql.MySqlProcessorFactory();
                    break;
            }
        }

        public DatabaseSetup() : this(null)
        {

        }

        public void UpAll()
        {
            ExecutarMigration("Up", true);
        }

        public void DownAll()
        {
            ExecutarMigration("Down", 0);
        }

        public void UpUltima()
        {
            ExecutarMigration("Up", -1);
        }

        public void DownUltima()
        {
            ExecutarMigration("Down", 1);
        }

        private void ExecutarMigration(string metodo, object parametro)
        {
            using (var processor = factory.Create("Data Source=ORC11G;Persist Security Info=True;User ID=EscalaTeste2;Password=EscalaTeste2", announcer, options))
            {
                var runner = new MigrationRunner(assembly, migrationContext, processor);
                int ExecutedMigrations = 0;
                foreach (var migration in runner.MigrationLoader.LoadMigrations())
                {
                    if (!runner.VersionLoader.VersionInfo.HasAppliedMigration(migration.Key))
                    {
                        try
                        {
                            runner.ApplyMigrationUp(
                            migration.Value,
                            true);
                            ExecutedMigrations++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                    }
                }

                if (announcer != null)
                    announcer.Write(ExecutedMigrations + " migrações foram executadas.", false);
            }
        }

        public class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }
            public string ProviderSwitches { get; set; }
        }
    }
}
