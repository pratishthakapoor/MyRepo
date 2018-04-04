using Autofac;
using System.Web.Http;
using System.Configuration;
using System.Reflection;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using ProactiveBot.Database;

namespace SimpleEchoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Bot Storage: This is a great spot to register the private state storage for your bot. 
            // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
            // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure

            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                    // Using Azure Table Storage
                    // var store = new TableBotDataStore(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); // requires Microsoft.BotBuilder.Azure Nuget package 

                    // To use CosmosDb or InMemory storage instead of the default table storage, uncomment the corresponding line below
                    //var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 
                     var store = new InMemoryDataStore(); // volatile in-memory store

                    builder.Register(c => store)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();

                });
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Enables creating Data.Activity object for the IMessageActivity object within EntityFrameworkActivityLogger

            /*AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Microsoft.Bot.Connector.IMessageActivity, SqlConnection.Activity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.From.Id))
                .ForMember(dest => dest.questions, opt => opt.MapFrom(src => src.From.Name))
                .ForMember(dest => dest.server_details, opt => opt.MapFrom(src => src.From.Name))
                .ForMember(dest => dest.middleware_details, opt => opt.MapFrom(src => src.From.Properties))
                .ForMember(dest => dest.database_details, opt => opt.MapFrom(src => src.From.Name));
            });

            var new_builder = new ContainerBuilder();
            new_builder.RegisterType<EntityFrameworkActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
            new_builder.Update(Conversation.Container);*/
        }
    }
}
