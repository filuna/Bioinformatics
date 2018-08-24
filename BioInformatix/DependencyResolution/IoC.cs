using BioInformatix.Services;
using MongoBaseRepository;
using ServiceConnector.Classes;
using ServiceConnector.Classes.Ifaces;
using StructureMap;
namespace BioInformatix
{
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.AssemblyContainingType<CazyConnector>();
                                      scan.AssemblyContainingType<MongoRepository>();
                                      scan.AssemblyContainingType<ServiceConnector.AbstractWsClient>();
                                      scan.WithDefaultConventions();
                                    });
                            x.For<ICazyConnector>().Use<CazyConnector>();
                            x.For<IMongoRepository>().Use<MongoRepository>().Ctor<string>("db").Is("BioInformatix");
                          x.For<IMongoMembershipService>().Use<MongoBaseRepository.Services.MongoMembershipService>();
                            x.For<IBioInformatixService>().Use<BioInformatixService>();
                            x.For<IClustalOmegaConnectorClient>().Use<ClustalOmegaConnectorClient>();
                            x.For<ISimplePhylologyConnectorClient>().Use<SimplePhylologyConnectorClient>();
                          x.For<IBlastConnectorClient>().Use<BlastConnectorClient>();
                        });
            return ObjectFactory.Container;
        }
    }
}