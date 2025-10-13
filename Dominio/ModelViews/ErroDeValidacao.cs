using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMinimal.Dominio.ModelViews
{
    public class ErroDeValidacao
    {
        public List<string> Mensagens { get; set; } = new List<string>();

    }
}