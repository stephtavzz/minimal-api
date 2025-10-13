using ApiMinimal.Dominio.Entidades.Servicos;
using ApiMinimal.Dominio.Entidades.Servicos.DTO;
using ApiMinimal.Dominio.Interfaces;
using ApiMinimal.Dominio.ModelViews;
using ApiMinimal.Infraestrutura.Db;
using ApiMinimal.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiMinimal.Dominio;
using ApiMinimal.Dominio.Enuns;

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); // Isso habilita a interface no navegador

#endregion

#region Home

app.MapGet("/", () => Results.Json(new Home()));

#endregion

#region Administradores

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});


app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    return Results.Ok(administradorServico.Todos(pagina));
});

app.MapGet("/Administradores/{id}", ([FromRoute] int? id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id ?? 0);

    if (administrador == null)
    {
        return Results.NotFound("Administrador não encontrado");
    }

    return Results.Ok(administrador);


});

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErroDeValidacao();

    if (string.IsNullOrEmpty(administradorDTO.Email))
    {
        validacao.Mensagens.Add("O email não pode ser vazio");
    }

    if (string.IsNullOrEmpty(administradorDTO.Senha))
    {
        validacao.Mensagens.Add("A senha não pode ser vazia");
    }

    if (administradorDTO.Perfil == null)
    {
        validacao.Mensagens.Add("O perfil não pode ser vazio");
    }

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil?.ToString() ?? Perfil.editor.ToString()  // Agora o código está correto
    };

    administradorServico.Incluir(administrador);

    return Results.Created($"/administradores/{administrador.Id}", administrador);
});

#endregion

#region Veiculos

ErroDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{

    
    var validacao = new ErroDeValidacao();

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ser vazio");
        
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("A marca não pode ficar em branco");
        
    }

    if (veiculoDTO.Ano < 1900)
    {
        validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1900");

    }
    
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
    
    }
);

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);


});

app.MapGet("/veiculos/{id}", ([FromRoute] int? id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id ?? 0);

    if (veiculo == null)
    {
        return Results.NotFound("Veículo não encontrado");
    }

    return Results.Ok(veiculo);


});

app.MapPut("/veiculos/{id}", ([FromRoute] int? id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{

    var veiculo = veiculoServico.BuscaPorId(id ?? 0);

    if (veiculo == null)
    {
        return Results.NotFound("Veículo não encontrado");
    }

    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }


    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);


});

app.MapDelete("/veiculos/{id}", ([FromRoute] int? id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id ?? 0);

    if (veiculo == null)
    {
        return Results.NotFound("Veículo não encontrado");
    }

    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
});


#endregion

app.Run();
