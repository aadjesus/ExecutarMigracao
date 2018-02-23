using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace ExecutarMigracao
{
    internal class Processo
    {
        private readonly Parametros _parametros;
        private readonly bool _create;
        private readonly string _down;


        public Processo(Parametros parametros)
        {
            _parametros = parametros;
            _create = _parametros.Valor == "CREATE";
            _down = _parametros.Valor == "DOWN"
                ? "-t=rollback Down"
                : string.Empty;
        }


        internal void Executar()
        {
            if (!DataBase())
                return;

            var argumentos = string.Format(
                "--provider SqlServer --conn \"{0};\" --a {1} --tag Desenvolvimento {2}",
                _parametros.Connection(_parametros.NomeBanco),
                _parametros.NomeAssembly,
                _down);

            try
            {
                var startInfo = new ProcessStartInfo(Migracao.ARQUIVO_MIGRATE_EXE, argumentos)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };

                using (var process = Process.Start(startInfo))
                {
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        Console.Error.WriteLine(args.Data);
                    };
                    process.OutputDataReceived += (sender, args) =>
                    {
                        Console.WriteLine(args.Data);
                    };
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                throw;
            }
        }

        private bool DataBase()
        {
            if (!DropDataBase())
                return false;

            using (var connection = new SqlConnection(_parametros.Connection(_parametros.NomeBanco)))
            {
                Console.WriteLine("CONSULTANDO DATABASE");
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    return CreateDatabase();
                }
            }
        }

        private bool CreateDatabase()
        {
            return CreateOrDrop("CREATE");
        }

        private bool DropDataBase()
        {
            if (_create)
                CreateOrDrop("DROP");

            return CreateDatabase();
        }

        private bool CreateOrDrop(string comando)
        {
            var drop = comando == "DROP";
            using (var connection = new SqlConnection(_parametros.Connection("Master")))
            {
                if (!drop)
                    Console.WriteLine(comando + " DATABASE");

                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    var textoDrop = drop
                        ? string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", _parametros.NomeBanco)
                        : string.Empty;

                    command.CommandText += string.Format("{0} {1} DATABASE {2}", textoDrop, comando, _parametros.NomeBanco);
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    if (!drop)
                    {
                        Console.Error.WriteLine(string.Format("ERRO: NAO FOI POSSIVEL {0} O DATABASE: {1}", comando, _parametros.NomeBanco));
                        Console.Error.WriteLine(ex);
                    }

                    return drop;
                }
            }
        }

    }
}
