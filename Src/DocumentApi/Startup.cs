using System.Collections.Generic;
using Azure.Storage.Blobs.Models;
using DocumentApi.Configuration;
using DocumentApi.Dal;
using DocumentApi.Infrastructure;
using DocumentApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DocumentApi
{
    public class Startup
    {
        private static class ConfigurationKeyNames
        {
            public static class BlobStorage
            {
                public const string ConnectionString = "ConnectionString";
                public const string DocumentContainerName = "DocumentContainerName";
                public const string ListContainerName = "ListContainerName";
            }
        }

        private static class BlobStoreOptionNames 
        {
            public const string List = "lists";
            public const string Document = "documents";
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }        

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            AddBlobStorageOptions(services, BlobStoreOptionNames.List, 
                PublicAccessType.None, ConfigurationKeyNames.BlobStorage.ListContainerName);
            AddBlobStorageOptions(services, BlobStoreOptionNames.Document, 
                PublicAccessType.Blob, ConfigurationKeyNames.BlobStorage.DocumentContainerName);

            services.AddSingleton<IDictionary<string, IBlobStore>>(
                serviceProvider =>
                    {
                        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<BlobStorageConfiguration>>();

                        Dictionary<string, IBlobStore> blobStoreRegistry =
                            new Dictionary<string, IBlobStore>();

                        blobStoreRegistry.Add(BlobStoreOptionNames.List, 
                            new BlobStore(optionsSnapshot.Get(BlobStoreOptionNames.List)));

                        blobStoreRegistry.Add(BlobStoreOptionNames.Document, 
                            new BlobStore(optionsSnapshot.Get(BlobStoreOptionNames.Document)));

                        return blobStoreRegistry;
                    });
          
            services.AddSingleton<IDocumentListRepository, DocumentListRepository>(
                serviceProvider => 
                    {
                        IDictionary<string, IBlobStore> blobStoreRegistry =
                            serviceProvider.GetRequiredService<IDictionary<string, IBlobStore>>();
                        return new DocumentListRepository(blobStoreRegistry[BlobStoreOptionNames.List]);
                    });
            services.AddSingleton<IDocumentRepository, DocumentRepository>(
                serviceProvider =>
                    {
                        IDictionary<string, IBlobStore> blobStoreRegistry = 
                            serviceProvider.GetRequiredService<IDictionary<string, IBlobStore>>();
                        return new DocumentRepository(blobStoreRegistry[BlobStoreOptionNames.Document]);
                    });            
            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddSingleton<IStartupInitializer, StartupInitializer>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddBlobStorageOptions(IServiceCollection services, string name, PublicAccessType publicAccessType, string containerNameKey)
        {
            services.AddOptions<BlobStorageConfiguration>(name)
               .Configure<IConfiguration>(
                   (options, configuration) =>
                       {
                           IConfiguration blobStorageConfiguration = configuration.GetSection(nameof(ConfigurationKeyNames.BlobStorage));

                           options.ConnectionString = blobStorageConfiguration.GetValue<string>(ConfigurationKeyNames.BlobStorage.ConnectionString);
                           options.ContainerName = blobStorageConfiguration.GetValue<string>(containerNameKey);
                           options.PublicAccessType = publicAccessType;
                       });
        }
    }
}
