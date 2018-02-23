using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExecutarMigracao
{
    internal class Parametros
    {
        private string _arquivo;
        public Parametros(string[] parametros)
        {
            if (parametros != null)
                Valor = parametros.FirstOrDefault();

            if (!string.IsNullOrEmpty(Valor))
                Valor = Valor.ToUpper();
        }

        public bool Validar()
        {
            var retorno = !string.IsNullOrEmpty(Valor);

            if (retorno)
                ProcurarDiretorio();
            
            if (retorno && string.IsNullOrEmpty(_arquivo))
            {
                MosntrarErro("Arquivo '*.Database.dll' nao encontrado.");
                retorno = false;
            }
            else if (retorno)
                Directory.SetCurrentDirectory(Diretorio);
            else
            {
                MosntrarErro("Parametros invalidos ou nao informados.");

                Console.WriteLine("Incluir no External Tools: ");
                Console.WriteLine("");
                Console.WriteLine("Arguments: Down, Up, Create");
                Console.WriteLine("Initial directory: $(SolutionDir)");
                Console.WriteLine("User Output window: true");
                Console.WriteLine("Close on exit: true");
            }

            return retorno;
        }

        public string NomeProjeto
        {
            get
            {
                var retorno = NomeAssembly
                   .Replace(".Infra.Database.dll", string.Empty)
                   .Split('.')
                   .Last();
                return retorno;
            }
        }

        public string NomeAssembly
        {
            get
            {
                return Path.GetFileName(_arquivo);
            }
        }

        public string Diretorio
        {
            get
            {
                return Path.GetDirectoryName(_arquivo);
            }
        }

        public string NomeBanco
        {
            get
            {
                var nomeBanco = NomeProjeto + "Teste";

                return nomeBanco;
            }
        }

        public string Valor { get; private set; }

        private void MosntrarErro(string erro)
        {
            if (erro != null && erro.Any())
            {
                var msgErro = string.Format("=== {0} ===", erro);
                var traco = new string('-', msgErro.Length);

                Console.Error.WriteLine(traco);
                Console.Error.WriteLine(msgErro);
                Console.Error.WriteLine(traco);
            }
        }

        public string Connection(string nomeBanco)
        {
            return string.Format("Server=localhost;Database={0};Uid=sa;Pwd=bgm123", nomeBanco);
        }

        private void ProcurarDiretorio()
        {
            Console.WriteLine("PROCURANDO ASSEMBLY DATABASE");

            var path = Directory.GetCurrentDirectory();
            _arquivo = Directory.GetFiles(path, "*.Database.dll", SearchOption.AllDirectories)
                .FirstOrDefault(f => f.ToUpper().Contains("BIN"));
        }
    }
}
