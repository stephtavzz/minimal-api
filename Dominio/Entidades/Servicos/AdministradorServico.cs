using ApiMinimal.Dominio.Entidades.Servicos.DTO;
using ApiMinimal.Dominio.Interfaces;
using ApiMinimal.Infraestrutura.Db;

namespace ApiMinimal.Dominio.Entidades.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;

        public AdministradorServico(DbContexto db)
        {
            _contexto = db;  // Atribuindo o parâmetro correto ao campo privado
        }

        public Administrador? BuscarPorId(int id)
        {

            return _contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
        }
        public Administrador? Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();

            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores
                .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)
                .FirstOrDefault();
            return adm;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();

            int itensPorPagina = 10;

            if (pagina != null)
                query = query.Skip((int)pagina - 1 * itensPorPagina).Take(itensPorPagina);
                return query.ToList();
        }
    }
}
