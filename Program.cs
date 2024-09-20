using FirebirdSql.Data.FirebirdClient;
using log_food_api;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data.Common;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

try
{
    DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);

    Db.connStrFood = builder.Configuration["ConnectionStrings:FOOD"];

    var mod = builder.Configuration["Modulos:SCH"];

    if (mod != null && mod.GenericConvert<bool>())
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        Db.connStrSCH = builder.Configuration["ConnectionStrings:SCH"];
    }
    
    mod = builder.Configuration["Modulos:SCEF"];

    if (mod != null && mod.GenericConvert<bool>())
    {
        try
        {
            DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdClientFactory.Instance);
            var connectionString = builder.Configuration["ConnectionStrings:SCEF"];
            Db.connStrSCEF = connectionString;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Conexão bem-sucedida!");
        }
        catch (Exception ex)
        {
            LogUtil.InserirEvento("", string.Concat("LogFoodApi - ", ex.Message));
        }
        
    }
}
catch (Exception ex)
{
    LogUtil.InserirEvento("", "Logicom Food Api - Erro ao inicializar Bancos de Dados (factory)." + Environment.NewLine + ex.Message);
}

//lowercase or uppercase
builder.Services.AddControllers().AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//autenticacao
var key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Logicom Food Api",
        Description = "Api do Produto Logicom Comanda",
        TermsOfService = new Uri("http://logicom.com.br/logicom_food_api/termos"),
        Contact = new OpenApiContact
        {
            Name = "Logicom Tecnologia",
            Url = new Uri("http://logicom.com.br")
        },
        /*License = new OpenApiLicense
        {
            Name = "Exemplo de Licen�a",
            Url = new Uri("https://logicom.com.br/licenca")
        }*/
    });

    //options.OperationFilter<SwaggerBodyTypeOperationFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

try
{
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    throw ex;
}
