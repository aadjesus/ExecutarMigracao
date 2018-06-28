using BgmRodotec.Escala.Infra.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using System;
using System.Reflection;

namespace BgmRodotec.Escala.Infra.Database
{
    public class DatabaseSetup : MigrationProcessorFactory
    {
        private readonly TextWriterAnnouncer announcer;
        private readonly Assembly assembly = Assembly.GetAssembly(typeof(CreateTableEmpresaFilialGaragem));
        private readonly RunnerContext migrationContext;
        private readonly MigrationOptions options;
        private readonly MigrationProcessorFactory factory = null;
        private readonly IMigrationProcessor processor;

        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            throw new NotImplementedException();
        }

        public DatabaseSetup()
        {
            var options = new ProcessorOptions
            {
                PreviewOnly = false,
                Timeout = new TimeSpan(300),
                //ProviderSwitches = RunnerContext.ProviderSwitches
            };

            var generator = new MigrationGeneratorFactory().GetGenerator("");

            var processor = new ConnectionlessProcessor(
                generator, 
                RunnerContext, 
                options);



            //IMigrationGenerator generator, IRunnerContext context, IMigrationProcessorOptions options



            //announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));

            //migrationContext = new RunnerContext(announcer)
            //{
            //    PreviewOnly = false,
            //    Timeout = 300
            //};

            options = new MigrationOptions { PreviewOnly = false, Timeout = 300 };
            //factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();


            //var assembly = Assembly.Load(@"d:\BgmRodotec\Globus7\BgmRodotec.Escala\src\BgmRodotec.Escala.Infra\Database\BgmRodotec.Escala.Infra.Database\bin\Debug\BgmRodotec.Escala.Infra.Database.dll");
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var context = new RunnerContext(announcer)
            {
                Namespace = "BgmRodotec.Escala.Infra.Database.Migrations"
            };

            factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();

            //var options = new ProcessorOptions { PreviewOnly = false };
            processor = factory.Create(
                "Data Source=AAUGUSTO;Initial Catalog=EscalaTeste;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                announcer,
                options);
            var migrator = new MigrationRunner(assembly, context, processor);

            //  migrator.MigrateUp(true);
        }

        public void UpAll()
        {
            ExecutarMigration("Up", true);
        }

        public void DownAll()
        {
            ExecutarMigration("Down", 0);
        }

        private void ExecutarMigration(string metodo, object parametro)
        {
            using (processor)
            {
                var runner = new MigrationRunner(assembly, migrationContext, processor);

                if (parametro is int && (int)parametro != 0)
                    parametro = runner.VersionLoader.VersionInfo.Latest() - (int)parametro;

                MethodInfo methodInfo;
                if (parametro is int)
                {
                    methodInfo = runner.GetType().GetMethod(
                        "Migrate" + metodo,
                        new Type[] { parametro.GetType(), typeof(bool) });

                    methodInfo.Invoke(
                                    runner,
                                    new object[] { parametro, true }); //true = useAutomaticTransactionManagement
                }
                else
                {
                    methodInfo = runner.GetType().GetMethod(
                    "Migrate" + metodo,
                    new Type[] { parametro.GetType() });

                    methodInfo.Invoke(
                                    runner,
                                    new object[] { parametro });
                }

            }
        }

        public class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }
            public string ProviderSwitches { get; set; }

            int? IMigrationProcessorOptions.Timeout => throw new NotImplementedException();
        }
    }
}
