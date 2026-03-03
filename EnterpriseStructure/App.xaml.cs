using Business.Abstract;
using Business.Services;
using DataAccess;
using DataAccess.Abstract;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Abstract;
using Presentation.DialogService;
using Presentation.Mediators;
using Presentation.ViewModel;
using System.IO;
using System.Windows;

namespace EnterpriseStructure
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            //DataBaseCreation();
            DataBaseMigration();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlite("Data Source=app.db"));

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(GetConnectionString(), 
                    b => b.MigrationsAssembly("Presentation")));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IStructureService, StructureService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IHRService, HRService>();

            services.AddTransient<IDialogService, DialogService>();
            services.AddTransient<IConfirmationMediator, WpfConfirmationMediator>();
            services.AddTransient<IErrorMediator, WpfErrorMediator>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
        }

        private string? GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            return configuration.GetConnectionString("defaultConnection");
        }

        private void DataBaseMigration()
        {
            var dbContext = _serviceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        //private void DataBaseCreation()
        //{
        //    var scope = ServiceProvider.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //    dbContext.Database.EnsureCreated();
        //}
    }
}
