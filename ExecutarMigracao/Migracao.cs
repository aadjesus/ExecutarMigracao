using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ExecutarMigracao
{
    internal class Migracao
    {
        private readonly Processo _processo;

        public Migracao(Parametros parametros)
        {
            _processo = new Processo(parametros);
        }

        public const string ARQUIVO_MIGRATE_EXE = "Migrate.exe";

        public bool ExtrairMigrateExe()
        {
            if (File.Exists(ARQUIVO_MIGRATE_EXE))
                return true;

            try
            {
                var arqivoMigrate = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                    .FirstOrDefault(f => f.Contains(ARQUIVO_MIGRATE_EXE));

                var assembly = Assembly.GetExecutingAssembly().GetManifestResourceStream(arqivoMigrate);

                using (var resourceFile = new FileStream(ARQUIVO_MIGRATE_EXE, FileMode.Create, FileAccess.ReadWrite))
                {
                    var b = new byte[assembly.Length + 1];
                    assembly.Read(b, 0, Convert.ToInt32(assembly.Length));
                    resourceFile.Write(b, 0, Convert.ToInt32(b.Length - 1));
                    resourceFile.Flush();
                    resourceFile.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            var retorno = File.Exists(ARQUIVO_MIGRATE_EXE);

            if (!retorno)
                Console.Error.WriteLine(string.Format("Arquivo '{0}' nao encontrado.", ARQUIVO_MIGRATE_EXE));

            return retorno;
        }

        public void Executar()
        {
            _processo.Executar();
            Console.WriteLine("FIM");
        }
    }
}
