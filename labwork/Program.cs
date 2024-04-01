using labwork;

string site = "../../../site";

DumpHttpServer server = new DumpHttpServer();

await server.RunAsync(site, 8888);

