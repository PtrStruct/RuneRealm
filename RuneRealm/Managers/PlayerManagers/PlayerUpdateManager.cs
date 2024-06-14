using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Environment;
using RuneRealm.Network;
using RuneRealm.Util;

namespace RuneRealm.Managers;

public class PlayerUpdateManager
{
    private static Player _player;
    private static readonly int _appearanceOffset = 0x100;
    private static readonly int _equipmentOffset = 0x200;
    private static RSStream PlayerUpdateBlock { get; set; }

    public static void Update()
    {
        foreach (var player in World.Players)
        {
            _player = player;
            PlayerUpdateBlock = new RSStream(new byte[5000]);
            UpdateCurrentPlayerMovement();

            if (player.Flags != PlayerUpdateFlags.None)
                UpdatePlayerState(player, PlayerUpdateBlock);

            UpdateLocalPlayers();
            AddPlayersToLocalList();
            Finalize(PlayerUpdateBlock);
            _player.Session.Writer.EndFrameVarSizeWord();
        }
    }


    private static void Finalize(RSStream PlayerFlagUpdateBlock)
    {
        if (PlayerFlagUpdateBlock.CurrentOffset > 0)
        {
            _player.Session.Writer.FinishBitAccess();
            _player.Session.Writer.WriteBytes(PlayerFlagUpdateBlock.Buffer, PlayerFlagUpdateBlock.CurrentOffset,
                0);
        }
        else
        {
            _player.Session.Writer.FinishBitAccess();
        }
    }

    private static void AddPlayersToLocalList()
    {
        foreach (var other in World.Players)
        {
            if (other.Session.Index == _player.Session.Index)
                continue;

            if (!_player.LocalPlayers.Contains(other) && other.Location.IsWithinArea(_player.Location))
            {
                /* In order to render the local player */
                other.Flags |= PlayerUpdateFlags.Appearance;

                AddLocalPlayer(_player.Session.Writer, _player, other);
                UpdatePlayerState(other, PlayerUpdateBlock);
            }
        }

        /* Finished adding local players */
        _player.Session.Writer.WriteBits(11, 2047);
    }

    private static void AddLocalPlayer(RSStream writer, Player player, Player other)
    {
        writer.WriteBits(11, other.Session.Index);
        writer.WriteBits(1, 1); /* Observed */
        writer.WriteBits(1, 1); /* Teleported */

        var delta = Location.Delta(player.Location, other.Location);
        writer.WriteBits(5, delta.Y);
        writer.WriteBits(5, delta.X);

        Console.WriteLine(
            $"Adding PlayerID: {other.Session.Index} To {player.Session.Index}'s LocalPlayerList at DeltaY: {other.Location.Y} - DeltaX: {other.Location.X}");
        player.LocalPlayers.Add(other);
    }

    private static void UpdateLocalPlayers()
    {
        _player.Session.Writer.WriteBits(8, _player.LocalPlayers.Count); // number of players to add

        foreach (var other in _player.LocalPlayers.ToList())
            if (other.Location.IsWithinArea(_player.Location) && !other.NeedsPositionUpdate)
            {
                UpdateLocalPlayerMovement(other, _player.Session.Writer);

                if (other.Flags != PlayerUpdateFlags.None) UpdatePlayerState(other, PlayerUpdateBlock);
            }
            else
            {
                RemovePlayer(other);
            }
    }

    private static void UpdateLocalPlayerMovement(Player player, RSStream writer)
    {
        var pDir = player.MovementHandler.PrimaryDirection;
        var sDir = player.MovementHandler.SecondaryDirection;
        if (pDir != -1)
        {
            writer.WriteBits(1, 1);
            if (sDir != -1)
                WriteRun(writer, pDir, sDir, player.Flags != PlayerUpdateFlags.None);
            else
                WriteWalk(writer, pDir, player.Flags != PlayerUpdateFlags.None);
        }
        else
        {
            if (player.Flags != PlayerUpdateFlags.None)
            {
                writer.WriteBits(1, 1);
                writer.WriteBits(2, 0);
            }
            else
            {
                writer.WriteBits(1, 0);
            }
        }
    }

    private static void RemovePlayer(Player other)
    {
        _player.Session.Writer.WriteBits(1, 0);
        _player.LocalPlayers.Remove(other);
    }

