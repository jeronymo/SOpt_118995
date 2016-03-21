using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOpt_118995
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<Contexto>(new DropCreateDatabaseAlways<Contexto>());

            using (var db = new Contexto())
            {
                var listaPessoasJuridicas = new List<pessoajuridica>()
                {
                    new pessoajuridica { nome = "Teste PJ", cnpj = "99346683000109"}
                };
                db.pessoajuridica.AddRange(listaPessoasJuridicas);

                var listaPessoasFisicas = new List<pessoafisica>()
                {
                    new pessoafisica { nome = "Teste PF", cpf = "35285181704" }
                };
                db.pessoafisica.AddRange(listaPessoasFisicas);

                db.SaveChanges();

                var lista = db.pessoajuridica
                                               .Select(m => new ResultadoPessoa { IdCliente = m.idPessoa, Nome = m.nome, Tipo = "Pessoa Jurídica", Documento = m.cnpj })
                                               .Concat(db.pessoafisica
                                               .Select(m => new ResultadoPessoa { IdCliente = m.idPessoa, Nome = m.nome, Tipo = "Pessoa Física", Documento = m.cpf }))
                                               .ToList();

                Console.WriteLine(string.Join(",", lista));
            }

            Console.Read();
        }
    }

    public class ResultadoPessoa
    {
        private string _documento;

        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }

        public string Documento
        {
            get { return this._documento; }
            set
            {
                switch (this.Tipo)
                {
                    case "Pessoa Jurídica":

                        this._documento = Convert.ToUInt64(value).ToString(@"00\.000\.000\/0000\-00");
                        break;

                    case "Pessoa Física":

                        this._documento = Convert.ToUInt64(value).ToString(@"000\.000\.000\-00");
                        break;

                    default:
                        this._documento = value;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return String.Format("IdCliente=[{0}] Nome=[{1}] Tipo=[{2}] Documento=[{3}]", IdCliente, Nome, Tipo, Documento);
        }
    }

    public class Contexto : DbContext
    {
        public Contexto()
            : base("Name=Contexto")
        {

        }

        public DbSet<pessoafisica> pessoafisica { get; set; }
        public DbSet<pessoajuridica> pessoajuridica { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }

    #region Entidades idênticas as coladas pelo usuário que perguntou

    [Table("pessoa")]
    public class pessoa
    {
        [Key]
        public int idPessoa { get; set; }

        [Required]
        [StringLength(90)]
        public string nome { get; set; }
    }


    [Table("pessoafisica")]
    public class pessoafisica : pessoa
    {

        [StringLength(11)]
        public string cpf { get; set; }

        [StringLength(20)]
        public string rg { get; set; }

    }


    [Table("pessoajuridica")]
    public class pessoajuridica : pessoa
    {

        [StringLength(15)]
        public string cnpj { get; set; }

        [StringLength(255)]
        public string nomeFantasia { get; set; }

    }

    #endregion
}
