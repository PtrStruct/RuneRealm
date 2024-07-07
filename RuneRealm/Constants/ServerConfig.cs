using RuneRealm.Environment;

namespace RuneRealm.Constants;

public class ServerConfig
{
    public static int TICK_RATE = 600;
    public static int PORT = 43594;
    public static int MAX_PLAYERS = 2000;
    public static int BUFFER_SIZE = 4096;
    public static int SERVER_EXP_BONUS = 25;
    public static int PACKET_FETCH_LIMIT = 25;
    public static bool STARTUP = true;
    public static Location SPAWN = new(2815, 3373, 0);
}