    private static void UpdatePlayerState(Player player, RSStream playerFlagUpdateBlock)
    {
        var mask = player.Flags;

        if ((int)mask >= 0x100)
        {
            mask |= PlayerUpdateFlags.FullMask;
            playerFlagUpdateBlock.WriteByte((byte)((int)mask & 0xFF));
            playerFlagUpdateBlock.WriteByte((byte)((int)mask >> 8));
        }
        else
        {
            playerFlagUpdateBlock.WriteByte((byte)mask);
        }

        if ((mask & PlayerUpdateFlags.Graphics) != 0) AppendGraphics(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.Animation) != 0) AppendAnimation(player, playerFlagUpdateBlock);
        //if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendNPCInteract(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.Appearance) != 0) AppendAppearance(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.FaceDirection) != 0) AppendFacingDirection(player, playerFlagUpdateBlock);
        //if ((mask & PlayerUpdateFlags.SingleHit) != 0) AppendSingleHit(player, playerFlagUpdateBlock);
    }

    private static void AppendFacingDirection(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndianA((int)player.FacingDirection.X);
        playerFlagUpdateBlock.WriteWordBigEndian((int)player.FacingDirection.Y);
    }

    private static void AppendGraphics(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentGfx);
        playerFlagUpdateBlock.WriteDWord(6553600);
    }

    private static void AppendAnimation(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentAnimation);
        playerFlagUpdateBlock.WriteByteC(0); //delay
    }

    // private static void AppendSingleHit(Player player, RSStream playerFlagUpdateBlock)
    // {
    //     playerFlagUpdateBlock.WriteByte((byte)player.MostRecentDamage.FirstAmount); //hitDamage
    //     playerFlagUpdateBlock.WriteByteA((byte)player.MostRecentDamage.FirstDamageType); //hitType
    //     playerFlagUpdateBlock.WriteByteC(player.CurrentHealth); //currentHealth
    //     playerFlagUpdateBlock.WriteByte(player.Skills.GetSkill(SkillId.HITPOINTS).Level); //maxHealth
    // }
    //
    // private static void AppendNPCInteract(Player player, RSStream updatetempBlock)
    // {
    //     if (player.InteractingEntity is Player target)
    //     {
    //         updatetempBlock.WriteWordBigEndian(target.Session.Index);
    //     }
    //     else if (player.InteractingEntity is NPC npc)
    //     {
    //         updatetempBlock.WriteWordBigEndian(npc.Index);
    //     }
    //     else
    //     {
    //         updatetempBlock.WriteWordBigEndian(0x00FFFF);
    //     }
    // }
    //


    private static void AppendAppearance(Player player, RSStream playerFlagUpdateBlock)
    {
        var updateBlockBuffer = new RSStream(new byte[256]);
        updateBlockBuffer.WriteByte(player.Gender);
        updateBlockBuffer.WriteByte(player.HeadIcon); // Skull Icon

        WriteHelmet(updateBlockBuffer, player);
        WriteCape(updateBlockBuffer, player);
        WriteAmulet(updateBlockBuffer, player);
        WriteWeapon(updateBlockBuffer, player);
        WriteBody(updateBlockBuffer, player);
        WriteShield(updateBlockBuffer, player);
        WriteArms(updateBlockBuffer, player);
        WriteLegs(updateBlockBuffer, player);

        WriteHair(updateBlockBuffer, player);
        WriteHands(updateBlockBuffer, player);
        WriteFeet(updateBlockBuffer, player);
        WriteBeard(updateBlockBuffer, player);

        WritePlayerColors(updateBlockBuffer, player);
        WriteMovementAnimations(updateBlockBuffer, player);

        updateBlockBuffer.WriteQWord(player.Username.ToLong());
        updateBlockBuffer.WriteByte(126);
        updateBlockBuffer.WriteWord(100);

        playerFlagUpdateBlock.WriteByteC(updateBlockBuffer.CurrentOffset);
        playerFlagUpdateBlock.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    private static void UpdateCurrentPlayerMovement()
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        _player.Session.Writer.InitBitAccess();
        if (_player.NeedsPositionUpdate)
            WritePositionUpdate(_player);
        else
            WriteMove(_player);
    }

    private static void WritePositionUpdate(Player player)
    {
        player.Session.Writer.WriteBits(1, 1); // set to true if updating thisPlayer
        player.Session.Writer.WriteBits(2, 3); // updateType - 3=jump to pos

        // the following applies to type 3 only
        player.Session.Writer.WriteBits(2, player.Location.Z); // height level (0-3)
        player.Session.Writer.WriteBits(1, 1); // set to true, if discarding walking queue (after teleport e.g.)
        player.Session.Writer.WriteBits(1,
            player.Flags != PlayerUpdateFlags.None ? 1 : 0); // UpdateRequired aka does come with UpdateFlags
        player.Session.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkY); // y-position
        player.Session.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkX); // x-position
    }

    private static void WriteMove(Player player)
    {
        var pDir = player.MovementHandler.PrimaryDirection;
        var sDir = player.MovementHandler.SecondaryDirection;

        if (pDir != -1)
        {
            player.Session.Writer.WriteBits(1, 1);
            if (sDir != -1)
                WriteRun(player.Session.Writer, pDir, sDir, player.Flags != PlayerUpdateFlags.None);
            else
                WriteWalk(player.Session.Writer, pDir, player.Flags != PlayerUpdateFlags.None);
        }
        else
        {
            if (player.Flags != PlayerUpdateFlags.None)
            {
                player.Session.Writer.WriteBits(1, 1);
                WriteUpdateStand(player);
            }
            else
            {
                WriteIdleStand(player);
            }
        }
    }

    private static void WriteRun(RSStream writer, int pDir, int sDir, bool updateRequired)
    {
        writer.WriteBits(2, 2); // 2 - running.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(3, sDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void WriteWalk(RSStream writer, int pDir, bool updateRequired)
    {
        writer.WriteBits(2, 1); // 1 - walking.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void WriteUpdateStand(Player player)
    {
        player.Session.Writer.WriteBits(2, 0);
    }

    private static void WriteIdleStand(Player player)
    {
        player.Session.Writer.WriteBits(1, 0);
    }

    private static void WriteBeard(RSStream stream, Player player)
    {
        var beard = player.AppearanceManager.Beard;

        if (beard != 0) //|| GameConstants.IsFullHelm(player.EquipmentManager.GetItem(EquipmentSlot.Helmet).ItemId) || GameConstants.IsFullMask(player.EquipmentManager.GetItem(EquipmentSlot.Helmet).ItemId)
            stream.WriteWord(_appearanceOffset + (int)beard);
        else
            stream.WriteByte(0);
    }

    private static void WriteFeet(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Boots).ItemId;
        var feetId = player.AppearanceManager.Feet;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteWord(_appearanceOffset + (int)feetId);
    }

    private static void WriteHands(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Gloves).ItemId;
        var handsId = player.AppearanceManager.Hands;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteWord(_appearanceOffset + (int)handsId);
    }

    private static void WriteHair(RSStream stream, Player player)
    {
        var isFullHelmOrMask = GameConstants.IsFullHelm(player.EquipmentManager.GetItem(EquipmentSlot.Helmet).ItemId) ||
                               GameConstants.IsFullMask(player.EquipmentManager.GetItem(EquipmentSlot.Helmet).ItemId);
        if (!isFullHelmOrMask)
        {
            var hair = player.AppearanceManager.Hair;
            stream.WriteWord(_appearanceOffset + (int)hair);
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private static void WriteLegs(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Legs).ItemId;
        var legsId = player.AppearanceManager.Legs;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteWord(_appearanceOffset + (int)legsId);
    }

    private static void WriteShield(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Shield).ItemId;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteBody(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Chest).ItemId;
        var torsoId = player.AppearanceManager.Torso;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteWord(_appearanceOffset + (int)torsoId);
    }

    private static void WriteWeapon(RSStream stream, Player player)
    {
        if (player.EquipmentManager.GetItem(EquipmentSlot.Weapon) == null)
        {
            stream.WriteByte(0);
            return;
        }

        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Weapon).ItemId;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteAmulet(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Amulet).ItemId;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteCape(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Cape).ItemId;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteHelmet(RSStream stream, Player player)
    {
        var itemId = player.EquipmentManager.GetItem(EquipmentSlot.Helmet).ItemId;
        if (itemId > -1)
            stream.WriteWord(_equipmentOffset + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteArms(RSStream stream, Player player)
    {
        var isFullBody = GameConstants.IsFullBody(player.EquipmentManager.GetItem(EquipmentSlot.Chest).ItemId);
        if (!isFullBody)
        {
            var arms = player.AppearanceManager.Arms;
            stream.WriteWord(_appearanceOffset + (int)arms);
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private static void WritePlayerColors(RSStream stream, Player player)
    {
        for (var i = 0; i < 5; i++) stream.WriteByte(player.ColorManager.GetColors()[i]);
    }

    private static void WriteMovementAnimations(RSStream stream, Player player)
    {
        foreach (var animation in player.AnimationManager.GetAnimations())
            stream.WriteWord(animation);
    }
}