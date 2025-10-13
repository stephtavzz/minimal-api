using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMinimal.Dominio.Entidades.Servicos.DTO;

namespace ApiMinimal.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);

        Administrador? Incluir(Administrador administrador);
        Administrador? BuscarPorId(int id);
        List<Administrador> Todos(int? pagina);
    }
}