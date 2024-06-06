using RuneRealm.Entities;

namespace RuneRealm.Network;

public class LoginManager
{
    public static bool Handshake(Player player)
    {
        var serverSessionKey = SessionEncryption.GenerateServerSessionKey();
        player.Session.Fill(2);

        var connectionType = player.Session.Reader.ReadUnsignedByte();
        Console.WriteLine($"ConnectionType: {connectionType}");
        var userHash = player.Session.Reader.ReadUnsignedByte();

        for (var i = 0; i < 8; i++)
            player.Session.Writer.WriteByte(0);

        player.Session.Writer.WriteByte(0); //responseCode 0 - Exchanges session keys, player name, password, etc. 
        player.Session.Writer.WriteQWord(serverSessionKey);
        player.Session.Flush(); /* Send */

        player.Session.Fill(2);
        var connectionStatus = player.Session.Reader.ReadUnsignedByte();
        var loginPacketSize = player.Session.Reader.ReadUnsignedByte();
        player.Session.Fill(loginPacketSize);

        var loginEncryptPacketSize = loginPacketSize - (36 + 1 + 1 + 2);
        var magicNumber = player.Session.Reader.ReadUnsignedByte();
        var revision = player.Session.Reader.ReadSignedWord();
        var clientVersion = player.Session.Reader.ReadUnsignedByte();

        var crcValues = new int[9];
        for (var i = 0; i < crcValues.Length; i++)
            crcValues[i] = player.Session.Reader.ReadDWord();

        var size2 = player.Session.Reader.ReadUnsignedByte();
        var magicNumber2 = player.Session.Reader.ReadUnsignedByte();

        var ISAACSeed = new int[4];
        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] = player.Session.Reader.ReadDWord();

        player.Session.InEncryption = new SessionEncryption(ISAACSeed);

        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] += 50;
        player.Session.OutEncryption = new SessionEncryption(ISAACSeed);
        player.Session.Writer.packetEncryption = player.Session.OutEncryption;

        var UID = player.Session.Reader.ReadDWord();
        player.Username = player.Session.Reader.ReadString();
        player.Password = player.Session.Reader.ReadString();


        player.Session.Writer.WriteByte((byte)ResponseCode.SuccessfulLogin); /* Secondary response code 2 = Login | 5 = Already logged in etc. */
        player.Session.Writer.WriteByte(2); /* Player Status */
        player.Session.Writer.WriteByte(0);
        player.Session.Flush();

        return true;
    }

    enum ResponseCode
    {
        RetryAfter2000msWithFailCount = -1,
        ExchangeKeysAndCredentials = 0,
        RetryAfter2000ms = 1,
        SuccessfulLogin = 2,
        InvalidUsernameOrPassword = 3,
        AccountDisabled = 4,
        AlreadyLoggedIn = 5,
        RuneScapeUpdated = 6,
        WorldFull = 7,
        CannotConnectLoginServerOffline = 8,
        LoginLimitExceeded = 9,
        CannotConnectBadSessionId = 10,
        LoginServerRejectedSession = 11,
        NeedMembersAccount = 12,
        CannotCompleteLoginTryDifferentWorld = 13,
        ServerUpdating = 14,
        UnknownError15 = 15,
        LoginAttemptsExceeded = 16,
        InMembersOnlyArea = 17,
        InvalidLoginServerRequested = 20,
        JustLeftAnotherWorld = 21,
        UnexplainedError
    }
}