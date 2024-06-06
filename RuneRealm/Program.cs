using RuneRealm.Cache.Items;
using RuneRealm.Data;
using RuneRealm.Data.ObjectsDef;
using RuneRealm.Environment;
using RuneRealm.Network;
using RuneRealm.Util;

var ifs = new IndexedFileSystem("../../../Data/cache", true);
RSBenchmark.Eval(() => { new ObjectDefinitionDecoder(ifs).Run(); }, "Loaded Objects in");
RSBenchmark.Eval(() => { RegionFactory.Load(ifs); }, "Loaded Regions in");
RSBenchmark.Eval(() => { new ItemDefinitionDecoder(ifs).Run(); }, "Decoded Item Definitions in");

RSServer server = new RSServer();
server.Run();