public class Program {
  public static void Main(string[] args) {
    var app = CreateApp(args);
    app.Run();
  }

  public static WebApplication CreateApp(string[] args) {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOpenApi();
    
    builder.Services
      .AddControllers()
      .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
      });

    builder.Services.AddSingleton<IReceiptInMemoryRepository, ReceiptInMemoryRepository>();
    builder.Services.AddSingleton<IReceiptService, ReceiptService>();

    builder.WebHost.UseUrls("http://0.0.0.0:5074");


    var app = builder.Build();

    if (app.Environment.IsDevelopment()) {
      app.MapOpenApi();
    } else {
      app.UseHttpsRedirection();
    }

    app.MapControllers();

    return app;
  }
}
